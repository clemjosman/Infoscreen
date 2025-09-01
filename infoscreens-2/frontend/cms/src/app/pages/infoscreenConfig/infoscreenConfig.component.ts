import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { CdkDragDrop, moveItemInArray, CdkDrag } from '@angular/cdk/drag-drop';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

import {
    BannerStyle,
    DataEndpointConfig,
    DeviceSleepConfig,
    FooterStyle,
    LocalCacheConfig,
    NodeConfig,
    Slide,
    SlideConfigWrapper,
    Theme
} from '../../../../../common';
import { ApiService, DataService, SnackbarService } from '@services/index';
import {
    apiInfoscreen_ConfigUpdate,
    apiInfoscreen_Light,
    apiInfoscreen_MetdaDataUpdate,
    apiInfoscreen_Status,
    apiLanguage,
    apiTenant
} from '@app/models';
import { TenantService, TranslationService } from '@vesact/web-ui-template';
import { SelectAddSlideComponent } from './select-add-slide/select-add-slide.component';
import moment from 'moment';

import { ApplicationInsightsService } from '../../services/app-insights.service';

const DEFAULT_HOURS_START = 20;
const DEFAULT_HOURS_END = 6;
const DEFAULT_HOURS = (timeRangePart: 'start' | 'end') => {
    return timeRangePart === 'start' ? DEFAULT_HOURS_START : DEFAULT_HOURS_END;
};
const DEFAULT_MINUTES = 0;

@Component({
    templateUrl: './infoscreenConfig.component.html',
    styleUrls: ['./infoscreenConfig.component.scss']
})
export class InfoscreenConfigComponent implements OnInit, OnDestroy {
    private _queryParamsSubscription: Subscription;
    public pathArray: string[] = ['menuItem.infoscreens', 'menuItem.infoscreenConfig'];

    readonly Theme = Theme;
    readonly BannerStyle = BannerStyle;
    readonly FooterStyle = FooterStyle;

    public isLoading: boolean = true;
    public infoscreenId: number = undefined;
    public infoscreen: apiInfoscreen_Light = undefined;
    public infoscreenConfig: NodeConfig = undefined;
    public infoscreenStatus: apiInfoscreen_Status = undefined;
    public languages: apiLanguage[] = undefined;

    public sociabbleSources = [
        { displayName: 'Actemium', fileName: 'sociabbleActemium.json' },
        { displayName: 'Axians', fileName: 'sociabbleAxians.json' },
        { displayName: 'ETAVIS', fileName: 'sociabbleEtavis.json' }
    ];

    public metadataForm: FormGroup = null;
    private _metadataForm_initialValues: any = undefined;

    public configForm: FormGroup = null;
    private _configForm_initialValues: any = undefined;

    constructor(
        private _dataService: DataService,
        private _route: ActivatedRoute,
        private _router: Router,
        private _formBuilder: FormBuilder,
        private _apiService: ApiService,
        private _snackbarService: SnackbarService,
        private _dialog: MatDialog,
        public tenantService: TenantService<apiTenant>,
        private _translateService: TranslateService,
        public translationService: TranslationService,
        private _appInsightsService: ApplicationInsightsService
    ) {}

    async ngOnInit() {
        this._queryParamsSubscription = this._route.params.subscribe(async params => {
            try {
                this.infoscreenId = parseInt(params.id);
                this._appInsightsService.logPageView('Infoscreen Config', '/infoscreen/' + this.infoscreenId.toString() + '/config');
                this.refresh();
            } catch (err) {
                this.navigateBack();
            }
        });
    }

    private _updatePathArray() {
        if (this.infoscreen?.displayName) {
            this.pathArray[1] = this.infoscreen.displayName;
        } else {
            this.pathArray[1] = 'menuItem.infoscreenConfig';
        }
    }

    ngOnDestroy() {
        if (this._queryParamsSubscription) {
            this._queryParamsSubscription.unsubscribe();
        }
    }

    public async refresh(): Promise<void> {
        try {
            this.isLoading = true;

            this.infoscreen = undefined;
            this.infoscreenConfig = undefined;
            this.metadataForm = undefined;

            [this.infoscreen, this.infoscreenConfig, this.languages] = await Promise.all([
                this._dataService.getInfoscreenAsync(this.infoscreenId, true),
                this._dataService.getInfoscreenConfigAsync(this.infoscreenId),
                this._dataService.getLanguagesAsync()
            ]);

            this._updatePathArray();
            this._setMetadataForm();
            this._setConfigForm();

            this.isLoading = false;
        } catch (err) {
            this.isLoading = false;
            this.navigateBack();
            return;
        }

        // Prone to error if MSB API is called too often,
        // Extracted it from try/catch to not make it critical for the page
        this._dataService.getInfoscreensStatusAsync(false).then(status => (this.infoscreenStatus = status.find(s => s.id === this.infoscreenId)));
    }

    public navigateBack(): void {
        this._router.navigateByUrl('/infoscreens');
    }

