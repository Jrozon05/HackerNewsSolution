import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class HackerNewsService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllStories(): Observable<any> {
    return this.http.get(`${this.apiUrl}/newest`);
  }

  getPagedStories(page: number, pageSize: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/paged?page=${page}&pageSize=${pageSize}`);
  }

  searchStories(query: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/search?query=${query}`);
  }
}
