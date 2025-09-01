// cms/src/app/pages/contentManagement/news/edit/news.edit.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, FormArray, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { DevicePreviewDialogComponent } from '@components/device-preview-dialog/device-preview-dialog.component';

// Toast UI Editor
import Editor from '@toast-ui/editor';
import '@toast-ui/editor/dist/i18n/de-de';
import '@toast-ui/editor/dist/i18n/fr-fr';
import '@toast-ui/editor/dist/i18n/it-it';

import { ConfirmationDialogComponent, IConfirmationDialogData } from '@components/index';

import IStringDictionary from '@interfaces/IStringDictionary';
import { ApiService, DataService, SnackbarService } from '@services/index';
import {
    apiNews,
    apiLanguage,
    apiNews_Publish,
    apiNews_Translate,
    apiInfoscreen_Light,
    apiAttachment_Published,
    apiInfoscreenGroup,
    apiTenant,
    apiNewsLayoutBox,
    apiCategory
} from '@models/index';
import moment from 'moment';
import { TenantService, TranslationService } from '@vesact/web-ui-template';
import { NewsLayout, NewsLayoutBox, NewsLayoutContentType } from '../../../../../../../common';
import { ApplicationInsightsService } from '../../../../services/app-insights.service';

const SUPPORTED_EXTENSIONS = ['png', 'jpg', 'jpeg', 'gif', 'pdf'];
const MAX_FILE_SIZE_MB = 10;

const PUBLICATION_OPTIONS = {
    publish: 1,
    draft: 2
};

const EXPIRATION_OPTIONS = {
    expire: 1,
    persist: 2
};

@Component({
    selector: 'app-news-edit',
    templateUrl: './news.edit.component.html',
    styleUrls: ['./news.edit.component.scss']
})
export class NewsEditComponent implements OnInit, OnDestroy {
    public pathArray: string[] = ['menuItem.contentManagement.group', 'menuItem.contentManagement.news', '...'];

    private routeDataSubscription: Subscription = undefined;
    private routeParamSubscription: Subscription = undefined;
    private languageSubscription: Subscription = undefined;

    publicationOptions = PUBLICATION_OPTIONS;
    expirationOptions = EXPIRATION_OPTIONS;
    newsLayout = NewsLayout;
    newsLayoutContentType = NewsLayoutContentType;

    isLoading: boolean = true;
    isNew: boolean = undefined;
    newsId: number = undefined;
    news: apiNews = undefined;
    newFile: apiAttachment_Published = undefined;
    deleteCurrentlyAssignedFile: boolean = false;
    infoscreens: apiInfoscreen_Light[] = undefined;
    infoscreenGroups: apiInfoscreenGroup[] = undefined;

    languages: apiLanguage[] = undefined;
    languagesLeftToAdd: apiLanguage[] = undefined;
    selectedNewLanguage: apiLanguage = undefined;

    allCategories: string[] = [];
    categories: string[] = [];

    expandedInfoscreenAssigmentDropdown: boolean = false;

