import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CacheService } from './cache.service';
import * as $ from 'jquery';
import { IndexableCache } from '../enums/indexableCache';
import { LoggingService } from './logging.service';
import { ConfigService } from './config.service';
import { Slide } from '../../../../common';
import { ignoreElements } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class RoutingService {
    DEFAULT_DURATION_SEC: number = 30;

    currentSlideIndex: number = -1;

    constructor(private router: Router, private cacheService: CacheService) {}

    async startRoutingLoop(): Promise<void> {
        try {
            var self = this;

            (async () => {
                $(document).keydown(function (e) {
                    switch (e.which) {
                        case 37: // left
                            self.navigateToPreviousSlide();
                            break;

                        case 38: // up
                            break;

                        case 39: // right
                            self.navigateToNextSlide();
                            break;

                        case 40: // down
                            break;

                        default:
                            return; // exit this handler for other keys
                    }
                    e.preventDefault(); // prevent the default action (scroll / move caret)
                });

                if (ConfigService.config.maintenanceMode) {
                    LoggingService.info(
                        RoutingService.name,
                        'startRoutingLoop',
                        'Entering in maintenance mode, a restart is needed once the maintenance is done'
                    );
                    this.router.navigate([Slide.Maintenance]);
                } else {
                    //DEBUG: Comment the following while loop to disable automatic slideshow when debugging
                    while (true) {
                        LoggingService.debug(RoutingService.name, 'startRoutingLoop', 'Slideshow started');
                        await this.delay(this.navigateToNextSlide());
                    }
                }
            })();
        } catch (error) {
            LoggingService.error(RoutingService.name, 'startRoutingLoop', 'An error occured in the rooting loop', error);
        }
    }

    navigateToNextSlide(): number {
        this.currentSlideIndex++;
        if (this.currentSlideIndex >= ConfigService.config.slides.order.length) {
            this.currentSlideIndex = 0;
        }

        var nextSlide = ConfigService.config.slides.order[this.currentSlideIndex];
        this.logSlideNavigationInfo(nextSlide);
        return this.navigateToSlide(nextSlide);
    }

    navigateToPreviousSlide(): number {
        this.currentSlideIndex--;
        if (this.currentSlideIndex < 0) {
            this.currentSlideIndex = ConfigService.config.slides.order.length - 1;
        }

        var previousSlide = ConfigService.config.slides.order[this.currentSlideIndex];
        this.logSlideNavigationInfo(previousSlide);
        return this.navigateToSlide(previousSlide);
    }

    private logSlideNavigationInfo(followingSlide) {
        LoggingService.trace(RoutingService.name, 'slideNavigation', 'To slide: ' + followingSlide + ' with index: ' + this.currentSlideIndex);
    }

    navigateToSlide(slide: Slide, doNotReturnDelay: boolean = false): number {
        this.router.navigate([slide]);

        // For slides that are always displayed like maintenance
        if (doNotReturnDelay) {
            return 0;
        }

        var duration = this.DEFAULT_DURATION_SEC * 1000;

        if (slide === Slide.Youtube) {
            let slideConfig = ConfigService.config.slides.config[slide];
            let configuredDuration =
                slideConfig.videos[this.cacheService.getIndexForCachedElements(IndexableCache.youtube, slideConfig.videos.length, false)].duration;
            if (configuredDuration) {
                duration = configuredDuration * 1000 + 1500; //adding 1.5sec for transition/loading time
            }
        } else if (slide === Slide.Iframe) {
            let slideConfig = ConfigService.config.slides.config[slide];
            let configuredDuration =
                slideConfig.pages[this.cacheService.getIndexForCachedElements(IndexableCache.iframe, slideConfig.pages.length, false)].duration;
            if (configuredDuration) {
                duration = configuredDuration * 1000 + 1500; //adding 1.5sec for transition/loading time
            }
        } else if (slide === Slide.MonitoringIframe) {
            let slideConfig = ConfigService.config.slides.config[slide];
            let configuredDuration =
                slideConfig.pages[this.cacheService.getIndexForCachedElements(IndexableCache.monitoringiframe, slideConfig.pages.length, false)]
                    .duration;
            if (configuredDuration) {
                duration = configuredDuration * 1000 + 1500; //adding 1.5sec for transition/loading time
            }
        } else if (slide === Slide.Spotlight) {
            let slideConfig = ConfigService.config.slides.config[slide];
            let configuredDuration =
                slideConfig.pages[this.cacheService.getIndexForCachedElements(IndexableCache.spotlight, slideConfig.pages.length, false)].duration;
            if (configuredDuration) {
                duration = configuredDuration * 1000 + 1500; //adding 1.5sec for transition/loading time
            }
        } else if (slide === Slide.LocalVideo) {
            let slideConfig = ConfigService.config.slides.config[slide];
            let configuredDuration =
                slideConfig.videos[this.cacheService.getIndexForCachedElements(IndexableCache.localVideo, slideConfig.videos.length, false)].duration;
            if (configuredDuration) {
                duration = configuredDuration * 1000 + 1500; //adding 1.5sec for transition/loading time
            }
        } else {
            let configuredDuration = ConfigService.config.slides.config[slide].duration;
            if (configuredDuration) {
                duration = configuredDuration * 1000;
            }
        }

        if (!duration || duration <= 0) {
            duration = this.DEFAULT_DURATION_SEC * 1000;
        }

        return duration;
    }

    delay(ms: number) {
        LoggingService.trace(RoutingService.name, 'delay', 'Slide duration: ' + ms / 1000 + 's');
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}
