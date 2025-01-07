import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

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

  filteredSchools: any[] = [];

  constructor() { }

  ngOnInit(): void {
    this.setupEventListeners();
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
