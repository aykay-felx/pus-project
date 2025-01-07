import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]})

export class MainComponent implements OnInit {
  @ViewChild('dropdownButton', { static: false }) dropdownButtonElement!: ElementRef;
  @ViewChild('headerArrow', { static: false }) headerArrowElement!: ElementRef;
  @ViewChild('selectTab1', { static: false }) selectTab1Element!: ElementRef;
  @ViewChild('selectTab2', { static: false }) selectTab2Element!: ElementRef;
  @ViewChild('hamburger', { static: false }) hamburgerElement!: ElementRef;
  @ViewChild('sidebar', { static: false }) sidebarElement!: ElementRef;
  @ViewChild('voivodeship', { static: false }) voivodeshipElement!: ElementRef;
  @ViewChild('nameInput', { static: false }) nameInputElement!: ElementRef;

  showDropdown: boolean = false;
  selectedVoivodeship: string = 'all-schools';
  activeTab: string = 'general';
  selectedSchoolTypes: { [key: string]: boolean } = {
    podstawowa: true,
    liceum: true,
    technikum: true,
  };
  debounceTimer: any;

  voivodeships: { value: string, label: string }[] = [
  { value: 'all-schools', label: 'Cała polska' },
  { value: 'dolnośląskie', label: 'Dolnośląskie' },
  { value: 'kujawsko-pomorskie', label: 'Kujawsko-pomorskie' },
  { value: 'lubelskie', label: 'Lubelskie' },
  { value: 'lubuskie', label: 'Lubuskie' },
  { value: 'łódzkie', label: 'Łódzkie' },
  { value: 'małopolskie', label: 'Małopolskie' },
  { value: 'mazowieckie', label: 'Mazowieckie' },
  { value: 'opolskie', label: 'Opolskie' },
  { value: 'podkarpackie', label: 'Podkarpackie' },
  { value: 'podlaskie', label: 'Podlaskie' },
  { value: 'pomorskie', label: 'Pomorskie' },
  { value: 'śląskie', label: 'Śląskie' },
  { value: 'świętokrzyskie', label: 'Świętokrzyskie' },
  { value: 'warmińsko-mazurskie', label: 'Warmińsko-mazurskie' },
  { value: 'wielkopolskie', label: 'Wielkopolskie' },
  { value: 'zachodniopomorskie', label: 'Zachodniopomorskie' }
];

  schoolTypes: { value: string, label: string }[] = [
    { value: 'Podstawowa', label: 'Podstawowa' },
    { value: 'Liceum', label: 'Liceum' },
    { value: 'Technikum', label: 'Technikum' }
  ];

  private oldSchoolsUrl = 'http://localhost:5000/api/rspo/old-schools';
  private newSchoolsUrl = 'http://localhost:5000/api/rspo/new-schools';

  filteredSchools: any[] = [];
  oldSchools: any[] = [];
  newSchools: any[] = [];
  schools: any[] = [];

  getSchools(): Observable<{ oldSchools: any[], newSchools: any[] }> {
    return forkJoin({
      oldSchools: this.http.get<any[]>(this.oldSchoolsUrl),
      newSchools: this.http.get<any[]>(this.newSchoolsUrl)
    });
  }

  constructor(private http: HttpClient) { }
  
  ngOnInit(): void {
    this.setupEventListeners();
    this.loadSchools();
  }

  public setupEventListeners() {
    this.dropdownButtonElement?.nativeElement?.addEventListener('click', () => this.toggleDropdown());
    this.headerArrowElement?.nativeElement?.addEventListener('click', () => this.toggleDropdown());
    this.selectTab1Element?.nativeElement?.addEventListener('click', () => this.activeTab = 'general');
    this.selectTab2Element?.nativeElement?.addEventListener('click', () => this.activeTab = 'detailed');
    this.hamburgerElement?.nativeElement?.addEventListener('click', () => this.toggleSidebar());
    // Uncomment and adjust if you need these functionalities:
    // this.voivodeshipElement?.nativeElement?.addEventListener('change', () => this.filterSchools());
    // this.nameInputElement?.nativeElement?.addEventListener('input', () => {
    //   clearTimeout(this.debounceTimer);
    //   this.debounceTimer = setTimeout(() => this.filterSchools(), this.nameInputElement?.nativeElement?.value.length < 2 ? 700 : 100);
    // });
  }

  public toggleDropdown() {
    this.showDropdown = !this.showDropdown;
    this.headerArrowElement.nativeElement.classList.toggle('arrow--up');
    this.headerArrowElement.nativeElement.classList.toggle('arrow--down');
  }

  public toggleSidebar() {
    this.sidebarElement.nativeElement.classList.toggle('sidebar--hide');
    this.hamburgerElement.nativeElement.classList.toggle('change-bar1');
    this.hamburgerElement.nativeElement.classList.toggle('change-bar2');
    this.hamburgerElement.nativeElement.classList.toggle('change-bar3');
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

  public filterSchools() {
    console.log('xd');
  }

  public filterSchoolsEvent(event: CustomEvent) {
    console.log('xd', event.detail.value);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.dropdownButtonElement?.nativeElement?.contains(event.target) && !this.headerArrowElement?.nativeElement?.contains(event.target)) {
      this.showDropdown = false;
      this.headerArrowElement.nativeElement.classList.remove('arrow--up');
      this.headerArrowElement.nativeElement.classList.add('arrow--down');
    }
  }

  // If you need this method, uncomment and implement:
  // public filterSchools() {
  //   // Implement your filtering logic here
  // }
}
