import { Component, Input } from "@angular/core";
import { TranslationPipe } from "../../pipes/translation.pipe";
import { InfoscreenNodeStatus } from "../../models/data/infoscreenNodeStatus";

@Component({
  templateUrl: "./infoscreenNodeStatusCard.html",
  styleUrls: ["./infoscreenNodeStatusCard.scss"],
  selector: "infoscreenNodeStatusCard",
})
export class InfoscreenNodeStatusCard {
  @Input() nodeStatus: InfoscreenNodeStatus | undefined = undefined;
  constructor(private translationPipe: TranslationPipe) {}

  ngOnInit() {}
}
