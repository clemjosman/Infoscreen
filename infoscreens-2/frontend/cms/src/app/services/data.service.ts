import { Injectable } from '@angular/core';

import { ApiService } from '@services/api.service';
import { apiInfoscreen_Light, apiInfoscreen_Status, apiLanguage, apiNews, apiVideo, apiInfoscreenGroup, apiCategory } from '@models/index';
import { NodeConfig } from '../../../../common';

@Injectable()
export class DataService {
    // Data
    private static _languages: apiLanguage[] = undefined;
    private static _categories: apiCategory[] = undefined;
    private static _infoscreens: apiInfoscreen_Light[] = undefined;
    private static _infoscreenGroups: apiInfoscreenGroup[] = undefined;
    private static _infoscreensStatus: apiInfoscreen_Status[] = undefined;
    private static _allNews: apiNews[] = undefined;
    private static _allVideos: apiVideo[] = undefined;

    constructor(private apiService: ApiService) {}

    //****************************************
    //             LANGUAGES
    //****************************************

    public async getLanguagesAsync(forceRefresh: boolean = false): Promise<apiLanguage[]> {
        return new Promise<apiLanguage[]>(async (resolve, reject) => {
            if (DataService._languages == undefined || forceRefresh) {
                try {
                    DataService._languages = await this.apiService.getLanguagesAsync();
                } catch (error) {
                    reject(error);
                }
            }

            resolve(DataService._languages);
        });
    }

    //****************************************
    //              CATEGORIES
    //****************************************

    public async getCategoriesAsync(forceRefresh: boolean = false): Promise<apiCategory[]> {
        return new Promise<apiCategory[]>(async (resolve, reject) => {
            if (DataService._categories == undefined || forceRefresh) {
                try {
                    DataService._categories = await this.apiService.getCategoriesAsync();
                } catch (error) {
                    reject(error);
                }
            }

            resolve(DataService._categories);
        });
    }

    //****************************************
    //              INFOSCREENS
    //****************************************

    public async getInfoscreensAsync(forceRefresh: boolean = false): Promise<apiInfoscreen_Light[]> {
        return new Promise<apiInfoscreen_Light[]>(async (resolve, reject) => {
            if (DataService._infoscreens == undefined || forceRefresh) {
                try {
                    DataService._infoscreens = await this.apiService.getInfoscreensAsync();
                } catch (error) {
                    reject(error);
                }
            }

            resolve(DataService._infoscreens);
        });
    }

    public async getInfoscreenAsync(infoscreenId: number, forceRefresh: boolean = false): Promise<apiInfoscreen_Light> {
        return new Promise<apiInfoscreen_Light>(async (resolve, reject) => {
            let localInfoscreenData = DataService._infoscreens?.find(i => i.id === infoscreenId);
            if (!localInfoscreenData || forceRefresh) {
                try {
                    localInfoscreenData = await this.apiService.getInfoscreenAsync(infoscreenId);
                    this.updateLocalInfoscreen(localInfoscreenData);
                } catch (error) {
                    reject(error);
                }
            }
            resolve(localInfoscreenData);
        });
    }

    public async getInfoscreenConfigAsync(infoscreenId: number): Promise<NodeConfig> {
        return new Promise<NodeConfig>(async (resolve, reject) => {
            try {
                resolve(await this.apiService.getInfoscreenConfigAsync(infoscreenId));
            } catch (error) {
                reject(error);
            }
        });
    }

    public async getInfoscreensStatusAsync(forceRefresh: boolean = false): Promise<apiInfoscreen_Status[]> {
        return new Promise<apiInfoscreen_Status[]>(async (resolve, reject) => {
            if (DataService._infoscreensStatus == undefined || forceRefresh) {
                try {
                    DataService._infoscreensStatus = await this.apiService.getInfoscreensStatusAsync();
                } catch (error) {
                    reject(error);
                }
            }

            resolve(DataService._infoscreensStatus);
        });
    }

    public updateLocalInfoscreen(infoscreen: apiInfoscreen_Light): void {
        if (DataService._infoscreens) {
            let index = DataService._infoscreens.findIndex(i => i.id === infoscreen.id);
            if (index >= 0) {
                DataService._infoscreens[index] = infoscreen;
            } else {
                DataService._infoscreens.push(infoscreen);
            }
        }
    }

