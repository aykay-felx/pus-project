import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IonicModule, ModalController, AlertController } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { AdminLoginComponent } from '../admin-login/admin-login.component';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { EditSchoolModalComponent } from '../edit-school-modal/edit-school-modal.component';
import { AuthService } from '../auth.service';
import { RouterModule } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { map } from 'rxjs/operators';
import { FetchSchoolsService } from '../fetch-progress.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
  standalone: true,
  imports: [ IonicModule, FormsModule, CommonModule, AdminLoginComponent, RouterModule ]
})
export class AdminComponent  implements OnInit, OnDestroy {
  progress: number = 0;
  isLoading: boolean = false;
  errorMessage: string = '';
  private fetchSubscription: Subscription | undefined;

  selectAllSchools() {
    this.filteredSchools.forEach(school => school.selected = true);
  }
  
  

  showFilter: boolean = false;
  userName: string = '';
  filters = {
    Nazwa: '',
    Typ: '',
    Miejscowosc: '',
    Wojewodztwo: '',
    Gmina: '',
    Powiat: ''
  };

  saveFilters() {
    console.log(this.filters);
  }

  resetFilters() {
    this.filters = {
      Nazwa: '',
      Typ: '',
      Miejscowosc: '',
      Wojewodztwo: '',
      Gmina: '',
      Powiat: ''
    };
  }

  changesMade: boolean = false;

  private oldSchoolsUrl = 'http://localhost:5000/api/rspo/old-school/old-schools';
  private newSchoolsUrl = 'http://localhost:5000/api/rspo/new-school/new-schools';


  filteredSchools: any[] = [];
  allSelected: boolean = false;
  oldSchools: any[] = [];
  newSchools: any[] = [];

  historyList: any[] = [];
  isLoadingHistory: boolean = false;
  historyErrorMessage: string = '';

  constructor(
    private http: HttpClient, 
    private modalController: ModalController, 
    private router: Router,
    private alertController: AlertController,
    private authService: AuthService,
    private route: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef,
    private cdr: ChangeDetectorRef,
    private fetchSchoolsService: FetchSchoolsService
  ) { }

  ngOnInit() {
    this.loadSchools();
  }

  ngOnDestroy() {
    if (this.fetchSubscription) {
      this.fetchSubscription.unsubscribe();
    }
  }

  toggleSelectAllSchools() {
    this.allSelected = !this.allSelected;
    this.filteredSchools.forEach(school => school.selected = this.allSelected);
  }

  saveSelectedSchools() {
    const selectedSchools = this.filteredSchools.filter(school => school.selected);
    if (selectedSchools.length > 0) {
      this.applyChanges(selectedSchools);
    } else {
      alert('Nie zaznaczono żadnej szkoły.');
    }
  }


  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  onLoginSuccess() {
    this.authService.login();
    this.loadSchools();
  }

  getSchools(): Observable<{ oldSchools: any[], newSchools: any[], filteredSchools: any[]}> {
    return forkJoin({
      oldSchools: this.http.get<any[]>(this.oldSchoolsUrl),
      newSchools: this.http.get<any[]>(this.newSchoolsUrl),
      filteredSchools: this.http.get<any[]>(this.newSchoolsUrl)
    }).pipe(
      map(response => {
        const filteredSchoolsWithSelected = response.filteredSchools.map(school => ({
          ...school,
          selected: false
        }));
  
        return {
          oldSchools: response.oldSchools,
          newSchools: response.newSchools,
          filteredSchools: filteredSchoolsWithSelected
        };
      })
    );
  }

  public async deleteSelectedSchools() {
    const alert = await this.alertController.create({
      header: 'Usuń szkołę',
      message: `Czy na pewno chcesz usunąć wybrane szkoły?`,
      buttons: [
        {
          text: 'Anuluj',
          role: 'cancel',
        },
        {
          text: 'Usuń',
          handler: async () => {
            this.filteredSchools.forEach(school => {
              if (school.selected) {
                this.deleteSchoolWithoutAsking(school.rspoNumer); // Usuwamy pojedynczą szkołę
              }
            });
          }
        }
      ]
    });
    await alert.present();
  }

  public goToMain(): void {
    this.router.navigate(['/main']);
  }

