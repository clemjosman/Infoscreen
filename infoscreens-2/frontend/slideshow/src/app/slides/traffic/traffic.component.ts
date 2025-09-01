import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, NodeConfig_Sync, Slide } from '../../../../../common';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

declare global {
    interface Window {
        initMap: () => void;
    }
}

@Component({
    templateUrl: './traffic.html',
    styleUrls: ['./traffic.scss']
})
export class TrafficComponent {
    slide: Slide = Slide.Traffic;
    apiLoaded: Observable<boolean>;
    apiKey: string;
    scriptRef: HTMLScriptElement = undefined;

    options: google.maps.MapOptions = {
        center: {
            lat: undefined,
            lng: undefined
        },
        zoom: undefined,
        streetViewControl: false,
        mapTypeControl: false,
        styles: []
    };

    constructor(private http: HttpClient) {
        try {
            this.http.get('cache/' + LocalCacheFileName.Config).subscribe((config: NodeConfig_Sync) => {
                this.options.center.lat = config.slides.config.traffic.gmap.latitude;
                this.options.center.lng = config.slides.config.traffic.gmap.longitude;
                this.options.zoom = config.slides.config.traffic.gmap.zoom;
                this.apiKey = config.slides.config.traffic.gmap.apiKey;

                this.apiLoaded = http.jsonp(`https://maps.googleapis.com/maps/api/js?key=${this.apiKey}`, 'callback').pipe(
                    map(() => {
                        return true;
                    }),
                    catchError(err => {
                        console.error(err);
                        return of(false);
                    })
                );

                LoggingService.debug(TrafficComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(TrafficComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }

    ngOnInit() {
        this.options.styles = [
            {
                elementType: 'geometry',
                stylers: [
                    {
                        color: '#1d2c4d'
                    }
                ]
            },
            {
                elementType: 'labels.text.fill',
                stylers: [
                    {
                        color: '#8ec3b9'
                    }
                ]
            },
            {
                elementType: 'labels.text.stroke',
                stylers: [
                    {
                        color: '#1a3646'
                    }
                ]
            },
            {
                featureType: 'administrative.country',
                elementType: 'geometry.stroke',
                stylers: [
                    {
                        color: '#4b6878'
                    }
                ]
            },
            {
                featureType: 'administrative.land_parcel',
                elementType: 'labels.text.fill',
                stylers: [
                    {
                        color: '#64779e'
                    }
                ]
            },
            {
                featureType: 'administrative.province',
                elementType: 'geometry.stroke',
                stylers: [
                    {
                        color: '#4b6878'
                    }
                ]
            },
            {
                featureType: 'landscape.man_made',
                elementType: 'geometry.stroke',
                stylers: [
                    {
                        color: '#334e87'
                    }
                ]
            },
            {
                featureType: 'landscape.natural',
                elementType: 'geometry',
                stylers: [
                    {
                        color: '#023e58'
                    }
                ]
            },
            {
                featureType: 'poi',
                elementType: 'geometry',
                stylers: [
                    {
                        color: '#283d6a'
                    }
                ]
            },
            {
                featureType: 'poi',
                elementType: 'labels.text.fill',
                stylers: [
                    {
                        color: '#6f9ba5'
                    }
                ]
            },
            {
                featureType: 'poi',
                elementType: 'labels.text.stroke',
                stylers: [
                    {
                        color: '#1d2c4d'
                    }
                ]
            },
            {
                featureType: 'poi.park',
                elementType: 'geometry.fill',
                stylers: [
                    {
                        color: '#023e58'
                    }
                ]
            },
            {
                featureType: 'poi.park',
                elementType: 'labels.text.fill',
                stylers: [
                    {
                        color: '#3C7680'
                    }
                ]
            },
            {
                featureType: 'road',
                elementType: 'geometry',
                stylers: [
                    {
                        color: '#304a7d'
                    }
                ]
            },
            {
                featureType: 'road',
                elementType: 'labels.text.fill',
                stylers: [
                    {
                        color: '#98a5be'
                    }
                ]
            },
            {
                featureType: 'road',
                elementType: 'labels.text.stroke',
                stylers: [
                    {
                        color: '#1d2c4d'
                    }
                ]
            },
            {
                featureType: 'road.highway',
                elementType: 'geometry',
                stylers: [
                    {
                        color: '#2c6675'
                    }
                ]
            },
            {
                featureType: 'road.highway',
                elementType: 'geometry.stroke',
                stylers: [
                    {
                        color: '#255763'
                    }
                ]
            },
            {
                featureType: 'road.highway',
                elementType: 'labels.text.fill',
                stylers: [
                    {
                        color: '#b0d5ce'
                    }
                ]
            },
            {
                featureType: 'road.highway',
                elementType: 'labels.text.stroke',
                stylers: [
                    {
                        color: '#023e58'
                    }
                ]
            },
            {
                featureType: 'transit',
                elementType: 'labels.text.fill',
                stylers: [
                    {
                        color: '#98a5be'
                    }
                ]
            },
            {
                featureType: 'transit',
                elementType: 'labels.text.stroke',
                stylers: [
                    {
                        color: '#1d2c4d'
                    }
                ]
            },
            {
                featureType: 'transit.line',
                elementType: 'geometry.fill',
                stylers: [
                    {
                        color: '#283d6a'
                    }
                ]
            },
            {
                featureType: 'transit.station',
                elementType: 'geometry',
                stylers: [
                    {
                        color: '#3a4762'
                    }
                ]
            },
            {
                featureType: 'water',
                elementType: 'geometry',
                stylers: [
                    {
                        color: '#0e1626'
                    }
                ]
            },
            {
                featureType: 'water',
                elementType: 'labels.text.fill',
                stylers: [
                    {
                        color: '#4e6d70'
                    }
                ]
            }
        ];
    }
}
