import { Component, Input } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { fuseAnimations } from "@vesact/web-ui-template";

@Component({
  selector: "breadcrumb",
  templateUrl: "./breadcrumb.component.html",
  styleUrls: ["./breadcrumb.component.scss"],
  animations: fuseAnimations,
})
export class BreadcrumbComponent {
  @Input()
  pathArray: string[];

  @Input()
  displayBackButton: boolean = true;

  @Input()
  backUrl: string = undefined;

  constructor(private router: Router, private route: ActivatedRoute) {}

  goBack() {
    if (!this.backUrl)
      this.router.navigate(["../"], { relativeTo: this.route });
    else this.router.navigateByUrl(this.backUrl);
  }
}
