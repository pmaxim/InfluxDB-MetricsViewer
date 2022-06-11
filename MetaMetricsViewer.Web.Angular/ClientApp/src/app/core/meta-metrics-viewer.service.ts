import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {catchError, Observable, of} from 'rxjs';
import {environment} from 'src/environments/environment';

import {MetaMetricsInstallation} from '../shared/models/meta-metrics-viewer/installation.model';
import {MetaMetricsMeasurement} from '../shared/models/meta-metrics-viewer/measurement.model';
import {MetaMetricsMeasurementRequest} from '../shared/models/meta-metrics-viewer/mesurement-request.model';
import {
  MetaMetricsTimeLineRequest,
  MetaMetricsTime4LinesRequest,
  MetaMetricsTime4LinesPaginationRequest
} from '../shared/models/meta-metrics-viewer/time-line-request.model';
import {
  MetaMetricsTimeLine, MetaMetricsTime4Lines,
  MetaMetricsFilters, MetaMetricsTime4LinesPagination
} from '../shared/models/meta-metrics-viewer/time-line.model';
import {UserProfileModel} from '../shared/models/account/user-profile-model';
import { BaseService } from './base.service';
import { protectedResources } from '../auth-config';
import { MetaMetricsRangeInfo, ranges } from '../shared/models/meta-metrics-viewer/range-info.model';

const apiUrl = environment.apiUrl + protectedResources.todoListApi.endpoint;

@Injectable({
  providedIn: 'root'
})

export class MetaMetricsViewerService extends BaseService {

  constructor(private readonly http: HttpClient) {
    super();
  }

  getGetSublicenseList(): Observable<string[]> {
    return this.http.get<string[]>(apiUrl + '/GetSublicenseList').pipe(
      catchError(this.handleError<string[]>('GetSublicenseList', []))
    );
  }

  getGetSublicenseListReq(req: MetaMetricsRangeInfo): Observable<string[]> {
    return this.http.post<string[]>(apiUrl + '/GetSublicenseListReq', JSON.stringify(req), this.httpOptions).pipe(
      catchError(this.handleError<string[]>('GetSublicenseListReq', []))
    );
  }

  getMeasurementList(): Observable<MetaMetricsMeasurement[]> {
    return this.http.get<MetaMetricsMeasurement[]>(apiUrl + '/GetMeasurementList').pipe(
      catchError(this.handleError<MetaMetricsMeasurement[]>('getMeasurementList', []))
    );
  }

  getMeasurementListBySublicense(req: MetaMetricsMeasurementRequest): Observable<MetaMetricsMeasurement[]> {
    return this.http.post<MetaMetricsMeasurement[]>(apiUrl + '/GetMeasurementListBySublicense', JSON.stringify(req), this.httpOptions).pipe(
      catchError(this.handleError<MetaMetricsMeasurement[]>('getMeasurementListBySublicense', []))
    );
  }

  getInstallationBySublicense(sublicense: string): Observable<MetaMetricsInstallation> {
    return this.http.post<MetaMetricsInstallation>(apiUrl + '/GetInstallationBySublicense', JSON.stringify(sublicense), this.httpOptions).pipe(
      catchError(this.handleError<MetaMetricsInstallation>('GetInstallationBySublicense', undefined))
    );
  }

  getTimeLine(req: MetaMetricsTimeLineRequest): Observable<MetaMetricsTimeLine> {
    return this.http.post<MetaMetricsTimeLine>(apiUrl + '/GetTimeLine', JSON.stringify(req), this.httpOptions).pipe(
      catchError(this.handleError<MetaMetricsTimeLine>('GetTimeLine', undefined))
    );
  }

  getTime4Lines(req: MetaMetricsTime4LinesRequest): Observable<Array<MetaMetricsTime4Lines>> {
    return this.http.post<Array<MetaMetricsTime4Lines>>(apiUrl + '/GetTime4Lines', JSON.stringify(req), this.httpOptions).pipe(
      catchError(this.handleError<Array<MetaMetricsTime4Lines>>('GetTime4Lines', undefined))
    );
  }

  getTime4LinesPagination(req: MetaMetricsTime4LinesPaginationRequest): Observable<MetaMetricsTime4LinesPagination> {
    return this.http.post<MetaMetricsTime4LinesPagination>(apiUrl + '/GetTime4LinesPagination', JSON.stringify(req), this.httpOptions).pipe(
      catchError(this.handleError<MetaMetricsTime4LinesPagination>('GetTime4LinesPagination', undefined))
    );
  }

  getFilters(): Observable<MetaMetricsFilters> {
    return this.http.get<MetaMetricsFilters>(apiUrl + '/GetFilters').pipe(
      catchError(this.handleError<MetaMetricsFilters>('GetFilters', undefined))
    );
  }

  getFiltersValue(req: MetaMetricsRangeInfo): Observable<MetaMetricsFilters> {
    return this.http.post<MetaMetricsFilters>(apiUrl + '/GetFiltersValue', JSON.stringify(req), this.httpOptions).pipe(
      catchError(this.handleError<MetaMetricsFilters>('GetFiltersValue', undefined))
    );
  }
}
