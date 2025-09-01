import { apiLanguage } from '../language/apiLanguage';

export interface apiInfoscreen_Light {
    /** Internal CMS ID */
    id: number;

    /** MSB node name */
    nodeId: string;

    /** MSB GUID ID */
    msbNodeId: string | null;

    displayName: string;
    description: string;
    infoscreenGroupId: number;
    contentAdminEmail: string;
    sendMailNoContent: boolean;

    /** Default language for CMS content */
    language: apiLanguage;
}