    public getMatchingLanguageObject(iso2: string): apiLanguage {
        return this.languages?.find(l => l.iso2 === iso2);
    }

    public force2CharactersOnChange($event): void {
        let currentValue = $event.target.value;
        let newValue = this.formatTo2Characters(currentValue);
        if (currentValue.toString() !== newValue) $event.target.value = newValue;
    }

    public formatTo2Characters(number: number | string): string {
        number = parseInt(number.toString());
        if (number >= 10) return number.toString();
        else if (number >= 1) return '0' + number.toString();
        else return '00';
    }

    public contentAdminEmailPlaceholder(): string {
        if (this.tenantService.currentTenant.contentAdminEmail) {
            return `${this.tenantService.currentTenant.contentAdminEmail} (${this._translateService.instant('label.infoscreen.tenantAdmin')})`;
        }
        return this._translateService.instant('label.infoscreen.contentAdminPlaceholder');
    }

    //*******************************
    //     METADATA
    //*******************************

    private _setMetadataForm(): void {
        this.metadataForm = this._formBuilder.group({
            displayName: new FormControl(this.infoscreen.displayName, [Validators.required, Validators.maxLength(200)]),
            description: new FormControl(this.infoscreen.description, [Validators.maxLength(500)]),
            defaultContentLanguage: new FormControl(this.infoscreen.language.id, [Validators.required]),
            sendMailNoContent: new FormControl(!!this.infoscreen.sendMailNoContent),
            contentAdminEmail: new FormControl(this.infoscreen.contentAdminEmail, [Validators.email])
        });
        this._metadataForm_initialValues = this.metadataForm.value;
    }

    // Using arrow function to keep scope to this component while being executed in the edit-card component
    public cancelMetadata: () => Promise<void> = async () => {
        this._appInsightsService.logEvent('CancelEditInfoscreenMetadata');
        this.metadataForm.reset(this._metadataForm_initialValues);
    };

    public saveMetadata: () => Promise<void> = async () => {
        this._appInsightsService.logEvent('SaveEditInfoscreenMetadata');
        if (this.metadataForm.valid) {
            this.isLoading = true;
            let metadata: apiInfoscreen_MetdaDataUpdate = {
                displayName: this.metadataForm.controls['displayName'].value,
                description: this.metadataForm.controls['description'].value,
                defaultLanguageId: this.metadataForm.controls['defaultContentLanguage'].value,
                sendMailNoContent: this.metadataForm.controls['sendMailNoContent'].value || false,
                contentAdminEmail: this.metadataForm.controls['contentAdminEmail'].value || ''
            };
            this.infoscreen = await this._apiService.updateInfoscreenMetadataAsync(this.infoscreenId, metadata);
            this._dataService.updateLocalInfoscreen(this.infoscreen);
            this._setMetadataForm();
            this._updatePathArray();
            this.isLoading = false;
        }
    };

    //*******************************
    //     CONFIG
    //*******************************
    @ViewChild('configFormNative') configFormNative: ElementRef<HTMLFormElement> = undefined;

    private _setConfigForm(): void {
        this._resetVariables();
        console.log();
        this.configForm = this._formBuilder.group({
            sociabbleSources: new FormControl(this.sociabbleSources, [Validators.required]),
            language: new FormControl(this.infoscreenConfig.frontendConfig.language, [Validators.required]),
            timezone: new FormControl(this.infoscreenConfig.frontendConfig.timezone),
            disableAnimations: new FormControl(!!this.infoscreenConfig.frontendConfig.disableAnimations),
            theme: new FormControl(this.infoscreenConfig.frontendConfig.theme, [Validators.required]),
            bannerStyle: new FormControl(this.infoscreenConfig.frontendConfig.bannerStyle, [Validators.required]),
            footerStyle: new FormControl(this.infoscreenConfig.frontendConfig.footerStyle, [Validators.required]),
            rollingMessage: new FormControl(this.infoscreenConfig.frontendConfig.rollingMessage),
            invertDuoBranding: new FormControl(!!this.infoscreenConfig.frontendConfig.invertDuoBranding),
            sleepConfig: this._generateSleepConfigFormGroup(),
            slides: new FormGroup({}),
            localCache: new FormGroup({}),
            dataEndpointConfig: new FormGroup({})
        });
        this._configForm_initialValues = this.configForm.value;
    }

    // Using arrow function to keep scope to this component while being executed in the edit-card component
    public cancelConfig: () => Promise<void> = async () => {
        this._appInsightsService.logEvent('CancelEditInfoscreenConfig');
        this.configForm.reset(this._configForm_initialValues);
        this._resetVariables();
    };

    private _resetVariables(): void {
        this.showedSlides = this.infoscreenConfig.frontendConfig.slides.order.slice(); // Slice to avoid editing original array
        this.slideConfigs = {
            ...this.infoscreenConfig.frontendConfig.slides.config
        };
        this.dataEndpointConfigs = {
            ...this.infoscreenConfig.backendConfig.dataEndpointConfig
        };
    }

