import { NgFor } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-list-skeleton',
  standalone: true,
  imports: [NgFor],
  template: `<div
    role="status"
    class="w-full space-y-1 border-gray-200 divide-y divide-gray-200 rounded shadow animate-pulse dark:divide-gray-700 px-5 pt-1 dark:border-gray-700"
  >
    <div
      class="flex items-center justify-between hover:cursor-pointer"
      *ngFor="let _ of items"
    >
      <div class="flex items-center mt-4 space-x-3">
        <svg
          class="w-12 h-12 text-gray-200 dark:text-gray-700"
          aria-hidden="true"
          xmlns="http://www.w3.org/2000/svg"
          fill="currentColor"
          viewBox="0 0 20 20"
        >
          <path
            d="M10 0a10 10 0 1 0 10 10A10.011 10.011 0 0 0 10 0Zm0 5a3 3 0 1 1 0 6 3 3 0 0 1 0-6Zm0 13a8.949 8.949 0 0 1-4.951-1.488A3.987 3.987 0 0 1 9 13h2a3.987 3.987 0 0 1 3.951 3.512A8.949 8.949 0 0 1 10 18Z"
          />
        </svg>
        <div>
          <div
            class="h-2.5 bg-gray-200 rounded-full dark:bg-gray-700 w-32 mb-2"
          ></div>
          <div class="w-48 h-2 bg-gray-200 rounded-full dark:bg-gray-700"></div>
        </div>
      </div>
    </div>
  </div> `,
})
export class ListSkeletonComponent {
  items: number[] = [];

  @Input('count') set setItems(count: number) {
    this.items = Array(count).fill(0);
  }
}
