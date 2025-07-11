import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { ChatComponent } from '../chat/chat.component';

@Component({
  selector: 'app-chat-container',
  standalone: true,
  imports: [
    CommonModule,
    SidebarComponent,
    ChatComponent
  ],
  templateUrl: './chat-container.component.html',
  styleUrls: ['./chat-container.component.scss']
})
export class ChatContainerComponent {
  @ViewChild(SidebarComponent) sidebar!: SidebarComponent;
  @ViewChild(ChatComponent) chat!: ChatComponent;

  selectedConversationId: number | null = null;

  onConversationSelected(conversationId: number) {
    this.selectedConversationId = conversationId;
    this.chat.onConversationSelected(conversationId);
  }

  onNewConversation() {
    this.selectedConversationId = null;
    this.chat.onNewConversation();
  }

  onConversationCreated(conversationId: number) {
    this.selectedConversationId = conversationId;
    // Refresh the sidebar to show the new conversation
    this.sidebar.refreshConversations();
  }

  onConversationUpdated(conversationId: number) {
    // Refresh sidebar when conversation is updated (e.g., new messages)
    this.sidebar.refreshConversations();
  }
}