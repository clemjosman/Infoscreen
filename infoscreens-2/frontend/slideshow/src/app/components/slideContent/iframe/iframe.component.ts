import { Component, Input } from "@angular/core";

@Component({
  templateUrl: "./iframe.html",
  styleUrls: ["./iframe.scss"],
  selector: "iframe-content",
})
export class IframeContentComponent {
  @Input() url: string;
  constructor() {}
}
