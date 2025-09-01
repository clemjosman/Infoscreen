import { AzureFunctionsCachesHelper } from "../helpers/azureFunctionsCachesHelper";
import { ConfigCacheHelper } from "../helpers/configCacheHelper";
import { OsService } from "./osService";
import { NodeService } from "./nodeService";
import { CacheConfig, CacheOptions } from "models/cacheConfig";
import { CacheRefreshProcessesConfig } from "models/cacheRefreshProcessesConfig";
import { LoggingService } from "./loggingService";
import { ConfigHelper } from "../helpers/configHelper";
const request = require("request-promise");
const fs = require("fs");
const path = require("path");
var jetpack = require("fs-jetpack");

// Create verifier for Zscaler root CA
const zscalerCertPath = "/usr/share/ca-certificates/local/zscaler.crt";
const certFileExists = fs.existsSync(zscalerCertPath);
const pathToZscalerCerts = certFileExists
  ? path.resolve(zscalerCertPath)
  : undefined;

const CACHE_UPDATE_RETRY_SECONDS = 30;

export class CacheService {
  private nodeId: string = undefined;
  private cacheFolderPaths: string[] = undefined;
  private refreshProcesses: CacheRefreshProcessesConfig[] = [];
  private useZscalerCertificate: boolean = true;
  private didZscalerCertificateOnceWorked: boolean = false;
  private didNoZscalerCertificateOnceWorked: boolean = false;

  /********************************************************************************************************************
     GENERAL INFORMATIONS ABOUT THE LOGIC

     - Each local cache refresh process has a name, a default refresh interval and is assigned to a type of slide
     - The name of the refresh process corresponds to the name in the config under localCache.refreshRates
     - The slide of the refresh process corresponds to the slide name in the config under slides.order
     - The node config refresh process is the 'master' of all other refresh processes because:
     - Other refresh processes only starts after the first config refresh
     - After each config refresh, the list of refresh processes will be checked
     and updated if there are changes in the config
     - It is the only one not linked to a slide name

     **********************************************************************************************************************/

  // 0- Entry point: Start the cache update process
  async initAndStartCachesUpdateAsync() {
    return new Promise<void>(async (resolve, reject) => {
      try {
        // 1- Get general informations about node and environment for all subsequent reauests.
        this.nodeId = NodeService.getNodeId();
        this.cacheFolderPaths = OsService.getCachedFolderPathArray();

        LoggingService.debug(
          CacheService.name,
          "Init",
          `Init for Node: ${this.nodeId}`
        );

        // 2- Set config refresh process (Do not use the UpdateRefreshProcessForCache function, as the config cache is not related to a slide!)
        await this.addCacheRefreshProcessAsync(
          ConfigCacheHelper.CACHE,
          ConfigCacheHelper.ROOT_URL
        );
        resolve();
      } catch (ex) {
        LoggingService.error(
          CacheService.name,
          "Init",
          `An exception occured during initialization of the cache updates, retry in ${CACHE_UPDATE_RETRY_SECONDS} seconds.`,
          ex,
          this.nodeId,
          JSON.stringify(this.cacheFolderPaths)
        );
        setTimeout(
          this.initAndStartCachesUpdateAsync,
          CACHE_UPDATE_RETRY_SECONDS * 1000
        );
        resolve();
      }
    });
  }

  private async addCacheRefreshProcessAsync(
    cache: CacheConfig,
    rootUrl: String,
    index: number = undefined
  ) {
    return new Promise<void>(async (resolve, reject) => {
      try {
        var interval_minutes = this.getCacheRefreshIntervalMinutes(cache);
        LoggingService.debug(
          CacheService.name,
          "addCacheRefreshProcessAsync",
          `Adding cache ${JSON.stringify(
            cache
          )} with rootUrl '${rootUrl}' and index: ${index} every ${interval_minutes} minutes.`
        );
        var refreshInterval = {
          name: cache.name,
          slide: cache.slide,
          interval_minutes: interval_minutes,
          nodeInterval: setInterval(
            this.cacheRefreshProcessAsync.bind(this),
            Math.floor(interval_minutes * 60000),
            cache,
            rootUrl
          ),
        } as CacheRefreshProcessesConfig;

        // Set interval for the next comming requests
        if (index) {
          LoggingService.debug(
            CacheService.name,
            "addCacheRefreshProcessAsync",
            `Pushing process at index: ${index}`
          );
          this.refreshProcesses.splice(index, 0, refreshInterval);
        } else {
          LoggingService.debug(
            CacheService.name,
            "addCacheRefreshProcessAsync",
            `Pushing process at the end of table`
          );
          this.refreshProcesses.push(refreshInterval);
        }

        // Trigger the first request manually
        LoggingService.debug(
          CacheService.name,
          "addCacheRefreshProcessAsync",
          `Running cache refresh process`
        );
        await this.cacheRefreshProcessAsync(cache, rootUrl);

        LoggingService.info(
          CacheService.name,
          "addCacheRefreshProcessAsync",
          cache.name +
            " - Refresh interval created: every " +
            interval_minutes +
            " min"
        );

        resolve();
      } catch (ex) {
        reject(ex);
      }
    });
  }

