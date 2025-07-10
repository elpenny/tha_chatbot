import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChatService, ChatMessageRequest, SSEMessage } from '../../api/chat.service';
import { Subject, takeUntil } from 'rxjs';

interface ChatMessage {
  content: string;
  isUser: boolean;
  isTyping?: boolean;
}

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="chat-container">
      <mat-card class="chat-card">
        <mat-card-header>
          <mat-card-title>AI Chatbot</mat-card-title>
        </mat-card-header>
        
        <mat-card-content class="chat-content">
          <div class="messages-container" #messagesContainer>
            <div *ngFor="let message of messages" 
                 class="message" 
                 [ngClass]="{'user-message': message.isUser, 'bot-message': !message.isUser}">
              <div class="message-content">
                {{ message.content }}
                <mat-spinner *ngIf="message.isTyping" diameter="16"></mat-spinner>
              </div>
            </div>
          </div>
          
          <div class="input-container">
            <mat-form-field class="message-input" appearance="outline">
              <input matInput 
                     [(ngModel)]="currentMessage" 
                     placeholder="Type your message..."
                     (keyup.enter)="sendMessage()"
                     [disabled]="isLoading">
            </mat-form-field>
            <button mat-fab 
                    color="primary" 
                    (click)="sendMessage()"
                    [disabled]="!currentMessage.trim() || isLoading">
              <mat-icon>send</mat-icon>
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .chat-container {
      height: 100vh;
      display: flex;
      padding: 20px;
      background-color: #f5f5f5;
    }
    
    .chat-card {
      width: 100%;
      max-width: 800px;
      margin: 0 auto;
      display: flex;
      flex-direction: column;
    }
    
    .chat-content {
      flex: 1;
      display: flex;
      flex-direction: column;
      height: calc(100vh - 180px);
    }
    
    .messages-container {
      flex: 1;
      overflow-y: auto;
      padding: 16px 0;
      margin-bottom: 16px;
    }
    
    .message {
      margin-bottom: 16px;
      display: flex;
    }
    
    .user-message {
      justify-content: flex-end;
    }
    
    .bot-message {
      justify-content: flex-start;
    }
    
    .message-content {
      max-width: 70%;
      padding: 12px 16px;
      border-radius: 18px;
      word-wrap: break-word;
      display: flex;
      align-items: center;
      gap: 8px;
    }
    
    .user-message .message-content {
      background-color: #2196f3;
      color: white;
    }
    
    .bot-message .message-content {
      background-color: #e0e0e0;
      color: #333;
    }
    
    .input-container {
      display: flex;
      gap: 12px;
      align-items: flex-end;
    }
    
    .message-input {
      flex: 1;
    }
  `]
})
export class ChatComponent implements OnInit, OnDestroy {
  messages: ChatMessage[] = [];
  currentMessage: string = '';
  isLoading: boolean = false;
  conversationId: number | null = null;
  
  private destroy$ = new Subject<void>();

  constructor(
    private chatService: ChatService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    // Add welcome message
    this.messages.push({
      content: 'Hello! How can I help you today?',
      isUser: false
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  sendMessage() {
    if (!this.currentMessage.trim() || this.isLoading) return;

    const userMessage = this.currentMessage;
    this.currentMessage = '';
    
    // Add user message
    this.messages.push({
      content: userMessage,
      isUser: true
    });

    // Add typing indicator for bot response
    const botMessage: ChatMessage = {
      content: '',
      isUser: false,
      isTyping: true
    };
    this.messages.push(botMessage);

    this.isLoading = true;

    const request: ChatMessageRequest = {
      content: userMessage,
      conversationId: this.conversationId
    };

    this.chatService.sendMessage(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (sseMessage) => {
          console.log('Received SSE message:', sseMessage);
          try {
            const data = JSON.parse(sseMessage.data);
            console.log('Parsed SSE data:', data);
            
            if (data.conversationId && !this.conversationId) {
              this.conversationId = data.conversationId;
            }
            
            if (data.content) {
              botMessage.content = data.content; // Use = instead of += since backend sends full content
              this.changeDetectorRef.detectChanges(); // Force UI update
            }
            
            if (data.isComplete) {
              botMessage.isTyping = false;
              this.isLoading = false;
              this.changeDetectorRef.detectChanges(); // Force UI update
            }
          } catch (error) {
            console.error('Error parsing SSE message:', error, 'Raw data:', sseMessage.data);
          }
        },
        complete: () => {
          botMessage.isTyping = false;
          this.isLoading = false;
          this.changeDetectorRef.detectChanges(); // Force UI update
        },
        error: (error) => {
          console.error('Error receiving message:', error);
          botMessage.content = 'Sorry, something went wrong. Please try again.';
          botMessage.isTyping = false;
          this.isLoading = false;
          this.changeDetectorRef.detectChanges(); // Force UI update
        }
      });
  }
}