import { Injectable } from '@angular/core';
// @ts-ignore: optional runtime dependency; provide via package.json at runtime
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { Notification } from '../models/models';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly HUB_URL = 'https://localhost:7153/hubs/notifications';
  private hubConnection: any = null;
  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  notifications$ = this.notificationsSubject.asObservable();

  constructor(private auth: AuthService) {
    this.auth.user$.subscribe({
      next: u => {
        try {
          if (u && this.auth.getToken()) this.start();
          else this.stop();
        } catch {
          this.stop();
        }
      },
      error: () => this.stop()
    });
  }

  private start() {
    if (this.hubConnection) return;

    const signalRAny = signalR as any;
    const HubConnectionBuilder: any = signalRAny?.HubConnectionBuilder;
    if (!HubConnectionBuilder) return;

    const LogLevel = signalRAny?.LogLevel;

    try {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(this.HUB_URL, { accessTokenFactory: () => this.auth.getToken() ?? '' })
        .configureLogging(LogLevel?.Information ?? 1)
        .withAutomaticReconnect()
        .build();
    } catch {
      this.hubConnection = null;
      return;
    }

    this.hubConnection.on('ReceiveNotification', (message: string) => {
      const n: Notification = {
        id: globalThis.crypto?.randomUUID?.() ?? `${Date.now()}`,
        title: 'Notification',
        message,
        createdOnUtc: new Date().toISOString(),
        isRead: false
      };
      const current = this.notificationsSubject.value;
      this.notificationsSubject.next([n, ...current]);
    });

    this.hubConnection.on('NewNotification', (payload: any) => {
      const n: Notification = {
        id: payload?.id ?? (globalThis.crypto?.randomUUID?.() ?? `${Date.now()}`),
        title: payload?.title ?? 'Notification',
        message: payload?.message,
        createdOnUtc: payload?.createdOnUtc ?? new Date().toISOString(),
        isRead: false
      };
      const current = this.notificationsSubject.value;
      this.notificationsSubject.next([n, ...current]);
    });

    this.hubConnection.start().catch(() => this.stop());
  }

  private stop() {
    if (!this.hubConnection) return;
    this.hubConnection.stop().finally(() => { this.hubConnection = null; this.notificationsSubject.next([]); });
  }

  markAllRead() {
    const items = this.notificationsSubject.value.map(n => ({ ...n, isRead: true }));
    this.notificationsSubject.next(items);
  }
}
