import { Component, Input, OnInit } from "@angular/core";
import { FormControl } from "@angular/forms";
import {
  apiCategory,
  apiInfoscreenGroup,
  apiInfoscreen_Light,
} from "@app/models";
import { DataService } from "../../../services/data.service";

export type ContentSearchParameters = {
  searchText: string;
  selectedInfoscreenIds: number[];
  selectedCategoryIds: number[];
};

@Component({
  selector: "content-search-card",
  templateUrl: "./content-search-card.component.html",
  styleUrls: ["./content-search-card.component.scss"],
})
export class ContentSearchCardComponent implements OnInit {
  @Input()
  isQueryOngoing: boolean = false;

  @Input()
  initialValues: ContentSearchParameters = undefined;

  @Input()
  onSearch: (searchParams: ContentSearchParameters) => any = undefined;

  searchText: string = undefined;

  selectedInfoscreenIds = new FormControl([]);
  groupedInfoscreens: {
    group: apiInfoscreenGroup;
    infoscreens: apiInfoscreen_Light[];
  }[] = [];

  selectedCategoryIds = new FormControl([]);
  categories: apiCategory[] = [];

  isLoading: boolean = true;

  constructor(private _dataService: DataService) {}

  async ngOnInit() {
    let infoscreenGroups: apiInfoscreenGroup[],
      infoscreens: apiInfoscreen_Light[],
      categories: apiCategory[];
    [infoscreenGroups, infoscreens, categories] = await Promise.all([
      this._dataService.getInfoscreenGroupsAsync(),
      this._dataService.getInfoscreensAsync(),
      this._dataService.getCategoriesAsync(),
    ]);

    infoscreenGroups = this._orderByName(infoscreenGroups);
    this.categories = this._orderByName(categories);

    for (let group of infoscreenGroups) {
      this.groupedInfoscreens.push({
        group,
        infoscreens: infoscreens.filter(
          (i) => i.infoscreenGroupId === group.id
        ),
      });
    }

    if (this.initialValues) {
      this.searchText = this.initialValues.searchText || "";
      this.selectedInfoscreenIds.setValue(
        this.initialValues.selectedInfoscreenIds || []
      );
      this.selectedCategoryIds.setValue(
        this.initialValues.selectedCategoryIds || []
      );
    }

    this.isLoading = false;
  }

  search() {
    if (this.onSearch)
      this.onSearch({
        searchText: this.searchText,
        selectedCategoryIds: this.selectedCategoryIds.value,
        selectedInfoscreenIds: this.selectedInfoscreenIds.value,
      });
  }

  clearFilters() {
    this.searchText = "";
    this.selectedInfoscreenIds.setValue([]);
    this.selectedCategoryIds.setValue([]);
    this.search();
  }

  private _orderByName<T extends { name: string }>(array: T[]): T[] {
    return array.sort((a, b) => {
      let aName = a.name.toLowerCase();
      let bName = b.name.toLowerCase();
      return aName === bName ? 0 : aName < bName ? -1 : 1;
    });
  }
}
