using System.Data.SqlClient;

namespace APBD_C09.Services;

public class ProductService : IProductService
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd9;Integrated Security=True;";
    public async Task<bool> DoesProductExistAsync(int id)
    {
        string command = "SELECT 1 FROM Product WHERE IdProduct = @id";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            await conn.OpenAsync();
            var obj = await cmd.ExecuteScalarAsync();
            return obj != null;
        }    
    }
    
    
    
    
}