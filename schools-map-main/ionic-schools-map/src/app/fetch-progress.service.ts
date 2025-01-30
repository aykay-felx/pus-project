import { Injectable, NgZone } from '@angular/core';
import { Observable } from 'rxjs';

interface ProgressMessage {
  page?: number;
  progress?: number;
  message?: string;
  error?: string;
}

@Injectable({
  providedIn: 'root',
})
export class FetchSchoolsService {
  constructor(private ngZone: NgZone) {}

  fetchSchools(): Observable<ProgressMessage> {
    return new Observable<ProgressMessage>((observer) => {
      const eventSource = new EventSource('http://localhost:5000/api/rspo/new-school/new-schools/fetch');

      eventSource.onmessage = (event) => {
        this.ngZone.run(() => {
          try {
            const data: ProgressMessage = JSON.parse(event.data);
            if (data.progress !== undefined) {
              observer.next(data);
            }
            if (data.message) {
              observer.complete();
            }
            if (data.error) {
              observer.error(data.error);
            }
          } catch (err) {
            observer.error('Błąd parsowania danych SSE');
          }
        });
      };

      eventSource.onerror = (error) => {
        this.ngZone.run(() => {
          observer.error('Błąd połączenia SSE');
          eventSource.close();
        });
      };

      return () => {
        eventSource.close();
      };
    });
  }
}
