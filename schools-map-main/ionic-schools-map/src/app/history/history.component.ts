import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { CommonModule } from '@angular/common';
import { IonicModule, AlertController } from '@ionic/angular';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.scss'],
  standalone: true,
  imports: [ IonicModule, CommonModule ]
})
export class HistoryComponent  implements OnInit {

  constructor(
    private router: Router,
    private authService: AuthService,
    private alertController: AlertController
  ) { }

  ngOnInit() {}

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  public goToMain(): void {
    this.router.navigate(['/main']);
  }

  public goToAdmin(): void {
    this.router.navigate(['/admin']);
  }

  async logout() {
    const alert = await this.alertController.create({
      header: 'Wylogowanie',
      message: 'Czy na pewno chcesz się wylogować?',
      buttons: [
        {
          text: 'Anuluj',
          role: 'cancel',
        },
        {
          text: 'Wyloguj się',
          handler: () => {
            localStorage.removeItem('authToken');
            this.authService.logout();
            this.router.navigate(['/main']);
          }
        }
      ]
    });
  
    await alert.present();
  }

}
