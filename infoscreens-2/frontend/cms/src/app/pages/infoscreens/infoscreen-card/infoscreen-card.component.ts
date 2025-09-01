import { Component, Input } from "@angular/core";
import { Router } from "@angular/router";

import { apiInfoscreen_Light } from "@models/index";
import { InfoscreenCachedStatus } from "@models/index";

@Component({
  selector: "infoscreen-card",
  templateUrl: "./infoscreen-card.component.html",
  styleUrls: ["./infoscreen-card.component.scss"],
})
export class InfoscreensCardComponent {
  @Input() infoscreen: apiInfoscreen_Light = undefined;
  @Input() status: InfoscreenCachedStatus = undefined;

  constructor(private _router: Router) {}

  public config(infoscreen: apiInfoscreen_Light) {
    this._router.navigateByUrl(`/infoscreen/${infoscreen.id}/config`);
  }
}