    private editNewsForm: FormGroup = undefined;
    private markdownEditors: { editor: Editor; languageId: number }[] = [];

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
        this.languageSubscription = this._translationService.currentLanguage.subscribe(() => this.updateEditorsLanguage());

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
                this.news = null;
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
                    PUBLICATION_OPTIONS.draft,
                    moment().startOf('day').toISOString(),
                    EXPIRATION_OPTIONS.expire,
                    moment().startOf('day').add(15, 'day').toISOString(),
                    NewsLayout.horizontal,
                    null,
                    true,
                    this.isValidApp(),
                    null,
                    [],
                    { size: 30, content: NewsLayoutContentType.file },
                    { size: 70, content: NewsLayoutContentType.text }
                );
                this.isLoading = false;
            } else {
                // For already existing news
                this.pathArray[2] = 'menuItem.contentManagement.editElement';

                this.routeParamSubscription = this.route.paramMap.subscribe(async routeParams => {
                    try {
                        // Get news data
                        this.newsId = parseInt(routeParams.get('newsId'));
                        this._appInsightsService.logPageView('News Edit', '/contentManagement/news/' + this.newsId.toString());
                        this.news = await this.dataService.getNewsAsync(this.newsId);

                        // Init multilingual forms for each languages
                        let multilingualContentFormItems: FormGroup[] = [];
                        let newsLanguages = Object.getOwnPropertyNames(this.news.title);
                        newsLanguages.forEach(newsLanguage => {
                            let language = this.languages.find(l => l.cultureCode === newsLanguage);
                            if (language) {
                                multilingualContentFormItems.push(
                                    this.createMultilingualFormItem(language, this.news.title[newsLanguage], this.news.contentMarkdown[newsLanguage])
                                );
                                this.removeLanguageFromAddLaguageSelect(language);
                            }
                        });

                        // Form element setup
                        this.initEditFormGroup(
                            multilingualContentFormItems,
                            this.news.isVisible ? PUBLICATION_OPTIONS.publish : PUBLICATION_OPTIONS.draft,
                            this.news.publicationDate,
                            this.news.expirationDate ? EXPIRATION_OPTIONS.expire : EXPIRATION_OPTIONS.persist,
                            this.news.expirationDate,
                            this.news.layout,
                            this.news.assignedToInfoscreenIds,
                            this.news.isForInfoscreens,
                            this.news.isForApp,
                            this.news.description,
                            this.news.categories.map(c => c.name),
                            this.news.box1,
                            this.news.box2
                        );
                        this.isLoading = false;
                    } catch (error) {
                        if (error.error) {
                            if (error.error.exceptionMessageLabel == 'error.newsNotFound') {
                                this.snackbarService.displayErrorSnackbar('news.notFound');
                            }
                            this.router.navigateByUrl('/contentManagement/news');
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
        if (this.languageSubscription) {
            this.languageSubscription.unsubscribe();
        }
    }

    initEditFormGroup(
        multilingualContent: FormGroup[],
        publication: number,
        publicationDate: string,
        expiration: number,
        expirationDate: string,
        layout: NewsLayout,
        assignedInfoscreenIds: number[],
        isForInfoscreens: boolean,
        isForApp: boolean,
        description: string,
        categories: string[],
        box1: apiNewsLayoutBox,
        box2: apiNewsLayoutBox
    ) {
        this.categories = categories;

        // Create form item
        this.editNewsForm = this.formBuilder.group({
            multilingualContent: new FormArray(multilingualContent),
            publication: new FormControl(publication, [Validators.required]),
            publicationDate: new FormControl(publicationDate, [Validators.required]),
            expiration: new FormControl(expiration),
            expirationDate: new FormControl(expirationDate),
            layout: new FormControl(layout || NewsLayout.horizontal),
            isForInfoscreens: new FormControl(isForInfoscreens),
            isForApp: new FormControl({
                value: isForApp,
                disabled: !this.isValidApp()
            }),
            assignedInfoscreenIds: new FormControl(assignedInfoscreenIds),
            description: new FormControl(description, [Validators.maxLength(250)]),
            box1Size: new FormControl(box1.size),
            box2Size: new FormControl(box2.size),
            box1ContentType: new FormControl(box1.content != NewsLayoutContentType.text ? NewsLayoutContentType.file : NewsLayoutContentType.text),
            box2ContentType: new FormControl(box2.content != NewsLayoutContentType.text ? NewsLayoutContentType.file : NewsLayoutContentType.text)
        });
    }

    //****************************************
    //        MULTILINGUAL FORM ITEM
    //****************************************

    createMultilingualFormItem(language: apiLanguage, title: string = '', contentMarkdown: string = '') {
        setTimeout(() => {
            this.initMarkdownEditor(language, contentMarkdown);
        }, 0);

        return this.formBuilder.group({
            language: language,
            title: new FormControl(title),
            contentMarkdown: new FormControl(contentMarkdown)
        });
    }

    addMultilingualFormItem(language: apiLanguage, title: string = '', contentMarkdown: string = '') {
        // Check if language is already used, if yes then abort add
        var multilingualContentFormArray = this.editNewsForm.get('multilingualContent') as FormArray;
        if (this.isLanguageAlreadyUsedInMultilingualItems(language, multilingualContentFormArray)) {
            return;
        }

        this.removeLanguageFromAddLaguageSelect(language);
        multilingualContentFormArray.push(this.createMultilingualFormItem(language, title, contentMarkdown));
    }

    removeMultilingualFormItem(index: number) {
        // If index not in range or will remove last item abort
        var multilingualContentFormArray = this.editNewsForm.get('multilingualContent') as FormArray;
        if (multilingualContentFormArray.value.length < 2 || multilingualContentFormArray.value.length <= index) {
            return;
        }

        var language = multilingualContentFormArray.value[index].language;
        var markdownEditorIndex = this.markdownEditors.findIndex(me => me.languageId === language.id);
        this.markdownEditors[markdownEditorIndex].editor.remove();
        this.markdownEditors.splice(markdownEditorIndex, 1);
        this.addLanguageToAddLaguageSelect(language);
        multilingualContentFormArray.removeAt(index);
    }

    async addAutomaticalyTranslatedMultilingualItem(toLanguage: apiLanguage, multilingualContentIndex: number) {
        // Check if language is already used, if yes then abort add
        var multilingualContentFormArray = this.editNewsForm.get('multilingualContent') as FormArray;
        if (this.isLanguageAlreadyUsedInMultilingualItems(toLanguage, multilingualContentFormArray)) {
            return;
        }

        this.isLoading = true;
        // Translate news
        var multilingualContent = multilingualContentFormArray.value[multilingualContentIndex];

        // Only translating the plain text, not the markdown or the html produced by the editor
        var plainContentText = this.html2text(
            this.getEditorForLanguage(multilingualContentFormArray.value[multilingualContentIndex].language).getHtml()
        );

        var newsToTranslate = new apiNews_Translate(multilingualContent.language.iso2, toLanguage.iso2, multilingualContent.title, plainContentText);

        try {
            var translatedNews = await this.apiService.translateNewsAsync(newsToTranslate);

            // Create new language from translation
            this.addMultilingualFormItem(toLanguage, translatedNews.title, translatedNews.content);
        } catch (error) {
        } finally {
            this.isLoading = false;
        }
    }

    isLanguageAlreadyUsedInMultilingualItems(language: apiLanguage, multilingualContentFormArray: FormArray): boolean {
        return multilingualContentFormArray.value.find(mlc => mlc.language.id === language.id);
    }

    //****************************************
    //         MARKDOWN EDITOR
    //****************************************

    createEditor(contentLanguage: apiLanguage, initialValue: string) {
        var element: HTMLElement = document.querySelector('#editor-' + contentLanguage.iso2);
        return new Editor({
            el: element,
            height: 'auto',
            initialEditType: 'wysiwyg',
            hideModeSwitch: true,
            initialValue,
            previewStyle: 'tab',
            usageStatistics: false,
            language: this._translationService.currentLanguage.value?.iso2.toLowerCase(),
            toolbarItems: ['heading', 'bold', 'italic', 'divider', 'ul', 'link'],
            events: {
                change: param => this.onMarkdownChange(param, contentLanguage, parseInt(element.getAttribute('multilingualContentIndex')))
            }
        });
    }

    getEditorForLanguage(language: apiLanguage): Editor {
        return this.markdownEditors.find(e => e.languageId == language.id).editor;
    }

    initMarkdownEditor(language: apiLanguage, initialValue: string) {
        var editor = this.createEditor(language, initialValue);
        this.markdownEditors.push({ editor, languageId: language.id });
    }

    onMarkdownChange(
        param: { source: 'wysiwyg' | 'markdown' | 'viewer'; data: MouseEvent },
        language: apiLanguage,
        multilingualContentIndex: number
    ): void {
        var editor = this.markdownEditors.find(o => o.languageId == language.id).editor;
        this.editNewsForm['controls']['multilingualContent']['controls'][multilingualContentIndex]['controls']['contentMarkdown'].setValue(
            editor.getMarkdown()
        );
    }

    updateEditorsLanguage() {
        this.markdownEditors = this.markdownEditors.map(o => {
            var currentValue = o.editor.getMarkdown();
            o.editor.remove();
            var newEditor = this.createEditor(
                this.languages.find(l => l.id == o.languageId),
                currentValue
            );
            return {
                ...o,
                editor: newEditor
            };
        });
    }

    html2text(html: string): string {
        var tag = document.createElement('div');
        tag.hidden = true;
        tag.innerHTML = html;

        var result = tag.innerText;
        tag.remove();

        return result;
    }

    //****************************************
    //         NOTIFICATIONS
    //****************************************

    isTodayOrBefore(date: string): boolean {
        return moment.utc(date).isSameOrBefore(moment.utc(), 'day');
    }

    isForApp(): boolean {
        return !!this.editNewsForm?.value?.isForApp;
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
    //          FILE (IMAGES/PDF)
    //****************************************

    newFileHandler(event: Event) {
        // Check if files are available
        var target = event.target as HTMLInputElement;
        if (!target.files || !target.files.length) return;

        const file = target.files[0];

        var mimeParts = file.type.split('/');
        var extension = mimeParts.length > 1 ? mimeParts[1] : '';

        if (SUPPORTED_EXTENSIONS.indexOf(extension) == -1) {
            target.value = '';
            this.snackbarService.displayErrorSnackbar('news.file.formatNotSupported');
            return;
        }

        // file size
        var filesize = file.size / 1024 / 1024; // MB
        if (filesize > MAX_FILE_SIZE_MB) {
            target.value = '';
            this.snackbarService.displayErrorSnackbar('news.image.fileTooBig');
            return;
        }

        var reader = new FileReader();
        this.isLoading = true;
        setTimeout(() => {
            reader.onload = e => {
                var base64 = reader.result.toString();
                this.newFile = {
                    fileName: file.name,
                    fileExtension: extension,
                    base64
                };
                this.deleteCurrentlyAssignedFile = true;
                this.isLoading = false;
                target.value = '';
            };
            reader.readAsDataURL(file);
        });
    }

    removeMetadataFromBase64(base64: string): string {
        return base64.split(',')[1].trim();
    }

    restoreFile() {
        this.newFile = null;
        this.deleteCurrentlyAssignedFile = false;
    }

    deleteFile() {
        this.newFile = null;
        this.deleteCurrentlyAssignedFile = true;
    }

    onBox1SizeChange() {
        this.editNewsForm.controls['box2Size'].setValue(100 - this.editNewsForm.controls['box1Size'].value);
    }

    onBox2SizeChange() {
        this.editNewsForm.controls['box1Size'].setValue(100 - this.editNewsForm.controls['box2Size'].value);
    }

    onBox1ContentTypeChange() {
        if (this.editNewsForm.controls['box1ContentType'].value == NewsLayoutContentType.text) {
            this.editNewsForm.controls['box2ContentType'].setValue(NewsLayoutContentType.file);
        } else {
            this.editNewsForm.controls['box2ContentType'].setValue(NewsLayoutContentType.text);
        }
    }

    onBox2ContentTypeChange() {
        if (this.editNewsForm.controls['box2ContentType'].value == NewsLayoutContentType.text) {
            this.editNewsForm.controls['box1ContentType'].setValue(NewsLayoutContentType.file);
        } else {
            this.editNewsForm.controls['box1ContentType'].setValue(NewsLayoutContentType.text);
        }
    }

    //****************************************
    //         INFOSCREEN ASSIGMNENT
    //****************************************

    isInfoscreenAssigned(infoscreenId: number): boolean {
        if (this.news && this.news.assignedToInfoscreenIds.find(id => id == infoscreenId)) {
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
        var selectedInfoscreensOfGroup = this.editNewsForm.controls['assignedInfoscreenIds'].value?.filter(id => infoscreenIdsOfGroup.includes(id));
        return selectedInfoscreensOfGroup && selectedInfoscreensOfGroup.length == infoscreenIdsOfGroup.length;
    }

    isGroupIndeterminate(groupId: number): boolean {
        var infoscreenIdsOfGroup = this.infoscreens.filter(i => i.infoscreenGroupId == groupId).map(i => i.id);
        var selectedInfoscreensOfGroup = this.editNewsForm.controls['assignedInfoscreenIds'].value?.filter(id => infoscreenIdsOfGroup.includes(id));
        return selectedInfoscreensOfGroup && selectedInfoscreensOfGroup.length > 0 && selectedInfoscreensOfGroup.length < infoscreenIdsOfGroup.length;
    }

    groupChange(groupId: number, checked: boolean): void {
        var infoscreenIdsOfGroup = this.infoscreens.filter(i => i.infoscreenGroupId == groupId).map(i => i.id);
        if (checked) {
            var infoscreenIdsToAdd = infoscreenIdsOfGroup.filter(id => !this.editNewsForm.controls['assignedInfoscreenIds'].value?.includes(id));
            this.editNewsForm.controls['assignedInfoscreenIds'].setValue(
                this.editNewsForm.controls['assignedInfoscreenIds'].value?.concat(infoscreenIdsToAdd) || infoscreenIdsToAdd
            );
        } else {
            this.editNewsForm.controls['assignedInfoscreenIds'].setValue(
                this.editNewsForm.controls['assignedInfoscreenIds'].value?.filter(id => !infoscreenIdsOfGroup.includes(id))
            );
        }
    }

    isInfoscreenChecked(infoscreenId: number): boolean {
        var result = this.editNewsForm.controls['assignedInfoscreenIds'].value?.find(id => infoscreenId == id);
        return !!result;
    }

    infoscreenChange(infoscreenId: number, checked: boolean): void {
        if (checked) {
            this.editNewsForm.controls['assignedInfoscreenIds'].setValue(
                this.editNewsForm.controls['assignedInfoscreenIds'].value?.concat([infoscreenId]) || [infoscreenId]
            );
        } else {
            this.editNewsForm.controls['assignedInfoscreenIds'].setValue(
                this.editNewsForm.controls['assignedInfoscreenIds'].value?.filter(id => id != infoscreenId)
            );
        }
    }

    //****************************************
    //                 SUBMIT
    //****************************************

    async onSubmit(data) {
        if (this.editNewsForm.invalid) {
            this.snackbarService.displayErrorSnackbar('formular.invalid');
            return;
        }

        const saveNews = async () => {
            try {
                this.isLoading = true;
                var apiNews_Publish = this.manipulateFromDataForRequest(data);
                await this.publishNewsAsync(apiNews_Publish);

                this.isLoading = false;
                this.router.navigateByUrl('/contentManagement/news');

                this.snackbarService.displaySuccessSnackbar('news.save.success');
            } catch (error) {
                this.isLoading = false;
                console.error(error);
                this.snackbarService.displayErrorSnackbar('news.save.error');
            }
        };

        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            closeOnNavigation: true,
            data: {
                actionTitle: this.isNew ? 'news.submit.new.confirmation.title' : 'news.submit.edit.confirmation.title',
                actionButton: this.isNew ? 'general.button.submit' : 'general.button.save',
                message: this.isNew ? 'news.submit.new.confirmation.message' : 'news.submit.edit.confirmation.message'
            } as IConfirmationDialogData
        });

        dialogRef.afterClosed().subscribe(async proceed => {
            if (proceed) {
                const hasMissingContent = data.multilingualContent.some(
                    (c: { contentMarkdown: string; title: string; language: apiLanguage }) => !c.contentMarkdown || !c.title
                );

                const hasImageFullScreen =
                    (data.box1ContentType === NewsLayoutContentType.file ? data.box1Size : data.box2Size) === 100 && this.newFile;

                if (hasMissingContent && !hasImageFullScreen) {
                    const missingContentDialogRef = this.dialog.open(ConfirmationDialogComponent, {
                        closeOnNavigation: true,
                        data: {
                            actionTitle: 'news.missingContent.confirmation.title',
                            actionButton: this.isNew ? 'general.button.submit' : 'general.button.save',
                            message: 'news.missingContent.confirmation.message'
                        } as IConfirmationDialogData
                    });
                    missingContentDialogRef.afterClosed().subscribe(async proceed => {
                        if (proceed) {
                            saveNews();
                        }
                    });
                } else {
                    saveNews();
                }
            }
        });
    }

    manipulateFromDataForRequest(data): apiNews_Publish {
        var id = null;
        if (!this.isNew) {
            id = this.news.id;
        }

        var title: IStringDictionary<string> = {};
        data.multilingualContent.forEach(c => (title[c.language.iso2] = c.title));

        var contentMarkdown: IStringDictionary<string> = {};
        data.multilingualContent.forEach(c => (contentMarkdown[c.language.iso2] = this.getEditorForLanguage(c.language).getMarkdown()));

        var contentHTML: IStringDictionary<string> = {};
        data.multilingualContent.forEach(c => (contentHTML[c.language.iso2] = this.getEditorForLanguage(c.language).getHtml()));

        var attachment: apiAttachment_Published = null;
        if (this.newFile)
            attachment = {
                fileName: this.newFile.fileName,
                fileExtension: this.newFile.fileExtension,
                base64: this.removeMetadataFromBase64(this.newFile.base64)
            };

        var deleteAttachment: number = null;
        if (this.news && this.news.attachment && this.deleteCurrentlyAssignedFile) deleteAttachment = this.news.attachment.id;

        let isForInfoscreens = data.isForInfoscreens;
        let isForApp = data.isForApp || false;

        var isVisible = data.publication == this.publicationOptions.publish;

        var expirationDate = data.expiration == this.expirationOptions.expire ? data.expirationDate : null;

        var box1: NewsLayoutBox = {
            size: data.box1Size,
            content: data.box1ContentType
        };

        var box2: NewsLayoutBox = {
            size: data.box2Size,
            content: data.box2ContentType
        };

        let categories = this.categories;

        return new apiNews_Publish(
            id,
            title,
            contentMarkdown,
            contentHTML,
            attachment,
            isVisible,
            data.publicationDate,
            expirationDate,
            data.assignedInfoscreenIds || [],
            isForInfoscreens,
            isForApp,
            data.description,
            categories,
            deleteAttachment,
            data.layout,
            box1,
            box2
        );
    }

    async publishNewsAsync(apiNews_Publish: apiNews_Publish): Promise<void> {
        return new Promise(async (resolve, reject) => {
            var publishedNews: apiNews;

            try {
                if (this.isNew) {
                    publishedNews = await this.apiService.postNewsAsync(apiNews_Publish);
                } else {
                    publishedNews = await this.apiService.putNewsAsync(apiNews_Publish);
                }

                await this.dataService.getNewsAsync(publishedNews.id, true, true);
            } catch (error) {
                reject(error);
            }

            resolve();
        });
    }

    //****************************************
    //                 ACTIONS
    //****************************************

    async deleteNews() {
        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            closeOnNavigation: true,
            data: {
                actionTitle: 'news.delete.confirmation.title',
                actionButton: 'general.button.delete',
                message: 'news.delete.confirmation.message'
            } as IConfirmationDialogData
        });

        dialogRef.afterClosed().subscribe(async proceed => {
            if (proceed) {
                try {
                    this.isLoading = true;
                    await this.apiService.deleteNewsAsync(this.news.id);
                    this.dataService.deleteNewsLocally(this.news.id);

                    this.isLoading = false;
                    this.router.navigateByUrl('/contentManagement/news');

                    this.snackbarService.displaySuccessSnackbar('news.delete.success');
                } catch (error) {
                    this.isLoading = false;
                }
            }
        });
    }

    async cancelEdit() {
        if (this.editNewsForm.pristine) {
            this.router.navigateByUrl('/contentManagement/news');
        } else {
            const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
                closeOnNavigation: true,
                data: {
                    actionTitle: this.isNew ? 'news.cancel.new.confirmation.title' : 'news.cancel.edit.confirmation.title',
                    actionButton: this.isNew ? 'general.button.discard' : 'general.button.discard',
                    message: this.isNew ? 'news.cancel.new.confirmation.message' : 'news.cancel.edit.confirmation.messsage'
                } as IConfirmationDialogData
            });

            dialogRef.afterClosed().subscribe(async proceed => {
                if (proceed) {
                    this.router.navigateByUrl('/contentManagement/news');
                }
            });
        }
    }

    async resetNotificationFlag() {
        this.isLoading = true;
        await this.apiService.resetNotificationFlag(this.newsId);
        this.isLoading = false;
        this.news.usersNotified = null;
        this.dataService.getNewsAsync(this.newsId, false, true);
    }

    //****************************************
    //         PRÉVISUALISATION SLIDESHOW
    //****************************************

    /**
     * Vérifier si on peut faire la prévisualisation
     */
    canPreview(): boolean {
        // Pour une news existante, toujours autoriser la prévisualisation
        if (this.news && this.news.id) {
            return true;
        }
        
        // Pour une nouvelle news, vérifier le formulaire
        if (this.editNewsForm && this.editNewsForm.value) {
            const formData = this.editNewsForm.value;
            
            // Vérifier qu'il y a au moins un contenu multilingue avec un titre
            if (formData.multilingualContent && formData.multilingualContent.length > 0) {
            const hasValidContent = formData.multilingualContent.some((content: any) => 
                content.title && content.title.trim().length > 0
            );
            
            return hasValidContent;
            }
        }
        
        return false;
        }

    /**
     * MÉTHODE PRINCIPALE - Ouvrir la prévisualisation slideshow exacte
     */
    openSlideshowPreview(): void {
        // Vérification de sécurité
        if (!this.canPreview()) {
            this.snackbarService.displayErrorSnackbar('Veuillez remplir au moins un titre avant la prévisualisation.');
            return;
        }
        
        try {
            // Préparer les données de la news pour la preview
            const newsPreviewData = this.prepareNewsForPreview();
            
            // Données de l'appareil par défaut (TV 1920x1080)
            const deviceData = {
            deviceType: 'tv' as const,
            width: 1920,
            height: 1080
            };

            // Langue actuelle
            const currentLanguage = this._translationService.currentLanguage.value?.iso2 || 'fr';

            // Construire l'URL avec les données
            const queryParams = {
            news: encodeURIComponent(JSON.stringify(newsPreviewData)),
            device: encodeURIComponent(JSON.stringify(deviceData)),
            language: currentLanguage
            };

            const url = this.router.serializeUrl(
            this.router.createUrlTree(['/news-preview-exact'], { queryParams })
            );

            // Ouvrir dans une nouvelle fenêtre
            window.open(url, '_blank', 'width=1920,height=1080,scrollbars=no,resizable=yes');
            
            this.snackbarService.displaySuccessSnackbar('Prévisualisation ouverte dans une nouvelle fenêtre.');
            
        } catch (error) {
            console.error('Erreur lors de l\'ouverture de la prévisualisation:', error);
            this.snackbarService.displayErrorSnackbar('Erreur lors de l\'ouverture de la prévisualisation.');
        }
    }

