using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BalanzaPOSNuevo
{
    public static class ExcelExporter
    {
        public static void ExportToCsv(DataTable data, string filePath, string reportType)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                // Encabezados
                var columnNames = data.Columns.Cast<DataColumn>().Select(column => $"\"{column.ColumnName}\"");
                sb.AppendLine(string.Join(",", columnNames));

                // Filas
                foreach (DataRow row in data.Rows)
                {
                    var fields = row.ItemArray.Select(field => $"\"{field.ToString().Replace("\"", "\"\"")}\"");
                    sb.AppendLine(string.Join(",", fields));
                }

                // Guardar archivo
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Reporte de {reportType} exportado a {filePath}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar el reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}