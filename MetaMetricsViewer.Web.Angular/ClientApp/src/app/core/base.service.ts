import {Injectable} from '@angular/core';
import {Observable, of} from "rxjs";
import {T} from "@angular/cdk/keycodes";
import {HttpHeaders} from "@angular/common/http";

export abstract class BaseService {
  protected httpOptions: { headers: HttpHeaders };

  constructor() {
    this.httpOptions = {headers: new HttpHeaders({'Content-Type': 'application/json'})};
  }


  protected handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(error);
      return of(result as T);
    };
  }
}
