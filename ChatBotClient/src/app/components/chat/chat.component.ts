import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild, ElementRef, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { ChatService, ChatMessageRequest } from '../../api/chat.service';
import { ChatMessage, SSEMessage } from '../../models/chat.model';
import { MessageComponent } from '../message/message.component';
import { ChatInputComponent } from '../chat-input/chat-input.component';
import { CHAT_CONSTANTS } from '../../constants/chat.constants';
import { Subject, takeUntil, Subscription } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MessageComponent,
    ChatInputComponent
  ],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {
  @ViewChild('messagesContainer') messagesContainer!: ElementRef;
  @Output() conversationCreated = new EventEmitter<number>();
  @Output() conversationUpdated = new EventEmitter<number>();
  
  messages: ChatMessage[] = [];
  isLoading = false;
  conversationId: number | null = null;
  constants = CHAT_CONSTANTS;
  
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

  onSendMessage(message: string) {
    if (!message.trim() || this.isLoading) return;

    // Add user message
    this.messages.push({
      content: message,
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
      content: message,
      conversationId: this.conversationId
    };

    this.currentStreamingSubscription = this.chatService.sendMessage(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (sseMessage) => this.handleSSEMessage(sseMessage, botMessage),
        complete: () => this.handleStreamComplete(botMessage),
        error: (error) => this.handleStreamError(error, botMessage)
      });
  }

  onCancelMessage() {
    if (this.currentStreamingSubscription) {
      this.currentStreamingSubscription.unsubscribe();
      this.currentStreamingSubscription = null;
      this.isLoading = false;
      
      // Update the last message to show it was cancelled
      if (this.messages.length > 0) {
        const lastMessage = this.messages[this.messages.length - 1];
        if (!lastMessage.isUser && lastMessage.isTyping) {
          lastMessage.content = lastMessage.content || CHAT_CONSTANTS.MESSAGES.RESPONSE_CANCELLED;
          lastMessage.isTyping = false;
        }
      }
      
      this.changeDetectorRef.detectChanges();
    }
  }

  onRateMessage(event: { messageId: number; rating: number }) {
    const message = this.messages.find(m => m.messageId === event.messageId);
    if (message) {
      const newRating = message.rating === event.rating ? CHAT_CONSTANTS.RATING_VALUES.NONE : event.rating;
      message.rating = newRating;
      
      this.chatService.updateMessageRating(event.messageId, { rating: newRating })
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          error: (error) => {
            // Revert the rating on error
            message.rating = message.rating === event.rating ? CHAT_CONSTANTS.RATING_VALUES.NONE : event.rating;
          }
        });
    }
  }

  trackByMessageId(index: number, message: ChatMessage): any {
    return message.messageId || index;
  }

  onConversationSelected(conversationId: number) {
    if (this.conversationId === conversationId) {
      return; // Already selected
    }
    
    this.conversationId = conversationId;
    this.loadConversationMessages(conversationId);
  }

  onNewConversation() {
    this.conversationId = null;
    this.messages = [];
    this.isLoading = false;
    if (this.currentStreamingSubscription) {
      this.currentStreamingSubscription.unsubscribe();
      this.currentStreamingSubscription = null;
    }
  }

  private loadConversationMessages(conversationId: number) {
    this.isLoading = true;
    this.messages = [];
    
    this.chatService.getConversationHistory(conversationId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          if (result.messages) {
            this.messages = result.messages.map(msg => ({
              content: msg.content || '',
              isUser: msg.role === CHAT_CONSTANTS.MESSAGE_ROLES.USER,
              messageId: msg.id,
              rating: msg.rating
            }));
          }
          this.isLoading = false;
          this.scrollToBottom();
        },
        error: (error) => {
          console.error('Error loading conversation:', error);
          this.isLoading = false;
        }
      });
  }

  private scrollToBottom() {
    setTimeout(() => {
      if (this.messagesContainer) {
        this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
      }
    }, 100);
  }

  private handleSSEMessage(sseMessage: SSEMessage, botMessage: ChatMessage) {
    try {
      const data = JSON.parse(sseMessage.data);
      
      if (data.conversationId && !this.conversationId) {
        this.conversationId = data.conversationId;
        // Emit event when a new conversation is created
        this.conversationCreated.emit(data.conversationId);
      }
      
      if (data.content) {
        botMessage.content = data.content;
        this.changeDetectorRef.detectChanges();
      }
      
      if (data.isComplete) {
        botMessage.isTyping = false;
        this.isLoading = false;
        if (data.conversationId) {
          this.loadConversationHistory(data.conversationId);
          // Emit event when conversation is updated with new message
          this.conversationUpdated.emit(data.conversationId);
        }
        this.changeDetectorRef.detectChanges();
      }
    } catch (error) {
      // Handle parse error silently
    }
  }

  private handleStreamComplete(botMessage: ChatMessage) {
    botMessage.isTyping = false;
    this.isLoading = false;
    this.currentStreamingSubscription = null;
    this.changeDetectorRef.detectChanges();
  }

  private handleStreamError(error: any, botMessage: ChatMessage) {
    if (error.name === 'AbortError') {
      botMessage.content = botMessage.content || CHAT_CONSTANTS.MESSAGES.RESPONSE_CANCELLED;
    } else {
      botMessage.content = CHAT_CONSTANTS.MESSAGES.ERROR_GENERIC;
    }
    botMessage.isTyping = false;
    this.isLoading = false;
    this.currentStreamingSubscription = null;
    this.changeDetectorRef.detectChanges();
  }

  private loadConversationHistory(conversationId: number) {
    this.chatService.getConversationHistory(conversationId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          if (result.messages) {
            this.updateMessagesWithHistory(result.messages);
          }
        },
        error: (error) => {
          // Handle error silently
        }
      });
  }

  private updateMessagesWithHistory(historyMessages: any[]) {
    const sortedHistoryMessages = [...historyMessages].sort((a, b) => 
      new Date(a.createdAt || 0).getTime() - new Date(b.createdAt || 0).getTime()
    );
    
    let userMessageIndex = 0;
    let botMessageIndex = 0;
    
    sortedHistoryMessages.forEach(historyMessage => {
      const isUser = historyMessage.role === CHAT_CONSTANTS.MESSAGE_ROLES.USER;
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
}