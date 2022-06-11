import { MetaMetricsRangeInfo } from "./range-info.model";

export interface MetaMetricsTimeLineRequest {
    sublicense: string;
    measurement: string;
    range: MetaMetricsRangeInfo;
    eachHours: number;
}

export interface MetaMetricsTime4LinesRequest {
  sublicense: string;
  measurement: Array<string>;
  range: MetaMetricsRangeInfo;
  eachHours: number;
  license: string;
  version: string;
  app: string;
  environment: string;
  project: string;
  server: string;
}

export class MetaMetricsTime4LinesPaginationRequest {
  sublicenses = new Array<string>();
  measurement?: Array<string>;
  range?: MetaMetricsRangeInfo;
  eachHours?: number;
  license = '';
  version = '';
  app = '';
  environment = '';
  project = '';
  server = '';
  pageNumber = 1;
  pageSize = 10;
  isNotValue = true;
  isCreateEmpty = false;
  sortColumns = new SortColumns();
}

export class SortColumns {
  columnName = '';
  isName = false;
  isTime = false;
  isServer = false;
  isVersion = false;
  isProduct = false;
}
