import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { AdminLoginComponent } from '../admin-login/admin-login.component';

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable, forkJoin } from 'rxjs';

import { ModalController } from '@ionic/angular';
import { EditSchoolModalComponent } from '../edit-school-modal/edit-school-modal.component'; // Zaimportuj komponent modalny



@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
  standalone: true,
  imports: [ IonicModule, FormsModule, CommonModule, AdminLoginComponent ]
})
export class AdminComponent  implements OnInit {
  
  isLoggedIn = false;

  private oldSchoolsUrl = 'http://localhost:5000/api/rspo/old-schools';
  private newSchoolsUrl = 'http://localhost:5000/api/rspo/new-schools';

  filteredSchools: any[] = [];
  oldSchools: any[] = [];
  newSchools: any[] = [];
  
  constructor(private http: HttpClient, private modalController: ModalController) { }

  ngOnInit() {}

  onLoginSuccess() {
    this.isLoggedIn = true;
    this.loadSchools();
  }

    getSchools(): Observable<{ oldSchools: any[], newSchools: any[] }> {
      return forkJoin({
        oldSchools: this.http.get<any[]>(this.oldSchoolsUrl),
        newSchools: this.http.get<any[]>(this.newSchoolsUrl)
      });
    }

  public loadSchools() {
    this.getSchools().subscribe(response => {
      // Process oldSchools
      this.oldSchools = response.oldSchools.map(school => ({
        ...school,
        isExpanded: false,
        isOldObj: true // Mark explicitly
      }));
      
      // Process newSchools
      this.newSchools = response.newSchools.map(school => ({
        ...school,
        isExpanded: false,
        isNewObj: true, // Mark explicitly
        matchedOldSchool: null // Prepare for comparison
      }));
    });
  }
  

  public toggleDetails(school: any) {
    school.isExpanded = !school.isExpanded;
  
    if (school.isExpanded && school.isNewObj) {
      // Find a matching old school
      school.matchedOldSchool = this.oldSchools.find(
        oldSchool => oldSchool.nazwa === school.nazwa
      );
    }
  }

  public compareValues(newVal: any, oldVal: any): string {
    if (newVal === oldVal) {
      return 'green';  // Same value
    } else if (!newVal || !oldVal) {
      return '';  // If one of the values is 'N/A' or empty, no highlight
    } else {
      return 'red';  // Different values
    }
  }

  async editSchool(school: any) {
    const modal = await this.modalController.create({
      component: EditSchoolModalComponent,
      componentProps: { school } // Przekazanie danych szkoły do modala
    });
  
    await modal.present();
  
    const { data } = await modal.onDidDismiss(); // Obsługa po zamknięciu
    if (data) {
      console.log('Zaktualizowane dane:', data);
      // Zaktualizuj dane szkoły w widoku
    }
  }
  
  applyChanges() {
    const changedSchools = this.newSchools.filter(school => {
      const oldSchool = school.matchedOldSchool;
      if (!oldSchool) {
        console.log('Brak przypisanego starego rekordu dla szkoły:', school.nazwa);
        return false;
      }
  
      console.log('Porównanie szkoły:', school, 'z', oldSchool);
  
      // Porównanie wszystkich pól
      return Object.keys(school).some(key => school[key] !== oldSchool[key]);
    });
  
    console.log('Znalezione zmienione szkoły:', changedSchools);
  
    if (changedSchools.length === 0) {
      console.log('Brak zmian do zatwierdzenia.');
      return;
    }
  
    const url = 'https://localhost:5001/api/RSPO/old-schools/apply-changes';
  
    this.http.post(url, changedSchools, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    }).subscribe(
      response => {
        console.log('Zmiany zostały pomyślnie zatwierdzone:', response);
        alert('Zmiany zostały zatwierdzone!');
      },
      error => {
        console.error('Błąd podczas zatwierdzania zmian:', error);
        alert('Wystąpił błąd podczas zatwierdzania zmian.');
      }
    );
  }
  
}
