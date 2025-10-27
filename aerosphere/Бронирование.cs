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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace aerosphere
{
    public partial class Бронирование : Form
    {
        DataBase database = new DataBase();
        private DataTable dataTable;
        private SqlDataAdapter adapter;
        public Бронирование()
        {
            InitializeComponent();
            UserAdmin();
        }

        private void UserAdmin()
        {
            if (Session.Admin == true)
            {
                аэропортыToolStripMenuItem.Visible = true;
                самолетыToolStripMenuItem.Visible = true;
                пользователиToolStripMenuItem.Visible = true;
               
            }
            else if (Session.Admin == false)
            {
                аэропортыToolStripMenuItem.Visible = false;
                самолетыToolStripMenuItem.Visible = false;
                пользователиToolStripMenuItem.Visible = false;
            }
        }

        private void LoadBooking()
        {
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    string query = $"SELECT * FROM [РейсыПредставление]";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        flightsDataGridView.AutoGenerateColumns = true;
                        flightsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                        flightsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        flightsDataGridView.ReadOnly = !Session.Admin;
                        flightsDataGridView.DataSource = dataTable;
                    }
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

        private void Бронирование_Load(object sender, EventArgs e)
        {
            LoadBooking();
            LoadUserBooking();
        }

        private void bookButton_Click(object sender, EventArgs e)
        {
            if (flightsDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите рейс для бронирования");
                return;
            }

            // Пример: временно задаем ID пользователя, позже заменишь на текущего пользователя из системы входа
            int userId = Session.UserId;

            int flightId = Convert.ToInt32(flightsDataGridView.SelectedRows[0].Cells["ID Рейса"].Value);
            decimal ticketPrice = Convert.ToDecimal(flightsDataGridView.SelectedRows[0].Cells["Базовая цена"].Value);

            try
            {
                using (SqlConnection conn = database.GetConnection())
                {
                    conn.Open();

                    // Проверка: есть ли места?
                    string seatQuery = "SELECT Мест FROM Рейсы WHERE [ID Рейса] = @FlightId";
                    using (SqlCommand seatCmd = new SqlCommand(seatQuery, conn))
                    {
                        seatCmd.Parameters.AddWithValue("@FlightId", flightId);
                        string seatStr = seatCmd.ExecuteScalar()?.ToString();
                        if (!int.TryParse(seatStr, out int availableSeats) || availableSeats <= 0)
                        {
                            MessageBox.Show("На выбранном рейсе нет доступных мест.");
                            return;
                        }
                    }

                    // Добавление брони
                    string insertQuery = @"INSERT INTO Бронирование ([ID Пользователя], [ID Рейса], [Дата бронирования], [Стоимость билета], [Статус]) 
                                   VALUES (@UserId, @FlightId, @Date, @Price, 'Активно')";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@UserId", userId);
                        insertCmd.Parameters.AddWithValue("@FlightId", flightId);
                        insertCmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        insertCmd.Parameters.AddWithValue("@Price", ticketPrice);

                        insertCmd.ExecuteNonQuery();
                    }

                    // Обновление мест в рейсе
                    string updateSeats = @"UPDATE Рейсы SET Мест = CAST(CAST(Мест AS int) - 1 AS nvarchar)
                                   WHERE [ID Рейса] = @FlightId";

                    using (SqlCommand updateCmd = new SqlCommand(updateSeats, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@FlightId", flightId);
                        updateCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Бронирование успешно выполнено!");
                    LoadBooking(); // Обновляем список рейсов
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при бронировании: " + ex.Message);
            }
        }

        private void LoadUserBooking ()
        {
            int userId = Session.UserId;

            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM БронированиеПредставление WHERE [Номер пользователя] = @UserId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);  // везде с маленькой "d"

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dataGridView1.AutoGenerateColumns = true;
                            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dataGridView1.ReadOnly = !Session.Admin;
                            dataGridView1.DataSource = dataTable;
                        }
                    }
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

        private void cancelBookingButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите бронирование для отмены.");
                return;
            }

            int bookingId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Номер бронирования"].Value);

            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    using (SqlCommand command = new SqlCommand("CancelBooking", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookingID", bookingId);

                        connection.Open();
                        command.ExecuteNonQuery();
                        
                        MessageBox.Show("Бронирование успешно отменено", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUserBooking(); // Обновляем список бронирований
                        LoadBooking(); // Обновляем список рейсов
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при отмене бронирования: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadUserBooking();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            LoadBooking();
        }


        #region Переход на другие страницы
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


        private void рейсыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Рейсы рейсы = new Рейсы();
            рейсы.ShowDialog();
            this.Close();
        }

        #endregion
        
    }

}
