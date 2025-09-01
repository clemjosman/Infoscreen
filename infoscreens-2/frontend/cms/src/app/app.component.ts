import { Component, Inject, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Platform } from '@angular/cdk/platform';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';

import { navigation } from '@app/navigation/navigation';

import {
    AuthService_B2C,
    FooterData,
    FuseConfigService,
    FuseNavigationService,
    FuseSidebarService,
    FuseSplashScreenService,
    FuseTranslationLoaderService,
    HelpSectionData,
    MaintenanceService,
    MaintenanceStatusDisplay,
    TenantService,
    TranslationService,
    UiLanguage,
    UiTenant
} from '@vesact/web-ui-template';

import { ConfigService, UserService } from '@services/index';
import { apiUser_Me } from './models';
import { DEFAULT_UI_LANGUAGE } from './app.config';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit, OnDestroy {
    private currentRoute: any = {};
    fuseConfig: any;
    navigation: any;

    languages: UiLanguage[];
    selectedLanguage: UiLanguage;

    tenants: UiTenant[] = [];
    selectedTenant: UiTenant = undefined;

    maintenanceData: MaintenanceStatusDisplay = undefined;
    helpData: HelpSectionData = {
        email: 'support.infoscreen@actemium.cloud',
        //phone: '+41 58 330 0 330',
        //helpLink: 'https://vincienergies.sharepoint.com/sites/ch/tempus'
    }
    footerData: FooterData = {
        copyrights: {
            label: 'vesact',
            url: 'http://www.actemium.ch/'
        },
        links: [
            {
                label: 'impressum',
                route: '/impressum'
            }
        ],
        devMode: true,
        buildDate: '2021-03-31T12:40:14.3984107+00:00',
        releaseName: 'Release-42'
    };


    // Private
    private _unsubscribeAll: Subject<any>;

    /**
   * Constructor
   *
   * @param {DOCUMENT} document
   * @param {FuseConfigService} _fuseConfigService
   * @param {FuseNavigationService} _fuseNavigationService
   * @param {FuseSidebarService} _fuseSidebarService
   * @param {FuseSplashScreenService} _fuseSplashScreenService
   * @param {FuseTranslationLoaderService} _fuseTranslationLoaderService
   * @param {Platform} _platform
   * @param {TranslateService} _translateService
   */
    constructor(
        @Inject(DOCUMENT) private document: any,
        private _fuseConfigService: FuseConfigService,
        private _fuseNavigationService: FuseNavigationService,
        private _fuseSidebarService: FuseSidebarService,
        private _fuseSplashScreenService: FuseSplashScreenService, // DO NOT DELETE - Needed to run constructor of FuseSplashScreenService to remove the splash screen on first routing event
        private _fuseTranslationLoaderService: FuseTranslationLoaderService,
        private _translateService: TranslateService,
        private _userService: UserService,
        private _tenantService: TenantService,
        private _translationService: TranslationService,
        private _platform: Platform,
        private _configService: ConfigService,
        private authService: AuthService_B2C, // DO NOT DELETE - Needed to run constructor of authService before root constructor to init the MSAL.js authService with the active account
        private _maintenanceService: MaintenanceService, // DO NOT DELETE - Needed to run constructor of maintenanceService before root constructor to init the maintenance check interval
    ) {
        // Get default navigation
        this.navigation = navigation;

        // Register the navigation to the service
        this._fuseNavigationService.register('main', this.navigation);

        // Set the main navigation as our current navigation
        this._fuseNavigationService.setCurrentNavigation('main');


        // Default translate service setup from fuse template
        /*
        // Add languages
        this._translateService.addLangs(['en', 'de', 'fr']);

        // Set the default language
        this._translateService.setDefaultLang('en');

        // Set the navigation translations
        this._fuseTranslationLoaderService.loadTranslations(translationEnglish, translationGerman, translationFrench);

        // Use a language
        this._translateService.use('en');
        */



        /**
         * ----------------------------------------------------------------------------------------------------
         * ngxTranslate Fix Start
         * ----------------------------------------------------------------------------------------------------
         */

        /**
         * If you are using a language other than the default one, i.e. Turkish in this case,
         * you may encounter an issue where some of the components are not actually being
         * translated when your app first initialized.
         *
         * This is related to ngxTranslate module and below there is a temporary fix while we
         * are moving the multi language implementation over to the Angular's core language
         * service.
         **/

        // Set the default language to 'en' and then back to 'tr'.
        // '.use' cannot be used here as ngxTranslate won't switch to a language that's already
        // been selected and there is no way to force it, so we overcome the issue by switching
        // the default language back and forth.

        /*setTimeout(() => {
            this._translateService.setDefaultLang('de');
            this._translateService.setDefaultLang('en');
        });*/


        /**
         * ----------------------------------------------------------------------------------------------------
         * ngxTranslate Fix End
         * ----------------------------------------------------------------------------------------------------
         */

        // Add is-mobile class to the body if the platform is mobile
        if (this._platform.ANDROID || this._platform.IOS) {
            this.document.body.classList.add('is-mobile');
        }

        // Set the private defaults
        this._unsubscribeAll = new Subject();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        // Subscribe to config changes
        this._fuseConfigService.config
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((config) => {

                this.fuseConfig = config;

                // Boxed
                if (this.fuseConfig.layout.width === 'boxed') {
                    this.document.body.classList.add('boxed');
                }
                else {
                    this.document.body.classList.remove('boxed');
                }

                // Color theme - Use normal for loop for IE11 compatibility
                for (let i = 0; i < this.document.body.classList.length; i++) {
                    const className = this.document.body.classList[i];

                    if (className.startsWith('theme-')) {
                        this.document.body.classList.remove(className);
                    }
                }

                this.document.body.classList.add(this.fuseConfig.colorTheme);
            });

        // Subscribe to user data change
        this._userService._authenticatedUserObservable
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe(async (newCurrentUser: apiUser_Me) => {
                this.updateLanguageAndSelectedLanguageModels(newCurrentUser);
            })

        // Subscribe to maintenance changes
        this._maintenanceService.displayMaintenanceData_Subject
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((maintenanceStatus) => {
                this.maintenanceData = maintenanceStatus;
            });

        // Subscribe to tenants list changes
        this._tenantService.tenants_Subject
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((tenants) => {
                this.tenants = tenants;
            });

        // Subscribe to tenants list changes
        this._tenantService.currentTenant_Subject
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((currentTenant) => {
                this.selectedTenant = currentTenant;
            });
    }

    /**
     * On destroy
     */
    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
    }
    isFullscreenRoute(): boolean {
        return this.currentRoute.fullscreen === true;
    }

    //****************************************
    //            TENANT SELECTOR
    //****************************************

    onTenantChange = async (tenant: UiTenant) => {
        this._tenantService.changeTenant(tenant, false);
    }


    //****************************************
    //            LANGUAGE SELECTOR
    //****************************************

    updateLanguageAndSelectedLanguageModels = async (currentUser: apiUser_Me) => {
        this.languages = await this._configService.getUiLanguagesAsync();

        if (currentUser) {
            var userSelectedIso2 = currentUser.iso2;
            var newSelectedLanguage = this.languages.find(l => l.iso2 === userSelectedIso2);

            if (!newSelectedLanguage) {
                newSelectedLanguage = DEFAULT_UI_LANGUAGE;
            }

            this.selectedLanguage = newSelectedLanguage;
            this._translationService.use(newSelectedLanguage.iso2);
        }
    }

    onLanguageChange = async (language: UiLanguage) => {
        await this._translationService.use(language.iso2);
    }
}