import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-admin-login',
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.scss'],
  standalone: true,
  imports: [ IonicModule, FormsModule, CommonModule ]
})

export class AdminLoginComponent  implements OnInit {
  @Output() loginSuccess = new EventEmitter<void>();

  username = "";
  password = "";

  constructor(private router: Router, private authService: AuthService) { }

  ngOnInit() {}

  public goToMain(): void {
    this.router.navigate(['/main']);
  }

  onLogin() {
    if (this.username === 'admin' && this.password === 'admin') {
      this.authService.login();
      this.loginSuccess.emit();
      this.router.navigate(['/admin']);
    } else {
      alert('Niepoprawny login lub haslo');
    }
  }
}
