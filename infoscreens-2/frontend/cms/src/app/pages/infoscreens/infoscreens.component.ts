import { Component, OnInit } from "@angular/core";

import { DataService } from "@services/index";
import {
  apiInfoscreen_Status,
  apiInfoscreen_Light,
  InfoscreenCachedStatus,
  apiInfoscreenGroup,
} from "@models/index";

@Component({
  templateUrl: "./infoscreens.component.html",
  styleUrls: ["./infoscreens.component.scss"],
})
export class InfoscreensComponent implements OnInit {
  public pathArray: string[] = ["menuItem.infoscreens"];

  isLoading: boolean = true;
  public infoscreens: apiInfoscreen_Light[] = undefined;
  public infoscreensStatus: apiInfoscreen_Status[] = undefined;
  public infoscreenGroups: apiInfoscreenGroup[] = undefined;

  constructor(private dataService: DataService) {}

  async ngOnInit() {
    this.refresh(false);
  }

  async refresh(forceRefresh: boolean) {
    this.isLoading = true;
    this.infoscreens = undefined;
    this.infoscreensStatus = undefined;
    this.infoscreenGroups = undefined;

    this.dataService
      .getInfoscreensStatusAsync(true)
      .then((status) => (this.infoscreensStatus = status))
      .catch((ex) => console.error(ex));

    try {
      [this.infoscreens, this.infoscreenGroups] = await Promise.all([
        this.dataService.getInfoscreensAsync(forceRefresh),
        this.dataService.getInfoscreenGroupsAsync(forceRefresh),
      ]);
    } catch (ex) {
    } finally {
      this.isLoading = false;
    }
  }

  getInfoscreenStatus(infoscreen: apiInfoscreen_Light): InfoscreenCachedStatus {
    return this.infoscreensStatus.find((is) => is.id === infoscreen.id)?.status;
  }

  getInfoscreensOfGroup(groupId: number): apiInfoscreen_Light[] {
    return this.infoscreens.filter((i) => i.infoscreenGroupId == groupId);
  }
}
