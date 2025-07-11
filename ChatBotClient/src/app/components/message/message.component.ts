import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChatMessage } from '../../models/chat.model';
import { CHAT_CONSTANTS } from '../../constants/chat.constants';

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="message" 
         [ngClass]="{'user-message': message.isUser, 'bot-message': !message.isUser}">
      <div class="message-content">
        {{ message.content }}
        <mat-spinner *ngIf="message.isTyping" [diameter]="constants.SIZES.SPINNER_DIAMETER"></mat-spinner>
      </div>
      <div class="message-actions" *ngIf="!message.isUser && !message.isTyping && message.messageId">
        <button mat-icon-button 
                (click)="onRateMessage(constants.RATING_VALUES.THUMBS_UP)"
                [class.rated]="message.rating === constants.RATING_VALUES.THUMBS_UP"
                class="rating-button thumbs-up">
          <mat-icon>thumb_up</mat-icon>
        </button>
        <button mat-icon-button 
                (click)="onRateMessage(constants.RATING_VALUES.THUMBS_DOWN)"
                [class.rated]="message.rating === constants.RATING_VALUES.THUMBS_DOWN"
                class="rating-button thumbs-down">
          <mat-icon>thumb_down</mat-icon>
        </button>
      </div>
    </div>
  `,
  styleUrls: ['./message.component.scss']
})
export class MessageComponent {
  @Input() message!: ChatMessage;
  @Output() rateMessage = new EventEmitter<{ messageId: number; rating: number }>();

  constants = CHAT_CONSTANTS;

  onRateMessage(rating: number) {
    if (this.message.messageId) {
      this.rateMessage.emit({ messageId: this.message.messageId, rating });
    }
  }
}