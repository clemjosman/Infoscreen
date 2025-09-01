import IStringDictionary from '@interfaces/IStringDictionary';
import { VideoBackground } from '../../../../../common';

export class apiVideo_Publish {
    id: number | null;
    title: IStringDictionary<string>;
    url: string;
    duration: number;
    background: VideoBackground | null;
    isVisible: boolean;
    publicationDate: string;
    expirationDate: string;
    assignedToInfoscreenIds: number[];
    isForInfoscreens: boolean;
    isForApp: boolean;
    description: string;
    categories: string[];
    
    constructor(
        id: number | null,
        title: IStringDictionary<string>,
        url: string,
        duration: number,
        background: VideoBackground | null,
        isVisible: boolean,
        publicationDate: string,
        expirationDate: string | null,
        assignedToInfoscreenIds: number[],
        isForInfoscreens: boolean,
        isForApp: boolean,
        description: string,
        categories: string[]
    ) {
        this.id = id;
        this.title = title;
        this.url = url;
        this.duration = duration;
        this.background = background;
        this.isVisible = isVisible;
        this.publicationDate = publicationDate;
        this.expirationDate = expirationDate;
        this.assignedToInfoscreenIds = assignedToInfoscreenIds;
        this.isForInfoscreens = isForInfoscreens;
        this.isForApp = isForApp;
        this.description = description;
        this.categories = categories;
    }
}
