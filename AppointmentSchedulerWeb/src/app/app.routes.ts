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
import { HomeComponent } from '../view/ui-process-components/home/home.component';
import { ConfigManagementComponent } from '../view/ui-process-components/config-management/config-management.component';
import { NotificationManagementComponent } from '../view/ui-process-components/notification-management/notification-management.component';
import { AvailabilityTimeSlotManagementComponent } from '../view/ui-process-components/availability-time-slot-management/availability-time-slot-management.component';
import { RegisterClientComponent } from '../view/ui-process-components/register-client/register-client.component';
import { EditClientComponent } from '../view/ui-process-components/edit-client/edit-client.component';
import { RegisterAssistantComponent } from '../view/ui-process-components/register-assistant/register-assistant.component';
import { EditAssistantComponent } from '../view/ui-process-components/edit-assistant/edit-assistant.component';

export const routes: Routes = [
  {
    path: "",
    component: RootComponent,
    children: [
      { path: "", component: HomeComponent },
      { path: "configuration", component: ConfigManagementComponent, title: "Cambiame", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR, RoleType.CLIENT, RoleType.ASSISTANT] } },
      { path: "notifications", component: NotificationManagementComponent, title: "Cambiame", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR, RoleType.CLIENT, RoleType.ASSISTANT] } },
      { path: "management/availability-time-slots", component: AvailabilityTimeSlotManagementComponent, title: "Cambiame", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
      { path: "management/appointment", component: AppointmentManagementComponent, title: "Cambiame", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
      { path: "management/assistant", component: AssistantManagementComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
      { path: "management/assistant/edit", component: EditAssistantComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
      { path: "management/assistant/register", component: RegisterAssistantComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
      { path: "management/client", component: ClientManagementComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
      { path: "management/client/register", component: RegisterClientComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
      { path: "management/client/edit", component: EditClientComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
      { path: "management/service", component: ServiceManagementComponent, title: "Citas", canActivate: [AuthGuard, RoleGuard], data: { roles: [RoleType.ADMINISTRATOR] } },
    ]
  },
  { path: "login", component: LoginComponent, canActivate: [RedirectIfAuthenticatedGuard] },
  { path: "signup", component: RegisterComponent, canActivate: [RedirectIfAuthenticatedGuard] },
  { path: '**', redirectTo: "" }
];
