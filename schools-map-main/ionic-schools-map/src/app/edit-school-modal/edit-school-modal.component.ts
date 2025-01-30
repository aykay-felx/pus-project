
import { ModalController } from '@ionic/angular';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-edit-school-modal',
  templateUrl: './edit-school-modal.component.html',
  styleUrls: ['./edit-school-modal.component.scss'],
  standalone: true,
  imports: [IonicModule, FormsModule, CommonModule]
})
export class EditSchoolModalComponent implements OnInit {
  @Input() school: any;

  constructor(
    private http: HttpClient, 
    private modalController: ModalController, 
  ) { }

  ngOnInit(): void {
    // Możesz dodać inicjalizację tutaj, jeśli jest potrzebna
    //console.log('Passed school: ', this.school);
    //console.log('OldSchool: ', this.school.matchedOldSchool);
  }

  public saveChanges() {
        const school = this.school;
        const oldSchool = this.school.matchedOldSchool;
        // Map fields to match the expected Swagger API structure
        const updatedSchool =  [{
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
            shouldApply: true,//school.dyrektor !== school.matchedOldSchool?.dyrektor,
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
  
          jezykiNauczane: school.jezykiNauczane || [],
          subFieldJezykiNauczane: {
            isDifferent: school.jezykiNauczane !== school.matchedOldSchool?.jezykiNauczane,
            oldValue: school.matchedOldSchool?.jezykiNauczane ? school.matchedOldSchool.jezykiNauczane.join(', ') : '',
            shouldApply: school.jezykiNauczane !== school.matchedOldSchool?.jezykiNauczane,
            isManual: false
          },
  
          isDifferentObj: true,
          isNewObj: school.isNewObj || false
        }];

    const url = 'http://localhost:5000/api/RSPO/old-school/old-schools/apply-changes';
    console.log('Payload being sent:', JSON.stringify(updatedSchool, null, 2));
    this.http.post(url, updatedSchool, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      responseType: 'text'  // Set responseType to 'text'
    }).subscribe(
      response => {
        console.log('Zmiany zostały pomyślnie zatwierdzone:', response);
        alert(response);  // Alert the text response
      },
      error => {
        console.error('Błąd podczas zatwierdzania zmian:', error);
        alert('Wystąpił błąd podczas zatwierdzania zmian.');
      }
    );
    this.modalController.dismiss(this.school);
  }
  

  close() {
    // Zamknij modal bez zapisywania
    this.modalController.dismiss();
  }
}

