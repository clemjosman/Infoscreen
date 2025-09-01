import { Component } from '@angular/core';
import { fuseAnimations } from '@vesact/web-ui-template';

@Component({
  selector: 'loading-spinner',
  templateUrl: './loading-spinner.component.html',
  styleUrls: ['./loading-spinner.component.scss'],
  animations   : fuseAnimations
})
export class LoadingSpinnerComponent{

  constructor() {}
}