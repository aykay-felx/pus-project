import { Component, OnInit, ElementRef, ViewChild, HostListener } from '@angular/core';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonMenu, IonSelect, IonCheckbox, IonSearchbar } from '@ionic/angular/standalone';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { LeafletMouseEvent } from 'leaflet';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
  standalone: true,
  imports: [IonHeader, IonToolbar, IonTitle, IonContent, IonMenu, IonSelect, IonCheckbox, IonSearchbar, CommonModule, IonicModule, FormsModule],
})
export class HomePage implements OnInit {
  @ViewChild('map') mapContainer: ElementRef;
  @ViewChild('addressInfoContainer') addressInfoContainer: ElementRef;
  @ViewChild('detailedInfoContainer') detailedInfoContainer: ElementRef;
  @ViewChild('voivodeship') voivodeshipElement: ElementRef;
  @ViewChild('nameInput') nameInputElement: ElementRef;
  @ViewChild('selectTab1') selectTab1Element: ElementRef;
  @ViewChild('selectTab2') selectTab2Element: ElementRef;
  @ViewChild('dropdownButton') dropdownButtonElement: ElementRef;
  @ViewChild('headerArrow') headerArrowElement: ElementRef;
  @ViewChild('hamburger') hamburgerElement: ElementRef;
  @ViewChild('sidebar') sidebarElement: ElementRef;

  showDropdown = false;
  activeTab = 'general';
  selectedVoivodeship = 'all-schools';
  selectedSchoolTypes = {
    podstawowa: true,
    liceum: true,
    technikum: true
  };

