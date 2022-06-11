import { ChangeDetectionStrategy, Component, OnInit, Inject } from '@angular/core';
import { Observable, retry, tap, Subject } from 'rxjs';

import { MetaMetricsViewerService } from '../../core/meta-metrics-viewer.service';
import { MetaMetricsInstallation } from '../../shared/models/meta-metrics-viewer/installation.model';
import { MetaMetricsMeasurement } from '../../shared/models/meta-metrics-viewer/measurement.model';
import { MetaMetricsRangeInfo, ranges } from '../../shared/models/meta-metrics-viewer/range-info.model';
import { TableVirtualScrollDataSource } from 'ng-table-virtual-scroll';

import { MsalBroadcastService, MsalService, MSAL_GUARD_CONFIG, MsalGuardConfiguration } from '@azure/msal-angular';
import {
  EventMessage, EventType, AuthenticationResult,
  InteractionStatus, InteractionType, PopupRequest,
  RedirectRequest
} from '@azure/msal-browser'
import { filter, takeUntil } from 'rxjs/operators';
import { MetaMetricsTime4Lines, MetaMetricsFilters } from '../../shared/models/meta-metrics-viewer/time-line.model';
import { Router } from '@angular/router';
import * as _ from 'underscore';

export interface DataElement {
  position: number;
  installation: MetaMetricsInstallation;
  sublicense: string; // for filter
  installationRequest: Observable<MetaMetricsInstallation>;
  dynamicRequest: Observable<Array<MetaMetricsTime4Lines>>;
  array4Lines: Array<MetaMetricsTime4Lines>;
  isSpinner: boolean;
}
export interface DynamicColumn {
  id: string;
  measurement: MetaMetricsMeasurement;
}

