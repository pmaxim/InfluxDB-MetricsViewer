import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LineChartComponent } from './components/line-chart/line-chart.component';
import { MaterialModule } from './material.module';
import { ChartsModule } from 'ng2-charts';
import { HeaderComponent } from './components/header/header.component';
import { ChartViewModalComponent } from './components/chart-view-modal/chart-view-modal.component';
import { SpinnerComponent } from '../shared/components/spinner/spinner.component';

@NgModule({
  declarations: [
    LineChartComponent,
    HeaderComponent,
    ChartViewModalComponent,
    SpinnerComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    ChartsModule
  ],
  exports: [
    LineChartComponent,    
    HeaderComponent,
    MaterialModule,
    ChartsModule,
    SpinnerComponent
  ]
})
export class SharedModule { }
