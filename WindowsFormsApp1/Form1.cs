using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace StorageApp {
    public partial class Form1 : Form {
        private Storage storage = new Storage();
        private int selectedItemId = -1;

        public Form1() {
            InitializeComponent();
            LoadDatabases();
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
        }

        private void LoadDatabases() {
            cmbDatabase.Items.Clear();
            foreach (var dbFile in Directory.GetFiles(".", "*.db")) {
                cmbDatabase.Items.Add(Path.GetFileName(dbFile));
            }
        }

        private void LoadData() {
            DataTable dataTable = storage.GetItemsAsDataTable();
            dataGridView.DataSource = dataTable;
            dataGridView.Columns["Номер"].HeaderText = "Номер";
            dataGridView.Columns["Имя"].HeaderText = "Имя";
            dataGridView.Columns["Количество"].HeaderText = "Количество";
        }

        private void btnSearchItem_Click(object sender, EventArgs e) {
            string name = txtItemName.Text;
            DataTable dataTable = storage.SearchItemsAsDataTable(name);
            dataGridView.DataSource = dataTable;
            dataGridView.Columns["Номер"].HeaderText = "Номер";
            dataGridView.Columns["Имя"].HeaderText = "Имя";
            dataGridView.Columns["Количество"].HeaderText = "Количество";
        }



        private void btnAddItem_Click(object sender, EventArgs e) {
            string name = txtItemName.Text;
            int quantity = (int)numericQuantity.Value;
            Item item = new Item(name, quantity);
            storage.AddItem(item);
            LoadData();
            ClearInputs();
        }


        private void btnRemoveItem_Click(object sender, EventArgs e) {
            if (dataGridView.SelectedRows.Count > 0) {
                // Получаем идентификатор выбранного элемента
                int id = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Номер"].Value);

                // Вызываем метод удаления элемента
                storage.DeleteItem(id);

                // Обновляем данные на форме
                LoadData();
            } else {
                MessageBox.Show("Пожалуйста, выберите элемент для удаления.");
            }
        }


        private void btnAddDatabase_Click(object sender, EventArgs e) {
            string dbName = Prompt.ShowDialog("Enter database name:", "Add Database");
            if (!string.IsNullOrWhiteSpace(dbName)) {
                dbName = $"{dbName}.db";
                if (!File.Exists(dbName)) {
                    SQLiteConnection.CreateFile(dbName);
                    LoadDatabases();
                    MessageBox.Show($"Database {dbName} created successfully.");
                } else {
                    MessageBox.Show("Database already exists.");
                }
            }
        }

        private void btnDeleteDatabase_Click(object sender, EventArgs e) {
            // Закрываем соединение с базой данных
            storage.CloseConnection();

            try {
                // Получаем путь к базе данных из класса Storage
                string databasePath = storage.GetDatabasePath();

                if (File.Exists(databasePath)) {
                    File.Delete(databasePath);
                    MessageBox.Show("База данных успешно удалена.");
                } else {
                    MessageBox.Show("Файл базы данных не найден.");
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error deleting database: {ex.Message}");
            }
        }



        private void cmbDatabase_SelectedIndexChanged(object sender, EventArgs e) {
            if (cmbDatabase.SelectedItem != null) {
                string selectedDatabase = cmbDatabase.SelectedItem.ToString();
                storage.SetDatabase(selectedDatabase);
                LoadData();
            }
        }

        // Пример обработки события выбора строки в DataGridView
        private void dataGridView_SelectionChanged(object sender, EventArgs e) {
            if (dataGridView.SelectedRows.Count > 0) {
                DataGridViewRow selectedRow = dataGridView.SelectedRows[0];
                txtItemName.Text = selectedRow.Cells["Имя"].Value.ToString();
                numericQuantity.Value = Convert.ToInt32(selectedRow.Cells["Количество"].Value);
            }
        }

        // Пример обработки события нажатия кнопки "Редактировать элемент"
        private void btnUpdateItem_Click(object sender, EventArgs e) {
            if (dataGridView.SelectedRows.Count > 0) {
                int id = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Номер"].Value);
                string name = txtItemName.Text;
                int quantity = (int)numericQuantity.Value;
                storage.UpdateItem(id, name, quantity);
                LoadData();
            }
        }


        private void ClearInputs() {
            txtItemName.Clear();
            numericQuantity.Value = 0;
            selectedItemId = -1;
        }
    }

    public static class Prompt {
        public static string ShowDialog(string text, string caption) {
            Form prompt = new Form() {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
