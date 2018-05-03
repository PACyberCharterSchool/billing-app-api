import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { StudentsRoutingModule } from './students-routing.module';

import { StudentsComponent } from './students.component';
import { StudentsListComponent } from './students-list/students-list.component';
import { StudentsDetailComponent } from './students-detail/students-detail.component';
import { StudentDatepickerComponent } from './student-datepicker/student-datepicker.component';
import { StudentDetailsInfoComponent } from './student-details-info/student-details-info.component';
import { StudentHistoryInfoComponent } from './student-history-info/student-history-info.component';
import { StudentAdvancedFilterComponent } from './student-advanced-filter/student-advanced-filter.component';
import { StudentAddressHistoryComponent } from './student-address-history/student-address-history.component';
import { StudentEnrollmentHistoryComponent } from './student-enrollment-history/student-enrollment-history.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { IepEnrolledPipe } from '../../pipes/iep-enrolled.pipe';
import { OrderByPipe } from '../../pipes/orderby.pipe';

@NgModule({
  declarations: [
    StudentsComponent,
    StudentsListComponent,
    StudentsDetailComponent,
    StudentDatepickerComponent,
    StudentDetailsInfoComponent,
    StudentHistoryInfoComponent,
    StudentAdvancedFilterComponent,
    StudentAddressHistoryComponent,
    StudentEnrollmentHistoryComponent,
    IepEnrolledPipe,
    OrderByPipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    StudentsRoutingModule,
    NgbModule,
    HttpClientModule
  ],
  providers: [ ]
})

export class StudentsModule { }