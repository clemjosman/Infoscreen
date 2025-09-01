import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TranslationPipe } from '../../pipes/translation.pipe';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../common';
import { UptownMenu } from '../../models';

@Component({
    templateUrl: './uptown-menu.html',
    styleUrls: ['./uptown-menu.scss']
})
export class UptownMenuComponent {
    slide: Slide = Slide.UptownMenu;
    title: string = undefined;
    base64Pdf: string = undefined;

    constructor(private http: HttpClient, private translationPipe: TranslationPipe) {
        try {
            this.translationPipe.getTranslationFromFileAsync('title.uptownmenu').then(text => {
                if (!this.title) this.title = text;
            });

            this.http.get('cache/' + LocalCacheFileName.UptownMenu).subscribe(async (data: UptownMenu) => {
                this.base64Pdf = data.base64Pdf;
                LoggingService.debug(UptownMenuComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(UptownMenuComponent.name, 'Preparing slide', 'An error occured while getting slide data', error);
        }
    }
}
