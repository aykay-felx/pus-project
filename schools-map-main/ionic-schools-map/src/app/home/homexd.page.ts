import { Component } from '@angular/core';
import { IonHeader, IonToolbar, IonTitle, IonContent } from '@ionic/angular/standalone';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FormsModule } from '@angular/forms';
import { HttpHeaders } from '@angular/common/http';
import * as L from 'leaflet';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
  standalone: true,
  imports: [IonHeader, IonToolbar, IonTitle, IonContent, CommonModule, IonicModule, FormsModule],
})
export class HomePage {
  constructor() {}
  showDropdown = false;
  activeTab = 'general';
  selectedVoivodeship = 'all-schools';
  selectedSchoolTypes = {
    podstawowa: true,
    liceum: true,
    technikum: true
  };

  headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Credentials': 'true'
  });

  header = document.querySelector(".header");
  hamburger = document.querySelector(".hamburger");
  bar1 = document.querySelector(".hamburger__bar-1");
  bar2 = document.querySelector(".hamburger__bar-2");
  bar3 = document.querySelector(".hamburger__bar-3");
  sidebar = document.querySelector(".sidebar");
  headerArrow = document.querySelector(".header__arrow");
  filterBtn = document.querySelector(".filter__btn");
  voivodeship = document.querySelector("#select-voivodeship");
  nameInput = document.querySelector(".header__search");

  addressInfoContainer = document.querySelector(".sidebar__address-info");
  detailedInfoContainer = document.querySelector(".sidebar__detailed-info");
  dropdownButton = document.querySelector(".dropdown-button");
  dropdownContent = document.querySelector(".dropdown-content");

  contentTab1 = document.querySelector("#content-1");
  contentTab2 = document.querySelector("#content-2");
  selectTab1 = document.querySelector("#tab1");
  selectTab2 = document.querySelector("#tab2");
  checkboxes = document.querySelectorAll("#school-type-checkbox");

  schools: any[] = [];
  markers = L.markerClusterGroup();

  //map center point
  mapZoom = 5;

  //set map zoom
  mapZoom = window.innerWidth > 800 ? 6 : 5;
  if (window.innerWidth > 800) {
    this.mapZoom = 6;
  } else if (window.innerWidth > 1200) {
    this.mapZoom = 6;
  }

  map = L.map("map").setView([51.9194, 19.1451], this.mapZoom);

  //map definition and page connection
  L.tileLayer(
    "https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=RakePZLBXp7DUCKOgD6V",
    {
      tileSize: 512,
      zoomOffset: -1,
      attribution:
        '\u003ca href="https://www.maptiler.com/copyright/" target="_blank"\u003e\u0026copy; MapTiler\u003c/a\u003e \u003ca href="https://www.openstreetmap.org/copyright" target="_blank"\u003e\u0026copy; OpenStreetMap contributors\u003c/a\u003e',
      crossOrigin: true,
    }
  ).addTo(this.map);

  // Przeniesienie funkcji do klasy
  //fetch all schools while page loaded
  document.onload = this.fetchAllSchools.bind(this);

  //EVENT LISTENERS

  //filter on school type checkbox change
  this.checkboxes.forEach((checkbox) =>
    checkbox.addEventListener("change", this.filterSchools.bind(this))
  );

  //filter on voivodeship change
  this.voivodeship.addEventListener("change", this.filterSchools.bind(this));

  //filter on input change with timeout to prevent lagging
  this.nameInput.addEventListener("input", this.onInputChange.bind(this));

  //show content on tabs click
  this.selectTab1.addEventListener("click", this.showContentTab1.bind(this));
  this.selectTab2.addEventListener("click", this.showContentTab2.bind(this));

  //search dropdown
  this.dropdownButton.addEventListener("click", this.toggleDropdown.bind(this));

  //search bar arrow click
  this.headerArrow.addEventListener("click", this.onHeaderArrowClick.bind(this));

  //sidebar hamburger
  this.hamburger.addEventListener("click", this.toggleSidebar.bind(this));

  //HANDLERS

  //clear content tabs
  deleteChildNodes(container: HTMLElement) {
    while (container.hasChildNodes()) {
      container.removeChild(container.lastChild);
    }
  }

  //get value from school type checkbox
  getCheckboxesValue(schoolTypes: string[]) {
    this.checkboxes.forEach((cb: HTMLInputElement) => {
      if (cb.checked) {
        schoolTypes.push(cb.value.toUpperCase());
      }
    });
  }

  //handle create single marker
  createMarker(school: any) {
    const marker = L.marker([school.latitude, school.longtitude]).on(
      "click",
      this.handleMarkerClick.bind(this)
    );
    marker.bindPopup(this.createPopup(school.businessData));
    marker.myID = school.id;
    this.markers.addLayer(marker);
  }

  //load all schools handle
  fetchAllSchools() {
    fetch("https://localhost:5001/Schools", {
      method: "GET",
      headers: this.headers,
    })
      .then((res) => res.json())
      .then((data) => {
        //write all schools to arr
        this.schools = data;
        //create marker for every school loaded
        this.schools.forEach((school) => {
          this.createMarker(school);
        });
        this.map.addLayer(this.markers);
      });
  }

  //filter schools
  filterSchools() {
    this.markers.clearLayers();
    const voivodeshipSelect = (this.voivodeship as HTMLSelectElement).value.toUpperCase();
    const typedValue = (this.nameInput as HTMLInputElement).value.toUpperCase();
    let schoolTypes: string[] = [];
    const filters = ["nazwa", "miejscowosc", "poczta"];
    this.getCheckboxesValue(schoolTypes);

    this.schools.forEach((school) => {
      const data = school.businessData;
      if (
        (data.wojewodztwo.toUpperCase() === voivodeshipSelect ||
          voivodeshipSelect === "ALL-SCHOOLS") &&
        schoolTypes.some((type) => type === data.typ.toUpperCase()) &&
        (data[filters[0]].toUpperCase().includes(typedValue) ||
          data[filters[1]].toUpperCase().includes(typedValue) ||
          data[filters[2]].toUpperCase().includes(typedValue))
      ) {
        this.createMarker(school);
      }
    });
    this.map.addLayer(this.markers);
  }

  //click on marker handle
  handleMarkerClick(e: L.LeafletMouseEvent) {
    //clear details content
    this.deleteChildNodes(this.addressInfoContainer);
    this.deleteChildNodes(this.detailedInfoContainer);
    //get id of target marker
    const targetID = (e.sourceTarget as any).myID;
    //get school with target id from DB
    fetch(`https://localhost:5001/Schools/${targetID}`, {
      method: "GET",
      headers: this.headers,
    })
      .then((res) => res.json())
      .then((data) => {
        const schoolData = data.businessData;
        const schoolObj = this.translateDetailsNaming(schoolData);
        //show sidebar on desktop
        if (window.innerWidth >= 1366) {
          if (this.sidebar.classList.contains("sidebar--hide")) {
            this.sidebar.classList.toggle("sidebar--hide");
          }
        }
        this.createTabs(schoolObj);
      });
  }

  //create popup handle
  createPopup(data: any): string {
    const schoolPopup = `
      <div class="popup">
        <p class="popup__school-name">${data.nazwa}</p>
        <p class="popup__data popup__city">
        ${
          data.miejscowosc === data.poczta && data.ulica != null
            ? ""
            : data.miejscowosc
        } ${data.ulica !== null ? "" : data.numerBudynku}</p>
        <p class="popup__data popup__address">${
          data.ulica !== null ? data.ulica + " " + data.numerBudynku : ""
        } ${data.numerLokalu != null ? " lok. " + data.numerLokalu : ""}</p>
        <p class="popup__data popup__city">${data.kodPocztowy} ${data.poczta}</p>
        <a class="popup__datapopup__website" target="_blank" href="${
          data.stronaInternetowa
        }" >
        ${data.stronaInternetowa !== null ? data.stronaInternetowa : ""}</a>
        <a class="popup__datapopup__website" href="mailto:${data.email}" >
        ${data.email !== null ? data.email : ""}</a>
      </div>
      `;

    return schoolPopup;
  }

  //create content tabs
  createTabs(schoolObj: any) {
    let category = "";
    let text = "";
    for (let i = 0; i < 2; i++) {
      i == 0 ? (category = "addressInfo") : (category = "detailedInfo");

      Object.keys(schoolObj[category]).map((key) => {
        const value = schoolObj[category][key] === null ? "" : schoolObj[category][key];

        const itemContainer = document.createElement("div");
        itemContainer.classList.add("sidebar__info-item");

        const itemKey = document.createElement("p");
        itemKey.classList.add("sidebar__item-key");
        itemKey.innerText = key + ": ";

        const itemValue = document.createElement("p");
        itemValue.classList.add("sidebar__item-value");

        //Rewrite to string if there is an array of object
        if (typeof value === "object") {
          text = "";
          for (let i = 0; i < value.length; i++) {
            if (i < value.length - 1) {
              text += value[i] + ", ";
            } else {
              text += value[i];
            }
          }
        } else {
          text = value;
        }

        itemValue.innerText = text;
        itemContainer.appendChild(itemKey);
        itemContainer.appendChild(itemValue);

        i == 0
          ? this.addressInfoContainer.appendChild(itemContainer)
          : this.detailedInfoContainer.appendChild(itemContainer);
      });
    }
  }

  translateDetailsNaming(schoolData: any) {
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

  onInputChange() {
    this.nameInput.value.length < 2
      ? setTimeout(() => this.filterSchools(), 700)
      : setTimeout(() => this.filterSchools(), 100);
  }

  showContentTab1() {
    this.contentTab1.style.display = "block";
    this.contentTab2.style.display = "none";
  }

  showContentTab2() {
    this.contentTab2.style.display = "block";
    this.contentTab1.style.display = "none";
  }

  toggleDropdown() {
    this.dropdownContent.classList.toggle("dropdown--invisible");
    this.dropdownContent.classList.toggle("dropdown--visible");
    if (!this.sidebar.classList.contains("sidebar--hide")) {
      this.sidebar.classList.toggle("sidebar--hide");
    }
  }

  onHeaderArrowClick() {
    this.bar1.classList.remove("change-bar1");
    this.bar2.classList.remove("change-bar2");
    this.bar3.classList.remove("change-bar3");
    if (this.dropdownContent.classList.contains("dropdown--invisible")) {
      this.headerArrow.classList.remove("arrow--up");
      this.headerArrow.classList.add("arrow--down");
    } else if (this.dropdownContent.classList.contains("dropdown--visible")) {
      this.headerArrow.classList.remove("arrow--down");
      this.headerArrow.classList.add("arrow--up");
    }
  }

  toggleSidebar() {
    this.sidebar.classList.toggle("sidebar--hide");
    if (this.dropdownContent.classList.contains("dropdown--visible")) {
      this.dropdownContent.classList.toggle("dropdown--visible");
      this.dropdownContent.classList.toggle("dropdown--invisible");
    }
    if (this.dropdownContent.classList.contains("dropdown--invisible")) {
      this.headerArrow.classList.remove("arrow--up");
      this.headerArrow.classList.add("arrow--down");
      this.bar1.classList.toggle("change-bar1");
      this.bar2.classList.toggle("change-bar2");
      this.bar3.classList.toggle("change-bar3");
    }
  }
}