  private headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Credentials': 'true',
  });

  private map: L.Map;
  private markers = L.markerClusterGroup();
  private mapZoom: number;
  private schools: any[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.initializeMap();
    this.fetchAllSchools();
    this.setupEventListeners();
  }

  private initializeMap() {
    this.mapZoom = window.innerWidth > 800 ? 6 : 5;
    this.map = L.map(this.mapContainer.nativeElement).setView([51.9194, 19.1451], this.mapZoom);

    L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=RakePZLBXp7DUCKOgD6V', {
      tileSize: 512,
      zoomOffset: -1,
      attribution: '\u003ca href="https://www.maptiler.com/copyright/" target="_blank"\u003e\u0026copy; MapTiler\u003c/a\u003e \u003ca href="https://www.openstreetmap.org/copyright" target="_blank"\u003e\u0026copy; OpenStreetMap contributors\u003c/a\u003e',
      crossOrigin: true,
    }).addTo(this.map);
  }

  private fetchAllSchools() {
    this.http.get<any[]>('https://localhost:5001/Schools', { headers: this.headers }).subscribe(
      data => {
        this.schools = data;
        this.schools.forEach(school => this.createMarker(school));
        this.map.addLayer(this.markers);
      }
    );
  }

  private createMarker(school: any) {
    const marker = L.marker([school.latitude, school.longitude]).on('click', (e) => this.handleMarkerClick(e));
    marker.bindPopup(this.createPopup(school.businessData));
    marker['myID'] = school.id;
    this.markers.addLayer(marker);
  }

  private createPopup(data: any): string {
    const schoolPopup = `
      <div class="popup">
        <p class="popup__school-name">${data.nazwa}</p>
        <p class="popup__data popup__city">
        ${
          data.miejscowosc === data.poczta && data.ulica !== null
            ? ""
            : data.miejscowosc
        } ${data.ulica !== null ? "" : data.numerBudynku}</p>
        <p class="popup__data popup__address">${
          data.ulica !== null ? data.ulica + " " + data.numerBudynku : ""
        } ${data.numerLokalu !== null ? " lok. " + data.numerLokalu : ""}</p>
        <p class="popup__data popup__city">${data.kodPocztowy} ${data.poczta}</p>
        <a class="popup__data popup__website" target="_blank" href="${
          data.stronaInternetowa
        }" >${data.stronaInternetowa !== null ? data.stronaInternetowa : ""}</a>
        <a class="popup__data popup__website" href="mailto:${data.email}" >${data.email !== null ? data.email : ""}</a>
      </div>
      `;
    return schoolPopup;
  }

  private handleMarkerClick(e: LeafletMouseEvent) {
    const targetID = e.target['myID'];
    this.http.get<any>(`https://localhost:5001/Schools/${targetID}`, { headers: this.headers }).subscribe(
      data => {
        const schoolData = data.businessData;
        const schoolObj = this.translateDetailsNaming(schoolData);
        if (window.innerWidth >= 1366) {
          this.toggleSidebar();
        }
        this.createTabs(schoolObj);
      }
    );
  }

  private translateDetailsNaming(schoolData: any): any {
    return {
      addressInfo: {
        "Nazwa szkoły": schoolData["nazwa"],
        Miejscowość: schoolData["miejscowosc"],
        "Kod pocztowy": schoolData["kodPocztowy"],
        Poczta: schoolData["poczta"],
        Województwo: schoolData["wojewodztwo"],
        Powiat: schoolData["powiat"],
        Gmina: schoolData["gmina"],
        Ulica: schoolData["ulica"],
        "Numer budynku": schoolData["numerBudynku"],
        "Numer lokalu": schoolData["numerLokalu"],
        Dyrektor: schoolData["dyrektor"],
        Telefon: schoolData["telefon"],
        Faks: schoolData["faks"],
        "Adres e-mail": schoolData["email"],
        "Strona WWW": schoolData["stronaInternetowa"],
        "Typ szkoły": schoolData["typ"],
        Przeznaczenie: schoolData["kategoriaUczniow"],
        "Status publiczności": schoolData["statusPublicznosc"],
        "Liczba uczniów": schoolData["liczbaUczniow"],
        "Nauczane języki": schoolData["jezykiNauczane"],
        "Tereny sportowe": schoolData["terenySportowe"],
      },
      detailedInfo: {
        "Struktura miejsca": schoolData["strukturaMiejsce"],
        "Rodzaj miejscowości": schoolData["rodzajMiejscowosci"],
        "Specyfika placówki": schoolData["specyfikaPlacowki"],
        "Numer RSPO": schoolData["rspoNumer"],
        Regon: schoolData["regonPodmiotu"],
        "NIP podmiotu": schoolData["nipPodmiotu"],
        "Miejsce w strukturze": schoolData["strukturaMiejsce"],
        "Data założenia": schoolData["dataRozpoczeciaDzialalnosci"],
        "Data likwidacji": schoolData["dataLikwidacji"],
        "Kod terytorialny (Miejscowość)": schoolData["kodTerytorialnyMiejscowosc"],
        "Kod terytorialny (Gmina)": schoolData["kodTerytorialnyGmina"],
        "Kod terytorialny (Powiat)": schoolData["kodTerytorialnyPowiat"],
        "Kod terytorialny (Województwo)": schoolData["kodTerytorialnyWojewodztwo"],
        "Podmiot nadrzędny (Nazwa)": schoolData["podmiotNadrzednyNazwa"],
        "Podmiot nadrzędny (Typ)": schoolData["podmiotNadrzednyTyp"],
        "Podmiot nadrzędny (RSPO)": schoolData["podmiotNadrzednyRspo"],
        "Organ prowadzący (Nazwa)": schoolData["organProwadzacyNazwa"],
        "Organ prowadzący (NIP)": schoolData["organProwadzacyNip"],
        "Organ prowadzący (Regon)": schoolData["organProwadzacyRegon"],
        "Organ prowadzący (Typ)": schoolData["organProwadzacyTyp"],
        "Organ prowadzący (Gmina)": schoolData["organProwadzacyGmina"],
        "Organ prowadzący (Powiat)": schoolData["organProwadzacyPowiat"],
        "Organ prowadzący (Województwo)": schoolData["organProwadzacyWojewodztwo"],
      },
    };
  }

  private createTabs(schoolObj: any) {
    this.deleteChildNodes(this.addressInfoContainer.nativeElement);
    this.deleteChildNodes(this.detailedInfoContainer.nativeElement);

    ['addressInfo', 'detailedInfo'].forEach((category, index) => {
      Object.keys(schoolObj[category]).forEach((key) => {
        const value = schoolObj[category][key] || '';

        const itemContainer = document.createElement("div");
        itemContainer.classList.add("sidebar__info-item");

        const itemKey = document.createElement("p");
        itemKey.classList.add("sidebar__item-key");
        itemKey.innerText = key + ": ";

        const itemValue = document.createElement("p");
        itemValue.classList.add("sidebar__item-value");

        let text = typeof value === "object" ? value.join(", ") : value;
        itemValue.innerText = text;

        itemContainer.appendChild(itemKey);
        itemContainer.appendChild(itemValue);

        if (index === 0) {
          this.addressInfoContainer.nativeElement.appendChild(itemContainer);
        } else {
          this.detailedInfoContainer.nativeElement.appendChild(itemContainer);
        }
      });
    });
  }

  private setupEventListeners() {
    this.dropdownButtonElement.nativeElement.addEventListener('click', () => this.toggleDropdown());
    this.headerArrowElement.nativeElement.addEventListener('click', () => this.toggleDropdown());
    this.selectTab1Element.nativeElement.addEventListener('click', () => this.activeTab = 'general');
    this.selectTab2Element.nativeElement.addEventListener('click', () => this.activeTab = 'detailed');
    this.hamburgerElement.nativeElement.addEventListener('click', () => this.toggleSidebar());
    this.voivodeshipElement.nativeElement.addEventListener('change', () => this.filterSchools());
    this.nameInputElement.nativeElement.addEventListener('input', () => {
      clearTimeout(this.debounceTimer);
      this.debounceTimer = setTimeout(() => this.filterSchools(), this.nameInputElement.nativeElement.value.length < 2 ? 700 : 100);
    });
  }

  private toggleDropdown() {
    this.showDropdown = !this.showDropdown;
    this.headerArrowElement.nativeElement.classList.toggle('arrow--up');
    this.headerArrowElement.nativeElement.classList.toggle('arrow--down');
  }

  private toggleSidebar() {
    this.sidebarElement.nativeElement.classList.toggle('sidebar--hide');
    this.hamburgerElement.nativeElement.classList.toggle('change-bar1');
    this.hamburgerElement.nativeElement.classList.toggle('change-bar2');
    this.hamburgerElement.nativeElement.classList.toggle('change-bar3');
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.dropdownButtonElement.nativeElement.contains(event.target) && !this.headerArrowElement.nativeElement.contains(event.target)) {
      this.showDropdown = false;
      this.headerArrowElement.nativeElement.classList.remove('arrow--up');
      this.headerArrowElement.nativeElement.classList.add('arrow--down');
    }
  }

  public filterSchools() {
    this.markers.clearLayers();
    const voivodeshipSelect = this.voivodeshipElement.nativeElement.value.toUpperCase();
    const typedValue = this.nameInputElement.nativeElement.value.toUpperCase();
    const filters = ["nazwa", "miejscowosc", "poczta"];
    let schoolTypes: string[] = [];

    this.schools.forEach(school => {
      const data = school.businessData;
      if (
        (data.wojewodztwo.toUpperCase() === voivodeshipSelect || voivodeshipSelect === "ALL-SCHOOLS") &&
        this.selectedSchoolTypes[school.typ.toLowerCase()] &&
        filters.some(filter => data[filter].toUpperCase().includes(typedValue))
      ) {
        this.createMarker(school);
      }
    });
    this.map.addLayer(this.markers);
  }

  public deleteChildNodes(container: HTMLElement) {
    while (container.firstChild) {
      container.removeChild(container.firstChild);
    }
  }
}
