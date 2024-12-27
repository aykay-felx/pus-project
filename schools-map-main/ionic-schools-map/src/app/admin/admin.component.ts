import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { AdminLoginComponent } from '../admin-login/admin-login.component';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
  standalone: true,
  imports: [ IonicModule, FormsModule, CommonModule, AdminLoginComponent ]
})
export class AdminComponent  implements OnInit {
  isLoggedIn = false;

  constructor() { }

  ngOnInit() {}

  onLoginSuccess() {
    this.isLoggedIn = true;
  }

}
