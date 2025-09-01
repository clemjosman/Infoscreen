import { Component, OnInit } from "@angular/core";
import {
  trigger,
  transition,
  animate,
  style,
  query,
  group,
} from "@angular/animations";
import { filter } from "rxjs/operators";
import { TranslationPipe } from "./pipes/translation.pipe";
import { ConfigService } from "./services/config.service";
import { BrandingService } from "./services/branding.service";
import { RoutingService } from "./services/routing.service";
import { InternetAccessService, LoggingService } from "./services";
import { ThemeService } from "./services/theme.service";

@Component({
  animations: [
    trigger("routeAnimations", [
      transition("* <=> *", [
        query(":enter, :leave", style({ position: "fixed", width: "100%" }), {
          optional: true,
        }),
        group([
          query(
            ":enter",
            [
              style({ opacity: "0" }),
              animate("1s ease-in-out", style({ opacity: "0" })),
              animate("2s ease-in-out", style({ opacity: "1" })),
            ],
            { optional: true }
          ),
          query(
            ":leave",
            [
              style({ opacity: "1" }),
              animate("0s ease-in-out", style({ opacity: "1" })),
              animate("1s ease-in-out", style({ opacity: "0" })),
            ],
            { optional: true }
          ),
        ]),
      ]),
    ]),
  ],
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  providers: [TranslationPipe],
})
export class AppComponent implements OnInit {
  isRoutingAnimationsDisabled: boolean = false;

  constructor(
    private routingService: RoutingService,
    private configService: ConfigService,
    private themeService: ThemeService,
    private internetAccessService: InternetAccessService,
    private brandingService: BrandingService
  ) {}

  async ngOnInit() {
    await this.configService.initAsync();
    this.brandingService.init();
    this.themeService.init();
    this.internetAccessService.init();
    this.routingService.startRoutingLoop();

    // Subscribing and logging here as doing it in the ConfigService would create a dependency loop with the LoggingService
    ConfigService.config$
      .pipe(filter((config) => !!config))
      .subscribe((newConfig) => {
        LoggingService.trace(
          ConfigService.name,
          "refreshConfig",
          "Using config: " + newConfig
        );
        this.isRoutingAnimationsDisabled = !!newConfig.disableAnimations;
      });
  }
}
