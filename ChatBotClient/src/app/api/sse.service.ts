import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

export interface SSEMessage {
  data: string;
  type?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SSEService {
  
  /**
   * Parse SSE response from HTTP stream
   */
  parseSSEStream(response: string): Observable<SSEMessage> {
    const subject = new Subject<SSEMessage>();
    
    // Split by double newlines to get individual events
    const events = response.split('\n\n');
    
    for (const event of events) {
      if (!event.trim()) continue;
      
      const lines = event.split('\n');
      const message: SSEMessage = { data: '' };
      
      for (const line of lines) {
        if (line.startsWith('data: ')) {
          message.data = line.substring(6);
        } else if (line.startsWith('event: ')) {
          message.type = line.substring(7);
        }
      }
      
      if (message.data) {
        subject.next(message);
      }
    }
    
    subject.complete();
    return subject.asObservable();
  }

  /**
   * Create EventSource for real-time SSE streaming
   */
  createEventSource(url: string, options?: EventSourceInit): Observable<SSEMessage> {
    return new Observable(observer => {
      const eventSource = new EventSource(url, options);
      
      eventSource.onmessage = event => {
        observer.next({
          data: event.data,
          type: event.type
        });
      };
      
      eventSource.onerror = error => {
        observer.error(error);
      };
      
      // Cleanup function
      return () => {
        eventSource.close();
      };
    });
  }
}