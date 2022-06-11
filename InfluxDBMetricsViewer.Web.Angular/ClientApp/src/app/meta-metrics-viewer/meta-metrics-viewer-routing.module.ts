import { RouterModule, Routes } from '@angular/router';
import { MetaMetricsViewerMainComponent } from './meta-metrics-viewer-main/meta-metrics-viewer-main.component';

const routes: Routes = [
  {
    path: '',
    component: MetaMetricsViewerMainComponent
  }
];

export const MetaMetricsViewerRoutingModule = RouterModule.forChild(routes);
