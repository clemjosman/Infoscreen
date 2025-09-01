import { Component, Input } from "@angular/core";

@Component({
  templateUrl: "./splitContent.html",
  styleUrls: ["./splitContent.scss"],
  selector: "split-content",
})
export class SplitContentComponent {
  @Input() layout: "vertical" | "horizontal" = "horizontal";
  @Input() firstContentProportion: number | "fitFirst" | "fitSecond" = 30;
  @Input() invertContents: boolean = false;
  constructor() {}

  getContentProportion(content: "first" | "second"): string {
    const isForFirstContent = content === "first";
    if (this.firstContentProportion === "fitFirst") {
      return isForFirstContent ? "fit-content" : "auto";
    } else if (this.firstContentProportion === "fitSecond") {
      return isForFirstContent ? "auto" : "fit-content";
    } else {
      if (this.firstContentProportion < 0) {
        return isForFirstContent ? "0%" : "100%";
      } else if (this.firstContentProportion > 100) {
        return isForFirstContent ? "100%" : "0%";
      } else {
        return isForFirstContent
          ? `${this.firstContentProportion}%`
          : `${100 - this.firstContentProportion}%`;
      }
    }
  }

  getFlexGrow(content: "first" | "second"): string {
    const isForFirstContent = content === "first";
    if (this.firstContentProportion === "fitFirst") {
      return isForFirstContent ? "0" : "1";
    } else if (this.firstContentProportion === "fitSecond") {
      return isForFirstContent ? "1" : "0";
    } else {
      return "0";
    }
  }
}
