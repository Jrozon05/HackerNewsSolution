import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HackerNewsService } from '../services/hacker-news.service';

@Component({
  selector: 'app-stories',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stories.component.html',
  styleUrls: ['./stories.component.css']
})
export class StoriesComponent implements OnInit {
  stories: any[] = [];

  constructor(private hackerNewsService: HackerNewsService) {}

  ngOnInit() {
    this.hackerNewsService.getPagedStories(1, 10).subscribe(data => {
      this.stories = data;
    });
  }
}
