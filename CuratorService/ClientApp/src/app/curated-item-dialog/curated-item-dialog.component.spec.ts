import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CuratedItemDialogComponent } from './curated-item-dialog.component';

describe('CuratedItemDialogComponent', () => {
  let component: CuratedItemDialogComponent;
  let fixture: ComponentFixture<CuratedItemDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CuratedItemDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CuratedItemDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
