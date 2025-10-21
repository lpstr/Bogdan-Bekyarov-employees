using EmployeesCollab.Models;
using System.Runtime.CompilerServices;

namespace EmployeesCollab.Contracts
{
    public interface IReportService
    {
        Task<PairCollabDTO> FindPairCollab(FileDataDTO data);
    }
}
