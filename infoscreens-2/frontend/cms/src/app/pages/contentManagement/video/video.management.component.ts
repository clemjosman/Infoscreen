import { Component, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { SelectionModel } from "@angular/cdk/collections";
import { MatTableDataSource } from "@angular/material/table";
import { MatSort, Sort } from "@angular/material/sort";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatDialog } from "@angular/material/dialog";
import moment from "moment";

import {
  IConfirmationDialogData,
  ConfirmationDialogComponent,
  ContentSearchParameters,
} from "@components/index";
import {
  ApiService,
  DataService,
  SnackbarService,
  UserService,
} from "@services/index";
import { apiVideo, apiInfoscreen_Light } from "@models/index";
import { TranslationService } from "@vesact/web-ui-template";

import {ApplicationInsightsService} from "../../../services/app-insights.service";

interface apiVideo_Table extends apiVideo {
  hasExpired: boolean;
}

interface VideosListFilters extends ContentSearchParameters {
  pageSize: number;
  pageIndex: number;
  sortActive: string;
  sortDirection: string;
}

const DEFAULT_VIDEOS_FILTERS: VideosListFilters = {
  pageIndex: 1,
  pageSize: 10,
  searchText: "",
  selectedCategoryIds: [],
  selectedInfoscreenIds: [],
  sortActive: "status",
  sortDirection: "desc",
};
const VIDEOS_FILTERS_KEY_LOCAL_STORAGE: string = "VIDEOS_LIST_FILTERS";

@Component({
  templateUrl: "./video.management.component.html",
  styleUrls: ["./video.management.component.scss"],
})
export class VideoManagementComponent implements OnInit {
  public pathArray: string[] = [
    "menuItem.contentManagement.group",
    "menuItem.contentManagement.videos",
  ];
  isLoading: boolean = true;
  fullPageLoading: boolean = false;
  filters: VideosListFilters = DEFAULT_VIDEOS_FILTERS;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  dataSource = new MatTableDataSource<apiVideo_Table>([]);
  displayedColumns = [
    "select",
    "title",
    "description",
    "publicationDate",
    "status",
    "actions",
  ];
  selection: SelectionModel<apiVideo_Table>;

  constructor(
    private dataService: DataService,
    private router: Router,
    private apiService: ApiService,
    private userService: UserService,
    public dialog: MatDialog,
    private snackbarService: SnackbarService,
    private _translationService: TranslationService,
    private _appInsightsService: ApplicationInsightsService
  ) {
    this._appInsightsService.logPageView('Video Management', '/contentManagement/videos')
  }

  async ngOnInit() {
    this.isLoading = true;
    this.fullPageLoading = false;

    const initialSelection = [];
    const allowMultiSelect = true;
    this.selection = new SelectionModel<apiVideo_Table>(
      allowMultiSelect,
      initialSelection
    );

    this.initTable();
    try {
      this.filters = {
        ...this.filters,
        ...JSON.parse(localStorage.getItem(VIDEOS_FILTERS_KEY_LOCAL_STORAGE)),
      };

      this.updateTableDataSource(
        await this.apiService.getFilteredVideosAsync(this.filters)
      );
    } catch (error) {
    } finally {
      this.isLoading = false;
    }
  }

