import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';

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

  onLogin() {
    if (this.username === 'admin' && this.password === 'admin') {
      this.loginSuccess.emit();
    } else {
      alert('Niepoprawny login lub haslo');
    }
  }

  constructor() { }

  ngOnInit() {}

}
