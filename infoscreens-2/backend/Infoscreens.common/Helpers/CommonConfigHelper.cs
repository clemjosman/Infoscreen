using Infoscreens.Common.Models.EntityFramework.CMS;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using vesact.common.file.Models;
using vesact.common.file.v2.Enumerations;
using vesact.common.message.v2.Models;

namespace Infoscreens.Common.Helpers
{
    public class CommonConfigHelper
    {
        public const string StorageConnectionStringName = "CMS_BLOB_CONNECTION_STRING";

        public static string StorageConnectionString
        {
            get
            {
                var connectionString = Environment.GetEnvironmentVariable(StorageConnectionStringName);
                if (String.IsNullOrWhiteSpace(connectionString))
                {
                    throw new Exception($"No value found for environment variable: {StorageConnectionStringName}");
                }
                return connectionString;
            }
        }

        public const string ActemiumTenantCode = "ch.actemium";

        public const string AxiansTenantCode = "ch.axians";

        public const string EtavisTenantCode = "ch.etavis";

        public static string BlobContainerName
        {
            get
            {
                return "infoscreens";
            }
        }

        public static string NodeConfigDirectoryPath
        {
            get
            {
                return "config/nodes";
            }
        }


        public static string ApiConfigDirectoryPath
        {
            get
            {
                return "config/apis";
            }
        }

        public static string ConfigDirectoryPath
        {
            get
            {
                return "config";
            }
        }

        public static string CacheDirectoryPath
        {
            get
            {
                return "cache/";
            }
        }

        public static string ReleaseDirectoryPath
        {
            get
            {
                return "releases/";
            }
        }

        public static string UptownMenuFile
        {
            get
            {
                return "cache/uptownMenu/menu.json";
            }
        }

        public static string FirmwarePlatformName
        {
            get
            {
                return "intel-x64-nuc-infoscreen";
            }
        }

        public static string InfoscreenOrganizationId
        {
            get
            {
                return "51aabcee-0ba1-4d40-bbcc-caa3f096dede";
            }
        }

        public static string GetReleaseName(string version)
        {
            return $"vesact-infoscreen@{version}.zip";
        }

        public static int TokenMinValidityMinutesLeft
        {
            get
            {
                return 2;
            }
        }

        public static TimeSpan BuildDownloadLinkTimeOfLive
        {
            get
            {
                // One year
                return new TimeSpan(365, 0, 0, 0);
            }
        }

        public static string DefaultUserIso2
        {
            get
            {
                return "en";
            }
        }

        // New tenant to this list must also be added to GetTenantMessageServiceConfig()
        public static IEnumerable<string> MobileAppAllowedTenantCodes
        {
            get
            {
                return new List<string>() { ActemiumTenantCode, AxiansTenantCode, EtavisTenantCode };
            }
        }

        public static bool IsLocalDevelopmentEnvironment
        {
            get
            {
                return string.Compare(GetEnvironment, LocalDevEnvironmentValue) == 0;
            }
        }

        public static bool IsProductionEnvironment
        {
            get
            {
                return string.Compare(GetEnvironment, ProductionEnvironmentValue) == 0;
            }
        }

        public static string MSBApiKey
        {
            get
            {
                return Environment.GetEnvironmentVariable("MSB_API_KEY");
            }
        }

        public static string CMSConnectionString
        {
            get
            {
                return Environment.GetEnvironmentVariable("CMS_CONNECTION_STRING");
            }
        }

        public static string BlobConnectionString
        {
            get
            {
                return Environment.GetEnvironmentVariable("CMS_CONNECTION_STRING");
            }
        }

        public static string AppInsightsKey
        {
            get
            {
                return Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
            }
        }

        public static string GetEnvironment
        {
            get
            {
                return Environment.GetEnvironmentVariable("ENVIRONMENT");
            }
        }
        
        public static string GetCmsTenantId
        {
            get
            {
                return "eabcc976-2da9-4e3c-bd25-2ec540b2765d";
            }
        }

        public static string GetCmsTenantName
        {
            get
            {
                return "actemiumch";
            }
        }

        public static string GetCmsClientId
        {
            get
            {
                return "c0983e8e-d90f-4fb0-bd8f-4a4f6f7efc59";
            }
        }

