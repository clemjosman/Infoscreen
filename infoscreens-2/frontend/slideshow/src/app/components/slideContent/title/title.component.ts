import { Component, Input } from "@angular/core";

@Component({
  templateUrl: "./title.html",
  styleUrls: ["./title.scss"],
  selector: "title-content",
})
export class TitleContentComponent {
  @Input() title: string;
  @Input() theme: "light" | "dark" = "light";

  constructor() {}
}
