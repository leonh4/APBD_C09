using System.Data;
using System.Data.SqlClient;
using APBD_C09.Models.DTOs;

namespace APBD_C09.Services;

public class DBService : IDBService
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd9;Integrated Security=True;";
    
    public async Task<int> AddProductToWarehouseAsync(ProductWarehouseDTO productWarehouse)
    {
        string command = "AddProductToWarehouse";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            
            cmd.CommandType = CommandType.StoredProcedure;
            
            cmd.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            cmd.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            cmd.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            cmd.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
            
            var obj = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(obj);
        }
    }
}