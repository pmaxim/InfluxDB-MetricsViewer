import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { MetaMetricsViewerService } from './meta-metrics-viewer.service';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    HttpClientModule
  ],
  providers: [MetaMetricsViewerService]
})
export class CoreModule { }
