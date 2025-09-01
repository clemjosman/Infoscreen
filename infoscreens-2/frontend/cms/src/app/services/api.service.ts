import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { RootApiService, TenantService, UiLanguage } from '@vesact/web-ui-template';
import { ConfigService } from '@services/config.service';
import { SnackbarService } from '@services/snackbar.service';
import { UserService } from '@services/user.service';
import {
    apiInfoscreen_Status,
    apiUser_Me,
    apiUser_UpdateSelectedLanguage,
    apiUser_UpdateSelectedTenant,
    apiLanguage,
    apiInfoscreen_Light,
    apiTenant,
    apiNews,
    apiNews_Translate,
    apiNews_Translated,
    apiNews_Publish,
    apiVideo,
    apiVideo_Publish,
    apiVideo_Translate,
    apiVideo_Translated,
    apiInfoscreenGroup,
    apiInfoscreen_MetdaDataUpdate,
    apiInfoscreen_ConfigUpdate,
    apiCategory,
    apiInfoscreenCategories
} from '@models/index';
import { NodeConfig } from '../../../../common';
import { ContentSearchParameters } from '@app/components';

@Injectable()
export class ApiService {
    constructor(
        private configService: ConfigService,
        private http: HttpClient,
        private userService: UserService,
        private snackbarService: SnackbarService,
        private _rootApiService: RootApiService,
        private _tenantService: TenantService
    ) {}

    private get _rootUrl(): string {
        return this._rootApiService.globalEndpoint;
    }

    private get _tenantCode(): string {
        return this._tenantService.currentTenant.code;
    }

    //****************************************
    //               LANGUAGES
    //****************************************

