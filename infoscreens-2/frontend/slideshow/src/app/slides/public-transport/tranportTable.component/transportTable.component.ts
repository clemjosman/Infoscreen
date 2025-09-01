import { Component, Input } from "@angular/core";

@Component({
  selector: "transportTable",
  templateUrl: "./transportTable.html",
  styleUrls: ["./transportTable.scss"],
})
export class TransportTableComponent {
  @Input() stationBoard: any;
  @Input() maxDisplayed: number = 3;

  constructor() {}
}
