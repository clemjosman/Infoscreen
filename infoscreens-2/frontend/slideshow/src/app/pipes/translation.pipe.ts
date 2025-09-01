import { Pipe, PipeTransform, Injector } from '@angular/core';
import { HttpClient, HttpXhrBackend } from '@angular/common/http';
import { LocalCacheFileName, NodeConfig_Sync, SlideshowLanguage } from '../../../../common';

@Pipe({ name: 'translateAsync' })
export class TranslationPipe implements PipeTransform {
    transform(textCode: string) {
        return this.getTranslationFromFileAsync(textCode).then(text => {
            return text;
        });
    }

    async getTranslationFromFileAsync(textCode) {
        const http = new HttpClient(new HttpXhrBackend({ build: () => new XMLHttpRequest() }));
        var config = await http.get<NodeConfig_Sync>('cache/' + LocalCacheFileName.Config).toPromise();
        var language = config.language;
        var texts;

        if (language === SlideshowLanguage.DE_CH) {
            texts = await http.get('assets/translations/de.json').toPromise();
        } else if (language === SlideshowLanguage.FR_CH) {
            texts = await http.get('assets/translations/fr.json').toPromise();
        } else if (language === SlideshowLanguage.IT_CH) {
            texts = await http.get('assets/translations/it.json').toPromise();
        } else {
            texts = await http.get('assets/translations/en.json').toPromise();
        }

        var translatedText = texts[textCode];

        if (translatedText == undefined || translatedText == null) return textCode;
        else return translatedText;
    }
}
