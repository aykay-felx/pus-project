import { Component, OnInit, ElementRef, ViewChild, HostListener } from '@angular/core';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonMenu, IonSelect, IonCheckbox, IonSearchbar } from '@ionic/angular/standalone';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { LeafletMouseEvent } from 'leaflet';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
  standalone: true,
  imports: [IonHeader, IonToolbar, IonTitle, IonContent, IonMenu, IonSelect, IonCheckbox, IonSearchbar, CommonModule, FormsModule],
})

export class HomePage {
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

  constructor() {}

  public setupEventListeners() {
    this.dropdownButtonElement.nativeElement.addEventListener('click', () => this.toggleDropdown());
    this.headerArrowElement.nativeElement.addEventListener('click', () => this.toggleDropdown());
    this.selectTab1Element.nativeElement.addEventListener('click', () => this.activeTab = 'general');
    this.selectTab2Element.nativeElement.addEventListener('click', () => this.activeTab = 'detailed');
    this.hamburgerElement.nativeElement.addEventListener('click', () => this.toggleSidebar());
    //this.voivodeshipElement.nativeElement.addEventListener('change', () => this.filterSchools());
    //this.nameInputElement.nativeElement.addEventListener('input', () => {
      //clearTimeout(this.debounceTimer);
      //this.debounceTimer = setTimeout(() => this.filterSchools(), this.nameInputElement.nativeElement.value.length < 2 ? 700 : 100);
    //});
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

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.dropdownButtonElement.nativeElement.contains(event.target) && !this.headerArrowElement.nativeElement.contains(event.target)) {
      this.showDropdown = false;
      this.headerArrowElement.nativeElement.classList.remove('arrow--up');
      this.headerArrowElement.nativeElement.classList.add('arrow--down');
    }
  }
}
