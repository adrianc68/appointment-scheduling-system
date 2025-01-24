import { Routes } from '@angular/router';
import { LoginComponent } from '../view/ui-process-components/login/login.component';
import { NotFoundComponent } from '../view/ui-process-components/not-found/not-found.component';

export const routes: Routes = [
  {path: "login", component: LoginComponent, title : "Log in" },
  {path: '**', component: NotFoundComponent, title : "Not found"}
];
