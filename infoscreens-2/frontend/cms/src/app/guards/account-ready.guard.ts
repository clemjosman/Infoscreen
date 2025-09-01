import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService_B2C } from '@vesact/web-ui-template';

import { UserService } from '@services/index';

@Injectable()
export class AccountReadyGuard implements CanActivate {

  constructor(private userService: UserService, private router: Router, private authService: AuthService_B2C)
  { }

  async canActivate(): Promise<boolean> {
    if(await this.authService.isAuthenticated()){
      // User is logged in and claims are available

      if (await this.userService.isUserAccountReadyAsync()) {
        // Logged in user is known and may access

        return true;

      } else {
        // Logged in user is unknown
        // -> redirect user

        this.router.navigateByUrl('/accountNotReady');
        return false;
      }
    }
    else{
      return false;
    }
  }
}