using System.Windows.Forms;

namespace aerosphere
{
    public class AirportsDataGridView : EditableDataGridView
    {
        public AirportsDataGridView()
        {
            tableName = "Аэропорты";
            primaryKeyColumn = "ID Аэропорта";
            RefreshData();
        }

        protected override void EditableDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                var column = this.Columns[e.ColumnIndex];
                if (column.Name == "Имя" || column.Name == "Город" || column.Name == "Страна")
                {
                    base.EditableDataGridView_CellValueChanged(sender, e);
                }
            }
        }
    }
} 