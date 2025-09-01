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
    templateUrl: './university.html',
    styleUrls: ['./university.scss']
})
export class UniversityComponent {
    slide: Slide = Slide.University;
    courseImageSource: string = 'assets/images/University.jpg';
    title: string = undefined;
    course: any = undefined;

    constructor(
        private http: HttpClient,
        private cacheService: CacheService,
        private dateService: DateService,
        private translationPipe: TranslationPipe,
        private imageHelper: ImageHelper
    ) {
        try {
            this.translationPipe.getTranslationFromFileAsync('title.university').then(text => {
                if (!this.title) this.title = text;
            });

            this.http.get('cache/' + LocalCacheFileName.University).subscribe(async data => {
                var universityData: any = data;
                universityData = universityData.map(c => {
                    c.date = this.createDate(c.startDate, c.endDate);
                    return c;
                });

                var numberOfCourses = Object.keys(universityData).length;
                this.course = universityData[this.cacheService.getIndexForCachedElements(IndexableCache.university, numberOfCourses)];

                if (!this.course) return;

                this.course.city = this.course.location.city;
                this.course.address = this.course.location.address;

                if (this.course.imageSrc) {
                    if (await this.imageHelper.isImageUrlValidAsync(this.course.imageSrc)) this.courseImageSource = this.course.imageSrc;
                }
                LoggingService.debug(UniversityComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(UniversityComponent.name, 'Preparing slide', 'An error occured while getting slide data', error);
        }
    }

    createDate(startDate, endDate) {
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
