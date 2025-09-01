import IStringDictionary from '@interfaces/IStringDictionary';
import { apiCategory, apiUser_Light } from '@models/index';
import { VideoBackground } from '../../../../../common';

export interface apiVideo {
    id: number;
    title: IStringDictionary<string>;
    url: string;
    umbedUrl: string;
    duration: number;
    background: VideoBackground | null;
    isVisible: boolean;
    publicationDate: string;
    expirationDate: string;
    creationDate: string;
    creator: apiUser_Light;
    lastEditDate: string;
    lastEditor: apiUser_Light;
    assignedToInfoscreenIds: number[];
    isForInfoscreens: boolean;
    isForApp: boolean;
    description: string;
    categories: apiCategory[];
    usersNotified: string;
}
