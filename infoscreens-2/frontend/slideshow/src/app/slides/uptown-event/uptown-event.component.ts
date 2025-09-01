import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment-timezone';
import { IndexableCache } from '../../enums/indexableCache';
import { SlideContentType } from '../../enums/slideContentType';
import { ImageHelper } from '../../helpers/Images.helper';
import { TranslationPipe } from '../../pipes/translation.pipe';
import { CacheService } from '../../services/cache.service';
import { DateService } from '../../services/date.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../common';

@Component({
    templateUrl: './uptown-event.html',
    styleUrls: ['./uptown-event.scss']
})
export class UptownEventComponent {
    slide: Slide = Slide.UptownEvent;
    eventImageSource: string = 'assets/images/banner/uptownBanner.jpg';
    title: string = undefined;
    event: any = undefined;

    constructor(
        private http: HttpClient,
        private cacheService: CacheService,
        private dateService: DateService,
        private translationPipe: TranslationPipe,
        private imageHelper: ImageHelper
    ) {
        try {
            this.translationPipe.getTranslationFromFileAsync('title.uptownevent').then(text => {
                if (!this.title) this.title = text;
            });

            this.http.get('cache/' + LocalCacheFileName.UptownEvent).subscribe(async data => {
                var eventData: any = data;
                eventData = eventData.map(e => {
                    e.date = this.createDate(e.starting, e.ending);
                    return e;
                });

                var numberOfEvents = Object.keys(eventData).length;
                this.event = eventData[this.cacheService.getIndexForCachedElements(IndexableCache.uptownEvent, numberOfEvents)];

                if (!this.event) return;

                if (this.event.picture) {
                    if (await this.imageHelper.isImageUrlValidAsync(this.event.picture)) this.eventImageSource = this.event.picture;
                }
                LoggingService.debug(UptownEventComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(UptownEventComponent.name, 'Preparing slide', 'An error occured while getting slide data', error);
        }
    }

    createDate(startDate: string, endDate: string) {
        var start = moment(startDate);
        var end = moment(endDate);

        if (start.isSame(end, 'date')) {
            return this.dateService.formatDatetoLocal(startDate, 'L HH:mm') + ' - ' + this.dateService.formatDatetoLocal(endDate, 'HH:mm');
        }

        if (start.isSame(end, 'month')) {
            return (
                this.dateService.formatDatetoLocal(startDate, 'DD' + this.dateService.getLocalSeparator()) +
                ' - ' +
                this.dateService.formatDatetoLocal(endDate, 'L')
            );
        }

        return this.dateService.formatDatetoLocal(startDate, 'L') + ' - ' + this.dateService.formatDatetoLocal(endDate, 'L');
    }
}
