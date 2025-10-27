using System.Windows.Forms;

namespace aerosphere
{
    public class AircraftDataGridView : EditableDataGridView
    {
        public AircraftDataGridView()
        {
            tableName = "Самолеты";
            primaryKeyColumn = "ID Самолета";
            RefreshData();
        }

        protected override void EditableDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                var column = this.Columns[e.ColumnIndex];
                if (column.Name == "Модель" || column.Name == "Кол-во мест")
                {
                    base.EditableDataGridView_CellValueChanged(sender, e);
                }
            }
        }
    }
} 