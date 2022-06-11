import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MetaMainViewerComponent } from './meta-main-viewer.component';

describe('MetaMainViewerComponent', () => {
  let component: MetaMainViewerComponent;
  let fixture: ComponentFixture<MetaMainViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MetaMainViewerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MetaMainViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
