import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as momentTZ from 'moment-timezone';
import { LoggingService } from '../../services/logging.service';
import { DateService } from '../../services/date.service';
import { TranslationPipe } from '../../pipes/translation.pipe';
import { LocalCacheFileName, NodeConfig_Sync } from '../../../../../common';

@Component({
    templateUrl: './maintenance.html',
    styleUrls: ['./maintenance.scss']
})
export class MaintenanceComponent {
    maintenanceEndDate: string | undefined = undefined;

    constructor(private http: HttpClient, private translation: TranslationPipe, private dateService: DateService) {
        try {
            this.http.get('cache/' + LocalCacheFileName.Config).subscribe((config: NodeConfig_Sync) => {
                if (config.maintenanceEndDate) {
                    this.translation.getTranslationFromFileAsync('footer.dateTimeFormat').then(format => {
                        this.maintenanceEndDate = this.dateService.formatDatetoLocal(momentTZ.utc(config.maintenanceEndDate), format);
                    });
                }
            });

            LoggingService.info(MaintenanceComponent.name, 'Preparing slide', 'Done');
        } catch (error) {
            LoggingService.error(MaintenanceComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
