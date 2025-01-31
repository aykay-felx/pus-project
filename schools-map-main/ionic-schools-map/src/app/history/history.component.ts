import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { CommonModule } from '@angular/common';
import { IonicModule, AlertController } from '@ionic/angular';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms'

export interface SchoolHistory {
  id: number;
  rspoNumer: string;
  changedAt: string;
  changes: string;
}

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.scss'],
  standalone: true,
  imports: [ IonicModule, CommonModule, FormsModule ]
})
export class HistoryComponent  implements OnInit {

  historyList: SchoolHistory[] = [];
  isLoading: boolean = false;
  errorMessage: string = '';
  rspoNumer: string = '';

  private historyUrl = 'http://localhost:5000/api/rspo/history';

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService,
    private alertController: AlertController,
    private http: HttpClient,
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const rspo = params.get('rspoNumer');
      if (rspo) {
        this.rspoNumer = rspo;
        this.fetchHistory();
      }
    });
  }

  fetchHistory(event?: any): void {
    this.isLoading = true;
    const url = `${this.historyUrl}/${this.rspoNumer}`;
    this.http.get<SchoolHistory[]>(url).subscribe({
      next: (data) => {
        this.historyList = data;
        this.isLoading = false;
        if (event) {
          event.target.complete();
        }
      },
      error: (error) => {
        console.error('Błąd podczas pobierania historii:', error);
        this.errorMessage = 'Wystąpił błąd podczas pobierania historii.';
        this.isLoading = false;
        if (event) {
          event.target.complete();
        }
      }
    });
  }

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
