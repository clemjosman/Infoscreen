import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Theme } from '../../../../common';
import { ConfigService } from './config.service';
import { LoggingService } from './logging.service';

export const DEFAULT_THEME: Theme = Theme.LightDefault;

@Injectable({
    providedIn: 'root'
})
export class ThemeService {
    private static _theme$: BehaviorSubject<Theme> = new BehaviorSubject<Theme>(DEFAULT_THEME);
    public static get theme$() {
        return ThemeService._theme$.asObservable();
    }
    public static get theme(): Theme {
        return ThemeService._theme$.value;
    }

    init() {
        ConfigService.config$.pipe(filter(config => !!config)).subscribe(
            config => {
                const newTheme = config.theme || DEFAULT_THEME;
                if (ThemeService.theme !== newTheme) this.setNewTheme(newTheme);
            },
            err => {
                LoggingService.error(
                    ThemeService.name,
                    'Config subscription',
                    'Could not get the theme configuration from the config file. Using default: ' + DEFAULT_THEME,
                    err
                );
            }
        );
    }

    setNewTheme(theme: Theme) {
        var themeClassPrefix = 'theme-';

        // Removing all theme classes
        var classes = document.body.className.split(' ').filter(function (c) {
            return c.lastIndexOf(themeClassPrefix, 0) !== 0;
        });
        document.body.className = classes.join(' ').trim();

        // Adding new theme class
        document.body.classList.add(`${themeClassPrefix}${theme}`);

        // Emitting new class
        ThemeService._theme$.next(theme);
    }
}
