import { ChangeDetectionStrategy, Component, OnChanges, SimpleChanges } from '@angular/core';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { Color } from 'ng2-charts';
import { Observable, tap } from 'rxjs';

import { MetaMetricsViewerService } from '../../../core/meta-metrics-viewer.service';
import { MetaMetricsRangeInfo } from '../../models/meta-metrics-viewer/range-info.model';
import { MetaMetricsTimeLine, MetaMetricsTime4Lines } from '../../models/meta-metrics-viewer/time-line.model';
import { MetaMetricsMeasurement } from '../../../shared/models/meta-metrics-viewer/measurement.model';

@Component({
  selector: 'app-line-chart',
  templateUrl: './line-chart.component.html',
  styleUrls: ['./line-chart.component.scss'],
  inputs: ['sublicense', 'measurement', 'range','array4Lines'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class LineChartComponent implements OnChanges {
  measurement!: MetaMetricsMeasurement;
  array4Lines!: Array<MetaMetricsTime4Lines>;

  lineChartData: ChartDataSets[] = [];

  lineChartOptions: ChartOptions = {
    responsive: true,
    // maintainAspectRatio: false,
    animation: {duration: 0},
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

  lineChartLegend = true;
  lineChartColors: Color[] = [{backgroundColor:'#b0bbd1'}];

  ngOnChanges(changes: SimpleChanges): void {
    if (this.array4Lines && this.array4Lines.length > 0 && this.measurement) {
      const t = this.array4Lines.filter(z => z._measurement === this.measurement.measurementName);
      if (t.length === 0) return;
      this.lineChartData = [{
        data: t[0].metaMetricsTimeLine.timeValues.map(t => { return { x: t.timestamp, y: t.value }; }),
        label: this.measurement.measurementDisplay //+ ' ' + this.measurement.measurementName
      }];
    }
  }
}
