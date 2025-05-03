namespace APBD_C09.Services;

public interface IProductWarehouseService
{
    Task<bool> HasOrderBeenRealisedAsync(int idProduct);
    
    Task<int> InsertOrderAsync(int idWarehouse, int idProduct, int idOrder, int amount);
}