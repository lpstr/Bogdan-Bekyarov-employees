
namespace EmployeesCollab.Models
{
    public class ReportRecordDTO
    {
        public ReportRecordDTO() { }
        public ReportRecordDTO(int empId, int projectId, DateTime? dateFrom, DateTime? dateTo)
        {
            EmpId = empId;
            ProjectId = projectId;
            DateFrom = dateFrom;
            DateTo = dateTo;
        }
        public int EmpId { get; set; }
        public int ProjectId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    } 
    public class FileRecordDTO
    {
        public int EmpId { get; set; }
        public int ProjectId { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
    }
}
