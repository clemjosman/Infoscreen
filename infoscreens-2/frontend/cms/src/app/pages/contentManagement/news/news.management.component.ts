import { Component, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { SelectionModel } from "@angular/cdk/collections";
import { MatTableDataSource } from "@angular/material/table";
import { MatSort, Sort } from "@angular/material/sort";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatDialog } from "@angular/material/dialog";
import moment from "moment";

import {
  ConfirmationDialogComponent,
  ContentSearchParameters,
  IConfirmationDialogData,
} from "@components/index";
import {
  ApiService,
  DataService,
  SnackbarService,
  UserService,
} from "@services/index";
import { apiNews, apiInfoscreen_Light } from "@models/index";
import { TranslationService } from "@vesact/web-ui-template";

import {ApplicationInsightsService} from "../../../services/app-insights.service";

interface apiNews_Table extends apiNews {
  hasExpired: boolean;
}

interface NewsListFilters extends ContentSearchParameters {
  pageSize: number;
  pageIndex: number;
  sortActive: string;
  sortDirection: string;
}

const DEFAULT_NEWS_FILTERS: NewsListFilters = {
  pageIndex: 1,
  pageSize: 10,
  searchText: "",
  selectedCategoryIds: [],
  selectedInfoscreenIds: [],
  sortActive: "status",
  sortDirection: "desc",
};
const NEWS_FILTERS_KEY_LOCAL_STORAGE: string = "NEWS_LIST_FILTERS";

@Component({
  templateUrl: "./news.management.component.html",
  styleUrls: ["./news.management.component.scss"],
})
export class NewsManagementComponent implements OnInit {
  public pathArray: string[] = [
    "menuItem.contentManagement.group",
    "menuItem.contentManagement.news",
  ];
  isLoading: boolean = true;
  fullPageLoading: boolean = false;
  filters: NewsListFilters = DEFAULT_NEWS_FILTERS;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  dataSource = new MatTableDataSource<apiNews_Table>([]);
  displayedColumns = [
    "select",
    "title",
    "description",
    "publicationDate",
    "status",
    "actions",
  ];
  selection: SelectionModel<apiNews_Table>;

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
    this._appInsightsService.logPageView('News Management', '/contentManagement/news')
  }

  async ngOnInit() {
    this.isLoading = true;
    this.fullPageLoading = false;

    const initialSelection = [];
    const allowMultiSelect = true;
    this.selection = new SelectionModel<apiNews_Table>(
      allowMultiSelect,
      initialSelection
    );

    this.initTable();
    try {
      this.filters = {
        ...this.filters,
        ...JSON.parse(localStorage.getItem(NEWS_FILTERS_KEY_LOCAL_STORAGE)),
      };

      this.updateTableDataSource(
        await this.apiService.getFilteredNewsAsync(this.filters)
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
      item: apiNews_Table,
      property: string
    ) => {
      switch (property) {
        case "title":
          return this.getNewsTitle(item).toLocaleLowerCase();
        case "description":
          return item.description?.toLocaleLowerCase() || "";
        case "publicationDate":
          return item.publicationDate;
        case "status":
          // We want to separate visible, expired and non-visible news.
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

  hasExpired(news: apiNews): boolean {
    return (
      moment.utc().isSameOrAfter(moment(news.expirationDate)) && news.isVisible
    );
  }

  updateTableDataSource(news: apiNews[]) {
    this.dataSource.data = news.map((n) => {
      return { ...n, hasExpired: this.hasExpired(n) };
    });
    const maxPageIndex = Math.floor(news.length / this.filters.pageSize);
    const requestedIndex = this.filters?.pageIndex || 0;
    const newIndex = Math.min(requestedIndex, maxPageIndex);
    this.paginator.pageIndex = newIndex;
    this.dataSource.paginator = this.paginator;
  }

  createNew() {
    this.router.navigateByUrl("/contentManagement/news/new");
  }

  edit(news: apiNews) {
    this.router.navigateByUrl(`/contentManagement/news/${news.id}`);
  }

  search = async (searchParams: ContentSearchParameters) => {
    this.isLoading = true;
    this.updateTableDataSource([]);
    try {
      const newFilters: NewsListFilters = {
        ...this.filters,
        pageIndex: 0, // After search changes, restart on first page
        ...searchParams,
      };
      this._updateFilters(newFilters);
      this.updateTableDataSource(
        await this.apiService.getFilteredNewsAsync(this.filters)
      );
    } catch (error) {
    } finally {
      this.isLoading = false;
    }
  };

  async delete(news: apiNews) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      closeOnNavigation: true,
      data: {
        actionTitle: "news.delete.confirmation.title",
        actionButton: "general.button.delete",
        message: "news.delete.confirmation.message",
      } as IConfirmationDialogData,
    });

    dialogRef.afterClosed().subscribe(async (proceed) => {
      if (proceed) {
        try {
          await this.apiService.deleteNewsAsync(news.id);
          this.updateTableDataSource(
            this.dataService.deleteNewsLocally(news.id)
          );

          this.snackbarService.displaySuccessSnackbar("news.delete.success");
        } catch (error) {}
      }
    });
  }

  getNewsTitle(news: apiNews): string {
    // By default try to use ui language
    var language = this._translationService.currentLanguage.value?.iso2;

    // If language not available for the news, display the first in the list
    if (!news.title[language]) {
      language = Object.getOwnPropertyNames(news.title)[0];
    }

    return news.title[language];
  }

  getNewsLastEditDate(news: apiNews): string {
    // By default try to use the last edit date, if not available use creation date
    return news.lastEditDate || news.creationDate;
  }

  getNewsLastEditor(news: apiNews): string {
    // By default try to use the last edit user, if not available use creator
    return news.lastEditor?.displayName || news.creator.displayName;
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
        actionTitle: "news.deleteList.confirmation.title",
        actionButton: "general.button.delete",
        message: "news.deleteList.confirmation.message",
      } as IConfirmationDialogData,
    });

    dialogRef.afterClosed().subscribe(async (proceed) => {
      if (proceed) {
        try {
          this.fullPageLoading = true;
          var newsIds = this.selection.selected.map((n) => n.id);
          await this.apiService.deleteMultipleNewsAsync(newsIds);
          this.updateTableDataSource(
            this.dataService.deleteMultipleNewsLocally(newsIds)
          );
          this.selection.clear();

          this.snackbarService.displaySuccessSnackbar(
            "news.deleteList.success"
          );
        } catch (error) {
        } finally {
          this.fullPageLoading = false;
        }
      }
    });
  }

  private _updateFilters(filters: NewsListFilters) {
    this.filters = { ...this.filters, ...filters };
    localStorage.setItem(
      NEWS_FILTERS_KEY_LOCAL_STORAGE,
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
