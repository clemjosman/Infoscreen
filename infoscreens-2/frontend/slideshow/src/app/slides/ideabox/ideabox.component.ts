import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CacheService } from '../../services/cache.service';
import { IndexableCache } from '../../enums/indexableCache';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../common';
import { Idea } from '../../models';

@Component({
    templateUrl: './ideabox.html',
    styleUrls: ['./ideabox.scss']
})
export class IdeaboxComponent {
    slide: Slide = Slide.Ideabox;
    ideaImageSource: string = 'assets/images/IdeaBox_cropped.jpg';
    idea: Idea | undefined;

    constructor(private http: HttpClient, private cacheService: CacheService) {
        try {
            this.http.get('cache/' + LocalCacheFileName.Ideabox).subscribe((data: Idea[]) => {
                var numberOfIdeas = Object.keys(data).length;
                this.idea = data[this.cacheService.getIndexForCachedElements(IndexableCache.ideaBox, numberOfIdeas)];
                LoggingService.debug(IdeaboxComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(IdeaboxComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
