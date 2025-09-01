import { Component, ViewEncapsulation, Input } from "@angular/core";

@Component({
  selector: "page-header",
  templateUrl: "./page-header.component.html",
  styleUrls: ["./page-header.component.scss"],
})
export class PageHeaderComponent {
  @Input()
  displayBreadcrumbBackButton: boolean = true;

  @Input()
  backUrl: string = undefined;

  @Input()
  pathArray: string[];

  @Input()
  contentOnNewLine: boolean = true;

  constructor() {}
}
