using ExcelManipulation.Models;

namespace ExcelManipulation.Services;
public interface IDataManipulationService
{
    ExcelParseResult ParseExcel(IFormFile file, string userId);
    public ExcelParseResult ParseCsv(IFormFile file, string userId);
    Export ExcelExport(List<string> row, List<string> column);
    Export CsvExport(List<string> row, List<string> column);
    Export PdfExport(List<string> row, List<string> column);


}