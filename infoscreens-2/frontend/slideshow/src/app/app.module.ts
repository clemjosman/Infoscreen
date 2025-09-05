import 'reflect-metadata';
import '../polyfills';
import { HttpClientModule, HttpClientJsonpModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GoogleMapsModule } from '@angular/google-maps';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

// Third party plugings
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { QRCodeModule } from 'angularx-qrcode';


// App
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

// Slides
import { CoffeeComponent } from './slides/coffee/coffee.component';
import { CustomJobOfferComponent } from './slides/customJobOffer/customJobOffer.component';
import { DailyWeatherComponent } from './slides/weather-daily/weather.component';
import { IdeaboxComponent } from './slides/ideabox/ideabox.component';
import { IframeComponent } from './slides/iframe/iframe.component';
import { InfoscreenMonitoringComponent } from './slides/infoscreenmonitoring/infoscreenmonitoring.component';
import { LocalVideoComponent } from './slides/localvideo/localvideo.component';
import { JobOffersComponent } from './slides/jobOffers/jobOffers.component';
import { MaintenanceComponent } from './slides/maintenance/maintenance.component';
import { MonitoringIframeComponent } from './slides/monitoringiframe/monitoringiframe.component';
import { NewsInternalComponent } from './slides/newsInternal/newsInternal.component';
import { NewsPublicComponent } from './slides/newsPublic/newsPublic.component';
import { PollutionIndicatorComponent } from './slides/weather-daily/pollution-indicator/pollution-indicator.component';
import { PublicTransportComponent } from './slides/public-transport/publicTransport.component';
import { SociabbleComponent } from './slides/sociabble/sociabble.component';
import { SpotlightComponent } from './slides/spotlight/spotlight.component';
import { StockComponent } from './slides/stock/stock.component';
import { TrafficComponent } from './slides/traffic/traffic.component';
import { TransportTableComponent } from './slides/public-transport/tranportTable.component/transportTable.component';
import { TwentyMinComponent } from './slides/twentyMin/twentyMin.component';
import { TwitterComponent } from './slides/twitter/twitter.component';
import { UniversityComponent } from './slides/university/university.component';
import { UniversityOverviewComponent } from './slides/universityOverview/universityOverview.component';
import { UptownArticleComponent } from './slides/uptown-article/uptown-article.component';
import { UptownEventComponent } from './slides/uptown-event/uptown-event.component';
import { UptownMenuComponent } from './slides/uptown-menu/uptown-menu.component';
import { WeeklyWeatherComponent } from './slides/weather-weekly/weather.component';
import { YoutubeComponent } from './slides/youtube/youtube.component';

// Components
import { CenteredTextContentComponent } from './components/slideContent/centeredText/centeredText.component';
import { FooterBarComponent } from './components/footer-bar/footer-bar';
import { IframeContentComponent } from './components/slideContent/iframe/iframe.component';
import { ImageContentComponent } from './components/slideContent/image/image.component';
import { InfoscreenNodeStatusCard } from './components/infoscreenNodeStatusCard/infoscreenNodeStatusCard.component';
import { InternetStatusComponent } from './components/internet-status/internet-status';
import { LocalVideoContentComponent } from './components/slideContent/localVideo/localVideo.component';
import { PdfAngularContentComponent } from './components/slideContent/pdf-angular/pdf-angular.component';
import { PdfNativeContentComponent } from './components/slideContent/pdf-native/pdf-native.component';
import { RollingMessageComponent } from './components/rollingMessage/rollingMessage.component';
import { SlideBannerComponent } from './components/slideBanner/slideBanner.component';
import { SlideBubbleComponent } from './components/slideBubble/slideBubble.component';
import { SplitContentComponent } from './components/slideContent/splitContent/splitContent.component';
import { TitleContentComponent } from './components/slideContent/title/title.component';
import { StarComponent } from './slides/ideabox/rating-component/star/star.component';
import { RatingComponent } from './slides/ideabox/rating-component/rating.component';


// Layouts
import { BannerLayoutComponent } from './layouts/bannerLayout/bannerLayout.component';

// Pipes
import { SafePipe } from './helpers/SafePipe';
import { TranslationPipe } from './pipes/translation.pipe';

// i18n
import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeDECH from '@angular/common/locales/de-CH';

// Services
import {
    BrandingService,
    CacheService,
    ConfigService,
    DateService,
    InternetAccessService,
    LoggingService,
    RoutingService,
    ThemeService,
    TranslationService
} from './services';

// Register de-CH culture
registerLocaleData(localeDECH);

@NgModule({
    declarations: [
        AppComponent,

        // Slides
        CoffeeComponent,
        CustomJobOfferComponent,
        DailyWeatherComponent,
        IdeaboxComponent,
        IframeComponent,
        InfoscreenMonitoringComponent,
        LocalVideoComponent,
        JobOffersComponent,
        MaintenanceComponent,
        MonitoringIframeComponent,
        NewsInternalComponent,
        NewsPublicComponent,
        PollutionIndicatorComponent,
        PublicTransportComponent,
        SociabbleComponent,
        SpotlightComponent,
        StockComponent,
        TrafficComponent,
        TransportTableComponent,
        TwentyMinComponent,
        TwitterComponent,
        UniversityComponent,
        UniversityOverviewComponent,
        UptownArticleComponent,
        UptownEventComponent,
        UptownMenuComponent,
        YoutubeComponent,
        WeeklyWeatherComponent,

        // Components
        CenteredTextContentComponent,
        FooterBarComponent,
        IframeContentComponent,
        ImageContentComponent,
        InfoscreenNodeStatusCard,
        InternetStatusComponent,
        LocalVideoContentComponent,
        PdfAngularContentComponent,
        PdfNativeContentComponent,
        RollingMessageComponent,
        SlideBannerComponent,
        SlideBubbleComponent,
        SplitContentComponent,
        TitleContentComponent,
        StarComponent,
        RatingComponent,

        // Layout
        BannerLayoutComponent,

        // Pipes
        SafePipe,
        TranslationPipe
    ],
    imports: [
        AppRoutingModule,
        BrowserAnimationsModule,
        BrowserModule,
        CommonModule,
        FormsModule,
        GoogleMapsModule,
        HttpClientJsonpModule,
        HttpClientModule,
        PdfViewerModule,
        QRCodeModule,
        RouterModule,
        MatIconModule
 
    ],
    providers: [
        // Local used for i18n extraction and by i18n pipes like DatePipe
        { provide: LOCALE_ID, useValue: 'de-ch' },

        // Services
        BrandingService,
        CacheService,
        ConfigService,
        DateService,
        InternetAccessService,
        LoggingService,
        RoutingService,
        ThemeService,
        TranslationService
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
