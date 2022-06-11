import { MetaMetricsTimeValue } from './time-value.model';

export class MetaMetricsTimeLine {
  measurement!: string;
  timeValues = new Array<MetaMetricsTimeValue>();    
}

export class MetaMetricsTime4Lines {
  result!: string;
  table!: number;
  _start!: Date | string;
  _stop!: string;
  _time!: Date | string;
  _value!: number;
  _field!: string;
  _measurement!: string;
  app!: string;
  env!: string;
  license!: string;
  mtype!: string;
  project!: string;
  server!: string;
  sublicense!: string;
  unit!: string;
  version!: string;
  name!: string;
  metaMetricsTimeLine!: MetaMetricsTimeLine;
  timeLines = new Array<MetaMetricsTimeLine>();
}

export class MetaMetricsFilters {
  apps = new Array<string>();
  environments = new Array<string>();
  projects = new Array<string>();
  versions = new Array<string>();
  licenses = new Array<string>();
  servers = new Array<string>();
  items = new Array<string>();
  subLicenses = new Array<string>();
  measurements = new Array<string>();
}

export class MetaMetricsTime4LinesPagination {
  count = 0;
  list = new Array<MetaMetricsTime4Lines>();
}
