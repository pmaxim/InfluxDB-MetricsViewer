import { Component, OnInit, Inject, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MetaMetricsMeasurement } from '../../models/meta-metrics-viewer/measurement.model';
import { MetaMetricsTimeLine, MetaMetricsTime4Lines } from '../../models/meta-metrics-viewer/time-line.model';
import { MetaMetricsViewerService } from '../../../core/meta-metrics-viewer.service';
import { MetaMetricsRangeInfo, ranges } from '../../models/meta-metrics-viewer/range-info.model';

@Component({
  selector: 'app-chart-view-modal',
  templateUrl: './chart-view-modal.component.html',
  styleUrls: ['./chart-view-modal.component.scss']
})
export class ChartViewModalComponent implements OnInit {
  measurement!: MetaMetricsMeasurement;
  array4Lines!: Array<MetaMetricsTime4Lines>;
  sublicense!: string;
  ranges: MetaMetricsRangeInfo[] = ranges();
  selectedRange!: MetaMetricsRangeInfo;

  constructor(public dialogRef: MatDialogRef<ChartViewModalComponent>,
    private readonly service: MetaMetricsViewerService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any) {
    this.measurement = data.measurement;
    this.sublicense = data.sublicense;
    this.selectedRange = this.ranges.find(x => x.default) ?? { display: '1w', weeks: 1 };
  }

  ngOnInit(): void {
    this.service.getTime4Lines({
      sublicense: this.sublicense,
      measurement: this.measurement.measurementName.split(','),
      range: this.selectedRange,
      eachHours: 1,
      license: '',
      version: '',
      app: '',
      environment: '',
      project: '',
      server: '',
    }).subscribe((info: Array<MetaMetricsTime4Lines>) => {
        if (info) {
          this.array4Lines = info;
        }
      }
    );
  }

  changeFilters(event: any) {
    this.ngOnInit();
  }

}
