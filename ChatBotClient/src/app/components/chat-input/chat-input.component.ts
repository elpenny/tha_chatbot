import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TextFieldModule } from '@angular/cdk/text-field';
import { CHAT_CONSTANTS } from '../../constants/chat.constants';

@Component({
  selector: 'app-chat-input',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    TextFieldModule
  ],
  template: `
    <div class="input-container">
      <mat-form-field class="message-input" appearance="outline">
        <textarea matInput 
                  [(ngModel)]="message" 
                  [placeholder]="constants.MESSAGES.PLACEHOLDER_TEXT"
                  (keydown)="onKeyDown($event)"
                  [disabled]="isLoading"
                  rows="1"
                  cdkTextareaAutosize
                  #autosize="cdkTextareaAutosize"
                  cdkAutosizeMinRows="1"
                  cdkAutosizeMaxRows="6"></textarea>
      </mat-form-field>
      <button mat-fab 
              color="primary" 
              (click)="onSendMessage()"
              [disabled]="!message.trim() || isLoading"
              *ngIf="!isLoading">
        <mat-icon>send</mat-icon>
      </button>
      <button mat-fab 
              color="warn" 
              (click)="onCancelMessage()"
              *ngIf="isLoading">
        <mat-icon>stop</mat-icon>
      </button>
    </div>
  `,
  styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent {
  @Input() isLoading = false;
  @Output() sendMessage = new EventEmitter<string>();
  @Output() cancelMessage = new EventEmitter<void>();

  message = '';
  constants = CHAT_CONSTANTS;

  onKeyDown(event: KeyboardEvent) {
    // Enter without Shift = send message
    // Shift+Enter = new line (default behavior)
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.onSendMessage();
    }
  }

  onSendMessage() {
    if (this.message.trim() && !this.isLoading) {
      this.sendMessage.emit(this.message);
      this.message = '';
    }
  }

  onCancelMessage() {
    this.cancelMessage.emit();
  }
}