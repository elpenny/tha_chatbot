import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import type { components } from './types';

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
  sendMessage(request: ChatMessageRequest): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'text/event-stream'
    });

    return this.http.post(`${this.baseUrl}/message`, request, {
      headers,
      responseType: 'text',
      observe: 'body'
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