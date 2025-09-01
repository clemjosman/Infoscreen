import { InfoscreenCachedStatus, apiInfoscreen_Light } from '@models/index';

export interface apiInfoscreen_Status extends apiInfoscreen_Light{
    status: InfoscreenCachedStatus;
}