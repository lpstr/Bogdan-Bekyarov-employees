import { Component } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

interface ProjectDetails {
  id: number;
  dateFrom: string;
  dateTo: string;
  totalDays: number;
}

interface PairCollabDTO {
  employee1Id: number;
  employee2Id: number;
  projects: ProjectDetails[];
  totalTimeTogether: number;
  errors: string[];
}

@Component({
  selector: 'app-report',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    BrowserAnimationsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatTableModule
  ],
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css']
})
export class ReportComponent {

  file!: File;
  selectedDateFormat: string = '';
  dateFormats = ['Any', 'yyyy-MM-dd', 'MM/dd/yyyy', 'dd-MM-yyyy', 'dd/MM/yyyy', 'yyyy/MM/dd', 'yyyyMMdd', 'dd.MM.yyyy', 'M/d/yyyy'];

  reportData: PairCollabDTO = {
  employee1Id: 0,
  employee2Id: 0,
  projects: [],
  totalTimeTogether: 0,
  errors: []
};
showError: boolean = false;
  tableData: any[] = [];
  displayedColumns: string[] = ['employee1', 'employee2', 'projectId', 'daysWorked'];

  constructor(private http: HttpClient) {}

  // File selection
  onFileSelected(event: any) {
    const selected = event.target.files[0];
    if (selected) {
      this.file = selected;
    }
  }

  // Submit to API
  getReport() {
    if (!this.file) return;

    const formData = new FormData();
    formData.append('FileContent', this.file);
    formData.append('DateFormat', this.selectedDateFormat);

    this.http.post<PairCollabDTO>('https://localhost:7113/api/Reports', formData)
      .subscribe({
        next: (response) => {
          this.reportData = response;
          this.prepareTableData();
        },
        error: (error: HttpErrorResponse) => {
          console.error('Error:', error.message);
          alert('There was an error fetching the report.');
        }
      });
  }

  prepareTableData() {
    if(this.reportData.errors.length > 0){
      this.showError = true;
    }
    this.tableData = [];
      for (const project of this.reportData.projects) {
        this.tableData.push({
          employee1Id: this.reportData.employee1Id,
          employee2Id: this.reportData.employee2Id,
          projectId: project.id,
          daysWorked: project.totalDays
        });
      }
  }
}
