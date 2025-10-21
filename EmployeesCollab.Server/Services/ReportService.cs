using CsvHelper;
using CsvHelper.Configuration;
using EmployeesCollab.Contracts;
using EmployeesCollab.Models;
using System.Globalization;

namespace EmployeesCollab.Services
{
    public class ReportService : IReportService
    {
        public async Task<PairCollabDTO> FindPairCollab(FileDataDTO data)
        {
            var result = new PairCollabDTO();
            var fileRecords = LoadRecordsFromCsv(data.FileContent);

            var dataErrors = ValidateContent(fileRecords, data.DateFormat);

            if (!dataErrors.Any())
            {
                result = GetLongestCollab(fileRecords);
            }
            else
            {
                result.Errors = dataErrors;
            }

            return result;
        }


        private PairCollabDTO GetLongestCollab(List<FileRecordDTO> items)
        {
            List<PairCollabDTO> totalCollabs = new List<PairCollabDTO>();
            List<ReportRecordDTO> mappedRecords = items.Select(p => new ReportRecordDTO
            {
                EmpId = p.EmpId,
                ProjectId = p.ProjectId,
                DateFrom = DateTime.Parse(p.DateFrom),
                DateTo =
                (!string.IsNullOrEmpty(p.DateTo) ?
                                    (p.DateTo.Trim().ToLower() != "null" ?
                                                DateTime.Parse(p.DateTo) : DateTime.Now.Date) : DateTime.Now.Date)
            }).ToList();

            var groupedRecords = items.GroupBy(p => p.EmpId).ToList();

            for (int i = 0; i < mappedRecords.Count; i++)
            {
                for (int j = i + 1; j < mappedRecords.Count; j++)
                {
                    var empA = mappedRecords[i];
                    var empB = mappedRecords[j];

                    if (empA.ProjectId == empB.ProjectId)
                    {
                        var totalCollab = GetOverlappingDays(empA.DateFrom.Value, empA.DateTo.Value, empB.DateFrom.Value, empB.DateTo.Value);

                        if (totalCollab != null)
                        {
                            totalCollab.Id = empA.ProjectId;

                            var exists = totalCollabs.Where(p =>
                            (p.Employee1Id == empA.EmpId && p.Employee2Id == empB.EmpId)
                            || (p.Employee1Id == empB.EmpId && p.Employee2Id == empA.EmpId)).FirstOrDefault();

                            if (exists != null)
                            {
                                exists.Projects.Add(totalCollab);
                                exists.TotalTimeTogether += totalCollab.TotalDays;
                            }
                            else
                            {
                                PairCollabDTO newCollab = new PairCollabDTO
                                {
                                    Employee1Id = empA.EmpId,
                                    Employee2Id = empB.EmpId,
                                    Projects = new List<ProjectDetails> { totalCollab },
                                    TotalTimeTogether = totalCollab.TotalDays
                                };
                                totalCollabs.Add(newCollab);
                            }
                        }
                    }
                }
            }

            return totalCollabs.OrderByDescending(p => p.TotalTimeTogether).FirstOrDefault();
        }

        #region File service helper

        private List<FileRecordDTO> LoadRecordsFromCsv(IFormFile fileContent)
        {
            var result = new List<FileRecordDTO>();

            using (var reader = new StreamReader(fileContent.OpenReadStream()))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HasHeaderRecord = false };
                using var csv = new CsvReader(reader, config);
                {
                    result = csv.GetRecords<FileRecordDTO>().ToList();
                }
            }

            return result;
        }

        #endregion

        #region Date helpers and validators

        private ProjectDetails GetOverlappingDays(DateTime firstStart, DateTime firstEnd, DateTime secondStart, DateTime secondEnd)
        {
            var result = new ProjectDetails();

            DateTime maxStart = firstStart > secondStart ? firstStart : secondStart;
            DateTime minEnd = firstEnd < secondEnd ? firstEnd : secondEnd;
            TimeSpan interval = minEnd - maxStart;
            double returnValue = interval > TimeSpan.FromSeconds(0) ? interval.TotalDays : 0;

            if (returnValue <= 0)
                return null;

            result.TotalDays = returnValue;
            result.DateFrom = maxStart;
            result.DateTo = minEnd;

            return result;
        }
        private List<string> ValidateContent(List<FileRecordDTO> records, string? dateFormat)
        {
            if(!string.IsNullOrEmpty(dateFormat) && dateFormat == "Any")
            {
                dateFormat = null;
            }
            List<string> errors = new List<string>();

            for (int i = 0; i < records.Count; i++)
            {
                var item = records[i];
                if (!ValidateDate(item.DateFrom, dateFormat))
                {
                    errors.Add(string.Format("Invalid [Date From] on row {0}", (i + 1).ToString()));
                }
                if (!string.IsNullOrEmpty(item.DateTo) && item.DateTo.Trim() != "NULL")
                {
                    if (!ValidateDate(item.DateTo, dateFormat))
                    {
                        errors.Add(string.Format("Invalid [Date To] on row {0}", (i + 1).ToString()));
                    }
                }
            }

            return errors;
        }

        private bool ValidateDate(string? date, string? dateFormat)
        {
            if (string.IsNullOrEmpty(dateFormat) || Settings.AllowedDateFormats.Any(p => p.Equals(dateFormat)))
            {
                string[] formatsToUse = !string.IsNullOrEmpty(dateFormat) ? (new string[] { dateFormat }) : (Settings.AllowedDateFormats);

                DateTime dateValue;
                if (DateTime.TryParseExact(date, formatsToUse, null, DateTimeStyles.None, out dateValue))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
