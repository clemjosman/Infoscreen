import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root',
})
export class ImageHelper {
  constructor() {}

  async isImageUrlValidAsync(url: string) {
    return new Promise((resolve, reject) => {
      var img = new Image();
      img.onload = () => resolve(true);
      img.onerror = () => resolve(false);
      img.src = url;
    });
  }
  
} 