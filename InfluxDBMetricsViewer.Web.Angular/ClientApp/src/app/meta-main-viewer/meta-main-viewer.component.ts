import {
  ChangeDetectionStrategy, Component,
  OnInit, Inject, ViewChild
} from '@angular/core';
import { Observable, retry, tap, Subject } from 'rxjs';

import { MetaMetricsViewerService } from '../core/meta-metrics-viewer.service';
import { MetaMetricsInstallation } from '../shared/models/meta-metrics-viewer/installation.model';
import { MetaMetricsMeasurement } from '../shared/models/meta-metrics-viewer/measurement.model';
import { MetaMetricsTimeLine } from '../shared/models/meta-metrics-viewer/time-line.model';
import { MetaMetricsRangeInfo, ranges } from '../shared/models/meta-metrics-viewer/range-info.model';
import { MetaMetricsTime4LinesPaginationRequest } from '../shared/models/meta-metrics-viewer/time-line-request.model';

import { MsalBroadcastService, MsalService, MSAL_GUARD_CONFIG, MsalGuardConfiguration } from '@azure/msal-angular';
import {
  EventMessage, EventType, AuthenticationResult,
  InteractionStatus, InteractionType, PopupRequest,
  RedirectRequest
} from '@azure/msal-browser'
import { filter, takeUntil } from 'rxjs/operators';
import {
  MetaMetricsTime4Lines, MetaMetricsFilters,
  MetaMetricsTime4LinesPagination
} from '../shared/models/meta-metrics-viewer/time-line.model';
import { Router } from '@angular/router';
import * as _ from 'underscore';
import { PageEvent } from '@angular/material/paginator';
declare var bootstrap: any;
import { MatPaginator } from '@angular/material/paginator';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { Color } from 'ng2-charts';

@Component({
  selector: 'app-meta-main-viewer',
  templateUrl: './meta-main-viewer.component.html',
  styleUrls: ['./meta-main-viewer.component.scss']
})

export class MetaMainViewerComponent implements OnInit {
  modelFilters = new MetaMetricsFilters();
  model = new MetaMetricsTime4LinesPagination();
  ranges: MetaMetricsRangeInfo[] = ranges();
  selectedRange!: MetaMetricsRangeInfo;
  req = new MetaMetricsTime4LinesPaginationRequest();
  defaultMeasurements: string[] = ['metakis__webcontext', 'metakis__workcontextrecords', 'metakis__patientdata', 'metatext__parsedocument_counter'];
  pagination = false;
  showFirstLastButtons = true;
  sublicense = '';
  app = '';
  environment = '';
  project = '';
  version = '';
  license = '';
  server = '';
  range = '';
  tableLoader = false;
  filterTimeRangeRequest = false;
  panel = new MetaMetricsTime4Lines();
  @ViewChild(MatPaginator, { static: false }) paginator!: MatPaginator;

  lineChartLegend = true;
  lineChartColors: Color[] = [{ backgroundColor: '#b0bbd1' }];
  lineChartData: Array<ChartDataSets[]> = [];
  lineChartOptions: ChartOptions = {
    responsive: true,
    // maintainAspectRatio: false,
    animation: { duration: 0 },
    scales: {
      xAxes: [
        {
          type: 'time',
          time: {
            unit: 'day',
            // displayFormats: {
            //   day: 'MMM D', // This is the default
            // },
          },
        }
      ]
    }
  };

  constructor(private readonly service: MetaMetricsViewerService,
    private readonly authService: MsalService,
    private readonly msalBroadcastService: MsalBroadcastService,
    private readonly router: Router,
    @Inject(MSAL_GUARD_CONFIG) private readonly msalGuardConfig: MsalGuardConfiguration) {
    this.req.range = this.ranges.find(x => x.default) ?? { display: '1w', weeks: 1 };
    this.req.measurement = this.defaultMeasurements;
    this.range = this.req.range['display'];
  }

  ngOnInit(): void {
    this.init();
  }

