import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-meta-map-viewer',
  templateUrl: './meta-map-viewer.component.html',
  styleUrls: ['./meta-map-viewer.component.scss']
})

export class MetaMapViewerComponent implements OnInit {
  lat = 22.2736308;
  long = 70.7512555;

  constructor() { }

  ngOnInit(): void {
  }

}