  private removeCacheRefreshProcess(refreshIntervalIndex: number) {
    var processName = this.refreshProcesses[refreshIntervalIndex].name;
    clearInterval(this.refreshProcesses[refreshIntervalIndex].nodeInterval);
    this.refreshProcesses.splice(refreshIntervalIndex, 1);

    LoggingService.info(
      CacheService.name,
      "removeCacheRefreshProcess",
      processName + " - Refresh interval deleted"
    );
  }

  private async cacheRefreshProcessAsync(cache: CacheConfig, rootUrl: String) {
    return new Promise<void>(async (resolve, reject) => {
      try {
        LoggingService.debug(
          CacheService.name,
          "cacheRefreshProcessAsync",
          `Refreshing cache for ${cache.name}`
        );
        var url = rootUrl + cache.apiEndpoint + "?nodeId=" + this.nodeId;
        try {
          var data = await this.updateCacheAsync(
            url,
            cache.localCacheFileName,
            cache.options
          );
        } catch (ex) {
          LoggingService.error(
            CacheService.name,
            "cacheRefreshProcessAsync",
            `An exception occured while getting data for cache ${
              cache && cache.name
            }.`,
            ex
          );
          resolve();
          return;
        }

        // For node config cache, trigger the update process of intervals for all sources to match the new received config
        if (cache.name === ConfigCacheHelper.CACHE.name) {
          LoggingService.debug(
            CacheService.name,
            "cacheRefreshProcessAsync",
            "Refreshing config cache -> updateRefreshProcessesForAllCachesAsync"
          );
          ConfigHelper.config = data;
          await this.updateRefreshProcessesForAllCachesAsync();
        }

        resolve(data);
      } catch (ex) {
        reject(ex);
      }
    });
  }

  private async updateRefreshProcessesForAllCachesAsync() {
    return new Promise<void>(async (resolve, reject) => {
      try {
        // Config Cache
        LoggingService.debug(
          CacheService.name,
          "updateRefreshProcessesForAllCachesAsync",
          "Updating refresh process for config cache..."
        );
        await this.updateRefreshProcessForCacheAsync(
          ConfigCacheHelper.CACHE,
          ConfigCacheHelper.ROOT_URL
        );

        LoggingService.debug(
          CacheService.name,
          "updateRefreshProcessesForAllCachesAsync",
          "Updating all others refresh process..."
        );
        // Caches from Azure function Infoscreen, avoiding forEach because of async/await
        for (let cache of AzureFunctionsCachesHelper.CACHES) {
          try {
            await this.updateRefreshProcessForCacheAsync(
              cache,
              AzureFunctionsCachesHelper.ROOT_URL
            );
          } catch (ex) {
            LoggingService.error(
              CacheService.name,
              "updateRefreshProcessesForAllCachesAsync",
              `An exception occured when updating refresh process for cache ${cache.name}`,
              ex
            );
          }
        }

        LoggingService.debug(
          CacheService.name,
          "updateRefreshProcessesForAllCachesAsync",
          "Done updating refresh processes"
        );
        resolve();
      } catch (ex) {
        reject(ex);
      }
    });
  }

