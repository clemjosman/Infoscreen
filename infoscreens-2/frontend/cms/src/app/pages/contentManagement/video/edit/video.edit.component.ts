import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, FormArray, FormControl, Validators, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { Subscription } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';

import { ConfirmationDialogComponent, IConfirmationDialogData } from '@components/index';

import IStringDictionary from '@interfaces/IStringDictionary';
import { ApiService, DataService, SnackbarService } from '@services/index';
import {
    apiLanguage,
    apiVideo,
    apiVideo_Translate,
    apiVideo_Publish,
    apiInfoscreen_Light,
    apiInfoscreenGroup,
    apiTenant,
    apiCategory
} from '@models/index';
import moment from 'moment';
import { TenantService, TranslationService } from '@vesact/web-ui-template';
import { VideoBackground } from '../../../../../../../common';

import { ApplicationInsightsService } from '../../../../services/app-insights.service';

const YoutubeUrlValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    var value = control.value as string;

    // General checks
    if (
        value &&
        (value.startsWith('https://www.youtube.com/') ||
            value.startsWith('http://www.youtube.com/') ||
            value.startsWith('www.youtube.com/') ||
            value.startsWith('https://youtube.com/') ||
            value.startsWith('http://youtube.com/') ||
            value.startsWith('youtube.com/') ||
            value.startsWith('https://www.youtu.be/') ||
            value.startsWith('http://www.youtu.be/') ||
            value.startsWith('www.youtu.be/') ||
            value.startsWith('https://youtu.be/') ||
            value.startsWith('http://youtu.be/') ||
            value.startsWith('youtu.be/'))
    ) {
        // youtube.com/watch?v=xxxxxxxx
        if (value.includes('youtube.com/watch') && value.includes('v=') && !value.endsWith('v=')) {
            return null;
        }
        // youtu.be/xxxxxxxx
        else if (value.includes('youtu.be/') && !value.endsWith('youtu.be/')) {
            return null;
        }
    }

    return { invalidUrl: 'The given value is not a valid youtube url.' };
};

const PUBLICATION_OPTIONS = {
    publish: 1,
    draft: 2
};

const EXPIRATION_OPTIONS = {
    expire: 1,
    persist: 2
};

@Component({
    templateUrl: './video.edit.component.html',
    styleUrls: ['./video.edit.component.scss']
})
export class VideoEditComponent implements OnInit, OnDestroy {
    public pathArray: string[] = ['menuItem.contentManagement.group', 'menuItem.contentManagement.videos'];

    //INFO: Needing this format for translation pipe
    CONFIG = { MIN_VIDEO_DURATION: 10 };

    readonly VideoBackground = VideoBackground;

    private routeDataSubscription: Subscription = undefined;
    private routeParamSubscription: Subscription = undefined;

    publicationOptions = PUBLICATION_OPTIONS;
    expirationOptions = EXPIRATION_OPTIONS;

    isLoading: boolean = true;
    isNew: boolean = undefined;
    videoId: number = undefined;
    video: apiVideo = undefined;
    infoscreens: apiInfoscreen_Light[] = undefined;
    infoscreenGroups: apiInfoscreenGroup[] = undefined;

    languages: apiLanguage[] = undefined;
    languagesLeftToAdd: apiLanguage[] = undefined;
    selectedNewLanguage: apiLanguage = undefined;

    allCategories: string[] = [];
    categories: string[] = [];

    expandedInfoscreenAssigmentDropdown: boolean = false;

    private editVideoForm: FormGroup = undefined;

    constructor(
        private apiService: ApiService,
        private dataService: DataService,
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        public dialog: MatDialog,
        public tenantService: TenantService<apiTenant>,
        private snackbarService: SnackbarService,
        private _translationService: TranslationService,
        private _appInsightsService: ApplicationInsightsService
    ) {}

