import { Component, HostBinding, Input, OnInit } from '@angular/core';
import { ConfigService } from '../../services';
import { BannerStyle, Slide } from '../../../../../common';

@Component({
    templateUrl: './bannerLayout.html',
    styleUrls: ['./bannerLayout.scss'],
    selector: 'banner-layout'
})
export class BannerLayoutComponent implements OnInit {
    @Input() slide: Slide;
    @Input() forcedTitle: string = undefined;
    @Input() headerWhenBannerHidden: boolean = true;
    @HostBinding('attr.banner-left')
    get isBannerLeft() {
        return this.bannerStyle === BannerStyle.Left ? '' : null;
    }
    @HostBinding('attr.banner-top')
    get isBannerTop() {
        return this.bannerStyle === BannerStyle.Top && this.showTopBanner() ? '' : null;
    }
    bannerStyle: BannerStyle = BannerStyle.Left;

    constructor() {}

    ngOnInit() {
        ConfigService.config$.subscribe(config => {
            if (config) {
                this.bannerStyle = config.bannerStyle;
            }
        });
    }

    showBanner(): boolean {
        return this.bannerStyle !== BannerStyle.None && (this.bannerStyle !== BannerStyle.Top || this.showTopBanner());
    }

    showTopBanner(): boolean {
        switch (this.slide) {
            case Slide.Coffee:
            case Slide.InfoscreenMonitoring:
            case Slide.LocalVideo:
            case Slide.MonitoringIframe:
            case Slide.Youtube:
                return false;
            default:
                return true;
        }
    }
}
