import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthenticationService } from '../../security/authentication/authentication.service';
import { UserCredentialsJwt } from '../../../view-model/business-entities/user-credentials-jwt';
import { LoggingService } from '../../operation-management/logginService/logging.service';
import { WebRoutes } from '../../operation-management/model/web-routes.constants';
import { LocalStorageKeys } from '../../security/model/local-storage-keys.constants';
import { ConfigService } from '../../operation-management/configService/config.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService implements OnDestroy {
  private hubConnection!: signalR.HubConnection;
  public messageReceived = new Subject<string>();
  private connectionEstablished = new Subject<boolean>();
  private authSubscription!: Subscription;

  constructor(private authService: AuthenticationService, private logginService: LoggingService, private configService: ConfigService) {
    //this.authSubscription = this.authService.getCredentials().pipe(
    //  map((credentials: UserCredentialsJwt | null) => credentials?.accessToken)
    //).subscribe(token => {
    //  if (token) {
    //    this.initConnection(token);
    //  } else {
    //    this.stopConnection();
    //  }
    //});
  }

  public removeMessageListener(methodName: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.off(methodName);
    }
  }

  public addMessageListener(methodName: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.on(methodName, (message: string) => {
        this.messageReceived.next(message);
      });
    } else {
      this.logginService.error(`SignalR cannot add listener ${methodName}, connection not established`);
    }
  }

  public sendMessage(methodName: string, message: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke(methodName, message)
        .catch(err => this.logginService.error(err));
    } else {
      this.logginService.error("SignalR cannot send message, connection not established");
    }
  }


  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => this.logginService.log("SignalR connection finished"))
        .catch(err => this.logginService.error(err));
      this.connectionEstablished.next(false);

    }
  }

  getConnectionStatus(): Observable<boolean> {
    return this.connectionEstablished.asObservable();
  }

  ngOnDestroy(): void {
    this.stopConnection();
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  private initConnection(accessToken: string) {
    //if (this.hubConnection) {
    //  this.stopConnection();
    //}
    //
    //this.hubConnection = new signalR.HubConnectionBuilder()
    //  .withUrl(`${this.configService.getApiBaseUrl()}${WebRoutes.notification_hub}?${LocalStorageKeys.access_token}=${accessToken}`)
    //  .configureLogging(signalR.LogLevel.Information)
    //  .withAutomaticReconnect()
    //  .build();
    //
    //this.startConnection();
  }

  private async startConnection() {
    //try {
    //  await this.hubConnection.start();
    //  this.connectionEstablished.next(true);
    //
    //} catch (err) {
    //  this.logginService.error(err);
    //  setTimeout(() => this.startConnection(), 5000);
    //  this.connectionEstablished.next(false);
    //}
  }






}

