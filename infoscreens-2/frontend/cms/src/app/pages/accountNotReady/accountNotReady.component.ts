import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';

import { UserService } from '@services/index';

import {ApplicationInsightsService} from "../../services/app-insights.service";

@Component({
  templateUrl: './accountNotReady.component.html',
  styleUrls: ['./accountNotReady.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AccountNotReadyComponent implements OnInit{

  isLoading: boolean = true;
  isNotReady: boolean = true;

  constructor(
    private userService: UserService,
    private router: Router,
    private _appInsightsService: ApplicationInsightsService
  ) {
    this._appInsightsService.logPageView('Account Not Ready', '/accountNotReady')
  }

  async ngOnInit(){
    this.isNotReady = !(await this.userService.isUserAccountReadyAsync());
    this.isLoading = false;
  }

  logout(){
    this.router.navigateByUrl('/auth/logout');
  }

  goToHomePage(){
    this.router.navigateByUrl('/');
  }
}