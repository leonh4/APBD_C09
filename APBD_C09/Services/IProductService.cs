namespace APBD_C09.Services;

public interface IProductService
{
    Task<bool> DoesProductExistAsync(int id);
}