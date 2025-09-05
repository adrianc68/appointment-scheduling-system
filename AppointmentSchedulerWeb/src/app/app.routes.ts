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
import { EditServiceComponent } from '../view/ui-process-components/edit-service/edit-service.component';
import { RegisterServiceComponent } from '../view/ui-process-components/register-service/register-service.component';
import { EditAvailabilityTimeSlotComponent } from '../view/ui-process-components/edit-availability-time-slot/edit-availability-time-slot.component';
import { RegisterAvailabilityTimeSlotComponent } from '../view/ui-process-components/register-availability-time-slot/register-availability-time-slot.component';
import { ServiceOfferManagementComponent } from '../view/ui-process-components/service-offer-management/service-offer-management.component';
import { RegisterServiceOfferComponent } from '../view/ui-process-components/register-service-offer/register-service-offer.component';
import { RegisterAppointmentAsStaffComponent } from '../view/ui-process-components/register-appointment-as-staff/register-appointment-as-staff.component';
import { ManagementLayoutComponent } from '../view/ui-components/navigation/management-layout/management-layout.component';

export const routes: Routes = [
  {
    path: "",
    component: RootComponent,
    children: [
      {
        path: "configuration",
        component: ConfigManagementComponent,
        canActivate: [AuthGuard, RoleGuard],
        data: { roles: [RoleType.ADMINISTRATOR, RoleType.CLIENT, RoleType.ASSISTANT] }
      },
      {
        path: "notifications",
        component: NotificationManagementComponent,
        canActivate: [AuthGuard, RoleGuard],
        data: { roles: [RoleType.ADMINISTRATOR, RoleType.CLIENT, RoleType.ASSISTANT] }
      },
      {
        path: "",
        component: ManagementLayoutComponent,
        data: { breadcrumb: "Inicio" },
        children: [
          {
            path: "",
            component: HomeComponent,
          },
          {
            path: "management",
            canActivate: [AuthGuard, RoleGuard],
            data: { roles: [RoleType.ADMINISTRATOR] },
            children: [
              {
                path: "availability-time-slots", data: { breadcrumb: "Disponibilidad" },
                children: [
                  { path: "", component: AvailabilityTimeSlotManagementComponent, },
                  { path: "edit", component: EditAvailabilityTimeSlotComponent, data: { breadcrumb: "Editar" } },
                  { path: "register", component: RegisterAvailabilityTimeSlotComponent, data: { breadcrumb: "Registrar" } }
                ]
              },
              {
                path: "service-offer", data: { breadcrumb: "Asignaciones" },
                children: [
                  { path: "", component: ServiceOfferManagementComponent },
                  { path: "register", component: RegisterServiceOfferComponent, data: { breadcrumb: "Registrar" } },
                ]
              },
              {
                path: "assistant", data: { breadcrumb: "Asistentes" },
                children: [
                  { path: "", component: AssistantManagementComponent },
                  { path: "edit", component: EditAssistantComponent, data: { breadcrumb: "Editar" } },
                  { path: "register", component: RegisterAssistantComponent, data: { breadcrumb: "Registrar" } },
                ]
              },
              {
                path: "client", data: { breadcrumb: "Clientes" },
                children: [
                  { path: "", component: ClientManagementComponent },
                  { path: "register", component: RegisterClientComponent, data: { breadcrumb: "Registrar" } },
                  { path: "edit", component: EditClientComponent, data: { breadcrumb: "Editar" } },
                ]
              },
              {
                path: "service", data: { breadcrumb: "Servicios" },
                children: [
                  { path: "", component: ServiceManagementComponent },
                  { path: "edit", component: EditServiceComponent, data: { breadcrumb: "Editar" } },
                  { path: "register", component: RegisterServiceComponent, data: { breadcrumb: "Registrar" } },
                ]
              },
              {
                path: "appointment", data: { breadcrumb: "Citas" },
                children: [
                  { path: "", component: AppointmentManagementComponent },
                  { path: "staff", component: RegisterAppointmentAsStaffComponent, data: { breadcrumb: "Registrar" } },
                ]
              },
            ]
          }
        ]
      },

    ]
  },
  { path: "login", component: LoginComponent, canActivate: [RedirectIfAuthenticatedGuard], data: { breadcrumb: "Iniciar sesión" } },
  { path: "signup", component: RegisterComponent, canActivate: [RedirectIfAuthenticatedGuard], data: { breadcrumb: "Registrarse" } },
  { path: "**", redirectTo: "" }
];