    public saveConfig: () => Promise<void> = async () => {
        this._appInsightsService.logEvent('SaveEditInfoscreenConfig');
        if (this.configForm.valid && this._verifyCusomConfigForms()) {
            this.isLoading = true;
            let config: apiInfoscreen_ConfigUpdate = {
                // Frontend related
                language: this.configForm.controls['language'].value || 'de-CH',
                timezone: this.configForm.controls['timezone'].value || 'Europe/Zurich',
                disableAnimations: !!this.configForm.controls['disableAnimations'].value,
                theme: this.configForm.controls['theme'].value,
                bannerStyle: this.configForm.controls['bannerStyle'].value,
                footerStyle: this.configForm.controls['footerStyle'].value,
                rollingMessage: this.configForm.controls['rollingMessage'].value || '',
                invertDuoBranding: !!this.configForm.controls['invertDuoBranding'].value,
                sleepConfig: this._generateSleepConfig(),
                slides: {
                    order: this.showedSlides,
                    config: this.slideConfigs
                },
                localCache: this._generateLocalCacheConfig(),
                // Backend related
                dataEndpointConfig: this._generateDataEndpointConfig()
            };
            this.infoscreenConfig = await this._apiService.updateInfoscreenConfigAsync(this.infoscreenId, config);
            this._setConfigForm();
            this.isLoading = false;
        }
    };

    private _verifyCusomConfigForms(): boolean {
        return this.configFormNative.nativeElement.reportValidity();
    }

    /** Is to be done later, until then, same config for all infoscreens
     *  Handles the local refresh rates
     */
    private _generateLocalCacheConfig(): LocalCacheConfig {
        let localCacheConfig = {
            refreshRates: { config: 0.5 }
        } as LocalCacheConfig;

        if (this.showedSlides.includes(Slide.Ideabox)) localCacheConfig.refreshRates.ideabox = 30;
        if (this.showedSlides.includes(Slide.InfoscreenMonitoring)) localCacheConfig.refreshRates.infoscreenmonitoring = 15;
        if (this.showedSlides.includes(Slide.CustomJobOffer)) localCacheConfig.refreshRates.customjoboffer = 45;
        if (this.showedSlides.includes(Slide.NewsInternal)) localCacheConfig.refreshRates.newsinternal = 30;
        if (this.showedSlides.includes(Slide.NewsPublic)) localCacheConfig.refreshRates.newspublic = 30;
        if (this.showedSlides.includes(Slide.WeatherDaily) || this.showedSlides.includes(Slide.WeatherWeekly))
            localCacheConfig.refreshRates.openweather = 20;
        if (this.showedSlides.includes(Slide.JobOffers)) localCacheConfig.refreshRates.joboffers = 45;
        if (this.showedSlides.includes(Slide.PublicTransport)) localCacheConfig.refreshRates.publictransport = 20;
        if (this.showedSlides.includes(Slide.Sociabble)) localCacheConfig.refreshRates.sociabble = 30;
        if (this.showedSlides.includes(Slide.TwentyMin)) localCacheConfig.refreshRates.twentymin = 45;
        if (this.showedSlides.includes(Slide.Twitter)) localCacheConfig.refreshRates.twitter = 60;
        if (this.showedSlides.includes(Slide.University)) localCacheConfig.refreshRates.university = 60;
        if (this.showedSlides.includes(Slide.UptownArticle)) localCacheConfig.refreshRates.uptownarticle = 30;
        if (this.showedSlides.includes(Slide.UptownEvent)) localCacheConfig.refreshRates.uptownevent = 30;
        if (this.showedSlides.includes(Slide.UptownMenu)) localCacheConfig.refreshRates.uptownmenu = 30;
        return localCacheConfig;
    }

    private _generateDataEndpointConfig(): DataEndpointConfig {
        let dataEndpointConfig = this.infoscreenConfig.backendConfig.dataEndpointConfig;

        if (this.dataEndpointConfigs.newsInternal?.maxNewsCount !== undefined && this.dataEndpointConfigs.newsInternal?.maxNewsCount !== null) {
            // Adding backend config for this slide if not present as it doesn't require to link a cached file to it
            if (!dataEndpointConfig.newsInternal) {
                dataEndpointConfig.newsInternal = this.dataEndpointConfigs.newsInternal;
            } else {
                dataEndpointConfig.newsInternal.maxNewsCount = this.dataEndpointConfigs.newsInternal.maxNewsCount;
            }
        }

        if (this.dataEndpointConfigs.newsPublic?.maxNewsCount !== undefined && this.dataEndpointConfigs.newsPublic?.maxNewsCount !== null) {
            dataEndpointConfig.newsPublic.maxNewsCount = this.dataEndpointConfigs.newsPublic.maxNewsCount;
        }

        if (this.dataEndpointConfigs.twentyMin?.maxNewsDateAge !== undefined && this.dataEndpointConfigs.twentyMin?.maxNewsDateAge !== null) {
            dataEndpointConfig.twentyMin.maxNewsDateAge = this.dataEndpointConfigs.twentyMin.maxNewsDateAge;
        }

        if (this.showedSlides.includes(Slide.UptownArticle)) {
            dataEndpointConfig.uptownArticle = {
                cachedFileName: 'articles.json',
                cachedFileNames: null
            };
        } else {
            dataEndpointConfig.uptownArticle = undefined;
        }

        if (this.showedSlides.includes(Slide.UptownEvent)) {
            dataEndpointConfig.uptownEvent = {
                cachedFileName: 'events.json',
                cachedFileNames: null
            };
        } else {
            dataEndpointConfig.uptownEvent = undefined;
        }

        if (this.showedSlides.includes(Slide.Sociabble)) {
            dataEndpointConfig.sociabble = {
                cachedFileName: null,
                cachedFileNames: this.slideConfigs.sociabble.fileNames
            };
        } else {
            dataEndpointConfig.sociabble = undefined;
        }

        return dataEndpointConfig;
    }

