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
    public partial class Регистрация : Form
    {
        DataBase database = new DataBase();

        public Регистрация()
        {
            InitializeComponent();
            RegButton.Click += RegButton_Click;
        }

        private void RegButton_Click(object sender, EventArgs e)
        {
            string name = NameBox.Text.Trim();
            string lastName = LastNameBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Text.Trim();

            using (SqlConnection connection = database.GetConnection())
            {
                using (SqlCommand command = new SqlCommand("RegisterUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Добавляем параметры
                    command.Parameters.AddWithValue("@Имя", name);
                    command.Parameters.AddWithValue("@Фамилия", lastName);
                    command.Parameters.AddWithValue("@Номер_телефона", phone);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Пароль", password);
                    command.Parameters.AddWithValue("@Администратор", false);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Регистрация успешно завершена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        Авторизация loginForm = new Авторизация();
                        loginForm.ShowDialog();
                        this.Close();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Ошибка при регистрации: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void label7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Авторизация loginForm = new Авторизация();
            loginForm.ShowDialog();
            this.Close();
        }
    }
}
