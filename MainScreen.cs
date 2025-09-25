using BalanzaPOSNuevo;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq; // Agregado para Cast, Select y Sum
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace BalanzaPOSNuevo
{
    public partial class MainScreen : Form
    {
        // -----------------------------------------------------
        // Sección de Variables a Nivel de Clase
        // -----------------------------------------------------

        #region Campos de Clase
        private Timer demoWeightTimer;
        private SerialPort serialPort;
        private DataTable productDataTable = new DataTable();
        private DataTable userDataTable = new DataTable();
        private DataTable saleItemsDataTable = new DataTable();
        private decimal currentProductRemainingStock;
        private int currentFoundProductId = -1;
        private string currentFoundProductCode = "";
        private string currentFoundProductName = "";
        private decimal currentFoundProductPrice = 0.0M;
        private string currentFoundProductUnit = "";
        private int currentLoggedInId;
        private BalanzaSimulator balanzaSimulator;
        private Random random = new Random();
        private decimal stableWeight;
        private SerialPort _serialPort;
        private System.Windows.Forms.Timer weightUpdateTimer;
        private string lastWeight = null;
        private DateTime lastWeightUpdate = DateTime.MinValue;
        private readonly TimeSpan debounceInterval = TimeSpan.FromMilliseconds(300);
        private readonly long loggedInUserId; // Agregar variable
        private decimal? currentWeight;
        private long lastSaleId;
        private string bufferSerial = string.Empty; // Variable a nivel de clase para acumular datos
        #endregion

        // Asegúrate de que esta clase esté definida en tu proyecto
        public class Product
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string Unit { get; set; }
            public decimal PricePerUnit { get; set; }
            public decimal Stock { get; set; }
            public decimal MinimumStock { get; set; } // ¡CORRECCIÓN AQUÍ! Debe coincidir con la BD
            public bool Active { get; set; } //
                                             // Otras propiedades relevantes como Active, etc.
        }
        public MainScreen(long userId, bool isAdmin)
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            File.AppendAllText("debug.log", $"[{DateTime.Now}] MainScreen_Constructor: Iniciando, UserId: {userId}, IsAdmin: {isAdmin}\n");
            Session.UserId = (int)userId;
            Session.IsAdmin = isAdmin;

            DatabaseHelper.InitializeDatabase();
            ConfiguracionUsuario.LoadSettings();

            saleItemsDataTable = new DataTable();
            InitializeSaleItemsDataTable();
            InitializeDataGridViews();
            dgvSaleItems.DataSource = saleItemsDataTable;

            btnConnectBalanza.Enabled = true;
            btnNewSale.Enabled = true;
            btnFinalizeSale.Enabled = true;
            this.FormClosing += MainScreen_FormClosing;
            PopulateSerialPorts();
            SetupAdminButtons();
            InitializeReportControls();
            LoadUserData();
            LoadProducts();
            UpdateSaleTable();
            File.AppendAllText("debug.log", $"[{DateTime.Now}] MainScreen_Constructor: Completado\n");
        }
        private void SetupUserInterface()
        {
            // Si el usuario es administrador, habilita los botones de administración.
            if (Session.IsAdmin)
            {
                btnAdmin.Visible = true;
                // Oculta otros elementos que no necesites
                // ...
            }
            else
            {
                // Si no es administrador, asegura que los botones estén ocultos o deshabilitados.
                btnAdmin.Visible = false;
                btnDevolucion.Visible = false;
            }
        }

        #region Métodos de Inicialización
        

        private void SetupAdminButtons()
        {
            btnAdmin.Visible = Session.IsAdmin;
            btnDevolucion.Visible = Session.IsAdmin;
        }



        #endregion

        #region Métodos de Producto
       

        // Dentro de MainScreen.cs
        // Declara esta variable dentro de la clase MainScreen, pero fuera de cualquier método.

        // Este es el método que abre el formulario de resumen
        // Dentro de la clase FormConfiguracion
        private void StandardizeFontsAndSizes()
        {
            Font standardFont = new Font("Segoe UI", 12F, FontStyle.Regular);
            Size textBoxSize = new Size(150, 30);
            Size buttonSize = new Size(120, 40);

            this.txtProductName.Font = standardFont;
            this.txtProductId.Font = standardFont;
            this.txtProductPrice.Font = standardFont;
            this.txtStock.Font = standardFont;
            this.txtMinimumStock.Font = standardFont;
            this.cboProductUnit.Font = standardFont;
            this.label7.Font = standardFont;
            this.label8.Font = standardFont;
            this.label9.Font = standardFont;
            this.label10.Font = standardFont;
            this.btnAddProduct.Font = standardFont;
            this.btnUpdateProduct.Font = standardFont;
            this.btnDeleteProduct.Font = standardFont;
            this.btnClearProductFields.Font = standardFont;

            this.txtProductName.Size = textBoxSize;
            this.txtProductId.Size = textBoxSize;
            this.txtProductPrice.Size = textBoxSize;
            this.txtStock.Size = textBoxSize;
            this.txtMinimumStock.Size = textBoxSize;
            this.cboProductUnit.Size = textBoxSize;
            this.btnAddProduct.Size = buttonSize;
            this.btnUpdateProduct.Size = buttonSize;
            this.btnDeleteProduct.Size = buttonSize;
            this.btnClearProductFields.Size = buttonSize;

            this.txtSaleProductName.Font = standardFont;
            this.txtSearchProductCode.Font = standardFont;
            this.txtSaleProductPrice.Font = standardFont;
            this.txt1Quantity.Font = standardFont;
            this.txtRemainingStock.Font = standardFont;
            this.label3.Font = standardFont;
            this.label4.Font = standardFont;
            this.label5.Font = standardFont;
            this.label23.Font = standardFont;
            this.lblSaleProductUnit.Font = standardFont;
            this.btnSearchProduct.Font = standardFont;
            this.btnAddSaleItem.Font = standardFont;

            this.txtSearchProductCode.Size = textBoxSize;
            this.txtSaleProductPrice.Size = textBoxSize;
            this.txt1Quantity.Size = textBoxSize;
            this.txtRemainingStock.Size = textBoxSize;
            this.btnSearchProduct.Size = new Size(200, 40);
            this.btnAddSaleItem.Size = new Size(200, 40);

            this.txtDiscount.Font = standardFont;
            this.cboPaymentMethod.Font = standardFont;
            this.label24.Font = standardFont;
            this.label25.Font = standardFont;
            this.btnFinalizeSale.Font = standardFont;
            this.btnClearAllItems.Font = standardFont;
            this.btnNewSale.Font = standardFont;
            this.btnDevolucion.Font = standardFont;
            this.btnRemoveSaleItem.Font = standardFont;

            this.txtDiscount.Size = textBoxSize;
            this.cboPaymentMethod.Size = textBoxSize;
            this.btnFinalizeSale.Size = buttonSize;
            this.btnClearAllItems.Size = buttonSize;
            this.btnNewSale.Size = buttonSize;
            this.btnDevolucion.Size = buttonSize;
            this.btnRemoveSaleItem.Size = buttonSize;
        }

        private void ShowSaleSummary()
        {
            if (saleItemsDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No hay productos en el carrito de venta para finalizar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                foreach (DataRow row in saleItemsDataTable.Rows)
                {
                    if (string.IsNullOrWhiteSpace(row["Unidad"]?.ToString()))
                    {
                        long productId = Convert.ToInt64(row["IdProducto"]);
                        var productInfo = GetProductInfo(productId);
                        row["Código"] = productInfo.Code;
                        row["Nombre"] = productInfo.Name;
                        row["Unidad"] = productInfo.Unit;
                        row["PrecioUnitario"] = productInfo.PricePerUnit;
                        row["Subtotal"] = Convert.ToDecimal(row["Cantidad"]) * productInfo.PricePerUnit;
                    }
                }

                decimal total = saleItemsDataTable.AsEnumerable()
                    .Sum(row => row.Field<decimal>("Subtotal"));
                decimal discount = txtDiscount.Text != "" ? Convert.ToDecimal(txtDiscount.Text.Replace(",", "."), CultureInfo.InvariantCulture) : 0;
                string paymentMethod = cboPaymentMethod.SelectedItem?.ToString() ?? "Efectivo";

                SaleSummaryForm summaryForm = new SaleSummaryForm(saleItemsDataTable, total, discount, paymentMethod);
                if (summaryForm.ShowDialog() == DialogResult.OK)
                {
                    btnNewSale.Enabled = true;
                    DatabaseHelper.SaveSaleToDatabase(saleItemsDataTable, total, discount, paymentMethod, Session.Username, 1);
                    ClearSaleInterface();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en ShowSaleSummary: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                MessageBox.Show($"Error al mostrar el resumen de venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDevolucion_Click(object sender, EventArgs e)
        {
            if (dgvSaleItems.CurrentRow == null)
            {
                MessageBox.Show("Por favor, selecciona un producto de la venta para devolver.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataRowView dataRowView = (DataRowView)dgvSaleItems.CurrentRow.DataBoundItem;
                DataRow row = dataRowView.Row;
                long productId = Convert.ToInt64(row["IdProducto"]);
                decimal quantity = Convert.ToDecimal(row["Cantidad"]);

                ProcessReturnAndRestock(productId, quantity);
                dgvSaleItems.Rows.Remove(dgvSaleItems.CurrentRow);

                MessageBox.Show("Devolución procesada exitosamente. El stock ha sido actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Log("btnDevolucion_Click", $"Error al procesar devolución: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al procesar la devolución: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeReportControls()
        {
            cboPaymentMethod.Items.AddRange(new[] { "Efectivo", "Tarjeta", "Transferencia" });
            cboPaymentMethod.SelectedIndex = 0;
        }

        private void ProcessReturnAndRestock(long productId, decimal quantity)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        DatabaseHelper.UpdateStockWithHistory(productId, quantity, "Devolución", Session.Username ?? "default_user", conn, transaction);
                        transaction.Commit();
                    }
                }
                Logger.Log("ProcessReturnAndRestock", $"Devolución procesada para producto ID {productId}, cantidad: {quantity}");
            }
            catch (Exception ex)
            {
                Logger.Log("ProcessReturnAndRestock", $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }
        ////Pega este método temporalmente en tu clase MainScreen:
        private void ShowColumnNames()
        {
            string columnNames = "Nombres de las columnas:\n";
            foreach (DataGridViewColumn column in dgvSaleItems.Columns)
            {
                columnNames += column.Name + "\n";
            }
            MessageBox.Show(columnNames);
        }
        //// Hasta aquimetodo temporal
        private void InitializeSerialPort()
        {
            _serialPort = new SerialPort();
            // Asegúrate de que estos valores coincidan con tu balanza
            _serialPort.PortName = "COM3"; // O el puerto seleccionado en tu configuración
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.ReadTimeout = 500; // Tiempo de espera para la lectura

            _serialPort.DataReceived += SerialPort_DataReceived;
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Esto debe ejecutarse en el hilo de la UI para actualizar el control
            this.Invoke(new MethodInvoker(delegate
            {
                bufferSerial += serialPort.ReadExisting(); // Acumula todos los datos recibidos

                // Asumiendo que cada "lectura" de peso termina con un salto de línea o retorno de carro
                if (bufferSerial.Contains("\r") || bufferSerial.Contains("\n"))
                {
                    string[] lines = bufferSerial.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0)
                    {
                        string lastLine = lines[lines.Length - 1].Trim(); // Toma la última línea completa
                        if (decimal.TryParse(lastLine, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight))
                        {
                            txtWeightDisplay.Text = weight.ToString("F3", CultureInfo.InvariantCulture); // Formatea a 3 decimales
                            Logger.Log("Peso Balanza", $"Peso: {weight:F3}");
                        }
                        else
                        {
                            Logger.Log("Advertencia", $"No se pudo parsear el peso: '{lastLine}'");
                        }
                    }
                    bufferSerial = string.Empty; // Limpia el buffer después de procesar
                }
            }));
        }

        private void ProcessSerialData(SerialPort port)
        {
            try
            {
                string data = port.ReadExisting();
                Logger.Log("Datos Balanza", $"Recibido: {data.Trim()}");

                // Regex más robusto para encontrar números decimales con posible signo
                // Ajusta este Regex al formato EXACTO de tu balanza.
                Match match = Regex.Match(data, @"[+-]?\s*(\d+(\.\d+)?)");

                if (match.Success)
                {
                    decimal weight;
                    // Usar CultureInfo.InvariantCulture para parsear el punto decimal
                    if (decimal.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out weight))
                    {
                        // Usa el formato de decimales de la configuración del usuario
                        string format = "F" + ConfiguracionUsuario.WeightDecimals.ToString();
                        txtWeightDisplay.Text = weight.ToString(format, CultureInfo.InvariantCulture);
                        txtWeightDisplay.Refresh(); // Fuerza la actualización del TextBox
                        Logger.Log("Peso Balanza", $"Peso: {txtWeightDisplay.Text}");
                    }
                }
            }
            catch (TimeoutException) { /* No hacer nada, es normal */ }
            catch (Exception ex)
            {
                Logger.Log("Error Balanza", $"Error en DataReceived: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
        private bool TryParseWeight(string data, out decimal weight)
        {
            weight = 0;
            // Expresión regular para encontrar números decimales (ajustar si tu balanza tiene otro formato)
            System.Text.RegularExpressions.Match match =
                System.Text.RegularExpressions.Regex.Match(data, @"\d+\.?\d*");

            if (match.Success)
            {
                return decimal.TryParse(match.Value, System.Globalization.NumberStyles.Any,
                                         System.Globalization.CultureInfo.InvariantCulture, out weight);
            }
            return false;
        }
        private void ReconfigureSerialPort(string newPortName, int newBaudRate)
        {
            try
            {
                if (!ValidateSerialPortParameters(out string serialPort, out int baudRate, out Parity parity, out int dataBits, out StopBits stopBits))
                {
                    Logger.Log("Error al reconfigurar puerto serial", "Parámetros inválidos");
                    MessageBox.Show("Parámetros del puerto serial inválidos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Actualizar configuración usando SaveSettings
                ConfiguracionUsuario.SaveSettings(
                    weightDecimals: ConfiguracionUsuario.WeightDecimals,
                    currencyDecimals: ConfiguracionUsuario.CurrencyDecimals,
                    currencySymbol: ConfiguracionUsuario.CurrencySymbol,
                    baudRate: newBaudRate,
                    serialPort: newPortName,
                    parity: parity,
                    dataBits: dataBits,
                    stopBits: stopBits
                );

                // Reiniciar el puerto serial
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        Logger.Log("Puerto serial anterior cerrado", $"Puerto: {_serialPort.PortName}");
                    }
                    _serialPort.Dispose();
                    _serialPort = null;
                }

                _serialPort = new SerialPort
                {
                    PortName = newPortName,
                    BaudRate = newBaudRate,
                    Parity = parity,
                    DataBits = dataBits,
                    StopBits = stopBits,
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };
                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.Open();
                Logger.Log("Puerto serial reconfigurado", $"Puerto: {newPortName}, BaudRate: {newBaudRate}, Parity: {parity}, DataBits: {dataBits}, StopBits: {stopBits}");

                // Actualizar UI
                lblConnectionStatus.Text = "Conectado";
                lblConnectionStatus.ForeColor = Color.Green;
                btnConnectBalanza.Enabled = false;
                btnDisconnectBalanza.Enabled = true;

                // Reiniciar temporizador
                if (weightUpdateTimer != null)
                {
                    weightUpdateTimer.Stop();
                    weightUpdateTimer.Start();
                    Logger.Log("Temporizador de peso reiniciado", "Intervalo: 500ms");
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error al reconfigurar puerto serial", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al reconfigurar puerto serial: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblConnectionStatus.Text = "Desconectado";
                lblConnectionStatus.ForeColor = Color.Red;
                btnConnectBalanza.Enabled = true;
                btnDisconnectBalanza.Enabled = false;
            }
        }

      
        private void InitializeConfiguration()
        {
            try
            {
                ConfiguracionUsuario.SaveSettings(
                    weightDecimals: 2,
                    currencyDecimals: ConfiguracionUsuario.CurrencyDecimals,
                    currencySymbol: ConfiguracionUsuario.CurrencySymbol,
                    baudRate: ConfiguracionUsuario.BaudRate,
                    serialPort: ConfiguracionUsuario.SerialPort,
                    parity: ConfiguracionUsuario.Parity,
                    dataBits: ConfiguracionUsuario.DataBits,
                    stopBits: ConfiguracionUsuario.StopBits
                );
                Logger.Log("Configuración inicializada", $"WeightDecimals={ConfiguracionUsuario.WeightDecimals}");
            }
            catch (Exception ex)
            {
                Logger.Log("Error al inicializar configuración", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al inicializar configuración: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Paso 4: Verificar ConfiguracionUsuario

        private void MainScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DisconnectSerialPort();
                weightUpdateTimer?.Stop();
                weightUpdateTimer?.Dispose();
                Logger.Log("Formulario MainScreen cerrándose", "Recursos liberados");
            }
            catch (Exception ex)
            {
                Logger.Log("Error al cerrar MainScreen", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        // Método en tu clase MainScreen
        private void CheckStockWarning(string productCode)
        {
            long productId = DatabaseHelper.GetProductIdFromCode(productCode);
            decimal currentStockDecimal = DatabaseHelper.ObtenerStockActual(productId);

            // ⭐ CLAVE: Crea una variable para el formato del peso
            string formatoPeso = "N" + ConfiguracionUsuario.WeightDecimals.ToString();

            if (currentStockDecimal <= 0)
            {
                MessageBox.Show($"Stock insuficiente. Stock disponible: {currentStockDecimal.ToString(formatoPeso)}",
                                "Advertencia de Stock",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }
        // Dentro de tu clase MainScreen
       

        private void CargarConfiguracion()
        {
            // Reemplaza la llamada antigua con las nuevas llamadas
            int baudRate = ConfiguracionUsuario.BaudRate;
            string serialPort = ConfiguracionUsuario.SerialPort;
            Parity parity = ConfiguracionUsuario.Parity;
            int dataBits = ConfiguracionUsuario.DataBits;
            StopBits stopBits = ConfiguracionUsuario.StopBits;

            // Asigna los valores a tus controles de la interfaz de usuario, si los tienes
            // Ejemplo:
            // numericUpDownBaudRate.Value = baudRate;
            // comboPorts.Text = serialPort;
        }

        private void ClearSaleInterface()
        {
            try
            {
                saleItemsDataTable.Clear();
                txtSearchProductCode.Text = "";
                txtSaleProductName.Text = "";
                lblSaleProductUnit.Text = "";
                txtSaleProductPrice.Text = "";
                txtRemainingStock.Text = "";
                txt1Quantity.Text = "";
                txtDiscount.Text = "";
                txtTotalSale.Text = "0.00";
                dgvSaleItems.DataSource = null;
                dgvSaleItems.DataSource = saleItemsDataTable;
                dgvSaleItems.Refresh();
                File.AppendAllText("debug.log", $"[{DateTime.Now}] ClearSaleInterface: Interfaz de venta limpiada\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en ClearSaleInterface: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
            }
        }

        private void ClearProductDetails()
        {
            // Limpia los Labels y TextBoxes que muestran la información del producto buscado
            // Por favor, reemplaza estos nombres con los que uses en tu formulario de ventas
            txtSearchProductCode.Clear();
            txtSaleProductName.Text = string.Empty;
            txtSaleProductPrice.Text = string.Empty;
            // ⭐ Asegúrate de que este campo también se limpie
            txtWeightDisplay.Text = string.Empty;
            txt1Quantity.Text = "";
        }

        private void AssignQuickProductsToButtons()
        {
            // Primero, desuscribe todos los eventos y limpia los botones
            for (int i = 1; i <= 10; i++) // Asume 10 botones rápidos fijos
            {
                Button btn = panel2.Controls.Find($"btnProductQuick{i}", true).FirstOrDefault() as Button;
                if (btn != null)
                {
                    btn.Click -= btnProductQuick_Click; // Desuscribe el evento anterior
                    btn.Text = "Vacío"; // Texto por defecto
                    btn.Tag = null;     // Limpia el Tag (importante)
                    btn.Enabled = false; // Deshabilita el botón si no hay producto
                }
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Consulta que une QuickProducts con Products para obtener toda la información del producto
                    string query = @"
                     SELECT qp.ButtonIndex, p.Id, p.Code, p.Name, p.Unit, p.PricePerUnit, p.Stock, p.MinimumStock, p.Active
                     FROM QuickProducts qp
                     JOIN Products p ON qp.ProductId = p.Id
                     WHERE p.Active = 1
                     ORDER BY qp.ButtonIndex";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int buttonIndex = reader.GetInt32(reader.GetOrdinal("ButtonIndex"));

                                // Busca el botón en el panel2
                                var button = panel2.Controls.Find($"btnProductQuick{buttonIndex}", true).FirstOrDefault() as Button;
                                if (button != null)
                                {
                                    // Crea un objeto Product completo y lo almacena en el Tag
                                    Product product = new Product
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                        Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? string.Empty : reader.GetString(reader.GetOrdinal("Code")),
                                        Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "N/A" : reader.GetString(reader.GetOrdinal("Name")),
                                        Unit = reader.IsDBNull(reader.GetOrdinal("Unit")) ? "unidad" : reader.GetString(reader.GetOrdinal("Unit")),
                                        PricePerUnit = reader.GetDecimal(reader.GetOrdinal("PricePerUnit")),
                                        Stock = reader.GetDecimal(reader.GetOrdinal("Stock")),
                                        MinimumStock = reader.IsDBNull(reader.GetOrdinal("MinimumStock")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MinimumStock")), // ¡CORRECCIÓN AQUÍ!
                                        Active = reader.GetBoolean(reader.GetOrdinal("Active")) // Añadir esto para Active
                                    };

                                    button.Text = product.Name;
                                    button.Tag = product; // ¡Guarda el objeto Product completo!
                                    button.Click += btnProductQuick_Click; // Suscribe el evento correcto
                                    button.Enabled = true; // Habilita el botón
                                    Logger.Log("Productos rápidos asignados", $"Botón btnProductQuick{buttonIndex} asignado a {product.Name}");
                                }
                                else
                                {
                                    Logger.Log("Advertencia", $"Botón btnProductQuick{buttonIndex} no encontrado en panel2 para asignar producto.");
                                }
                            }
                        }
                    }
                }
                Logger.Log("Productos rápidos asignados", "Proceso de asignación de botones rápidos completado.");
            }
            catch (Exception ex)
            {
                Logger.Log("Error al asignar productos rápidos", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al asignar productos rápidos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuickProduct_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null || button.Tag == null) return;

                var quickProduct = button.Tag as Product;
                if (quickProduct == null) return;

                txtSearchProductCode.Text = quickProduct.Code;
                txtSaleProductName.Text = quickProduct.Name;
                lblSaleProductUnit.Text = quickProduct.Unit;
                txtSaleProductPrice.Text = quickProduct.PricePerUnit.ToString("N" + ConfiguracionUsuario.CurrencyDecimals, CultureInfo.InvariantCulture);
                txtRemainingStock.Text = quickProduct.Stock.ToString("N" + ConfiguracionUsuario.WeightDecimals, CultureInfo.InvariantCulture);

                File.AppendAllText("debug.log", $"[{DateTime.Now}] btnQuickProduct_Click: Producto seleccionado: Código={quickProduct.Code}, Nombre={quickProduct.Name}\n");
                UpdateSaleTable();
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnQuickProduct_Click: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                MessageBox.Show($"Error al seleccionar producto rápido: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ProductButton_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag is Product product)
                {
                    currentFoundProductId = product.Id;
                    currentFoundProductCode = product.Code;
                    currentFoundProductName = product.Name;
                    currentFoundProductPrice = product.PricePerUnit;

                    string weightFormat = "N" + ConfiguracionUsuario.WeightDecimals;
                    string priceFormat = "N" + ConfiguracionUsuario.CurrencyDecimals;
                    txtProductId.Text = product.Id.ToString();
                    txtSearchProductCode.Text = product.Code?.PadLeft(6, '0') ?? string.Empty;
                    txtSaleProductName.Text = product.Name ?? string.Empty;
                    txtSaleProductPrice.Text = product.PricePerUnit.ToString(priceFormat, CultureInfo.InvariantCulture);
                    txtStock.Text = product.Stock.ToString(weightFormat, CultureInfo.InvariantCulture);
                    txtMinimumStock.Text = product.MinimumStock.ToString(weightFormat, CultureInfo.InvariantCulture);
                    cboProductUnit.Text = product.Unit ?? string.Empty;
                    txtRemainingStock.Text = product.Stock.ToString(weightFormat, CultureInfo.InvariantCulture);
                    txtRemainingStock.ForeColor = product.Stock <= product.MinimumStock ? Color.Red : Color.Black;
                    cboWeightUnit.Text = product.Unit ?? string.Empty;
                    cboWeightUnit.Enabled = false;

                    // Asignar cantidad
                    if (decimal.TryParse(txtWeightDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight) && weight > 0)
                    {
                        txt1Quantity.Text = weight.ToString(weightFormat, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        txt1Quantity.Text = 1.ToString(weightFormat, CultureInfo.InvariantCulture);
                    }

                    Logger.Log("Producto seleccionado desde botón", $"ID={product.Id}, Code={product.Code}, Name={product.Name}, Price={product.PricePerUnit}, Unit={product.Unit}, Stock={product.Stock}, Quantity={txt1Quantity.Text}");
                    Logger.Log("Verificación de controles", $"txtProductId.Text={txtProductId.Text}, txtSearchProductCode.Text={txtSearchProductCode.Text}, txtSaleProductName.Text={txtSaleProductName.Text}, txtSaleProductPrice.Text={txtSaleProductPrice.Text}, cboProductUnit.Text={cboProductUnit.Text}, txtRemainingStock.Text={txtRemainingStock.Text}, txt1Quantity.Text={txt1Quantity.Text}, cboWeightUnit.Text={cboWeightUnit.Text}, txtProductId.Visible={txtProductId.Visible}, txtSaleProductName.Visible={txtSaleProductName.Visible}, txtSaleProductPrice.Visible={txtSaleProductPrice.Visible}, txt1Quantity.Visible={txt1Quantity.Visible}");
                }
                else
                {
                    Logger.Log("Error en ProductButton_Click", "El botón no tiene un producto asociado");
                    MessageBox.Show("El botón no tiene un producto asociado. Configure los productos rápidos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error en ProductButton_Click", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al seleccionar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public class SaleDetail
        {
            public long ProductId { get; set; }
            public double Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double LineTotal { get; set; }
         
        }


        // -----------------------------------------------------
        // Constructor del Formulario MainScreen
        // -----------------------------------------------------

        // Constructor sin parámetros (usado si solo haces new MainScreen())
        public MainScreen()
        {
            InitializeComponent();
            File.AppendAllText("debug.log", $"[{DateTime.Now}] MainScreen_Constructor: Iniciando constructor MainScreen\n");
            _serialPort = new SerialPort(); // Asegúrate de que se inicialice aquí
            _serialPort.DataReceived += SerialPort_DataReceived;
           
            ConfiguracionUsuario.LoadSettings();
            PopulateSerialPorts();
            InitializeSaleItemsDataTable();
            InitializeDataGridViews();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            dgvUsers.DataError += dgvUsers_DataError;
            this.Load += new System.EventHandler(this.MainScreen_Load);
            this.FormClosing += new FormClosingEventHandler(this.MainScreen_FormClosing);
            LoadUserData();
            txtSearchProductCode.Text = "000000";
            txtProductId.Text = "000000";
            // Asegúrate de que los estados iniciales de los botones sean correctos
            btnConnectBalanza.Enabled = true;
            btnDisconnectBalanza.Enabled = false;
            lblConnectionStatus.Text = "Desconectado";
            lblConnectionStatus.ForeColor = Color.Red;
        }

        private void PopulateComPorts()
        {
            cmbPorts.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            cmbPorts.Items.AddRange(ports);
            if (ports.Length > 0)
            {
                cmbPorts.SelectedIndex = 0; // Selecciona el primero por defecto
            }
        }
        private void LoadUserSettings()
        {
            // Esto debería cargar los valores de ConfiguracionUsuario en tus controles de UI si existen
            // Ejemplo:
            // cmbBaudRate.SelectedItem = ConfiguracionUsuario.BaudRate.ToString();
            // cmbParity.SelectedItem = ConfiguracionUsuario.Parity.ToString();
            // ...
            if (!string.IsNullOrEmpty(ConfiguracionUsuario.SerialPort) && cmbPorts.Items.Contains(ConfiguracionUsuario.SerialPort))
            {
                cmbPorts.SelectedItem = ConfiguracionUsuario.SerialPort;
            }
        }

        // Constructor que acepta el ID de usuario logueado y una referencia al LoginScreen
        public MainScreen(int Id, LoginScreen loginForm) : this() // Llama al constructor sin parámetros
        {
            this.currentLoggedInId = Id;
            
            // Aquí podrías cargar datos específicos del usuario o permisos (ej. deshabilitar pestañas)
            // if (!IsAdminUser(Id)) { tabControlMain.TabPages.Remove(tabPageUsers); }
        }

        // Variable global para almacenar la cantidad de decimales.
        // Puedes inicializarla con un valor por defecto.
        private void MainScreen_Load(object sender, EventArgs e)
        {
            // Inicializar saleItemsDataTable si es nulo
            if (saleItemsDataTable == null)
            {
                saleItemsDataTable = new DataTable();
            }

            // Agregar columnas solo si no existen
            if (saleItemsDataTable.Columns.Count == 0)
            {
                saleItemsDataTable.Columns.Add("IdProducto", typeof(long));
                saleItemsDataTable.Columns.Add("Código", typeof(string));
                saleItemsDataTable.Columns.Add("Nombre", typeof(string));
                saleItemsDataTable.Columns.Add("Cantidad", typeof(decimal));
                saleItemsDataTable.Columns.Add("Unidad", typeof(string));
                saleItemsDataTable.Columns.Add("PrecioUnitario", typeof(decimal));
                saleItemsDataTable.Columns.Add("Subtotal", typeof(decimal));
            }

            InitializeDataGridViews();
            ConfiguracionUsuario.LoadSettings();
            SetupSerialPort();
            LoadProducts();
           
           

            File.AppendAllText("debug.log", $"[{DateTime.Now}] MainScreen_Load ejecutado. Columnas en saleItemsDataTable: {string.Join(", ", saleItemsDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}\n");
        }

        // -----------------------------------------------------
        // Manejador de Eventos de Carga del Formulario (MainScreen_Load)
        // -----------------------------------------------------
        private void InitializeUsability()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(this.txtSearchProductCode, "Ingrese el código del producto o presione Enter para buscar");
            toolTip.SetToolTip(this.txt1Quantity, "Ingrese la cantidad del producto a vender");
            toolTip.SetToolTip(this.btnAddSaleItem, "Añade el producto seleccionado a la venta");
            toolTip.SetToolTip(this.txtProductName, "Ingrese el nombre del producto");
            toolTip.SetToolTip(this.txtProductId, "Ingrese el código único del producto");
            toolTip.SetToolTip(this.txtDiscount, "Ingrese el descuento para la venta");
            toolTip.SetToolTip(this.cboPaymentMethod, "Seleccione el método de pago");

            this.txtSearchProductCode.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnSearchProduct_Click(s, e);
                    e.Handled = true;
                }
            };
        }

         
        private async Task<decimal> GetWeightFromScaleAsync()
        {
            try
            {
                // Configuración del puerto serial (ajusta según la balanza)
                using (var serialPort = new System.IO.Ports.SerialPort())
                {
                    serialPort.PortName = "COM1"; // Reemplaza con el puerto correcto
                    serialPort.BaudRate = 9600;   // Ajusta según la balanza
                    serialPort.Parity = System.IO.Ports.Parity.None;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = System.IO.Ports.StopBits.One;
                    serialPort.ReadTimeout = 200; // Timeout reducido

                    try
                    {
                        serialPort.Open();
                        await Task.Run(() => serialPort.Write("W\r\n")); // Comando asíncrono
                        string response = await Task.Run(() => serialPort.ReadLine());
                        serialPort.Close();

                        // Parsear la respuesta
                        string weightString = response.Trim();
                        if (weightString.Contains("kg"))
                        {
                            weightString = weightString.Replace("kg", "").Trim();
                        }
                        if (decimal.TryParse(weightString, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal weight))
                        {
                            Logger.Log("Peso leído desde balanza", $"Peso: {weight}");
                            return weight;
                        }
                        else
                        {
                            Logger.Log("Error al leer peso", $"Respuesta inválida: {response}");
                            return 0m;
                        }
                    }
                    catch (TimeoutException)
                    {
                        Logger.Log("Error en GetWeightFromScaleAsync", "Timeout al leer desde la balanza");
                        return 0m;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Error en GetWeightFromScaleAsync", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                        return 0m;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error en GetWeightFromScaleAsync", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                return 0m;
            }
        }



        
        private decimal GetWeightFromScale()
        {
            try
            {
                // Configuración del puerto serial (ajusta según la balanza)
                using (var serialPort = new System.IO.Ports.SerialPort())
                {
                    serialPort.PortName = "COM1"; // Reemplaza con el puerto correcto (por ejemplo, COM1, COM2)
                    serialPort.BaudRate = 9600;   // Ajusta según la balanza (9600 es común)
                    serialPort.Parity = System.IO.Ports.Parity.None;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = System.IO.Ports.StopBits.One;
                    serialPort.ReadTimeout = 1000; // Timeout de 1 segundo

                    try
                    {
                        serialPort.Open();
                        // Enviar comando para solicitar peso (depende de la balanza, consulta el manual)
                        serialPort.Write("W\r\n"); // Ejemplo: comando 'W' para solicitar peso
                        string response = serialPort.ReadLine(); // Leer respuesta
                        serialPort.Close();

                        // Parsear la respuesta (ajusta según el formato de la balanza)
                        // Ejemplo: respuesta "ST,GS,  1.234kg" -> extraer "1.234"
                        string weightString = response.Trim();
                        if (weightString.Contains("kg"))
                        {
                            weightString = weightString.Replace("kg", "").Trim();
                        }
                        if (decimal.TryParse(weightString, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal weight))
                        {
                            Logger.Log("Peso leído desde balanza", $"Peso: {weight}");
                            return weight;
                        }
                        else
                        {
                            Logger.Log("Error al leer peso", $"Respuesta inválida: {response}");
                            return 0m; // Retornar 0 si no se puede parsear
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Error en GetWeightFromScale", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                        return 0m; // Retornar 0 en caso de error
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error en GetWeightFromScale", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                return 0m; // Retornar 0 en caso de error general
            }
        }

        private void PopulateSerialPorts()
        {
            try
            {
                string[] ports = SerialPort.GetPortNames();
                if (cmbPorts.InvokeRequired)
                {
                    cmbPorts.BeginInvoke(new Action(() =>
                    {
                        cmbPorts.Items.Clear();
                        cmbPorts.Items.AddRange(ports);
                        if (ports.Length > 0)
                        {
                            cmbPorts.SelectedItem = ConfiguracionUsuario.SerialPort;
                        }
                        Logger.Log("PopulateSerialPorts", $"Puertos seriales cargados: {string.Join(", ", ports)} [2025-09-22 02:00:00 -05]");
                    }));
                }
                else
                {
                    cmbPorts.Items.Clear();
                    cmbPorts.Items.AddRange(ports);
                    if (ports.Length > 0)
                    {
                        cmbPorts.SelectedItem = ConfiguracionUsuario.SerialPort;
                    }
                    Logger.Log("PopulateSerialPorts", $"Puertos seriales cargados: {string.Join(", ", ports)} [2025-09-22 02:00:00 -05]");
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error en PopulateSerialPorts", $"{ex.Message}\nStackTrace: {ex.StackTrace} [2025-09-22 02:00:00 -05]");
                MessageBox.Show($"Error al cargar puertos seriales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupSerialPort()
        {
            try
            {
                serialPort = new SerialPort
                {
                    PortName = ConfiguracionUsuario.SerialPort,
                    BaudRate = ConfiguracionUsuario.BaudRate,
                    Parity = ConfiguracionUsuario.Parity,
                    DataBits = ConfiguracionUsuario.DataBits,
                    StopBits = ConfiguracionUsuario.StopBits
                };
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Puerto serial {ConfiguracionUsuario.SerialPort} abierto correctamente.\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al abrir puerto serial: {ex.Message}\n");
                MessageBox.Show("No se pudo conectar con la balanza.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Método ficticio para obtener peso (implementar según hardware de la balanza)

        private void ConfigureQuickProducts()
        {
            try
            {
                using (Form configForm = new Form { Text = "Configurar Productos Rápidos", Size = new Size(600, 400) })
                {
                    DataGridView dgvQuickProducts = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        AutoGenerateColumns = false,
                        AllowUserToAddRows = false, // No se añaden filas por el usuario, siempre hay 10
                        AllowUserToDeleteRows = false // No se borran filas por el usuario
                    };
                    dgvQuickProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ButtonIndex", HeaderText = "Índice Botón", DataPropertyName = "ButtonIndex", ReadOnly = true });

                    DataGridViewComboBoxColumn productColumn = new DataGridViewComboBoxColumn
                    {
                        Name = "ProductId",
                        HeaderText = "Producto",
                        DataPropertyName = "ProductId",
                        DisplayMember = "Name",
                        ValueMember = "Id",
                        DataSource = GetProductsForComboBox() // Usa el método que incluye "(Ninguno)"
                    };
                    dgvQuickProducts.Columns.Add(productColumn);

                    DataTable dt = new DataTable();
                    dt.Columns.Add("ButtonIndex", typeof(int));
                    // dt.Columns.Add("ProductId", typeof(int)); // <<-- CAMBIA ESTO
                    dt.Columns.Add("ProductId", typeof(object)); // <--- A ESTO (o typeof(int) si manejas 0 como "Ninguno")

                    // Pre-llenar con 10 filas para los 10 botones rápidos
                    for (int i = 1; i <= 10; i++)
                    {
                        dt.Rows.Add(i, DBNull.Value); // Inicializa con ProductId null
                    }

                    using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        string query = @"
                    SELECT qp.ButtonIndex, qp.ProductId
                    FROM QuickProducts qp
                    ORDER BY qp.ButtonIndex"; // Carga lo que ya está configurado
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int buttonIndex = reader.GetInt32(reader.GetOrdinal("ButtonIndex"));
                                    // Encuentra la fila existente por ButtonIndex y actualízala
                                    DataRow existingRow = dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ButtonIndex") == buttonIndex);
                                    if (existingRow != null)
                                    {
                                        existingRow["ProductId"] = reader.IsDBNull(reader.GetOrdinal("ProductId")) ? DBNull.Value : (object)reader.GetInt32(reader.GetOrdinal("ProductId"));
                                    }
                                }
                            }
                        }
                    }
                    dgvQuickProducts.DataSource = dt;

                    // Panel para los botones al final del formulario de configuración
                    FlowLayoutPanel bottomPanel = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Bottom,
                        AutoSize = true,
                        FlowDirection = FlowDirection.RightToLeft // Alinea los botones a la derecha
                    };

                    Button btnSave = new Button { Text = "Guardar", AutoSize = true, Margin = new Padding(5) };
                    btnSave.Click += (s, e) =>
                    {
                        using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                        {
                            conn.Open();
                            using (var transaction = conn.BeginTransaction())
                            {
                                // 1. Limpiar todos los QuickProducts existentes
                                string deleteQuery = "DELETE FROM QuickProducts";
                                using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, conn, transaction))
                                {
                                    cmd.ExecuteNonQuery();
                                }

                                // 2. Insertar solo los QuickProducts que tienen un ProductId asignado
                                string insertQuery = "INSERT INTO QuickProducts (ButtonIndex, ProductId) VALUES (@ButtonIndex, @ProductId)";
                                foreach (DataRow row in dt.Rows)
                                {
                                    // Solo inserta si hay un ProductId válido (no DBNull.Value)
                                    if (row["ProductId"] != DBNull.Value && row["ProductId"] != null)
                                    {
                                        using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn, transaction))
                                        {
                                            cmd.Parameters.AddWithValue("@ButtonIndex", row.Field<int>("ButtonIndex"));
                                            cmd.Parameters.AddWithValue("@ProductId", row.Field<int>("ProductId"));
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                                transaction.Commit();
                            }
                        }
                        Logger.Log("Configuración de productos rápidos guardada", $"Botones configurados: {dt.Rows.Count}");
                        AssignQuickProductsToButtons(); // ¡IMPORTANTE! Recarga los botones en el formulario principal
                        configForm.Close();
                    };
                    bottomPanel.Controls.Add(btnSave);

                    Button btnCancel = new Button { Text = "Cancelar", AutoSize = true, Margin = new Padding(5) };
                    btnCancel.Click += (s, e) => { configForm.Close(); };
                    bottomPanel.Controls.Add(btnCancel);

                    configForm.Controls.Add(dgvQuickProducts);
                    configForm.Controls.Add(bottomPanel); // Añade el panel con los botones al formulario
                    configForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error al configurar productos rápidos", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al configurar productos rápidos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nuevo método para poblar el ComboBox del DataGridView con opción "Ninguno"
        private DataTable GetProductsForComboBox()
        {
            DataTable productsDt = new DataTable();
            productsDt.Columns.Add("Id", typeof(int));
            productsDt.Columns.Add("Name", typeof(string));

            // Añade una fila para la opción "Ninguno" que permite desasignar un producto
            productsDt.Rows.Add(DBNull.Value, "(Ninguno)");

            using (SQLiteConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                // Solo productos activos
                string query = "SELECT Id, Name FROM Products WHERE Active = 1 ORDER BY Name";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productsDt.Rows.Add(reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                }
            }
            return productsDt;
        }

        private void ShowSaleSummary_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar columnas de saleItemsDataTable
                if (saleItemsDataTable == null || saleItemsDataTable.Columns.Count == 0)
                {
                    Logger.Log("Error en ShowSaleSummary_Click", "saleItemsDataTable es nulo o no tiene columnas");
                    MessageBox.Show("No hay ítems de venta para mostrar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Logger.Log("ShowSaleSummary_Click", $"Columnas de saleItemsDataTable: {string.Join(", ", saleItemsDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
                if (!saleItemsDataTable.Columns.Contains("CódigoProducto") || !saleItemsDataTable.Columns.Contains("NombreProducto") ||
                    !saleItemsDataTable.Columns.Contains("Cantidad") || !saleItemsDataTable.Columns.Contains("Unidad") ||
                    !saleItemsDataTable.Columns.Contains("PrecioUnitario") || !saleItemsDataTable.Columns.Contains("Subtotal"))
                {
                    Logger.Log("Error en ShowSaleSummary_Click", "Faltan columnas esperadas en saleItemsDataTable");
                    throw new ArgumentException("Faltan columnas esperadas en saleItemsDataTable");
                }

                decimal total = saleItemsDataTable.AsEnumerable().Sum(row => Convert.ToDecimal(row["Subtotal"], CultureInfo.InvariantCulture));
                decimal discount = 0; // Ajusta según tu lógica
                string paymentMethod = "Efectivo"; // Ajusta según tu lógica
                SaleSummaryForm summaryForm = new SaleSummaryForm(saleItemsDataTable, total, discount, paymentMethod);
                summaryForm.ShowDialog();
                Logger.Log("Resumen de venta mostrado", $"Total={total}, Descuento={discount}, Método de pago={paymentMethod}");
            }
            catch (Exception ex)
            {
                Logger.Log("Error al mostrar SaleSummaryForm", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al mostrar el resumen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeSaleItemsDataTable()
        {
            saleItemsDataTable = new DataTable();
            saleItemsDataTable.Columns.Add("IdProducto", typeof(int));
            saleItemsDataTable.Columns.Add("Código", typeof(string));
            saleItemsDataTable.Columns.Add("Nombre", typeof(string));
            saleItemsDataTable.Columns.Add("PrecioUnitario", typeof(decimal));
            saleItemsDataTable.Columns.Add("Cantidad", typeof(decimal));
            saleItemsDataTable.Columns.Add("Unidad", typeof(string));
            saleItemsDataTable.Columns.Add("Subtotal", typeof(decimal));
            dgvSaleItems.DataSource = saleItemsDataTable;
        }


        // Este método maneja el clic de los botones btnProductQuick1 a btnProductQuick10

        private void btnProductQuick_Click(object sender, EventArgs e)
        {
            try
            {
                Button clickedButton = sender as Button;
                if (clickedButton == null || clickedButton.Tag == null)
                {
                    Logger.Log("Advertencia", "Clic en botón rápido sin Tag o botón nulo.");
                    return;
                }

                // Recupera el objeto Product completo del Tag
                Product product = clickedButton.Tag as Product;

                if (product != null)
                {
                    // Rellenar los campos del formulario principal
                    txtSearchProductCode.Text = product.Code;
                    txtSaleProductName.Text = product.Name;
                    lblSaleProductUnit.Text = product.Unit;
                    txtSaleProductPrice.Text = product.PricePerUnit.ToString("N" + ConfiguracionUsuario.CurrencyDecimals, CultureInfo.InvariantCulture);
                    txtRemainingStock.Text = product.Stock.ToString("N" + ConfiguracionUsuario.WeightDecimals, CultureInfo.InvariantCulture);

                    // Lógica para la cantidad inicial (similar a lo que ya tenías)
                    string weightFormat = "N" + ConfiguracionUsuario.WeightDecimals;
                    if (decimal.TryParse(txtWeightDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight) && weight > 0)
                    {
                        txt1Quantity.Text = weight.ToString(weightFormat, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        txt1Quantity.Text = 1.ToString(weightFormat, CultureInfo.InvariantCulture);
                    }

                    // Llama a AddSaleItem con el objeto Product
                    // Necesitarías un método AddSaleItem(Product product, decimal quantity)
                    // Si UpdateSaleTable se encarga de añadirlo al dgv, simplemente llama a UpdateSaleTable()
                    btnAddSaleItem_Click(this, EventArgs.Empty);

                    Logger.Log("Info", $"Producto rápido agregado: Código={product.Code}, Nombre={product.Name}");
                }
                else
                {
                    Logger.Log("Advertencia", "El Tag del botón rápido no contiene un objeto Product válido.");
                    MessageBox.Show("Error: El botón rápido no tiene un producto configurado correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error", $"Error al procesar clic en producto rápido: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show("Error al procesar el producto rápido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeDatabase()
        {
            try
            {
                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                CREATE TABLE IF NOT EXISTS QuickProducts (
                    ButtonIndex INTEGER PRIMARY KEY,
                    ProductId INTEGER,
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                )";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.Log("Inicialización de base de datos", "Tabla QuickProducts creada o verificada");
                    }

                    // Verificar si QuickProducts está vacía
                    query = "SELECT COUNT(*) FROM QuickProducts";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        long count = (long)cmd.ExecuteScalar();
                        if (count == 0)
                        {
                            // Poblar con los primeros 10 productos activos
                            query = "SELECT Id FROM Products WHERE Active = 1 ORDER BY Id LIMIT 10";
                            using (SQLiteCommand selectCmd = new SQLiteCommand(query, conn))
                            {
                                using (SQLiteDataReader reader = selectCmd.ExecuteReader())
                                {
                                    int buttonIndex = 1;
                                    while (reader.Read())
                                    {
                                        query = "INSERT INTO QuickProducts (ButtonIndex, ProductId) VALUES (@ButtonIndex, @ProductId)";
                                        using (SQLiteCommand insertCmd = new SQLiteCommand(query, conn))
                                        {
                                            insertCmd.Parameters.AddWithValue("@ButtonIndex", buttonIndex);
                                            insertCmd.Parameters.AddWithValue("@ProductId", reader.GetInt32(0));
                                            insertCmd.ExecuteNonQuery();
                                            Logger.Log("Inicialización de QuickProducts", $"Asignado ButtonIndex={buttonIndex}, ProductId={reader.GetInt32(0)}");
                                            buttonIndex++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error al inicializar base de datos", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al inicializar la base de datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '.' && (sender == txtProductPrice || sender == txtStock || sender == txtMinimumStock))
            {
                string text = (sender as TextBox).Text;
                int decimalPlaces = text.Contains(".") ? text.Split('.')[1].Length : 0;
                int maxDecimals = (sender == txtProductPrice) ? ConfiguracionUsuario.CurrencyDecimals : ConfiguracionUsuario.WeightDecimals;
                if (decimalPlaces >= maxDecimals)
                {
                    e.Handled = true;
                }
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT Id, Code, Name, Unit, PricePerUnit, Stock FROM Products WHERE Active = 1", conn))
                    {
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable productsTable = new DataTable();
                            adapter.Fill(productsTable);
                            if (dgvProducts.InvokeRequired)
                            {
                                dgvProducts.BeginInvoke(new Action(() =>
                                {
                                    dgvProducts.DataSource = null;
                                    dgvProducts.DataSource = productsTable;
                                    Logger.Log("LoadProducts", $"Tabla de productos cargada con {productsTable.Rows.Count} ítems [2025-09-22 02:00:00 -05]");
                                }));
                            }
                            else
                            {
                                dgvProducts.DataSource = null;
                                dgvProducts.DataSource = productsTable;
                                Logger.Log("LoadProducts", $"Tabla de productos cargada con {productsTable.Rows.Count} ítems [2025-09-22 02:00:00 -05]");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error en LoadProducts", $"{ex.Message}\nStackTrace: {ex.StackTrace} [2025-09-22 02:00:00 -05]");
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchName = txtSearchProduct.Text.Trim();
                if (string.IsNullOrEmpty(searchName)) return;

                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT Id, Code, Name, PricePerUnit, Unit, Stock, MinimumStock
                FROM Products
                WHERE Name LIKE @Name AND Active = 1";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", $"%{searchName}%");
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentFoundProductId = reader.GetInt32(reader.GetOrdinal("Id"));
                                currentFoundProductCode = reader.GetString(reader.GetOrdinal("Code")).PadLeft(6, '0');
                                currentFoundProductName = reader.GetString(reader.GetOrdinal("Name"));
                                currentFoundProductPrice = reader.GetDecimal(reader.GetOrdinal("PricePerUnit"));

                                string weightFormat = "N" + ConfiguracionUsuario.WeightDecimals;
                                string priceFormat = "N" + ConfiguracionUsuario.CurrencyDecimals;
                                txtProductId.Text = currentFoundProductId.ToString();
                                txtSearchProductCode.Text = currentFoundProductCode;
                                txtSaleProductName.Text = currentFoundProductName;
                                txtSaleProductPrice.Text = currentFoundProductPrice.ToString(priceFormat, CultureInfo.InvariantCulture);
                                txtStock.Text = reader.GetDecimal(reader.GetOrdinal("Stock")).ToString(weightFormat, CultureInfo.InvariantCulture);
                                txtMinimumStock.Text = reader.GetDecimal(reader.GetOrdinal("MinimumStock")).ToString(weightFormat, CultureInfo.InvariantCulture);
                                cboProductUnit.Text = reader.GetString(reader.GetOrdinal("Unit"));
                                txtRemainingStock.Text = reader.GetDecimal(reader.GetOrdinal("Stock")).ToString(weightFormat, CultureInfo.InvariantCulture);
                                txtRemainingStock.ForeColor = reader.GetDecimal(reader.GetOrdinal("Stock")) <= reader.GetDecimal(reader.GetOrdinal("MinimumStock")) ? Color.Red : Color.Black;
                                cboWeightUnit.Text = reader.GetString(reader.GetOrdinal("Unit"));
                                cboWeightUnit.Enabled = false;

                                if (decimal.TryParse(txtWeightDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight) && weight > 0)
                                {
                                    txt1Quantity.Text = weight.ToString(weightFormat, CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    txt1Quantity.Text = 1.ToString(weightFormat, CultureInfo.InvariantCulture);
                                }

                                Logger.Log("Producto encontrado por nombre", $"Name={currentFoundProductName}, Code={currentFoundProductCode}, Quantity={txt1Quantity.Text}");
                            }
                            else
                            {
                                Logger.Log("Búsqueda por nombre", $"No se encontró producto con nombre: {searchName}");
                                ClearSaleItemFields();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error en txtSearchProduct_TextChanged", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al buscar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // -----------------------------------------------------
        // ⭐ MÉTODO PARA CARGAR LOS DATOS DE USUARIO CORRECTAMENTE
        // ----------------------------------------------------- 
        private void LoadUserData()
        {
            string query = "SELECT Id, Username, IsAdmin, Active, Expires FROM Users";
            using (var conn = DatabaseHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            userDataTable.Clear();
                            adapter.Fill(userDataTable);
                            dgvUsers.DataSource = userDataTable;

                            dgvUsers.AutoGenerateColumns = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar los datos de usuario: {ex.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializePaymentMethods()
        {
            cboPaymentMethod.Items.Clear();
            cboPaymentMethod.Items.AddRange(new string[] { "Efectivo", "Tarjeta", "Transferencia" });
            cboPaymentMethod.SelectedIndex = 0; // Selecciona "Efectivo" por defecto
        }
        private bool IsWeightBased(string unit)
        {
            return unit.ToLower() == "kg" || unit.ToLower() == "gr" || unit.ToLower() == "lb";
        }

        // -----------------------------------------------------
        // Método Auxiliar para Inicializar DataGridViews (Necesario)
        // -----------------------------------------------------

        private void InitializeUserInterface()
        {
            try
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] InitializeUserInterface completado\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en InitializeUserInterface: {ex.Message}\n");
                MessageBox.Show($"Error al inicializar la interfaz: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDataGridViews()
        {
            dgvSaleItems.AutoGenerateColumns = false;
            dgvSaleItems.Columns.Clear();
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IdProducto",
                HeaderText = "ID Producto",
                Name = "IdProducto",
                Visible = false
            });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Código",
                HeaderText = "Código",
                Name = "Código"
            });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Nombre",
                HeaderText = "Nombre",
                Name = "Nombre",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PrecioUnitario",
                HeaderText = "Precio Unitario",
                Name = "PrecioUnitario",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N" + ConfiguracionUsuario.CurrencyDecimals }
            });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cantidad",
                HeaderText = "Cantidad",
                Name = "Cantidad",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N" + ConfiguracionUsuario.WeightDecimals }
            });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Unidad",
                HeaderText = "Unidad",
                Name = "Unidad"
            });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Subtotal",
                HeaderText = "Subtotal",
                Name = "Subtotal",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N" + ConfiguracionUsuario.CurrencyDecimals }
            });

            dgvSaleItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };
            dgvSaleItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvSaleItems.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvSaleItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvSaleItems.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // Repite este patrón para otros DataGridViews que tengas (ej. dgvSales, dgvClients)
       

        // -----------------------------------------------------
        // Métodos Auxiliares para Cargar Datos
        // -----------------------------------------------------

        private void LoadProductData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT Id, Code, Name, Unit, PricePerUnit, MinimumStock, Stock FROM Products WHERE Active = 1", conn))
                    {
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable productsTable = new DataTable();
                            adapter.Fill(productsTable);
                            dgvProducts.DataSource = productsTable;
                        }
                    }
                }
                Logger.Log("Productos cargados", "Datos de productos cargados en dgvProducts");
            }
            catch (Exception ex)
            {
                Logger.Log("Error al cargar productos", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDgvProducts()
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.Columns.Clear();
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Code", HeaderText = "Código", DataPropertyName = "Code" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Nombre", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Unit", HeaderText = "Unidad", DataPropertyName = "Unit" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "PricePerUnit", HeaderText = "Precio U.", DataPropertyName = "PricePerUnit", DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "MinimumStock", HeaderText = "Stock Mínimo", DataPropertyName = "MinimumStock", DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", DataPropertyName = "Stock", DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvProducts.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Active", HeaderText = "Activo", DataPropertyName = "Active" });
        }
        private void SetColumnVisibilityBasedOnRole()
        {
            // Itera sobre todas las columnas del DataGridView
            foreach (DataGridViewColumn column in dgvUsers.Columns)
            {
                // El nombre de usuario siempre es visible para todos los roles.
                if (column.Name == "Username")
                {
                    column.Visible = true;
                }
                // Todas las demás columnas solo son visibles si el usuario actual es administrador.
                else
                {
                    column.Visible = Session.IsAdmin;
                }
            }
        }
        // -----------------------------------------------------
        // Métodos Auxiliares para Manipulación de Productos
        // -----------------------------------------------------

        private int selectedSaleItemIdColumn = -1; // Variable para guardar el ID del producto seleccionado para actualizar/eliminar


        // -----------------------------------------------------
        // Métodos Auxiliares para Manipulación de Usuarios (vacíos por ahora)
        // -----------------------------------------------------
        private void ClearUserFields()
        {
            txtUsernameUser.Text = string.Empty;
            txtPasswordUser.Text = string.Empty;
            chkIsAdminUser.Checked = false;
            chkActiveUser.Checked = true;
            chkUserExpires.Checked = false;
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker1.Enabled = false;
            dateTimePicker1.Visible = false;
        }

        // -----------------------------------------------------
        // Lógica del Modo Demo de Balanza
        // -----------------------------------------------------
        private void DemoWeightTimer_Tick(object sender, EventArgs e)
        {
            if (balanzaSimulator != null)
            {
                balanzaSimulator.UpdateWeight();
            }
        }


        private void StartDemoMode()
        {
            if (balanzaSimulator != null)
            {
                balanzaSimulator.Start();
                lblConnectionStatus.Text = "Modo Demo";
                lblConnectionStatus.ForeColor = Color.Orange;
            }
        }

        // -----------------------------------------------------
        // Lógica de Conexión/Desconexión de Balanza Real
        // -----------------------------------------------------
        private void ShowDGVColumnNames()
        {
            string columnNames = "Nombres de las columnas en dgvProducts:\n";
            foreach (DataGridViewColumn column in dgvProducts.Columns)
            {
                columnNames += column.Name + "\n";
            }
            MessageBox.Show(columnNames);
        }
        // Método para refrescar la lista de puertos serie disponibles
        private void RefreshSerialPorts()
        {
            cmbPorts.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] No se encontraron puertos seriales\n");
                MessageBox.Show("No se encontraron puertos seriales.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                cmbPorts.Items.AddRange(ports);
                if (cmbPorts.Items.Count > 0)
                {
                    cmbPorts.SelectedIndex = 0;
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Puertos cargados: {string.Join(", ", ports)}\n");
                }
            }
        }

        // Método para intentar conectar a la balanza
        private void ConnectBalanza()
        {
            if (cmbPorts == null || cmbPorts.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona un puerto COM.", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedPort = cmbPorts.SelectedItem.ToString();
            if (serialPort != null && serialPort.IsOpen)
            {
                DisconnectBalanza();
            }
            serialPort = new SerialPort(selectedPort);

            // ⭐ Cargar los parámetros desde la configuración del usuario
            try
            {
                serialPort.BaudRate = ConfiguracionUsuario.BaudRate;
                serialPort.Parity = ConfiguracionUsuario.Parity;
                serialPort.DataBits = ConfiguracionUsuario.DataBits;
                serialPort.StopBits = ConfiguracionUsuario.StopBits;
                serialPort.Handshake = Handshake.None; // Este valor puede seguir fijo si siempre es el mismo

                serialPort.Open();
                serialPort.DataReceived += SerialPort_DataReceived;
                // ... el resto de tu código de conexión
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la balanza: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisconnectBalanza();
            }
        }
        private void LoadSerialPorts()
        {
            cmbPorts.Items.Clear();
            cmbPorts.Items.AddRange(SerialPort.GetPortNames());
            if (cmbPorts.Items.Count > 0)
                cmbPorts.SelectedIndex = 0;
        }
        // Método para desconectar de la balanza
        private void DisconnectBalanza()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.DataReceived -= SerialPort_DataReceived; // Desasociar el evento
                serialPort.Close();
                serialPort.Dispose();
                serialPort = null; // Liberar la instancia
            }
            if (btnConnectBalanza != null) btnConnectBalanza.Enabled = true;
            if (btnDisconnectBalanza != null) btnDisconnectBalanza.Enabled = false;
            if (cmbPorts != null) cmbPorts.Enabled = true;
            MessageBox.Show("Desconectado de la balanza.", "Desconexión", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Evento que se dispara cuando se reciben datos del puerto serie
        private void DisconnectSerialPort()
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.DataReceived -= SerialPort_DataReceived;
                    serialPort.Close();
                    Logger.Log("Puerto serial cerrado", $"Puerto: {ConfiguracionUsuario.SerialPort}");
                }
                serialPort?.Dispose();
                serialPort = null;
            }
            catch (Exception ex)
            {
                Logger.Log("Error al cerrar puerto serial", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al cerrar puerto serial: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Este método simula la lectura de un peso nuevo de la balanza
        private void SimulateNewWeightReading()
        {
            // Genera un peso aleatorio entre 0.0 y 5.0 kg
            decimal newDemoWeight = (decimal)(new Random().NextDouble() * 5.0);

            // ⭐ CLAVE: Redondea el peso con el número de decimales de la configuración
            newDemoWeight = Math.Round(newDemoWeight, ConfiguracionUsuario.WeightDecimals);

            // ✅ La lógica clave: Si el nuevo peso es diferente del anterior...
            if (newDemoWeight != stableWeight)
            {
                stableWeight = newDemoWeight;
                txtWeightDisplay.Text = stableWeight.ToString($"F{ConfiguracionUsuario.WeightDecimals}");
                txtWeightDisplay.BackColor = System.Drawing.Color.White;
                demoWeightTimer.Stop();
                demoWeightTimer.Start();
            }
         }
        // -----------------------------------------------------
        // Manejadores de Eventos de Botones y Controles
        // -----------------------------------------------------

        private void MainScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Asegúrate de desconectar la balanza si está conectada al cerrar el formulario
            DisconnectBalanza();
            // Si el MainScreen se cierra, podemos volver a mostrar el LoginScreen
            // o cerrar la aplicación completamente.
            // if (loginScreen != null)
            // {
            //     loginScreen.Show();
            // }
            Application.Exit(); // Cierra la aplicación cuando se cierra la ventana principal
        }

        private void tabPageSettings_Click(object sender, EventArgs e)
        {
            // Lógica para cuando se hace clic en la pestaña de Configuración
        }

        private void txtProductPricePerKilo_TextChanged(object sender, EventArgs e)
        {
            // Este evento podría ser obsoleto si estás usando txtProductPrice.
            // Si existe en tu MainScreen.Designer.cs para un control llamado 'txtProductPricePerKilo',
            // y no lo usas, puedes eliminar su asignación en el diseñador (propiedades -> eventos -> TextChanged)
            // o simplemente dejar el método vacío.
        }
        
        private void btnSetTare_Click(object sender, EventArgs e)
        {

        }

        private void btnClearTare_Click(object sender, EventArgs e)
        {

        }

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            string code = txtSearchProductCode.Text.Trim().Replace("_", "");
            code = code.PadLeft(6, '0');
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Por favor, ingrese un código de producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SQLiteConnection conn = DatabaseHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Code, Name, PricePerUnit, Unit, Stock FROM Products WHERE Code = @Code";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Code", code);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtSaleProductName.Text = reader.GetString(reader.GetOrdinal("Name"));
                                txtSaleProductPrice.Text = reader.GetDecimal(reader.GetOrdinal("PricePerUnit")).ToString("F2");
                                lblSaleProductUnit.Text = reader.GetString(reader.GetOrdinal("Unit"));
                                txt1Quantity.Text = "1";
                                txtRemainingStock.Text = reader.GetDecimal(reader.GetOrdinal("Stock")).ToString("F2");
                            }
                            else
                            {
                                MessageBox.Show("Producto no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtSaleProductName.Text = "";
                                txtSaleProductPrice.Text = "";
                                lblSaleProductUnit.Text = "";
                                txt1Quantity.Text = "";
                                txtRemainingStock.Text = "";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter writer = new StreamWriter("debug.log", true))
                    {
                        writer.WriteLine($"[{DateTime.Now}] Error al buscar el producto: {ex.Message}");
                    }
                    MessageBox.Show($"Error al buscar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

        // Nuevo método para buscar el producto en la base de datos
        private DataTable SearchProductByCode(string code)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = DatabaseHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    // La columna para el código es "Code" en tu DB y "txtSaleItemIdColumn" en tu UI de gestión
                    string query = "SELECT Id, Code, Name, \"PricePerUnit\", Unit, Active FROM Products WHERE Code = @Code LIMIT 1";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Code", code);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al buscar producto por código: {ex.Message}", "Error de Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            return dt;
        }

        
        private bool GetProductQuantityAndUnit(out decimal quantity, out string quantityUnit)
        {
            quantity = 0;
            quantityUnit = currentFoundProductUnit;

            string unit = currentFoundProductUnit.ToLower();
            string quantityText;

            if (unit == "kg" || unit == "gr" || unit == "lb")
            {
                quantityText = txtWeightDisplay.Text;
            }
            else
            {
                quantityText = txt1Quantity.Text;
            }

            if (string.IsNullOrWhiteSpace(quantityText))
            {
                MessageBox.Show("La cantidad no puede estar vacía. Por favor, ingrese un número válido.", "Error de Cantidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(quantityText.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out quantity) || quantity <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número válido mayor que cero.", "Error de Cantidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void txtSearchProductCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchCode = txtSearchProductCode.Text.Trim();
                if (string.IsNullOrEmpty(searchCode)) return;

                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT Id, Code, Name, PricePerUnit, Unit, Stock, MinimumStock
                FROM Products
                WHERE Code = @Code AND Active = 1";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Code", searchCode.PadLeft(6, '0'));
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentFoundProductId = reader.GetInt32(reader.GetOrdinal("Id"));
                                currentFoundProductCode = reader.GetString(reader.GetOrdinal("Code")).PadLeft(6, '0');
                                currentFoundProductName = reader.GetString(reader.GetOrdinal("Name"));
                                currentFoundProductPrice = reader.GetDecimal(reader.GetOrdinal("PricePerUnit"));

                                string weightFormat = "N" + ConfiguracionUsuario.WeightDecimals;
                                string priceFormat = "N" + ConfiguracionUsuario.CurrencyDecimals;
                                txtProductId.Text = currentFoundProductId.ToString();
                                txtSearchProductCode.Text = currentFoundProductCode;
                                txtSaleProductName.Text = currentFoundProductName;
                                txtSaleProductPrice.Text = currentFoundProductPrice.ToString(priceFormat, CultureInfo.InvariantCulture);
                                txtStock.Text = reader.GetDecimal(reader.GetOrdinal("Stock")).ToString(weightFormat, CultureInfo.InvariantCulture);
                                txtMinimumStock.Text = reader.GetDecimal(reader.GetOrdinal("MinimumStock")).ToString(weightFormat, CultureInfo.InvariantCulture);
                                cboProductUnit.Text = reader.GetString(reader.GetOrdinal("Unit"));
                                txtRemainingStock.Text = reader.GetDecimal(reader.GetOrdinal("Stock")).ToString(weightFormat, CultureInfo.InvariantCulture);
                                txtRemainingStock.ForeColor = reader.GetDecimal(reader.GetOrdinal("Stock")) <= reader.GetDecimal(reader.GetOrdinal("MinimumStock")) ? Color.Red : Color.Black;
                                cboWeightUnit.Text = reader.GetString(reader.GetOrdinal("Unit"));
                                cboWeightUnit.Enabled = false;

                                if (decimal.TryParse(txtWeightDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight) && weight > 0)
                                {
                                    txt1Quantity.Text = weight.ToString(weightFormat, CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    txt1Quantity.Text = 1.ToString(weightFormat, CultureInfo.InvariantCulture);
                                }

                                Logger.Log("Producto encontrado", $"Code={currentFoundProductCode}, Name={currentFoundProductName}, Quantity={txt1Quantity.Text}");
                            }
                            else
                            {
                                Logger.Log("Búsqueda de producto", $"No se encontró producto con código: {searchCode}");
                                ClearSaleItemFields();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error en txtSearchProductCode_TextChanged", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al buscar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddSaleItem_Click(object sender, EventArgs e)
        {
            try
            {
                string productCode = txtSearchProductCode.Text.Trim().PadLeft(6, '0');
                string quantityText = txt1Quantity.Text.Trim();

                if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(quantityText))
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnAddSaleItem_Click: Código de producto o cantidad vacíos: Código={productCode}, Cantidad={quantityText}\n");
                    MessageBox.Show("Código de producto o cantidad vacíos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(quantityText, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal quantity) || quantity <= 0)
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnAddSaleItem_Click: Cantidad inválida: {quantityText}\n");
                    MessageBox.Show($"Cantidad inválida: {quantityText}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT Id, Code, Name, Unit, PricePerUnit, Stock FROM Products WHERE Code = @Code AND Active = 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@Code", productCode);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                long productId = reader.GetInt64(0);
                                string name = reader.GetString(2);
                                string unit = reader.GetString(3);
                                decimal pricePerUnit = reader.GetDecimal(4);
                                decimal stock = reader.GetDecimal(5);

                                if (quantity > stock)
                                {
                                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnAddSaleItem_Click: Stock insuficiente para {name}. Disponible: {stock}\n");
                                    MessageBox.Show($"Stock insuficiente para {name}. Disponible: {stock}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                bool itemExists = false;
                                foreach (DataRow row in saleItemsDataTable.Rows)
                                {
                                    if (row["Código"].ToString() == productCode)
                                    {
                                        decimal existingQuantity = Convert.ToDecimal(row["Cantidad"]);
                                        row["Cantidad"] = existingQuantity + quantity;
                                        row["Subtotal"] = (existingQuantity + quantity) * pricePerUnit;
                                        itemExists = true;
                                        break;
                                    }
                                }

                                if (!itemExists)
                                {
                                    decimal subtotal = quantity * pricePerUnit;
                                    DataRow row = saleItemsDataTable.NewRow();
                                    row["IdProducto"] = productId;
                                    row["Código"] = productCode;
                                    row["Nombre"] = name;
                                    row["PrecioUnitario"] = pricePerUnit;
                                    row["Cantidad"] = quantity;
                                    row["Unidad"] = unit;
                                    row["Subtotal"] = subtotal;
                                    saleItemsDataTable.Rows.Add(row);
                                }

                                UpdateSaleTable();
                                txtSearchProductCode.Text = "";
                                txt1Quantity.Text = "";
                                txtSaleProductName.Text = "";
                                lblSaleProductUnit.Text = "";
                                txtSaleProductPrice.Text = "";
                                txtRemainingStock.Text = "";
                                File.AppendAllText("debug.log", $"[{DateTime.Now}] btnAddSaleItem_Click: Ítem añadido/actualizado: Código={productCode}, Nombre={name}, Cantidad={quantity}, Subtotal={quantity * pricePerUnit}\n");
                            }
                            else
                            {
                                File.AppendAllText("debug.log", $"[{DateTime.Now}] btnAddSaleItem_Click: Producto con código {productCode} no encontrado o no activo\n");
                                MessageBox.Show($"Producto con código {productCode} no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnAddSaleItem_Click: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                MessageBox.Show($"Error al añadir ítem: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            if (!char.IsControl(e.KeyChar))
            {
                txtWeightDisplay.Select(txtWeightDisplay.Text.Length, 0);
            }
        }
        private void ClearProductSearchFields()
        {
            // Asegúrate de que estos controles existan en tu formulario
            // Si no usas txtSearchProductCode, txtWeightDisplay, txtQuantity, etc., puedes omitir esas líneas.
            if (txtSearchProductCode != null) txtSearchProductCode.Clear();
            // Nota: Si txtWeightDisplay es un Label o no debe ser editable, podrías usar .Text = "0.000 kg";
            // Si es un TextBox y muestra el peso de la balanza
            if (txtWeightDisplay != null) txtWeightDisplay.Text = "0.000 kg";

            // Si usas un campo para cantidad manual, asegúrate de que txtQuantity exista
  

            if (lblSaleProductUnit != null) lblSaleProductUnit.Text = "";
            if (txtSaleProductPrice != null) txtSaleProductPrice.Text = "0.00"; // Precio por unidad en 0
            if (lblSaleProductUnit != null) lblSaleProductUnit.Text = ""; // Vacía la unidad (ej. "kg")

            // Resetear las variables de producto encontrado para la próxima búsqueda
            currentFoundProductId = -1;
            currentFoundProductCode = "";
            currentFoundProductName = "";
            currentFoundProductPrice = 0.0M;
            currentFoundProductUnit = "";
        }

        private decimal CalculateTotalSale()
        {
            decimal total = 0;
            try
            {
                foreach (DataRow row in saleItemsDataTable.Rows)
                {
                    if (saleItemsDataTable.Columns.Contains("SaleItemSubtotalColumn") && row["SaleItemSubtotalColumn"] != DBNull.Value)
                    {
                        total += Convert.ToDecimal(row["SaleItemSubtotalColumn"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular el total de la venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en CalculateTotalSale: {ex.Message}\n");
                return 0;
            }
            return total;
        }

        private void UpdateTotalSaleDisplay()
        {
            decimal total = saleItemsDataTable.AsEnumerable().Sum(row => Convert.ToDecimal(row["SaleItemSubtotalColumn"]));
            txtTotalSale.Text = $"S/. {total:F2}";
        }
        private void btnClosePort_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    MessageBox.Show("Puerto cerrado.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Puerto no está abierto o no inicializado.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cerrar el puerto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeInputValidation()
        {
            System.Windows.Forms.TextBox txtProductPrice = this.txtSaleProductPrice;
            System.Windows.Forms.TextBox txtStock = this.txtRemainingStock;
            System.Windows.Forms.TextBox txtDiscount = this.txtDiscount;

            txtProductPrice.KeyPress += NumericTextBox_KeyPress;
            txtStock.KeyPress += NumericTextBox_KeyPress;
            txtDiscount.KeyPress += NumericTextBox_KeyPress;
        }


        private (string Code, string Name, string Unit, decimal PricePerUnit) GetProductInfo(long productId)
        {
           try
           {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Code, Name, Unit, PricePerUnit FROM Products WHERE Id = @Id";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {   
                        cmd.Parameters.AddWithValue("@Id", productId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                               return (
                               reader["Code"].ToString(),
                               reader["Name"].ToString(),
                               reader["Unit"].ToString(),
                               Convert.ToDecimal(reader["PricePerUnit"])
                                );
                            }
                            throw new Exception($"No se encontró el producto con ID {productId}");
                        }
                    }
                }
           }
               catch (Exception ex)
               {
                   File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al obtener información del producto: {ex.Message}\n");
                  throw;
               }
        }
        private string GetProductUnit(long productId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Unit FROM Products WHERE Id = @Id";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", productId);
                        object result = cmd.ExecuteScalar();
                        return result?.ToString() ?? throw new Exception($"No se encontró la unidad para el producto con ID {productId}");
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al obtener la unidad del producto: {ex.Message}\n");
                throw;
            }
        }

        private void btnFinalizeSale_Click(object sender, EventArgs e)
        {
            try
            {
                decimal total = saleItemsDataTable.AsEnumerable()
                    .Sum(row => row.Field<decimal>("Subtotal"));
                decimal discount = 0; // Ajusta según la lógica de descuentos
                string paymentMethod = "Efectivo"; // Ajusta según el método de pago
                string username = Session.Username; // Asume que Session.Username existe
                int cashRegisterId = 1; // Ajusta según la configuración
                long saleId = DatabaseHelper.SaveSaleToDatabase(saleItemsDataTable, total, discount, paymentMethod, username, cashRegisterId);
                lastSaleId = saleId;
                var saleSummary = new SaleSummaryForm(saleItemsDataTable.Copy(), total, discount, paymentMethod);
                saleSummary.ShowDialog();
                saleItemsDataTable.Clear();
                UpdateSaleTable();
                Logger.Log("Info", $"Venta finalizada, abriendo resumen para SaleId={lastSaleId}");
            }
            catch (Exception ex)
            {
                Logger.Log("Error", $"Error al finalizar venta: {ex.Message}");
                MessageBox.Show("Error al finalizar la venta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewSale_Click(object sender, EventArgs e)
        {
            try
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] btnNewSale_Click: Verificando columnas en saleItemsDataTable: {string.Join(", ", saleItemsDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}\n");

                if (saleItemsDataTable.Rows.Count > 0)
                {
                    decimal total = saleItemsDataTable.AsEnumerable()
                        .Sum(row => row.Field<decimal>("Subtotal"));

                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        using (var transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                long saleId;
                                using (var cmd = new SQLiteCommand(
                                    "INSERT INTO Sales (UserId, SaleDate, Total, PaymentMethod) VALUES (@UserId, @SaleDate, @Total, @PaymentMethod); SELECT last_insert_rowid();", conn))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", Session.UserId); // Reemplaza loggedInUserId
                                    cmd.Parameters.AddWithValue("@SaleDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    cmd.Parameters.AddWithValue("@Total", total);
                                    cmd.Parameters.AddWithValue("@PaymentMethod", cboPaymentMethod.SelectedItem?.ToString() ?? "Efectivo");
                                    saleId = (long)cmd.ExecuteScalar();
                                }

                                foreach (DataRow row in saleItemsDataTable.Rows)
                                {
                                    long productId = Convert.ToInt64(row["IdProducto"]);
                                    decimal quantity = Convert.ToDecimal(row["Cantidad"]);
                                    decimal pricePerUnit = Convert.ToDecimal(row["PrecioUnitario"]);
                                    decimal subtotal = Convert.ToDecimal(row["Subtotal"]);

                                    using (var cmd = new SQLiteCommand(
                                        "INSERT INTO SaleItems (SaleId, ProductId, Quantity, PricePerUnit, Subtotal) VALUES (@SaleId, @ProductId, @Quantity, @PricePerUnit, @Subtotal)", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@SaleId", saleId);
                                        cmd.Parameters.AddWithValue("@ProductId", productId);
                                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                                        cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit);
                                        cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                                        cmd.ExecuteNonQuery();
                                    }

                                    using (var cmd = new SQLiteCommand(
                                        "UPDATE Products SET Stock = Stock - @Quantity WHERE Id = @ProductId", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                                        cmd.Parameters.AddWithValue("@ProductId", productId);
                                        cmd.ExecuteNonQuery();
                                    }

                                    using (var cmd = new SQLiteCommand(
                                        "INSERT INTO StockHistory (ProductId, Quantity, ChangeDate, ChangeType, UserId) VALUES (@ProductId, @Quantity, @ChangeDate, @ChangeType, @UserId)", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@ProductId", productId);
                                        cmd.Parameters.AddWithValue("@Quantity", -quantity);
                                        cmd.Parameters.AddWithValue("@ChangeDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        cmd.Parameters.AddWithValue("@ChangeType", "Venta");
                                        cmd.Parameters.AddWithValue("@UserId", Session.UserId); // Reemplaza loggedInUserId
                                        cmd.ExecuteNonQuery();
                                    }
                                }

                                transaction.Commit();
                                File.AppendAllText("debug.log", $"[{DateTime.Now}] btnNewSale_Click: Venta guardada: ID Venta={saleId}, Ítems={saleItemsDataTable.Rows.Count}, Total={total}\n");
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnNewSale_Click: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                                MessageBox.Show($"Error al guardar venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }

                ClearSaleInterface();
                btnNewSale.Enabled = true;
                File.AppendAllText("debug.log", $"[{DateTime.Now}] btnNewSale_Click: Interfaz limpiada, btnNewSale habilitado\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnNewSale_Click: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                MessageBox.Show($"Error al iniciar nueva venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemoveSaleItem_Click(object sender, EventArgs e)
        {
            if (dgvSaleItems.SelectedRows.Count > 0)
            {
                var confirmResult = MessageBox.Show("¿Está seguro de que desea eliminar este producto de la venta?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    DataGridViewRow selectedRow = dgvSaleItems.SelectedRows[0];

                    if (selectedRow.Cells["Código"].Value != null && selectedRow.Cells["Cantidad"].Value != null)
                    {
                        string productCode = selectedRow.Cells["Código"].Value.ToString().Trim();
                        decimal quantityChange = Convert.ToDecimal(selectedRow.Cells["Cantidad"].Value);

                        // Paso 1: Eliminar el ítem de la tabla de ventas
                        saleItemsDataTable.Rows.RemoveAt(selectedRow.Index);

                        // Paso 2: Recalcular el total
                        UpdateSaleTable();

                        // Paso 3: Actualizar el stock en la UI
                        try
                        {
                            UpdateStockInUI(productCode, quantityChange);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al restaurar stock en btnRemoveSaleItem_Click: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                        }

                        MessageBox.Show("Producto eliminado de la venta.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

       
        private decimal GetStableWeight()
{
    try
    {
        if (serialPort == null || !serialPort.IsOpen)
        {
            Logger.Log("GetStableWeight", "Balanza no conectada [2025-09-22 10:20:00 -05]");
            return 0;
        }

        string weightText = txtWeightDisplay.InvokeRequired
            ? (string)txtWeightDisplay.Invoke(new Func<string>(() => txtWeightDisplay.Text))
            : txtWeightDisplay.Text;

        if (decimal.TryParse(weightText, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal weight) && weight > 0)
        {
            return weight;
        }

        Logger.Log("GetStableWeight", $"Peso inválido: {weightText} [2025-09-22 10:20:00 -05]");
        return 0;
    }
    catch (Exception ex)
    {
        Logger.Log("Error en GetStableWeight", $"{ex.Message}\nStackTrace: {ex.StackTrace} [2025-09-22 10:20:00 -05]");
        return 0;
    }
}
        private void btnClearAllItems_Click(object sender, EventArgs e)
        {
            {
                // Llama al método que se encarga de limpiar toda la interfaz de la venta.
                ClearSaleInterface();
            }
        }

        private void UpdateTotalAmount()
        {
            try
            {
                decimal total = saleItemsDataTable.AsEnumerable()
                    .Sum(row => row.Field<decimal>("Subtotal"));
                txtTotalSale.Text = total.ToString($"N{ConfiguracionUsuario.CurrencyDecimals}", CultureInfo.InvariantCulture);
                File.AppendAllText("debug.log", $"[{DateTime.Now}] UpdateTotalAmount: Total actualizado a {total}\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en UpdateTotalAmount: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
            }
        }
        private void RefreshProductTable()
        {
            DataTable productData = DatabaseHelper.ObtenerTodosLosProductos();
            dgvProducts.DataSource = productData;
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var row = dgvProducts.SelectedRows[0];
                txtProductId.Text = row.Cells["Code"].Value?.ToString();
                cboProductUnit.Text = row.Cells["Unit"].Value?.ToString();
                txtStock.Text = row.Cells["Stock"].Value?.ToString();
                txtProductPrice.Text = row.Cells["PricePerUnit"].Value?.ToString();
                txtMinimumStock.Text = row.Cells["MinimumStock"].Value?.ToString();
                chkProductActive.Checked = Convert.ToBoolean(row.Cells["Active"].Value);
                txtProductName.Text = row.Cells["Name"].Value?.ToString();
                Logger.Log("Info", $"Producto seleccionado: Código={txtProductId.Text}, Nombre={txtProductName.Text}");
            }
        }
        private void tabPageProducts_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Este evento podría ser obsoleto o incorrecto si tienes chkProductActive.
            // Si existe en tu MainScreen.Designer.cs para un control llamado 'checkBox1',
            // y no lo usas, puedes eliminar su asignación en el diseñador o dejar el método vacío.
        }

       
        private void panelProductDetails_Paint(object sender, PaintEventArgs e)
        {

        }

        // --- MANEJADORES DE EVENTOS PARA PRODUCTOS ---

        // Asegúrate de agregar esta referencia

        // Método corregido para actualizar el stock en la interfaz de usuario

        private void UpdateStockInUI(string productCode, decimal quantityChange)
        {
            long productId = DatabaseHelper.GetProductIdFromCode(productCode);

            if (productId != -1)
            {
                decimal currentStock = DatabaseHelper.ObtenerStockActual(productId);
                decimal newStock = currentStock + quantityChange;
                decimal minimumStock = DatabaseHelper.ObtenerStockMinimo(productId);

                // ⭐ CLAVE: Usar la variable 'newStock' que contiene el stock actualizado.
                txtRemainingStock.Text = newStock.ToString($"N{ConfiguracionUsuario.WeightDecimals}");

                // ⭐ CLAVE: Usar la variable 'newStock' y especificar el namespace de Color.
                txtRemainingStock.ForeColor = (newStock <= minimumStock) ? System.Drawing.Color.Red : System.Drawing.Color.Black;
            }
            else
            {
                throw new InvalidOperationException("Producto no encontrado en la base de datos.");
            }
        }

        private void UpdateProductInfoForSale(int productId, string productUnit)
        {
            decimal stockActual = DatabaseHelper.ObtenerStockActual(productId);
            decimal stockMinimo = DatabaseHelper.ObtenerStockMinimo(productId);

            decimal quantityInSale = 0;
            foreach (DataRow row in saleItemsDataTable.Rows)
            {
                if (Convert.ToInt64(row["IdProducto"]) == productId)
                {
                    quantityInSale += Convert.ToDecimal(row["Cantidad"]);
                }
            }

            decimal remainingStock = stockActual - quantityInSale;

            string formatoPeso = "N" + ConfiguracionUsuario.WeightDecimals.ToString();
            txtStock.Text = remainingStock.ToString(formatoPeso);
            txtStock.ForeColor = (remainingStock < stockMinimo) ? Color.Red : Color.Black;

            if (remainingStock < stockMinimo)
            {
                MessageBox.Show($"Alerta de stock: El producto '{txtProductName.Text}' tiene {remainingStock.ToString(formatoPeso)} {productUnit}, lo que está por debajo del mínimo ({stockMinimo.ToString(formatoPeso)}).", "Alerta de Stock", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void txtProductId_Enter(object sender, EventArgs e)
        {
            txtProductId.SelectionStart = txtProductId.Text.Length;
        }

        private void UpdateProductDecimals()
        {
            try
            {
                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                UPDATE Products 
                SET PricePerUnit = ROUND(PricePerUnit, @CurrencyDecimals),
                    Stock = ROUND(Stock, @WeightDecimals),
                    MinimumStock = ROUND(MinimumStock, @WeightDecimals)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CurrencyDecimals", ConfiguracionUsuario.CurrencyDecimals);
                        cmd.Parameters.AddWithValue("@WeightDecimals", ConfiguracionUsuario.WeightDecimals);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Logger.Log("Valores de productos actualizados", $"Filas afectadas: {rowsAffected}");
                    }
                }
                LoadProductData();
            }
            catch (Exception ex)
            {
                Logger.Log("Error al actualizar decimales de productos", ex.Message);
                MessageBox.Show($"Error al actualizar decimales de productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearProductControls()
        {
            txtProductId.Text = string.Empty;
            txtProductName.Text = string.Empty;
            txtProductPrice.Text = 0.0M.ToString($"N{ConfiguracionUsuario.CurrencyDecimals}", CultureInfo.InvariantCulture);
            txtStock.Text = 0.0M.ToString($"N{ConfiguracionUsuario.WeightDecimals}", CultureInfo.InvariantCulture);
            txtMinimumStock.Text = 0.0M.ToString($"N{ConfiguracionUsuario.WeightDecimals}", CultureInfo.InvariantCulture);
            cboProductUnit.SelectedIndex = -1;
            chkProductActive.Checked = true;
            selectedSaleItemIdColumn = -1;
        }

        // ⭐ CLAVE 3: Debes añadir este método auxiliar en alguna parte de tu clase
        private decimal GetProductStock(int productId, SQLiteConnection conn, SQLiteTransaction transaction)
        {
            string query = "SELECT Stock FROM Products WHERE Id = @ProductId";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                object result = cmd.ExecuteScalar();
                return (result != null) ? Convert.ToDecimal(result) : 0;
            }
        }

        // ✅ NUEVO MÉTODO DE VERIFICACIÓN
        private bool IsCodeDuplicate(string code, int currentProductId)
        {
            using (SQLiteConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Products WHERE Code = @Code AND Id != @Id";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
                    cmd.Parameters.AddWithValue("@Id", currentProductId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
                string productCode = txtProductId.Text.Trim();
                string productName = txtProductName.Text.Trim();
                string priceText = txtProductPrice.Text.Trim();
                string unit = cboProductUnit.Text.Trim();
                string stockText = txtStock.Text.Trim();
                string minimumStockText = txtMinimumStock.Text.Trim();
                bool isActive = chkProductActive.Checked;

                if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(priceText) || string.IsNullOrWhiteSpace(unit) || string.IsNullOrWhiteSpace(stockText) || string.IsNullOrWhiteSpace(minimumStockText))
                {
                    Logger.Log("Error al agregar producto", "Todos los campos son obligatorios");
                    MessageBox.Show("Todos los campos son obligatorios.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(priceText.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pricePerUnit) || pricePerUnit < 0)
                {
                    Logger.Log("Error al agregar producto", "El precio debe ser un número válido");
                    MessageBox.Show("El precio debe ser un número válido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(stockText.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal stock) || stock < 0)
                {
                    Logger.Log("Error al agregar producto", "El stock debe ser un número válido");
                    MessageBox.Show("El stock debe ser un número válido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(minimumStockText.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal minimumStock) || minimumStock < 0)
                {
                    Logger.Log("Error al agregar producto", "El stock mínimo debe ser un número válido");
                    MessageBox.Show("El stock mínimo debe ser un número válido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                productCode = productCode.PadLeft(6, '0');
                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Products WHERE Code = @Code";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Code", productCode);
                        long count = (long)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            Logger.Log("Error al agregar producto", $"El código '{productCode}' ya existe");
                            MessageBox.Show("El código de producto ya existe. Por favor, usa un código diferente.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Buscar el primer Id disponible
                    query = "SELECT MIN(t1.Id + 1) FROM Products t1 LEFT JOIN Products t2 ON t1.Id + 1 = t2.Id WHERE t2.Id IS NULL";
                    int newId;
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        newId = result == DBNull.Value ? 1 : Convert.ToInt32(result);
                    }

                    pricePerUnit = Math.Round(pricePerUnit, ConfiguracionUsuario.CurrencyDecimals);
                    stock = Math.Round(stock, ConfiguracionUsuario.WeightDecimals);
                    minimumStock = Math.Round(minimumStock, ConfiguracionUsuario.WeightDecimals);

                    query = @"
                INSERT INTO Products (Id, Code, Name, PricePerUnit, Unit, Stock, MinimumStock, Active)
                VALUES (@Id, @Code, @Name, @PricePerUnit, @Unit, @Stock, @MinimumStock, @Active)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", newId);
                        cmd.Parameters.AddWithValue("@Code", productCode);
                        cmd.Parameters.AddWithValue("@Name", productName);
                        cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit);
                        cmd.Parameters.AddWithValue("@Unit", unit);
                        cmd.Parameters.AddWithValue("@Stock", stock);
                        cmd.Parameters.AddWithValue("@MinimumStock", minimumStock);
                        cmd.Parameters.AddWithValue("@Active", isActive ? 1 : 0);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Logger.Log("Producto agregado", $"Filas afectadas: {rowsAffected}, Id={newId}, Code={productCode}");
                    }
                }

                LoadProductData();
                AssignQuickProductsToButtons();
                ClearProductFields();
                MessageBox.Show("Producto agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Log("Error al agregar producto", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al agregar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProducts.SelectedRows.Count == 0)
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnUpdateProduct_Click: No hay producto seleccionado\n");
                    MessageBox.Show("Por favor, seleccione un producto para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow row = dgvProducts.SelectedRows[0];
                if (row.Cells["Id"].Value == null)
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnUpdateProduct_Click: ID de producto no válido o no seleccionado\n");
                    MessageBox.Show("ID de producto no válido o no seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                long productId = Convert.ToInt64(row.Cells["Id"].Value);
                string code = txtProductId.Text.Trim().PadLeft(6, '0'); // Reemplaza txtProductCode
                string name = txtProductName.Text.Trim();
                string unit = cboProductUnit.SelectedItem?.ToString().Trim() ?? ""; // Reemplaza txtProductUnit
                if (string.IsNullOrWhiteSpace(unit))
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnUpdateProduct_Click: Unidad no seleccionada\n");
                    MessageBox.Show("Por favor, seleccione una unidad válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!decimal.TryParse(txtProductPrice.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal pricePerUnit) || pricePerUnit <= 0)
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnUpdateProduct_Click: Precio inválido: {txtProductPrice.Text}\n");
                    MessageBox.Show("Por favor, ingrese un precio válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!decimal.TryParse(txtStock.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal stock) || stock < 0) // Reemplaza txtProductStock
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnUpdateProduct_Click: Stock inválido: {txtStock.Text}\n");
                    MessageBox.Show("Por favor, ingrese un stock válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "UPDATE Products SET Code = @Code, Name = @Name, Unit = @Unit, PricePerUnit = @PricePerUnit, Stock = @Stock WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", productId);
                        cmd.Parameters.AddWithValue("@Code", code);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Unit", unit);
                        cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit);
                        cmd.Parameters.AddWithValue("@Stock", stock);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        File.AppendAllText("debug.log", $"[{DateTime.Now}] btnUpdateProduct_Click: Producto actualizado: ID={productId}, Código={code}, Nombre={name}, Filas afectadas={rowsAffected}\n");
                        MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                LoadProducts();
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnUpdateProduct_Click: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                MessageBox.Show($"Error al actualizar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearProductFields()
        {
            txtProductId.Text = string.Empty;
            txtSearchProductCode.Text = string.Empty;
            txtProductName.Text = string.Empty;
            txtProductPrice.Text = string.Empty;
            txtStock.Text = string.Empty;
            txtMinimumStock.Text = string.Empty;
            cboProductUnit.Text = string.Empty;
            chkProductActive.Checked = true;
        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                if (!dgvProducts.Columns.Contains("Id"))
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Error: La columna 'Id' no existe en dgvProducts\n");
                    MessageBox.Show("Error: La columna 'Id' no está definida en la tabla de productos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var confirmResult = MessageBox.Show("¿Estás seguro de que quieres eliminar este producto?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    int selectedProductId = Convert.ToInt32(dgvProducts.SelectedRows[0].Cells["Id"].Value);

                    using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                    {
                        try
                        {
                            conn.Open();
                            string query = "DELETE FROM Products WHERE Id = @Id";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Id", selectedProductId);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Producto eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadProductData(); // Vuelve a cargar la tabla
                                    ClearProductControls();
                                }
                                else
                                {
                                    MessageBox.Show("No se pudo eliminar el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al eliminar producto: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                            MessageBox.Show($"Error al eliminar producto: {ex.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecciona un producto de la lista para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateSaleTotal()
        {
            try
            {
                decimal total = saleItemsDataTable.AsEnumerable().Sum(row => row.Field<decimal>("SaleItemSubtotalColumn"));
                // Actualizar control de total (por ejemplo, lblTotal)
                txtTotalSale.Text = $"Total: {ConfiguracionUsuario.CurrencySymbol} {total:F2}";
                Logger.Log("UpdateSaleTotal", $"Total actualizado: {total} [2025-09-21 20:02:00 -05]");
            }
            catch (Exception ex)
            {
                Logger.Log("Error en UpdateSaleTotal", $"{ex.Message}\nStackTrace: {ex.StackTrace} [2025-09-21 20:02:00 -05]");
            }
        }
        private void ClearSaleItemFields()
        {
            try
            {
                txtSearchProductCode.Clear();
                txt1Quantity.Clear();
                txtSaleProductName.Text = string.Empty;
                lblSaleProductUnit.Text = string.Empty;
                txtSaleProductPrice.Clear();
                txtRemainingStock.Clear();
                txtDiscount?.Clear();
                cboPaymentMethod.SelectedIndex = -1;
                txtTotalSale.Text = $"Total: {ConfiguracionUsuario.CurrencySymbol} 0.00";
                Logger.Log("ClearSaleItemFields", "Campos de venta limpiados [2025-09-21 22:03:00 -05]");
            }
            catch (Exception ex)
            {
                Logger.Log("Error en ClearSaleItemFields", $"{ex.Message}\nStackTrace: {ex.StackTrace} [2025-09-21 22:03:00 -05]");
            }
         }
        private void txtWeightDisplay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            if (!char.IsControl(e.KeyChar))
            {
                txtWeightDisplay.Select(txtWeightDisplay.Text.Length, 0);
            }
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.SelectedRows.Count > 0)
                {
                    if (!dgvUsers.Columns.Contains("Id"))
                    {
                        Logger.Log("Error", "La columna 'Id' no existe en dgvUsers");
                        MessageBox.Show("Error: La columna 'Id' no está definida en la tabla de usuarios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DataGridViewRow row = dgvUsers.SelectedRows[0];
                    txtId.Text = row.Cells["Id"].Value?.ToString() ?? string.Empty;
                    txtUsernameUser.Text = row.Cells["Username"].Value?.ToString() ?? string.Empty;
                    txtPasswordUser.Text = string.Empty;
                    chkIsAdminUser.Checked = Convert.ToBoolean(row.Cells["IsAdmin"].Value);
                    chkActiveUser.Checked = Convert.ToBoolean(row.Cells["Active"].Value);
                    chkUserExpires.Checked = Convert.ToBoolean(row.Cells["Expires"].Value);

                    if (chkUserExpires.Checked && row.Cells["ExpiryDate"].Value != null && row.Cells["ExpiryDate"].Value != DBNull.Value && DateTime.TryParse(row.Cells["ExpiryDate"].Value.ToString(), out DateTime expiryDate))
                    {
                        dateTimePicker1.Value = expiryDate;
                        dateTimePicker1.Enabled = true;
                        dateTimePicker1.Visible = true;
                    }
                    else
                    {
                        dateTimePicker1.Value = DateTime.Now;
                        dateTimePicker1.Enabled = chkUserExpires.Checked;
                        dateTimePicker1.Visible = chkUserExpires.Checked;
                    }

                    Logger.Log("Fila seleccionada en dgvUsers", $"Id={txtId.Text}, Expires={chkUserExpires.Checked}, ExpiryDate={(row.Cells["ExpiryDate"].Value?.ToString() ?? "NULL")}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error en dgvUsers_SelectionChanged", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al cargar datos del usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<SaleDetail> GetSaleDetailsFromGrid()
        {
            List<SaleDetail> details = new List<SaleDetail>();

            foreach (DataGridViewRow row in dgvSaleItems.Rows)
            {
                if (!row.IsNewRow)
                {
                    details.Add(new SaleDetail
                    {
                        ProductId = Convert.ToInt64(row.Cells["IdProducto"].Value),
                        Quantity = Convert.ToDouble(row.Cells["Cantidad"].Value),
                        UnitPrice = Convert.ToDouble(row.Cells["PrecioUnitario"].Value),
                        LineTotal = Convert.ToDouble(row.Cells["Subtotal"].Value)
                    });
                }
            }
            return details;
        }
        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProducts.Rows[e.RowIndex];

                // Verificar que la columna Id exista
                if (!dgvProducts.Columns.Contains("Id"))
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Error: La columna 'Id' no existe en dgvProducts\n");
                    MessageBox.Show("Error: La columna 'Id' no está definida en la tabla de productos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                txtProductId.Tag = row.Cells["Id"].Value != null ? Convert.ToInt32(row.Cells["Id"].Value) : 0;
                txtProductId.Text = row.Cells["Code"].Value?.ToString() ?? string.Empty;
                txtProductName.Text = row.Cells["Name"].Value?.ToString() ?? string.Empty;
                txtProductPrice.Text = row.Cells["PricePerUnit"].Value != null && decimal.TryParse(row.Cells["PricePerUnit"].Value.ToString(), out decimal price)
                    ? price.ToString("N2")
                    : "0,00";
                txtStock.Text = row.Cells["Stock"].Value != null && decimal.TryParse(row.Cells["Stock"].Value.ToString(), out decimal stock)
                    ? stock.ToString("N2")
                    : "0,00";
                txtMinimumStock.Text = row.Cells["MinimumStock"].Value != null && decimal.TryParse(row.Cells["MinimumStock"].Value.ToString(), out decimal minimumStock)
                    ? minimumStock.ToString("N2")
                    : "0,00";
                cboProductUnit.Text = row.Cells["Unit"].Value?.ToString() ?? string.Empty;

                if (row.Cells["Active"].Value != null)
                {
                    var activeValue = row.Cells["Active"].Value;
                    if (activeValue is bool)
                        chkProductActive.Checked = (bool)activeValue;
                    else if (activeValue is int)
                        chkProductActive.Checked = (int)activeValue != 0;
                    else if (activeValue is string)
                        chkProductActive.Checked = activeValue.ToString() == "1" || activeValue.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);
                    else
                        chkProductActive.Checked = false;
                }
                else
                {
                    chkProductActive.Checked = true;
                }
            }
        }

        // --- MANEJADORES DE EVENTOS PARA USUARIOS (vacíos por ahora) ---
        private void chkUserExpires_CheckedChanged(object sender, EventArgs e)
        {
            // Habilita o deshabilita el DateTimePicker según el estado del CheckBox
            dateTimePicker1.Enabled = chkUserExpires.Checked;
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsernameUser.Text) || string.IsNullOrWhiteSpace(txtPasswordUser.Text))
                {
                    Logger.Log("Error", "Campos de usuario incompletos");
                    MessageBox.Show("Por favor, complete todos los campos requeridos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string query = @"
                    INSERT INTO Users (Username, Password, IsAdmin, Active, Expires, ExpiryDate)
                    VALUES (@Username, @Password, @IsAdmin, @Active, @Expires, @ExpiryDate)";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Username", txtUsernameUser.Text);
                            cmd.Parameters.AddWithValue("@Password", txtPasswordUser.Text); // Considerar encriptar
                            cmd.Parameters.AddWithValue("@IsAdmin", chkIsAdminUser.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@Active", chkActiveUser.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@Expires", chkUserExpires.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@ExpiryDate", chkUserExpires.Checked ? (object)dateTimePicker1.Value : DBNull.Value);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Logger.Log("INSERT usuario ejecutado", $"Filas afectadas: {rowsAffected}");
                        }
                        transaction.Commit();
                    }
                }

                LoadUsers();
            }
            catch (Exception ex)
            {
                Logger.Log("Error al cargar datos de usuarios", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al cargar datos de usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (!Session.IsAdmin)
            {
                MessageBox.Show("Solo los administradores pueden actualizar usuarios.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Por favor, selecciona un usuario para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Convert.ToInt32(txtId.Text);
            string username = txtUsernameUser.Text.Trim();
            string password = txtPasswordUser.Text;
            bool isAdmin = chkIsAdminUser.Checked;
            bool isActive = chkActiveUser.Checked;
            bool expires = chkUserExpires.Checked;
            DateTime expirationDate = dateTimePicker1.Value;

            string query;
            if (string.IsNullOrWhiteSpace(password))
            {
                query = "UPDATE Users SET Username = @Username, IsAdmin = @IsAdmin, Active = @Active, Expires = @Expires, ExpiryDate = @ExpiryDate WHERE Id = @UserId";
            }
            else
            {
                query = "UPDATE Users SET Username = @Username, PasswordHash = @PasswordHash, IsAdmin = @IsAdmin, Active = @Active, Expires = @Expires, ExpiryDate = @ExpiryDate WHERE Id = @UserId";
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Username", username);
                        if (!string.IsNullOrWhiteSpace(password))
                        {
                            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                            cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        }
                        cmd.Parameters.AddWithValue("@IsAdmin", isAdmin ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Active", isActive ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Expires", expires ? 1 : 0);
                        cmd.Parameters.AddWithValue("@ExpiryDate", expires ? (object)expirationDate : DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Usuario actualizado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadUsers();
                            ClearUserFields();
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al actualizar usuario: {ex.Message}\n");
                    MessageBox.Show($"Error al actualizar usuario: {ex.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (!Session.IsAdmin)
            {
                MessageBox.Show("Solo los administradores pueden eliminar usuarios.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Por favor, selecciona un usuario para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Convert.ToInt32(txtId.Text);

            var result = MessageBox.Show($"¿Estás seguro de que deseas eliminar al usuario con ID {userId}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM Users WHERE Id = @UserId";

                using (var conn = DatabaseHelper.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        using (var cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Usuario eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadUsers();
                                ClearUserFields();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al eliminar usuario: {ex.Message}\n");
                        MessageBox.Show($"Error al eliminar usuario: {ex.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnClearUserFields_Click(object sender, EventArgs e)
        {
            ClearUserFields();
        }

      
        private void HighlightEmptyFields()
        {
            txtProductId.BackColor = string.IsNullOrWhiteSpace(txtProductId.Text) ? Color.FromArgb(255, 204, 204) : Color.White;
            txtProductName.BackColor = string.IsNullOrWhiteSpace(txtProductName.Text) ? Color.FromArgb(255, 204, 204) : Color.White;
            txtProductPrice.BackColor = string.IsNullOrWhiteSpace(txtProductPrice.Text) ? Color.FromArgb(255, 204, 204) : Color.White;
            cboProductUnit.BackColor = cboProductUnit.SelectedItem == null ? Color.FromArgb(255, 204, 204) : Color.White;
            txtStock.BackColor = string.IsNullOrWhiteSpace(txtStock.Text) ? Color.FromArgb(255, 204, 204) : Color.White;
            txtMinimumStock.BackColor = string.IsNullOrWhiteSpace(txtMinimumStock.Text) ? Color.FromArgb(255, 204, 204) : Color.White;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar parámetros del puerto serial
                if (!ValidateSerialPortParameters(out string serialPort, out int baudRate, out Parity parity, out int dataBits, out StopBits stopBits))
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Error: Parámetros del puerto serial inválidos\n");
                    MessageBox.Show("Los parámetros del puerto serial no son válidos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validar decimales de peso
                if (!int.TryParse(numericUpDownDecimalesBalanza.Text, out int decimalesPeso) || decimalesPeso < 0 || decimalesPeso > 4)
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Error: Decimales de peso inválidos ({numericUpDownDecimalesBalanza.Text})\n");
                    MessageBox.Show("Los decimales de peso no son válidos (deben estar entre 0 y 4).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validar decimales de precio
                if (!int.TryParse(numericUpDownDecimalesPrecio.Text, out int decimalesPrecio) || decimalesPrecio < 0 || decimalesPrecio > 4)
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Error: Decimales de precio inválidos ({numericUpDownDecimalesPrecio.Text})\n");
                    MessageBox.Show("Los decimales de precio no son válidos (deben estar entre 0 y 4).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validar símbolo de moneda
                if (string.IsNullOrEmpty(txtCurrencySymbol.Text))
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Error: Símbolo de moneda inválido\n");
                    MessageBox.Show("El símbolo de moneda no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string currencySymbol = txtCurrencySymbol.Text;

                // Guardar configuraciones
                ConfiguracionUsuario.SaveSettings(
                    decimalesPeso,
                    decimalesPrecio,
                    currencySymbol,
                    baudRate,
                    serialPort,
                    parity,
                    dataBits,
                    stopBits
                );

                File.AppendAllText("debug.log", $"[{DateTime.Now}] Configuración guardada: Peso={decimalesPeso}, Precio={decimalesPrecio}, Moneda={currencySymbol}, Puerto={serialPort}, BaudRate={baudRate}\n");
                MessageBox.Show("Configuración guardada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al guardar configuración: {ex.Message}\n");
                MessageBox.Show($"Error al guardar configuración: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUsers()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Username", typeof(string));
            dt.Columns.Add("IsAdmin", typeof(bool));
            dt.Columns.Add("Active", typeof(bool));
            dt.Columns.Add("Expires", typeof(bool));
            dt.Columns.Add("ExpiryDate", typeof(DateTime));

            using (SQLiteConnection conn = DatabaseHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Id, Username, IsAdmin, Active, Expires, ExpiryDate FROM Users";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow newRow = dt.NewRow();
                                newRow["Id"] = reader.GetInt32(reader.GetOrdinal("Id"));
                                newRow["Username"] = reader.GetString(reader.GetOrdinal("Username"));
                                newRow["IsAdmin"] = reader.GetBoolean(reader.GetOrdinal("IsAdmin"));
                                newRow["Active"] = reader.GetBoolean(reader.GetOrdinal("Active"));
                                newRow["Expires"] = reader.GetBoolean(reader.GetOrdinal("Expires"));
                                if (reader.IsDBNull(reader.GetOrdinal("ExpiryDate")))
                                {
                                    newRow["ExpiryDate"] = DBNull.Value;
                                }
                                else
                                {
                                    newRow["ExpiryDate"] = reader.GetDateTime(reader.GetOrdinal("ExpiryDate"));
                                }
                                dt.Rows.Add(newRow);
                            }
                        }
                    }

                    dgvUsers.AutoGenerateColumns = false;
                    dgvUsers.Columns.Clear();
                    dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id" });
                    dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Usuario", DataPropertyName = "Username" });
                    dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { Name = "IsAdmin", HeaderText = "Es Admin", DataPropertyName = "IsAdmin" });
                    dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Active", HeaderText = "Activo", DataPropertyName = "Active" });
                    dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Expires", HeaderText = "Expira", DataPropertyName = "Expires" });
                    var expiryColumn = new DataGridViewTextBoxColumn { Name = "ExpiryDate", HeaderText = "Fecha de Expiración", DataPropertyName = "ExpiryDate" };
                    expiryColumn.DefaultCellStyle.Format = "dd/MM/yyyy";
                    expiryColumn.DefaultCellStyle.NullValue = string.Empty;
                    dgvUsers.Columns.Add(expiryColumn);

                    dgvUsers.DataSource = dt;
                    Logger.Log("Datos de usuarios cargados correctamente", $"Filas: {dt.Rows.Count}, Columnas: {string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
                }
                catch (Exception ex)
                {
                    Logger.Log("Error al cargar datos de usuarios", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                    MessageBox.Show($"Error al cargar datos de usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }
        private void tabPageReports_Click(object sender, EventArgs e)
        {

        }

        private void btnGenerateSalesReport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT s.Id, s.SaleDate, s.Total, s.Discount, s.PaymentMethod, s.Username, p.Name AS ProductName, si.Quantity, si.UnitPrice, si.Subtotal
                FROM Sales s
                JOIN SaleItems si ON s.Id = si.SaleId
                JOIN Products p ON si.ProductId = p.Id
                WHERE s.SaleDate BETWEEN @StartDate AND @EndDate";
                    if (cmbProductFilter.SelectedIndex > 0)
                    {
                        query += " AND p.Name = @ProductName";
                    }

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", dtpReportStartDate.Value.Date);
                        cmd.Parameters.AddWithValue("@EndDate", dtpReportEndDate
                          .Value.Date.AddDays(1));
                        if (cmbProductFilter.SelectedIndex > 0)
                        {
                            cmd.Parameters.AddWithValue("@ProductName", cmbProductFilter.SelectedItem.ToString());
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvReports.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnGenerateSalesReport_Click: {ex.Message}\n");
            }
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = dtpReportStartDate.Value;
                DateTime endDate = dtpReportEndDate.Value;

                // Validar que la fecha inicial no sea mayor que la fecha final
                if (startDate > endDate)
                {
                    MessageBox.Show("La fecha inicial no puede ser mayor que la fecha final.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Aquí iría la lógica para generar el reporte de ventas
                // Por ejemplo, consultar una base de datos con las ventas entre startDate y endDate
                // y llenar dgvSalesReports con los resultados
                DataTable reportData = new DataTable(); // Suponiendo que obtienes los datos de una base de datos
                                                        // Ejemplo: reportData = database.GetSalesReport(startDate, endDate);

                // Asignar los datos al DataGridView
                dgvSalesReports.DataSource = reportData;
                lblStatusMessage.Text = $"Reporte generado para el período {startDate.ToShortDateString()} - {endDate.ToShortDateString()}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatusMessage.Text = "Error al generar el reporte";
            }
        }
        private void tabPageSettings_Enter(object sender, EventArgs e)
        {
            // Lógica para cuando la pestaña de Configuración se activa
            RefreshSerialPorts(); // Cuando la pestaña de configuración se activa, refresca la lista de puertos.
        }

        private void btnRefreshPorts_Click(object sender, EventArgs e)
        {
            try
            {
                // Refrescar la lista de puertos
                RefreshSerialPorts();
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al refrescar puertos: {ex.Message}\n");
                MessageBox.Show($"Error al refrescar puertos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDisconnectBalanza_Click(object sender, EventArgs e)
        {
            try
            {
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        Logger.Log("Puerto serial cerrado", $"Puerto: {_serialPort.PortName}");
                    }
                    _serialPort.Dispose(); // Liberar recursos
                    _serialPort = null;    // Anular la referencia
                    Logger.Log("Puerto serial liberado", "Balanza desconectada");
                }

                lblConnectionStatus.Text = "Desconectado";
                lblConnectionStatus.ForeColor = Color.Red;
                btnConnectBalanza.Enabled = true;
                btnDisconnectBalanza.Enabled = false;
                txtWeightDisplay.Text = string.Empty;
                txtWeightDisplay.Refresh(); // Esto está bien en Windows Forms

                // Detener temporizador (si existe y se usa para el peso)
                if (weightUpdateTimer != null)
                {
                    weightUpdateTimer.Stop();
                    Logger.Log("Temporizador de peso detenido", "Balanza desconectada");
                }

                MessageBox.Show("Balanza desconectada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Log("Error al desconectar balanza", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al desconectar la balanza: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkDemoMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDemoMode.Checked)
            {
                // Iniciar simulador
                balanzaSimulator = new BalanzaSimulator(weight =>
                {
                    decimal roundedWeight = Math.Round((decimal)weight, ConfiguracionUsuario.WeightDecimals);
                    string formattedWeight = roundedWeight.ToString($"F{ConfiguracionUsuario.WeightDecimals}", CultureInfo.InvariantCulture);
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            txtWeightDisplay.Text = formattedWeight;
                            txtWeightDisplay.BackColor = Color.White;
                        }));
                    }
                    else
                    {
                        txtWeightDisplay.Text = formattedWeight;
                        txtWeightDisplay.BackColor = Color.White;
                    }
                });
                balanzaSimulator.Start();
                btnConnectBalanza.Enabled = false;
                btnDisconnectBalanza.Enabled = false;
                lblConnectionStatus.Text = "Modo Demo Activo";
                lblConnectionStatus.ForeColor = Color.Green;
            }
            else
            {
                // Detener simulador y restaurar estado
                balanzaSimulator?.Stop();
                balanzaSimulator = null;
                btnConnectBalanza.Enabled = true;
                btnDisconnectBalanza.Enabled = serialPort != null && serialPort.IsOpen;
                lblConnectionStatus.Text = serialPort != null && serialPort.IsOpen ? "Conectado" : "Desconectado";
                lblConnectionStatus.ForeColor = serialPort != null && serialPort.IsOpen ? Color.Green : Color.Red;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ClearSaleControls()
        {
            saleItemsDataTable.Rows.Clear();
            dgvSaleItems.DataSource = saleItemsDataTable;

            txtSearchProductCode.Clear();
            txtSaleProductName.Text = string.Empty;

            // ✅ CORRECCIÓN: Un Label no tiene .Clear(), usa .Text = string.Empty
            txtTotalSale.Text = string.Empty;

            txtStock.Clear();
            txtStock.ForeColor = System.Drawing.Color.Black;

            txtSearchProductCode.Enabled = true;
            txtSaleProductName.Enabled = true;
            btnAddSaleItem.Enabled = true;
            btnFinalizeSale.Enabled = true;
           // btnNewSale.Enabled = false;

            // Refresca la lista de productos principal después de la venta
            LoadProductData();
        }

        // En tu formulario MainScreen.cs

        private void LoadProductDetailsByCode(string productCode)
        {
            decimal stockRestante = 0;
            DataTable productData = SearchProductByCode(productCode);
            if (productData != null && productData.Rows.Count > 0)
            {
                DataRow productRow = productData.Rows[0];
                decimal stockInicial = DatabaseHelper.ObtenerStockActual(currentFoundProductId);
                decimal cantidadEnCarrito = 0;
                foreach (DataRow row in saleItemsDataTable.Rows)
                {
                    if (Convert.ToInt64(row["IdProducto"]) == currentFoundProductId)
                    {
                        cantidadEnCarrito += Convert.ToDecimal(row["Cantidad"]);
                    }
                }
                stockRestante = stockInicial - cantidadEnCarrito;
                string formato = "N" + ConfiguracionUsuario.WeightDecimals.ToString();
                txtRemainingStock.Text = stockRestante.ToString(formato);
            }
            else
            {
                ClearProductDetails();
            }
            txtRemainingStock.ForeColor = (stockRestante <= DatabaseHelper.ObtenerStockMinimo(currentFoundProductId)) ? Color.Red : Color.Black;
        }

        // **Nota:** No modifiques LoadUserData, ya que es para usuarios, no para productos.
        private void txtSearchProductCode_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearchProductCode.Text))
            {
                int code;
                if (int.TryParse(txtSearchProductCode.Text, out code))
                {
                    txtSearchProductCode.Text = code.ToString("D6");
                }
                else
                {
                    MessageBox.Show("Por favor, introduce un código numérico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSearchProductCode.Clear();
                    txtSearchProductCode.Focus();
                }
            }
        }

        private void txtSearchProductCode_Enter(object sender, EventArgs e)
        {
            // Selecciona todo el texto y luego mueve el cursor al final
            txtSearchProductCode.SelectionStart = txtSearchProductCode.Text.Length;
            txtSearchProductCode.SelectionLength = 0;
        }

       
        private void txtProductId_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Muestra la fecha y hora actual en el formato que prefieras
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {

        }

        private void chkActiveUser_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUserId_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUserId_Click(object sender, EventArgs e)
        {

        }

        private void txtPasswordUser_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void dgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Verifica si la columna no es "Username"
            if (dgvUsers.Columns[e.ColumnIndex].Name != "Username")
            {
                // Lógica para administradores
                if (Session.IsAdmin)
                {
                    // Oculta las celdas de las filas no seleccionadas
                    if (e.RowIndex != -1 && dgvUsers.SelectedRows.Count > 0 && e.RowIndex != dgvUsers.SelectedRows[0].Index)
                    {
                        e.Value = "********";
                        e.FormattingApplied = true;
                    }
                }
                // Lógica para usuarios normales (no administradores)
                else
                {
                    // Oculta todas las celdas que no sean "Username" en todas las filas
                    e.Value = "********";
                    e.FormattingApplied = true;
                }
            }
        }


        private void dgvUsers_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en dgvUsers: {e.Exception.Message}\n");
        }

        private void txtTotalSale_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv",
                    Title = "Exportar Reporte",
                    FileName = $"Reporte_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    DataTable reportData = (DataTable)dgvReports.DataSource;
                    if (reportData == null || reportData.Rows.Count == 0)
                    {
                        MessageBox.Show("No hay datos para exportar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (saveFileDialog.FilterIndex == 1)
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Reporte");
                            worksheet.Cell(1, 1).InsertTable(reportData);
                            workbook.SaveAs(saveFileDialog.FileName);
                        }
                    }
                    else
                    {
                        using (var writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            writer.WriteLine(string.Join(",", reportData.Columns.Cast<DataColumn>().Select(c => $"\"{c.ColumnName}\"")));
                            foreach (DataRow row in reportData.Rows)
                            {
                                writer.WriteLine(string.Join(",", row.ItemArray.Select(i => $"\"{i}\"")));
                            }
                        }
                    }

                    MessageBox.Show("Reporte exportado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar el reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnExportToExcel_Click: {ex.Message}\n");
            }
        }

 
        private void btnGenerateHourlyReport_Click(object sender, EventArgs e)
        {
            dgvHourlyReports.AutoGenerateColumns = false;
            dgvHourlyReports.Columns.Clear();

            // Configurar columnas
            var hourCol = new DataGridViewTextBoxColumn();
            hourCol.Name = "Hour";
            hourCol.HeaderText = "Hora";
            hourCol.DataPropertyName = "Hour";
            hourCol.DefaultCellStyle.Format = "HH:00";
            dgvHourlyReports.Columns.Add(hourCol);

            var totalAmountCol = new DataGridViewTextBoxColumn();
            totalAmountCol.Name = "TotalAmount";
            totalAmountCol.HeaderText = "Monto Total";
            totalAmountCol.DataPropertyName = "TotalAmount";
            totalAmountCol.DefaultCellStyle.Format = "N" + ConfiguracionUsuario.CurrencyDecimals.ToString();
            dgvHourlyReports.Columns.Add(totalAmountCol);

            var totalSalesCol = new DataGridViewTextBoxColumn();
            totalSalesCol.Name = "TotalSales";
            totalSalesCol.HeaderText = "Ventas Totales";
            totalSalesCol.DataPropertyName = "TotalSales";
            dgvHourlyReports.Columns.Add(totalSalesCol);

            var totalItemsCol = new DataGridViewTextBoxColumn();
            totalItemsCol.Name = "TotalItemsSold";
            totalItemsCol.HeaderText = "Total de Ítems Vendidos";
            totalItemsCol.DataPropertyName = "TotalItemsSold";
            totalItemsCol.DefaultCellStyle.Format = "N" + ConfiguracionUsuario.WeightDecimals.ToString();
            dgvHourlyReports.Columns.Add(totalItemsCol);

            // Validar fechas
            if (dtpReportStartDate.Value.Date > dtpReportEndDate.Value.Date)
            {
                MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblStatusMessage.Text = "";
                return;
            }

            // Validar rango de fechas (máximo 31 días)
            if ((dtpReportEndDate.Value.Date - dtpReportStartDate.Value.Date).TotalDays > 31)
            {
                MessageBox.Show("El rango de fechas no puede exceder 31 días.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblStatusMessage.Text = "";
                return;
            }

            DateTime startDate = dtpReportStartDate.Value.Date;
            DateTime endDate = dtpReportEndDate.Value.Date.AddDays(1).AddSeconds(-1);

            lblStatusMessage.Text = "Generando reporte por horas, por favor espere...";

            using (SQLiteConnection conn = DatabaseHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT
                    strftime('%H:00', S.SaleDate) AS Hour,
                    SUM(SD.LineTotal) AS TotalAmount,
                    COUNT(DISTINCT S.SaleId) AS TotalSales,
                    SUM(SD.Quantity) AS TotalItemsSold
                FROM Sales S
                INNER JOIN SaleDetails SD ON S.SaleId = SD.SaleId
                WHERE S.SaleDate BETWEEN @StartDate AND @EndDate
                GROUP BY strftime('%H', S.SaleDate)
                ORDER BY Hour";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("No hay ventas en el rango de fechas seleccionado.", "Reporte Vacío", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvHourlyReports.DataSource = null;
                                lblStatusMessage.Text = "Reporte vacío.";
                            }
                            else
                            {
                                dgvHourlyReports.DataSource = dt;
                                lblStatusMessage.Text = $"Reporte por hora generado con {dt.Rows.Count} registros.";
                                POSLogger.Log("Reporte por hora generado", $"Registros: {dt.Rows.Count}, Rango: {startDate:yyyy-MM-dd} a {endDate:yyyy-MM-dd}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al generar el reporte por horas: {ex.Message}", "Error de Reporte", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatusMessage.Text = "Error al generar el reporte.";
                    POSLogger.Log("Error al generar reporte por hora", ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            // Restablecer las fechas a un valor por defecto, por ejemplo, hoy
            dtpReportStartDate.Value = DateTime.Today;
            dtpReportEndDate.Value = DateTime.Today;

            // Limpiar el DataGridView
            dgvSalesReports.DataSource = null;

            // Limpiar el mensaje de estado
            lblStatusMessage.Text = "";
        }

        private void btnDeleteAllSalesData_Click(object sender, EventArgs e)
        {
            if (!Session.IsAdmin)
            {
                MessageBox.Show("No tienes permisos para realizar esta acción.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DialogResult result = MessageBox.Show(
                "¿Estás seguro de que quieres borrar TODOS los reportes? Esta acción es irreversible y se registrará en un log.",
                "Confirmar Borrado",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        using (var transaction = conn.BeginTransaction())
                        {
                            string deleteDetailsQuery = "DELETE FROM SaleDetails;";
                            using (SQLiteCommand cmd = new SQLiteCommand(deleteDetailsQuery, conn, transaction))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            string deleteSalesQuery = "DELETE FROM Sales;";
                            using (SQLiteCommand cmd = new SQLiteCommand(deleteSalesQuery, conn, transaction))
                            {
                                int rowsAffected = cmd.ExecuteNonQuery();
                                MessageBox.Show($"Se han eliminado {rowsAffected} registros de ventas.", "Borrado Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            string logQuery = "INSERT INTO AdminLogs (UserId, Action, Timestamp) VALUES (@UserId, 'Borrado de Reportes', @Timestamp)";
                            using (SQLiteCommand logCmd = new SQLiteCommand(logQuery, conn, transaction))
                            {
                                logCmd.Parameters.AddWithValue("@UserId", currentLoggedInId);
                                logCmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                                logCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            dgvSalesReports.DataSource = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al borrar los reportes: {ex.Message}\n");
                        MessageBox.Show($"Ocurrió un error al borrar los reportes: {ex.Message}", "Error de Borrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void HandleNumericInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo procesar si la tecla es un dígito o Backspace y el control es un MaskedTextBox
            if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                if (sender is MaskedTextBox maskedTextBox)
                {
                    // Obtener el texto del control, sin la máscara
                    string currentText = maskedTextBox.Text.Replace(" ", "").Replace("_", "");

                    if (e.KeyChar == (char)Keys.Back)
                    {
                        // Si es backspace, elimina el último carácter
                        if (currentText.Length > 0)
                        {
                            currentText = currentText.Substring(0, currentText.Length - 1);
                        }
                    }
                    else
                    {
                        // Si es un dígito, agrégalo al final
                        currentText += e.KeyChar.ToString();
                    }

                    // Rellenar con ceros a la izquierda para mantener el formato
                    string formattedText = currentText.PadLeft(maskedTextBox.Mask.Length, '0');

                    // Asegurar que la longitud no exceda la de la máscara
                    if (formattedText.Length > maskedTextBox.Mask.Length)
                    {
                        formattedText = formattedText.Substring(1);
                    }

                    // Actualizar el texto del control y mover el cursor al final
                    maskedTextBox.Text = formattedText;
                    maskedTextBox.SelectionStart = maskedTextBox.Text.Length;
                    e.Handled = true;
                }
            }
        }

        
        private void HandleDecimalInput_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtProductId_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowDGVColumnNames();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtWeightDisplay_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAccept_Click_1(object sender, EventArgs e)
        {

        }

        private void txtProductPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtProductId_Click(object sender, EventArgs e)
        {
            // Selecciona todo el texto al entrar en el control
            txtProductId.SelectAll();
        }

        private void txtProductPrice1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void AddProductToSale(object sender, EventArgs e)
        {

        }



        private void numericUpDownDecimalesPrecio_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int newCurrencyDecimals = (int)numericUpDownDecimalesPrecio.Value;
                ConfiguracionUsuario.SaveSettings(
                    weightDecimals: ConfiguracionUsuario.WeightDecimals,
                    currencyDecimals: newCurrencyDecimals,
                    currencySymbol: ConfiguracionUsuario.CurrencySymbol,
                    baudRate: ConfiguracionUsuario.BaudRate,
                    serialPort: ConfiguracionUsuario.SerialPort,
                    parity: ConfiguracionUsuario.Parity,
                    dataBits: ConfiguracionUsuario.DataBits,
                    stopBits: ConfiguracionUsuario.StopBits
                );

                if (txtProductPrice != null && decimal.TryParse(txtProductPrice.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                {
                    txtProductPrice.Text = price.ToString("N" + ConfiguracionUsuario.CurrencyDecimals, System.Globalization.CultureInfo.InvariantCulture);
                }
                if (txtTotalSale != null && decimal.TryParse(txtTotalSale.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal total))
                {
                    txtTotalSale.Text = total.ToString("N" + ConfiguracionUsuario.CurrencyDecimals, System.Globalization.CultureInfo.InvariantCulture);
                }

                if (dgvSaleItems.Columns["PricePerUnit"] != null)
                {
                    dgvSaleItems.Columns["PricePerUnit"].DefaultCellStyle.Format = "N" + ConfiguracionUsuario.CurrencyDecimals;
                }
                if (dgvSaleItems.Columns["TotalPrice"] != null)
                {
                    dgvSaleItems.Columns["TotalPrice"].DefaultCellStyle.Format = "N" + ConfiguracionUsuario.CurrencyDecimals;
                }

                if (dgvProducts.Columns["PricePerUnit"] != null)
                {
                    dgvProducts.Columns["PricePerUnit"].DefaultCellStyle.Format = "N" + ConfiguracionUsuario.CurrencyDecimals;
                }

                UpdateProductDecimals();
                Logger.Log("Decimales de precio actualizados", $"Nuevo valor: {ConfiguracionUsuario.CurrencyDecimals}");
            }
            catch (Exception ex)
            {
                Logger.Log("Error en numericUpDownDecimalesPrecio_ValueChanged", ex.Message);
                MessageBox.Show($"Error al actualizar decimales de precio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtRemainingStock_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearchProductCode_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void numericUpDownTimeout_ValueChanged(object sender, EventArgs e)
        {
            // Ejemplo: Configurar el timeout para la comunicación con la balanza
            // Puedes usar este valor en btnConnectBalanza_Click
        }
        private void FormConfiguracion_Load(object sender, EventArgs e)
        {
            // Carga los valores desde la base de datos o archivo de configuración
            ConfiguracionUsuario.LoadSettings();

            // Asigna el valor cargado al control NumericUpDown
            numericUpDownDecimalesBalanza.Value = ConfiguracionUsuario.CurrencyDecimals;
        }

        

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            {
                // ⭐ Si este botón abre un panel, carga los datos aquí.
                LoadUsers();
                // panelAdmin.Visible = true;
            }
        }

        private void btnGenerateStockReport_Click(object sender, EventArgs e)
        {
            if (dtpReportStartDate == null || dtpReportEndDate == null || cmbProductFilter == null)
            {
                MessageBox.Show("Los controles de fecha o filtro no están inicializados.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnGenerateStockReport_Click: Controles no inicializados\n");
                return;
            }

            dgvStockReports.AutoGenerateColumns = false;
            dgvStockReports.Columns.Clear();

            dgvStockReports.Columns.Add("Code", "Código de Producto");
            dgvStockReports.Columns["Code"].DataPropertyName = "Code";

            dgvStockReports.Columns.Add("Name", "Nombre de Producto");
            dgvStockReports.Columns["Name"].DataPropertyName = "Name";

            dgvStockReports.Columns.Add("Unit", "Unidad");
            dgvStockReports.Columns["Unit"].DataPropertyName = "Unit";

            var stockActualCol = new DataGridViewTextBoxColumn
            {
                Name = "Stock",
                HeaderText = "Stock Actual",
                DataPropertyName = "Stock",
                DefaultCellStyle = { Format = $"F{ConfiguracionUsuario.WeightDecimals}" }
            };
            dgvStockReports.Columns.Add(stockActualCol);

            var stockMinimoCol = new DataGridViewTextBoxColumn
            {
                Name = "MinimumStock",
                HeaderText = "Stock Mínimo",
                DataPropertyName = "MinimumStock",
                DefaultCellStyle = { Format = $"F{ConfiguracionUsuario.WeightDecimals}" }
            };
            dgvStockReports.Columns.Add(stockMinimoCol);

            var stockIngresadoCol = new DataGridViewTextBoxColumn
            {
                Name = "StockIngresado",
                HeaderText = "Stock Ingresado",
                DataPropertyName = "StockIngresado",
                DefaultCellStyle = { Format = $"F{ConfiguracionUsuario.WeightDecimals}" }
            };
            dgvStockReports.Columns.Add(stockIngresadoCol);

            dgvStockReports.Columns.Add("Username", "Ingresado por");
            dgvStockReports.Columns["Username"].DataPropertyName = "Username";

            var timestampCol = new DataGridViewTextBoxColumn
            {
                Name = "Timestamp",
                HeaderText = "Fecha y Hora",
                DataPropertyName = "Timestamp",
                DefaultCellStyle = { Format = "G" }
            };
            dgvStockReports.Columns.Add(timestampCol);

            var alertCol = new DataGridViewTextBoxColumn
            {
                Name = "Alert",
                HeaderText = "Estado",
                DataPropertyName = "Alert"
            };
            dgvStockReports.Columns.Add(alertCol);

            using (SQLiteConnection conn = DatabaseHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT
                            P.Code,
                            P.Name,
                            P.Unit,
                            P.Stock,
                            P.MinimumStock,
                            SUM(CASE WHEN SM.MovementType = 'Ingreso' THEN SM.Quantity ELSE 0 END) AS StockIngresado,
                            SM.Username,
                            MAX(SM.MovementDate) AS Timestamp,
                            CASE WHEN P.Stock < P.MinimumStock THEN 'Bajo' ELSE 'Normal' END AS Alert
                        FROM Products P
                        LEFT JOIN StockMovements SM ON P.Id = SM.ProductId AND SM.MovementType = 'Ingreso'
                        WHERE P.Active = 1
                        AND (SM.MovementDate IS NULL OR SM.MovementDate BETWEEN @StartDate AND @EndDate)";
                    if (cmbProductFilter.SelectedIndex > 0)
                    {
                        query += " AND P.Name = @ProductName";
                    }
                    query += " GROUP BY P.Id ORDER BY SM.MovementDate DESC";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", dtpReportStartDate.Value.Date);
                        cmd.Parameters.AddWithValue("@EndDate", dtpReportEndDate.Value.Date.AddDays(1));
                        if (cmbProductFilter.SelectedIndex > 0)
                        {
                            cmd.Parameters.AddWithValue("@ProductName", cmbProductFilter.SelectedItem.ToString());
                        }

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("No hay datos de stock disponibles.", "Reporte Vacío", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                dgvStockReports.DataSource = null;
                            }
                            else
                            {
                                dgvStockReports.DataSource = dt;

                                foreach (DataGridViewRow row in dgvStockReports.Rows)
                                {
                                    if (row.Cells["Alert"].Value?.ToString() == "Bajo")
                                    {
                                        row.DefaultCellStyle.BackColor = System.Drawing.Color.Yellow;
                                    }
                                }

                                Logger.Log("Reporte de stock generado", $"Registros: {dt.Rows.Count}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al generar el reporte de stock: {ex.Message}", "Error de Reporte", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.Log("Error al generar reporte de stock", ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPageProducts) // Ajusta "tabProducts" al nombre real
            {
                LoadProducts();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void SaveSettingsToDatabase()
        {
            using (SQLiteConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // ⭐ Corregido: La sentencia UPDATE ahora actualiza la columna 'Value'
                // donde la columna 'Key' es 'CurrencyDecimals'.
                string query = "UPDATE Settings SET Value = @Value WHERE Key = 'CurrencyDecimals'";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    // ⭐ Ahora el parámetro es @Value
                    cmd.Parameters.AddWithValue("@Value", ConfiguracionUsuario.CurrencyDecimals);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static class POSLogger
        {
            public static void Log(string action, string details)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] {action}: {details}\n");
            }
        }

        private void btnConnectBalanza_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedPort = cmbPorts.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedPort))
                {
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnConnectBalanza_Click: Puerto no seleccionado\n");
                    MessageBox.Show("Seleccione un puerto serial.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Si el puerto seleccionado es diferente al guardado, actualiza la configuración.
                if (selectedPort != ConfiguracionUsuario.SerialPort)
                {
                    ConfiguracionUsuario.SaveSettings(
                        ConfiguracionUsuario.WeightDecimals,
                        ConfiguracionUsuario.CurrencyDecimals,
                        ConfiguracionUsuario.CurrencySymbol,
                        ConfiguracionUsuario.BaudRate, // Asume que BaudRate, Parity, DataBits, StopBits vienen de tu UI o están fijos
                        selectedPort,
                        ConfiguracionUsuario.Parity,
                        ConfiguracionUsuario.DataBits,
                        ConfiguracionUsuario.StopBits
                    );
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnConnectBalanza_Click: Configuración de puerto actualizada a {selectedPort}\n");
                }

                // Llamar a SetupSerialPort para configurar y abrir el puerto.
                SetupSerialPort();

                // Inicializar o iniciar el temporizador.
                
                // Actualizar el estado de la UI.
                lblConnectionStatus.Text = "Conectado";
                lblConnectionStatus.ForeColor = Color.Green;
                btnConnectBalanza.Enabled = false;
                btnDisconnectBalanza.Enabled = true;

                // Asegurarse de que el ComboBox muestre el puerto conectado
                if (cmbPorts.Items.Count > 0 && cmbPorts.SelectedItem?.ToString() != ConfiguracionUsuario.SerialPort)
                {
                    cmbPorts.SelectedItem = ConfiguracionUsuario.SerialPort;
                }

                File.AppendAllText("debug.log", $"[{DateTime.Now}] btnConnectBalanza_Click: Balanza conectada exitosamente\n");
                MessageBox.Show($"Balanza conectada al puerto {ConfiguracionUsuario.SerialPort} con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnConnectBalanza_Click: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                MessageBox.Show($"Error al conectar la balanza: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblConnectionStatus.Text = "Desconectado";
                lblConnectionStatus.ForeColor = Color.Red;
                btnConnectBalanza.Enabled = true;
                btnDisconnectBalanza.Enabled = false;

                // Asegurarse de detener el temporizador en caso de fallo
                if (weightUpdateTimer != null && weightUpdateTimer.Enabled)
                {
                    weightUpdateTimer.Stop();
                }
            }
        }

        private void UpdateSaleTable()
        {
            try
            {
                dgvSaleItems.DataSource = null;
                dgvSaleItems.DataSource = saleItemsDataTable;
                decimal total = saleItemsDataTable.AsEnumerable()
                    .Sum(row => row.Field<decimal>("Subtotal"));
                txtTotalSale.Text = total.ToString($"F{ConfiguracionUsuario.CurrencyDecimals}");
                if (saleItemsDataTable.Rows.Count > 0)
                {
                    var lastRow = saleItemsDataTable.Rows[saleItemsDataTable.Rows.Count - 1];
                    txt1Quantity.Text = lastRow["Cantidad"].ToString();
                    txtSaleProductName.Text = lastRow["Nombre"].ToString();
                    txtSaleProductPrice.Text = lastRow.Field<decimal>("PrecioUnitario").ToString($"F{ConfiguracionUsuario.CurrencyDecimals}");
                    txtSearchProductCode.Text = lastRow["Código"].ToString();
                    lblSaleProductUnit.Text = lastRow["Unidad"].ToString();
                    txtRemainingStock.Text = DatabaseHelper.GetProductStockFromCode(lastRow["Código"].ToString()).ToString("F2");
                }
                else
                {
                    txt1Quantity.Text = "0";
                    txtSaleProductName.Text = "";
                    txtSaleProductPrice.Text = "";
                    txtSearchProductCode.Text = "";
                    lblSaleProductUnit.Text = "";
                    txtRemainingStock.Text = "";
                }
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Total actualizado: {total}, Cantidad primer ítem: {txt1Quantity.Text}\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al actualizar tabla de ventas: {ex.Message}\n");
            }
        }
        private bool ValidateSerialPortParameters(out string serialPort, out int baudRate, out Parity parity, out int dataBits, out StopBits stopBits)
        {
            serialPort = null;
            baudRate = 0;
            parity = Parity.None;
            dataBits = 0;
            stopBits = StopBits.One;

            try
            {
                string selectedPort;
                if (cmbPorts.InvokeRequired)
                {
                    selectedPort = (string)cmbPorts.Invoke(new Func<string>(() => cmbPorts.SelectedItem?.ToString()));
                }
                else
                {
                    selectedPort = cmbPorts.SelectedItem?.ToString();
                }

                if (string.IsNullOrEmpty(selectedPort))
                {
                    Logger.Log("ValidateSerialPortParameters", "Puerto serial no seleccionado [2025-09-22 00:40:00 -05]");
                    MessageBox.Show("Seleccione un puerto serial.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT SerialPort, BaudRate, Parity, DataBits, StopBits FROM Settings WHERE Id = 1", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                serialPort = reader.GetString(0);
                                baudRate = reader.GetInt32(1);
                                string parityStr = reader.GetString(2);
                                dataBits = reader.GetInt32(3);
                                string stopBitsStr = reader.GetString(4);

                                try
                                {
                                    parity = (Parity)Enum.Parse(typeof(Parity), parityStr, true);
                                    stopBits = (StopBits)Enum.Parse(typeof(StopBits), stopBitsStr, true);
                                }
                                catch (ArgumentException ex)
                                {
                                    Logger.Log("ValidateSerialPortParameters", $"Valor inválido para Parity o StopBits: Parity={parityStr}, StopBits={stopBitsStr}, Error={ex.Message} [2025-09-22 00:40:00 -05]");
                                    MessageBox.Show($"Configuración inválida: Parity={parityStr}, StopBits={stopBitsStr}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                            }
                            else
                            {
                                Logger.Log("ValidateSerialPortParameters", "Configuración de balanza no encontrada en Settings [2025-09-22 00:40:00 -05]");
                                MessageBox.Show("Configuración de balanza no encontrada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Error en ValidateSerialPortParameters", $"{ex.Message}\nStackTrace: {ex.StackTrace} [2025-09-22 00:40:00 -05]");
                MessageBox.Show($"Error al validar parámetros del puerto serial: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ProcessReturnAndRestock(int productId, decimal quantity)
        {
            using (SQLiteConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string updateStockQuery = "UPDATE Products SET Stock = Stock + @Quantity WHERE Id = @ProductId";
                        using (SQLiteCommand cmd = new SQLiteCommand(updateStockQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@ProductId", productId);
                            cmd.ExecuteNonQuery();
                        }

                        string movementQuery = "INSERT INTO StockHistory (ProductId, Quantity, Reason, Username, Date) VALUES (@ProductId, @Quantity, @Reason, @Username, @Date)";
                        using (SQLiteCommand cmdMovement = new SQLiteCommand(movementQuery, conn, transaction))
                        {
                            cmdMovement.Parameters.AddWithValue("@ProductId", productId);
                            cmdMovement.Parameters.AddWithValue("@Quantity", quantity);
                            cmdMovement.Parameters.AddWithValue("@Reason", "Devolución");
                            cmdMovement.Parameters.AddWithValue("@Username", Session.Username ?? "default_user");
                            cmdMovement.Parameters.AddWithValue("@Date", DateTime.Now);
                            cmdMovement.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        File.AppendAllText("debug.log", $"[{DateTime.Now}] Devolución procesada: ProductId={productId}, Quantity={quantity}\n");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al procesar devolución: {ex.Message}\n");
                        throw;
                    }
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
               
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                    serialPort = null;
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] btnLogout_Click: Puerto serial cerrado\n");
                }

                Session.Logout();
                LoginScreen loginScreen = new LoginScreen();
                loginScreen.Show();
                this.Close();
                File.AppendAllText("debug.log", $"[{DateTime.Now}] btnLogout_Click: Sesión cerrada, LoginScreen mostrado\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en btnLogout_Click: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
                MessageBox.Show($"Error al cerrar sesión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboParity_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbProductFilter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
       

        private void btnExportStockReport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv",
                    Title = "Exportar Reporte de Stock",
                    FileName = $"Reporte_Stock_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    DataTable reportData = (DataTable)dgvStockReports.DataSource;
                    if (reportData == null || reportData.Rows.Count == 0)
                    {
                        MessageBox.Show("No hay datos para exportar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (saveFileDialog.FilterIndex == 1)
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Reporte de Stock");
                            worksheet.Cell(1, 1).InsertTable(reportData);
                            workbook.SaveAs(saveFileDialog.FileName);
                        }
                    }
                    else
                    {
                        using (var writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            writer.WriteLine(string.Join(",", reportData.Columns.Cast<DataColumn>().Select(c => $"\"{c.ColumnName}\"")));
                            foreach (DataRow row in reportData.Rows)
                            {
                                writer.WriteLine(string.Join(",", row.ItemArray.Select(i => $"\"{i}\"")));
                            }
                        }
                    }

                    MessageBox.Show("Reporte de stock exportado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    File.AppendAllText("debug.log", $"[{DateTime.Now}] Reporte de stock exportado: {saveFileDialog.FileName}\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error al exportar reporte de stock: {ex.Message}\n");
                MessageBox.Show($"Error al exportar el reporte de stock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txt1Quantity_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnClearProductFields_Click(object sender, EventArgs e)
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            txtProductPrice.Text = "";
            cboProductUnit.SelectedIndex = -1;
            txtStock.Text = "";
            txtMinimumStock.Text = "";
            chkProductActive.Checked = true;
            MessageBox.Show("Campos limpiados.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void numericUpDownDecimalesBalanza_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int newWeightDecimals = (int)numericUpDownDecimalesBalanza.Value;
                ConfiguracionUsuario.SaveSettings(
                    weightDecimals: newWeightDecimals,
                    currencyDecimals: ConfiguracionUsuario.CurrencyDecimals,
                    currencySymbol: ConfiguracionUsuario.CurrencySymbol,
                    baudRate: ConfiguracionUsuario.BaudRate,
                    serialPort: ConfiguracionUsuario.SerialPort,
                    parity: ConfiguracionUsuario.Parity,
                    dataBits: ConfiguracionUsuario.DataBits,
                    stopBits: ConfiguracionUsuario.StopBits
                );

                if (txtWeightDisplay != null && decimal.TryParse(txtWeightDisplay.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal weight))
                {
                    txtWeightDisplay.Text = weight.ToString("N" + ConfiguracionUsuario.WeightDecimals, System.Globalization.CultureInfo.InvariantCulture);
                }

                if (dgvSaleItems.Columns["Weight"] != null)
                {
                    dgvSaleItems.Columns["Weight"].DefaultCellStyle.Format = "N" + ConfiguracionUsuario.WeightDecimals;
                }
                if (dgvSaleItems.Columns["Stock"] != null)
                {
                    dgvSaleItems.Columns["Stock"].DefaultCellStyle.Format = "N" + ConfiguracionUsuario.WeightDecimals;
                }

                if (dgvProducts.Columns["Stock"] != null)
                {
                    dgvProducts.Columns["Stock"].DefaultCellStyle.Format = "N" + ConfiguracionUsuario.WeightDecimals;
                }
                if (dgvProducts.Columns["MinimumStock"] != null)
                {
                    dgvProducts.Columns["MinimumStock"].DefaultCellStyle.Format = "N" + ConfiguracionUsuario.WeightDecimals;
                }

                UpdateProductDecimals();
                Logger.Log("Decimales de peso actualizados", $"Nuevo valor: {ConfiguracionUsuario.WeightDecimals}");
            }
            catch (Exception ex)
            {
                Logger.Log("Error en numericUpDownDecimalesBalanza_ValueChanged", ex.Message);
                MessageBox.Show($"Error al actualizar decimales de peso: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductDetails(string productCode)
        {
            try
            {
                using (var connection = new SQLiteConnection("Data Source=BalanzaPOS.db;Version=3;"))
                {
                    connection.Open();
                    string query = "SELECT Code, Name, Price, Unit FROM Products WHERE Code = @Code";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Code", productCode);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtProductId.Text = reader["Code"].ToString();
                                txtProductName.Text = reader["Name"].ToString();
                                txtProductPrice.Text = reader["Price"].ToString();
                                cboProductUnit.Text = reader["Unit"].ToString();
                                // Otros campos según sea necesario
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"[{DateTime.Now}] Error en LoadProductDetails: {ex.Message}\n");
                MessageBox.Show($"Error al cargar detalles del producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnProductQuick1_Click(object sender, EventArgs e)
        {
           
        }

        private void chkProductActive_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabPageSales_Click(object sender, EventArgs e)
        {

        }

        private void txtSearchProductCode_MaskInputRejected_1(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void txtWeightDisplay_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
    #endregion
}
