import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, NodeConfig_Sync, Slide } from '../../../../../common';

@Component({
    templateUrl: './stock.html',
    styleUrls: ['./stock.scss']
})
export class StockComponent {
    slide: Slide = Slide.Stock;
    exchangeUrl: string;
    stockUrl: string;

    constructor(private http: HttpClient) {
        try {
            this.http.get('cache/' + LocalCacheFileName.Config).subscribe((config: NodeConfig_Sync) => {
                this.stockUrl = config.slides.config.stock.stock.url;
                this.exchangeUrl = config.slides.config.stock.exchange.url;
                LoggingService.debug(StockComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(StockComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
