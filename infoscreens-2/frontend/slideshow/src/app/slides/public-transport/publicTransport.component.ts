import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment-timezone';
import { TranslationPipe } from '../../pipes/translation.pipe';
import { DateService } from '../../services/date.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../common';

@Component({
    templateUrl: './publicTransport.html',
    styleUrls: ['./publicTransport.scss'],
    providers: [TranslationPipe]
})
export class PublicTransportComponent {
    slide: Slide = Slide.PublicTransport;
    publicTransportData: any;

    constructor(private http: HttpClient, private translationPipe: TranslationPipe, private dateService: DateService) {
        try {
            this.publicTransportData = {
                station: {
                    id: '',
                    name: ''
                },
                stationBoard: [
                    {
                        lineNumber: '',
                        destination: '',
                        departureTimestamp: '',
                        delay: null
                    },
                    {
                        lineNumber: '',
                        destination: '',
                        departureTimestamp: '',
                        delay: null
                    }
                ],
                destinationGroups: []
            };

            this.translationPipe.getTranslationFromFileAsync('publicTransport.timeFormat').then(format => {
                this.http.get('cache/' + LocalCacheFileName.PublicTransport).subscribe(data => {
                    data = this.filterOldStationBoard(data);

                    data['stationBoard'] = data['stationBoard']
                        .map(sb => {
                            sb.departureTimestamp = this.dateService.formatDatetoLocal(sb.departureTimestamp, format);
                            return sb;
                        })
                        .slice(0, 8);

                    data = this.filterStationBoardToDestinationGroups(data);

                    this.publicTransportData = data;
                    LoggingService.debug(PublicTransportComponent.name, 'Preparing slide', 'Done');
                });
            });
        } catch (error) {
            LoggingService.error(PublicTransportComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }

    filterOldStationBoard(data) {
        try {
            // Exit if no stationBoard is provided
            if (!data['stationBoard'] || data['stationBoard'].length === 0) return data;

            data['stationBoard'] = data['stationBoard'].filter(sb => {
                return moment(sb['departureTimestamp']).isAfter(moment.utc());
            });

            return data;
        } catch (error) {
            LoggingService.error(PublicTransportComponent.name, 'filterOldStationBoard', 'An error occured while filtering the old stations', error);
        }
    }

    filterStationBoardToDestinationGroups(data) {
        try {
            // Exit if no destination group is provided
            if (!data['destinationGroups'] || data['destinationGroups'].length === 0) return data;

            data['destinationGroups'] = data['destinationGroups'].map(dg => {
                dg.stationBoard = [];

                // If no criterias are given, return all station board
                if (!dg['criterias'] || dg['criterias'].length === 0) {
                    dg.stationBoard = data['stationBoard'];
                } else {
                    // Filter by given criterias
                    dg.stationBoard = data['stationBoard']
                        .filter(sb =>
                            dg['criterias'].some(c => {
                                return (
                                    (c['lineNumber'] == null || c['lineNumber'] == sb['lineNumber']) &&
                                    (c['destination'] == null || sb['destination'].includes(c['destination']))
                                );
                            })
                        )
                        .slice(0, 2);
                }

                return dg;
            });

            return data;
        } catch (error) {
            LoggingService.error(
                PublicTransportComponent.name,
                'filterStationBoardToDestinationGroups',
                'An error occured while filtering the stations of the destination group',
                error
            );
        }
    }
}
