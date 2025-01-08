
import { ModalController } from '@ionic/angular';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-school-modal',
  templateUrl: './edit-school-modal.component.html',
  styleUrls: ['./edit-school-modal.component.scss'],
  standalone: true,
  imports: [IonicModule, FormsModule, CommonModule]
})
export class EditSchoolModalComponent implements OnInit {
  @Input() school: any; // Przekazane dane szkoły

  constructor(private modalController: ModalController) {}

  ngOnInit(): void {
    // Możesz dodać inicjalizację tutaj, jeśli jest potrzebna
  }

  saveChanges() {
    // Zatwierdź zmiany i zamknij modal
    this.modalController.dismiss(this.school);
  }

  close() {
    // Zamknij modal bez zapisywania
    this.modalController.dismiss();
  }
}

