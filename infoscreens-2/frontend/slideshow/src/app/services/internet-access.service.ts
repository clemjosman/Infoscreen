import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class InternetAccessService {
  private static _isConnected$: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(navigator.onLine);
  public static get isConnected$() {
    return InternetAccessService._isConnected$.asObservable();
  }
  public static get isConnected(): boolean {
    return InternetAccessService._isConnected$.value;
  }

  init() {
    window.addEventListener("offline", (_) => {
      this.setIsConnected(false);
    });
    window.addEventListener("online", (_) => {
      this.setIsConnected(true);
    });
    this.setIsConnected(navigator.onLine);
  }

  private setIsConnected(isConnected: boolean) {
    InternetAccessService._isConnected$.next(isConnected);
  }
}