    async ngOnInit() {
        try {
            this.isLoading = true;
            let allCategoriesApi: apiCategory[] = undefined;

            [this.languages, this.infoscreens, this.infoscreenGroups, allCategoriesApi] = await Promise.all([
                this.dataService.getLanguagesAsync(),
                this.dataService.getInfoscreensAsync(),
                this.dataService.getInfoscreenGroupsAsync(),
                this.apiService.getCategoriesAsync()
            ]);

            this.allCategories = allCategoriesApi.map(c => c.name);
            this.languagesLeftToAdd = this.languages.slice();
        } catch (error) {
            this.isLoading = false;
        }

        // Subscribe to changes (will be called even if only the parameter changes)
        this.routeDataSubscription = this.route.data.subscribe(async routeData => {
            this.isNew = routeData.isNew;

            if (this.isNew) {
                this.video = null;
                this.pathArray[2] = 'menuItem.contentManagement.newElement';

                // By default, select the ui language as default language
                let defaultLanguage = this.languages.find(l => l.iso2 === this._translationService.currentLanguage.value?.iso2);
                if (!defaultLanguage) {
                    // It can happen that the ui language is not in the content language list, in that case use the first content language of the list
                    defaultLanguage = this.languages[0];
                }
                let defaultMultilingualContentFormItem: FormGroup[] = [this.createMultilingualFormItem(defaultLanguage)];

                // Remove default language from select
                this.removeLanguageFromAddLaguageSelect(defaultLanguage);

                // Form element setup
                this.initEditFormGroup(
                    defaultMultilingualContentFormItem,
                    null,
                    null,
                    null,
                    PUBLICATION_OPTIONS.draft,
                    moment().startOf('day').toISOString(),
                    EXPIRATION_OPTIONS.expire,
                    moment().startOf('day').add(15, 'day').toISOString(),
                    null,
                    true,
                    this.isValidApp(),
                    null,
                    []
                );
                this.isLoading = false;
            } else {
                // For already existing video
                this.pathArray[2] = 'menuItem.contentManagement.editElement';

                this.routeParamSubscription = this.route.paramMap.subscribe(async routeParams => {
                    try {
                        // Get video data
                        this.videoId = parseInt(routeParams.get('videoId'));
                        this._appInsightsService.logPageView('Video Edit', '/contentManagement/videos/' + this.videoId.toString());
                        this.video = await this.dataService.getVideoAsync(this.videoId);

                        // Init multilingual forms for each languages
                        let multilingualContentFormItems: FormGroup[] = [];
                        let videoLanguages = Object.getOwnPropertyNames(this.video.title);
                        videoLanguages.forEach(videoLanguage => {
                            let language = this.languages.find(l => l.cultureCode === videoLanguage);
                            if (language) {
                                multilingualContentFormItems.push(this.createMultilingualFormItem(language, this.video.title[videoLanguage]));
                                this.removeLanguageFromAddLaguageSelect(language);
                            }
                        });

                        // Form element setup
                        this.initEditFormGroup(
                            multilingualContentFormItems,
                            this.video.url,
                            this.video.duration,
                            this.video.background,
                            this.video.isVisible ? PUBLICATION_OPTIONS.publish : PUBLICATION_OPTIONS.draft,
                            this.video.publicationDate,
                            this.video.expirationDate ? EXPIRATION_OPTIONS.expire : EXPIRATION_OPTIONS.persist,
                            this.video.expirationDate,
                            this.video.assignedToInfoscreenIds,
                            this.video.isForInfoscreens,
                            this.video.isForApp,
                            this.video.description,
                            this.video.categories.map(c => c.name)
                        );
                        this.isLoading = false;
                    } catch (error) {
                        if (error.error) {
                            if (error.error.exceptionMessageLabel == 'error.videoNotFound') {
                                this.snackbarService.displayErrorSnackbar('video.notFound');
                            }
                            this.router.navigateByUrl('/contentManagement/videos');
                        }
                    }
                });
            }
        });
    }

    ngOnDestroy() {
        if (this.routeDataSubscription) {
            this.routeDataSubscription.unsubscribe();
        }
        if (this.routeParamSubscription) {
            this.routeParamSubscription.unsubscribe();
        }
    }

