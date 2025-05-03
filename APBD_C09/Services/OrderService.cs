using System.Data.SqlClient;

namespace APBD_C09.Services;

public class OrderService : IOrderService
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd9;Integrated Security=True;";
    public async Task<bool> HasProductBeenOrderedAsync(int idProduct, int amount, DateTime createdAt)
    {
        string command = "SELECT 1 FROM \"Order\" WHERE IdProduct = @idProduct AND Amount = @amount AND CreatedAt < @createdAt";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@idProduct", idProduct);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@createdAt", createdAt);
            
            await conn.OpenAsync();
            var obj = await cmd.ExecuteScalarAsync();
            return obj != null;
        }    
    }

    public async Task UpdateFulfilledAtAsync(int idProduct)
    {
        string command = "UPDATE \"Order\" SET FulfilledAt = @FulfilledAt WHERE IdProduct = @idProduct";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (var tran = (SqlTransaction)await conn.BeginTransactionAsync())
            using (SqlCommand cmd = new SqlCommand(command, conn, tran))
            {
                cmd.Parameters.AddWithValue("@idProduct", idProduct);
                cmd.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    await tran.CommitAsync();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }

    public async Task<int> GetOrderIdAsync(int idProduct, int amount, DateTime createdAt)
    {
        string command = "SELECT IdOrder FROM \"Order\" WHERE IdProduct = @idProduct AND Amount = @amount AND CreatedAt < @createdAt";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@idProduct", idProduct);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@createdAt", createdAt);
            
            await conn.OpenAsync();
            var obj = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(obj);
        }   
    }
}