        public static TimeSpan AttachmentFileSasExpiry_Infoscreens
        {
            get
            {
                return new TimeSpan(7, 0, 0, 0, 0); // 7 Days
            }
        }

        public static TimeSpan AttachmentFileSasExpiry_CMS
        {
            get
            {
                return new TimeSpan(2, 0, 0); // 2 Hours
            }
        }

        public static JsonSerializerSettings JsonCamelCaseSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.None,
                    Culture = CultureInfo.InvariantCulture,
                    DateFormatString = "yyyy-MM-ddTHH:mm:sszzz",
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateParseHandling = DateParseHandling.DateTimeOffset,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
            }
        }

        public static string LocalDevEnvironmentValue
        {
            get
            {
                return "LocalDev";
            }
        }

        public static string ProductionEnvironmentValue
        {
            get
            {
                return "Production";
            }
        }

        public static string LocalDevObjectId
        {
            get
            {
                return Environment.GetEnvironmentVariable("DEV_OBJECT_ID");
            }
        }

        public static DbContextOptions<CMSDbModel> GetCMSDbContextOptions
        {
            get
            {
                var optionsBuilder = new DbContextOptionsBuilder<CMSDbModel>();
                return optionsBuilder
                            .UseSqlServer(CMSConnectionString)
                            .Options;
            }
        }

        public static string B2CBackendAppId
        {
            get
            {
                return "0d431638-7c43-4616-91e4-b588abc95c86";
            }
        }

        // Azure Resource "vesact-ideabox-translator":
        public static string TransationApiKey_Microsoft
        {
            get
            {
                return "d2cf62adc65143d5acdab59e2bebc9ae";
            }
        }

        public static string ObjectIdClaimType
        {
            get
            {
                return "http://schemas.microsoft.com/identity/claims/objectidentifier";
            }
        }

        public static string ObjectIdClaim
        {
            get
            {
                return "oid";
            }
        }

        public static string AzureOpenAiEndpoint
        {
            get
            {
                return Environment.GetEnvironmentVariable("AZURE_OPEN_AI_ENDPOINT");
            }
        }

        public static string AzureOpenAiKey
        {
            get
            {
                return Environment.GetEnvironmentVariable("AZURE_OPEN_AI_KEY");
            }
        }

        // Notifications

        public static TimeSpan NewsNotificationPeriod
        {
            get
            {
                return new TimeSpan(2, 2, 0, 0); // 2 Days and 2 hours
            }
        }

        public static string LanguageSeviceAppCode
        {
            get
            {
                return "INFOSCREENS_CMS";
            }
        }


        // File Service

        public static FileServiceConfig<CMSDbModel> GetFileServiceConfig
        {
            get
            {
                static CMSDbModel newDbContext() { return new CMSDbModel(GetCMSDbContextOptions); }
                var fileServiceDbConfig = new FileServiceConfig_DB<CMSDbModel>(newDbContext);

                var azureBlobStorageConnectionString = CommonConfigHelper.StorageConnectionString;
                var temporaryContainerName = "temporary";
                var publicContainerName = "public";
                var retentionContainerName = "files";
                var fileServiceAzureBlobConfig = new FileServiceConfig_AzureBlob(azureBlobStorageConnectionString, temporaryContainerName, publicContainerName, retentionContainerName);


                var deleteTemporaryFilesInDay = IsProductionEnvironment ? 7 : 0;
                var deleteBlobsInDays = IsProductionEnvironment ? 90 : 0;
                var deleteUnknownFilesInDays = 0;
                var deleteRecordsInDays = IsProductionEnvironment ? 30 : 0;
                var defaultStart = DateTimeOffset.Parse("2020-01-01 00:00:00 +00:00");
                var fileServiceCleanUpConfig = new FileServiceConfig_CleanUp(deleteTemporaryFilesInDay, deleteBlobsInDays, deleteUnknownFilesInDays, deleteRecordsInDays, defaultStart);

                var authorizedFileExtensions = new List<FileExtensions>()
                {
                    FileExtensions.jpeg,
                    FileExtensions.jpg,
                    FileExtensions.png,
                    FileExtensions.gif,
                    FileExtensions.pdf
                };
                int? maxFileSize = null;
                return new FileServiceConfig<CMSDbModel>(fileServiceDbConfig, fileServiceAzureBlobConfig, fileServiceCleanUpConfig, authorizedFileExtensions, maxFileSize);
            }
        }


        // Message Service
        public static string SenderMail
        {
            get
            {
                return "do-not-reply.vesact@vinci-energies.com";
            }
        }

        public static MessageServiceConfig<SMTPConfig, FirebaseConfig_V1, CMSDbModel> GetTenantMessageServiceConfig(Tenant tenant)
        {
            return GetTenantMessageServiceConfig(tenant.Code);
        }

        public static MessageServiceConfig<SMTPConfig, FirebaseConfig_V1, CMSDbModel> GetTenantMessageServiceConfig(string tenantCode)
        {
            static CMSDbModel newDbContext() { return new CMSDbModel(GetCMSDbContextOptions); }
            var messageServiceDbConfig = new MessageServiceConfig_DB<CMSDbModel>(newDbContext);

            var firebasePushConfig = tenantCode switch
            {
                ActemiumTenantCode => GetMyActemiumFirebaseV1Config,
                AxiansTenantCode => GetMyAxiansFirebaseV1Config,
                EtavisTenantCode => GetMyEtavisFirebaseV1Config,
                _ => throw new ArgumentOutOfRangeException(nameof(tenantCode)),
            };

            const string smtpHost = "smtpextapp.vinci-energies.net";
            const string smtpUsername = "messageservice.actemium@smtpappext.net"; // "infoscreens.actemium@smtpappext.net";
            const string smtpPassword = "aTy=UA73R;V2NXSA!G#G"; // "1TK8ap64gOe3h5MZ125!";
            const int smtpPort = 587;

            var smtpConfig = new SMTPConfig(smtpHost, smtpUsername, smtpPassword, smtpPort);

            return new MessageServiceConfig<SMTPConfig, FirebaseConfig_V1, CMSDbModel>(
                messageServiceDbConfig,
                pushProviderConfig: firebasePushConfig,
                emailProviderConfig: smtpConfig
            );
        }

        public static FirebaseConfig_V1 GetMyActemiumFirebaseV1Config
        {
            get
            {
                var type = "service_account";
                var projectId = "vesact-my-actemium";
                var privateKeyId = "aba72f8b6fd82dd4c01e872a3490e1c44889cdd3";
                var privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCptSsq0qoqs9G6\nA8j9FaHwC12CTRQuq+N3s7oNhUGEjnvaAHkkKmQkr1nf3WygGbWbbTtSSVxkqsXx\nvRefE72ZKGXOss0yMWEw+pwIgfcKAZd7zrChwLwQJYTCgkdsvmmca26rAOUPdhHw\nWZ+cH+qPe2Aa/cuHlelbv4kgUF6wV+t25xeUjZlRAESYlXdUvKlmQImQG5G472Q3\nWg+B2B0Nm3m/wVGe74r8C9ENzH/CUkhXVszyu69iQ5ZKvOnksRNW04diGpoPZ8UO\nWtD6oZYXpx4u+moT8TRD6fdlWcBzDs72ZuN4hHxebKrxniwL2AHNcMkFq3O4Cz7n\nRCU5TwHHAgMBAAECggEABPzEBgAm7BZYxYSy9W0bUziiSOAHBdAbYTsw7pDJ+iFN\n1nxJEx0D0Ggu1ftl49adlbHCGor5YiP6qT4LYats2QFs7IQor57v4kabK0zWe77M\nOuE6I6bMYBUaL7UwcCuwYt5tZC0kji3EHTIlFKuDczAHBQQMZTGsp/ubBg4WmkwO\n5U9QoA6SqNKajwJc1MHlni8358zcd0DZmHp22uwLXdo6thQtEWZbQOotORXUX4iP\n0vFZkHiSagasKfuysz903uqRPiSEeGXq408BESA94xnI17LzPd0ttlW7O2Bw8fk3\n0D68G8ST3D0mb0nfJUco4OLn5Uu2hWKmgfHx+HZ9wQKBgQDbAUvCeClzmLB1o8uZ\n+SkMoTZLNBndXpDsLS96pmjEMmHkYDt7FSOgXbQtabEBj/NsK07UA3TtX541f+qP\nktxFX5QseYDzj1afTDoaOKBQJRvlSa7b6EyZ6marJOGeCPXsjFxWKrQuK/vAk48Y\n4FetpBB2aplKlLpwqeqJUNQ1twKBgQDGYAvKKYYUQVCwK7Gx4KnkiNmVaw6x8fyH\nSBkf+I6tZ+flnVg6SHo3zszaxGZDzCpPWEUMGUKnFNggo38OKjNtJugjyUNwJeWl\nMlMsPXHaXwxtEXOSUM3dI59EqUCILKrbTtpjgDs4jkk3ZspudvVGmQ5XXS7lNCxm\nfAbx/BcUcQKBgBnx6Vu8iIydRippoTE4h1hrsNbv8TB8h4azQ7CwDjFQFrDLhcBr\nHLFe6H2Iy+N3fksORmRvlGaasU99O/iOWXIOhKtQKaqxDH4KtcdtdfCItOlQ9Vow\nBwMk9czIrcHfJeNELZw3qujdZRHHaWh1C24SxbCBqG9hd0TpnFNwM/O5AoGBAJYV\nbnYKCm3iqJhQxoTMc9z6iJvaPmj0qtjy/EdK9y9MsI7xByI5WXZFmIlfE/vxSIV1\nQ/AOYHYmeAnhiKv/056eU/1XNRVxJphgRbrv1eagCLev/19QkR/g5ou8/ZzJJo77\nzpB/IM7+H3ix5S/EAnfiLo58apFwcrMq3NpRnmiRAoGAT0vkIGxoNbDEXdSBojdj\nPY/+uv5XbNQPda/D4sknXPEoGhp5CP/Ypy9bW/L4FpnaIus/CGLbOSvXCkiwHuIX\nHPcY+aM/A81XikFyTysN7M6+qsusDVvmibW2u9erC/aGte4cCQ0Kc3AHnbJuo6hc\nhLqJ9mcTQwC6qFqf6lezNbA=\n-----END PRIVATE KEY-----\n";
                var clientEmail = "firebase-adminsdk-wsv0z@vesact-my-actemium.iam.gserviceaccount.com";
                var clientId = "113528136449523980286";
                var authUri = "https://accounts.google.com/o/oauth2/auth";
                var tokenUri = "https://oauth2.googleapis.com/token";
                var authProviderX509CertUrl = "https://www.googleapis.com/oauth2/v1/certs";
                var clientX509CertUrl = "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-wsv0z%40vesact-my-actemium.iam.gserviceaccount.com";
                
                return new FirebaseConfig_V1(
                    type,
                    projectId,
                    privateKeyId,
                    privateKey,
                    clientEmail,
                    clientId,
                    authUri,
                    tokenUri,
                    authProviderX509CertUrl,
                    clientX509CertUrl
                );
            }
        }

        public static FirebaseConfig_V1 GetMyAxiansFirebaseV1Config
        {
            get
            {
                var type = "service_account";
                var projectId = "vesact-my-axians";
                var privateKeyId = "811982daaaf0d0f6ca18309b48dac3967057df9c";
                var privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDizpNreBUDKXxG\nL9rzlYGeohd6oebgWaoNWi4ix3O+NOnOlOsTsQdezABx9aQmk8oJiU2Me0Qg+YOw\nFy3OFjB7FIBD1C7m6SXKxExi8CYn3xoKl9UIGUEDsoGTCaBIE7/s1224c42y47XP\nsX+0XLcUowxJ7zX1P21Ji7TrS8F7oefhRvcOH9VCpT5JkKsZFxS8ankLgW81hYVx\n0hrLR2Pw1WlGm4BmPKmiKSVsf0MqbVIiQiQaf9RbtGAUk53a+bG7Bjv3KVmAgRVO\nq3aOnKqbeTeTK7dbxWKJ3/mCykIyUuDdbaECL3z655jg2hlhwTTptfJWuc6fivSl\nGofhoZrvAgMBAAECggEAG12ofPGWu3RwebmBdh2Gb8EICzRouo4nDghhA7JEHWGe\nCl5+hLg59u6vaRCl9z+iw4bNFBO1czMX+BG75Z9YxykAlu6NoC/mbvJDczadq/bn\n2fpa6mi9pvgNWVPygm/bmvSdWQGcScbe44n7qiP7tSI9M9iIjqhSA+s276MK2+NB\ncuHmysX+86ZNkvNVvM/HARARCeRkPGN5gFvGICFKgJUzW0Z3Hto7JTVgiF/oA9ms\njmzzvkMZDiAo1ZNJ2C2FPYM45PHPyXzi84aef0kMWAOdxlo8NT5puWWqDyZp9ILx\n1MJio9Ni9e/BQ5DkhIQrWe7RbRPpJq7rAs22Lp4y4QKBgQD9DdcJ6uOYIeDzsF6r\nqJuZmwitODvrHC/xa2dgRbhP5x8vkLy8QXx9JPYy7G4fQJki8Om3+rY3Ljd944me\nX9z7PDUJOWXmiq+2eS9ANf5c/BT7DiwFr7+zCZ/TaBhiAHbO8XBlWVb9np6aiY/r\nTn0kbWHRCp0o18tSYQHJLEx5zwKBgQDlcoNo9ro7fQZ9WmiClUJUfy8xHBVreB6Y\nD6J6S03S0u2J95iIkINwirbt+dQqtCVl+6siIC5ijoXeZWqvvniomxCjgwefNMvm\nshTjjUo5EBqj5ErVODtEHxlNEqJuB3PYkGlRaIRdEMwiq7eJqkk7fdxl7yulSqbk\nwXWDmSu04QKBgFQuyHzKIVC45IbtA3GDs8/T3X0SmgxK4kAkBM3oI7qhHqTKN59B\nAdL3+tdSBAhtiwQutPG5e+i7fUZp7Zw8M914WARrjdS2CmJfyjJIRAhW6/vg6RAz\nOUisSPkhRgtEMEID2+o7YE8a95Rdcv1KrSLzKHlMWJ//uen7z4ZHMGHNAoGAfB18\nvjuws9tGtmpAXFrXHZQK+BrD73Vzvze94T944pEc02Puy/i4URmIKKTawxrdnBQO\nF5Cm5sNH4OylUFnNNQ9kab7IUbOilLFCIXFddA6lCoDEhiA71vanngx1duVrIfvE\nLLhH2XvroNmYl8xYfujvXuiIiIogS1/m7H/fp6ECgYB2yr78VIOSWvb4bjYJEt8P\nhpJODOjpb2gHadcR42zTIvP4cZ59VdIa89eQ8soZe0hr4xXYdNhdn21+lvBP/spC\nOBM7O7AbBAjwFTOSoIL690TxOfQ2NSTjo5ff8VQUmE9UaPnxxHRspVlqMDZFtnpN\n9Qj+pPBrWhEIp43p3F5rgQ==\n-----END PRIVATE KEY-----\n";
                var clientEmail = "firebase-adminsdk-hcvvt@vesact-my-axians.iam.gserviceaccount.com";
                var clientId = "111797189725147017391";
                var authUri = "https://accounts.google.com/o/oauth2/auth";
                var tokenUri = "https://oauth2.googleapis.com/token";
                var authProviderX509CertUrl = "https://www.googleapis.com/oauth2/v1/certs";
                var clientX509CertUrl = "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-hcvvt%40vesact-my-axians.iam.gserviceaccount.com";
                
                return new FirebaseConfig_V1(
                    type,
                    projectId,
                    privateKeyId,
                    privateKey,
                    clientEmail,
                    clientId,
                    authUri,
                    tokenUri,
                    authProviderX509CertUrl,
                    clientX509CertUrl
                );
            }
        }

        public static FirebaseConfig_V1 GetMyEtavisFirebaseV1Config
        {
            get
            {
                var type = "service_account";
                var projectId = "vesact-my-etavis";
                var privateKeyId = "b12b481e885e13fb6c3923147c5da47ac8398ffd";
                var privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCaNLVKODRYrNjZ\nIzKggq42Qwn96VfbFzADYdpHGwOYqP7qPec+K6osN7s9sWZ1D5BC6yCvv9WEhR0i\nmO9RQE0Fqmb1bQd+XhF49P/tuPITpLst+HKoiqPC/VvCdRqVzHQpHJGcpWTWFBQH\nq0iGgQGy2nB7bre/DcYosE1HRvfT1RLrhdOd5CAVctkzdFfBfPohH3Eyut3P4LpM\n1YJO8PsoziKm1XDDVAcd97SP1rXHAv2lPbIswj7ryltgswQlgqJfdNSH59itM902\nHAsBuAW7hMjPsCRZYj3sk0xCdFj5DXc/thmpmGy8GU21gfF93py6bw2R4yuF/tpU\ncFMjPwfBAgMBAAECggEAA+TAThUwtyt4FzV1Dgn2qZ6UNkHJytmhR/iUyRZUxyo/\nM31oJRD3rUGj++kKh/qiXxouO1nJ45xz0Z+QbVoFB9dXXauI1Kc95DK1cpRoolfW\ntRCwnnJPs7jvc+++gdlFdpNG1vBzJRehMNdhj6nat+AdlnRDdfK8VeMrtFjFoabV\n2IFN7U1b98+jBPbA0RLfGIfGp8858ozgd+1DeVcH48M1Ir5YRl/eW/MM6ps2JJc8\njM0nFE5eo9xTd6snPT0BFARMAFRjdhvtHmR9eDAULBy/8VxHphtZ9BfMTex8ovyC\nEyNGcj+UMs9FhL7OrS4Gu0H6apN/2dyVskn9n2l7AQKBgQDIrNNtWBXl+ZWszmoh\np0JKRtvbZh89AaYljLy/V/MXCmuq05zG+56YiXgNytMD8qDJRFrMfyH8qOtIGv1o\nZCC/KyUK+sOTJd4tf7Fm0Chr8066wTKpxDGTt8gXiKkIL2xsNuWC8dshaHHtxrj0\nQeHPjFTAafzkx4cEAA48pgbf4QKBgQDEuDIBAEJyZUF851ZwJN/21td0mf+lBGc1\n/AxStaCvDDr01dDQWGoD5lBBOCxh6nKotiznYuksJCrehLHe/fNqwynXw8Ue9/CD\nEbcMg+87EoVVUgYLU/FXkCXDfDb7UQ1KDftkAley/0QoRkxir1QH4sRftlHO8hKS\ntDHVS6ij4QKBgQDHW8JzR2YB4TCLy0huhG57wedbEBoKjl/Tv/wnDyWlhymtPKrs\nlz5YhYEqGowFyP1o1apqrAZErj+Hk6CaG5bYs/EJ9lNjFOSjSM8ht7vgeoeFSred\nJhamjJaQoqhiRFKvMt5RdxpeFxfPw3Ms3WKKwPvVZ7XgRkRgefmq0LnVoQKBgCGF\nwf+pq6kS2DYQtgomPoQx2EVmeMoGFhB9AxyVFuy9iM5wIUVfy7Eebk6u4+FLsn5N\njRlhIZsEDsockrMNU4299ENweQbt+W5cdnULhpjqbw47DAyiIV5qkgTeBgCNr1YP\nS+ee/pLiLRsv35RxjEBLxGsLBqXdWozlaBiQ7AwBAoGAOHpJmV2QKsk/6m/dfT94\nAa+3sLVTNr+kF1BNLD1FVEcuBlOjtYGLpb5R97u4AtFWC6tzFuVDY0VvCYnZzwm0\nRFuyZemHD4Up1n8JjyUffQExMtn3KAfpgvFQlReWp/jxQBR0OAV2fH+53xClU/CE\nf+U4beAgtwuZJXwqJJI3aEY=\n-----END PRIVATE KEY-----\n";
                var clientEmail = "firebase-adminsdk-uruy8@vesact-my-etavis.iam.gserviceaccount.com";
                var clientId = "100241589427691233142";
                var authUri = "https://accounts.google.com/o/oauth2/auth";
                var tokenUri = "https://oauth2.googleapis.com/token";
                var authProviderX509CertUrl = "https://www.googleapis.com/oauth2/v1/certs";
                var clientX509CertUrl = "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-uruy8%40vesact-my-etavis.iam.gserviceaccount.com";

                return new FirebaseConfig_V1(
                    type,
                    projectId,
                    privateKeyId,
                    privateKey,
                    clientEmail,
                    clientId,
                    authUri,
                    tokenUri,
                    authProviderX509CertUrl,
                    clientX509CertUrl
                );
            }
        }
    }
}
