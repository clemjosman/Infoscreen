import { Component, Input, SimpleChanges } from "@angular/core";

@Component({
  templateUrl: "./pdf-angular.html",
  styleUrls: ["./pdf-angular.scss"],
  selector: "pdf-angular-content",
})
export class PdfAngularContentComponent {
  @Input() base64: string;
  base64Display: string = undefined;
  blob: Blob = undefined;
  blobUrl: string = undefined;
  constructor() {}

  ngOnChanges(changes: SimpleChanges) {
    const b64: string = changes.base64.currentValue;
    if (b64) {
      // Prepare for display
      if (b64.startsWith("data:application/pdf;base64,")) {
        this.base64Display = b64;
      } else {
        this.base64Display = `data:application/pdf;base64,${b64}`;
      }

      // Create blob and generate link
      const byteCharacters = atob(b64);
      const byteNumbers = new Array(byteCharacters.length);
      for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
      }
      const byteArray = new Uint8Array(byteNumbers);
      this.blob = new Blob([byteArray], { type: "application/pdf" });
      this.blobUrl = window.URL.createObjectURL(this.blob);
    } else {
      this.blobUrl = undefined;
      this.blob = undefined;
      this.base64Display = undefined;
    }
  }

  ngOnDestroy() {
    if (this.blobUrl) {
      window.URL.revokeObjectURL(this.blobUrl);
      this.blob = undefined;
      this.blobUrl = undefined;
      this.base64Display = undefined;
    }
  }
}
