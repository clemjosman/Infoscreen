// cms/src/app/app.module.ts
// Angular Components:
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DragDropModule } from "@angular/cdk/drag-drop";
import { TranslateModule } from "@ngx-translate/core";
import { DatePipe } from '@angular/common';

// Angular Material - COMPLET:
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatNativeDateModule } from "@angular/material/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatMenuModule } from "@angular/material/menu";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatInputModule } from "@angular/material/input";
import { MatChipsModule } from "@angular/material/chips";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatCardModule } from "@angular/material/card";
import { MatDividerModule } from "@angular/material/divider";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatSortModule } from "@angular/material/sort";
import { MatTableModule } from "@angular/material/table";
import { MatDialogModule } from "@angular/material/dialog";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatRadioModule } from "@angular/material/radio";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSliderModule } from '@angular/material/slider';

// Third party plugins
import { PdfViewerModule } from "ng2-pdf-viewer";

// Authentication Azure B2C:
import {
  IPublicClientApplication,
  PublicClientApplication,
  InteractionType,
  BrowserCacheLocation,
  LogLevel as MsalLogLevel,
} from "@azure/msal-browser";
import {
  MsalGuard,
  MsalInterceptor,
  MsalBroadcastService,
  MsalInterceptorConfiguration,
  MsalModule,
  MsalService,
  MSAL_GUARD_CONFIG,
  MSAL_INSTANCE,
  MSAL_INTERCEPTOR_CONFIG,
  MsalGuardConfiguration,
  MsalRedirectComponent,
} from "@azure/msal-angular";

// ########################
//    @vesact template
// ########################

import {
  // Fuse modules
  FuseModule,
  FuseSharedModule,
  FuseProgressBarModule,
  FuseSidebarModule,
  FuseThemeOptionsModule,
  ToolbarModule,
  WebUiTemplateModule,
  WEB_UI_TEMPLATE_CONFIG,

  // Layout
  DefaultLayoutModule,
  CardPageModule,
  EmptyPageModule,
  AuthPagesModule,
  MaintenancePageModule,

  // Guards
  AuthGuard,
  MaintenanceGuard,

  // Services
  AuthService_B2C,
  TranslationService,

  // Pipes
  VesactDatePipeModule,
  VesactDateFromNowPipeModule,
  VesactDateFromTodayPipeModule,
} from "@vesact/web-ui-template";

// ########################
//       Application
// ########################

// App config
import {
  appConfig,
  getBackendRootUrl,
  getCurrentEnvironment,
  getMsalLogLevel,
} from "@app/app.config";
import { environment } from "@environments/environment";

// Application Startpoint:
import { AppComponent } from "./app.component";

// Routing:
import { AppRoutingModule } from "./app-routing.module";

// Guards:
import { AccountReadyGuard } from "@guards/account-ready.guard";

// Layout:
import { PageHeaderComponent } from "@app/layout/page-header/page-header.component";

// COMPOSANTS DE PRÉVISUALISATION:
import { DevicePreviewDialogComponent } from './components/device-preview-dialog/device-preview-dialog.component';
import { NewsPreviewExactComponent } from './components/news-preview/news-preview-exact.component';

// Components
import {
  BreadcrumbComponent,
  CategoriesInputCardComponent,
  ConfirmationDialogComponent,
  ContentSearchCardComponent,
  LoadingSpinnerComponent,
  RequiredStarComponent,
  SnackbarComponent
} from "@components/index";

// Pages:
import {
  AccountNotReadyComponent,
  ContentManagementComponent,
  InfoscreenConfigComponent,
  InfoscreenEditCardComponent,
  InfoscreenEditCardActionButtonsComponent,
  InfoscreensComponent,
  InfoscreensCardComponent,
  NewsEditComponent,
  NewsManagementComponent,
  SelectAddSlideComponent,
  VideoEditComponent,
  VideoManagementComponent,
} from "@pages/index";

// Services:
import {
  ApiService,
  ConfigService,
  DataService,
  SnackbarService,
  UserService,
} from "@services/index";
import { ApplicationInsightsService } from "./services/app-insights.service";
import { NewsPreviewService } from './services/news-preview.service';

// Adapting config to environment
var envConfig = appConfig;
envConfig.auth.uri = getBackendRootUrl(getCurrentEnvironment());

const isIE =
  window.navigator.userAgent.indexOf("MSIE ") > -1 ||
  window.navigator.userAgent.indexOf("Trident/") > -1;

