import { Component, Input } from "@angular/core";

@Component({
  templateUrl: "./image.html",
  styleUrls: ["./image.scss"],
  selector: "image-content",
})
export class ImageContentComponent {
  @Input() url: string;
  constructor() {}
}
