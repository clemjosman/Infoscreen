import { Component, Input } from "@angular/core";

@Component({
  selector: "infoscreen-edit-card",
  templateUrl: "./edit-card.component.html",
  styleUrls: ["./edit-card.component.scss"],
})
export class InfoscreenEditCardComponent {
  @Input() isMetadata: boolean = false;
  @Input() displayButtonsHeader: boolean = true;
  @Input() headerLabel: string = "";
  @Input("cancel") cancelCallback: () => any = undefined;
  @Input() disableCancel: boolean = true;
  @Input("save") saveCallback: () => any = undefined;
  @Input() disableSave: boolean = true;

  constructor() {}

  public cancel() {
    if (this.cancelCallback) this.cancelCallback();
  }

  public save() {
    if (this.saveCallback) this.saveCallback();
  }
}
