import IStringDictionary from '@interfaces/IStringDictionary';
import { apiUser_Light, apiAttachment, apiCategory } from '@models/index';
import { NewsLayout, NewsLayoutContentType } from '../../../../../common';

export interface apiNewsLayoutBox {
    size: number;
    content: NewsLayoutContentType;
}

export interface apiNews {
    id: number;
    title: IStringDictionary<string>;
    contentMarkdown: IStringDictionary<string>;
    contentHTML: IStringDictionary<string>;
    isVisible: boolean;
    publicationDate: string;
    expirationDate: string;
    layout: NewsLayout;
    attachment: apiAttachment;
    creationDate: string;
    creator: apiUser_Light;
    lastEditDate: string;
    lastEditor: apiUser_Light;
    assignedToInfoscreenIds: number[];
    isForInfoscreens: boolean;
    isForApp: boolean;
    description: string;
    categories: apiCategory[];
    usersNotified: string | null;
    box1: apiNewsLayoutBox;
    box2: apiNewsLayoutBox;
}
