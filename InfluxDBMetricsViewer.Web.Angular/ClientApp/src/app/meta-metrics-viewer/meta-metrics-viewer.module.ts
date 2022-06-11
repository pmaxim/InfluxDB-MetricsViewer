import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TableVirtualScrollModule } from 'ng-table-virtual-scroll';
import { SharedModule } from '../shared/shared.module';
import { MetaMetricsViewerMainComponent } from './meta-metrics-viewer-main/meta-metrics-viewer-main.component';
import { MetaMetricsViewerRoutingModule } from './meta-metrics-viewer-routing.module';
import { MetaMetricsViewerDetailsComponent } from './meta-metrics-viewer-details/meta-metrics-viewer-details.component';


@NgModule({
  declarations: [
    MetaMetricsViewerMainComponent,
    MetaMetricsViewerDetailsComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    TableVirtualScrollModule,
    MetaMetricsViewerRoutingModule
  ]
})
export class MetaMetricsViewerModule { }
