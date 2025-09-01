import { Component, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoggingService } from '../../services/logging.service';
import { Chart } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import * as moment from 'moment-timezone';
import { LocalCacheFileName, Slide } from '../../../../../common';
import { _getIconName } from '../weather-daily/weather.component';
import { BrandingService } from '../../services';

@Component({
    templateUrl: './weather.html',
    styleUrls: ['./weather.scss']
})
export class WeeklyWeatherComponent implements AfterViewInit {
    slide: Slide = Slide.WeatherWeekly;
    weather: any;
    hourlyTemps: any;
    hourlyDates: any;
    Math: Math; // Providing the Math object to the html template
    useIcons: boolean = true;

    @ViewChild('WeeklyWeatherChart')
    WeeklyWeatherChart: ElementRef;
    ngAfterViewInit() {
        try {
            BrandingService.branding$.subscribe(branding => {
                // All brands now use the icons, keeping the logic in case of a change of opinion
                this.useIcons = true;
            });

            this.http.get('cache/' + LocalCacheFileName.OpenWeather).subscribe(data => {
                this.weather = data;

                var style = getComputedStyle(document.body);
                var textColor = style.getPropertyValue('--text-color');
                var chartLineColor = style.getPropertyValue('--weather-chart-line-color');
                var chartBackgroundColor = style.getPropertyValue('--weather-chart-background-color');

                let i = 0;
                this.hourlyTemps = [];
                this.hourlyDates = [];
                this.weather.hourly.forEach(hourly => {
                    if (moment(hourly.dt).isAfter(moment(this.weather.current.dt)) == true && i < 7) {
                        this.hourlyTemps[i] = Math.round(hourly.temp);
                        this.hourlyDates[i] = this.getHour(hourly.dt);
                        i++;
                    }
                });

                const ctx = this.WeeklyWeatherChart.nativeElement;
                const WeeklyWeatherChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: this.hourlyDates,
                        datasets: [
                            {
                                data: this.hourlyTemps,
                                backgroundColor: [chartBackgroundColor],
                                borderColor: [chartLineColor],
                                borderWidth: 3,
                                pointStyle: 'line'
                            }
                        ]
                    },
                    plugins: [ChartDataLabels],
                    options: {
                        scales: {
                            xAxes: [
                                {
                                    ticks: {
                                        fontSize: 16,
                                        fontColor: textColor
                                    },
                                    gridLines: {
                                        display: false
                                    }
                                }
                            ],
                            yAxes: [
                                {
                                    ticks: {
                                        min: Math.min.apply(Math, this.hourlyTemps) - 2,
                                        max: Math.max.apply(Math, this.hourlyTemps) + 4,
                                        display: false
                                    },
                                    gridLines: {
                                        display: false
                                    }
                                }
                            ]
                        },
                        legend: {
                            display: false
                        },
                        responsive: true,
                        plugins: {
                            // Change options for ALL labels of THIS CHART
                            datalabels: {
                                color: textColor,
                                align: 'top',
                                font: {
                                    size: 16
                                }
                            }
                        }
                    }
                });
                LoggingService.debug(WeeklyWeatherComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(WeeklyWeatherComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }

    constructor(private http: HttpClient) {
        this.Math = Math;
    }

    getDay(timestamp) {
        const weekday = ['label.sunday', 'label.monday', 'label.tuesday', 'label.wednesday', 'label.thursday', 'label.friday', 'label.saturday'];
        var date = new Date(timestamp * 1000);
        var day = weekday[date.getDay()];
        return day.toString();
    }

    getHour(timestamp) {
        var date = new Date(timestamp * 1000);
        var hour = date.getHours();
        return hour + ':00';
    }

    getIconName(openWeatherIconName: string) {
        return _getIconName(openWeatherIconName);
    }
}
