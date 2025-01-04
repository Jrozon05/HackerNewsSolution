import { Component, OnInit } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { HackerNewsService } from '../services/hacker-news.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-stories',
  standalone: true,
  imports: [TableModule, CommonModule, ButtonModule, InputTextModule, HttpClientModule, FormsModule], // Add FormsModule here
  templateUrl: './stories.component.html',
  styleUrls: ['./stories.component.css']
})
export class StoriesComponent implements OnInit {
  stories: any[] = [];
  totalStories: number = 0; // Total number of stories for pagination
  first = 0; // Current offset
  rows = 10; // Rows per page
  loading: boolean = false; // Loading indicator
  searchQuery: string = ''; // Search query

  constructor(private hackerNewsService: HackerNewsService) {}

  ngOnInit() {
    this.loadStories(this.first, this.rows, this.searchQuery);
  }

  loadStories(offset: number, pageSize: number, query: string) {
    this.loading = true;

    if (query) {
      // Perform a search if a query exists
      this.hackerNewsService.searchStories(query).subscribe(
        (data: any) => {
          this.stories = data || [];
          this.totalStories = data.total || 0; // Update total records if provided by API
          this.loading = false;
        },
        (error) => {
          console.error('Error loading stories', error);
          this.loading = false;
        }
      );
    } else {
      // Perform paginated fetch
      const page = offset / pageSize + 1;
      this.hackerNewsService.getPagedStories(page, pageSize).subscribe(
        (data: any) => {
          this.stories = data || [];
          this.totalStories = data.total || 0;
          this.loading = false;
        },
        (error) => {
          console.error('Error loading stories', error);
          this.loading = false;
        }
      );
    }
  }

  pageChange(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    this.loadStories(this.first, this.rows, this.searchQuery);
  }

  onSearch() {
    this.first = 0;
    this.loadStories(this.first, this.rows, this.searchQuery);
  }

  isLastPage(): boolean {
    return this.stories ? this.first + this.rows >= this.totalStories : true;
  }

  isFirstPage(): boolean {
    return this.stories ? this.first === 0 : true;
  }
}
