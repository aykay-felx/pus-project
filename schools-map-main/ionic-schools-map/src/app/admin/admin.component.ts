import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { AdminLoginComponent } from '../admin-login/admin-login.component';

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';

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

  constructor(private http: HttpClient) { }

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

}
