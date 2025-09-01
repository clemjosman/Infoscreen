import { UiTenant } from "@vesact/web-ui-template";

export interface apiTenant extends UiTenant {
  id: number;
  code: string;
  displayName: string;
  appName?: string;
  contentAdminEmail?: string;
}
