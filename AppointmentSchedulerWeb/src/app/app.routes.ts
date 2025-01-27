import { Routes } from '@angular/router';
import { LoginComponent } from '../view/ui-process-components/login/login.component';
import { AuthGuard } from '../view/model/auth-guard';
import { RootComponent } from '../view/ui-process-components/root/root.component';
import { AppointmentManagementComponent } from '../view/ui-process-components/appointment-management/appointment-management.component';

export const routes: Routes = [
  {
    path: "",
    component: RootComponent,
  },
  { path: "login", component: LoginComponent, title: "Log in" },
  { path: "appointment", component: AppointmentManagementComponent, title: "Citas", canActivate: [AuthGuard] },
  { path: '**', redirectTo: "" }
];