export function loggerCallback(logLevel: MsalLogLevel, message: string) {
  console.log(message);
}

export function MSALInstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: "af4d9b83-86a8-4da5-8b35-5f08f1ce9c82",
      authority: envConfig.auth.policies.find(
        (p) => p.name == "B2C_1_SUSI_VINCI-ENERGIES"
      )?.authority,
      redirectUri: "/",
      navigateToLoginRequestUrl: true,
      postLogoutRedirectUri: "/auth/logout",
      knownAuthorities: [envConfig.auth.authorityDomain],
    },
    cache: {
      cacheLocation: BrowserCacheLocation.LocalStorage,
      storeAuthStateInCookie: isIE,
    },
    system: {
      loggerOptions: {
        loggerCallback,
        logLevel: getMsalLogLevel(),
        piiLoggingEnabled: false,
      },
    },
  });
}

export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, Array<string>>();
  protectedResourceMap.set(envConfig.auth.uri, envConfig.auth.scopes);

  return {
    interactionType: InteractionType.Redirect,
    protectedResourceMap,
  };
}

export function MSALGuardConfigFactory(): MsalGuardConfiguration {
  return {
    interactionType: InteractionType.Redirect,
    authRequest: {
      scopes: envConfig.auth.scopes,
    },
  };
}

@NgModule({
  declarations: [
    // Application:
    AppComponent,
    PageHeaderComponent,

    // COMPOSANTS DE PRÉVISUALISATION:
    DevicePreviewDialogComponent,
    NewsPreviewExactComponent,

    // Components:
    BreadcrumbComponent,
    CategoriesInputCardComponent,
    ConfirmationDialogComponent,
    ContentSearchCardComponent,
    LoadingSpinnerComponent,
    RequiredStarComponent,
    SnackbarComponent,

    // Pages:
    AccountNotReadyComponent,
    ContentManagementComponent,
    InfoscreenConfigComponent,
    InfoscreenEditCardComponent,
    InfoscreenEditCardActionButtonsComponent,
    InfoscreensComponent,
    InfoscreensCardComponent,
    NewsEditComponent,
    NewsManagementComponent,
    SelectAddSlideComponent,
    VideoEditComponent,
    VideoManagementComponent,
  ],
  imports: [
    // Angular:
    BrowserModule,
    BrowserAnimationsModule,
    DragDropModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    PdfViewerModule,
    TranslateModule.forRoot(),

    // Angular Material COMPLET:
    MatAutocompleteModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatIconModule,
    MatMenuModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatSelectModule,
    MatExpansionModule,
    MatInputModule,
    MatChipsModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCardModule,
    MatDividerModule,
    MatPaginatorModule,
    MatSortModule,
    MatTableModule,
    MatDialogModule,
    MatSnackBarModule,
    MatRadioModule,
    MatTooltipModule,
    MatSliderModule,

    // Routing:
    AppRoutingModule,

    // Fuse modules
    FuseModule.forRoot(envConfig.fuseConfig),
    FuseProgressBarModule,
    FuseSharedModule,
    FuseSidebarModule,
    FuseThemeOptionsModule,
    ToolbarModule,
    WebUiTemplateModule,

    // Layout
    DefaultLayoutModule,
    CardPageModule,
    EmptyPageModule,
    AuthPagesModule,
    MaintenancePageModule,

    // MSAL
    MsalModule,
  ],
  providers: [
    // --- @vesact ---

    // Guards
    AuthGuard,
    MaintenanceGuard,

    // Services
    AuthService_B2C,
    TranslationService,

    DatePipe,
    // Authentication Azure B2C
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true,
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory,
    },
    {
      provide: MSAL_GUARD_CONFIG,
      useFactory: MSALGuardConfigFactory,
    },
    {
      provide: MSAL_INTERCEPTOR_CONFIG,
      useFactory: MSALInterceptorConfigFactory,
    },
    {
      provide: WEB_UI_TEMPLATE_CONFIG,
      useValue: envConfig,
    },
    MsalService,
    MsalGuard,
    MsalBroadcastService,

    // --- app ---
    // Guards:
    AccountReadyGuard,

    // Services:
    ApiService,
    ConfigService,
    DataService,
    SnackbarService,
    UserService,
    ApplicationInsightsService,
    NewsPreviewService,
  ],
  bootstrap: [AppComponent, MsalRedirectComponent],
})
export class AppModule {}