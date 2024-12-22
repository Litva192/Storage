using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace StorageApp {
    public class Storage {
        private string connectionString;
        private SQLiteConnection connection;

        public Storage() {
            // Получаем путь к папке с исполняемым файлом программы
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // Определяем относительный путь к файлу базы данных
            string databasePath = Path.Combine(baseDir, "storage_database.sqlite");
            connectionString = $"Data Source={databasePath};Version=3;";
            connection = new SQLiteConnection(connectionString);
        }

        public void OpenConnection() {
            if (connection.State == ConnectionState.Closed) {
                connection.Open();
            }
        }

        public void CloseConnection() {
            if (connection.State == ConnectionState.Open) {
                connection.Close();
            }
        }

        public string GetDatabasePath() {
            return connectionString.Split('=')[1].Split(';')[0];
        }

        public void DeleteItem(int id) {
            try {
                OpenConnection();
                string query = "DELETE FROM Items WHERE Id = @Id";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                MessageBox.Show($"Error deleting item: {ex.Message}");
            }
            finally {
                CloseConnection();
            }
        }

    public void SetDatabase(string databaseName) {
            connectionString = $"Data Source={databaseName};Version=3;";
            CreateDatabase();
        }

        private void CreateDatabase() {
            try {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString)) {
                    connection.Open();
                    string createTableQuery = "CREATE TABLE IF NOT EXISTS Items (Id INTEGER PRIMARY KEY, Name TEXT, Quantity INTEGER)";
                    SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
                    command.ExecuteNonQuery();
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error creating database: {ex.Message}");
            }
        }


        public DataTable GetItemsAsDataTable() {
            DataTable dataTable = new DataTable();
            try {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString)) {
                    connection.Open();
                    string query = "SELECT Id AS 'Номер', Name AS 'Имя', Quantity AS 'Количество' FROM Items";
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error loading items: {ex.Message}");
            }
            return dataTable;
        }

        public DataTable SearchItemsAsDataTable(string name) {
            DataTable dataTable = new DataTable();
            try {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString)) {
                    connection.Open();
                    string query = "SELECT Id AS 'Номер', Name AS 'Имя', Quantity AS 'Количество' FROM Items WHERE Name LIKE @Name";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", "%" + name + "%");
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    adapter.Fill(dataTable);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error searching items: {ex.Message}");
            }
            return dataTable;
        }


        public void AddItem(Item item) {
            try {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString)) {
                    connection.Open();
                    string query = "INSERT INTO Items (Name, Quantity) VALUES (@Name, @Quantity)";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", item.Name);
                    command.Parameters.AddWithValue("@Quantity", item.Quantity);
                    command.ExecuteNonQuery();
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error adding item: {ex.Message}");
            }
        }



        public void UpdateItem(int id, string name, int quantity) {
            try {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString)) {
                    connection.Open();
                    string query = "UPDATE Items SET Name = @Name, Quantity = @Quantity WHERE Id = @Id";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error updating item: {ex.Message}");
            }
        }



        public void ClearAllPoolsAndCloseConnections() {
            try {
                SQLiteConnection.ClearAllPools();
            } catch (Exception ex) {
                MessageBox.Show($"Error clearing connection pools: {ex.Message}");
            }
        }

        public void DeleteDatabase(string databaseName) {
            try {
                ClearAllPoolsAndCloseConnections();
                if (File.Exists(databaseName)) {
                    File.Delete(databaseName);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error deleting database: {ex.Message}");
            }
        }
    }
}
