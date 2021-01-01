import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { CuratedItem } from './models/CuratedItem';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Curator Gallery';
  formItem: CuratedItem | undefined;
  eventNewItem: Subject<CuratedItem> = new Subject<CuratedItem>()

  constructor(public dialog: MatDialog) {}

  gotNewItem(newItem: CuratedItem) {
    this.eventNewItem.next(newItem);
  }

  get diagnostic() { return JSON.stringify(this.formItem); }
}