    initEditFormGroup(
        multilingualContent: FormGroup[],
        url: string,
        duration: number,
        background: VideoBackground | null,
        publication: number,
        publicationDate: string,
        expiration: number,
        expirationDate: string,
        assignedInfoscreenIds: number[],
        isForInfoscreens: boolean,
        isForApp: boolean,
        description: string,
        categories: string[]
    ) {
        this.categories = categories;

        // Create form item
        this.editVideoForm = this.formBuilder.group({
            multilingualContent: new FormArray(multilingualContent),
            url: new FormControl(url, [Validators.required, YoutubeUrlValidator]),
            duration: new FormControl(duration, [Validators.required, Validators.min(this.CONFIG.MIN_VIDEO_DURATION)]),
            background: new FormControl(background || 'none'), // Default to string 'none' to avoid display issue if null is provided
            publication: new FormControl(publication, [Validators.required]),
            publicationDate: new FormControl(publicationDate, [Validators.required]),
            expiration: new FormControl(expiration),
            expirationDate: new FormControl(expirationDate),
            isForInfoscreens: new FormControl(isForInfoscreens),
            isForApp: new FormControl({
                value: isForApp,
                disabled: !this.isValidApp()
            }),
            assignedInfoscreenIds: new FormControl(assignedInfoscreenIds),
            description: new FormControl(description, [Validators.maxLength(250)])
        });
    }

    //****************************************
    //        MULTILINGUAL FORM ITEM
    //****************************************

    createMultilingualFormItem(language: apiLanguage, title: string = ''): FormGroup {
        return this.formBuilder.group({
            language: language,
            title: new FormControl(title, [Validators.required])
        });
    }

    addMultilingualFormItem(language: apiLanguage, title: string = '') {
        // Check if language is already used, if yes then abort add
        var multilingualContentFormArray = this.editVideoForm.get('multilingualContent') as FormArray;
        if (this.isLanguageAlreadyUsedInMultilingualItems(language, multilingualContentFormArray)) {
            return;
        }

        this.removeLanguageFromAddLaguageSelect(language);
        multilingualContentFormArray.push(this.createMultilingualFormItem(language, title));
    }

    removeMultilingualFormItem(index: number) {
        // If index not in range or will remove last item abort
        var multilingualContentFormArray = this.editVideoForm.get('multilingualContent') as FormArray;
        if (multilingualContentFormArray.value.length < 2 || multilingualContentFormArray.value.length <= index) {
            return;
        }

        var language = multilingualContentFormArray.value[index].language;
        this.addLanguageToAddLaguageSelect(language);
        multilingualContentFormArray.removeAt(index);
    }

    async addAutomaticalyTranslatedMultilingualItem(toLanguage: apiLanguage, multilingualContentIndex: number) {
        // Check if language is already used, if yes then abort add
        var multilingualContentFormArray = this.editVideoForm.get('multilingualContent') as FormArray;
        if (this.isLanguageAlreadyUsedInMultilingualItems(toLanguage, multilingualContentFormArray)) {
            return;
        }

        this.isLoading = true;
        // Translate video
        var multilingualContent = multilingualContentFormArray.value[multilingualContentIndex];
        var videoToTranslate = new apiVideo_Translate(multilingualContent.language.iso2, toLanguage.iso2, multilingualContent.title);

        try {
            var translatedVideo = await this.apiService.translateVideoAsync(videoToTranslate);

            // Create new language from translation
            this.removeLanguageFromAddLaguageSelect(toLanguage);
            multilingualContentFormArray.push(this.createMultilingualFormItem(toLanguage, translatedVideo.title));
        } catch (error) {
        } finally {
            this.isLoading = false;
        }
    }

    isLanguageAlreadyUsedInMultilingualItems(language: apiLanguage, multilingualContentFormArray: FormArray): boolean {
        return multilingualContentFormArray.value.find(mlc => mlc.language.id === language.id);
    }

    //****************************************
    //         NOTIFICATIONS
    //****************************************

    isTodayOrBefore(date: string): boolean {
        return moment.utc(date).isSameOrBefore(moment.utc(), 'day');
    }

    isForApp(): boolean {
        return !!this.editVideoForm?.value?.isForApp;
    }

    isValidApp(): boolean {
        return ['MyActemium', 'MyAxians', 'MyETAVIS'].includes(this.tenantService.currentTenant.appName);
    }

    //****************************************
    //         ADD LANGUAGE SELECT
    //****************************************

    addLanguageToAddLaguageSelect(language: apiLanguage) {
        this.languagesLeftToAdd.push(language);
        this.languagesLeftToAdd = this.languagesLeftToAdd.sort((a, b) =>
            a.displayName[this._translationService.currentLanguage.value?.cultureName] >
            b.displayName[this._translationService.currentLanguage.value?.cultureName]
                ? 1
                : -1
        );
        this.selectedNewLanguage = this.languagesLeftToAdd[0];
    }

