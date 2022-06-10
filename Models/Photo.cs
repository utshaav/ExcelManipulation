#nullable disable
using System.ComponentModel.DataAnnotations.Schema;

namespace ExcelManipulation.Models;
public class Photo
 {
     public int Id { get; set; }
     public byte[] Bytes { get; set; }
     public string Description { get; set; }
     public string FileExtension { get; set; }
     public decimal Size { get; set; }
     public Guid EmployeeId { get; set; }
     [ForeignKey("EmployeeId")]
     public Employee employee { get; set; }
 }