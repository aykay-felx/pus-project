import { Routes } from '@angular/router';
import { MainComponent } from './main/main.component'
import { AdminComponent } from './admin/admin.component'
import { HistoryComponent } from './history/history.component';
import { EditSchoolModalComponent } from './edit-school-modal/edit-school-modal.component';


export const routes: Routes = [
  { path: 'main', component: MainComponent },
  { path: 'admin', component: AdminComponent},
  { path: 'history', component: HistoryComponent},
  { path: 'edit', component: EditSchoolModalComponent},
  { path: '', redirectTo : '/main', pathMatch: 'full' },
  { path: '**', redirectTo: '/main' },
];
