import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { TranslateService } from "@ngx-translate/core";

import {
  SnackbarComponent,
  ISnackbarData,
  SnackbarTypes,
} from "@components/snackbar/snackbar.component";

@Injectable()
export class SnackbarService {
  constructor(
    private snackBar: MatSnackBar,
    private translateService: TranslateService
  ) {}

  public displaySuccessSnackbar(message: string, duration: number = 5000) {
    message = this.translateService.instant(message);
    this.displaySnackbar(
      {
        message,
        type: SnackbarTypes.success,
      },
      duration
    );
  }

  public displayErrorSnackbar(message: string, duration: number = 5000) {
    message = this.translateService.instant(message);
    this.displaySnackbar(
      {
        message,
        type: SnackbarTypes.error,
      },
      duration
    );
  }

  private displaySnackbar(data: ISnackbarData, duration: number = 5000) {
    this.snackBar.openFromComponent(SnackbarComponent, {
      duration,
      data,
    });
  }
}
