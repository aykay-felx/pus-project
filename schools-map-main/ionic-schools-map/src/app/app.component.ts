import { Component } from '@angular/core';
import { IonApp, IonRouterOutlet } from '@ionic/angular/standalone';
import { MainComponent } from './main/main.component';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrl: 'app.component.scss',
  standalone: true,
  imports: [IonApp, IonRouterOutlet, MainComponent],
})
export class AppComponent {
  constructor() {}
}
