export class ScheduledService {
  name: string;
  price: number;
  minutes: number;
  uuid: string;
  startDate: Date;
  endDate: Date;

  constructor(name: string, price: number, minutes: number, uuid: string, startDate: Date, endDate: Date) {
    this.name = name;
    this.price = price;
    this.minutes = minutes;
    this.uuid = uuid;
    this.startDate = startDate;
    this.endDate = endDate;
  }
}
