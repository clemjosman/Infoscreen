import { apiUser_Light, apiTenant} from '@models/index';

export interface apiUser_Me extends apiUser_Light{
    iso2: string;
    tenants: apiTenant[];
    selectedTenant: apiTenant;
}