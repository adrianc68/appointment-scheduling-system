import { Routes } from '@angular/router';
import { LoginComponent } from '../view/ui-process-components/login/login.component';
import { AuthGuard } from '../view/model/auth-guard';
import { RootComponent } from '../view/ui-process-components/root/root.component';
import { AppointmentManagementComponent } from '../view/ui-process-components/appointment-management/appointment-management.component';
import { AssistantManagementComponent } from '../view/ui-process-components/assistant-management/assistant-management.component';
import { ClientManagementComponent } from '../view/ui-process-components/client-management/client-management.component';
import { ServiceManagementComponent } from '../view/ui-process-components/service-management/service-management.component';
import { RegisterComponent } from '../view/ui-process-components/register/register.component';

export const routes: Routes = [
  {
    path: "",
    component: RootComponent,
  },
  { path: "login", component: LoginComponent, title: "Log in" },
  { path: "signup", component: RegisterComponent, title: "Register" },
  { path: "appointment/management", component: AppointmentManagementComponent, title: "Citas", canActivate: [AuthGuard] },
  { path: "assistant/management", component: AssistantManagementComponent, title: "Citas", canActivate: [AuthGuard] },
  { path: "client/management", component: ClientManagementComponent, title: "Citas", canActivate: [AuthGuard] },
  { path: "service/management", component: ServiceManagementComponent, title: "Citas", canActivate: [AuthGuard] },
  { path: '**', redirectTo: "" }
];
