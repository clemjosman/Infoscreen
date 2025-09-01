import IStringDictionary from '@interfaces/IStringDictionary';
import { apiAttachment_Published } from '@models/index';
import { NewsLayout, NewsLayoutBox } from '../../../../../common';

export class apiNews_Publish {
    id: number | null;
    title: IStringDictionary<string>;
    contentMarkdown: IStringDictionary<string>;
    contentHTML: IStringDictionary<string>;
    attachment: apiAttachment_Published;
    deleteAttachment: number;
    isVisible: boolean;
    publicationDate: string;
    expirationDate: string;
    assignedToInfoscreenIds: number[];
    isForInfoscreens: boolean;
    isForApp: boolean;
    description: string;
    categories: string[];
    layout: NewsLayout;
    box1: NewsLayoutBox;
    box2: NewsLayoutBox;

    constructor(
        id: number | null,
        title: IStringDictionary<string>,
        contentMarkdown: IStringDictionary<string>,
        contentHTML: IStringDictionary<string>,
        attachment: apiAttachment_Published,
        isVisible: boolean,
        publicationDate: string,
        expirationDate: string | null,
        assignedToInfoscreenIds: number[],
        isForInfoscreens: boolean,
        isForApp: boolean,
        description: string,
        categories: string[],
        deleteAttachment: number = null,
        layout: NewsLayout,
        box1: NewsLayoutBox,
        box2: NewsLayoutBox
    ) {
        this.id = id;
        this.title = title;
        this.contentMarkdown = contentMarkdown;
        this.contentHTML = contentHTML;
        this.attachment = attachment;
        this.deleteAttachment = deleteAttachment;
        this.isVisible = isVisible;
        this.publicationDate = publicationDate;
        this.expirationDate = expirationDate;
        this.assignedToInfoscreenIds = assignedToInfoscreenIds;
        this.isForInfoscreens = isForInfoscreens;
        this.isForApp = isForApp;
        this.description = description;
        this.categories = categories;
        this.layout = layout;
        this.box1 = box1;
        this.box2 = box2;
    }
}
