import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IndexableCache } from '../../enums/indexableCache';
import { ImageHelper } from '../../helpers/Images.helper';
import { CacheService } from '../../services/cache.service';
import { DateService } from '../../services/date.service';
import { LoggingService } from '../../services/logging.service';
import { ActemiumNews, LocalCacheFileName, Slide } from '../../../../../common';
import { JobOffersJob } from '../../models';

@Component({
    templateUrl: './jobOffers.html',
    styleUrls: ['./jobOffers.scss']
})
export class JobOffersComponent {
    readonly DEFAULT_IMAGE_JOB = 'assets/images/JobOffers_Default_Image.jpg';
    slide: Slide = Slide.JobOffers;
    jobImageLink: string = this.DEFAULT_IMAGE_JOB;
    job: JobOffersJob;

    constructor(private http: HttpClient, private cacheService: CacheService, private dateService: DateService, private imageHelper: ImageHelper) {
        try {
            this.http.get('cache/' + LocalCacheFileName.JobOffers).subscribe(async (data: JobOffersJob[]) => {
                var numberOfJobs = Object.keys(data).length;
                var tmpJob: JobOffersJob = data[this.cacheService.getIndexForCachedElements(IndexableCache.joboffers, numberOfJobs)];
                tmpJob.publishedAt = tmpJob.publishedAt ? this.dateService.formatDatetoLocal(tmpJob.publishedAt, 'L') : null;
                this.job = tmpJob;

                LoggingService.debug(JobOffersComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(JobOffersComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
