import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CHAT_CONSTANTS } from '../../constants/chat.constants';

@Component({
  selector: 'app-chat-input',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="input-container">
      <mat-form-field class="message-input" appearance="outline">
        <input matInput 
               [(ngModel)]="message" 
               [placeholder]="constants.MESSAGES.PLACEHOLDER_TEXT"
               (keyup.enter)="onSendMessage()"
               [disabled]="isLoading">
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