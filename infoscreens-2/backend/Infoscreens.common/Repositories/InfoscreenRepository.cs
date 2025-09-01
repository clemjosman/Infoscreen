using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.Configs;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Common.Repositories
{
    public class InfoscreenRepository : IInfoscreenRepository
    {
        private readonly ILogger<InfoscreenRepository> _logger;
        private readonly IDatabaseRepository _databaseRepository;

        public InfoscreenRepository(ILogger<InfoscreenRepository> logger, IDatabaseRepository databaseRepository)
        {
            logger.LogDebug(new LogItem(1, "InfoscreenRepository() Creating a new instance."));

            _logger = logger;
            _databaseRepository = databaseRepository;

            _logger.LogTrace(new LogItem(2, "InfoscreenRepository() New instance has been created."));
        }

        #region Update

        public async Task<Infoscreen> UpdateInfoscreenMetadataAsync(Infoscreen infoscreen, apiInfoscreen_MetaDataUpdate metaDataUpdate)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "UpdateInfoscreenMetadataAsync() called.")
                {
                    Custom1 = infoscreen != null ? infoscreen.ToString() : "Parameter infoscreen is null.",
                    Custom4 = metaDataUpdate != null ? JsonConvert.SerializeObject(metaDataUpdate) : "Parameter metaDataUpdate is null."
                });

                if (infoscreen == null)
                    throw new ArgumentNullException(nameof(infoscreen));

                if (metaDataUpdate == null)
                    throw new ArgumentNullException(nameof(metaDataUpdate));


                // Update fields
                infoscreen.DisplayName = metaDataUpdate.DisplayName;
                infoscreen.Description = metaDataUpdate.Description;
                infoscreen.SendMailNoContent = metaDataUpdate.SendMailNoContent;
                infoscreen.ContentAdminEmail = metaDataUpdate.ContentAdminEmail;
                infoscreen.DefaultContentLanguageId = metaDataUpdate.DefaultLanguageId;
                infoscreen.DefaultContentLanguage = await _databaseRepository.GetLanguageAsync(metaDataUpdate.DefaultLanguageId);

                // Persist modifications
                await _databaseRepository.UpdateInfoscreenAsync(infoscreen);


                _logger.LogDebug(new LogItem(11, "UpdateInfoscreenMetadataAsync() finished.")
                {
                    Custom1 = infoscreen != null ? infoscreen.ToString() : "Updated infoscreen is null.",
                    Custom4 = metaDataUpdate != null ? JsonConvert.SerializeObject(metaDataUpdate) : "Parameter metaDataUpdate is null."
                });

                // Return modified infoscreen
                return infoscreen;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "UpdateInfoscreenMetadataAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<NodeConfig> UpdateInfoscreenConfigAsync(Infoscreen infoscreen, apiInfoscreen_ConfigUpdate configUpdate)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "UpdateInfoscreenConfigAsync() called.")
                {
                    Custom1 = infoscreen != null ? infoscreen.ToString() : "Parameter infoscreen is null.",
                    Custom4 = configUpdate != null ? JsonConvert.SerializeObject(configUpdate) : "Parameter configUpdate is null."
                });

                if (infoscreen == null)
                    throw new ArgumentNullException(nameof(infoscreen));

                if (configUpdate == null)
                    throw new ArgumentNullException(nameof(configUpdate));

                // Get current config
                var config = await BlobRepository.GetNodeConfigurationAsync(infoscreen.NodeId);

                // Update
                config.Update(configUpdate);

                // Persist modifications
                await BlobRepository.UpdateNodeConfigurationAsync(infoscreen.NodeId, config);

                // Get new config
                config = await BlobRepository.GetNodeConfigurationAsync(infoscreen.NodeId);

                _logger.LogDebug(new LogItem(11, "UpdateInfoscreenConfigAsync() finished.")
                {
                    Custom1 = config != null ? config.ToString() : "Updated config is null.",
                    Custom4 = configUpdate != null ? JsonConvert.SerializeObject(configUpdate) : "Parameter configUpdate is null."
                });

                // Return modified config
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "UpdateInfoscreenConfigAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Update

        #region Management

        public async Task TriggerNeededFirmwareUpdates()
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "TriggerNeededFirmwareUpdates() called."));

                // Get infoscreens
                var infoscreens = await _databaseRepository.GetInfoscreensAsync();

                foreach(var infoscreen in infoscreens.Where(i => !i.IsVirtualInfoscreen()))
                {
                    try
                    {
                        // Getting state and config
                        var deviceTwin = await IotHubRepository.GetDeviceTwinAsync(infoscreen.NodeId);
                        var config = await BlobRepository.GetNodeConfigurationAsync(infoscreen.NodeId);

                        // Ignore if no firmware version is defined
                        if (string.IsNullOrWhiteSpace(config.FirmwareVersion))
                        {
                            _logger.LogWarning(new LogItem(10, $"Infoscreen #{infoscreen.Id} has no desired firmware version configured"));
                            continue;
                        }

                        // Format versions
                        var desiredFirmwareVersion = config.FirmwareVersion.Split(".").Select(v => int.Parse(v)).ToArray();
                        var raucState = deviceTwin.Properties.Reported.RaucState;
                        var partitionState = raucState?.Rootfs0?.State == "booted" ? raucState?.Rootfs0 : raucState?.Rootfs1;
                        var currentFirmwareVersion = partitionState?.FirmwareVersion?.Split(".")?.Select(v => int.Parse(v))?.ToArray() ?? [0,0,0];

                        // Format should be x.y.z -> size of 3
                        if (desiredFirmwareVersion.Length != 3 || currentFirmwareVersion.Length != 3)
                        {
                            _logger.LogWarning(new LogItem(10, $"Infoscreen #{infoscreen.Id} has badly formated versions")
                            {
                                Custom1= $"Desired: {string.Join('.', desiredFirmwareVersion)}",
                                Custom2 = $"Reported: {string.Join('.', currentFirmwareVersion)}"
                            });
                            continue;
                        }

                        // Compare versions and trigger update if needed
                        for( var i = 0; i < 3; i++)
                        {
                            if (desiredFirmwareVersion[i] != currentFirmwareVersion[i]) {

                                // Send firmware update request
                                var url = $"https://msb.actemium.ch/api/organizations/{CommonConfigHelper.InfoscreenOrganizationId}/nodes/{infoscreen.NodeId}/updatefirmware?force=true&platform={CommonConfigHelper.FirmwarePlatformName}&version={string.Join('.', desiredFirmwareVersion)}";
                                using var client = new HttpClient();
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CommonConfigHelper.MSBApiKey);
                                var httpResponse = await client.PutAsync(url, null);

                                if(httpResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    _logger.LogInformation(new LogItem(10, $"Update for infoscreen #{infoscreen.Id} has been triggered")
                                    {
                                        Custom1 = $"Desired: {string.Join('.', desiredFirmwareVersion)}",
                                        Custom2 = $"Reported: {string.Join('.', currentFirmwareVersion)}"
                                    });
                                }
                                else
                                {
                                    _logger.LogWarning(new LogItem(10, $"Update for infoscreen #{infoscreen.Id} failed")
                                    {
                                        Custom1 = $"Desired: {string.Join('.', desiredFirmwareVersion)}",
                                        Custom2 = $"Reported: {string.Join('.', currentFirmwareVersion)}"
                                    });
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(new LogItem(300, ex, "TriggerNeededFirmwareUpdates() has thrown an exception while handling an infoscreen: {0}", ex.Message) {
                            Custom1 = $"Infoscreen #{infoscreen.Id}"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "TriggerNeededFirmwareUpdates() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Management
    }
}