    //------------------------------------
    //            SLEEP RANGES
    //------------------------------------

    private _generateSleepConfig(): DeviceSleepConfig {
        let sleepControls = this.configForm['controls'].sleepConfig['controls'];
        var daily_start_value = sleepControls.daily['controls'].startTime.value;
        var daily_end_value = sleepControls.daily['controls'].endTime.value;
        var weekend_start_value = sleepControls.weekend['controls'].startTime.value;
        var weekend_end_value = sleepControls.weekend['controls'].endTime.value;

        const timeValuesToUTCStrings = (values: { hours: number; minutes: number }[]): string[] => {
            return values.map(v => {
                var utc = moment(`${v.hours}:${v.minutes}`, 'HH:mm').utc();
                return `${this.formatTo2Characters(utc.hours())}:${this.formatTo2Characters(utc.minutes())}`;
            });
        };

        let daily_start: string, daily_end: string, weekend_start: string, weekend_end: string;
        [daily_start, daily_end, weekend_start, weekend_end] = timeValuesToUTCStrings([
            daily_start_value,
            daily_end_value,
            weekend_start_value,
            weekend_end_value
        ]);

        return {
            daily: {
                startTime: daily_start || '19:00',
                duration: Math.round(this._getDurationFromStartAndEnd(daily_start, daily_end, false)) || 0
            },
            weekend: {
                startTime: weekend_start || '19:00',
                duration: Math.round(this._getDurationFromStartAndEnd(weekend_start, weekend_end, true)) || 0
            }
        };
    }

    private _generateSleepConfigFormGroup(): FormGroup {
        const _parseTime = (time: string, timeRangePart: 'start' | 'end'): { hours: number; minutes: number } => {
            if (!time?.includes(':'))
                return {
                    hours: DEFAULT_HOURS(timeRangePart),
                    minutes: DEFAULT_MINUTES
                };

            var hours_utc = parseInt(time.split(':')[0]) % 24 || DEFAULT_HOURS(timeRangePart);
            var minutes_utc = parseInt(time.split(':')[1]) % 60 || DEFAULT_MINUTES;
            var local = moment.utc(`${hours_utc}:${minutes_utc}`, 'HH:mm').local();

            return {
                hours: parseInt(local.format('HH')),
                minutes: parseInt(local.format('mm'))
            };
        };

        let daily_config = this.infoscreenConfig.frontendConfig?.sleepConfig?.daily;
        let daily_startTime = _parseTime(daily_config?.startTime, 'start');
        let daily_endTime = _parseTime(this._getEndTimeFromStartAndDuration(daily_config?.startTime, daily_config?.duration), 'end');

        let weekend_config = this.infoscreenConfig.frontendConfig?.sleepConfig?.weekend;
        let weekend_startTime = _parseTime(weekend_config?.startTime, 'start');
        let weekend_endTime = _parseTime(this._getEndTimeFromStartAndDuration(weekend_config?.startTime, weekend_config?.duration), 'end');

        const _generateTimeFormGroup = (time: { hours: number; minutes: number }): FormGroup => {
            return new FormGroup({
                hours: new FormControl(this.formatTo2Characters(time.hours), [Validators.max(23), Validators.min(0)]),
                minutes: new FormControl(this.formatTo2Characters(time.minutes), [Validators.max(59), Validators.min(0)])
            });
        };

        return new FormGroup({
            daily: new FormGroup({
                startTime: _generateTimeFormGroup(daily_startTime),
                endTime: _generateTimeFormGroup(daily_endTime)
            }),
            weekend: new FormGroup({
                startTime: _generateTimeFormGroup(weekend_startTime),
                endTime: _generateTimeFormGroup(weekend_endTime)
            })
        });
    }

