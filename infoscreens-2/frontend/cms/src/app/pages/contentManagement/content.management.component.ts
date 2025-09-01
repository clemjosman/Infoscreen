import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  templateUrl: './content.management.component.html',
  styleUrls: ['./content.management.component.scss']
})
export class ContentManagementComponent{

  constructor(private router: Router, private route: ActivatedRoute) { }

  openNews(){
    this.router.navigate(['./news'], {relativeTo: this.route});
  }

  openVideos()
  {
    this.router.navigate(['./videos'], {relativeTo: this.route});
  }
}