using System.Data.SqlClient;

using var connection = new SqlConnection(@"data source=.\SQLEXPRESS;initial catalog=GuidsDb;trusted_connection=true");
connection.Open();

try {
    var cmdClean = new SqlCommand("DROP TABLE Guids", connection);
    cmdClean.ExecuteNonQuery();
} catch (Exception) { }

var cmdCreate = new SqlCommand("CREATE TABLE Guids (Id uniqueidentifier PRIMARY KEY, InsertOrder int);", connection);
cmdCreate.ExecuteNonQuery();

for (int i = 1; i <= 25; i++) {
    var cmdInsert = new SqlCommand("INSERT INTO Guids (Id, InsertOrder) VALUES (@Id, @InsertOrder)", connection);
    cmdInsert.Parameters.AddWithValue("@Id", Medo.Uuid7.NewMsSqlUniqueIdentifier());
    cmdInsert.Parameters.AddWithValue("@InsertOrder", i);
    cmdInsert.ExecuteNonQuery();
}

var cmdRead = new SqlCommand("SELECT Id, InsertOrder FROM Guids", connection);
using var reader = cmdRead.ExecuteReader();
while (reader.Read()) {
    Console.WriteLine(String.Format("{0} {1}", reader[0], reader[1]));
}
Console.ReadLine();

connection.Close();