    //****************************************
    //           INFOSCREEN GROUPS
    //****************************************

    public async getInfoscreenGroupsAsync(forceRefresh: boolean = false): Promise<apiInfoscreenGroup[]> {
        return new Promise<apiInfoscreenGroup[]>(async (resolve, reject) => {
            if (DataService._infoscreenGroups == undefined || forceRefresh) {
                try {
                    DataService._infoscreenGroups = await this.apiService.getInfoscreenGroupsAsync();
                } catch (error) {
                    reject(error);
                }
            }

            resolve(DataService._infoscreenGroups);
        });
    }

    //****************************************
    //                  NEWS
    //****************************************

    public async getAllNewsAsync(forceRefresh: boolean = false): Promise<apiNews[]> {
        return new Promise<apiNews[]>(async (resolve, reject) => {
            if (DataService._allNews == undefined || forceRefresh) {
                try {
                    DataService._allNews = await this.apiService.getAllNewsAsync();
                } catch (error) {
                    reject(error);
                }
            }

            resolve(DataService._allNews);
        });
    }

    public async getNewsAsync(newsId: number, trySearchLocally: boolean = true, forceLocalRefresh: boolean = false): Promise<apiNews> {
        return new Promise<apiNews>(async (resolve, reject) => {
            let news: apiNews = null;

            if (forceLocalRefresh) {
                try {
                    await this.getAllNewsAsync(true);
                } catch (error) {
                    reject(error);
                }
            }

            if (trySearchLocally && DataService._allNews && DataService._allNews.length > 0) {
                news = DataService._allNews.find(n => n.id == newsId);
            }

            // Get the value from backend if not found locally or local search not wanted
            if (news == null) {
                try {
                    news = await this.apiService.getNewsAsync(newsId);
                } catch (error) {
                    reject(error);
                }
            }

            resolve(news);
        });
    }

    public deleteNewsLocally(newsId: number): apiNews[] {
        DataService._allNews = DataService._allNews.filter(n => n.id !== newsId);
        return DataService._allNews;
    }

    public deleteMultipleNewsLocally(newsIds: number[]): apiNews[] {
        DataService._allNews = DataService._allNews.filter(n => !newsIds.includes(n.id));
        return DataService._allNews;
    }

    //****************************************
    //                VIDEO
    //****************************************

    public async getAllVideosAsync(forceRefresh: boolean = false): Promise<apiVideo[]> {
        return new Promise<apiVideo[]>(async (resolve, reject) => {
            if (DataService._allVideos == undefined || forceRefresh) {
                try {
                    DataService._allVideos = await this.apiService.getAllVideosAsync();
                } catch (error) {
                    reject(error);
                }
            }

            resolve(DataService._allVideos);
        });
    }

    public async getVideoAsync(videoId: number, trySearchLocally: boolean = true, forceLocalRefresh: boolean = false): Promise<apiVideo> {
        return new Promise<apiVideo>(async (resolve, reject) => {
            let video: apiVideo = null;

            if (forceLocalRefresh) {
                try {
                    await this.getAllVideosAsync(true);
                } catch (error) {
                    reject(error);
                }
            }

            if (trySearchLocally && DataService._allVideos && DataService._allVideos.length > 0) {
                video = DataService._allVideos.find(n => n.id == videoId);
            }

            // Get the value from backend if not found locally or local search not wanted
            if (video == null) {
                try {
                    video = await this.apiService.getVideoAsync(videoId);
                } catch (error) {
                    reject(error);
                }
            }

            resolve(video);
        });
    }

    public deleteVideoLocally(videoId: number): apiVideo[] {
        DataService._allVideos = DataService._allVideos.filter(v => v.id !== videoId);
        return DataService._allVideos;
    }

    public deleteMultipleVideosLocally(videoIds: number[]): apiVideo[] {
        DataService._allVideos = DataService._allVideos.filter(v => !videoIds.includes(v.id));
        return DataService._allVideos;
    }
}