    public resultingSleepTime(scope: 'daily' | 'weekend'): string {
        let isWeekend = scope === 'weekend';
        let sleepControls = this.configForm['controls'].sleepConfig['controls'];
        var start = sleepControls[scope]['controls'].startTime.value;
        var end = sleepControls[scope]['controls'].endTime.value;

        let duration = this._getDurationFromStartAndEnd(`${start.hours}:${start.minutes}`, `${end.hours}:${end.minutes}`, isWeekend);

        let sleepTime = moment.duration(duration, 'seconds');
        let sleepTime_Hours = sleepTime.days() * 24 + sleepTime.hours();
        let sleepTime_minutes = sleepTime.minutes();
        return `${this.formatTo2Characters(sleepTime_Hours)}:${this.formatTo2Characters(sleepTime_minutes)}`;
    }

    /**
     * Converts a start time and a duration into an end time
     * @param {string} start
     * @param {number} duration
     * @returns {string} Formatted end time (HH:mm:ss)
     */
    private _getEndTimeFromStartAndDuration(start: string, duration: number): string {
        if (!start?.includes(':')) return `${DEFAULT_HOURS('end')}:00`;
        if (!duration) duration = 0;
        var startTime = this._prepareMomentFromTimeString(start, 'start');
        startTime = startTime.add(duration, 'seconds');
        return `${startTime.format('HH:mm')}`;
    }

    /**
     * Returns the duration (seconds) between start and end times
     * @param {string} start Start time (format: HH:mm)
     * @param end End time (format: HH:mm)
     * @returns {number} Seconds between start and end times
     */
    private _getDurationFromStartAndEnd(start: string, end: string, overWeekend: boolean): number {
        // No sleep case same date - also for weekend
        if (start === end) return 0;

        var startTime = this._prepareMomentFromTimeString(start, 'start');
        var endTime = this._prepareMomentFromTimeString(end, 'end');
        if (endTime.isBefore(startTime) && !overWeekend) {
            endTime = endTime.add(1, 'day');
        }

        if (overWeekend) endTime = endTime.add(3, 'days');

        var duration = moment.duration(endTime.diff(startTime));
        return duration.asSeconds();
    }

    /**
     * Assign the hours and minutes to a moment object
     * @param {string} time Format: HH:mm
     * @param {'start' | 'end'} timeRangePart If this is the start of end of the time range (will have different default values)
     * @returns {moment.Moment} Moment object
     */
    private _prepareMomentFromTimeString(time: string, timeRangePart: 'start' | 'end'): moment.Moment {
        if (!time?.includes(':')) return moment().set('hours', DEFAULT_HOURS(timeRangePart)).set('minutes', 0).set('seconds', 0);

        var hours = parseInt(time.split(':')[0]);
        var minutes = parseInt(time.split(':')[1]);
        var momentTime = moment().set('hours', hours).set('minutes', minutes).set('seconds', 0);
        return momentTime;
    }

    //------------------------------------
    //            SLIDE ORDER
    //------------------------------------
    public showedSlides: Slide[] = [];

    dropSlide = (event: CdkDragDrop<Slide>) => {
        moveItemInArray(this.showedSlides, event.previousIndex, event.currentIndex);
    };

    /**
     * Predicate function that avoids having twice the same slide one after another.
     * /!\ Keep in mind that the drag index is from the array before any changes
     *     and the drop index is from the array after the change
     *     They cannot be used in the same 'context'. Therefore we create a
     *     temporary array that is a COPY and check the logic on the temporary result
     */
    sortSlidePredicate = (dropIndex: number, item: CdkDrag<number>) => {
        var dragIndex = item.data;
        var selectedSlide = this.showedSlides[dragIndex];
        var forbiddenNeighbors = this.getForbiddenNeighborSlides(selectedSlide);

        if (dragIndex - dropIndex === 0) return true;

        var { previous, next } = this.getPreviousAndNextIndexes(dropIndex, this.showedSlides);

        var temporaryArray = this.showedSlides.slice();
        moveItemInArray(temporaryArray, dragIndex, dropIndex);

        return !forbiddenNeighbors.includes(temporaryArray[previous]) && !forbiddenNeighbors.includes(temporaryArray[next]);
    };

    cannotBeDragged(slide: Slide, index: number): boolean {
        var { previous, next } = this.getPreviousAndNextIndexes(index, this.showedSlides);
        var forbiddenNeighbors = this.getForbiddenNeighborSlides(this.showedSlides[previous]);
        return forbiddenNeighbors.includes(this.showedSlides[next]);
    }

    getPreviousAndNextIndexes(index: number, array: any[]): { previous: number; next: number } {
        var previousIndex = index > 0 ? index - 1 : array.length - 1;
        var nextIndex = index < array.length - 1 ? index + 1 : 0;
        return {
            previous: previousIndex,
            next: nextIndex
        };
    }

    removeSlide(index: number) {
        var { previous, next } = this.getPreviousAndNextIndexes(index, this.showedSlides);
        var previousSlide = this.showedSlides[previous];
        var forbiddenNeighbors = this.getForbiddenNeighborSlides(previousSlide);

        if (forbiddenNeighbors.includes(this.showedSlides[next]) || this.showedSlides.length <= 3) {
            this._snackbarService.displayErrorSnackbar('infoscreen.slideDelete.forbiddenNeighbors');
        } else {
            let slide = this.showedSlides[index];
            this.showedSlides.splice(index, 1);

            if (this.showedSlides.indexOf(slide) === -1) delete this.slideConfigs[slide];
        }
    }