/**
 * Ouvrir la prévisualisation avec sélecteur d'appareil
 */
/**
 * Ouvrir la prévisualisation avec sélecteur d'appareil
 */
/**
 * Ouvrir la prévisualisation avec sélecteur d'appareil
 */
openPreviewWithDeviceSelector(): void {
    if (!this.canPreview()) {
        this.snackbarService.displayErrorSnackbar('Veuillez remplir au moins un titre avant la prévisualisation.');
        return;
    }
    
    // MARQUER LE FORMULAIRE COMME PRISTINE TEMPORAIREMENT pour éviter la popup de sauvegarde
    const originalPristine = this.editNewsForm.pristine;
    this.editNewsForm.markAsPristine();
    
    try {
        const newsPreviewData = this.prepareNewsForPreview();
        
        const dialogRef = this.dialog.open(DevicePreviewDialogComponent, {
            width: '520px',
            maxWidth: '90vw',
            data: { newsData: newsPreviewData },
            disableClose: false,
            autoFocus: true
        });

        dialogRef.afterClosed().subscribe(result => {
            // RESTAURER L'ÉTAT ORIGINAL DU FORMULAIRE
            if (!originalPristine) {
                this.editNewsForm.markAsDirty();
            }
            
            if (result && result.launched) {
                this.snackbarService.displaySuccessSnackbar('Prévisualisation ouverte dans une nouvelle fenêtre.');
            }
        });
        
    } catch (error) {
        // RESTAURER L'ÉTAT ORIGINAL EN CAS D'ERREUR
        if (!originalPristine) {
            this.editNewsForm.markAsDirty();
        }
        console.error('Erreur lors de l\'ouverture de la prévisualisation:', error);
        this.snackbarService.displayErrorSnackbar('Erreur lors de l\'ouverture de la prévisualisation.');
    }
}
/**
 * Ouvrir directement la prévisualisation sans vérifications
 */
