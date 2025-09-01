import { UiLanguage, WebUiTemplateConfig } from "@vesact/web-ui-template";
import { LogLevel as MsalLogLevel } from "@azure/msal-browser";

// LANGUAGE SERVICE //
import static_EN from "../assets/data/staticEN.json";
import static_DE from "../assets/data/staticDE.json";
import static_FR from "../assets/data/staticFR.json";
import static_IT from "../assets/data/staticIT.json";
export const APP_CODE = "INFOSCREENS_CMS";
export const DEFAULT_UI_LANGUAGE: UiLanguage = {
  iso2: "en",
  flagPath: "assets/flags/gb.svg",
  languageName: "English",
  cultureName: "en-GB",
  flagCode: "en",
  languageId: 4,
};

export const getLanguageServiceRootUrl = (): string => {
  let environment = getCurrentEnvironment();
  switch (environment) {
    case Environments.PRODUCTION:
      return "https://cluster.actemium.ch/";
    case Environments.STAGING:
      return "https://cluster.actemium.ch/";
    case Environments.TEST:
      return "https://cluster.actemium.ch/";
    case Environments.DEV:
      return "https://cluster.actemium.ch/";
    default:
      return undefined;
  }
};

export const getStaticLabels = (
  iso2: string
): { [textCode: string]: string } => {
  switch (iso2.toLowerCase()) {
    case "en":
      return static_EN;
    case "de":
      return static_DE;
    case "fr":
      return static_FR;
    case "it":
      return static_IT;
    default:
      throw `Requested language with iso2 "${iso2}" is not supported`;
  }
};

export const uiLanguageFlagSetup = (language: UiLanguage): UiLanguage => {
  language.flagPath = `assets/flags/${language.flagCode}.svg`;
  return language;
};

// BACKEND CONFIG //
//export const CLUSTER_DEV: string = "http://localhost:7072/";
export const CLUSTER_DEV: string = "https://infoscreens-backend-staging.actemium.ch/";
export const CLUSTER_TEST: string = "https://infoscreens-backend-staging.actemium.ch/";
export const CLUSTER_STAG: string = "https://infoscreens-backend-staging.actemium.ch/";
export const CLUSTER_PROD: string = "https://infoscreens-backend.actemium.ch/";
export const API_BASEPATH: string = "api/";

// HOST CONFIG //
export const HOSTS_TEST: string[] = ["infoscreens-test.actemium.ch"];
export const HOSTS_STAG: string[] = ["infoscreens-staging.actemium.ch"];
export const HOSTS_PROD: string[] = ["infoscreens.actemium.ch"];

// ENVIRONMENT CONFIG //
export enum Environments {
  PRODUCTION = "production",
  STAGING = "staging",
  TEST = "test",
  DEV = "dev",
}

export const isProductionEnv = (): boolean => {
  return HOSTS_PROD.indexOf(window.location.host) > -1;
};

export const isStagingEnv = (): boolean => {
  return HOSTS_STAG.indexOf(window.location.host) > -1;
};

export const isTestEnv = (): boolean => {
  return HOSTS_TEST.indexOf(window.location.host) > -1;
};

export const getCurrentEnvironment = (): Environments => {
  if (isProductionEnv()) {
    return Environments.PRODUCTION;
  } else if (isStagingEnv()) {
    return Environments.STAGING;
  } else if (isTestEnv()) {
    return Environments.TEST;
  } else {
    return Environments.DEV;
  }
};

export const getBackendRootUrl = (environment: string): string => {
  switch (environment) {
    case Environments.PRODUCTION:
      return `${CLUSTER_PROD}${API_BASEPATH}`;
    case Environments.STAGING:
      return `${CLUSTER_STAG}${API_BASEPATH}`;
    case Environments.TEST:
      return `${CLUSTER_TEST}${API_BASEPATH}`;
    case Environments.DEV:
      return `${CLUSTER_DEV}${API_BASEPATH}`;
    default:
      return "";
  }
};

export const getMsalLogLevel = (): MsalLogLevel => {
  return MsalLogLevel.Warning;
};

