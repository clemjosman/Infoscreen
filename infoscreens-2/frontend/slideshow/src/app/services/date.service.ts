import * as moment from "moment-timezone";
import { Injectable } from "@angular/core";
import { LoggingService } from "./logging.service";
import { ConfigService } from "./config.service";
import { filter } from "rxjs/operators";

const DEFAULT_LANGUAGE: string = "de-CH";
const DEFAULT_TIMEZONE: string = "Europe/Zurich";

@Injectable({
  providedIn: "root",
})
export class DateService {
  language: string = DEFAULT_LANGUAGE;
  timezone: string = DEFAULT_TIMEZONE;

  constructor() {
    try {
      ConfigService.config$.pipe(filter((config) => !!config)).subscribe(
        (data) => {
          this.language = data.language || DEFAULT_LANGUAGE;
          this.timezone = data.timezone || DEFAULT_TIMEZONE;
        },
        (err) =>
          LoggingService.error(
            DateService.name,
            "Constructor",
            "Could not get the time configuration from the config file. Using defaults: " +
              DEFAULT_TIMEZONE +
              " " +
              DEFAULT_LANGUAGE,
            err
          )
      );
    } catch (error) {}
  }

  formatDatetoLocal(date, format): string {
    return moment(date)
      .tz(this.timezone || DEFAULT_TIMEZONE)
      .locale(this.language || DEFAULT_LANGUAGE)
      .format(format);
  }

  getLocalSeparator(): string {
    let LocalDate = moment().locale(this.language).format("l");
    return LocalDate.indexOf("/") > -1
      ? "/"
      : LocalDate.indexOf("-") > -1
      ? "-"
      : ".";
  }
}
