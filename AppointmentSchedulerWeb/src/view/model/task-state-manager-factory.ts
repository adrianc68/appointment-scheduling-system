import { Injectable } from '@angular/core';
import { TaskStateManagerService } from './task-state-manager.service';

@Injectable({
  providedIn: 'root'
})
export class TaskStateManagerFactory {
  createInstance() {
    return new TaskStateManagerService();
  }
}

