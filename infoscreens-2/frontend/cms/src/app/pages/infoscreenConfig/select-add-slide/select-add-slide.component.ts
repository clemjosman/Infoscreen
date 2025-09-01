import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Slide } from '../../../../../../common';

export interface SelectAddSlideData {
    allowedSlides: Slide[];
    forbiddenNeighborSlides: Slide[];
    slidesNeedingAdminConfig: Slide[];
    slidesNotYetUsed: Slide[];
}

@Component({
    selector: 'select-add-slide',
    templateUrl: './select-add-slide.component.html',
    styleUrls: ['./select-add-slide.component.scss']
})
export class SelectAddSlideComponent {
    selectedSlide: Slide = undefined;

    constructor(public dialogRef: MatDialogRef<SelectAddSlideComponent>, @Inject(MAT_DIALOG_DATA) public data: SelectAddSlideData) {
        this.selectedSlide = data.allowedSlides[0];
    }

    isSlideUsed(slide: Slide): boolean {
        return this.data.slidesNotYetUsed.includes(slide);
    }

    cancel(): void {
        this.dialogRef.close();
    }
}
