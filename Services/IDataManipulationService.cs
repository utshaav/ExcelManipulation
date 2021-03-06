using ExcelManipulation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcelManipulation.Services;
public interface IDataManipulationService
{
    ExcelParseResult ParseExcel(IFormFile file, string userId);
    public ExcelParseResult ParseCsv(IFormFile file, string userId);
    Export ExcelExport(List<string> row, List<string> column);
    Export CsvExport(List<string> row, List<string> column);
    Task<Export> PdfExportAsync(List<string> row, List<string> column, HttpContext httpCtx);
    public Task<List<SelectListItem>> ImporterDD();


}