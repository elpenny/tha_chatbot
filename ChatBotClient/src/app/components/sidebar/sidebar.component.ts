import { Component, OnInit, OnDestroy, Output, EventEmitter, Input, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ChatService, ConversationSummary } from '../../api/chat.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatTooltipModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit, OnDestroy {
  @Output() conversationSelected = new EventEmitter<number>();
  @Output() newConversation = new EventEmitter<void>();
  @Input() selectedConversationId: number | null = null;

  conversations: ConversationSummary[] = [];
  isLoading = false;
  
  private destroy$ = new Subject<void>();

  constructor(
    private chatService: ChatService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadConversations();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadConversations() {
    this.isLoading = true;
    this.chatService.getConversationList(20)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.conversations = result.conversations || [];
          this.isLoading = false;
          this.changeDetectorRef.detectChanges();
        },
        error: (error) => {
          this.isLoading = false;
          this.changeDetectorRef.detectChanges();
        }
      });
  }

  refreshConversations() {
    this.loadConversations();
  }

  onSelectConversation(conversationId: number) {
    this.conversationSelected.emit(conversationId);
  }

  onNewConversation() {
    this.newConversation.emit();
  }

  getConversationTitle(conversation: ConversationSummary): string {
    return conversation.title || `Conversation ${conversation.id || 'Unknown'}`;
  }

  getConversationPreview(conversation: ConversationSummary): string {
    if (conversation.lastMessage) {
      return conversation.lastMessage.length > 50 
        ? conversation.lastMessage.substring(0, 50) + '...'
        : conversation.lastMessage;
    }
    return `${conversation.messageCount || 0} messages`;
  }

  formatDate(dateString?: string): string {
    if (!dateString) return '';
    
    const date = new Date(dateString);
    const now = new Date();
    const diffTime = Math.abs(now.getTime() - date.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays === 1) {
      return 'Today';
    } else if (diffDays === 2) {
      return 'Yesterday';
    } else if (diffDays <= 7) {
      return `${diffDays - 1} days ago`;
    } else {
      return date.toLocaleDateString();
    }
  }
}