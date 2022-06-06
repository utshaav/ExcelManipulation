#nullable disable
namespace ExcelManipulation.Models;

public class Export{
    public bool Success { get; set; }
    public byte[] File {get;set;}
    public string Extension { get; set; }
}