    getForbiddenNeighborSlides(slide: Slide): Slide[] {
        // Can add more logic to it if needed
        return [slide];
    }

    addSlide() {
        let forbiddenNeighborSlides_FirstSlide = this.getForbiddenNeighborSlides(this.showedSlides[0]);
        let forbiddenNeighborSlides_LastSlide = this.getForbiddenNeighborSlides(this.showedSlides[this.showedSlides.length - 1]);

        let forbiddenNeighborSlides = forbiddenNeighborSlides_FirstSlide.concat(forbiddenNeighborSlides_LastSlide);

        // Removing duplicates
        forbiddenNeighborSlides = forbiddenNeighborSlides.filter((value, index, self) => {
            return self.indexOf(value) === index;
        });

        // SLIDES NEEDING ADMIN CONFIG
        let slidesNeedingAdminConfig: Slide[] = [];
        const dataEndpointConfig = this.infoscreenConfig.backendConfig.dataEndpointConfig;
        if (!dataEndpointConfig.ideabox) slidesNeedingAdminConfig.push(Slide.Ideabox);
        if (!dataEndpointConfig.customJobOffer) slidesNeedingAdminConfig.push(Slide.CustomJobOffer);
        if (!dataEndpointConfig.microservicebus) slidesNeedingAdminConfig.push(Slide.InfoscreenMonitoring);
        if (!dataEndpointConfig.newsPublic) slidesNeedingAdminConfig.push(Slide.NewsPublic);
        if (!dataEndpointConfig.openWeather) slidesNeedingAdminConfig.push(Slide.WeatherDaily, Slide.WeatherWeekly);
        if (!dataEndpointConfig.publicTransport) slidesNeedingAdminConfig.push(Slide.PublicTransport);
        if (!dataEndpointConfig.twentyMin) slidesNeedingAdminConfig.push(Slide.TwentyMin);
        if (!dataEndpointConfig.twitter) slidesNeedingAdminConfig.push(Slide.Twitter);
        if (!dataEndpointConfig.university) slidesNeedingAdminConfig.push(Slide.University);

        // SLIDES CATEGORIES
        let globallyAvailableSlides = [
            Slide.Iframe,
            Slide.NewsInternal,
            Slide.Traffic,
            Slide.WeatherDaily,
            Slide.WeatherWeekly,
            Slide.Twitter,
            Slide.Youtube
        ];
        let swissAvailableSlides = [Slide.TwentyMin, Slide.PublicTransport];
        let vinciAvailableSlides = [Slide.Stock, Slide.Ideabox, Slide.Sociabble];
        let uptownAvailableSlides = [Slide.UptownArticle, Slide.UptownEvent, Slide.UptownMenu];

        // Also called 'Discontinued' slides
        let devOnlySlides = [
            Slide.Coffee,
            Slide.InfoscreenMonitoring,
            Slide.CustomJobOffer,
            Slide.LocalVideo,
            Slide.MonitoringIframe,
            Slide.University,
            Slide.Spotlight
        ];
        let actemiumSpecificSlides = [Slide.NewsPublic, Slide.JobOffers];
        let axiansSpecificSlides = [Slide.JobOffers];
        //let nonSelectableSlides = [
        //  Slide.Maintenance
        //]

        let isDevInfoscreen = this.infoscreen.nodeId.includes('DEV');
        let isActemiumTenant = this.tenantService.currentTenant.code === 'ch.actemium';
        let isAxiansTenant = this.tenantService.currentTenant.code === 'ch.axians';
        let isUptownTenant = this.tenantService.currentTenant.code === 'ch.uptown';
        let isSwiss = isActemiumTenant || isAxiansTenant || isUptownTenant;
        let isVinci = isActemiumTenant || isAxiansTenant || isUptownTenant;
        let isUptown = isActemiumTenant || isAxiansTenant || isUptownTenant;

        // Getting allowed slides
        let allowedSlides = [
            ...globallyAvailableSlides,
            ...(isSwiss ? swissAvailableSlides : []),
            ...(isVinci ? vinciAvailableSlides : []),
            ...(isUptown ? uptownAvailableSlides : []),
            ...(isActemiumTenant ? actemiumSpecificSlides : []),
            ...(isAxiansTenant ? axiansSpecificSlides : []),
            ...(isDevInfoscreen ? devOnlySlides : [])
        ];

        // Ensuring slides only appears once in the array
        allowedSlides = allowedSlides.filter((value, index, self) => self.indexOf(value) === index);

        // Removing slides needing admin config
        allowedSlides = allowedSlides.filter(s => !slidesNeedingAdminConfig.includes(s));

        // Removing slides forbidden due to neighbor
        allowedSlides = allowedSlides.filter(s => !forbiddenNeighborSlides.includes(s));

        // Getting non-used slides
        let slidesNotYetUsed = allowedSlides.filter(s => !this.showedSlides.includes(s));

        const dialogRef = this._dialog.open(SelectAddSlideComponent, {
            width: '350px',
            data: {
                allowedSlides,
                forbiddenNeighborSlides,
                slidesNeedingAdminConfig,
                slidesNotYetUsed
            }
        });

        dialogRef.afterClosed().subscribe((slide: Slide) => {
            if (slide) this.showedSlides.push(slide);

            // Assigning front-end slide config default value if not already available (duration, etc.)
            // Using if/else over switch/case to use Intellisens to identify missing slides
            if (!this.slideConfigs[slide]) {
                if (
                    slide === Slide.Coffee ||
                    slide === Slide.Ideabox ||
                    slide === Slide.InfoscreenMonitoring ||
                    slide === Slide.CustomJobOffer ||
                    slide === Slide.Maintenance ||
                    slide === Slide.NewsInternal ||
                    slide === Slide.NewsPublic ||
                    slide === Slide.JobOffers ||
                    slide === Slide.PublicTransport ||
                    slide === Slide.TwentyMin ||
                    slide === Slide.Sociabble ||
                    slide === Slide.Twitter ||
                    slide === Slide.University ||
                    slide === Slide.UptownArticle ||
                    slide === Slide.UptownEvent ||
                    slide === Slide.UptownMenu ||
                    slide === Slide.WeatherDaily ||
                    slide === Slide.WeatherWeekly
                ) {
                    this.slideConfigs[slide] = { duration: 30 };
                } else if (slide === Slide.Iframe || slide === Slide.MonitoringIframe) {
                    this.slideConfigs[slide] = {
                        pages: []
                    };
                } else if (slide === Slide.LocalVideo) {
                    this.slideConfigs[slide] = {
                        videos: []
                    };
                } else if (slide === Slide.Spotlight) {
                    this.slideConfigs[slide] = {
                        pages: []
                    };
                } else if (slide === Slide.Stock) {
                    this.slideConfigs[slide] = {
                        duration: 30,
                        exchange: {
                            url: 'https://vesactinfoscreens.z6.web.core.windows.net/Stock/currencyExchange1.html'
                        },
                        stock: {
                            url: 'https://vesactinfoscreens.z6.web.core.windows.net/Stock/vinciStock.html'
                        }
                    };
                } else if (slide === Slide.Traffic) {
                    this.slideConfigs[slide] = {
                        duration: 30,
                        gmap: {
                            latitude: 47.50724,
                            longitude: 7.6130904,
                            zoom: 13,
                            apiKey: ''
                        }
                    };
                } else if (slide === Slide.UniversityOverview) {
                    this.slideConfigs[slide] = {
                        duration: 30,
                        page: {
                            url: 'https://app1.edoobox.com/de/vesact?edref=vesact&edtag=DE&edcode=fXj3zbaMFq1NbjA'
                        }
                    };
                } else {
                    console.error(`Slide ${slide} not handled in post add script!`);
                    return;
                }

                // Adding backend config for this slide as it doesn't require to link a cached file to it
                // which would normaly have to be done by an admin in the json config file
                if (slide === Slide.NewsInternal && !this.dataEndpointConfigs.newsInternal) {
                    this.dataEndpointConfigs.newsInternal = { maxNewsCount: 3 };
                }

                this.addSlideConfigEntry(slide);
            }
        });
    }

