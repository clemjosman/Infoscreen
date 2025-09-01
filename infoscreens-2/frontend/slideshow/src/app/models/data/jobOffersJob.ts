import { SlideshowLanguage } from '../../../../../common';

export interface JobOffersJob {
    title: string;
    content: string;
    showUrl: string;
    applyUrl: string;
    companyName: string;
    city: string;
    countryCode: string;
    publishedAt: string;
    language: SlideshowLanguage | null;
}
