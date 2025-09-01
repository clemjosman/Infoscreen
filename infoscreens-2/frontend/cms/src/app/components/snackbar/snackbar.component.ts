import { Component, Inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';

export enum SnackbarTypes {
  success = 'success',
  warning = 'warning',
  error = 'error'
}

export interface ISnackbarData {
  message: string;
  type: SnackbarTypes
}
 
@Component({
  selector: 'snackbar',
  templateUrl: './snackbar.component.html',
  styleUrls: ['./snackbar.component.scss']
})
export class SnackbarComponent{
  constructor(@Inject(MAT_SNACK_BAR_DATA) public data: ISnackbarData) {}

}