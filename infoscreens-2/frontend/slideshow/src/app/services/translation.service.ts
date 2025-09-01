import { Injectable } from '@angular/core';
import { HttpClient, HttpXhrBackend } from '@angular/common/http';
import { LoggingService } from './logging.service';
import { LocalCacheFileName, NodeConfig_Sync, SlideshowLanguage } from '../../../../common';

@Injectable({
    providedIn: 'root'
})
export class TranslationService {
    constructor() {}

    async chooseTranslationAsync(source: {
        [key in SlideshowLanguage]?: string;
    }): Promise<any> {
        try {
            return new Promise(async (resolve, reject) => {
                const http = new HttpClient(new HttpXhrBackend({ build: () => new XMLHttpRequest() }));
                var config = await http.get<NodeConfig_Sync>('cache/' + LocalCacheFileName.Config).toPromise();
                var culture = config.language;
                var iso2 = culture.split('-')[0];

                // Get via full culture
                var attributeRetained = Object.keys(source).find(attribute => attribute === culture);

                // Else get via iso2
                if (!attributeRetained) attributeRetained = Object.keys(source).find(attribute => attribute.split('-')[0] === iso2);

                // Else use first language available
                if (!attributeRetained) attributeRetained = Object.keys(source)[0];

                if (attributeRetained) {
                    resolve(source[attributeRetained]);
                    return;
                } else throw Error('No translation has been found for language ' + iso2 + ' within ' + JSON.stringify(source));
            });
        } catch (error) {
            LoggingService.error(
                TranslationService.name,
                'chooseTranslationAsync',
                'An error occured while choosing a translation',
                error,
                JSON.stringify(source)
            );
        }
    }
}