private openPreviewDirectly(): void {
    try {
        const newsPreviewData = this.prepareNewsForPreview();
        
        const dialogRef = this.dialog.open(DevicePreviewDialogComponent, {
            width: '520px',
            maxWidth: '90vw',
            data: { newsData: newsPreviewData },
            disableClose: false,
            autoFocus: true
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result && result.launched) {
                this.snackbarService.displaySuccessSnackbar('Prévisualisation ouverte dans une nouvelle fenêtre.');
            }
        });
        
    } catch (error) {
        console.error('Erreur lors de l\'ouverture de la prévisualisation:', error);
        this.snackbarService.displayErrorSnackbar('Erreur lors de l\'ouverture de la prévisualisation.');
    }
}
    /**
     * Préparer les données pour la prévisualisation
     */
    /**
   * Préparer les données pour la prévisualisation - FORMAT CORRECT
   */
    private prepareNewsForPreview(): any {
        // Si c'est une news existante, utiliser this.news
        if (this.news && this.news.id) {
        const currentLang = this._translationService.currentLanguage.value?.iso2 || 'fr';
        
        return {
            id: this.news.id,
            title: this.extractTranslatedValue(this.news.title, currentLang),
            content: this.extractTranslatedValue(this.news.contentHTML, currentLang),
            description: this.news.description || '',
            layout: this.news.layout || 'horizontal',
            publicationDate: this.news.publicationDate || new Date().toISOString(),
            isVisible: this.news.isVisible !== undefined ? this.news.isVisible : true,
            categories: this.news.categories ? this.news.categories.map(c => c.name) : [],
            box1: this.news.box1 || { content: 'file', size: 30 },
            box2: this.news.box2 || { content: 'text', size: 70 },
            attachment: this.getAttachmentData()
        };
        }
        
        // Sinon, utiliser les données du formulaire
        const formData = this.editNewsForm.value;
        const currentLang = this._translationService.currentLanguage.value?.iso2 || 'fr';
        
        // Récupérer le titre et contenu de la langue courante
        let title = 'Titre par défaut';
        let content = '<p>Contenu par défaut</p>';
        
        if (formData.multilingualContent && formData.multilingualContent.length > 0) {
        // Chercher le contenu dans la langue courante
        const currentLangContent = formData.multilingualContent.find(
            (content: any) => content.language && content.language.iso2 === currentLang
        );
        
        if (currentLangContent) {
            title = currentLangContent.title || title;
            try {
            const editor = this.getEditorForLanguage(currentLangContent.language);
            if (editor) {
                content = editor.getHtml() || content;
            }
            } catch (error) {
            console.warn('Impossible de récupérer le contenu de l\'éditeur:', error);
            content = currentLangContent.contentMarkdown || content;
            }
        } else {
            // Prendre le premier contenu disponible
            const firstContent = formData.multilingualContent[0];
            if (firstContent) {
            title = firstContent.title || title;
            try {
                const editor = this.getEditorForLanguage(firstContent.language);
                if (editor) {
                content = editor.getHtml() || content;
                }
            } catch (error) {
                content = firstContent.contentMarkdown || content;
            }
            }
        }
        }
        
        // Déterminer la configuration des boxes selon le format attendu
        const box1 = {
        content: formData.box1ContentType || 'file',
        size: formData.box1Size || 30
        };
        
        const box2 = {
        content: formData.box2ContentType || 'text', 
        size: formData.box2Size || 70
        };
        
        return {
        id: 0,
        title: title,
        content: content,
        description: formData.description || '',
        layout: formData.layout || 'horizontal',
        publicationDate: formData.publicationDate || new Date().toISOString(),
        isVisible: formData.publication === PUBLICATION_OPTIONS.publish,
        categories: this.categories || [],
        box1: box1,
        box2: box2,
        attachment: this.getAttachmentData()
        };
    }

    /**
     * Extraire une valeur traduite d'un objet multilingue
     */
    private extractTranslatedValue(multilingualObj: any, language: string): string {
        if (!multilingualObj || typeof multilingualObj !== 'object') {
        return '';
        }

        // Essayer la langue demandée
        if (multilingualObj[language]) {
        return multilingualObj[language];
        }

        // Essayer d'autres langues comme fallback
        const fallbackLanguages = ['fr', 'en', 'de', 'it'];
        for (const fallbackLang of fallbackLanguages) {
        if (multilingualObj[fallbackLang]) {
            return multilingualObj[fallbackLang];
        }
        }

        // Prendre la première valeur disponible
        const values = Object.values(multilingualObj);
        return values.length > 0 ? values[0] as string : '';
    }
    /**
     * Récupérer les données d'attachment de manière sécurisée
     */
    private getAttachmentData(): any {
        // Nouveau fichier uploadé
        if (this.newFile) {
            return {
            fileSrc: this.newFile.base64,
            fileExtension: this.newFile.fileExtension,
            fileName: this.newFile.fileName
            };
        } 
        
        // Attachment existant (pas supprimé)
        if (this.news && this.news.attachment && !this.deleteCurrentlyAssignedFile) {
            const attachment = this.news.attachment as any;
            return {
            fileSrc: attachment.url || attachment.fileSrc || '',
            fileExtension: attachment.fileExtension || '',
            fileName: attachment.fileName || ''
            };
        }
        
        return undefined;
    }

    /**
     * Méthode de debug pour diagnostiquer les problèmes
     */
    debugPreview(): void {
        console.log('=== DEBUG PREVIEW ===');
        console.log('this.news:', this.news);
        console.log('this.editNewsForm:', this.editNewsForm);
        console.log('this.editNewsForm.value:', this.editNewsForm?.value);
        console.log('canPreview():', this.canPreview());
        console.log('this.newFile:', this.newFile);
        console.log('this.deleteCurrentlyAssignedFile:', this.deleteCurrentlyAssignedFile);
        
        if (this.editNewsForm?.value?.multilingualContent) {
            console.log('multilingualContent:', this.editNewsForm.value.multilingualContent);
            this.editNewsForm.value.multilingualContent.forEach((content: any, index: number) => {
                console.log(`Content ${index}:`, content);
                try {
                    const editor = this.getEditorForLanguage(content.language);
                    console.log(`Editor ${index}:`, editor ? 'Existe' : 'N\'existe pas');
                    if (editor) {
                        console.log(`HTML ${index}:`, editor.getHtml());
                    }
                } catch (error) {
                    console.error(`Erreur editor ${index}:`, error);
                }
            });
        }
        
        // Tester la préparation des données
        try {
            const previewData = this.prepareNewsForPreview();
            console.log('Preview data préparée:', previewData);
            alert('Debug réussi - voir console');
        } catch (error) {
            console.error('Erreur lors de la préparation:', error);
            alert('Erreur - voir console');
        }
    }
    
}