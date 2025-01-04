import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root' // Makes the service globally available without needing to include it in providers
})
export class HackerNewsService {
  private apiUrl = 'http://localhost:5217/api/stories'; // Replace with your backend API URL

  constructor(private http: HttpClient) {}

  getPagedStories(page: number, pageSize: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/paged?page=${page}&pageSize=${pageSize}`);
  }

  searchStories(query: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/search?query=${query}`);
  }
}
