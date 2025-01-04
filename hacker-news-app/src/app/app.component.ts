import { Component } from '@angular/core';
import { StoriesComponent } from './stories/stories.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [StoriesComponent],
  template: `<app-stories></app-stories>`,
  styles: []
})
export class AppComponent {}
