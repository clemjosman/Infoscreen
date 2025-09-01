import {
  AfterViewChecked,
  AfterViewInit,
  Component,
  ElementRef,
  NgZone,
  OnDestroy,
  OnInit,
  ViewChild,
} from "@angular/core";
import { ConfigService } from "../../services";

@Component({
  templateUrl: "./rollingMessage.html",
  styleUrls: ["./rollingMessage.scss"],
  selector: "rolling-message",
})
export class RollingMessageComponent
  implements OnInit, AfterViewInit, OnDestroy
{
  @ViewChild("container")
  container: ElementRef<HTMLDivElement> | undefined;

  message: string = "";
  animationsAllowed: boolean = true;
  animationDuration: number = 20;
  resizeObserver: ResizeObserver | undefined = undefined;

  constructor(private _ngZone: NgZone) {}

  ngOnInit() {
    ConfigService.config$.subscribe((config) => {
      if (config) {
        this.message = config.rollingMessage.trim() || "";
        this.animationsAllowed = !config.disableAnimations;
        this.ngAfterViewInit();
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribeResizer();
  }

  ngAfterViewInit(): void {
    this.unsubscribeResizer();
    if (!this.container) return;

    this.resizeObserver = new ResizeObserver(
      (entries: ResizeObserverEntry[]) => {
        for (let entry of entries) {
          this._ngZone.run(() => {
            this.updateAnimationSpeed(entry.contentRect.width);
          });
        }
      }
    );

    this.resizeObserver.observe(this.container.nativeElement);
  }

  unsubscribeResizer(): void {
    if (this.resizeObserver) {
      this.resizeObserver.disconnect();
      this.resizeObserver = undefined;
    }
  }

  updateAnimationSpeed(containerWidth: number): number {
    if (!this.message) return 20;
    const px_per_second_speed = 75;
    const travel_distance = 2 * containerWidth; //width of the container + the padding to hide it before starting scrolling
    this.animationDuration = travel_distance / px_per_second_speed;
  }
}
