import { Injectable } from '@angular/core';
import { IndexableCache } from '../enums/indexableCache';
import { LoggingService } from './logging.service';

@Injectable({
  providedIn: 'root',
})
export class CacheService {
  
  indexes: number[];

  constructor() { 
    // Get number of indexable caches and initialize the array
    this.indexes = new Array(Object.keys(IndexableCache).length / 2).fill(0);
  }

  getIndexForCachedElements(indexableCache: IndexableCache, arraySize: number, incrementAfterGet: boolean = true)
  {   

    var value = this.indexes[indexableCache];

    if(incrementAfterGet == true)
    {
      if(this.indexes[indexableCache] >= arraySize - 1)
      {
        this.indexes[indexableCache] = 0;
      }
      else
      {
        this.indexes[indexableCache]++;
      }
    }
    LoggingService.trace(CacheService.name, "getIndexForCachedElements", "Number "+(value+1)+" out of "+arraySize+' for '+indexableCache)

    return value;
  }
}