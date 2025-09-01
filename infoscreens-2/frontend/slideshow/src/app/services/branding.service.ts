import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Branding } from '../../../../common/enums/brandings';
import { ConfigService } from './config.service';
import { LoggingService } from './logging.service';

export const DEFAULT_BRANDING: Branding = Branding.ActemiumCH;

@Injectable({
    providedIn: 'root'
})
export class BrandingService {
    public nodeTags: string[] = [];

    private static _branding$: BehaviorSubject<Branding> = new BehaviorSubject<Branding>(DEFAULT_BRANDING);
    public static get branding$() {
        return BrandingService._branding$.asObservable();
    }
    public static get branding(): Branding {
        return BrandingService._branding$.value;
    }

    constructor() {}

    init() {
        ConfigService.config$.pipe(filter(config => !!config)).subscribe(
            config => {
                this.nodeTags = config.nodeTags || [];
                this._updateBranding();
            },
            err => {
                LoggingService.error(
                    BrandingService.name,
                    'Config subscription',
                    'Could not get the nodeTags branding configuration from the config file. Using defaults: ' + DEFAULT_BRANDING,
                    err
                );
            }
        );
    }

    private _updateBranding(): void {
        if (this.nodeTags.length != 0) {
            let newBranding: Branding = undefined;
            for (const tag of this.nodeTags) {
                switch (tag) {
                    case '#AxiansCH':
                        newBranding = Branding.AxiansCH;
                        break;
                    case '#ActemiumCH':
                        newBranding = Branding.ActemiumCH;
                        break;
                    case '#EtavisCH':
                        newBranding = Branding.EtavisCH;
                        break;
                    case '#RobomatCH':
                        newBranding = Branding.RobomatCH;
                        break;
                    case '#Uptown':
                        newBranding = Branding.Uptown;
                }
                if (newBranding) break;
            }
            BrandingService._branding$.next(newBranding || Branding.ActemiumCH);
        }
    }
}