  private async updateRefreshProcessForCacheAsync(
    cache: CacheConfig,
    rootUrl: String
  ) {
    return new Promise<void>(async (resolve, reject) => {
      try {
        var refreshIntervalIndex = this.refreshProcesses.findIndex(
          (i) => i.name == cache.name
        );

        // Only set cache refresh process when the slide is displayed or for config cache
        if (
          ConfigHelper.config.slides.order.includes(cache.slide) ||
          ConfigCacheHelper.CACHE.name === cache.name
        ) {
          // If in the slide list and interval is not set: set interval and add it to the array
          if (refreshIntervalIndex == -1) {
            LoggingService.debug(
              CacheService.name,
              "updateRefreshProcessForCacheAsync",
              `Cache ${cache.name} is new, adding it...`
            );
            await this.addCacheRefreshProcessAsync(cache, rootUrl);
          } else {
            LoggingService.debug(
              CacheService.name,
              "updateRefreshProcessForCacheAsync",
              `Cache ${cache.name} already exist.`
            );
            if (
              this.refreshProcesses[refreshIntervalIndex].interval_minutes !==
              this.getCacheRefreshIntervalMinutes(cache)
            ) {
              LoggingService.debug(
                CacheService.name,
                "updateRefreshProcessForCacheAsync",
                `...but refresh interval doesn't match anymore. Removing it and adding it again...`
              );
              // Removing refresh interval from array and putting it back again with correct time interval
              // Using the same index is important so that other threads still points to the right interval index
              this.removeCacheRefreshProcess(refreshIntervalIndex);
              await this.addCacheRefreshProcessAsync(
                cache,
                rootUrl,
                refreshIntervalIndex
              );
            }
          }
        } else {
          LoggingService.debug(
            CacheService.name,
            "updateRefreshProcessForCacheAsync",
            `Cache ${cache.name} not needed.`
          );

          // If not in slide list but interval is set and not needed by other slide: clear interval and remove it from the array
          if (refreshIntervalIndex >= 0) {
            let displayedSlidesSharingSameCache =
              AzureFunctionsCachesHelper.CACHES.filter(
                (c) =>
                  c.name === cache.name &&
                  ConfigHelper.config.slides.order.includes(c.slide)
              );
            let cacheNeededByOtherDisplayedSlide =
              displayedSlidesSharingSameCache.filter(
                (c) => c.slide !== cache.slide
              ).length > 0;

            if (!cacheNeededByOtherDisplayedSlide) {
              LoggingService.debug(
                CacheService.name,
                "updateRefreshProcessForCacheAsync",
                `RefreshInterval is set but also not needed by any other displayed slide, deleting it...`
              );
              this.removeCacheRefreshProcess(refreshIntervalIndex);
            } else {
              LoggingService.debug(
                CacheService.name,
                "updateRefreshProcessForCacheAsync",
                `RefreshInterval is set but needed by an other displayed slide, keeping it.`
              );
            }
          }
        }

        resolve();
      } catch (ex) {
        reject(ex);
      }
    });
  }

  // Get the configured refresh interval or the default one if not specified in the config
  private getCacheRefreshIntervalMinutes(cache: CacheConfig) {
    var timeInterval_minutes = undefined;
    if (
      ConfigHelper.config &&
      ConfigHelper.config.localCache &&
      ConfigHelper.config.localCache.refreshRates &&
      ConfigHelper.config.localCache.refreshRates[cache.name]
    ) {
      timeInterval_minutes =
        ConfigHelper.config.localCache.refreshRates[cache.name];
    } else {
      timeInterval_minutes = cache.defaultRefreshInterval;
    }
    return timeInterval_minutes;
  }

