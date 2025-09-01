import { Component, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoggingService } from '../../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../../common';

const NUMBER_OF_POLLUTION_LEVELS: number = 5;
export enum Pollutant {
    NO2 = 'no2',
    PM10 = 'pm10',
    O3 = 'o3'
}
export type PollutantDetails = {
    pollutant: Pollutant;
    value: number;
    level: number;
};

@Component({
    selector: 'pollution-indicator',
    templateUrl: './pollution-indicator.html',
    styleUrls: ['./pollution-indicator.scss']
})
export class PollutionIndicatorComponent implements AfterViewInit {
    weather: any;
    pollutionLevels: { level: number; activated: boolean }[];
    pollutants: PollutantDetails[];

    ngAfterViewInit() {
        try {
            this.http.get('cache/' + LocalCacheFileName.OpenWeather).subscribe(data => {
                this.weather = data;

                // Setup pollution levels
                this.pollutionLevels = [];
                for (let j = 1; j <= NUMBER_OF_POLLUTION_LEVELS; j++) {
                    this.pollutionLevels.push({
                        level: j,
                        activated: this.weather?.pollution?.main?.aqi >= j
                    });
                }
                function getPollutantLevel(pollutant: Pollutant, value: number): number {
                    const pollutant_thresholds: { [pollutant in Pollutant]: number[] } = {
                        no2: [50, 100, 200, 400],
                        pm10: [25, 50, 90, 180],
                        o3: [60, 120, 180, 240]
                    };
                    const thresholds = pollutant_thresholds[pollutant];
                    for (let i = 0; i < thresholds.length; i++) {
                        if (value < thresholds[i]) return i + 1;
                    }
                    return 5;
                }
                this.pollutants = [];
                for (const pollutant of Object.values(Pollutant)) {
                    const value = this.weather?.pollution?.components[pollutant];
                    this.pollutants.push({
                        pollutant,
                        level: getPollutantLevel(pollutant, value),
                        value
                    });
                }

                LoggingService.debug(PollutionIndicatorComponent.name, 'Preparing component', 'Done');
            });
        } catch (error) {
            LoggingService.error(PollutionIndicatorComponent.name, 'Preparing component', 'An error occured while getting the component data', error);
        }
    }

    constructor(private http: HttpClient) {}
}
