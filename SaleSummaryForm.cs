using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace BalanzaPOSNuevo
{
    public partial class SaleSummaryForm : Form
    {
        private int _numeroDecimales;

        // Constructor para DataTable
        public SaleSummaryForm(DataTable saleItems, decimal total, decimal discount, string paymentMethod)
        {
            InitializeComponent();
            _numeroDecimales = ConfiguracionUsuario.WeightDecimals;
            File.AppendAllText("debug.log", $"[{DateTime.Now}] SaleSummaryForm inicializado con DataTable: Columnas recibidas: {string.Join(", ", saleItems.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}\n");
            LoadSaleDetails(saleItems.Copy(), total, discount, paymentMethod);
        }

        // Constructor para saleId
        public SaleSummaryForm(long saleId)
        {
            InitializeComponent();
            _numeroDecimales = ConfiguracionUsuario.WeightDecimals;
            File.AppendAllText("debug.log", $"[{DateTime.Now}] SaleSummaryForm inicializado con SaleId={saleId}\n");
            LoadSaleItems(saleId);
        }

        private void LoadSaleDetails(DataTable saleItems, decimal total, decimal discount, string paymentMethod)
        {
            try
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] LoadSaleDetails: Columnas en saleItems: {string.Join(", ", saleItems.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}\n");

                string[] requiredColumns = { "IdProducto", "Código", "Nombre", "Cantidad", "Unidad", "PrecioUnitario", "Subtotal" };
                var missingColumns = requiredColumns.Where(col => !saleItems.Columns.Contains(col)).ToList();
                if (missingColumns.Any())
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en LoadSaleDetails: Faltan columnas: {string.Join(", ", missingColumns)}\n");
                    throw new ArgumentException($"Falta una columna requerida: {string.Join(", ", missingColumns)}");
                }

                dgvSummaryItems.AutoGenerateColumns = false;
                dgvSummaryItems.Columns.Clear();

                var idColumn = new DataGridViewTextBoxColumn
                {
                    Name = "IdProducto",
                    HeaderText = "ID",
                    DataPropertyName = "IdProducto",
                    Visible = false
                };
                dgvSummaryItems.Columns.Add(idColumn);

                var codeColumn = new DataGridViewTextBoxColumn
                {
                    Name = "Código",
                    HeaderText = "Código",
                    DataPropertyName = "Código",
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
                };
                dgvSummaryItems.Columns.Add(codeColumn);

                var nameColumn = new DataGridViewTextBoxColumn
                {
                    Name = "Nombre",
                    HeaderText = "Producto",
                    DataPropertyName = "Nombre",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 200
                };
                dgvSummaryItems.Columns.Add(nameColumn);

                var quantityColumn = new DataGridViewTextBoxColumn
                {
                    Name = "Cantidad",
                    HeaderText = "Cantidad",
                    DataPropertyName = "Cantidad",
                    DefaultCellStyle = { Format = $"N{_numeroDecimales}", Alignment = DataGridViewContentAlignment.MiddleRight }
                };
                dgvSummaryItems.Columns.Add(quantityColumn);

                var unitColumn = new DataGridViewTextBoxColumn
                {
                    Name = "Unidad",
                    HeaderText = "Unidad",
                    DataPropertyName = "Unidad",
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
                };
                dgvSummaryItems.Columns.Add(unitColumn);

                var unitPriceColumn = new DataGridViewTextBoxColumn
                {
                    Name = "PrecioUnitario",
                    HeaderText = "Precio U.",
                    DataPropertyName = "PrecioUnitario",
                    DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
                };
                dgvSummaryItems.Columns.Add(unitPriceColumn);

                var subtotalColumn = new DataGridViewTextBoxColumn
                {
                    Name = "Subtotal",
                    HeaderText = "Subtotal",
                    DataPropertyName = "Subtotal",
                    DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
                };
                dgvSummaryItems.Columns.Add(subtotalColumn);

                dgvSummaryItems.Font = new Font("Consolas", 16f, FontStyle.Regular);
                dgvSummaryItems.BackgroundColor = Color.White;
                dgvSummaryItems.EnableHeadersVisualStyles = false;
                dgvSummaryItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
                dgvSummaryItems.ColumnHeadersDefaultCellStyle.Font = new Font("Consolas", 14f, FontStyle.Regular);
                dgvSummaryItems.DefaultCellStyle.BackColor = Color.White;
                dgvSummaryItems.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
                dgvSummaryItems.DefaultCellStyle.SelectionForeColor = Color.Black;
                dgvSummaryItems.GridColor = Color.LightGray;

                dgvSummaryItems.DataSource = saleItems;

                dgvSummaryItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                foreach (DataGridViewColumn col in dgvSummaryItems.Columns)
                {
                    col.ReadOnly = true;
                    col.Resizable = DataGridViewTriState.False;
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                if (dgvSummaryItems.Columns.Contains("Nombre"))
                {
                    dgvSummaryItems.Columns["Nombre"].FillWeight = 150;
                    dgvSummaryItems.Columns["Nombre"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dgvSummaryItems.Columns["Nombre"].MinimumWidth = 200;
                }

                dgvSummaryItems.AllowUserToAddRows = false;
                dgvSummaryItems.ReadOnly = true;

                lblSummaryTotal.Font = new Font("Consolas", 14f, FontStyle.Regular);
                lblDiscount.Font = new Font("Consolas", 14f, FontStyle.Regular);
                lblPaymentMethod.Font = new Font("Consolas", 14f, FontStyle.Regular);
                lblSummaryTotal.Text = $"Total a Pagar: {ConfiguracionUsuario.CurrencySymbol} {(total - discount):F2}";
                lblDiscount.Text = $"Descuento: {ConfiguracionUsuario.CurrencySymbol} {discount:F2}";
                lblPaymentMethod.Text = $"Método de Pago: {paymentMethod}";

                File.AppendAllText("debug.log", $"[{DateTime.Now}] SaleSummaryForm cargado: Datos de resumen configurados correctamente\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al cargar datos en SaleSummaryForm: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                MessageBox.Show($"Error al cargar el resumen de la venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSaleItems(long saleId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SQLiteCommand(
                        "SELECT si.ProductId AS 'IdProducto', p.Code AS 'Código', p.Name AS 'Nombre', si.Quantity AS 'Cantidad', si.Unit AS 'Unidad', si.UnitPrice AS 'PrecioUnitario', si.Subtotal AS 'Subtotal' " +
                        "FROM SaleItems si JOIN Products p ON si.ProductId = p.Id WHERE si.SaleId = @SaleId", conn);
                    cmd.Parameters.AddWithValue("@SaleId", saleId);
                    var adapter = new SQLiteDataAdapter(cmd);
                    var saleItems = new DataTable();
                    adapter.Fill(saleItems);

                    // Obtener total, descuento y método de pago desde la tabla Sales
                    var cmdSales = new SQLiteCommand(
                        "SELECT Total, Discount, PaymentMethod FROM Sales WHERE Id = @SaleId", conn);
                    cmdSales.Parameters.AddWithValue("@SaleId", saleId);
                    using (var reader = cmdSales.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal total = reader.GetDecimal(0);
                            decimal discount = reader.GetDecimal(1);
                            string paymentMethod = reader.GetString(2);
                            LoadSaleDetails(saleItems, total, discount, paymentMethod);
                        }
                        else
                        {
                            throw new Exception($"No se encontró la venta con ID {saleId}");
                        }
                    }
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Resumen de venta cargado para SaleId={saleId}\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al cargar resumen de venta: {ex.Message}\n");
                MessageBox.Show("Error al cargar el resumen de la venta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            File.AppendAllText("debug.log", $"[{DateTime.Now}] Botón Aceptar en SaleSummaryForm: Cerrando formulario\n");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void lblPaymentMethod_Click(object sender, EventArgs e)
        {
        }

        private void SaleSummaryForm_Load(object sender, EventArgs e)
        {
        }
    }
}