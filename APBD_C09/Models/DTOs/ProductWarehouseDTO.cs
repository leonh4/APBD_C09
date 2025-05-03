using System.ComponentModel.DataAnnotations;

namespace APBD_C09.Models.DTOs;

public class ProductWarehouseDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "IdProduct must be greater than 0")]
    public int IdProduct { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "IdWarehouse must be greater than 0")]
    public int IdWarehouse { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Ammount must be greater than 0")]
    public int Amount { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
}