using APBD_C09.Models.DTOs;

namespace APBD_C09.Services;

public interface IDBService
{ 
    Task<int> AddProductToWarehouseAsync(ProductWarehouseDTO productWarehouse);
}