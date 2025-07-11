export interface ChatMessage {
  content: string;
  isUser: boolean;
  isTyping?: boolean;
  messageId?: number;
  rating?: number | null;
}

export interface SSEMessage {
  data: string;
  type?: string;
}