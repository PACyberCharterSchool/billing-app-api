import { SchoolDistrict } from './school-district.model';

export class Refund {
  id: number;
  amount: number;
  checkNumber: string;
  date: Date;
  schoolYear: string;
  created: Date;
  lastUpdated: Date;
  schoolDistrict: SchoolDistrict;
  username: string;
}