  public showHistory(event: Event, rspoNumer: string): void {
    event.stopPropagation();
    if (rspoNumer) {
      this.router.navigate(['/history', rspoNumer]);
    } else {
      this.alertController.create({
        header: 'Błąd',
        message: 'Numer RSPO szkoły jest nieokreślony.',
        buttons: ['OK']
      }).then(alert => alert.present());
    }
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

  

  toggleFilter() {
    this.showFilter = !this.showFilter;
  }

  inProgress = false;

  updateData() {
    this.inProgress = true;
    this.progress = 0;
    this.cdr.detectChanges();

    const url = 'https://localhost:5001/api/rspo/new-school/new-schools/fetch';
    const eventSource = new EventSource(url);

    eventSource.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log('Received SSE message:', data);
      
      if (data.progress !== undefined) {
        this.progress = data.progress;
        this.cdr.detectChanges();
      }
      
      if (data.message === 'Fetch complete') {
        eventSource.close();
        this.compareData();
        this.cdr.detectChanges();
      }
    };

    eventSource.onerror = (error) => {
      console.error('Error in SSE connection:', error);
      this.inProgress = false;
      eventSource.close();
      this.cdr.detectChanges();
    };
  }
  
  compareData() {
    const url = 'https://localhost:5001/api/rspo/new-school/new-schools/compare';
  
    this.http.get(url).subscribe(
      (response) => {
        console.log('Comparison completed:', response);
        this.inProgress = false;
        this.loadSchools();
      },
      (error) => {
        console.error('Error while calling compare endpoint:', error);
      }
    );
  }


  cancelUpdate() {
    if (this.fetchSubscription) {
      this.fetchSubscription.unsubscribe();
      this.isLoading = false;
      this.progress = 0;
      this.alertController.create({
        header: 'Anulowano',
        message: 'Operacja aktualizacji została anulowana.',
        buttons: ['OK']
      }).then(alert => alert.present());
    }
  }  
  
  public loadSchools() {
    this.getSchools().subscribe(response => {
      this.oldSchools = response.oldSchools.map(school => ({
        ...school,
        isExpanded: false,
        isOldObj: true
      }));

      this.newSchools = response.newSchools.map(school => ({
        ...school,
        isExpanded: false,
        isNewObj: true,
        matchedOldSchool: null
      }));

      this.filteredSchools = response.newSchools.map(school => ({
        ...school,
        isExpanded: false,
        isNewObj: true,
        matchedOldSchool: null
      }));
  
      this.changeDetectorRef.detectChanges();
    });
  }

  isFilled(){
    return this.filters.Miejscowosc == "" && this.filters.Wojewodztwo == "" && this.filters.Gmina == "" && this.filters.Nazwa == "" && this.filters.Powiat == "" && this.filters.Typ == ""
  }

  applyFilters() {
    const queryParams = Object.keys(this.filters)
      .filter(key => this.filters[key as keyof typeof this.filters])
      .map(key => {
        let value = this.filters[key as keyof typeof this.filters];
        
        if (typeof value === 'string' && value.length > 0) {
          value = value.charAt(0).toUpperCase() + value.slice(1);
        }
        
        return `${key}=${value}`;
      })
      .join('&');
    
    console.log(queryParams);
  
    const filterUrl = `http://localhost:5000/api/rspo/new-school/new-schools/filters?${queryParams}`;
    console.log(this.isFilled());
    
    this.http.get<any[]>(filterUrl).subscribe(response => {
      if (this.isFilled()) {
        this.filteredSchools = this.newSchools;
        console.log('Filtrowane szkoły:', response);
      } else {
        this.filteredSchools = response;
        console.log('Filtrowane szkoły:', response);
      }
    });
}

