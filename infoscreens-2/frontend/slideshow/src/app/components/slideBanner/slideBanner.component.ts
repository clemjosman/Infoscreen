import { Component, HostBinding, Input } from '@angular/core';
import { BrandingService } from '../../services';
import { TranslationPipe } from '../../pipes/translation.pipe';
import { BannerStyle, Slide } from '../../../../../common';
import { ConfigService } from '../../services/config.service';
import { Branding } from '../../../../../common/enums/brandings';

@Component({
    templateUrl: './slideBanner.html',
    styleUrls: ['./slideBanner.scss'],
    selector: 'slide-banner'
})
export class SlideBannerComponent {
    @Input() slide: Slide;
    @Input() forcedTitle: string = undefined;
    @Input() headerWhenHidden: boolean = true;
    @Input('showTopBanner') _showTopBanner: () => boolean;
    @HostBinding('attr.banner-top')
    get isBannerHidden() {
        return this.bannerStyle === this.topBannerStyle ? '' : null;
    }
    readonly leftBannerStyle = BannerStyle.Left;
    readonly topBannerStyle = BannerStyle.Top;
    readonly noneBannerStyle = BannerStyle.None;
    public bannerStyle: BannerStyle = BannerStyle.Left;
    title: string = null;
    bannerIconSource: string = null;
    isTwentyMin: boolean = false;
    isTwitter: boolean = false;
    isStock: boolean = false;
    backgroundPath: string = 'assets/images/banner/unknown.png';
    backgroundColor: string = 'rgba(0, 115, 255, 0.25)';
    filter: {
        grayScale: number;
        brightness: number;
        sepia: number;
        hueRotate: number;
        saturate: number;
        contrast: number;
    } = {
        grayScale: 100,
        brightness: 26,
        sepia: 100,
        hueRotate: -180,
        saturate: 700,
        contrast: 0.8
    };

    constructor(private translationPipe: TranslationPipe) {}

    ngOnInit() {
        ConfigService.config$.subscribe(config => {
            if (config) {
                this.bannerStyle = config.bannerStyle;
            }
        });

        if (this.slide) {
            var textCode = undefined;

            BrandingService.branding$.subscribe(branding => {
                switch (branding) {
                    case Branding.AxiansCH:
                        this.backgroundColor = 'rgba(0, 0, 0, 0.25)';
                        this.filter.saturate = 177;
                        break;
                    case Branding.EtavisCH:
                        this.backgroundColor = 'rgba(0, 0, 0, 0.25)';
                        this.filter.brightness = 28;
                        this.filter.hueRotate = 160;
                        this.filter.saturate = 177;
                        break;
                    case Branding.ActemiumCH:
                    default:
                        this.backgroundColor = 'rgba(0, 115, 255, 0.25)';
                        this.filter.saturate = 700;
                        break;
                }
            });

            switch (this.slide) {
                case Slide.Coffee:
                    textCode = 'title.coffee';
                    this.backgroundPath = 'assets/images/banner/coffeeBanner.png';
                    break;

                case Slide.CustomJobOffer:
                    textCode = 'title.jobOffer';
                    this.backgroundPath = 'assets/images/banner/joboffers.png';
                    break;

                case Slide.Ideabox:
                    textCode = 'title.ideabox';
                    this.backgroundPath = 'assets/images/banner/ideaboxBanner.png';
                    break;

                case Slide.Iframe:
                    textCode = 'title.iframe';
                    this.backgroundPath = 'assets/images/banner/iframeBanner.png';
                    break;

                case Slide.InfoscreenMonitoring:
                case Slide.MonitoringIframe:
                    textCode = 'title.monitoring';
                    this.backgroundPath = 'assets/images/banner/monitoringBanner.png';
                    break;

                case Slide.JobOffers:
                    textCode = 'title.jobOffers';
                    this.backgroundPath = 'assets/images/banner/joboffers.png';
                    break;

                case Slide.LocalVideo:
                    textCode = 'title.localVideo';
                    this.backgroundPath = 'assets/images/banner/youtubeBanner.png';
                    break;

                case Slide.NewsInternal:
                    textCode = 'title.internalNews';
                    this.backgroundPath = 'assets/images/banner/newsBanner.png';
                    break;

                case Slide.NewsPublic:
                    textCode = 'title.publicNews';
                    this.backgroundPath = 'assets/images/banner/newsBanner.png';
                    break;

                case Slide.PublicTransport:
                    textCode = 'title.publicTransport';
                    this.backgroundPath = 'assets/images/banner/publicTransportBanner.png';
                    break;

                case Slide.Sociabble:
                    textCode = 'title.sociabblepost';
                    this.backgroundPath = 'assets/images/banner/sociabbleBanner.png';
                    break;

                case Slide.Spotlight:
                    textCode = 'title.spotlight';
                    this.backgroundPath = 'assets/images/banner/spotlightBanner.png';
                    break;

                case Slide.Stock:
                    textCode = 'title.stock';
                    this.backgroundPath = 'assets/images/banner/stockBanner.png';
                    this.isStock = true;
                    break;

                case Slide.Traffic:
                    textCode = 'title.traffic';
                    this.backgroundPath = 'assets/images/banner/trafficBanner.png';
                    break;

                case Slide.TwentyMin:
                    textCode = 'title.twentymin';
                    this.backgroundPath = 'assets/images/banner/newsBanner.png';
                    this.isTwentyMin = true;
                    break;

                case Slide.Twitter:
                    textCode = 'title.twitter';
                    this.backgroundPath = 'assets/images/banner/twitter.png';
                    this.isTwitter = true;
                    break;

                case Slide.University:
                    textCode = 'title.university';
                    this.backgroundPath = 'assets/images/banner/universityBanner.png';
                    break;

                case Slide.UptownArticle:
                    textCode = 'title.uptownarticle';
                    this.backgroundPath = 'assets/images/banner/uptownBanner.jpg';
                    break;

                case Slide.UptownEvent:
                    textCode = 'title.uptownevent';
                    this.backgroundPath = 'assets/images/banner/uptownBanner.jpg';
                    break;

                case Slide.UptownMenu:
                    textCode = 'title.uptownmenu';
                    this.backgroundPath = 'assets/images/banner/uptownBanner.jpg';
                    break;

                case Slide.WeatherWeekly:
                    textCode = 'title.weatherWeekly';
                    this.backgroundPath = 'assets/images/banner/weatherBanner.png';
                    break;

                case Slide.WeatherDaily:
                    textCode = 'title.weatherDaily';
                    this.backgroundPath = 'assets/images/banner/weatherBanner.png';
                    break;

                case Slide.Youtube:
                    textCode = 'title.youtube';
                    this.backgroundPath = 'assets/images/banner/youtubeBanner.png';
                    break;

                case Slide.UniversityOverview:
                case Slide.Maintenance:
                    return;

                default:
                    return;
            }

            this.translationPipe.getTranslationFromFileAsync(textCode).then(text => {
                this.title = text;
            });
        }
    }

    showTopBanner(): boolean {
        if (this._showTopBanner) {
            return this._showTopBanner();
        }
        return false;
    }
}
