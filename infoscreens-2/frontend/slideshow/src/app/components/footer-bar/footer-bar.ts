import { Component, OnDestroy, OnInit } from '@angular/core';
import {
    BrandingService,
    ConfigService,
    DateService,
    ThemeService,
    DEFAULT_BRANDING,
    DEFAULT_DATE_FORMAT,
    DEFAULT_FOOTER_STYLE,
    DEFAULT_THEME
} from '../../services';
import { TranslationPipe } from '../../pipes/translation.pipe';
import * as momentTZ from 'moment-timezone';
import { DateFormat, FooterStyle, Theme } from '../../../../../common';
import { filter, takeUntil } from 'rxjs/operators';
import { ReplaySubject } from 'rxjs';
import { Branding } from '../../../../../common/enums/brandings';

@Component({
    templateUrl: './footer-bar.html',
    styleUrls: ['./footer-bar.scss'],
    selector: 'footer-bar'
})
export class FooterBarComponent implements OnInit, OnDestroy {
    private _destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    private _timeInterval: NodeJS.Timer;

    currentTime: string;
    dateFormat: DateFormat = DEFAULT_DATE_FORMAT;
    invertDuoBranding: boolean = false;
    secondBranding: Branding | undefined = undefined;

    branding: Branding = DEFAULT_BRANDING;
    theme: Theme = DEFAULT_THEME;

    readonly FooterStyle = FooterStyle;
    footerStyle: FooterStyle = DEFAULT_FOOTER_STYLE;

    constructor(private _translation: TranslationPipe, private _dateService: DateService) {}

    ngOnInit() {
        this.setupTimeInterval();

        BrandingService.branding$.pipe(takeUntil(this._destroyed$)).subscribe(branding => {
            this.branding = branding;
        });

        ThemeService.theme$.pipe(takeUntil(this._destroyed$)).subscribe(theme => {
            this.theme = theme;
        });

        // Subscribing and logging here as doing it in the ConfigService would create a dependency loop with the LoggingService
        ConfigService.config$
            .pipe(
                takeUntil(this._destroyed$),
                filter(config => !!config)
            )
            .subscribe(newConfig => {
                this.footerStyle = newConfig.footerStyle || DEFAULT_FOOTER_STYLE;
                this.dateFormat = newConfig.dateFormat || DEFAULT_DATE_FORMAT;
                this.invertDuoBranding = !!newConfig.invertDuoBranding;
                this.secondBranding = newConfig.secondBranding;
                this.setupTimeInterval();
            });
    }

    ngOnDestroy() {
        this._destroyed$.next(true);
        this._destroyed$.complete();

        clearInterval(this._timeInterval);
    }

    setupTimeInterval() {
        this._translation.getTranslationFromFileAsync(`footer.dateTimeFormat.${this.dateFormat}`).then(format => {
            if (this._timeInterval) clearInterval(this._timeInterval);
            this._timeInterval = setInterval(() => {
                this.currentTime = this._dateService.formatDatetoLocal(momentTZ.utc(), format);
            }, 1000);
        });
    }

    getBrandName(position: 'primary' | 'secondary'): string {
        const returnOwn = (position === 'primary' && !this.invertDuoBranding) || (position === 'secondary' && this.invertDuoBranding);
        if (returnOwn) return this.branding;

        if (this.secondBranding) return this.secondBranding;

        switch (this.branding) {
            case Branding.ActemiumCH:
                return Branding.AxiansCH;

            case Branding.AxiansCH:
            case Branding.EtavisCH:
            default:
                return Branding.ActemiumCH;
        }
    }

    getLogoTheme(): 'White' | 'Colour' {
        switch (this.theme) {
            case Theme.LightAndDarkBlueFooter:
                return 'White';
            case Theme.RobomatOrange:
                return 'White';
            case Theme.UptownBlue:
            case Theme.LightDefault:
            default:
                return 'Colour';
        }
    }
}
