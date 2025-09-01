import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoggingService } from '../../services/logging.service';
import { InfoscreenNodeStatus } from '../../models/data/infoscreenNodeStatus';
import { LocalCacheFileName, Slide } from '../../../../../common';

@Component({
    templateUrl: './infoscreenmonitoring.html',
    styleUrls: ['./infoscreenmonitoring.scss']
})
export class InfoscreenMonitoringComponent {
    INFOSCREENS_PER_ROW: number = 4;
    slide: Slide = Slide.InfoscreenMonitoring;

    // The issue that made this logic so complex is having an html template with ngFor to iterate the data
    // based on the category and lines. For this we need to create a list of indexes to iterate.
    nodesStatusData: InfoscreenNodeStatus[] = [];
    nodesStatusForCards: {
        categoryName: string;
        status: InfoscreenNodeStatus[][];
    }[] = [];
    rowIndexesPerCategory: number[][] = [];

    constructor(private http: HttpClient) {
        try {
            this.http.get('cache/' + LocalCacheFileName.InfoscreenMonitoring).subscribe(async data => {
                this.nodesStatusData = <any>data;

                // Sorting by node name so they appear sorted on the slide
                // It is easier to make the sorting here as the nodes will be splitted
                // in multiple arrays in each category (display line)
                this.nodesStatusData = this.nodesStatusData.sort((a, b) =>
                    a.deviceId.toLocaleLowerCase().localeCompare(b.deviceId.toLocaleLowerCase())
                );

                var i = 0;

                var nodeTags = this.nodesStatusData.slice().map(s => s.tags.find(t => t !== 'Infoscreen' && t !== 'ALL' && t !== 'ActemiumCH'));
                for (var nodeIndex = 0; nodeIndex < nodeTags.length; nodeIndex++) {
                    // Bypass node without tags
                    if (nodeTags[nodeIndex]) {
                        var categoryIndex = this.nodesStatusForCards.findIndex(s => s.categoryName === nodeTags[nodeIndex]);
                        if (categoryIndex < 0) {
                            // Add new category
                            this.nodesStatusForCards.push({
                                categoryName: nodeTags[nodeIndex],
                                status: []
                            });
                            categoryIndex = this.nodesStatusForCards.length - 1;
                        }

                        // Category already existing, adding entry
                        var numberOfRows = this.nodesStatusForCards[categoryIndex].status.length;
                        if (
                            numberOfRows === 0 ||
                            this.nodesStatusForCards[categoryIndex].status[numberOfRows - 1].length === this.INFOSCREENS_PER_ROW
                        ) {
                            if (numberOfRows === 0) {
                                this.rowIndexesPerCategory.push([]);
                            }
                            this.nodesStatusForCards[categoryIndex].status.push([]);
                            this.rowIndexesPerCategory[categoryIndex].push(numberOfRows);
                            numberOfRows++;
                        }
                        this.nodesStatusForCards[categoryIndex].status[numberOfRows - 1].push(this.nodesStatusData[nodeIndex]);
                    }
                }

                // Sorting the categories by name
                this.nodesStatusForCards = this.nodesStatusForCards.sort((a, b) =>
                    a.categoryName.toLocaleLowerCase().localeCompare(b.categoryName.toLocaleLowerCase())
                );

                LoggingService.debug(InfoscreenMonitoringComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(InfoscreenMonitoringComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }

    isLastCategory(categoryIndex: number): boolean {
        return categoryIndex === this.nodesStatusForCards.length - 1;
    }
}
