import { Component, Input } from "@angular/core";

@Component({
  templateUrl: "./localVideo.html",
  styleUrls: ["./localVideo.scss"],
  selector: "local-video-content",
})
export class LocalVideoContentComponent {
  @Input() source: string;
  constructor() {}
}
