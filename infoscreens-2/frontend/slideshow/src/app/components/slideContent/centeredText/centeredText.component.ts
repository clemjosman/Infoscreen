import { Component, Input } from "@angular/core";

@Component({
  templateUrl: "./centeredText.html",
  styleUrls: ["./centeredText.scss"],
  selector: "centered-text-content",
})
export class CenteredTextContentComponent {
  @Input() textCode: string;
  constructor() {}
}
