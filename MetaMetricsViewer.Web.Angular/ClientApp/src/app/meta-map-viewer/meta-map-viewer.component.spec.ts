import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MetaMapViewerComponent } from './meta-map-viewer.component';

describe('MetaMapViewerComponent', () => {
  let component: MetaMapViewerComponent;
  let fixture: ComponentFixture<MetaMapViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MetaMapViewerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MetaMapViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