    //------------------------------------
    //            SLIDE CONFIG
    //------------------------------------
    public slideConfigs: SlideConfigWrapper = {};

    /**
     * Check if a slide requires user entries
     * @param slide Slide that must be checked
     * @returns Boolean representing if the slide requires user entries
     */
    canAddSlideConfigEntry(slide: Slide): boolean {
        return [Slide.Iframe, Slide.MonitoringIframe, Slide.LocalVideo, Slide.Spotlight].includes(slide);
    }

    /**
     * Adds a new entry (like a new link for iframe slide) if possible on the selected slide
     * @param slide Slide that should receive a new entry
     */
    addSlideConfigEntry(slide: Slide) {
        // Filter out slides that do not require entries from user
        if (
            slide === Slide.Coffee ||
            slide === Slide.Ideabox ||
            slide === Slide.InfoscreenMonitoring ||
            slide === Slide.CustomJobOffer ||
            slide === Slide.Maintenance ||
            slide === Slide.NewsInternal ||
            slide === Slide.NewsPublic ||
            slide === Slide.JobOffers ||
            slide === Slide.PublicTransport ||
            slide === Slide.Stock ||
            slide === Slide.Sociabble ||
            slide === Slide.Traffic ||
            slide === Slide.TwentyMin ||
            slide === Slide.Twitter ||
            slide === Slide.University ||
            slide === Slide.UniversityOverview ||
            slide === Slide.UptownArticle ||
            slide === Slide.UptownEvent ||
            slide === Slide.UptownMenu ||
            slide === Slide.WeatherDaily ||
            slide === Slide.WeatherWeekly
        ) {
            return;
        } else if (slide === Slide.Iframe || slide === Slide.MonitoringIframe) {
            this.slideConfigs[slide].pages.push({
                bannerTitle: '',
                duration: 30,
                url: ''
            });
        } else if (slide === Slide.LocalVideo) {
            this.slideConfigs[slide].videos.push({
                duration: 30,
                file: '',
                title: ''
            });
        } else if (slide === Slide.Spotlight) {
            this.slideConfigs[slide].pages.push({
                duration: 30,
                url: ''
            });
        } else {
            console.error(`Slide ${slide} not handled in addSlideConfigEntry!`);
            return;
        }
    }

