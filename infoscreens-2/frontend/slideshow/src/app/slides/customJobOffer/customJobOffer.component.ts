import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CacheService } from '../../services/cache.service';
import { IndexableCache } from '../../enums/indexableCache';
import { ImageHelper } from '../../helpers/Images.helper';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../common';

@Component({
    templateUrl: './customJobOffer.html',
    styleUrls: ['./customJobOffer.scss']
})
export class CustomJobOfferComponent {
    DEFAULT_IMAGE_JOBOFFER = 'assets/images/JobOffers_Default_Image.jpg';
    slide: Slide = Slide.CustomJobOffer;
    jobOfferImageLink: string = this.DEFAULT_IMAGE_JOBOFFER;

    customJobOffer: {
        title: string;
        description: string;
        imgSrc: string;
        skills: string[];
        contact: {
            company: string;
            name: string;
            email: string;
            phone: string;
            address: string;
            website: string;
        };
    };
    skillsColumn1: string[] = [];
    skillsColumn2: string[] = [];

    constructor(private http: HttpClient, private cacheService: CacheService, private imageHelper: ImageHelper) {
        try {
            this.customJobOffer = {
                title: null,
                description: null,
                imgSrc: null,
                skills: [],
                contact: {
                    company: null,
                    name: null,
                    email: null,
                    phone: null,
                    address: null,
                    website: null
                }
            };

            this.http.get('cache/' + LocalCacheFileName.CustomJobOffer).subscribe(async data => {
                var numberOfJobOffers = Object.keys(data).length;
                this.customJobOffer = data[this.cacheService.getIndexForCachedElements(IndexableCache.joboffers, numberOfJobOffers)];

                if (this.customJobOffer?.skills?.length > 0) {
                    // Taking 4 first elements in the first column
                    this.skillsColumn1 = [...this.customJobOffer.skills];
                    this.skillsColumn1.splice(Math.min(this.customJobOffer.skills.length, 4));

                    if (this.customJobOffer.skills.length > 4) {
                        // Taking elements from 5 to 8 in the second column
                        this.skillsColumn2 = [...this.customJobOffer.skills];
                        this.skillsColumn2.splice(0, 4);
                        this.skillsColumn2.splice(Math.min(this.customJobOffer.skills.length - 4, 4));
                    }
                }

                if (this.customJobOffer.imgSrc) {
                    if (await this.imageHelper.isImageUrlValidAsync(this.customJobOffer.imgSrc)) this.jobOfferImageLink = this.customJobOffer.imgSrc;
                }

                LoggingService.debug(CustomJobOfferComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(CustomJobOfferComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