  init() {
    this.getTable();
    if (this.filterTimeRangeRequest) {
      this.service.getFiltersValue((this.req.range) as any).pipe(tap((data: MetaMetricsFilters) => {
        if (data) {
          this.modelFilters = data;
        }
      })).subscribe();
    } else {
      this.service.getFilters().pipe(tap((data: MetaMetricsFilters) => {
        if (data) {
          this.modelFilters = data;
        }
      })).subscribe();
    }
  }

  getTable() {
    this.req.isNotValue = true;
    this.req.isCreateEmpty = true;
    this.tableLoader = true;
    this.service.getTime4LinesPagination(this.req).pipe(tap((data: MetaMetricsTime4LinesPagination) => {
      if (data) {
        this.tableLoader = false;
        this.model = data;
        this.pagination = this.model.count > this.req.pageSize;
      }
    })).subscribe();
  }

  handlePageEvent(event: PageEvent) {
    this.req.pageSize = event.pageSize;
    this.req.pageNumber = event.pageIndex + 1;
    this.getTable();;
  }

  changeFilter(s: string) {
    switch (s) {
      case 'sublicense':
        this.req.sublicenses = new Array<string>();
        if (this.sublicense !== '') {
          this.req.sublicenses.push(this.sublicense);
        }
        break;
      case 'app':
        this.req.app = this.app;
        break;
      case 'environment':
        this.req.environment = this.environment;
        break;
      case 'project':
        this.req.project = this.project;
        break;
      case 'version':
        this.req.version = this.version;
        break;
      case 'license':
        this.req.license = this.license;
        break;
      case 'server':
        this.req.server = this.server;
        break;
      case 'range':
        this.req.range = this.ranges.find(x => x.display === this.range);
        if (this.filterTimeRangeRequest) {
          this.service.getFiltersValue((this.req.range) as any).pipe(tap((data: MetaMetricsFilters) => {
            if (data) {
              this.modelFilters = data;
            }
          })).subscribe();
        }
        break;
    default:
    }
    this.req.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getTable();
  }

  selectTable(p: MetaMetricsTime4Lines) {
    this.panel = new MetaMetricsTime4Lines();
    const myOffcanvas: any = document.getElementById('offcanvasRight');
    const bsOffcanvas = new bootstrap.Offcanvas(myOffcanvas);
    const r = new MetaMetricsTime4LinesPaginationRequest();
    r.range = this.req.range;
    r.isNotValue = false;
    r.isCreateEmpty = true;
    r.sublicenses.push(p.sublicense);
    this.service.getTime4LinesPagination(r).pipe(tap((data: MetaMetricsTime4LinesPagination) => {
      if (data) {
        this.panel = data.list[0];
        for (let i = 0; i < data.list[0].timeLines.length; i++) {
          this.lineChartData[i] = [{
            data: data.list[0].timeLines[i].timeValues.map(t => { return { x: t.timestamp, y: t.value }; }),
            label: data.list[0].timeLines[i].measurement
          }];
        }
      }
    })).subscribe();
    bsOffcanvas.show();
  }

  sortTable(s: string) {
    this.req.sortColumns.columnName = s;
    this.req.pageNumber = 1;
    this.paginator.pageIndex = 0;
    switch (s) {
      case 'name':
        this.req.sortColumns.isName = !this.req.sortColumns.isName;
        break;
      case 'time':
        this.req.sortColumns.isTime = !this.req.sortColumns.isTime;
        break;
      case 'server':
        this.req.sortColumns.isServer = !this.req.sortColumns.isServer;
        break;
      case 'version':
        this.req.sortColumns.isVersion = !this.req.sortColumns.isVersion;
        break;
      case 'product':
        this.req.sortColumns.isProduct = !this.req.sortColumns.isProduct;
        break;
    default:
    }
    this.getTable();
  }

  getLineChartData(p: MetaMetricsTimeLine) {
    debugger;
  }

  clone(model: any) {
    const d = JSON.stringify(model);
    return JSON.parse(d);
  }
}
