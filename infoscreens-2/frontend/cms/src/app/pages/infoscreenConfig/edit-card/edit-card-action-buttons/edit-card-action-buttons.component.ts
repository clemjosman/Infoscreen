import { Component, Input } from "@angular/core";

@Component({
  selector: "infoscreen-edit-card-action-buttons",
  templateUrl: "./edit-card-action-buttons.component.html",
  styleUrls: ["./edit-card-action-buttons.component.scss"],
})
export class InfoscreenEditCardActionButtonsComponent {
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
