import { Component, OnDestroy, OnInit } from "@angular/core";
import { InternetAccessService } from "../../services";
import { takeUntil } from "rxjs/operators";
import { ReplaySubject } from "rxjs";

@Component({
  templateUrl: "./internet-status.html",
  styleUrls: ["./internet-status.scss"],
  selector: "internet-status",
})
export class InternetStatusComponent implements OnInit, OnDestroy {
  private _destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

  isConnected: boolean = true;

  constructor() {}

  ngOnInit() {
    InternetAccessService.isConnected$
      .pipe(takeUntil(this._destroyed$))
      .subscribe((isConnected) => {
        this.isConnected = isConnected;
      });
  }

  ngOnDestroy() {
    this._destroyed$.next(true);
    this._destroyed$.complete();
  }
}