  public async deleteSchoolWithoutAsking(rspoNumer: string): Promise<void> {
    if (!rspoNumer) {
      return;
    }
  
    const deleteUrlNewSchool = `http://localhost:5000/api/rspo/new-school/new-schools/${rspoNumer}`;
    const deleteUrlOldSchool = `http://localhost:5000/api/rspo/old-school/oldschools/${rspoNumer}`;

    try {
      await this.http.delete(deleteUrlNewSchool).toPromise();
      this.newSchools = this.newSchools.filter(school => school.rspoNumer !== rspoNumer);

      await this.alertController.create({
        header: 'Sukces',
        message: 'Szkoła została pomyślnie usunięta.',
        buttons: ['OK'],
      }).then(alert => alert.present());

    } catch (error: any) {
      const errorMessage = error?.error?.message || '';
      if (errorMessage.includes('not found')) {
        try {
          await this.http.delete(deleteUrlOldSchool).toPromise();
          this.oldSchools = this.oldSchools.filter(school => school.rspoNumer !== rspoNumer);

          await this.alertController.create({
            header: 'Sukces',
            message: 'Szkoła została pomyślnie usunięta.',
            buttons: ['OK'],
          }).then(alert => alert.present());

        } catch (oldSchoolError) {
          await this.alertController.create({
            header: 'Błąd',
            message: 'Wystąpił błąd podczas usuwania szkoły.',
            buttons: ['OK'],
          }).then(alert => alert.present());
        }
      }
    }

    this.loadSchools();
  }
  
  
  
  
  public async deleteSchool(event: Event, rspoNumer: string): Promise<void> {
    event.stopPropagation();
    if (!rspoNumer) {
      return;
    }
  
    const alert = await this.alertController.create({
      header: 'Usuń szkołę',
      message: `Czy na pewno chcesz usunąć szkołę o numerze RSPO: ${rspoNumer}?`,
      buttons: [
        {
          text: 'Anuluj',
          role: 'cancel',
        },
        {
          text: 'Usuń',
          handler: async () => {
            const deleteUrlNewSchool = `http://localhost:5000/api/rspo/new-school/new-schools/${rspoNumer}`;
            const deleteUrlOldSchool = `http://localhost:5000/api/rspo/old-school/oldschools/${rspoNumer}`;
  
            try {
              await this.http.delete(deleteUrlNewSchool).toPromise();
              this.newSchools = this.newSchools.filter(school => school.rspoNumer !== rspoNumer);
  
              await this.alertController.create({
                header: 'Sukces',
                message: 'Szkoła została pomyślnie usunięta.',
                buttons: ['OK'],
              }).then(alert => alert.present());
  
            } catch (error: any) {
              const errorMessage = error?.error?.message || '';
              if (errorMessage.includes('not found')) {
                try {
                  await this.http.delete(deleteUrlOldSchool).toPromise();
                  this.oldSchools = this.oldSchools.filter(school => school.rspoNumer !== rspoNumer);
  
                  await this.alertController.create({
                    header: 'Sukces',
                    message: 'Szkoła została pomyślnie usunięta.',
                    buttons: ['OK'],
                  }).then(alert => alert.present());
  
                } catch (oldSchoolError) {
                  await this.alertController.create({
                    header: 'Błąd',
                    message: 'Wystąpił błąd podczas usuwania szkoły.',
                    buttons: ['OK'],
                  }).then(alert => alert.present());
                }
              }
            }
  
            this.loadSchools();
          },
        },
      ],
    });
  
    await alert.present();
  }

  public toggleDetails(school: any) {
    school.isExpanded = !school.isExpanded;
  
    if (school.isExpanded && school.isNewObj) {
      school.matchedOldSchool = this.oldSchools.find(
        oldSchool => oldSchool.rspoNumer === school.rspoNumer
      );
    }

    if (!school.rspoNumer && school.matchedOldSchool) {
      school.rspoNumer = school.matchedOldSchool.rspoNumer;
    }

    console.log('toggleDetails - school.rspoNumer:', school.rspoNumer);
  }

  public compareValues(newVal: any, oldVal: any): string {
    if (newVal === oldVal) {
      return 'green';
    } else if (!newVal || !oldVal) {
      return ''; 
    } else {
      return 'red'; 
    }
  }

  async editSchool(event: Event, school: any) {
    event.stopPropagation();
    const modal = await this.modalController.create({
      component: EditSchoolModalComponent,
      componentProps: { school } 
    });
  
    await modal.present();
  
    const { data } = await modal.onDidDismiss();
    if (data) {
      console.log('Zaktualizowane dane:', data);
      this.loadSchools();
    }
  }
  
