import { Component } from '@angular/core';
import { Slide } from '../../../../../common';
import { LoggingService } from '../../services/logging.service';

@Component({
    templateUrl: '../iframe/iframe.html',
    styleUrls: ['../iframe/iframe.scss']
})
export class CoffeeComponent {
    slide: Slide = Slide.Coffee;
    bannerTitle: string = undefined;
    iframeSource: string;

    constructor() {
        try {
            this.iframeSource = 'https://vesactsmartcoffeepoc.z6.web.core.windows.net/#/display/coffeeMachine/150/white';
            LoggingService.debug(CoffeeComponent.name, 'Preparing slide', 'Done');
        } catch (error) {
            LoggingService.error(CoffeeComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
