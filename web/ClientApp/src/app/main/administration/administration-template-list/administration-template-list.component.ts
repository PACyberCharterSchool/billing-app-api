import { Component, OnInit } from '@angular/core';

import { UtilitiesService } from '../../../services/utilities.service';
import { TemplatesService } from '../../../services/templates.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

import { Globals } from '../../../globals';

import { Template } from '../../../models/template.model';

import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-administration-template-list',
  templateUrl: './administration-template-list.component.html',
  styleUrls: ['./administration-template-list.component.scss']
})
export class AdministrationTemplateListComponent implements OnInit {
  private property: string;
  private isDescending: boolean;
  public direction: number;
  public searchText: string;
  private skip: number;
  public templates: Template[];
  private allTemplates: Template[];
  private selectedTemplateFile;
  private selectedSchoolYear;
  private selectedTemplateType;

  constructor(
    private globals: Globals,
    private utilitiesService: UtilitiesService,
    private templatesService: TemplatesService,
    private academicYearsService: AcademicYearsService,
    private ngbModal: NgbModal
  ) {
    this.property = 'name';
    this.direction = 1;
    this.skip = 0;
  }

  ngOnInit() {
    this.templatesService.getTemplates(this.skip).subscribe(
      data => {
        console.log('AdministrationTemplateListComponent.ngOnInit():  data is ', data);
        this.templates = this.allTemplates = data['templates'];
      },
      error => {
        console.log('AdministrationTemplateListComponent.ngOnInit():  error is ', error);
      }
    );
  }

  sort(property): void {
    this.isDescending = !this.isDescending; // change the direction
    this.property = property;
    this.direction = this.isDescending ? 1 : -1;
  }

  filterTemplateRecords(): void {
    this.templates = this.allTemplates.filter(
      (i) => {
        const re = new RegExp(this.searchText, 'gi');
        if (i &&
          i.name.search(re) !== -1
        ) {
          return true;
        }
        return false;
      }
    );
    console.log('AdministrationTemplateListComponent.filterTemplateRecords():  templates are ', this.templates);
  }

  resetTemplateRecords(): void {
    this.templates = this.allTemplates;
    this.searchText = '';
  }

  refreshTemplateList(): void {
    this.templatesService.getTemplates(this.skip).subscribe(
      data => {
        console.log('AdministrationTemplateListComponent.ngOnInit():  data is ', data);
        this.templates = this.allTemplates = data['templates'];
      },
      error => {
        console.log('AdministrationTemplateListComponent.ngOnInit():  error is ', error);
      }
    );
  }

  listDisplayableFields(): Object[] {
    if (this.templates) {
      const fields = this.utilitiesService.objectKeys(this.templates[0]);
      const rejected = ['id'];
      return fields.filter((i) => !rejected.includes(i));
    }
  }

  listDisplayableValues(t: Template): Object[] {
    if (t) {
      const vkeys = this.listDisplayableFields();
      const selected = this.utilitiesService.pick(t, vkeys);

      return this.utilitiesService.objectValues(selected);
    }
  }

  getSchoolYears(): string[] {
    return this.academicYearsService.getAcademicYears();
  }

  importTemplate(importTemplateContent): void {
    this.ngbModal.open(importTemplateContent, { centered: true }).result.then(
      (result) => {
      },
      (reason) => {
        console.log('AdministrationTemplateListComponent.importTemplate():  reason is ', reason);
      }
    );
  }

  setExcelTemplateUrl($event): void {
    if ($event) {
      if ($event.target.files && $event.target.files.length > 0) {
        this.selectedTemplateFile = $event.target.files;
      }
    }
  }

  setSelectedSchoolYear(year: string): void {
    this.selectedSchoolYear = year;
  }

  getTemplateTypes(): Object[] {
    return [
      { type: 'Invoice', label: 'Invoice'},
      { type: 'StudentInformation', label: 'Student Information' },
      { type: 'BulkInvoice', label: 'Bulk Invoice' }
    ];
  }

  setSelectedTemplateType(selectedType: Object): void {
    this.selectedTemplateType = selectedType;
  }

  doImport(): void {
    if (this.selectedTemplateFile) {
      const formData = new FormData();
      formData.append('content', this.selectedTemplateFile[0], this.selectedTemplateFile[0].name);
      formData.append('type', this.selectedTemplateType.type);
      formData.append('year', this.selectedSchoolYear.replace(/\s+/g, ''));
      this.templatesService.putTemplatesByTypeAndByYear(formData).subscribe(
        data => {
          console.log('ApplicationTemplateListComponent.doImport():  data is ', data['template']);
          this.refreshTemplateList();
        },
        error => {
          console.log('ApplicationTemplateListComponent.doImport():  error is ', error);
        }
      );
    }
  }
}
