import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { DateFormat, FooterStyle, LocalCacheFileName, NodeConfig_Sync } from '../../../../common';

export const DEFAULT_FOOTER_STYLE: FooterStyle = FooterStyle.RollingText;
export const DEFAULT_DATE_FORMAT: DateFormat = DateFormat.Long;
@Injectable({
    providedIn: 'root'
})
export class ConfigService {
    static refreshInterval = undefined;
    static readonly CONFIG_REFRESH_RATE_MS: number = 30000;

    private static _config$: BehaviorSubject<NodeConfig_Sync> = new BehaviorSubject<NodeConfig_Sync>(undefined);
    public static get config$() {
        return ConfigService._config$.asObservable();
    }
    public static get config(): NodeConfig_Sync {
        return ConfigService._config$.value;
    }

    constructor(private http: HttpClient) {}

    async initAsync(): Promise<void> {
        return new Promise<void>(async (resolve, reject) => {
            try {
                await this._refreshConfig();
                if (ConfigService.refreshInterval) {
                    clearInterval(ConfigService.refreshInterval);
                }
                ConfigService.refreshInterval = setInterval(() => this._refreshConfig(), ConfigService.CONFIG_REFRESH_RATE_MS);
                resolve();
            } catch (error) {
                reject(error);
            }
        });
    }

    private _refreshConfig(): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            try {
                this.http.get<NodeConfig_Sync>('cache/' + LocalCacheFileName.Config).subscribe(data => {
                    if (data) {
                        ConfigService._config$.next(data);
                    }
                    resolve();
                });
            } catch (error) {
                reject(error);
            }
        });
    }
}
