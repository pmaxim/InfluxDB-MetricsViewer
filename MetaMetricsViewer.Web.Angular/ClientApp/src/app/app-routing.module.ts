import {Injectable, NgModule} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterModule, RouterStateSnapshot, Routes, UrlTree} from '@angular/router';
import {MetaMetricsViewerModule} from './meta-metrics-viewer/meta-metrics-viewer.module';
import {PageNotFoundComponent} from './page-not-found/page-not-found.component';
import { FormsModule } from '@angular/forms';
import { MsalGuard } from '@azure/msal-angular';
import { MetaMapViewerComponent } from './meta-map-viewer/meta-map-viewer.component';
import { MetaMainViewerComponent } from './meta-main-viewer/meta-main-viewer.component';


const routes: Routes = [
  { path: 'meta-metrics-viewer', loadChildren: () => MetaMetricsViewerModule, canActivate: [MsalGuard]},
  { path: '', redirectTo: '/meta-main-viewer', pathMatch: 'full' },
  { path: 'meta-map-viewer', component: MetaMapViewerComponent, canActivate: [MsalGuard] },
  { path: 'meta-main-viewer', component: MetaMainViewerComponent, canActivate: [MsalGuard] },
  {path: '**', component: PageNotFoundComponent}
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    FormsModule,
  ],
  exports: [RouterModule],
  providers: [
    MsalGuard
  ]
})

export class AppRoutingModule {}