    removeLanguageFromAddLaguageSelect(language: apiLanguage) {
        this.languagesLeftToAdd = this.languagesLeftToAdd.filter(l => l.id !== language.id);
        this.languagesLeftToAdd = this.languagesLeftToAdd.sort((a, b) =>
            a.displayName[this._translationService.currentLanguage.value?.cultureName] >
            b.displayName[this._translationService.currentLanguage.value?.cultureName]
                ? 1
                : -1
        );

        if (this.languagesLeftToAdd.length > 0) {
            this.selectedNewLanguage = this.languagesLeftToAdd[0];
        }
    }

    //****************************************
    //         INFOSCREEN ASSIGMNENT
    //****************************************

    isInfoscreenAssigned(infoscreenId: number): boolean {
        if (this.video && this.video.assignedToInfoscreenIds.find(id => id == infoscreenId)) {
            return true;
        }
        return false;
    }

    showInfoscreenAssignmentCheckboxes() {
        var checkboxes = document.getElementById('infoscreen-assignment-checkboxes');
        if (this.expandedInfoscreenAssigmentDropdown) {
            checkboxes.style.display = 'none';
        } else {
            checkboxes.style.display = 'block';
        }
        this.expandedInfoscreenAssigmentDropdown = !this.expandedInfoscreenAssigmentDropdown;
    }

    getInfoscreen(infoscreenId: number): apiInfoscreen_Light {
        return this.infoscreens.find(i => i.id == infoscreenId);
    }
    getInfoscreensOfGroup(groupId: number): apiInfoscreen_Light[] {
        return this.infoscreens.filter(i => i.infoscreenGroupId == groupId);
    }

    isGroupChecked(groupId: number): boolean {
        var infoscreenIdsOfGroup = this.infoscreens.filter(i => i.infoscreenGroupId == groupId).map(i => i.id);
        var selectedInfoscreensOfGroup = this.editVideoForm.controls['assignedInfoscreenIds'].value?.filter(id => infoscreenIdsOfGroup.includes(id));
        return selectedInfoscreensOfGroup && selectedInfoscreensOfGroup.length == infoscreenIdsOfGroup.length;
    }

    isGroupIndeterminate(groupId: number): boolean {
        var infoscreenIdsOfGroup = this.infoscreens.filter(i => i.infoscreenGroupId == groupId).map(i => i.id);
        var selectedInfoscreensOfGroup = this.editVideoForm.controls['assignedInfoscreenIds'].value?.filter(id => infoscreenIdsOfGroup.includes(id));
        return selectedInfoscreensOfGroup && selectedInfoscreensOfGroup.length > 0 && selectedInfoscreensOfGroup.length < infoscreenIdsOfGroup.length;
    }

    groupChange(groupId: number, checked: boolean): void {
        var infoscreenIdsOfGroup = this.infoscreens.filter(i => i.infoscreenGroupId == groupId).map(i => i.id);
        if (checked) {
            var infoscreenIdsToAdd = infoscreenIdsOfGroup.filter(id => !this.editVideoForm.controls['assignedInfoscreenIds'].value?.includes(id));
            this.editVideoForm.controls['assignedInfoscreenIds'].setValue(
                this.editVideoForm.controls['assignedInfoscreenIds'].value?.concat(infoscreenIdsToAdd) || infoscreenIdsToAdd
            );
        } else {
            this.editVideoForm.controls['assignedInfoscreenIds'].setValue(
                this.editVideoForm.controls['assignedInfoscreenIds'].value?.filter(id => !infoscreenIdsOfGroup.includes(id))
            );
        }
    }

    isInfoscreenChecked(infoscreenId: number): boolean {
        var result = this.editVideoForm.controls['assignedInfoscreenIds'].value?.find(id => infoscreenId == id);
        return !!result;
    }

    infoscreenChange(infoscreenId: number, checked: boolean): void {
        if (checked) {
            this.editVideoForm.controls['assignedInfoscreenIds'].setValue(
                this.editVideoForm.controls['assignedInfoscreenIds'].value?.concat([infoscreenId]) || [infoscreenId]
            );
        } else {
            this.editVideoForm.controls['assignedInfoscreenIds'].setValue(
                this.editVideoForm.controls['assignedInfoscreenIds'].value?.filter(id => id != infoscreenId)
            );
        }
    }

