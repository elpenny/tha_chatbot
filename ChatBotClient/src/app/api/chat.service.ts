import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import type { components } from './types';

export interface SSEMessage {
  data: string;
  type?: string;
}

// Type aliases for convenience
export type ChatMessageRequest = components['schemas']['ChatMessageRequest'];
export type GetConversationHistoryResult = components['schemas']['GetConversationHistoryResult'];
export type UpdateMessageRatingRequest = components['schemas']['UpdateMessageRatingRequest'];

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private readonly baseUrl = 'http://localhost:5051/api/chat';

  constructor(private http: HttpClient) {}

  /**
   * Send a message and receive Server-Sent Events stream
   */
  sendMessage(request: ChatMessageRequest): Observable<SSEMessage> {
    return new Observable<SSEMessage>(observer => {
      const abortController = new AbortController();
      
      fetch(`${this.baseUrl}/message`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'text/event-stream'
        },
        body: JSON.stringify(request),
        signal: abortController.signal
      })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const reader = response.body?.getReader();
        if (!reader) {
          throw new Error('No response body reader available');
        }
        
        const decoder = new TextDecoder();
        let buffer = '';
        
        const readStream = (): void => {
          reader.read().then(({ done, value }) => {
            if (done) {
              observer.complete();
              return;
            }
            
            buffer += decoder.decode(value, { stream: true });
            
            // Process complete SSE messages
            const lines = buffer.split('\n');
            buffer = lines.pop() || ''; // Keep incomplete line in buffer
            
            let currentMessage: { data?: string; event?: string } = {};
            
            for (const line of lines) {
              if (line.trim() === '') {
                // Empty line indicates end of message
                if (currentMessage.data) {
                  observer.next({
                    data: currentMessage.data,
                    type: currentMessage.event
                  });
                }
                currentMessage = {};
              } else if (line.startsWith('data: ')) {
                currentMessage.data = line.substring(6);
              } else if (line.startsWith('event: ')) {
                currentMessage.event = line.substring(7);
              }
            }
            
            readStream();
          }).catch(error => {
            observer.error(error);
          });
        };
        
        readStream();
      })
      .catch(error => {
        observer.error(error);
      });
      
      // Cleanup function
      return () => {
        abortController.abort();
      };
    });
  }

  /**
   * Get conversation history
   */
  getConversationHistory(conversationId: number): Observable<GetConversationHistoryResult> {
    return this.http.get<GetConversationHistoryResult>(`${this.baseUrl}/conversations/${conversationId}`);
  }

  /**
   * Update message rating
   */
  updateMessageRating(messageId: number, request: UpdateMessageRatingRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/messages/${messageId}/rating`, request);
  }
}