  applyChanges(selectedSchools: any[] = this.newSchools) {
    const changedSchools = selectedSchools
      .filter(school => {
        const oldSchool = school.matchedOldSchool = this.oldSchools.find(
          oldSchool => oldSchool.rspoNumer === school.rspoNumer
        );
        if (!oldSchool) {
          console.log('Brak przypisanego starego rekordu dla szkoły:', school.nazwa);
          return false;
        }
  
        console.log('Porównanie szkoły:', school, 'z', oldSchool);
  
        return Object.keys(school).some(key => school[key] !== oldSchool[key]);
      })
      .map(school => {
        const oldSchool = school.matchedOldSchool;
  
        return {
          rspoNumer: school.rspoNumer || '',
          subFieldRspoNumer: {
            isDifferent: school.rspoNumer !== school.matchedOldSchool?.rspoNumer,
            oldValue: school.matchedOldSchool?.rspoNumer || '',
            shouldApply: school.rspoNumer !== school.matchedOldSchool?.rspoNumer,
            isManual: true
          },
          
          longitude: school.longitude || 0,
          subFieldLongitude: {
            isDifferent: school.longitude !== school.matchedOldSchool?.longitude,
            oldValue: String(oldSchool?.longitude || '0'),
            shouldApply: school.longitude !== school.matchedOldSchool?.longitude,
            isManual: true
          },
  
          latitude: school.latitude || 0,
          subFieldLatitude: {
            isDifferent: school.latitude !== school.matchedOldSchool?.latitude,
            oldValue: String(oldSchool?.latitude || '0'),
            shouldApply: school.latitude !== school.matchedOldSchool?.latitude,
            isManual: true
          },
  
          typ: school.typ || '',
          subFieldTyp: {
            isDifferent: school.typ !== school.matchedOldSchool?.typ,
            oldValue: school.matchedOldSchool?.typ || '',
            shouldApply: school.typ !== school.matchedOldSchool?.typ,
            isManual: true
          },
  
          nazwa: school.nazwa || '',
          subFieldNazwa: {
            isDifferent: school.nazwa !== school.matchedOldSchool?.nazwa,
            oldValue: school.matchedOldSchool?.nazwa || '',
            shouldApply: school.nazwa !== school.matchedOldSchool?.nazwa,
            isManual: true
          },
  
          miejscowosc: school.miejscowosc || '',
          subFieldMiejscowosc: {
            isDifferent: school.miejscowosc !== school.matchedOldSchool?.miejscowosc,
            oldValue: school.matchedOldSchool?.miejscowosc || '',
            shouldApply: school.miejscowosc !== school.matchedOldSchool?.miejscowosc,
            isManual: true
          },
  
          wojewodztwo: school.wojewodztwo || '',
          subFieldWojewodztwo: {
            isDifferent: school.wojewodztwo !== school.matchedOldSchool?.wojewodztwo,
            oldValue: school.matchedOldSchool?.wojewodztwo || '',
            shouldApply: school.wojewodztwo !== school.matchedOldSchool?.wojewodztwo,
            isManual: true
          },
  
          kodPocztowy: school.kodPocztowy || '',
          subFieldKodPocztowy: {
            isDifferent: school.kodPocztowy !== school.matchedOldSchool?.kodPocztowy,
            oldValue: school.matchedOldSchool?.kodPocztowy || '',
            shouldApply: school.kodPocztowy !== school.matchedOldSchool?.kodPocztowy,
            isManual: true
          },
  
          numerBudynku: school.numerBudynku || '',
          subFieldNumerBudynku: {
            isDifferent: school.numerBudynku !== school.matchedOldSchool?.numerBudynku,
            oldValue: school.matchedOldSchool?.numerBudynku || '',
            shouldApply: school.numerBudynku !== school.matchedOldSchool?.numerBudynku,
            isManual: true
          },
  
          email: school.email || '',
          subFieldEmail: {
            isDifferent: school.email !== school.matchedOldSchool?.email,
            oldValue: school.matchedOldSchool?.email || '',
            shouldApply: school.email !== school.matchedOldSchool?.email,
            isManual: true
          },
  
          ulica: school.ulica || '',
          subFieldUlica: {
            isDifferent: school.ulica !== school.matchedOldSchool?.ulica,
            oldValue: school.matchedOldSchool?.ulica || '',
            shouldApply: school.ulica !== school.matchedOldSchool?.ulica,
            isManual: true
          },
  
          telefon: school.telefon || '',
          subFieldTelefon: {
            isDifferent: school.telefon !== school.matchedOldSchool?.telefon,
            oldValue: school.matchedOldSchool?.telefon || '',
            shouldApply: school.telefon !== school.matchedOldSchool?.telefon,
            isManual: true
          },
  
          statusPublicznosc: school.statusPublicznosc || '',
          subFieldStatusPublicznosc: {
            isDifferent: school.statusPublicznosc !== school.matchedOldSchool?.statusPublicznosc,
            oldValue: school.matchedOldSchool?.statusPublicznosc || '',
            shouldApply: school.statusPublicznosc !== school.matchedOldSchool?.statusPublicznosc,
            isManual: true
          },
  
          stronaInternetowa: school.stronaInternetowa || '',
          subFieldStronaInternetowa: {
            isDifferent: school.stronaInternetowa !== school.matchedOldSchool?.stronaInternetowa,
            oldValue: school.matchedOldSchool?.stronaInternetowa || '',
            shouldApply: school.stronaInternetowa !== school.matchedOldSchool?.stronaInternetowa,
            isManual: true
          },
  
          dyrektor: school.dyrektor || '',
          subFieldDyrektor: {
            isDifferent: school.dyrektor !== school.matchedOldSchool?.dyrektor,
            oldValue: school.matchedOldSchool?.dyrektor || '',
            shouldApply: school.dyrektor !== school.matchedOldSchool?.dyrektor,
            isManual: true
          },
  
          nipPodmiotu: school.nipPodmiotu || '',
          subFieldNipPodmiotu: {
            isDifferent: school.nipPodmiotu !== school.matchedOldSchool?.nipPodmiotu,
            oldValue: school.matchedOldSchool?.nipPodmiotu || '',
            shouldApply: school.nipPodmiotu !== school.matchedOldSchool?.nipPodmiotu,
            isManual: true
          },
  
          regonPodmiotu: school.regonPodmiotu || '',
          subFieldRegonPodmiotu: {
            isDifferent: school.regonPodmiotu !== school.matchedOldSchool?.regonPodmiotu,
            oldValue: school.matchedOldSchool?.regonPodmiotu || '',
            shouldApply: school.regonPodmiotu !== school.matchedOldSchool?.regonPodmiotu,
            isManual: true
          },
  
          dataZalozenia: school.dataZalozenia || '',
          subFieldDataZalozenia: {
            isDifferent: school.dataZalozenia !== school.matchedOldSchool?.dataZalozenia,
            oldValue: school.matchedOldSchool?.dataZalozenia || '',
            shouldApply: school.dataZalozenia !== school.matchedOldSchool?.dataZalozenia,
            isManual: true
          },
  
          liczbaUczniow: school.liczbaUczniow || 0,
          subFieldLiczbaUczniow: {
            isDifferent: school.liczbaUczniow !== school.matchedOldSchool?.liczbaUczniow,
            oldValue: String(oldSchool?.liczbaUczniow || '0'),
            shouldApply: school.liczbaUczniow !== school.matchedOldSchool?.liczbaUczniow,
            isManual: true
          },
  
          kategoriaUczniow: school.kategoriaUczniow || '',
          subFieldKategoriaUczniow: {
            isDifferent: school.kategoriaUczniow !== school.matchedOldSchool?.kategoriaUczniow,
            oldValue: school.matchedOldSchool?.kategoriaUczniow || '',
            shouldApply: school.kategoriaUczniow !== school.matchedOldSchool?.kategoriaUczniow,
            isManual: true
          },
  
          specyfikaPlacowki: school.specyfikaPlacowki || '',
          subFieldSpecyfikaPlacowki: {
            isDifferent: school.specyfikaPlacowki !== school.matchedOldSchool?.specyfikaPlacowki,
            oldValue: school.matchedOldSchool?.specyfikaPlacowki || '',
            shouldApply: school.specyfikaPlacowki !== school.matchedOldSchool?.specyfikaPlacowki,
            isManual: true
          },
  
          gmina: school.gmina || '',
          subFieldGmina: {
            isDifferent: school.gmina !== school.matchedOldSchool?.gmina,
            oldValue: school.matchedOldSchool?.gmina || '',
            shouldApply: school.gmina !== school.matchedOldSchool?.gmina,
            isManual: true
          },
  
          powiat: school.powiat || '',
          subFieldPowiat: {
            isDifferent: school.powiat !== school.matchedOldSchool?.powiat,
            oldValue: school.matchedOldSchool?.powiat || '',
            shouldApply: school.powiat !== school.matchedOldSchool?.powiat,
            isManual: true
          },
  
          jezykiNauczane: school.jezykiNauczane || '',
          subFieldJezykiNauczane: {
            isDifferent: school.jezykiNauczane !== school.matchedOldSchool?.jezykiNauczane,
            oldValue: school.matchedOldSchool?.jezykiNauczane ? school.matchedOldSchool.jezykiNauczane.join(', ') : '',
            shouldApply: school.jezykiNauczane !== school.matchedOldSchool?.jezykiNauczane,
            isManual: false
          },
  
          isDifferentObj: true,
          isNewObj: school.isNewObj || false
        };
      });
  
    console.log('Znalezione zmienione szkoły (API format):', changedSchools);
      
    if (changedSchools.length === 0) {
      console.log('Brak zmian do zatwierdzenia.');
      return;
    }

    const url = 'https://localhost:5001/api/RSPO/old-schools/apply-changes';
    this.http.post(url, changedSchools, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      responseType: 'text'
    }).subscribe(
      response => {
        console.log('Zmiany zostały pomyślnie zatwierdzone:', response);
        alert(response);
      },
      error => {
        console.error('Błąd podczas zatwierdzania zmian:', error);
        alert('Wystąpił błąd podczas zatwierdzania zmian.');
      }
    );
  }
}
