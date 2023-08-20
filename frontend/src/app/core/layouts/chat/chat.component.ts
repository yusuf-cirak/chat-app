import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './chat.component.html',
})
export class ChatComponent {}
