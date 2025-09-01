import {apiLanguage_Light } from '@models/index';

export interface apiNews_Translated{
    language: apiLanguage_Light;
    title: string;
    content: string;
}