    /**
     * Delete a user entry (like a link for iframe slide) if possible on the selected slide
     * @param slide Slide that should get one of its entry to be deleted
     * @param index Index of the entry to delete
     */
    deleteConfigEntry(slide: Slide, index: number) {
        // Filter out slides that do not require entries from user
        if (
            slide === Slide.Coffee ||
            slide === Slide.Ideabox ||
            slide === Slide.InfoscreenMonitoring ||
            slide === Slide.CustomJobOffer ||
            slide === Slide.Maintenance ||
            slide === Slide.NewsInternal ||
            slide === Slide.NewsPublic ||
            slide === Slide.JobOffers ||
            slide === Slide.PublicTransport ||
            slide === Slide.Stock ||
            slide === Slide.Sociabble ||
            slide === Slide.Traffic ||
            slide === Slide.TwentyMin ||
            slide === Slide.Twitter ||
            slide === Slide.University ||
            slide === Slide.UniversityOverview ||
            slide === Slide.UptownArticle ||
            slide === Slide.UptownEvent ||
            slide === Slide.UptownMenu ||
            slide === Slide.WeatherDaily ||
            slide === Slide.WeatherWeekly
        ) {
            return;
        } else if (slide === Slide.Iframe || slide === Slide.MonitoringIframe) {
            this.slideConfigs[slide].pages.splice(index, 1);
        } else if (slide === Slide.LocalVideo) {
            this.slideConfigs[slide].videos.splice(index, 1);
        } else if (slide === Slide.Spotlight) {
            this.slideConfigs[slide].pages.splice(index, 1);
        } else {
            console.error(`Slide ${slide} not handled in deleteConfigEntry!`);
            return;
        }
    }

    /**
     * Get the backend config class name of a specific slide
     * @param slide Slide which backend config type should be returned
     * @returns Class name of the backend config type that is associates to the slide
     */
    public getConfigType(slide: Slide): string {
        if (
            slide === Slide.Coffee ||
            slide === Slide.Ideabox ||
            slide === Slide.InfoscreenMonitoring ||
            slide === Slide.CustomJobOffer ||
            slide === Slide.Maintenance ||
            slide === Slide.NewsInternal ||
            slide === Slide.NewsPublic ||
            slide === Slide.JobOffers ||
            slide === Slide.PublicTransport ||
            slide === Slide.TwentyMin ||
            slide === Slide.Twitter ||
            slide === Slide.University ||
            slide === Slide.UptownArticle ||
            slide === Slide.UptownEvent ||
            slide === Slide.UptownMenu ||
            slide === Slide.WeatherDaily ||
            slide === Slide.WeatherWeekly
        ) {
            return 'SimpleSlideConfig';
        } else if (slide === Slide.Iframe || slide === Slide.MonitoringIframe) {
            return 'IframeSlideConfig';
        } else if (slide === Slide.LocalVideo) {
            return 'LocalVideoSlideConfig';
        } else if (slide === Slide.Sociabble) {
            return 'SociabbleSlideConfig';
        } else if (slide === Slide.Spotlight) {
            return 'SpotlightSlideConfig';
        } else if (slide === Slide.Stock) {
            return 'StockSlideConfig';
        } else if (slide === Slide.Traffic) {
            return 'TrafficSlideConfig';
        } else if (slide === Slide.UniversityOverview) {
            return 'UniversityOverviewSlideConfig';
        } else {
            console.error(`Slide ${slide} not handled in addSlideConfigEntry!`);
            return;
        }
    }

    //------------------------------------
    //       DATA ENDPOINT CONFIG
    //------------------------------------
    public dataEndpointConfigs: DataEndpointConfig = {};

    displayNewsInternalDataEndpointConfig(): Slide {
        return this.showedSlides.includes(Slide.NewsInternal) ? Slide.NewsInternal : undefined;
    }

    displayNewsPublicDataEndpointConfig(): Slide {
        return this.showedSlides.includes(Slide.NewsPublic) ? Slide.NewsPublic : undefined;
    }

    displayTwentyMinDataEndpointConfig(): Slide {
        return this.showedSlides.includes(Slide.TwentyMin) ? Slide.TwentyMin : undefined;
    }
}
