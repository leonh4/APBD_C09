namespace APBD_C09.Services;

public interface IWarehouseService
{
    Task<bool> DoesWarehouseExistAsync(int warehouseId);
}