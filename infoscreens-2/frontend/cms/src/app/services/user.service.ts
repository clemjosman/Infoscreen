import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { share } from 'rxjs/operators';
import { AuthService_B2C, RootApiService, TenantService, TranslationService, UiLanguage, UiTenant } from '@vesact/web-ui-template';

import { apiUser_Me, apiTenant, apiUser_UpdateSelectedLanguage, apiUser_UpdateSelectedTenant} from '@models/index';
import { SnackbarService } from './snackbar.service';

@Injectable()
export class UserService {

    private _authenticatedUser: apiUser_Me = undefined;

    // Observables
    public _authenticatedUserObservable: BehaviorSubject<apiUser_Me> = new BehaviorSubject<apiUser_Me>(undefined);


    constructor(
        private http: HttpClient,
        private _authService: AuthService_B2C,
        private _tenantService: TenantService,
        private _translationService: TranslationService,
        private _rootApiService: RootApiService,
        private snackbarService: SnackbarService
    ) {
        this._authService.setBeforeSignOutCallback(() => this.emitAuthenticatedUser(undefined));

        this._tenantService.setFetchUserTenants(async () => {
            if(this._authenticatedUserObservable.value)
                return this._authenticatedUserObservable.value.tenants;
            else
                return (await this.getCurrentUserAsync()).tenants;
        });

        this._tenantService.setFetchUserHomeTenant(async () => {
            if(this._authenticatedUserObservable.value)
                return this._authenticatedUserObservable.value.selectedTenant;
            else
                return (await this.getCurrentUserAsync()).selectedTenant;
        });

        this._tenantService.setBeforeTenantChange(async (newTenant) => await this.updateSelectedTenantAsync(newTenant));

        this._translationService.currentLanguage.subscribe(async (newLanguage) => {
            if(!newLanguage)
                return;

            let user = await this.getCurrentUserAsync();
            if(user && user.iso2 != newLanguage.iso2)
                await this.updateSelectedLanguageAsync(newLanguage);
        })
    }

    public emitAuthenticatedUser(value: apiUser_Me): apiUser_Me {
        if(value === this._authenticatedUser)
            return;
        
        if(!value)
        {
            this._authenticatedUser = undefined;
            this._authenticatedUserObservable.next(undefined);
        }
        else
        {
            this._authenticatedUser = value;
            this._authenticatedUserObservable.next(value);
            this._tenantService.setTenants(value.tenants);
            this._tenantService.setCurrentTenant(value.selectedTenant);
        }
        return value;
    }

    public getCurrentUserAsync(forceReload: boolean = false): Promise<apiUser_Me>
    {
        return new Promise<apiUser_Me>(async (resolve, reject) => {
            try{
                if(this._authenticatedUser == undefined ||forceReload)
                {
                    if(await this._authService.isAuthenticated()){
                        this.emitAuthenticatedUser(await this._getCurrentUserMeAsync.toPromise());
                    }
                }
                resolve(this._authenticatedUser);
            }
            catch(error){
                reject(error);
            }
        });
    }

    // This function was initialy intended to be part of the ApiService
    // But because of a circular dependency issue, it has to be defined here
    private _getCurrentUserMeAsync: Observable<apiUser_Me> = this.http.get<apiUser_Me>(this._rootApiService.globalEndpoint+'v1/user/me').pipe(share());

    public async isUserAccountReadyAsync(): Promise<boolean>{
        return new Promise<boolean>(async (resolve, reject) => {
            try{
                var user = await this.getCurrentUserAsync()
                if(user){
                    resolve(true);
                }
            }
            catch(error){}
            finally{
                resolve(false);
            }
        });
    }

    //****************************************
    //               LANGUAGES
    //****************************************

    public updateSelectedLanguageAsync(newSelectedLanguage: UiLanguage): Promise<apiUser_Me>{
        var body = new apiUser_UpdateSelectedLanguage(newSelectedLanguage.iso2);

        return new Promise<apiUser_Me>((resolve, reject) => {
            this.http.put(this._rootApiService.globalEndpoint+'v1/user/selectedLanguage', body).subscribe(
                (data: apiUser_Me) => {
                    this.emitAuthenticatedUser(data);
                    resolve(data)
                },
                (errorResponse: any) => {
                    reject(errorResponse);
                }
            )
        });
    }

    //****************************************
    //               TENANTS
    //****************************************

    public updateSelectedTenantAsync(newSelecedTenant: UiTenant): Promise<apiUser_Me>{
        var body = new apiUser_UpdateSelectedTenant(parseInt(newSelecedTenant.id.toString()));

        return new Promise<apiUser_Me>((resolve, reject) => {
            this.http.put(this._rootApiService.globalEndpoint+'v1/user/selectedTenant', body).subscribe(
                (data: apiUser_Me) => {
                    this.emitAuthenticatedUser(data);
                    resolve(data)
                },
                (errorResponse: any) => {
                    this.snackbarService.displayErrorSnackbar('user.updateTenant.error');
                    // TODO: log error
                    reject(errorResponse);
                }
            )
        });
    }
}