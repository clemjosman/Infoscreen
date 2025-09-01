import { COMMA, ENTER } from "@angular/cdk/keycodes";
import {
  Component,
  ElementRef,
  ViewChild,
  OnInit,
  Input,
  Output,
  EventEmitter,
} from "@angular/core";
import { FormControl, Validators } from "@angular/forms";
import { MatAutocompleteSelectedEvent } from "@angular/material/autocomplete";
import { MatChipInputEvent } from "@angular/material/chips";
import { Observable } from "rxjs";
import { map, startWith } from "rxjs/operators";

const CATEGOY_NAME_MAX_LENGTH = 25;

@Component({
  selector: "categories-input-card",
  templateUrl: "./categories-input-card.component.html",
  styleUrls: ["./categories-input-card.component.scss"],
})
export class CategoriesInputCardComponent implements OnInit {
  separatorKeysCodes: number[] = [ENTER, COMMA];
  categoryCtrl = new FormControl("", [
    Validators.maxLength(CATEGOY_NAME_MAX_LENGTH),
  ]);
  filteredCategories: Observable<string[]>;

  readonly CATEGOY_NAME_MAX_LENGTH = CATEGOY_NAME_MAX_LENGTH;

  @Input()
  selectedCategories: string[] = [];
  @Output()
  selectedCategoriesChange = new EventEmitter<string[]>();

  @Input()
  allCategories: string[] = [];

  @ViewChild("categoryInput") categoryInput: ElementRef<HTMLInputElement>;

  constructor() {}

  ngOnInit() {
    this.allCategories = this._sortCategories(this.allCategories);
    this.selectedCategories = this._sortCategories(this.selectedCategories);
    this.selectedCategoriesChange.emit(this.selectedCategories);

    // On input changes, filter list
    this.filteredCategories = this.categoryCtrl.valueChanges.pipe(
      startWith(null as string | null),
      map((category: string | null) => this._filter(category))
    );
  }

  /**
   * Adds a category to the selected list
   * @param event Event emitted when an autocompletion element has been selected
   */
  add(event: MatChipInputEvent): void {
    const value = (event.value || "").trim();
    this._addCategory(value);

    if (value.length > CATEGOY_NAME_MAX_LENGTH) return;

    // Reset input
    event.input!.value = "";

    // Refresh list
    this.categoryCtrl.setValue(null);
  }

  /**
   * Removes a category from the selected list
   * @param category Category removed
   */
  remove(category: string): void {
    const index = this.selectedCategories.indexOf(category);
    if (index >= 0) {
      this.selectedCategories.splice(index, 1);
      this.selectedCategoriesChange.emit(this.selectedCategories);
    }

    // Refresh list
    this.categoryCtrl.setValue(null);
  }

  /**
   * Adds the selected category to the selected list
   * @param event Event emitted when an autocompletion element has been selected
   */
  selected(event: MatAutocompleteSelectedEvent): void {
    this._addCategory(event.option.viewValue);
    this.categoryInput.nativeElement.value = "";
    this.categoryCtrl.setValue(null);
  }

  /**
   * Checks the category to add (null, trim, length, duplicate) and adds it if everything is good
   * @param category Category to add
   */
  private _addCategory(category: string | null): void {
    if (!category) return;
    category = category.trim();

    if (category.length > CATEGOY_NAME_MAX_LENGTH) return;

    // Do not add already added categories
    if (
      this.selectedCategories
        .map((c) => c.toLowerCase())
        .includes(category.toLowerCase())
    )
      return;

    // Add and sort selected
    this.selectedCategories.push(category);
    this.selectedCategories = this._sortCategories(this.selectedCategories);
    this.selectedCategoriesChange.emit(this.selectedCategories);
  }

  /**
   * Alphabetically sorted categories
   */
  private _sortCategories(categories: string[]): string[] {
    return categories.sort((a, b) => {
      a = a.toLowerCase();
      b = b.toLowerCase();
      return a === b ? 0 : a < b ? -1 : 1;
    });
  }

  /**
   * Filters the category list to keep those contained the given filterValue and that are not yet selected
   * @param filterValue Filter value
   * @returns Filtered category list
   */
  private _filter(filterValue: string | null): string[] {
    filterValue = filterValue ? filterValue.toLowerCase() : "";

    return this.allCategories.filter(
      (category) =>
        category.toLowerCase().includes(filterValue) &&
        !this.selectedCategories.includes(category)
    );
  }
}
