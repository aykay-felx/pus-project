import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { Router } from '@angular/router';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import * as L from 'leaflet';
import 'leaflet.markercluster';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    HttpClientModule,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]})

export class MainComponent implements OnInit {
  @ViewChild('dropdownButton', { static: false }) dropdownButtonElement!: ElementRef;
  @ViewChild('headerArrow', { static: false }) headerArrowElement!: ElementRef
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

  schools: any[] = [];
  filteredSchools: any[] = [];
  markers: any = L.markerClusterGroup();
  map!: L.Map;

  headers = new HttpHeaders({
    'Content-Type': 'application/json',
    Accept: 'application/json',
  });

  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    this.setupEventListeners();
    this.initializeMap();
  }

  goToAdmin(): void {
    this.router.navigate(['/admin']);
  }

  private initializeMap(): void {
    const markers = L.markerClusterGroup();

    let mapZoom = window.innerWidth > 800 ? 6 : 5;

    const map = L.map('map').setView([51.9194, 19.1451], mapZoom);

    L.tileLayer(
      'https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=RakePZLBXp7DUCKOgD6V',
      {
        tileSize: 512,
        zoomOffset: -1,
        attribution:
          '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> ' +
          '<a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>',
        crossOrigin: true,
      }
    ).addTo(map);

    const marker = L.marker([51.9194, 19.1451]);
    markers.addLayer(marker);
    map.addLayer(markers);
  }


  fetchAllSchools(): void {
    this.http.get('http://localhost:5000/api/rspo/old-schools', { headers: this.headers }).subscribe(
      (data: any) => {
        this.schools = data;
        this.schools.forEach((school: any) => this.createMarker(school));
        this.map.addLayer(this.markers);
      },
      (error) => console.error('Failed to fetch schools:', error)
    );
  }

  createMarker(school: any): void {
    const marker = L.marker([school.latitude, school.longitude])
      .on('click', () => this.handleMarkerClick(school.id))
      .bindPopup(this.createPopup(school.businessData));

    (marker as any).myID = school.id;
    this.markers.addLayer(marker);
  }

  handleMarkerClick(schoolId: string): void {
    this.http.get(`http://localhost:5000/api/rspo/old-schools/${schoolId}`, { headers: this.headers }).subscribe(
      (data: any) => {
        console.log('School details:', data);
      },
      (error: any) => console.error('Failed to fetch school details:', error)
    );
  }

  createPopup(data: any): string {
    return `
      <div class="popup">
        <p class="popup__school-name">${data.nazwa}</p>
        <p class="popup__city">${data.miejscowosc} ${data.ulica || ''}</p>
        <p class="popup__address">${data.kodPocztowy} ${data.poczta}</p>
        <a href="${data.stronaInternetowa}" target="_blank">${data.stronaInternetowa}</a>
        <a href="mailto:${data.email}">${data.email}</a>
      </div>`;
  }

  public setupEventListeners() {
    this.dropdownButtonElement?.nativeElement?.addEventListener('click', () => this.toggleDropdown());
    this.headerArrowElement?.nativeElement?.addEventListener('click', () => this.toggleDropdown());
    this.selectTab1Element?.nativeElement?.addEventListener('click', () => this.activeTab = 'general');
    this.selectTab2Element?.nativeElement?.addEventListener('click', () => this.activeTab = 'detailed');
    this.hamburgerElement?.nativeElement?.addEventListener('click', () => this.toggleSidebar());
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

  // public filterSchools() {
  //   // Implement your filtering logic here
  // }
}
