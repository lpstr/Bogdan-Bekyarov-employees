namespace EmployeesCollab.Models
{
    public class PairCollabDTO
    {
        public int Employee1Id { get;set; }
        public int Employee2Id { get;set;} 
        public List<ProjectDetails> Projects { get; set; }
        public double TotalTimeTogether { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
    public class ProjectDetails
    {
        public int Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public double TotalDays { get; set; }
    }
}
