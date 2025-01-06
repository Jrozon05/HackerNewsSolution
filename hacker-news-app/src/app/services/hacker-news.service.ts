import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HackerNewsService {
  private apiUrl = 'https://hacker-news-api-fjegh3fcf7bnbjdb.eastus2-01.azurewebsites.net/api/stories';

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
