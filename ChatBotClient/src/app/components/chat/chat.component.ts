import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChatService, ChatMessageRequest, SSEMessage } from '../../api/chat.service';
import { Subject, takeUntil, Subscription } from 'rxjs';

interface ChatMessage {
  content: string;
  isUser: boolean;
  isTyping?: boolean;
  messageId?: number;
  rating?: number | null;
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
            <div *ngFor="let message of messages; trackBy: trackByMessageId" 
                 class="message" 
                 [ngClass]="{'user-message': message.isUser, 'bot-message': !message.isUser}">
              <div class="message-content">
                {{ message.content }}
                <mat-spinner *ngIf="message.isTyping" diameter="16"></mat-spinner>
              </div>
              <div class="message-actions" *ngIf="!message.isUser && !message.isTyping && message.messageId">
                <button mat-icon-button 
                        (click)="rateMessage(message.messageId!, 1)"
                        [class.rated]="message.rating === 1"
                        class="rating-button thumbs-up">
                  <mat-icon>thumb_up</mat-icon>
                </button>
                <button mat-icon-button 
                        (click)="rateMessage(message.messageId!, -1)"
                        [class.rated]="message.rating === -1"
                        class="rating-button thumbs-down">
                  <mat-icon>thumb_down</mat-icon>
                </button>
              </div>
            </div>
          </div>
          
          <div class="input-container">
            <mat-form-field class="message-input" appearance="outline">
              <input matInput 
                     [(ngModel)]="currentMessage" 
                     placeholder="Start chatting..."
                     (keyup.enter)="sendMessage()"
                     [disabled]="isLoading">
            </mat-form-field>
            <button mat-fab 
                    color="primary" 
                    (click)="sendMessage()"
                    [disabled]="!currentMessage.trim() || isLoading"
                    *ngIf="!isLoading">
              <mat-icon>send</mat-icon>
            </button>
            <button mat-fab 
                    color="warn" 
                    (click)="cancelMessage()"
                    *ngIf="isLoading">
              <mat-icon>stop</mat-icon>
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
      align-items: flex-start;
    }
    
    .message-input {
      flex: 1;
    }
    
    .message-actions {
      display: flex;
      gap: 4px;
      margin-top: 8px;
      justify-content: flex-start;
    }
    
    .rating-button {
      width: 32px;
      height: 32px;
      color: #666;
      transition: color 0.2s ease;
    }
    
    .rating-button:hover {
      color: #333;
    }
    
    .rating-button.rated.thumbs-up {
      color: #4caf50;
    }
    
    .rating-button.rated.thumbs-down {
      color: #f44336;
    }
    
    .rating-button.rated:hover {
      color: inherit;
    }
  `]
})
export class ChatComponent implements OnInit, OnDestroy {
  messages: ChatMessage[] = [];
  currentMessage: string = '';
  isLoading: boolean = false;
  conversationId: number | null = null;
  
  private destroy$ = new Subject<void>();
  private currentStreamingSubscription: Subscription | null = null;

  constructor(
    private chatService: ChatService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    // No initial welcome message
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

    this.currentStreamingSubscription = this.chatService.sendMessage(request)
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
              // Set message ID when response is complete - we'll need to get this from conversation history
              if (data.conversationId) {
                this.loadConversationHistory(data.conversationId);
              }
              this.changeDetectorRef.detectChanges(); // Force UI update
            }
          } catch (error) {
            console.error('Error parsing SSE message:', error, 'Raw data:', sseMessage.data);
          }
        },
        complete: () => {
          botMessage.isTyping = false;
          this.isLoading = false;
          this.currentStreamingSubscription = null;
          this.changeDetectorRef.detectChanges(); // Force UI update
        },
        error: (error) => {
          console.error('Error receiving message:', error);
          if (error.name === 'AbortError') {
            botMessage.content = botMessage.content || 'Response cancelled.';
          } else {
            botMessage.content = 'Sorry, something went wrong. Please try again.';
          }
          botMessage.isTyping = false;
          this.isLoading = false;
          this.currentStreamingSubscription = null;
          this.changeDetectorRef.detectChanges(); // Force UI update
        }
      });
  }

  cancelMessage() {
    if (this.currentStreamingSubscription) {
      this.currentStreamingSubscription.unsubscribe();
      this.currentStreamingSubscription = null;
      this.isLoading = false;
      
      // Update the last message to show it was cancelled
      if (this.messages.length > 0) {
        const lastMessage = this.messages[this.messages.length - 1];
        if (!lastMessage.isUser && lastMessage.isTyping) {
          lastMessage.content = lastMessage.content || 'Response cancelled.';
          lastMessage.isTyping = false;
        }
      }
      
      this.changeDetectorRef.detectChanges();
    }
  }

  trackByMessageId(index: number, message: ChatMessage): any {
    return message.messageId || index;
  }

  rateMessage(messageId: number, rating: number) {
    // Find the message and update its rating optimistically
    const message = this.messages.find(m => m.messageId === messageId);
    if (message) {
      const newRating = message.rating === rating ? null : rating; // Toggle rating if same value
      message.rating = newRating;
      
      this.chatService.updateMessageRating(messageId, { rating: newRating })
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            console.log('Rating updated successfully');
          },
          error: (error) => {
            console.error('Error updating rating:', error);
            // Revert the rating on error
            message.rating = message.rating === rating ? null : rating;
          }
        });
    }
  }

  private loadConversationHistory(conversationId: number) {
    this.chatService.getConversationHistory(conversationId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          if (result.messages) {
            // Sort messages by creation time to ensure proper order
            const sortedHistoryMessages = [...result.messages].sort((a, b) => 
              new Date(a.createdAt || 0).getTime() - new Date(b.createdAt || 0).getTime()
            );
            
            // Track indices for user and bot messages separately
            let userMessageIndex = 0;
            let botMessageIndex = 0;
            
            sortedHistoryMessages.forEach(historyMessage => {
              const isUser = historyMessage.role === 0; // 0 = User, 1 = Bot
              
              // Find the corresponding message by matching role and index
              const currentMessages = this.messages.filter(m => m.isUser === isUser);
              const messageIndex = isUser ? userMessageIndex : botMessageIndex;
              
              if (messageIndex < currentMessages.length) {
                const currentMessage = currentMessages[messageIndex];
                if (historyMessage.id) {
                  currentMessage.messageId = historyMessage.id;
                  currentMessage.rating = historyMessage.rating;
                }
              }
              
              if (isUser) {
                userMessageIndex++;
              } else {
                botMessageIndex++;
              }
            });
            
            this.changeDetectorRef.detectChanges();
          }
        },
        error: (error) => {
          console.error('Error loading conversation history:', error);
        }
      });
  }
}