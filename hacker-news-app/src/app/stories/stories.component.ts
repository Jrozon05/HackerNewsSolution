import { Component, OnInit } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { HackerNewsService } from '../services/hacker-news.service';
import { FormsModule } from '@angular/forms';
import { RatingModule } from 'primeng/rating';

@Component({
  selector: 'app-stories',
  standalone: true,
  imports: [
    TableModule,
    CommonModule,
    ButtonModule,
    InputTextModule,
    HttpClientModule,
    FormsModule,
    RatingModule,
  ],
  templateUrl: './stories.component.html',
  styleUrls: ['./stories.component.css']
})
export class StoriesComponent implements OnInit {
  stories: any[] = [];
  totalStories: number = 0;
  first = 0;
  rows = 10;
  loading: boolean = false;
  searchQuery: string = '';

  constructor(private hackerNewsService: HackerNewsService) {}

  ngOnInit() {
    this.fetchAllStories();
  }

  fetchAllStories() {
    this.loading = true;
    this.hackerNewsService.getAllStories().subscribe(
      (data: any) => {
        this.stories = (data || []).map((story: any) => ({
          ...story
        }));
        this.totalStories = this.stories.length;
        this.loading = false;
      },
      (error) => {
        console.error('Error loading all stories', error);
        this.stories = [];
        this.totalStories = 0;
        this.loading = false;
      }
    );
  }

  loadStories(offset: number, pageSize: number) {
    this.loading = true;
    const page = offset / pageSize + 1;

    this.hackerNewsService.getPagedStories(page, pageSize).subscribe(
      (data: any) => {
        this.stories = (data || []).map((story: any) => ({
          ...story
        }));
        this.totalStories = this.stories.length || 0;
        this.loading = false;
      },
      (error) => {
        console.error('Error loading paged stories', error);
        this.stories = [];
        this.totalStories = 0;
        this.loading = false;
      }
    );
  }

  pageChange(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    this.loadStories(this.first, this.rows);
  }

  onSearch() {
    if (this.searchQuery.trim() === '') {
      this.fetchAllStories();
    } else {
      this.loading = true;
      this.hackerNewsService.searchStories(this.searchQuery).subscribe(
        (data: any) => {
          this.stories = data || [];
          this.totalStories = this.stories.length;
          this.loading = false;
        },
        (error) => {
          console.error('Error searching stories', error);
          this.stories = [];
          this.totalStories = 0;
          this.loading = false;
        }
      );
    }
  }

  isLastPage(): boolean {
    return this.stories ? this.first + this.rows >= this.totalStories : true;
  }

  isFirstPage(): boolean {
    return this.stories ? this.first === 0 : true;
  }
}
