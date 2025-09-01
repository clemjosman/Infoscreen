import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';

import { Slide } from '../../../common';

import { CoffeeComponent } from './slides/coffee/coffee.component';
import { CustomJobOfferComponent } from './slides/customJobOffer/customJobOffer.component';
import { DailyWeatherComponent } from './slides/weather-daily/weather.component';
import { IdeaboxComponent } from './slides/ideabox/ideabox.component';
import { IframeComponent } from './slides/iframe/iframe.component';
import { InfoscreenMonitoringComponent } from './slides/infoscreenmonitoring/infoscreenmonitoring.component';
import { JobOffersComponent } from './slides/jobOffers/jobOffers.component';
import { LocalVideoComponent } from './slides/localvideo/localvideo.component';
import { MaintenanceComponent } from './slides/maintenance/maintenance.component';
import { MonitoringIframeComponent } from './slides/monitoringiframe/monitoringiframe.component';
import { NewsInternalComponent } from './slides/newsInternal/newsInternal.component';
import { NewsPublicComponent } from './slides/newsPublic/newsPublic.component';
import { PublicTransportComponent } from './slides/public-transport/publicTransport.component';
import { SociabbleComponent } from './slides/sociabble/sociabble.component';
import { SpotlightComponent } from './slides/spotlight/spotlight.component';
import { StockComponent } from './slides/stock/stock.component';
import { TrafficComponent } from './slides/traffic/traffic.component';
import { TwentyMinComponent } from './slides/twentyMin/twentyMin.component';
import { TwitterComponent } from './slides/twitter/twitter.component';
import { UniversityComponent } from './slides/university/university.component';
import { UniversityOverviewComponent } from './slides/universityOverview/universityOverview.component';
import { UptownArticleComponent } from './slides/uptown-article/uptown-article.component';
import { UptownEventComponent } from './slides/uptown-event/uptown-event.component';
import { UptownMenuComponent } from './slides/uptown-menu/uptown-menu.component';
import { YoutubeComponent } from './slides/youtube/youtube.component';
import { WeeklyWeatherComponent } from './slides/weather-weekly/weather.component';

const routes: Routes = [
    // Default Route:
    // { path: '', pathMatch: 'full', redirectTo: Slide.Coffee},
    {
        path: Slide.CustomJobOffer,
        component: CustomJobOfferComponent,
        data: { animation: Slide.CustomJobOffer }
    },
    {
        path: Slide.Coffee,
        component: CoffeeComponent,
        data: { animation: Slide.Coffee }
    },
    {
        path: Slide.Ideabox,
        component: IdeaboxComponent,
        data: { animation: Slide.Ideabox }
    },
    {
        path: Slide.Iframe,
        component: IframeComponent,
        data: { animation: Slide.Iframe }
    },
    {
        path: Slide.InfoscreenMonitoring,
        component: InfoscreenMonitoringComponent,
        data: { animation: Slide.InfoscreenMonitoring }
    },
    {
        path: Slide.JobOffers,
        component: JobOffersComponent,
        data: { animation: Slide.JobOffers }
    },
    {
        path: Slide.LocalVideo,
        component: LocalVideoComponent,
        data: { animation: Slide.LocalVideo }
    },
    {
        path: Slide.Maintenance,
        component: MaintenanceComponent,
        data: { animation: Slide.Maintenance }
    },
    {
        path: Slide.MonitoringIframe,
        component: MonitoringIframeComponent,
        data: { animation: Slide.MonitoringIframe }
    },
    {
        path: Slide.NewsInternal,
        component: NewsInternalComponent,
        data: { animation: Slide.NewsInternal }
    },
    {
        path: Slide.NewsPublic,
        component: NewsPublicComponent,
        data: { animation: Slide.NewsPublic }
    },
    {
        path: Slide.PublicTransport,
        component: PublicTransportComponent,
        data: { animation: Slide.PublicTransport }
    },
    {
        path: Slide.Sociabble,
        component: SociabbleComponent,
        data: { animation: Slide.Sociabble }
    },
    {
        path: Slide.Spotlight,
        component: SpotlightComponent,
        data: { animation: Slide.Spotlight }
    },
    {
        path: Slide.Stock,
        component: StockComponent,
        data: { animation: Slide.Stock }
    },
    {
        path: Slide.Traffic,
        component: TrafficComponent,
        data: { animation: Slide.Traffic }
    },
    {
        path: Slide.TwentyMin,
        component: TwentyMinComponent,
        data: { animation: Slide.TwentyMin }
    },
    {
        path: Slide.Twitter,
        component: TwitterComponent,
        data: { animation: Slide.Twitter }
    },
    {
        path: Slide.University,
        component: UniversityComponent,
        data: { animation: Slide.University }
    },
    {
        path: Slide.UniversityOverview,
        component: UniversityOverviewComponent,
        data: { animation: Slide.UniversityOverview }
    },
    {
        path: Slide.UptownEvent,
        component: UptownEventComponent,
        data: { animation: Slide.UptownEvent }
    },
    {
        path: Slide.UptownArticle,
        component: UptownArticleComponent,
        data: { animation: Slide.UptownArticle }
    },
    {
        path: Slide.UptownMenu,
        component: UptownMenuComponent,
        data: { animation: Slide.UptownMenu }
    },
    {
        path: Slide.WeatherDaily,
        component: DailyWeatherComponent,
        data: { animation: Slide.WeatherDaily }
    },
    {
        path: Slide.WeatherWeekly,
        component: WeeklyWeatherComponent,
        data: { animation: Slide.WeatherWeekly }
    },
    {
        path: Slide.Youtube,
        component: YoutubeComponent,
        data: { animation: Slide.Youtube }
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { useHash: true })],
    exports: [RouterModule]
})
export class AppRoutingModule {}
