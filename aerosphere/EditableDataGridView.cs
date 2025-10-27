using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace aerosphere
{
    public class EditableDataGridView : DataGridView
    {
        protected DataBase database;
        protected string tableName;
        protected string primaryKeyColumn;
        protected bool isAdmin;

        public EditableDataGridView()
        {
            database = new DataBase();
            isAdmin = Session.Admin;
            this.AllowUserToAddRows = isAdmin;
            this.AllowUserToDeleteRows = isAdmin;
            this.AllowUserToOrderColumns = true;
            this.AllowUserToResizeColumns = true;
            this.AllowUserToResizeRows = true;
            this.MultiSelect = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.EditMode = DataGridViewEditMode.EditOnEnter;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ReadOnly = !isAdmin;

            this.CellValueChanged += EditableDataGridView_CellValueChanged;
            this.UserDeletingRow += EditableDataGridView_UserDeletingRow;
            this.UserAddedRow += EditableDataGridView_UserAddedRow;
        }

        protected virtual void EditableDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!isAdmin || e.RowIndex < 0 || e.ColumnIndex < 0) return;

            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var row = this.Rows[e.RowIndex];
                            var primaryKeyValue = row.Cells[primaryKeyColumn].Value;
                            var columnName = this.Columns[e.ColumnIndex].Name;
                            var newValue = row.Cells[e.ColumnIndex].Value;

                            string updateQuery = $"UPDATE {tableName} SET [{columnName}] = @NewValue WHERE [{primaryKeyColumn}] = @PrimaryKeyValue";
                            using (SqlCommand command = new SqlCommand(updateQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@NewValue", newValue ?? DBNull.Value);
                                command.Parameters.AddWithValue("@PrimaryKeyValue", primaryKeyValue);
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RefreshData();
            }
        }

        protected virtual void EditableDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!isAdmin) return;

            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var primaryKeyValue = e.Row.Cells[primaryKeyColumn].Value;
                            string deleteQuery = $"DELETE FROM {tableName} WHERE [{primaryKeyColumn}] = @PrimaryKeyValue";
                            using (SqlCommand command = new SqlCommand(deleteQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@PrimaryKeyValue", primaryKeyValue);
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                            RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                RefreshData();
            }
        }

        protected virtual void EditableDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (!isAdmin) return;

            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var row = e.Row;
                            string columns = string.Join(", ", this.Columns.Cast<DataGridViewColumn>()
                                .Where(c => c.Name != primaryKeyColumn)
                                .Select(c => $"[{c.Name}]"));
                            string parameters = string.Join(", ", this.Columns.Cast<DataGridViewColumn>()
                                .Where(c => c.Name != primaryKeyColumn)
                                .Select(c => $"@{c.Name}"));

                            string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";
                            using (SqlCommand command = new SqlCommand(insertQuery, connection, transaction))
                            {
                                foreach (DataGridViewColumn column in this.Columns)
                                {
                                    if (column.Name != primaryKeyColumn)
                                    {
                                        var value = row.Cells[column.Name].Value;
                                        command.Parameters.AddWithValue($"@{column.Name}", value ?? DBNull.Value);
                                    }
                                }
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RefreshData();
            }
        }

        public virtual void RefreshData()
        {
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    connection.Open();
                    string query = $"SELECT * FROM {tableName}";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        this.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 