export const enableAngularProdMode = (): boolean => {
  return isProductionEnv() || isStagingEnv() || isTestEnv();
};

export const appConfig: WebUiTemplateConfig = {
  appName: "Infoscreens",
  appIconUrl: "assets/logos/actemium.png",
  appBackgroundUrl: "/assets/images/dark-material-bg.jpg",
  api: {
    getGlobalBackendEndpoint: getBackendRootUrl,
    getTenantBackendEndpoint: (
      environment: string,
      tenantCode: string
    ): string => {
      return `${getBackendRootUrl(environment)}/${tenantCode}`;
    },
  },
  environment: {
    getEnvironment: getCurrentEnvironment,
    isProductionEnvironment: (environment: string): boolean => {
      return environment === Environments.PRODUCTION;
    },
    validEnvironments: [
      Environments.PRODUCTION,
      Environments.STAGING,
      Environments.TEST,
      Environments.DEV,
    ],
  },
  isHybridApp: false,
  language: {
    APP_CODE,
    defaultIso2Language: DEFAULT_UI_LANGUAGE.iso2,
    languageServiceRootUrl: getLanguageServiceRootUrl(),
    validIso2Languages: ["en", "de", "fr", "it"],
    getStaticLabels,
    uiLanguageFlagSetup,
  },
  maintenance: {
    appName: "infoscreens-cms",
    refreshInterval_ms: 300000, // 5 minutes
    cacheLifespan_ms: 60000, // 1 minute
  },
  languageSwitches: [
    {
      iso2: "en",
      flagPath: "assets/flags/gb.svg",
    },
    {
      iso2: "de",
      flagPath: "assets/flags/de.svg",
    },
    {
      iso2: "fr",
      flagPath: "assets/flags/fr.svg",
    },
    {
      iso2: "it",
      flagPath: "assets/flags/it.svg",
    },
  ],
  tenant: {
    useLocalStorageTenantAsDefault: false,
  },
  auth: {
    rootUrlPath: "/auth/",
    defaultRedirectUrl: "/",
    defaultPolicyCode: undefined, //vinci
    providers: [
      {
        policyCode: "vinci",
        policyName: "B2C_1_SUSI_VINCI-ENERGIES",
        hidden: false,
        textCode: "Vinci Account",
        redirectUrl: "/",
      },
      {
        policyCode: "email",
        policyName: "B2C_1_SI_EMAIL",
        hidden: false,
        textCode: "Email",
      },
    ],
    scopes: [
      "https://actemiumch.onmicrosoft.com/Infoscreens_CMS_Backend/user_impersonation",
    ],
    uri: "---completed_by_env_variable---",
    authorityDomain: "actemiumch.b2clogin.com",
    policies: [
      {
        name: "B2C_1_SUSI_VINCI-ENERGIES",
        authority:
          "https://actemiumch.b2clogin.com/actemiumch.onmicrosoft.com/B2C_1_SUSI_VINCI-ENERGIES",
        domainHint: "vinci-energies.net",
        logoutFromIdp: {
          mandatory: false,
          idpLogoutLabel: "auth.logout.vinci",
          postLogoutRedirectUri:
            "https://login.microsoftonline.com/cae7d061-08f3-40dd-80c3-3c0b8889224a/oauth2/logout",
        },
      },
    ],
  },
  fuseConfig: {
    // Color themes can be defined in src/app/app.theme.scss
    colorTheme: "theme-default",
    customScrollbars: true,
    layout: {
      style: "vertical-layout-2",
      width: "fullwidth",
      navbar: {
        primaryBackground: "fuse-navy-700",
        secondaryBackground: "fuse-navy-900",
        folded: false,
        hidden: false,
        position: "left",
        variant: "vertical-style-2",
      },
      toolbar: {
        customBackgroundColor: false,
        background: "fuse-white-500",
        hidden: false,
        position: "below-static",
      },
      footer: {
        customBackgroundColor: true,
        background: "fuse-navy-900",
        hidden: true,
        position: "below-fixed",
      },
      sidepanel: {
        hidden: true,
        position: "right",
      },
    },
  },
};
