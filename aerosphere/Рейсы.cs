using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace aerosphere
{

    public partial class Рейсы : Form
    {
        DataBase database = new DataBase();
        private DataTable dataTable;
        private SqlDataAdapter adapter;
        private DataTable viewTable;
        private SqlDataAdapter viewAdapter;

        public Рейсы()
        {
            InitializeComponent();
            LoadFlightsToDgv();
            UserAdmin();
        }

        private void UserAdmin()
        {
            if (Session.Admin == true)
            {
                аэропортыToolStripMenuItem.Visible = true;
                самолетыToolStripMenuItem.Visible = true;
                пользователиToolStripMenuItem.Visible = true;
                saveBtn.Visible = true;
                dataGridView2.Visible = true;
                // Загружаем представление во второй DataGridView
                LoadViewToDgv2();
            }
            else if (Session.Admin == false) 
            {
                аэропортыToolStripMenuItem.Visible = false;
                самолетыToolStripMenuItem.Visible = false;
                пользователиToolStripMenuItem.Visible = false;
                saveBtn.Visible = false;
                dataGridView2.Visible = false;
                dataGridView1.Size = new Size(776, 382);
            }
        }

        private void LoadFlightsToDgv()
        {
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    // Для админа загружаем полную таблицу, для пользователя - представление
                    string query = Session.Admin 
                        ? "SELECT * FROM [Рейсы]" 
                        : "SELECT * FROM [РейсыПредставление]";

                    adapter = new SqlDataAdapter(query, connection);
                    
                    // Создаем команды для обновления данных только для админа
                    if (Session.Admin)
                    {
                        SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                        adapter.InsertCommand = commandBuilder.GetInsertCommand();
                        adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                        adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                    }

                    dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.ReadOnly = !Session.Admin;
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка SQL: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Общая ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadViewToDgv2()
        {
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM [РейсыПредставление]";
                    viewAdapter = new SqlDataAdapter(query, connection);
                    viewTable = new DataTable();
                    viewAdapter.Fill(viewTable);
                    dataGridView2.AutoGenerateColumns = true;
                    dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView2.ReadOnly = true; // Представление всегда только для чтения
                    dataGridView2.DataSource = viewTable;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка SQL: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Общая ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Рейсы_Load(object sender, EventArgs e)
        {
            LoadFlightsToDgv();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    string query = $"SELECT * FROM [Рейсы]";
                    // Инициализируем адаптер
                    adapter = new SqlDataAdapter(query, connection);

                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                    adapter.InsertCommand = commandBuilder.GetInsertCommand();
                    adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                    adapter.DeleteCommand = commandBuilder.GetDeleteCommand();


                    adapter.Update(dataTable);
                    MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Перезагружаем данные для отображения актуальной информации
                    LoadFlightsToDgv();
                    LoadViewToDgv2();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Переход на другие страницы
        private void бронированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Бронирование бронирование = new Бронирование();
            бронирование.ShowDialog();
            this.Close();
        }

        private void аэропортыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Аэропорты аэропорты = new Аэропорты();
            аэропорты.ShowDialog();
            this.Close();
        }

        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Пользователи пользователи = new Пользователи();
            пользователи.ShowDialog();
            this.Close();

        }

        private void самолетыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Самолеты самолеты = new Самолеты();
            самолеты.ShowDialog();
            this.Close();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            string RaiceNumb = textBox1.Text.Trim();
            try
            {
                if (string.IsNullOrEmpty(RaiceNumb))
                {
                    // Если поисковый запрос пустой, показываем все данные
                    dataTable.DefaultView.RowFilter = "";
                }
                else
                {
                    // Фильтруем существующую таблицу
                    dataTable.DefaultView.RowFilter = $"[Номер рейса] LIKE '%{RaiceNumb}%'";
                }

                // Обновляем DataGridView
                dataGridView1.DataSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Общая ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadFlightsToDgv();
            LoadViewToDgv2();
        }
    }
}
