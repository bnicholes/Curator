import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CuratorService } from '../curator.service';
import { MessageService } from '../message.service';
import { CuratedItem } from '../models/CuratedItem';

@Component({
  selector: 'app-curated-item-dialog',
  templateUrl: './curated-item-dialog.component.html',
  styleUrls: ['./curated-item-dialog.component.css']
})
export class CuratedItemDialogComponent implements OnInit {

  submitted = false;
  curatedItem : CuratedItem | undefined;

  onSubmit(data: CuratedItem) {
    this.submitted = true;

    this.CuratorService.savePhoto(data).subscribe( (response) => {
      this.curatedItem = response as CuratedItem;
      //console.log(`${this.curatedItem.Name} ${this.curatedItem.Path}`);
    }, (error) => {
      console.log(error);
      this.messageService.add(error);
    })

  }

  constructor(public dialogRef: MatDialogRef<CuratedItemDialogComponent>,
    private CuratorService: CuratorService , private messageService: MessageService,
    @Inject(MAT_DIALOG_DATA) public model: CuratedItem) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  ngOnInit(): void {
  }

  get diagnostic() { return JSON.stringify(this.model); }
}
