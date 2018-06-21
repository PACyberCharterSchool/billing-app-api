import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FormsModule } from '@angular/forms';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { InvoiceCreateFormComponent } from './invoice-create-form.component';

import { NgbModule, NgbTypeahead, NgbTypeaheadConfig, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Globals } from '../../../globals';

import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { TemplatesService } from '../../../services/templates.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

describe('InvoiceCreateFormComponent', () => {
  let component: InvoiceCreateFormComponent;
  let fixture: ComponentFixture<InvoiceCreateFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceCreateFormComponent ],
      imports: [
        FormsModule,
        NgbModule.forRoot()
      ],
      providers: [
        NgbTypeahead,
        NgbTypeaheadConfig,
        NgbActiveModal,
        Globals,
        ReportsService,
        UtilitiesService,
        AcademicYearsService,
        TemplatesService,
        SchoolDistrictService,
        HttpClient,
        HttpHandler
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceCreateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
