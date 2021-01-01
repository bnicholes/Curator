import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { CuratorService } from '../curator.service';
import { MessageService } from '../message.service';
import { CuratedItem } from '../models/CuratedItem';
import { MatDialog } from '@angular/material/dialog';

import { CuratedItemDialogComponent } from '../curated-item-dialog/curated-item-dialog.component';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { Observable, Subscription } from 'rxjs';

@Component({
  selector: 'app-photo-gallery',
  templateUrl: './photo-gallery.component.html',
  styleUrls: ['./photo-gallery.component.css']
})
export class PhotoGalleryComponent implements OnInit, OnDestroy {
  curatedItems: CuratedItem[] | undefined;
  private eventNewItemSubscription!: Subscription;

  @Input()
  eventNewItem!: Observable<CuratedItem>;

  constructor(public dialog: MatDialog, private CuratorService: CuratorService , private messageService: MessageService) { }

  ngOnInit(): void {
    this.eventNewItemSubscription = this.eventNewItem.subscribe(item => this.curatedItems?.push(item));
    this.CuratorService.getPhotos().subscribe( response => {
      this.curatedItems = response as CuratedItem[];
    }, error => {
      console.log(error);
      this.messageService.add(error);
    })
  }

  ngOnDestroy() {
    this.eventNewItemSubscription.unsubscribe();
  }

  EditCuratedPhoto(item: CuratedItem): void {
    const dialogRef = this.dialog.open(CuratedItemDialogComponent, {
      width: '600px',
      data: item
    });

    dialogRef.afterClosed().subscribe(result => {
      item = result;
    });
  }

  RemoveCuratedPhoto(item: CuratedItem): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '350px',
      data: `Remove ${item.Name} from the database?`
    });

    dialogRef.afterClosed().subscribe(result => {
      if(result) {
        this.CuratorService.removePhoto(item.Key).subscribe( response => {
          this.curatedItems = this.curatedItems?.filter(x => x.Key != item.Key);
        }, error => {
          console.log(error);
          this.messageService.add(error);
        })
      }
    });

  }
}