    public getLanguagesAsync(): Promise<apiLanguage[]> {
        return new Promise<apiLanguage[]>((resolve, reject) => {
            this.http.get(this._rootUrl + 'v1/language').subscribe(
                (data: apiLanguage[]) => {
                    resolve(data);
                },
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('language.getList.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public updateSelectedLanguageAsync(newSelectedLanguage: UiLanguage): Promise<apiUser_Me> {
        var body = new apiUser_UpdateSelectedLanguage(newSelectedLanguage.iso2);

        return new Promise<apiUser_Me>((resolve, reject) => {
            this.http.put(this._rootUrl + 'v1/user/selectedLanguage', body).subscribe(
                (data: apiUser_Me) => {
                    this.userService.emitAuthenticatedUser(data);
                    resolve(data);
                },
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('user.updateLanguage.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    //****************************************
    //              CATEGORIES
    //****************************************

    public getCategoriesAsync(): Promise<apiCategory[]> {
        return new Promise<apiCategory[]>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/category/${this._tenantCode}`).subscribe(
                (data: apiCategory[]) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('category.getList.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public getInfoscreenCategoriesAsync(infoscreenId: number): Promise<apiInfoscreenCategories> {
        return new Promise<apiInfoscreenCategories>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/category/${this._tenantCode}/infoscreen/${infoscreenId}`).subscribe(
                (data: apiInfoscreenCategories) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('infoscreen.getCategories.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    //****************************************
    //              INFOSCREENS
    //****************************************

    public getInfoscreensAsync(): Promise<apiInfoscreen_Light[]> {
        return new Promise<apiInfoscreen_Status[]>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/infoscreens/${this._tenantCode}`).subscribe(
                (data: apiInfoscreen_Status[]) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('infoscreen.getList.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public getInfoscreenAsync(infoscreenId: number): Promise<apiInfoscreen_Light> {
        return new Promise<apiInfoscreen_Light>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/infoscreen/${infoscreenId}`).subscribe(
                (data: apiInfoscreen_Light) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('infoscreen.get.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public updateInfoscreenMetadataAsync(infoscreenId: number, metadata: apiInfoscreen_MetdaDataUpdate): Promise<apiInfoscreen_Light> {
        return new Promise<apiInfoscreen_Light>((resolve, reject) => {
            this.http.put(this._rootUrl + `v1/infoscreen/${infoscreenId}/metadata`, metadata).subscribe(
                (data: apiInfoscreen_Light) => {
                    this.snackbarService.displaySuccessSnackbar('infoscreen.updateMetadata.sucess');
                    resolve(data);
                },
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('infoscreen.updateMetadata.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public getInfoscreenConfigAsync(infoscreenId: number): Promise<NodeConfig> {
        return new Promise<NodeConfig>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/infoscreen/${infoscreenId}/config`).subscribe(
                (data: NodeConfig) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('infoscreen.getConfig.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public updateInfoscreenConfigAsync(infoscreenId: number, config: apiInfoscreen_ConfigUpdate): Promise<NodeConfig> {
        return new Promise<NodeConfig>((resolve, reject) => {
            this.http.put(this._rootUrl + `v1/infoscreen/${infoscreenId}/config`, config).subscribe(
                (data: NodeConfig) => {
                    this.snackbarService.displaySuccessSnackbar('infoscreen.updateConfig.sucess');
                    resolve(data);
                },
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('infoscreen.updateConfig.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public getInfoscreensStatusAsync(): Promise<apiInfoscreen_Status[]> {
        return new Promise<apiInfoscreen_Status[]>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/infoscreens/${this._tenantCode}/status`).subscribe(
                (data: apiInfoscreen_Status[]) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('infoscreens.getStatus.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    //****************************************
    //          INFOSCREEN GROUPS
    //****************************************

    public getInfoscreenGroupsAsync(): Promise<apiInfoscreenGroup[]> {
        return new Promise<apiInfoscreenGroup[]>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/infoscreens/${this._tenantCode}/groups`).subscribe(
                (data: apiInfoscreenGroup[]) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('infoscreen.getGroups.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    //****************************************
    //                NEWS
    //****************************************

    public getAllNewsAsync(): Promise<apiNews[]> {
        return new Promise<apiNews[]>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/news/${this._tenantCode}`).subscribe(
                (data: apiNews[]) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('news.getList.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public getFilteredNewsAsync(searchParameters: ContentSearchParameters): Promise<apiNews[]> {
        return new Promise<apiNews[]>((resolve, reject) => {
            let searchText = searchParameters?.searchText ? `&search=${encodeURIComponent(searchParameters.searchText)}` : '';
            let selectedCategories = searchParameters?.selectedCategoryIds?.length
                ? `&categories=${searchParameters.selectedCategoryIds.join(',')}`
                : '';
            let selectedInfoscreens = searchParameters?.selectedInfoscreenIds?.length
                ? `&infoscreens=${searchParameters.selectedInfoscreenIds.join(',')}`
                : '';

            let query = `${searchText}${selectedCategories}${selectedInfoscreens}`;
            if (query.length > 0) query = '?' + query.substring(1);

            this.http.get(this._rootUrl + `v1/news/${this._tenantCode}${query}`).subscribe(
                (data: apiNews[]) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('news.getList.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public getNewsAsync(newsId: number): Promise<apiNews> {
        return new Promise<apiNews>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/news/${this._tenantCode}/${newsId}`).subscribe(
                (data: apiNews) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('news.get.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public postNewsAsync(news: apiNews_Publish): Promise<apiNews> {
        if (news.id) {
            throw 'Cannot post a news with id.';
        }

        return new Promise<apiNews>((resolve, reject) => {
            this.http.post(this._rootUrl + `v1/news/${this._tenantCode}`, news).subscribe(
                (data: apiNews) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('news.save.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public putNewsAsync(news: apiNews_Publish): Promise<apiNews> {
        if (!news.id) {
            throw 'Cannot put a news without id.';
        }

        return new Promise<apiNews>((resolve, reject) => {
            this.http.put(this._rootUrl + `v1/news/${this._tenantCode}`, news).subscribe(
                (data: apiNews) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('news.save.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public deleteNewsAsync(newsId: number): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            this.http.delete(this._rootUrl + `v1/news/${this._tenantCode}/${newsId}`).subscribe(
                () => resolve(),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('news.delete.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public deleteMultipleNewsAsync(newsIds: number[]): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            this.http.delete(this._rootUrl + `v1/news/${this._tenantCode}?newsIds=${newsIds.join(',')}`).subscribe(
                () => resolve(),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('news.deleteList.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public translateNewsAsync(newsTranslate: apiNews_Translate): Promise<apiNews_Translated> {
        return new Promise<apiNews_Translated>((resolve, reject) => {
            this.http.post(this._rootUrl + `v1/news/${this._tenantCode}/translate`, newsTranslate).subscribe(
                (data: apiNews_Translated) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('news.translate.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public resetNotificationFlag(newsId: number): Promise<apiNews> {
        return new Promise<apiNews>((resolve, reject) => {
            this.http.post(this._rootUrl + `v1/news/${this._tenantCode}/${newsId}/resetNotification`, undefined).subscribe(
                (data: apiNews) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('news.restNotification.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    //****************************************
    //               VIDEO
    //****************************************

    public getAllVideosAsync(): Promise<apiVideo[]> {
        return new Promise<apiVideo[]>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/video/${this._tenantCode}`).subscribe(
                (data: apiVideo[]) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('video.getList.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public getFilteredVideosAsync(searchParameters: ContentSearchParameters): Promise<apiVideo[]> {
        return new Promise<apiVideo[]>((resolve, reject) => {
            let searchText = searchParameters?.searchText ? `&search=${encodeURIComponent(searchParameters.searchText)}` : '';
            let selectedCategories = searchParameters?.selectedCategoryIds?.length
                ? `&categories=${searchParameters.selectedCategoryIds.join(',')}`
                : '';
            let selectedInfoscreens = searchParameters?.selectedInfoscreenIds?.length
                ? `&infoscreens=${searchParameters.selectedInfoscreenIds.join(',')}`
                : '';

            let query = `${searchText}${selectedCategories}${selectedInfoscreens}`;
            if (query.length > 0) query = '?' + query.substring(1);

            this.http.get(this._rootUrl + `v1/video/${this._tenantCode}${query}`).subscribe(
                (data: apiVideo[]) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('video.getList.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public getVideoAsync(videoId: number): Promise<apiVideo> {
        return new Promise<apiVideo>((resolve, reject) => {
            this.http.get(this._rootUrl + `v1/video/${this._tenantCode}/${videoId}`).subscribe(
                (data: apiVideo) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('video.get.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public postVideoAsync(video: apiVideo_Publish): Promise<apiVideo> {
        if (video.id) {
            throw 'Cannot post a video with id.';
        }

        return new Promise<apiVideo>((resolve, reject) => {
            this.http.post(this._rootUrl + `v1/video/${this._tenantCode}`, video).subscribe(
                (data: apiVideo) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('video.save.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public putVideoAsync(video: apiVideo_Publish): Promise<apiVideo> {
        if (!video.id) {
            throw 'Cannot put a video without id.';
        }

        return new Promise<apiVideo>((resolve, reject) => {
            this.http.put(this._rootUrl + `v1/video/${this._tenantCode}`, video).subscribe(
                (data: apiVideo) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('video.save.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public deleteVideoAsync(videoId: number): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            this.http.delete(this._rootUrl + `v1/video/${this._tenantCode}/${videoId}`).subscribe(
                () => resolve(),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('video.delete.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public deleteMultipleVideosAsync(videoIds: number[]): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            this.http.delete(this._rootUrl + `v1/video/${this._tenantCode}?videoIds=${videoIds.join(',')}`).subscribe(
                () => resolve(),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('video.deleteList.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }

    public translateVideoAsync(videoTranslate: apiVideo_Translate): Promise<apiVideo_Translated> {
        return new Promise<apiVideo_Translated>((resolve, reject) => {
            this.http.post(this._rootUrl + `v1/video/${this._tenantCode}/translate`, videoTranslate).subscribe(
                (data: apiVideo_Translated) => resolve(data),
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('video.translate.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            );
        });
    }
}
