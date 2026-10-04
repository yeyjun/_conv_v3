using System;
using MySql.Data.Client;

public class FreshData
{
	public FreshData()
	{
	}
    public async static void GetData()
    {
        using (var connection = new MySqlConnection("your_connection_string"))
        {
            await connection.OpenAsync();
            using (var command = new MySqlCommand("SELECT * FROM your_table", connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader["your_column_name"]);
                    }
                }
            }
        }
    }
}
