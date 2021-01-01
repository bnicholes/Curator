import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { CuratedItem } from './models/CuratedItem';

@Injectable({
  providedIn: 'root'
})
export class CuratorService {
  // Use for local developement
  curatorServiceUrl = 'https://localhost/service/curator';

  // Use for production
  //curatorServiceUrl = 'service/curator';

  constructor(private http: HttpClient) { }

  getPhotos(): Observable<CuratedItem[]> {
      return this.http.get<CuratedItem[]>(this.curatorServiceUrl).pipe(catchError(this.handleError));
  }

  savePhoto(data: CuratedItem): Observable<CuratedItem> {
    const body = JSON.stringify(data);
    const headerOptions = new HttpHeaders({ 'Content-Type':'application/json'});
    if (data.Key == null) {
      return this.http.post<CuratedItem>(this.curatorServiceUrl, body, {headers: headerOptions}).pipe(catchError(this.handleError));
    }
    else {
      return this.http.put<CuratedItem>(this.curatorServiceUrl + '/' + encodeURIComponent(data.Key), body, {headers: headerOptions}).pipe(catchError(this.handleError));
    }
  }

  removePhoto(key: string): Observable<object> {
    const headerOptions = new HttpHeaders({ 'Content-Type':'application/json'});
    return this.http.delete(this.curatorServiceUrl + '/' + encodeURIComponent(key), {headers: headerOptions}).pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // Return an observable with a user-facing error message.
    return throwError(
      'Something bad happened; please try again later.');
  }
}
