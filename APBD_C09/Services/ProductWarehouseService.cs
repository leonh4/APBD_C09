using System.Data.SqlClient;
using APBD_C09.Models.DTOs;

namespace APBD_C09.Services;

public class ProductWarehouseService : IProductWarehouseService
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd9;Integrated Security=True;";
    
    public async Task<bool> HasOrderBeenRealisedAsync(int idProduct)
    {
        string command = "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @idOrder";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@idOrder", idProduct);
            await conn.OpenAsync();
            
            var obj = await cmd.ExecuteScalarAsync();
            return obj != null;
        }    
    }

    public async Task<int> InsertOrderAsync(ProductWarehouseDTO productWarehouseDto)
    {
        // SRP violation 
        string getOrderIdCommand = "SELECT IdOrder FROM \"Order\" WHERE IdProduct = @idProduct AND Amount = @amount AND CreatedAt < @createdAt";
        int orderId = 0;

        string getPriceCommand = "SELECT Price * @amount FROM Product WHERE IdProduct = @idProduct";
        decimal price = 0;

        string commandInsert = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) 
                                 VALUES (@idWarehouse, @idProduct, @idOrder, @amount, @price, @createdAt)
                                 SELECT CAST(SCOPE_IDENTITY() AS int)";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            
            using (var tran = (SqlTransaction) await conn.BeginTransactionAsync())
            {   
                // Getting ID Order - need for INSERT
                using (SqlCommand cmdHelpOrderId = new SqlCommand(getOrderIdCommand, conn, tran))
                {
                    cmdHelpOrderId.Parameters.AddWithValue("@idProduct", productWarehouseDto.IdProduct);
                    cmdHelpOrderId.Parameters.AddWithValue("@amount", productWarehouseDto.Amount);
                    cmdHelpOrderId.Parameters.AddWithValue("@createdAt", productWarehouseDto.CreatedAt);
                    
                    var obj = await cmdHelpOrderId.ExecuteScalarAsync();
                    if (obj == null)
                    {
                        tran.Rollback();
                        throw new Exception();
                    }
                    orderId = Convert.ToInt32(obj);
                }
                
                // Getting Price - needed for INSERT
                using (SqlCommand cmdHelpPrice = new SqlCommand(getPriceCommand, conn, tran))
                {
                    cmdHelpPrice.Parameters.AddWithValue("@idProduct", productWarehouseDto.IdProduct);
                    cmdHelpPrice.Parameters.AddWithValue("@amount", productWarehouseDto.Amount);
                    
                   var obj = await cmdHelpPrice.ExecuteScalarAsync();
                   if (obj == null)
                   {
                       tran.Rollback();
                       throw new Exception();
                   }
                   price = Convert.ToInt32(obj);
                }

                using (SqlCommand cmdInsert = new SqlCommand(commandInsert, conn, tran))
                {
                    cmdInsert.Parameters.AddWithValue("@idWarehouse", productWarehouseDto.IdWarehouse);
                    cmdInsert.Parameters.AddWithValue("@idProduct", productWarehouseDto.IdProduct);
                    cmdInsert.Parameters.AddWithValue("@idOrder", orderId);
                    cmdInsert.Parameters.AddWithValue("@amount", productWarehouseDto.Amount);
                    cmdInsert.Parameters.AddWithValue("@price", price);
                    cmdInsert.Parameters.AddWithValue("@createdAt", DateTime.Now);

                    try
                    {
                        var obj = await cmdInsert.ExecuteScalarAsync();
                        await tran.CommitAsync();
                        return Convert.ToInt32(obj);
                    }
                    catch (Exception ex)
                    {
                        await tran.RollbackAsync();
                        throw;
                    }
                }
            }
        }
    }
}