  initTable() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.sortingDataAccessor = (
      item: apiVideo_Table,
      property: string
    ) => {
      switch (property) {
        case "title":
          return this.getVideoTitle(item).toLocaleLowerCase();
        case "description":
          return item.description?.toLocaleLowerCase() || "";
        case "publicationDate":
          return item.publicationDate;
        case "status":
          // We want to separate visible, expired and non-visible videos.
          // By dividing the expired by 1000 we put them at the end
          // By multiplying the visible by 1000 we put them at the beginning and let the planned ones in-between
          if (item.isVisible && item.hasExpired)
            return moment(item.publicationDate).unix() / 1000;
          else if (item.isVisible && !item.hasExpired)
            return moment(item.publicationDate).unix() * 1000;
          else return moment.utc(item.publicationDate).unix();
        default:
          return item.id;
      }
    };
  }

  hasExpired(video: apiVideo): boolean {
    return (
      moment.utc().isSameOrAfter(moment(video.expirationDate)) &&
      video.isVisible
    );
  }

  updateTableDataSource(videos: apiVideo[]) {
    this.dataSource.data = videos.map((v) => {
      return { ...v, hasExpired: this.hasExpired(v) };
    });
    const maxPageIndex = Math.floor(videos.length / this.filters.pageSize);
    const requestedIndex = this.filters?.pageIndex || 0;
    const newIndex = Math.min(requestedIndex, maxPageIndex);
    this.paginator.pageIndex = newIndex;
    this.dataSource.paginator = this.paginator;
  }

  createNew() {
    this.router.navigateByUrl("/contentManagement/videos/new");
  }

  edit(video: apiVideo) {
    this.router.navigateByUrl(`/contentManagement/videos/${video.id}`);
  }

  search = async (searchParams: ContentSearchParameters) => {
    this.isLoading = true;
    this.updateTableDataSource([]);
    try {
      const newFilters: VideosListFilters = {
        ...this.filters,
        pageIndex: 0, // After search changes, restart on first page
        ...searchParams,
      };
      this._updateFilters(newFilters);
      this.updateTableDataSource(
        await this.apiService.getFilteredVideosAsync(this.filters)
      );
    } catch (error) {
    } finally {
      this.isLoading = false;
    }
  };

  async delete(video: apiVideo) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      closeOnNavigation: true,
      data: {
        actionTitle: "video.delete.confirmation.title",
        actionButton: "general.button.delete",
        message: "video.delete.confirmation.message",
      } as IConfirmationDialogData,
    });

    dialogRef.afterClosed().subscribe(async (proceed) => {
      if (proceed) {
        try {
          await this.apiService.deleteVideoAsync(video.id);
          this.updateTableDataSource(
            this.dataService.deleteVideoLocally(video.id)
          );

          this.snackbarService.displaySuccessSnackbar("video.delete.success");
        } catch (error) {}
      }
    });
  }

  getVideoTitle(video: apiVideo): string {
    // By default try to use ui language
    var language = this._translationService.currentLanguage.value?.iso2;

    // If language not available for the video, display the first in the list
    if (!video.title[language]) {
      language = Object.getOwnPropertyNames(video.title)[0];
    }

    return video.title[language];
  }

  getVideoLastEditDate(video: apiVideo): string {
    // By default try to use the last edit date, if not available use creation date
    return video.lastEditDate || video.creationDate;
  }

  getVideoLastEditor(video: apiVideo): string {
    // By default try to use the last edit user, if not available use creator
    return video.lastEditor?.displayName || video.creator.displayName;
  }

  isToday(date: string): boolean {
    return moment(date).isSame(moment.utc(), "day");
  }

  isInTheFuture(date: string): boolean {
    return moment(date).isAfter(moment.utc());
  }

  //*************************
  //      CHECK BOXES
  //*************************

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected == numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected()
      ? this.selection.clear()
      : this.dataSource.data.forEach((row) => this.selection.select(row));
  }

  async deleteSelected() {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      closeOnNavigation: true,
      data: {
        actionTitle: "video.deleteList.confirmation.title",
        actionButton: "general.button.delete",
        message: "video.deleteList.confirmation.message",
      } as IConfirmationDialogData,
    });

    dialogRef.afterClosed().subscribe(async (proceed) => {
      if (proceed) {
        try {
          this.fullPageLoading = true;
          var videoIds = this.selection.selected.map((v) => v.id);
          await this.apiService.deleteMultipleVideosAsync(videoIds);
          this.updateTableDataSource(
            this.dataService.deleteMultipleVideosLocally(videoIds)
          );
          this.selection.clear();

          this.snackbarService.displaySuccessSnackbar(
            "video.deleteList.success"
          );
        } catch (error) {
        } finally {
          this.fullPageLoading = false;
        }
      }
    });
  }

  private _updateFilters(filters: VideosListFilters) {
    this.filters = { ...this.filters, ...filters };
    localStorage.setItem(
      VIDEOS_FILTERS_KEY_LOCAL_STORAGE,
      JSON.stringify(filters)
    );
  }

  onSortChange(event: Sort): void {
    const newFilters = {
      ...this.filters,
      sortActive: event.active,
      sortDirection: event.direction,
    };
    this._updateFilters(newFilters);
  }

  onPageChange(event: PageEvent): void {
    const newFilters = {
      ...this.filters,
      pageIndex: event.pageIndex,
      pageSize: event.pageSize,
    };
    this._updateFilters(newFilters);
  }
}