    //****************************************
    //                 SUBMIT
    //****************************************

    async onSubmit(data) {
        if (this.editVideoForm.invalid) {
            this.snackbarService.displayErrorSnackbar('formular.invalid');
            return;
        }

        const saveVideo = async () => {
            try {
                this.isLoading = true;
                var apiVideo_Publish = this.manipulateFromDataForRequest(data);
                await this.publishVideoAsync(apiVideo_Publish);

                this.isLoading = false;
                this.router.navigateByUrl('/contentManagement/videos');

                this.snackbarService.displaySuccessSnackbar('video.save.success');
            } catch (error) {
                this.isLoading = false;
                this.snackbarService.displayErrorSnackbar('video.save.error');
            }
        };

        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            closeOnNavigation: true,
            data: {
                actionTitle: this.isNew ? 'video.submit.new.confirmation.title' : 'video.submit.edit.confirmation.title',
                actionButton: this.isNew ? 'general.button.submit' : 'general.button.save',
                message: this.isNew ? 'video.submit.new.confirmation.message' : 'video.submit.edit.confirmation.message'
            } as IConfirmationDialogData
        });

        dialogRef.afterClosed().subscribe(async proceed => {
            if (proceed) {
                saveVideo();
            }
        });
    }

    manipulateFromDataForRequest(data): apiVideo_Publish {
        var id = null;
        if (!this.isNew) {
            id = this.video.id;
        }

        var title: IStringDictionary<string> = {};
        data.multilingualContent.forEach(c => (title[c.language.iso2] = c.title));

        let isForInfoscreens = data.isForInfoscreens;
        let isForApp = data.isForApp || false;

        var isVisible = data.publication == this.publicationOptions.publish;

        var expirationDate = data.expiration == this.expirationOptions.expire ? data.expirationDate : null;

        let categories = this.categories;

        return new apiVideo_Publish(
            id,
            title,
            data.url,
            data.duration,
            data.background === 'none' ? null : data.background,
            isVisible,
            data.publicationDate,
            expirationDate,
            data.assignedInfoscreenIds || [],
            isForInfoscreens,
            isForApp,
            data.description,
            categories
        );
    }

    async publishVideoAsync(apiVideo_Publish: apiVideo_Publish): Promise<void> {
        return new Promise(async (resolve, reject) => {
            var publishedVideo: apiVideo;

            try {
                if (this.isNew) {
                    publishedVideo = await this.apiService.postVideoAsync(apiVideo_Publish);
                } else {
                    publishedVideo = await this.apiService.putVideoAsync(apiVideo_Publish);
                }
                await this.dataService.getVideoAsync(publishedVideo.id, true, true);
            } catch (error) {
                reject(error);
            }

            resolve();
        });
    }

    //****************************************
    //                 ACTIONS
    //****************************************

    async deleteVideo() {
        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            closeOnNavigation: true,
            data: {
                actionTitle: 'video.delete.confirmation.title',
                actionButton: 'general.button.delete',
                message: 'video.delete.confirmation.message'
            } as IConfirmationDialogData
        });

        dialogRef.afterClosed().subscribe(async proceed => {
            if (proceed) {
                try {
                    this.isLoading = true;
                    await this.apiService.deleteVideoAsync(this.video.id);
                    this.dataService.deleteVideoLocally(this.video.id);

                    this.isLoading = false;
                    this.router.navigateByUrl('/contentManagement/videos');

                    this.snackbarService.displaySuccessSnackbar('video.delete.success');
                } catch (error) {
                    this.isLoading = false;
                }
            }
        });
    }

    async cancelEdit() {
        if (this.editVideoForm.pristine) {
            this.router.navigateByUrl('/contentManagement/videos');
        } else {
            const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
                closeOnNavigation: true,
                data: {
                    actionTitle: this.isNew ? 'video.cancel.new.confirmation.title' : 'video.cancel.edit.confirmation.title',
                    actionButton: this.isNew ? 'general.button.discard' : 'general.button.discard',
                    message: this.isNew ? 'video.cancel.new.confirmation.message' : 'video.cancel.edit.confirmation.messsage'
                } as IConfirmationDialogData
            });

            dialogRef.afterClosed().subscribe(async proceed => {
                if (proceed) {
                    this.router.navigateByUrl('/contentManagement/videos');
                }
            });
        }
    }
}
