using System.Data.SqlClient;

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

    public async Task<int> InsertOrderAsync(int idWarehouse, int idProduct, int idOrder, int amount)
    {
        
        string commandPrice = "SELECT Price * @amount FROM Product WHERE IdProduct = @idProduct";
        decimal price = 0;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(commandPrice, conn))
        {
            cmd.Parameters.AddWithValue("@idProduct", idProduct);
            cmd.Parameters.AddWithValue("@amount", amount);
            
            await conn.OpenAsync();
            var obj = await cmd.ExecuteScalarAsync();
            price = Convert.ToDecimal(obj);
        }
        
        string commandInsert = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) 
                                 VALUES (@idWarehouse, @idProduct, @idOrder, @amount, @price, @createdAt)
                                 SELECT CAST(SCOPE_IDENTITY() AS int)";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            
            using (var tran = (SqlTransaction)await conn.BeginTransactionAsync())
            using (SqlCommand cmd = new SqlCommand(commandInsert, conn, tran))
            {
                cmd.Parameters.AddWithValue("@idWarehouse", idWarehouse);
                cmd.Parameters.AddWithValue("@idProduct", idProduct);
                cmd.Parameters.AddWithValue("@idOrder", idOrder);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

                
                try
                {
                    var obj = await cmd.ExecuteScalarAsync();
                    await tran.CommitAsync();
                    return Convert.ToInt32(obj);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}