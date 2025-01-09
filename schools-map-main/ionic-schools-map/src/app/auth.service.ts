import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedInSubject = new BehaviorSubject<boolean>(this.hasToken());

  constructor() { }

  private hasToken(): boolean {
    return !!localStorage.getItem('authToken');
  }

  login() {
    localStorage.setItem('authToken', 'your-jwt-token');
    this.loggedInSubject.next(true);
  }

  logout() {
    localStorage.removeItem('authToken');
    this.loggedInSubject.next(false);
  }

  isLoggedIn(): boolean {
    return this.loggedInSubject.value;
  }

  getLoggedInStatus() {
    return this.loggedInSubject.asObservable();
  }
}
