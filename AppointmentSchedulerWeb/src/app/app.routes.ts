import { Routes } from '@angular/router';
import { LoginComponent } from '../view/ui-process-components/login/login.component';
import { AuthGuard } from '../view/model/auth-guard';
import { RootComponent } from '../view/ui-process-components/root/root.component';
import { AppointmentManagementComponent } from '../view/ui-process-components/appointment-management/appointment-management.component';
import { AssistantManagementComponent } from '../view/ui-process-components/assistant-management/assistant-management.component';
import { ClientManagementComponent } from '../view/ui-process-components/client-management/client-management.component';
import { ServiceManagementComponent } from '../view/ui-process-components/service-management/service-management.component';
import { RegisterComponent } from '../view/ui-process-components/register/register.component';
import { RedirectIfAuthenticatedGuard } from '../view/model/redirect-if-authenticated-guard';
import { RoleType } from '../view-model/business-entities/types/role.types';
import { RoleGuard } from '../view/model/role-guard';

export const routes: Routes = [
  {
    path: "",
    component: RootComponent,
  },
  { path: "login", component: LoginComponent, canActivate: [RedirectIfAuthenticatedGuard] },
  { path: "signup", component: RegisterComponent, canActivate: [RedirectIfAuthenticatedGuard] },
  { path: "management/appointment", component: AppointmentManagementComponent, title: "Administrar citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
  { path: "management/assistant", component: AssistantManagementComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
  { path: "management/client", component: ClientManagementComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
  { path: "management/service", component: ServiceManagementComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
  { path: '**', redirectTo: "" }
];