@Component({
  selector: 'app-meta-metrics-viewer-main',
  templateUrl: './meta-metrics-viewer-main.component.html',
  styleUrls: ['./meta-metrics-viewer-main.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class MetaMetricsViewerMainComponent implements OnInit {
  staticColumns: string[] = ['position','installation','bis'];
  dynamicColumns: DynamicColumn[] = [];
  defaultMeasurements: string[] = ['metakis__webcontext', 'metakis__workcontextrecords', 'metakis__patientdata', 'metatext__parsedocument_counter'];
  displayedColumns: string[] = [];

  dataSource = <TableVirtualScrollDataSource<DataElement>>(new TableVirtualScrollDataSource());
  measurements: MetaMetricsMeasurement[] = [];
  modelFilters = new MetaMetricsFilters();

  selectedRowIndex!: number | null;
  selectedRow!: DataElement;
  selectedRowMeasurements$!: Observable<MetaMetricsMeasurement[]>;
  sidenavOpened = false;

  ranges: MetaMetricsRangeInfo[] = ranges();
  selectedRange!: MetaMetricsRangeInfo;

  private readonly _destroying$ = new Subject();
  loginDisplay = false;

  filterLicense = '';
  filterVersion = '';
  filterApp = '';
  filterEnvironment = '';
  filterProject = '';
  filterServer = '';
  filterSubLicense = '';

  isNameDesc = false;
  bufferMultiplier = 0;

  constructor(private readonly service: MetaMetricsViewerService,
    private readonly authService: MsalService,
    private readonly msalBroadcastService: MsalBroadcastService,
    private readonly router: Router,
    @Inject(MSAL_GUARD_CONFIG) private readonly msalGuardConfig: MsalGuardConfiguration) {
    this.updateDisplayedColumns();
    this.selectedRange = this.ranges.find(x => x.default) ?? { display: '1w', weeks: 1};
  }

  ngOnInit(): void {
    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None)
      )
      .subscribe(() => {
        this.setLoginDisplay();
        this.checkAndSetActiveAccount();
        if (this.loginDisplay) this.init();
      });
  }

  init() {
    this.service.getGetSublicenseList().pipe(tap((data: Array<string>) => {
      if (data) {
        if (this.isNameDesc) data = data.reverse();
        this.dataSource.data = data.map<DataElement>((x, index) => {
          return {
            position: index + 1,
            installation: { sublicense: x },
            dynamicRequest: this.getDynamicRequest(x),
            installationRequest: this.getInstallationInfo(x),
            sublicense: x,
            array4Lines: [],
            isSpinner: false
          }
        });
      }
    })).subscribe();

    this.service.getMeasurementList().pipe(tap((data: any) => {
       if (data) {
         this.measurements = data;
         this.dynamicColumns = this.defaultMeasurements.map(x => this.measurements.find(m=>m.measurementName == x))
                                                       .filter(this.isNotNullOrUndefined)
                                                       .map(x => { return {id: this.genUniqueId(), measurement: x}});
         this.updateDisplayedColumns();
       }
    })).subscribe();

    this.service.getFilters().pipe(tap((data: MetaMetricsFilters) => {
      if (data) {
        this.modelFilters = data;
        this.dynamicColumns = this.defaultMeasurements.map(x => this.measurements.find(m => m.measurementName == x))
          .filter(this.isNotNullOrUndefined)
          .map(x => { return { id: this.genUniqueId(), measurement: x } });
        this.updateDisplayedColumns();
      }
    })).subscribe();
  }

  updateDisplayedColumns() {
    this.displayedColumns = this.staticColumns.concat(this.dynamicColumns.map(x => x.id));
    this.displayedColumns.push('server');
    this.displayedColumns.push('version');
    this.displayedColumns.push('license');
    this.displayedColumns.push('app');
    this.displayedColumns.push('env');
    this.displayedColumns.push('sublicense');
    this.displayedColumns.push('project');
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  onClickRow(row: any, rowIndex: number) {
    this.selectedRowIndex = rowIndex;
    this.selectedRow = row;
    this.selectedRowMeasurements$ = this.service.getMeasurementListBySublicense({sublicense: this.selectedRow.sublicense, range: this.selectedRange});
    if (!this.sidenavOpened) {
      this.sidenavOpened = true;
    }
  }

  private getInstallationInfo(sublicense: string): Observable<MetaMetricsInstallation> {
    return this.service.getInstallationBySublicense(sublicense).pipe(
      retry(2),
      tap((info: any) => {
        if (info) {
          const dataRow = this.dataSource.data.find(x => x.sublicense === sublicense);
          if (dataRow) {
            dataRow.installation = { ...dataRow.installation, license: info.license, lastTimestamp: info.lastTimestamp };
          }
        }
      })
    );
  }

  private getDynamicRequest(sublicense: string): Observable<Array<MetaMetricsTime4Lines>> {
    return this.service.getTime4Lines({
          sublicense: sublicense,
          measurement: this.defaultMeasurements,
          range: this.selectedRange,
          eachHours: 24,
          license: this.filterLicense,
          version: this.filterVersion,
          app: this.filterApp,
          environment: this.filterEnvironment,
          project: this.filterProject,
          server: this.filterServer,
        }).pipe(
      retry(2),
      tap((info: Array<MetaMetricsTime4Lines>) => {
        if (info) {
          const dataRow = this.dataSource.data.find(x => x.sublicense === sublicense);
          if (dataRow) {
            if (info.length === 0) dataRow.isSpinner = true;
              else dataRow.array4Lines = info;
          }
        }
      })
    );
  }

  private genUniqueId(): string {
    const dateStr = Date
      .now()
      .toString(36);

    const randomStr = Math
      .random()
      .toString(36)
      .substring(2, 8);

    return `${dateStr}-${randomStr}`;
  }

  private isNotNullOrUndefined<T extends Object>(input: null | undefined | T): input is T {
    return input != null;
  }

  checkAndSetActiveAccount() {
    /**
     * If no active account set but there are accounts signed in, sets first account to active account
     * To use active account set here, subscribe to inProgress$ first in your component
     * Note: Basic usage demonstrated. Your app may require more complicated account selection logic
     */
    let activeAccount = this.authService.instance.getActiveAccount();

    if (!activeAccount && this.authService.instance.getAllAccounts().length > 0) {
      let accounts = this.authService.instance.getAllAccounts();
      this.authService.instance.setActiveAccount(accounts[0]);
    }
  }

  setLoginDisplay() {
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
  }

  login() {
    if (this.msalGuardConfig.interactionType === InteractionType.Popup) {
      if (this.msalGuardConfig.authRequest) {
        this.authService.loginPopup({ ...this.msalGuardConfig.authRequest } as PopupRequest)
          .subscribe((response: AuthenticationResult) => {
            this.authService.instance.setActiveAccount(response.account);
          });
      } else {
        this.authService.loginPopup()
          .subscribe((response: AuthenticationResult) => {
            this.authService.instance.setActiveAccount(response.account);
          });
      }
    } else {
      if (this.msalGuardConfig.authRequest) {
        this.authService.loginRedirect({ ...this.msalGuardConfig.authRequest } as RedirectRequest);
      } else {
        this.authService.loginRedirect();
      }
    }
  }

  dynamicColumnsHeaderChange(event: any, index: number) {
    this.defaultMeasurements[index] = event.value.measurementName;
    this.ngOnInit();
  }

  //https://github.com/diprokon/ng-table-virtual-scroll
  changeFilters(event: any) {
    //this.bufferMultiplier = this.measurements.length;
    this.ngOnInit();
  }

  logout() {
    this.authService.logout();
  }

  goToMap() {
    this.router.navigateByUrl('/meta-map-viewer');
  }

  sortColumn(column: string) {
    this.isNameDesc = !this.isNameDesc;
    this.ngOnInit();
  }
  //[ngClass]="{highlighted: selectedRow && selectedRowIndex === i, highlightedOnHover: true}"
  seeShow(row: DataElement) {
    if (this.filterServer && this.filterServer.length > 0) {
      if (row.array4Lines.length === 0) {
        return 'td-hidden ';
      }
      if (row.array4Lines.length > 0 && this.filterServer !== row.array4Lines[0].server) {
        return 'td-hidden ';
      }
    }
    let s = '';
    if (this.selectedRow && this.selectedRowIndex === row.position - 1) s += 'highlighted highlightedOnHover';
    return s;
  }

  ngOnDestroy(): void {
    this._destroying$.next(undefined);
    this._destroying$.complete();
  }
}
