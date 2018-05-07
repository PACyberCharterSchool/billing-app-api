import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { Globals } from '../../../globals';

import { Student } from '../../../models/student.model';
import { SchoolDistrict } from '../../../models/school-district.model';

import { StudentsService } from '../../../services/students.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { CurrentStudentService } from '../../../services/current-student.service';

@Component({
  selector: 'app-students-list',
  templateUrl: './students-list.component.html',
  styleUrls: ['./students-list.component.scss']
})
export class StudentsListComponent implements OnInit {

  private students: Student[];
  private schoolDistricts: SchoolDistrict[];
  private advancedSearchEnabled: boolean;
  private searchText: string;
  private isDescending: boolean;
  private property: string;
  private direction: number;
  private items: Student[];
  private startDate: Date;
  private endDate: Date;
  private selectedStudent: Student;
  private skip: number;

  constructor(
    private globals: Globals,
    private studentsService: StudentsService,
    private schoolDistrictService: SchoolDistrictService,
    private currentStudentService: CurrentStudentService,
    private router: Router) {
      this.advancedSearchEnabled = false;
      this.isDescending = false;
      this.property = 'paCyberId';
      this.direction = 1;
      this.skip = 0;
    }

  ngOnInit() {
    this.studentsService.getStudents(this.skip).subscribe(
      data => {
        this.updateScrollingSkip();
        this.students = this.items = data['students'];
        console.log('StudentsListComponent.ngOnInit():  students are ', this.students);
      }
    );

    this.schoolDistrictService.getSchoolDistricts().subscribe(
      data => {
        this.schoolDistricts = data['schoolDistricts'];
        console.log('StudentsListComponent.ngOnInit():  school districts are', this.schoolDistricts);
      }
    );

    this.currentStudentService.currentStudent.subscribe((student) => this.selectedStudent = student);
  }

  dateSelectedStartDateHandler(date: Date) {
    this.studentsService.getStudentsFilteredByStartDate(date).subscribe(
      data => {
        this.students = this.items = data['students'];
      }
    );
  }

  dateSelectedEndDateHandler(date: Date) {
    this.studentsService.getStudentsFilteredByEndDate(date).subscribe(
      data => {
        this.students = this.items = data['students'];
      }
    );
  }

  studentsUpdatedHandler(students: Student[]) {
    this.students = this.items = students;
    console.log('StudentsListComponent.studentsUpdatedHandler():  students are ', this.students);
  }

  getStudents($event) {
    this.studentsService.getStudents(this.skip).subscribe(
      data => {
        this.students = this.items = data['students'];
        console.log('StudentsListComponent.getStudents():  students are ', this.students);
      }
    );
  }

  sort(property) {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  showStudentDetails(studentId: number) {
    console.log('StudentsListComponent.showStudentDetails():  studentId is ', studentId);
    const s: Student = this.students.find((student) => student.id === studentId);
    this.currentStudentService.changeStudent(s);
    this.router.navigate(['/students', { id: studentId, outlets: {'action': [`${studentId}`]} }]);
  }

  toggleAdvancedSearchTools() {
    this.advancedSearchEnabled = !this.advancedSearchEnabled;
  }

  resetStudentList() {
    this.studentsService.getStudents(this.skip).subscribe(
      data => {
        this.students = this.items = data['students'];
      }
    );
  }

  filterStudentListByNameOrId() {
    if (this.searchText) {
      this.studentsService.getStudentsFilteredByNameOrId(this.searchText).subscribe(
        data => {
          this.students = this.items = data['students'];
          console.log('StudentsListComponent.filterStudentList():  students are ', data);
        }
      );
    }
  }

  onScroll($event) {
    this.getStudents($event);
  }

  private updateScrollingSkip() {
    this.skip += this.globals.take;
  }
}