  // Function to make get https calls and update local cache with received data
  private async updateCacheAsync(
    url,
    cachedFileName,
    cacheOptions: CacheOptions = null
  ): Promise<any> {
    return new Promise((resolve, reject) => {
      try {
        const options = {
          uri: url,
          ca:
            pathToZscalerCerts && this.useZscalerCertificate
              ? fs.readFileSync(pathToZscalerCerts, "utf8")
              : undefined,
          strictSSL: true,
        };
        LoggingService.debug(
          CacheService.name,
          "updateCacheAsync",
          "Options for request",
          JSON.stringify(options)
        );

        request(options)
          .then((response) => {
            if (response == "") {
              LoggingService.info(
                CacheService.name,
                "updateCacheAsync",
                "No data received from cache update request",
                url,
                cachedFileName,
                JSON.stringify(cacheOptions)
              );
              reject("No data received");
            }

            // Storing which ca certificate config worked
            if (
              !this.didZscalerCertificateOnceWorked &&
              !this.didNoZscalerCertificateOnceWorked
            ) {
              if (this.useZscalerCertificate) {
                this.didZscalerCertificateOnceWorked = true;
              } else {
                this.didNoZscalerCertificateOnceWorked = true;
              }
            }

            if (cacheOptions) {
              // Adding mSB maintenance mode to config
              if (cacheOptions.isNodeConfig) {
                LoggingService.debug(
                  CacheService.name,
                  "updateCacheAsync",
                  "Received config data, enhancing them with mSB data..."
                );
                var configObject = JSON.parse(response);
                configObject = {
                  nodeId: NodeService.getNodeId(),
                  maintenanceMode: NodeService.getNodeMaintenanceMode(),
                  maintenanceEndDate: NodeService.getNodeMaintenanceEndDate(),
                  azureFunctionEndpoint: NodeService.getAzureFunctionEndpoint(),
                  nodeTags: NodeService.getNodeTags(),
                  ...configObject,
                };
                LoggingService.debug(
                  CacheService.name,
                  "updateCacheAsync",
                  "Successfully enhanced config data."
                );
                response = JSON.stringify(configObject);
              }

              if (cacheOptions.randomizeSociabble) {
                LoggingService.debug(
                  CacheService.name,
                  "updateCacheAsync",
                  "Received Sociabble data, merge & shuffle data..."
                );
                var dataObject = JSON.parse(response);
                var merged = [];
                try {
                  dataObject.forEach((category, index) => {
                    merged = merged.concat(category);
                  });
                  merged = this.randomizeArray(merged);
                } catch (ex) {
                  LoggingService.error(
                    CacheService.name,
                    "updateCacheAsync",
                    "An error occured while merge & shuffle the Sociabble news",
                    ex,
                    null,
                    null,
                    null,
                    JSON.stringify(dataArray)
                  );
                }
                response = JSON.stringify(merged);
                LoggingService.debug(
                  CacheService.name,
                  "updateCacheAsync",
                  "Successfully merged and shuffle Sociabble data."
                );
              }

              // Randomizing news from TwentyMinutes
              if (cacheOptions.randomizeTwentyMinNews) {
                LoggingService.debug(
                  CacheService.name,
                  "updateCacheAsync",
                  "Received 20Min data, shuffling data..."
                );
                var dataObject = JSON.parse(response);
                try {
                  dataObject.forEach((channel, index) => {
                    var news = channel.news;
                    news = this.randomizeArray(news);
                    dataObject[index].news = news;
                  });
                } catch (ex) {
                  LoggingService.error(
                    CacheService.name,
                    "updateCacheAsync",
                    "An error occured while shuffling the twentyMin news",
                    ex,
                    null,
                    null,
                    null,
                    JSON.stringify(dataArray)
                  );
                }

                response = JSON.stringify(dataObject);
                LoggingService.debug(
                  CacheService.name,
                  "updateCacheAsync",
                  "Successfully shuffled 20Min data."
                );
              }

              // Randomizing array of data
              if (cacheOptions.randomizeArray) {
                LoggingService.debug(
                  CacheService.name,
                  "updateCacheAsync",
                  "Received array data, shuffling data..."
                );
                var dataArray = JSON.parse(response);
                try {
                  dataArray = this.randomizeArray(dataArray);
                } catch (ex) {
                  LoggingService.error(
                    CacheService.name,
                    "updateCacheAsync",
                    "An error occured while shuffling the array",
                    ex,
                    null,
                    null,
                    null,
                    JSON.stringify(dataArray)
                  );
                }
                response = JSON.stringify(dataArray);
                LoggingService.debug(
                  CacheService.name,
                  "updateCacheAsync",
                  "Successfully shuffled array data."
                );
              }
            }

            LoggingService.debug(
              CacheService.name,
              "updateCacheAsync",
              "Writing data to cache folder paths..."
            );
            // May have multiple paths
            for (var cachedFolderPath of this.cacheFolderPaths) {
              var cachedFilePath = cachedFolderPath + cachedFileName;
              jetpack.write(cachedFilePath, response);
            }

            LoggingService.debug(
              CacheService.name,
              "updateCacheAsync",
              "Returning data"
            );
            return resolve(JSON.parse(response));
          })
          .catch((error) => {
            // If no ca certificate config worked until now, switch the config
            if (
              !this.didZscalerCertificateOnceWorked &&
              !this.didNoZscalerCertificateOnceWorked
            ) {
              this.useZscalerCertificate = !this.useZscalerCertificate;
            }

            LoggingService.error(
              CacheService.name,
              "updateCacheAsync",
              "An error occured while getting cache data",
              error,
              url,
              cachedFileName,
              JSON.stringify(cacheOptions)
            );
            reject(error);
          });
      } catch (error) {
        LoggingService.error(
          CacheService.name,
          "updateCacheAsync",
          "An error occured while updating a cahe",
          error,
          url,
          cachedFileName,
          JSON.stringify(cacheOptions)
        );
        reject(error);
      }
    });
  }

  // Recursive function used to randomize content of an array
  private randomizeArray(data: any[], iteration: number = 0) {
    if (!data || !data.length || data.length <= 1) return data;

    var arr: any[] = data;
    var n = arr.length;
    var tempArr = [];
    for (var i = 0; i < n - 1; i++) {
      // The following line removes one random element from arr and pushes it onto tempArr
      tempArr.push(arr.splice(Math.floor(Math.random() * arr.length), 1)[0]);
    }
    // Push the remaining item onto tempArr
    tempArr.push(arr[0]);
    arr = tempArr;

    if (iteration < 100) {
      return this.randomizeArray(arr, iteration + 1);
    } else {
      return arr;
    }
  }
}
