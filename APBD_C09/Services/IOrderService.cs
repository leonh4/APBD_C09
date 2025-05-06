namespace APBD_C09.Services;

public interface IOrderService
{
    Task<bool> HasProductBeenOrderedAsync(int idProduct, int amount, DateTime createdAt);

    Task UpdateFulfilledAtAsync(int idProduct);
}