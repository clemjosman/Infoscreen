import { Injectable } from '@angular/core';
import { DEFAULT_UI_LANGUAGE } from '@app/app.config';

import { UiLanguage } from '@vesact/web-ui-template';
import { TranslationService } from '@vesact/web-ui-template';

@Injectable({
    providedIn: 'root' // Single instance for all DI
})
export class ConfigService {

    constructor(private _translationService: TranslationService) {}

    public getUiLanguagesAsync(): Promise<UiLanguage[]>
    {
        return new Promise<UiLanguage[]>(async (resolve, reject) => {
            var languages : UiLanguage[];

            try{
                languages = await this._translationService.getUiLanguagesAsync();
            }
            catch(ex)
            {
                console.error(ex);
                reject(ex);
            }
            finally
            {
                if(languages && languages.length > 0)
                {
                    resolve(languages);
                }
                else
                {
                    // Only return default language if nothing was returned from API or exception occured
                    resolve([DEFAULT_UI_LANGUAGE]);
                }
            }
        });
    }
}