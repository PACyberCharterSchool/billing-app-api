export enum ReportType {
  Invoice = 'Invoice',
  StudentInformation = 'StudentInformation',
  BulkInvoice = 'BulkInvoice',
  TotalsOnly = 'TotalsOnlyInvoice',
  BulkStudentInformation = 'BulkStudentInformation',
  AccountsReceivableAsOf = 'AccountsReceivableAsOf',
  AccountsReceivableAging = 'AccountsReceivableAging',
  CSIU = 'Csiu',
  UniPayInvoiceSummary = 'UniPayInvoiceSummary'
}

class ReportMetadata {
  id: number;
  type: string;
  schoolYear: string;
  scope: string;
  name: string;
  approved: boolean;
  created: Date;
}

export class Report extends ReportMetadata {
  data: string;
  xlsx: Blob;
  pdf: Blob;
}
