import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CuratedItemDialogComponent } from '../curated-item-dialog/curated-item-dialog.component';
import { CuratedItem } from '../models/CuratedItem';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  @Input() title: string | undefined;
  @Output() eventNewItem = new EventEmitter<CuratedItem>();

  formItem: CuratedItem | undefined;

  constructor(public dialog: MatDialog) { }

  openDialog(): void {
    const dialogRef = this.dialog.open(CuratedItemDialogComponent, {
      width: '600px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result !== undefined) {
        this.eventNewItem.emit(result);
        this.formItem = result;
      }
    });
  }
}
