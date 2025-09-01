// cms/src/app/app-routing.module.ts
import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

// --- @vesact ---
import {
  // Auth pages
  AuthLoginComponent,
  AuthLoginPortalComponent,
  AuthLogoutComponent,
  AuthUserUnknownComponent,
  Page404Component,

  // Guards
  AuthGuard,
  MaintenanceGuard,
  MaintenancePageComponent,
} from "@vesact/web-ui-template";

// --- App---

// Auth related
import { AccountNotReadyComponent } from "@pages/index";

// COMPOSANT DE PRÉVISUALISATION
import { NewsPreviewExactComponent } from './components/news-preview/news-preview-exact.component';

// Pages
import {
  InfoscreenConfigComponent,
  InfoscreensComponent,
  ContentManagementComponent,
  NewsEditComponent,
  NewsManagementComponent,
  VideoEditComponent,
  VideoManagementComponent,
} from "@pages/index";
import { AccountReadyGuard } from "./guards/account-ready.guard";

const routes: Routes = [
  // --- @vesact ---
  // Auth related
  {
    path: "auth/logout",
    component: AuthLogoutComponent,
  },
  {
    path: "auth/login",
    component: AuthLoginPortalComponent,
  },
  {
    path: "auth/login/:policyCode",
    component: AuthLoginComponent,
  },
  {
    path: "auth/user-unknown",
    component: AuthUserUnknownComponent,
  },

  // Maintenance
  {
    path: "maintenance",
    component: MaintenancePageComponent,
  },

  // --- App ---
  {
    path: "accountNotReady",
    component: AccountNotReadyComponent,
  },

  // Infoscreens related
  {
    path: "infoscreens",
    component: InfoscreensComponent,
    canActivate: [AuthGuard, MaintenanceGuard, AccountReadyGuard],
  },

  {
    path: "infoscreen/:id",
    redirectTo: "infoscreen/:id/config",
    pathMatch: "full",
  },
  {
    path: "infoscreen/:id/config",
    component: InfoscreenConfigComponent,
    canActivate: [AuthGuard, MaintenanceGuard, AccountReadyGuard],
  },

  // Content management related
  {
    path: "contentManagement",
    component: ContentManagementComponent,
    canActivate: [AuthGuard, MaintenanceGuard, AccountReadyGuard],
    children: [
      {
        path: "",
        redirectTo: "news",
        pathMatch: "full",
      },
      {
        path: "news",
        component: NewsManagementComponent,
        canActivate: [AuthGuard, MaintenanceGuard, AccountReadyGuard],
      },
      {
        path: "videos",
        component: VideoManagementComponent,
        canActivate: [AuthGuard, MaintenanceGuard, AccountReadyGuard],
      },
    ],
  },

  {
    path: "contentManagement/news/new",
    component: NewsEditComponent,
    canActivate: [AuthGuard, MaintenanceGuard, AccountReadyGuard],
    data: { isNew: true },
  },
  {
    path: "contentManagement/news/:newsId",
    component: NewsEditComponent,
    canActivate: [AuthGuard, MaintenanceGuard, AccountReadyGuard],
    data: { isNew: false },
  },

  {
    path: "contentManagement/videos/new",
    component: VideoEditComponent,
    canActivate: [AuthGuard, MaintenanceGuard, AccountReadyGuard],
    data: { isNew: true },
  },
  {
    path: "contentManagement/videos/:videoId",
    component: VideoEditComponent,
    canActivate: [AuthGuard, MaintenanceGuard, AccountReadyGuard],
    data: { isNew: false },
  },

  // ✅ ROUTE DE PRÉVISUALISATION SLIDESHOW
  {
    path: 'news-preview-exact',
    component: NewsPreviewExactComponent
  },

  // Redirect when no path
  {
    path: "",
    redirectTo: "contentManagement/news",
    pathMatch: "full",
  },

  // Redirect if route wasn't found:
  {
    path: "**",
    component: Page404Component,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: false })],
  exports: [RouterModule],
})
export class AppRoutingModule {}