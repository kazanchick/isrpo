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

namespace aerosphere
{

    public partial class Самолеты : Form
    {
        DataBase database = new DataBase();
        private DataTable dataTable;
        private SqlDataAdapter adapter;
        public Самолеты()
        {
            InitializeComponent();
        }

        private void LoadAircraftToDgv()
        {
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    string query = $"SELECT * FROM [Самолеты]";

                    // Инициализируем адаптер
                    adapter = new SqlDataAdapter(query, connection);

                    // Создаем команды для обновления данных

                    // Инициализируем таблицу данных
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Настраиваем DataGridView
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.ReadOnly = !Session.Admin; // Разрешаем редактирование только для админа
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

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    string query = $"SELECT * FROM [Самолеты]";
                    // Инициализируем адаптер
                    adapter = new SqlDataAdapter(query, connection);

                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                    adapter.InsertCommand = commandBuilder.GetInsertCommand();
                    adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                    adapter.DeleteCommand = commandBuilder.GetDeleteCommand();


                    adapter.Update(dataTable);
                    MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Перезагружаем данные для отображения актуальной информации
                    LoadAircraftToDgv();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Самолеты_Load(object sender, EventArgs e)
        {
            LoadAircraftToDgv();
        }

        #region Переход на другие страницы
        private void аэропортыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Аэропорты аэропорты = new Аэропорты();
            аэропорты.ShowDialog();
            this.Close();
        }

        private void бронированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Бронирование бронирование = new Бронирование();
            бронирование.ShowDialog();
            this.Close();
        }

        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Пользователи пользователи = new Пользователи();
            пользователи.ShowDialog();
            this.Close();

        }

        private void рейсыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Рейсы рейсы = new Рейсы();
            рейсы.ShowDialog();
            this.Close();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            string AircraftName = textBox1.Text.Trim();

            try
            {
                if (string.IsNullOrEmpty(AircraftName))
                {
                    // Если поисковый запрос пустой, показываем все данные
                    dataTable.DefaultView.RowFilter = "";
                }
                else
                {
                    // Фильтруем существующую таблицу
                    dataTable.DefaultView.RowFilter = $"Модель LIKE '%{AircraftName}%'";
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
            LoadAircraftToDgv();
        }
    }
}
