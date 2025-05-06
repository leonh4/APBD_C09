using APBD_C09.Models.DTOs;

namespace APBD_C09.Services;

public interface IProductWarehouseService
{
    Task<bool> HasOrderBeenRealisedAsync(int idProduct);
    
    Task<int> InsertOrderAsync(ProductWarehouseDTO productWarehouseDto);
}