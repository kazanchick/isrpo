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
using System.Xml.Linq;

namespace aerosphere
{
    public partial class Авторизация : Form
    {
        DataBase database = new DataBase();
        public Авторизация()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Text.Trim();

            try
            {
                using (SqlConnection connection = database.GetConnection())
                using (SqlCommand command = new SqlCommand("VerifyUserLogin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Session.UserId = reader.GetInt32(0);
                            Session.Admin = reader.GetBoolean(5);
                            this.Hide();
                            Рейсы mainForm = new Рейсы();
                            mainForm.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Неверный email или пароль.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка входа: " + ex.Message);
            }
            

        }
   
        private void RegBtn_Click(object sender, EventArgs e)
        {
            Регистрация регистрация = new Регистрация();
            this.Hide();
            регистрация.ShowDialog();
            this.Close();
        }
    }

}

