import { Component, OnInit } from '@angular/core';

import { Report } from '../../../models/report.model';

import { UtilitiesService } from '../../../services/utilities.service';
import { ReportsService } from '../../../services/reports.service';
import { StudentRecordsService } from '../../../services/student-records.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { FileSaverService } from '../../../services/file-saver.service';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-student-activity-list',
  templateUrl: './student-activity-list.component.html',
  styleUrls: ['./student-activity-list.component.scss']
})
export class StudentActivityListComponent implements OnInit {
  public reports: Report[];
  public allReports: Report[];
  private skip: number;
  public property: string;
  public direction: number;
  private isDescending: boolean;
  public searchText: string;
  public selectedScope: string;
  public selectedCreateScope: string;
  public selectedCreateSchoolYear: string;
  public spinnerMsg: string;
  public selectedAsOfBillingDate: Date;
  public scopes: string[];

  constructor(
    private utilitiesService: UtilitiesService,
    private ngbModal: NgbModal,
    private reportsService: ReportsService,
    private studentRecordsService: StudentRecordsService,
    private academicYearsService: AcademicYearsService,
    private fileSaverService: FileSaverService,
    private ngxSpinnerService: NgxSpinnerService
  ) { }

  ngOnInit() {
    this.spinnerMsg = 'Loading bulk student activity reports.  Please wait...';
    this.skip = 0;

    this.ngxSpinnerService.show();
    this.studentRecordsService.getHeaders().subscribe(
      data => {
        this.scopes = data['scopes'];
        this.selectedScope = this.scopes[0];
      },
      error => {
        console.log('StudentActivityListComponent.ngOnInit():  error is ', error);
      }
    );

    this.reportsService.getActivities(null, null, null, null).subscribe(
      data => {
        console.log('StudentActivityListComponent.ngOnInit(): invoices are ', data['reports']);
        this.reports = this.allReports = data['reports'];
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log('StudentActivityComponent.ngOnInit(): error is ', error);
        this.ngxSpinnerService.hide();
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  listDisplayableFields() {
    if (this.allReports) {
      const fields = this.utilitiesService.objectKeys(this.allReports[0]);
      const rejected = ['data', 'xlsx', 'type', 'id'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(report: Report) {
    const vkeys = this.listDisplayableFields();
    const selected = this.utilitiesService.pick(report, vkeys);

    return this.utilitiesService.objectValues(selected);
  }

  refreshActivityReports(): void {
    this.reportsService.getActivities(null, null, null, null).subscribe(
      data => {
        console.log('StudentActivityListComponent.ngOnInit(): invoices are ', data['reports']);
        this.reports = this.allReports = data['reports'];
        this.ngxSpinnerService.hide();
      },
      error => {
        console.log('StudentActivityComponent.ngOnInit(): error is ', error);
        this.ngxSpinnerService.hide();
      }
    );
  }

  filterByScope(scope: string): void {
    this.selectedScope = scope;
    this.reportsService.getActivities(null, null, scope, null).subscribe(
      data => {
        this.reports = this.allReports = data['reports'];
      },
      error => {
        console.log('StudentActivityListComponent.filterByScope():  error is ', error);
      }
    );
  }

  filterStudentActivityReports(): void {

  }

  private generateBulkActivityName(schoolYear: string, asOfDate: string): string {
    const billingDate: Date = new Date(asOfDate);
    const months: string[] = [
      'January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'
    ];

    return 'BulkActivity_' + this.selectedCreateScope + '_' + schoolYear;
  }

  getSchoolYears(): string[] {
    return this.academicYearsService.getAcademicYears();
  }

  selectSchoolYear(year: string): void {
    this.selectedCreateSchoolYear = year;
  }

  create(): void {
    this.reportsService.createBulkActivity(
      {
        'schoolYear': this.selectedCreateSchoolYear.replace(/\s+/g, ''),
        'name': this.generateBulkActivityName(this.selectedCreateSchoolYear, this.selectedCreateScope),
        'bulkStudentInformation': {
          'asOf': this.selectedAsOfBillingDate,
          'toSchoolDistrict': this.selectedAsOfBillingDate,
          'toPDE': this.selectedAsOfBillingDate,
          'scope': this.selectedCreateScope
        }
      }
    ).subscribe(
      data => {
        console.log('StudentActivityListComponent.create(): data is ', data['reports']);
        this.ngxSpinnerService.hide();
        this.refreshActivityReports();
      },
      error => {
        console.log('StudentActivityListComponent.create(): error is ', error);
        this.ngxSpinnerService.hide();
      }
    );
  }

  displayCreateBulkActivityDialog(bulkCreateContent): void {
    const modal = this.ngbModal.open(bulkCreateContent, { centered: true, size: 'sm' });
    modal.result.then(
      (result) => {
        console.log('StudentActivityListComponent.createBulkInvoice(): result is ', result);
        this.ngxSpinnerService.show();
      },
      (reason) => {
        console.log('StudentActivityListComponent.createBulkInvoice(): reason is ', reason);
      }
    );
  }

  downloadStudentActivityReport(invoice: Report) {
    this.spinnerMsg = 'Downloading student activity.  Please wait...';
    this.ngxSpinnerService.show();
    this.reportsService.getReportDataByFormat(invoice, 'excel').subscribe(
      data => {
        console.log('InvoiceListComponent().downloadInvoiceStudentActivity():  data is ', data);
        this.ngxSpinnerService.hide();
        this.fileSaverService.saveStudentActivityAsExcelFile(data, invoice);
      },
      error => {
        this.ngxSpinnerService.hide();
        console.log('InvoiceListComponent().downloadInvoiceStudentActivity():  error is ', error);
      }
    );
  }

  selectScope(scope: string): void {
    this.selectedScope = scope;
  }

  selectCreateScope(scope: string): void {
    this.selectedCreateScope = scope;
  }
}
