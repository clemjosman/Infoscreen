import { Injectable } from '@angular/core';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { environment } from '../../environments/environment';
import { TenantService } from '@vesact/web-ui-template';

export const LOG_SETTING_STORAGE_KEY: string = "LOG_SETTING";
export const FORCE_LOGGING_FOR_ALL: boolean = true;
export const DEFAULT_LOGGING_SETTING: boolean = true;
@Injectable()
export class ApplicationInsightsService {
  appInsights: ApplicationInsights;
  constructor(private _tenantService: TenantService) {
    // Set logging to active by default
    if(!this.hasLoggingSetting()){
      this.setLoggingSetting(DEFAULT_LOGGING_SETTING);
    }

    this.appInsights = new ApplicationInsights({
      config: {
        instrumentationKey: environment.appInsights.instrumentationKey,
        enableAutoRouteTracking: true, // option to log all route changes
        appId: this._getAppId(),
      },
    });
    this.appInsights.loadAppInsights();
  }

  private _getAppId(): string {
    return `Tempus-Mobile-${environment.production ? 'Prod' : 'Test'}`;
  }

  private _getTenantId(): string {
    return `${this._tenantService?.currentTenant?.code || 'unknown' }`;
  }

  private _defaultProperties() {
    return {
      appId: this._getAppId(),
      tenantId: this._getTenantId(),
    }
  }

  // option to call manually
  public logPageView(name?: string, url?: string, properties?: { [key: string]: any }) {
    if(!this.getLoggingSetting()) return;
    this.appInsights.trackPageView({
      name: name,
      uri: url,
      properties: {
        ...properties,
        ...this._defaultProperties()
      },
    });
  }

  public logEvent(name: string, properties?: { [key: string]: any }) {
    if(!this.getLoggingSetting()) return;
    this.appInsights.trackEvent(
      { name: name },
      {
        ...properties,
        ...this._defaultProperties()
      }
    );
  }

  public logMetric(
    name: string,
    average: number,
    properties?: { [key: string]: any }
  ) {
    if(!this.getLoggingSetting()) return;
    this.appInsights.trackMetric(
      { name: name, average: average },
      {
        ...properties,
        ...this._defaultProperties()
      }
    );
  }

  public logException(exception: Error, severityLevel?: number, properties?: { [key: string]: any}) {
    if(!this.getLoggingSetting()) return;
    this.appInsights.trackException({
      exception: exception,
      severityLevel: severityLevel,
      properties: { 
        ...properties,
        ...this._defaultProperties()
      },
    });
  }

  public logTrace(message: string, properties?: { [key: string]: any }) {
    if(!this.getLoggingSetting()) return;
    this.appInsights.trackTrace(
      { message: message },
      {
        ...properties,
        ...this._defaultProperties()
      }
    );
  }

  private hasLoggingSetting(): boolean {
    return localStorage.getItem(LOG_SETTING_STORAGE_KEY) != null;
  }
  
  public getLoggingSetting(): boolean {
    return FORCE_LOGGING_FOR_ALL || localStorage.getItem(LOG_SETTING_STORAGE_KEY) == "1" ? true : false;
  }
  
  public setLoggingSetting(active: boolean): void {
    localStorage.setItem(LOG_SETTING_STORAGE_KEY, active ? "1" : "0");
  }
}
