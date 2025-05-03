using System.Data.SqlClient;

namespace APBD_C09.Services;

public class WarehouseService : IWarehouseService
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd9;Integrated Security=True;";
    
    public async Task<bool> DoesWarehouseExistAsync(int warehouseId)
    {
        string command = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @id";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@id", warehouseId);
            await conn.OpenAsync();
            var obj = await cmd.ExecuteScalarAsync();
            return obj != null;
        }    
    }
}