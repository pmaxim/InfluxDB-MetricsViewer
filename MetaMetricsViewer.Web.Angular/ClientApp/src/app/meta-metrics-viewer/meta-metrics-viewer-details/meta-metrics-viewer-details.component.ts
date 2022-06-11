import {
  ChangeDetectionStrategy, Component, OnChanges,
  SimpleChanges, ChangeDetectorRef
} from '@angular/core';

import { MetaMetricsRangeInfo } from '../../shared/models/meta-metrics-viewer/range-info.model';
import { MetaMetricsMeasurement } from '../../shared/models/meta-metrics-viewer/measurement.model';
import { MetaMetricsViewerService } from '../../core/meta-metrics-viewer.service';
import { MetaMetricsTime4Lines } from '../../shared/models/meta-metrics-viewer/time-line.model';
import { MatDialog } from '@angular/material/dialog';
import { ChartViewModalComponent } from '../../shared/components/chart-view-modal/chart-view-modal.component';

@Component({
  selector: 'app-meta-metrics-viewer-details',
  templateUrl: './meta-metrics-viewer-details.component.html',
  styleUrls: ['./meta-metrics-viewer-details.component.scss'],
  inputs: ['sublicense', 'measurements','range'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class MetaMetricsViewerDetailsComponent implements OnChanges {
  sublicense!: string;
  measurements: MetaMetricsMeasurement[] | null = [];
  range!: MetaMetricsRangeInfo;
  array4Lines!: Array<MetaMetricsTime4Lines>;

  constructor(private readonly service: MetaMetricsViewerService,
    private readonly changeDetectorRef: ChangeDetectorRef,
    private readonly  dialog: MatDialog) { 
  }

  showModal(item: MetaMetricsMeasurement) {
    const dialogRef = this.dialog.open(ChartViewModalComponent,
      {
        height: '400px',
        width: '600px',
        data: {
          measurement: item,
          sublicense: this.sublicense
        }
      });

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.measurements) return;

    this.array4Lines = new Array<MetaMetricsTime4Lines>();

    this.service.getTime4Lines({
      sublicense: this.sublicense,
      measurement: this.measurements.map(z => z.measurementName),
      range: this.range,
      eachHours: 1,
      license: '',
      version: '',
      app: '',
      environment: '',
      project: '',
      server: ''
    }).subscribe((response: Array<MetaMetricsTime4Lines>) => {
      this.array4Lines = response;
      this.changeDetectorRef.detectChanges();
    });
  }
}


