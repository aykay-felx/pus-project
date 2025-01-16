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
    this.fetchAllSchools();
    this.initializeMap();
  }

  public goToAdmin(): void {
    this.router.navigate(['/admin']);
  }

  private initializeMap(): void {
    this.markers = L.markerClusterGroup();

    const mapZoom = window.innerWidth > 800 ? 6 : 5;

    this.map = L.map('map').setView([51.9194, 19.1451], mapZoom);

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
    ).addTo(this.map);

    const testMarker = L.marker([51.9194, 19.1451]);
    this.markers.addLayer(testMarker);
    this.map.addLayer(this.markers);
  }

  fetchAllSchools(): void {
    this.http.get('http://localhost:5000/api/rspo/new-school/new-schools', { headers: this.headers }).subscribe(
      (data: any) => {
        this.schools = data.map((school: any) => ({
          ...school
        }));
        // Iterate through the schools and create markers for each
        this.schools.forEach((school: any, index: number) => {
          if (!school) {
            console.error(`School at index ${index} is undefined.`);
            return;
          }
          this.createMarker(school);
        });
        // Add the markers to the map
        this.map.addLayer(this.markers);
      },
      (error) => console.error('Failed to fetch schools:', error)
    );
  }
  
  createMarker(school: any): void {
    if (!school || !school.latitude || !school.longitude) {
      console.error('Invalid school data for marker creation:', school);
      return;
    }
  
    const marker = L.marker([school.latitude, school.longitude])
      .on('click', () => this.handleMarkerClick(school.rspoNumer))
      .bindPopup(this.createPopup(school));
  
    (marker as any).myID = school.rspoNumer;
    this.markers.addLayer(marker);
  }
  

  handleMarkerClick(schoolId: string): void {
    // this.http.get(`http://localhost:5000/api/rspo/old-schools/${schoolId}`, { headers: this.headers }).subscribe(
    //   (data: any) => {
    //     console.log('School details:', data);
    //   },
    //   (error: any) => console.error('Failed to fetch school details:', error)
    // );
    const school = this.schools.find((s: any) => s.rspoNumer === schoolId);

    if (school) {
      //console.log('School details:', school);
    } else {
      console.error('School not found for ID:', schoolId);
    }
  }

  createPopup(data: any) {
    const nazwa = data.nazwa || 'Brak nazwy';  // Fallback if 'nazwa' is missing
    const miejscowosc = data.miejscowosc || 'Brak miejscowości';  // Fallback if 'miejscowosc' is missing
    const ulica = data.ulica ? data.ulica : 'Brak ulicy';  // Fallback to empty string if 'ulica' is missing
    const stronaInternetowa = data.stronaInternetowa || 'Brak strony internetowej';  // Fallback link if 'stronaInternetowa' is missing
    const email = data.email || 'Brak adresu e-mail';  // Fallback if 'email' is missing
  
    return `
      <div class="popup">
        <p class="popup__school-name">${nazwa}</p>
        <p class="popup__city">${miejscowosc} ${ulica}</p>
        <a href="${stronaInternetowa}" target="_blank">${stronaInternetowa}</a>
        <a href="mailto:${email}">${email}</a>
      </div>
    `;
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
