import { Component, Input } from "@angular/core";

@Component({
  templateUrl: "./slideBubble.html",
  styleUrls: ["./slideBubble.scss"],
  selector: "slideBubble",
})
export class SlideBubbleComponent {
  @Input() title: string;

  constructor() {}
}
