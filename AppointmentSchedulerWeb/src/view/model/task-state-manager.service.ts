import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoadingState } from './loading-state.type';

@Injectable()

export class TaskStateManagerService {
  private stateSubject = new BehaviorSubject<LoadingState>(LoadingState.NO_ACTION_PERFORMED);

  constructor() { }

  setState(newState: LoadingState): void {
    this.stateSubject.next(newState);
  }

  getState(): LoadingState {
    return this.stateSubject.value;
  }

  getStateAsObservable(): Observable<LoadingState> {
    return this.stateSubject.asObservable();
  }

}
