using BalanzaPOSNuevo;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using BalanzaPOSNuevo.Models; // Ajusta si tu namespace raíz es diferente o si moviste los modelos
using BalanzaPOSNuevo.Services; // Ajusta si tu namespace raíz es diferente o si moviste los servicios
using BalanzaPOSNuevo.Helpers; // Ajusta si tu namespace raíz es diferente o si moviste los helpers
using Microsoft.VisualBasic;



namespace BalanzaPOSNuevo
{
    public partial class MainScreen : Form
    {
        // -----------------------------------------------------
        // Sección de Variables a Nivel de Clase
        // -----------------------------------------------------

        #region Campos de Clase
        private Timer demoWeightTimer;
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
        private ProductService _productService;
        private SaleService _saleService;
        private QuickProductService _quickProductService;
        private ScaleService _scaleService;
        private Product _selectedProduct;
        private bool balanzaReceivingData = false;
        private System.Windows.Forms.Timer balanzaHeartbeatTimer; // Temporizador para detectar inactividad      // Bandera para indicar si hubo recepción de datos
        private const int HEARTBEAT_INTERVAL_MS = 3000;         // 3 segundos
        private bool _isQuickButtonClick = false; // ⭐ DECLARA ESTA VARIABLE AQUÍ
        private bool _isProcessingScaleData = false;
        private bool _isProcessingUIUpdate = false; // Bandera unificada

        #endregion

        // Asegúrate de que esta clase esté definida en tu proyecto

        public MainScreen(long userId, bool isAdmin)
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            File.AppendAllText("debug.log", $"[{DateTime.Now}] MainScreen_Constructor: Iniciando, UserId: {userId}, IsAdmin: {isAdmin}\n");
            Session.UserId = (int)userId;
            Session.IsAdmin = isAdmin;
            InitializeServices(); 
            DatabaseHelper.InitializeDatabase();
            ConfiguracionUsuario.LoadSettings();

            saleItemsDataTable = new DataTable();
            InitializeDataGridViews();
            dgvSaleItems.DataSource = saleItemsDataTable;
             // ⭐ Aquí se inicializa
                                                                                                  // ...
            btnConnectBalanza.Enabled = true;
            btnDisconnectBalanza.Enabled = false;
            UpdateBalanzaStatusUI("Desconectada", Color.Gray);
            btnNewSale.Enabled = true;
            btnFinalizeSale.Enabled = true;
            this.FormClosing += MainScreen_FormClosing;
            PopulateSerialPorts();
            SetupAdminButtons();
            InitializeReportControls();
            //LoadUserData();
            LoadProductsToDataGridView();
            dgvProducts.CellContentClick += dgvProducts_CellContentClick;
            UpdateSaleTable();
            File.AppendAllText("debug.log", $"[{DateTime.Now}] MainScreen_Constructor: Completado\n");
        }
        private void SetupUserInterface()
        {
            // Si el usuario es administrador, habilita los botones de administración.
            if (Session.IsAdmin)
            {
                btnTestQuickProduct.Visible = true;
                // Oculta otros elementos que no necesites
                // ...
            }
            else
            {
                // Si no es administrador, asegura que los botones estén ocultos o deshabilitados.
                btnTestQuickProduct.Visible = false;
                btnDevolucion.Visible = false;
            }
        }

        #region Métodos de Inicialización
        

        private void SetupAdminButtons()
        {
            btnTestQuickProduct.Visible = Session.IsAdmin;
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
            this.txtProductCode.Font = standardFont;
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
            this.txtProductCode.Size = textBoxSize;
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
           // this.btnSearchProduct.Font = standardFont;
            this.btnAddSaleItem.Font = standardFont;

            this.txtSearchProductCode.Size = textBoxSize;
            this.txtSaleProductPrice.Size = textBoxSize;
            this.txt1Quantity.Size = textBoxSize;
            this.txtRemainingStock.Size = textBoxSize;
          //  this.btnSearchProduct.Size = new Size(200, 40);
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
                    _saleService.FinalizeSale(discount, paymentMethod, Session.Username, 1);
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

            _serialPort.DataReceived += serialPort_DataReceived;
        }


        private void BalanzaHeartbeatTimer_Tick(object sender, EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen) // ⭐ Comprobar si el puerto está realmente abierto
            {
                if (!balanzaReceivingData)
                {
                    // Si no hemos recibido datos en el intervalo, asumimos desconexión
                    UpdateBalanzaStatusUI("Desconectada (Inactiva)", Color.Red);
                    Logger.Log("Balanza", "Balanza inactiva detectada. Actualizando estado a desconectada.");
                }
                // Si sí recibimos datos, el DataReceived ya actualiza el estado a verde.
                balanzaReceivingData = false; // Resetear para la próxima comprobación
            }
            else
            {
                // Si el puerto no está abierto, el temporizador no debería estar corriendo o debe mostrar desconectado
                UpdateBalanzaStatusUI("Desconectada", Color.Red);
                balanzaHeartbeatTimer.Stop(); // Detener el temporizador si el puerto no está abierto
                Logger.Log("Balanza", "Temporizador de heartbeat detenido: Puerto serial no está abierto.");
            }
        }
        // En el constructor de MainScreen (o en un método de inicialización):
        private void InitializeBalanzaHeartbeat()
        {
            balanzaHeartbeatTimer = new System.Windows.Forms.Timer();
            balanzaHeartbeatTimer.Interval = HEARTBEAT_INTERVAL_MS;
            balanzaHeartbeatTimer.Tick += BalanzaHeartbeatTimer_Tick;
            //balanzaHeartbeatTimer.Start();
        }

        private void UpdateBalanzaStatusUI(string status, Color color)
        {
            if (lblStatusBalanza.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lblStatusBalanza.Text = status;
                    lblStatusBalanza.BackColor = color; // ⭐ Cambia a BackColor
                    lblStatusBalanza.ForeColor = Color.White; // O el color de texto que prefieras para contraste
                });
            }
            else
            {
                lblStatusBalanza.Text = status;
                lblStatusBalanza.BackColor = color; // ⭐ Cambia a BackColor
                lblStatusBalanza.ForeColor = Color.White;
            }
        }

        // ARCHIVO: MainScreen.cs

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
            {
                UpdateBalanzaStatusUI("Desconectada", Color.Red);
                return;
            }

            try
            {
                string receivedData = _serialPort.ReadLine();
                // Agrega un log para ver los datos EXACTOS que recibes de la balanza
                Logger.Log("Balanza Data", $"Datos recibidos: '{receivedData}'");

                // ⭐ PASO 1: Procesar los datos de la balanza
                // Si la balanza envía PUNTO como decimal (lo más común):
                // Elimina caracteres no numéricos, y reemplaza la coma por punto si sabes que la balanza envía coma.
                // O limpia la cadena antes de parsear para solo dejar números y un punto.
                string cleanedWeightString = receivedData.Trim(); // Por ejemplo, "  1.234kg\r\n"
                                                                  // Aquí debes adaptar esta lógica a la estructura EXACTA de tu balanza.
                                                                  // Por ejemplo, si siempre termina en "kg\r\n" y el peso está al inicio:
                cleanedWeightString = cleanedWeightString.Replace("kg", "").Replace("\r", "").Replace("\n", "").Trim();

                // ⭐ MUY IMPORTANTE: SI TU BALANZA USA COMA COMO DECIMAL Y TU SISTEMA ESPERA PUNTO, CONVIERTE:
                cleanedWeightString = cleanedWeightString.Replace(',', '.'); // Si la balanza usa coma, pero decimal.Parse espera punto.

                // ⭐ PASO 2: Parsear el peso a decimal
                decimal weight;
                // Usa InvariantCulture para PARSEAR si la balanza envía un PUNTO decimal.
                // Usa CultureInfo.CurrentCulture si la balanza envía COMA decimal (y tu sistema usa coma).
                // Lo más seguro con dispositivos es PARSEAR con InvariantCulture si usas PUNTO,
                // o usar una culture específica si sabes que la balanza usa COMA.
                if (!decimal.TryParse(cleanedWeightString, NumberStyles.Any, CultureInfo.InvariantCulture, out weight))
                {
                    Logger.Log("Error Balanza", $"No se pudo parsear el peso '{cleanedWeightString}'. Datos crudos: '{receivedData}'");
                    // No se pudo obtener el peso, salir o mostrar un error.
                    return;
                }
                int decimalesPeso = ConfiguracionUsuario.WeightDecimals; // Asumiendo que esta propiedad existe
                weight = Math.Round(weight, decimalesPeso); // ⭐ Redondea el valor al número de decimales configurado

                string weightFormat = "N" + decimalesPeso; // Usa la variable local si la obtuviste

                this.Invoke((MethodInvoker)delegate
                {
                    _isProcessingUIUpdate = true;
                    Logger.Log("AutoVenta DEBUG", "Balanza: Actualizando txtWeightDisplay.Text.");
                    
                    balanzaReceivingData = true; // Indicar que se recibieron datos

                    // ⭐ CORRECCIÓN: Usar CurrentCulture para MOSTRAR en la UI
                    txtWeightDisplay.Text = weight.ToString(weightFormat, CultureInfo.CurrentCulture);
                   
                    Logger.Log("AutoVenta DEBUG", "Balanza: Fin de actualización de UI."); // AÑADIR este log
                    _isProcessingUIUpdate = false;
                });
            }
            catch (Exception ex)
            {
                Logger.Log("Error Balanza", $"Error en serialPort_DataReceived: {ex.Message}. StackTrace: {ex.StackTrace}");
                this.Invoke((MethodInvoker)delegate
                {
                    UpdateBalanzaStatusUI("Error de Lectura", Color.OrangeRed);
                });
            }
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
                    scalePortName: ConfiguracionUsuario.ScalePortName, // <-- Corregido
                    scaleBaudRate: ConfiguracionUsuario.ScaleBaudRate, // <-- Corregido
                    scaleParity: ConfiguracionUsuario.ScaleParity, // <-- Corregido (aunque este ya estaba bien con el prefijo)
                    scaleDataBits: ConfiguracionUsuario.ScaleDataBits, // <-- Corregido
                    scaleStopBits: ConfiguracionUsuario.ScaleStopBits // <-- Corregido
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
                _serialPort.DataReceived += serialPort_DataReceived;
                _serialPort.Open();
                Logger.Log("Puerto serial reconfigurado", $"Puerto: {newPortName}, BaudRate: {newBaudRate}, Parity: {parity}, DataBits: {dataBits}, StopBits: {stopBits}");

                // Actualizar UI
                lblStatusBalanza.Text = "Conectado";
                lblStatusBalanza.ForeColor = Color.Green;
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
                lblStatusBalanza.Text = "Desconectado";
                lblStatusBalanza.ForeColor = Color.Red;
                btnConnectBalanza.Enabled = true;
                btnDisconnectBalanza.Enabled = false;
            }
        }

        private void LoadScaleSettingsAndConnect()
        {
            // Cargar la configuración actual de la balanza
            ConfiguracionUsuario.LoadSettings();

            // Solo intentar conectar si hay un puerto configurado
            if (!string.IsNullOrEmpty(ConfiguracionUsuario.ScalePortName))
            {
                try
                {
                    // ⭐ Esta es la clave: Llama a SetupSerialPort() aquí.
                    // SetupSerialPort ya se encarga de _serialPort = new SerialPort(...), 
                    // _serialPort.Open(), y de asignar el DataReceived event.
                    SetupSerialPort();

                    // Si la conexión fue exitosa, iniciamos el timer heartbeat
                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        if (balanzaHeartbeatTimer != null && !balanzaHeartbeatTimer.Enabled)
                        {
                            balanzaHeartbeatTimer.Start();
                            Logger.Log("Balanza", "Temporizador de heartbeat iniciado automáticamente.");
                        }
                        UpdateBalanzaStatusUI("Conectada", Color.Green); // Establece el color correcto si tiene éxito
                        btnConnectBalanza.Enabled = false;
                        btnDisconnectBalanza.Enabled = true;
                    }
                    else
                    {
                        // Si SetupSerialPort no lanzó excepción pero serialPort no está abierto (ej. puerto ocupado)
                        UpdateBalanzaStatusUI("Desconectada", Color.Red);
                        btnConnectBalanza.Enabled = true;
                        btnDisconnectBalanza.Enabled = false;
                        Logger.Log("Balanza", $"Error: Puerto '{ConfiguracionUsuario.ScalePortName}' no pudo abrirse aunque SetupSerialPort no lanzó excepción.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Error de Balanza", $"Fallo la conexión automática al inicio en {ConfiguracionUsuario.ScalePortName}: {ex.Message}");
                    UpdateBalanzaStatusUI("Desconectada", Color.Red); // El color rojo es correcto aquí
                    btnConnectBalanza.Enabled = true;
                    btnDisconnectBalanza.Enabled = false;
                    // Asegurarse de detener el temporizador si ya estaba corriendo y falló la reconexión
                    if (balanzaHeartbeatTimer != null && balanzaHeartbeatTimer.Enabled)
                    {
                        balanzaHeartbeatTimer.Stop();
                    }
                }
            }
            else
            {
                // Si no hay puerto configurado, mostrar como desconectado y habilitar conectar
                UpdateBalanzaStatusUI("Desconectada (Sin configurar)", Color.Gray);
                btnConnectBalanza.Enabled = true;
                btnDisconnectBalanza.Enabled = false;
            }
        }
        private void InitializeConfiguration()
        {
            try
            {
                ConfiguracionUsuario.SaveSettings(
            weightDecimals: ConfiguracionUsuario.WeightDecimals,
            currencyDecimals: ConfiguracionUsuario.CurrencyDecimals,
            currencySymbol: ConfiguracionUsuario.CurrencySymbol,
            scalePortName: ConfiguracionUsuario.ScalePortName, // <-- Corregido
            scaleBaudRate: ConfiguracionUsuario.ScaleBaudRate, // <-- Corregido
            scaleParity: ConfiguracionUsuario.ScaleParity, // <-- Corregido (aunque este ya estaba bien con el prefijo)
            scaleDataBits: ConfiguracionUsuario.ScaleDataBits, // <-- Corregido
            scaleStopBits: ConfiguracionUsuario.ScaleStopBits // <-- Corregido
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
            _scaleService.Dispose(); // Libera los recursos del puerto serial
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
            int baudRate = ConfiguracionUsuario.ScaleBaudRate;
            string serialPort = ConfiguracionUsuario.ScalePortName;
            Parity parity = ConfiguracionUsuario.ScaleParity;
            int dataBits = ConfiguracionUsuario.ScaleDataBits;
            StopBits stopBits = ConfiguracionUsuario.ScaleStopBits;

            // Asigna los valores a tus controles de la interfaz de usuario, si los tienes
            // Ejemplo:
            // numericUpDownBaudRate.Value = baudRate;
            // comboPorts.Text = serialPort;
        }

        // EN BalanzaPOSNuevo\BalanzaPOSNuevo\MainScreen.cs

        private void ClearSaleInterface()
        {
            try
            {
                // ⭐ ELIMINAR: saleItemsDataTable.Clear();


                txtSearchProductCode.Text = "";
                txtSaleProductName.Text = "";
                lblSaleProductUnit.Text = "";
                txtSaleProductPrice.Text = "";
                txtRemainingStock.Text = "";
                txt1Quantity.Text = "1";
                txtDiscount.Text = "0.00";
                txtTotalSale.Text = "0.00";

                // NO TOCAR dgvSaleItems.DataSource. Solo actualizar el control.
                dgvSaleItems.Refresh();

                _selectedProduct = null; // Asegúrate de limpiar el producto seleccionado

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
            // Limpieza inicial de botones
            for (int i = 1; i <= 10; i++)
            {
                Button btn = panel2.Controls.Find($"btnProductQuick{i}", true).FirstOrDefault() as Button;
                if (btn != null)
                {
                    btn.Click -= btnProductQuick_Click;
                    btn.Text = "Vacío";
                    btn.Tag = null;
                    btn.Enabled = false; // Se deshabilitan inicialmente
                }
            }

            try
            {
                Logger.Log("Productos rápidos asignados", "Iniciando carga de botones rápidos desde el servicio.");

                // ⭐ ESTA ES LA CLAVE: ¿QUÉ DEVUELVE ESTE MÉTODO?
                Dictionary<int, Product> assignedProducts = _quickProductService.GetAssignedQuickProducts();

                // ⭐ AÑADIR ESTE LOG PARA DEPURAR
                if (assignedProducts == null || assignedProducts.Count == 0)
                {
                    Logger.Log("Productos rápidos asignados", "El servicio GetAssignedQuickProducts() devolvió un diccionario vacío o nulo.");
                }
                else
                {
                    Logger.Log("Productos rápidos asignados", $"El servicio GetAssignedQuickProducts() devolvió {assignedProducts.Count} productos asignados.");
                    foreach (var entry in assignedProducts)
                    {
                        Logger.Log("Productos rápidos asignados", $"  - Botón: {entry.Key}, Producto: {entry.Value?.Name ?? "NULO"}");
                    }
                }

                foreach (var entry in assignedProducts)
                {
                    int buttonIndex = entry.Key;
                    Product product = entry.Value; // product podría ser null si el servicio lo devuelve así.

                    var button = panel2.Controls.Find($"btnProductQuick{buttonIndex}", true).FirstOrDefault() as Button;
                    if (button != null)
                    {
                        if (product != null) // ⭐ Asegurarse de que el producto no sea nulo antes de usarlo
                        {
                            button.Text = $"{product.Name}\n({product.PricePerUnit.ToString("C", CultureInfo.CurrentCulture)})"; // Mostrar nombre y precio
                            button.Tag = product; // Guardar el objeto Product completo
                            button.Click += btnProductQuick_Click;
                            button.Enabled = true; // ⭐ Habilitar el botón si tiene un producto
                            Logger.Log("Productos rápidos asignados", $"Botón btnProductQuick{buttonIndex} asignado a {product.Name}");
                        }
                        else
                        {
                            button.Text = $"[{buttonIndex}] Producto no encontrado"; // Mostrar que el producto es nulo
                            button.Tag = null;
                            button.Click -= btnProductQuick_Click; // Quitar el evento si no hay producto
                            button.Enabled = true; // Podrías dejarlo habilitado para asignar, o deshabilitado si no hay nada
                            Logger.Log("Productos rápidos asignados", $"Botón btnProductQuick{buttonIndex} no tiene producto asignado (NULL).");
                        }
                    }
                    else
                    {
                        Logger.Log("Advertencia", $"Botón btnProductQuick{buttonIndex} no encontrado en panel2 para asignar producto.");
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

        private void ClearQuickProductButtons()
        {
            foreach (Control control in panel2.Controls)
            {
                if (control is Button button)
                {
                    if (int.TryParse(button.Name.Replace("btnQuickProduct", ""), out int buttonNumber))
                    {
                        button.Text = $"Rápido {buttonNumber}";
                        button.Tag = buttonNumber; // Asigna el número del botón como Tag inicial
                        button.Enabled = false; // Deshabilita por defecto
                    }
                }
            }
        }
        private void btnQuickProduct_Click(object sender, EventArgs e)
        {
            try
            {
                _isQuickButtonClick = true; // ⭐ 1. Activar la bandera al inicio del clic del botón rápido

                var button = sender as Button;
                if (button == null) return;
                if (button.Tag == null) return;

                var quickProduct = button.Tag as Product;
                if (quickProduct == null) return;

                _selectedProduct = quickProduct; // Asigna el producto seleccionado
                currentFoundProductId = quickProduct.Id;
                currentFoundProductCode = quickProduct.Code;
                currentFoundProductName = quickProduct.Name;
                currentFoundProductPrice = quickProduct.PricePerUnit;

                // ... tu lógica para llenar los TextBoxes de la UI de venta ...
                string weightFormat = "N" + ConfiguracionUsuario.WeightDecimals;
                string priceFormat = "N" + ConfiguracionUsuario.CurrencyDecimals;

                txtSearchProductCode.Text = quickProduct.Code?.PadLeft(6, '0') ?? string.Empty;
                txtSaleProductName.Text = quickProduct.Name;
                lblSaleProductUnit.Text = quickProduct.Unit;
                txtSaleProductPrice.Text = quickProduct.PricePerUnit.ToString(priceFormat, CultureInfo.InvariantCulture);
                txtRemainingStock.Text = quickProduct.Stock.ToString(weightFormat, CultureInfo.InvariantCulture);

                // Lógica de Cantidad (Peso vs. Unidad)
                if (decimal.TryParse(txtWeightDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight) && weight > 0)
                {
                    txt1Quantity.Text = weight.ToString(weightFormat, CultureInfo.InvariantCulture);
                }
                else
                {
                    txt1Quantity.Text = 1.ToString(weightFormat, CultureInfo.InvariantCulture);
                }

                // NO LLAMES a btnAddSaleItem_Click(null, null); aquí si quieres que sea un paso manual.
                // Esa línea debe eliminarse por completo.

                Logger.Log("Info", $"Producto rápido cargado: Código={quickProduct.Code}, Nombre={quickProduct.Name}");
            }
            catch (Exception ex)
            {
                Logger.Log("Error en btnQuickProduct_Click", $"Error: {ex.Message}");
            }
            finally
            {
                // ⭐ 2. Desactivar la bandera DESPUÉS de que toda la lógica del clic del botón ha terminado.
                // El Task.Delay (como sugerí antes) puede ser útil si observas que el evento dgvProducts_SelectionChanged
                // se dispara casi instantáneamente y la bandera se desactiva antes de que dgvProducts_SelectionChanged lo compruebe.
                // Pero intentemos sin él primero. Si sigue fallando, lo ponemos.
                _isQuickButtonClick = false;
            }
        }
        private void ProductButton_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag is Product product)
                {
                    _selectedProduct = product;
                    currentFoundProductId = product.Id;
                    currentFoundProductCode = product.Code;
                    currentFoundProductName = product.Name;
                    currentFoundProductPrice = product.PricePerUnit;

                    string weightFormat = "N" + ConfiguracionUsuario.WeightDecimals;
                    string priceFormat = "N" + ConfiguracionUsuario.CurrencyDecimals;
                    txtProductCode.Text = product.Id.ToString();
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
                    Logger.Log("Verificación de controles", $"txtProductId.Text={txtProductCode.Text}, txtSearchProductCode.Text={txtSearchProductCode.Text}, txtSaleProductName.Text={txtSaleProductName.Text}, txtSaleProductPrice.Text={txtSaleProductPrice.Text}, cboProductUnit.Text={cboProductUnit.Text}, txtRemainingStock.Text={txtRemainingStock.Text}, txt1Quantity.Text={txt1Quantity.Text}, cboWeightUnit.Text={cboWeightUnit.Text}, txtProductId.Visible={txtProductCode.Visible}, txtSaleProductName.Visible={txtSaleProductName.Visible}, txtSaleProductPrice.Visible={txtSaleProductPrice.Visible}, txt1Quantity.Visible={txt1Quantity.Visible}");
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

        // Constructor actualizado para inyección de dependencias
        public MainScreen()
        {
            InitializeComponent();
            InitializeServices();
            InitializeUIComponents(); // Asegúrate de que esto se llama
            _saleService = new SaleService(); // Instancia tu servicio aquí o globalmente si ya lo hiciste
            dgvSaleItems.DataSource = _saleService.CurrentSaleItems; // Enlaza el DGV al DataTable del servicio

            // ⭐ SUSCRIPCIONES A EVENTOS
            _saleService.SaleUpdated += HandleSaleUpdated;
            _saleService.SaleFinalized += HandleSaleFinalized;
           
            // Puedes cargar productos al inicio si es necesario
            LoadProductsToDataGridView();

            // Inicializar el ComboBox de métodos de pago
            InitializePaymentMethods();
        }

        // -----------------------------------------------------------------------------------
        // Métodos de Eventos (Manejadores de Eventos del Servicio)
        // -----------------------------------------------------------------------------------

        private void HandleSaleUpdated(object sender, EventArgs e)
        {
            // ... (Tu lógica existente para actualizar dgvSaleItems y lblTotalSale) ...

            // Habilitar el botón de Totalizar/Guardar Venta solo si hay ítems en la venta
            btnNewSale.Enabled = _saleService.CurrentSaleItems.Rows.Count > 0;

            txtTotalSale.Text = _saleService.TotalSale.ToString("C2");

            // ... (otras actualizaciones de UI para primer ítem, etc.) ...
        }

        private void HandleSaleFinalized(object sender, EventArgs e)
        {
            // El servicio ya ha guardado y finalizado la venta.

            // 1. Indicar al servicio que comience la nueva venta.
            // Esto llama a CurrentSaleItems.Clear() y luego a OnSaleUpdated().
            _saleService.NewSale();

            // 2. Limpiar la interfaz de usuario de texto.
            ClearSaleInterface();
            dgvSaleItems.Refresh(); // Forzar el redibujado
            btnNewSale.Enabled = false;
        }

        private void InitializeServices()
        {
            _productService = new ProductService();
            _saleService = new SaleService();
            _quickProductService = new QuickProductService(_productService); // Pasa ProductService
            _scaleService = new ScaleService();

            // Suscribirse a los eventos de los servicios
            _saleService.SaleUpdated += (s, e) => UpdateSaleUI(); // Actualiza la UI de venta
            _scaleService.WeightReceived += (s, weight) => UpdateWeightDisplay(weight); // Actualiza el display de peso
            _scaleService.ConnectionStatusChanged += (s, status) => UpdateScaleStatusUI(status);
            _scaleService.ErrorOccurred += (s, error) => ShowScaleError(error);
        }

        private void InitializeUIComponents()
        {
            // Cualquier inicialización de la UI que no requiera datos cargados
            ConfigureSaleDataGridViewColumns(); // Configura las columnas del DGV de venta
            // ... otras inicializaciones visuales ...
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
            if (!string.IsNullOrEmpty(ConfiguracionUsuario.ScalePortName) && cmbPorts.Items.Contains(ConfiguracionUsuario.ScalePortName))
            {
                cmbPorts.SelectedItem = ConfiguracionUsuario.ScalePortName;
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
            LoadProductsToDataGridView();
            AssignQuickProductsToButtons();
            LoadScaleSettingsAndConnect();
            UpdateSaleUI(); // Muestra el total de la venta (0.00 al inicio)
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
            toolTip.SetToolTip(this.txtProductName, "Ingrese el nombre del producto"); // Parece ser txtSaleProductName
            toolTip.SetToolTip(this.txtProductCode, "Ingrese el código único del producto"); // Parece ser txtSearchProductCode
            toolTip.SetToolTip(this.txtDiscount, "Ingrese el descuento para la venta");
            toolTip.SetToolTip(this.cboPaymentMethod, "Seleccione el método de pago");

            this.txtSearchProductCode.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    // ⭐ NO LLAMAR A btnSearchProduct_Click.
                    // En su lugar, puedes forzar la ejecución de la lógica de TextChanged si quieres
                    // una búsqueda inmediata al presionar Enter, o llamar a btnAddSaleItem_Click
                    // si el producto ya está en la UI.

                    // Opción 1: Si txtSearchProductCode_TextChanged ya hace la búsqueda:
                    // txtSearchProductCode_TextChanged(s, EventArgs.Empty); 

                    // Opción 2: Si el producto ya está cargado y Enter es para añadirlo:
                    if (_selectedProduct != null)
                    {
                        btnAddSaleItem_Click(btnAddSaleItem, EventArgs.Empty);
                    }
                    else
                    {
                        // Si no hay producto seleccionado, haz la búsqueda
                        // Puedes llamar a un método auxiliar de búsqueda, o simplemente dejar que
                        // txtSearchProductCode_TextChanged se encargue, ya que ya lo hace.
                        // Para este ejemplo, haremos una llamada directa para asegurar la búsqueda
                        PerformProductSearchAndLoadUI(txtSearchProductCode.Text.Trim());
                    }

                    e.Handled = true; // Prevenir el "ding"
                   
                }
            };
            this.txtSearchProductCode.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;         // Indica que ya manejaste la pulsación de la tecla
                    e.SuppressKeyPress = true; // ⭐ SUPRIME LA TECLA COMPLETA (evita el "ding" y otros efectos)
                }
            };
        }
         


        private void PerformProductSearchAndLoadUI(string code)
        {
            try
            {
                Product product = _saleService.GetProductByCode(code);
                LoadProductToSaleUI(product);
                if (product == null)
                {
                    MessageBox.Show($"Producto con código {code} no encontrado.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error", $"Error en búsqueda por Enter: {ex.Message}");
                MessageBox.Show($"Error al buscar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // EN BalanzaPOSNuevo\BalanzaPOSNuevo\MainScreen.cs (donde estaba la llamada)

        private void txtSearchProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true; // Evita el "ding" de Windows

                string productCode = txtSearchProductCode.Text.Trim();
                if (string.IsNullOrWhiteSpace(productCode))
                {
                    ClearSaleItemFields(); // Limpiar si el campo está vacío
                    return;
                }

                // ⭐ Usar el servicio ProductService (NO SaleService, ya que GetProductByCode es una operación de Producto)
                // Necesitas una instancia de ProductService en MainScreen.cs
                // MainScreen: private ProductService _productService;
                // MainScreen Constructor: _productService = new ProductService();
                Product foundProduct = _productService.GetProductByCode(productCode.PadLeft(6, '0'));

                LoadProductToSaleUI(foundProduct); // ⭐ Llama a tu método centralizado de carga de UI de venta

                // Opcional: limpiar el campo de búsqueda después de añadir
                // txtSearchProductCode.Clear(); 
            }
        }

        private void txtSaleProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true; // Evita el "ding" de Windows

                string productName = txtSaleProductName.Text.Trim();
                if (string.IsNullOrWhiteSpace(productName))
                {
                    ClearSaleItemFields(); // Limpiar si el campo está vacío
                    return;
                }

                // ⭐ Nuevo método en ProductService para buscar por nombre (o puedes usar GetAllProducts y filtrar)
                Product foundProduct = _productService.GetProductByName(productName);

                LoadProductToSaleUI(foundProduct);

                // Opcional: limpiar el campo de búsqueda después de añadir
                // txtSaleProductName.Clear();
            }
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

        // Este método es llamado por el evento WeightReceived del ScaleService
        private void UpdateWeightDisplay(decimal weight)
        {
            // Asegúrate de que esto se ejecuta en el hilo de la UI si el evento viene de un hilo secundario
            if (txtWeightDisplay.InvokeRequired)
            {
                txtWeightDisplay.Invoke(new Action<decimal>(UpdateWeightDisplay), weight);
            }
            else
            {
                txtWeightDisplay.Text = weight.ToString("F3", CultureInfo.InvariantCulture); // "F3" para 3 decimales
                txtWeightDisplay.Refresh(); // Fuerza el refresco
            }
        }

        private void UpdateScaleStatusUI(string status)
        {
            if (lblStatusBalanza.InvokeRequired) // Asumiendo que tienes un Label para el estado
            {
                lblStatusBalanza.Invoke(new Action<string>(UpdateScaleStatusUI), status);
            }
            else
            {
                lblStatusBalanza.Text = status;
                Logger.Log("Balanza", $"Estado de la balanza: {status}");
            }
        }

        private void ShowScaleError(string error)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ShowScaleError), error);
            }
            else
            {
                MessageBox.Show(error, "Error de Balanza", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para cargar y conectar la balanza (ejecutar en MainScreen_Load)
      
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
                            cmbPorts.SelectedItem = ConfiguracionUsuario.ScalePortName;
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
                        cmbPorts.SelectedItem = ConfiguracionUsuario.ScalePortName;
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
            // 1. CERRAR Y LIMPIAR CUALQUIER INSTANCIA ANTERIOR
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null; // Establecer a null para asegurar que se crea una nueva instancia
                Logger.Log("Balanza", "Puerto serial anterior cerrado y liberado.");
            }
            else if (_serialPort != null) // Si no estaba abierto pero existía una instancia
            {
                _serialPort.Dispose();
                _serialPort = null;
                Logger.Log("Balanza", "Instancia de puerto serial anterior liberada.");
            }


            try
            {
                // 2. RECUPERAR CONFIGURACIÓN ACTUAL
                string portName = ConfiguracionUsuario.ScalePortName;
                int baudRate = ConfiguracionUsuario.ScaleBaudRate;
                Parity parity = ConfiguracionUsuario.ScaleParity;
                int dataBits = ConfiguracionUsuario.ScaleDataBits;
                StopBits stopBits = ConfiguracionUsuario.ScaleStopBits;

                if (string.IsNullOrEmpty(portName))
                {
                    throw new InvalidOperationException("Nombre de puerto serial no configurado.");
                }

                // 3. CREAR NUEVA INSTANCIA DE SerialPort
                _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

                // 4. ASIGNAR EL MANEJADOR DE EVENTOS
                // ⭐ CRÍTICO: Aquí es donde se conecta el evento.
                _serialPort.DataReceived += serialPort_DataReceived;
                _serialPort.ErrorReceived += SerialPort_ErrorReceived; // También maneja errores de puerto

                // 5. ABRIR EL PUERTO
                _serialPort.Open();

                Logger.Log("Balanza", $"Conectado a la balanza en {portName}");
                UpdateBalanzaStatusUI("Conectada", Color.Green);

            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Log("Error de Balanza", $"Acceso denegado al puerto {ConfiguracionUsuario.ScalePortName}: {ex.Message}");
                MessageBox.Show($"Acceso denegado al puerto {ConfiguracionUsuario.ScalePortName}. Asegúrese de que no esté siendo usado por otra aplicación.", "Error de Balanza", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _serialPort = null; // Asegurar que es null si hubo fallo
                UpdateBalanzaStatusUI("Desconectada", Color.Red);
                throw; // Relanza la excepción para que btnConnectBalanza_Click la capture
            }
            catch (Exception ex)
            {
                Logger.Log("Error de Balanza", $"Error al configurar/abrir puerto serial: {ex.Message}");
                _serialPort = null; // Asegurar que es null si hubo fallo
                UpdateBalanzaStatusUI("Desconectada", Color.Red);
                throw; // Relanza la excepción
            }
        }

        // ⭐ Nuevo método para manejar errores del puerto serial
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Logger.Log("Error de Balanza (Evento ErrorReceived)", $"Error: {e.EventType}");
            // Puedes agregar lógica aquí para desconectar o intentar reconectar.
            UpdateBalanzaStatusUI("Error", Color.OrangeRed);
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
                        AllowUserToAddRows = false,
                        AllowUserToDeleteRows = false
                    };
                    dgvQuickProducts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ButtonIndex", HeaderText = "Índice Botón", DataPropertyName = "ButtonIndex", ReadOnly = true });

                    DataGridViewComboBoxColumn productColumn = new DataGridViewComboBoxColumn
                    {
                        Name = "ProductId",
                        HeaderText = "Producto",
                        DataPropertyName = "ProductId",
                        DisplayMember = "Name",
                        ValueMember = "Id",
                        DataSource = GetProductsForComboBoxForConfig() // Nuevo método para el combo box
                    };
                    dgvQuickProducts.Columns.Add(productColumn);

                    DataTable dt = new DataTable();
                    dt.Columns.Add("ButtonIndex", typeof(int));
                    dt.Columns.Add("ProductId", typeof(object)); // <-- ¡CORREGIDO: Tipo object para permitir DBNull.Value!

                    for (int i = 1; i <= 10; i++)
                    {
                        dt.Rows.Add(i, DBNull.Value);
                    }

                    // Cargar la configuración actual desde el servicio
                    Dictionary<int, Product> currentQuickProducts = _quickProductService.GetAssignedQuickProducts();
                    foreach (var entry in currentQuickProducts)
                    {
                        int buttonIndex = entry.Key;
                        Product product = entry.Value;
                        DataRow existingRow = dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ButtonIndex") == buttonIndex);
                        if (existingRow != null)
                        {
                            existingRow["ProductId"] = product.Id; // Asigna el ID del producto
                        }
                    }
                    dgvQuickProducts.DataSource = dt;

                    FlowLayoutPanel bottomPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
                    Button btnSave = new Button { Text = "Guardar", AutoSize = true, Margin = new Padding(5) };
                    btnSave.Click += (s, e) =>
                    {
                        try
                        {
                            _quickProductService.SaveQuickProductConfiguration(dt); // Usar el servicio
                            AssignQuickProductsToButtons(); // Recarga los botones en el formulario principal
                            configForm.Close();
                            MessageBox.Show("Configuración guardada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("Error", $"Error al guardar configuración de productos rápidos: {ex.Message}");
                            MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };
                    bottomPanel.Controls.Add(btnSave);

                    Button btnCancel = new Button { Text = "Cancelar", AutoSize = true, Margin = new Padding(5) };
                    btnCancel.Click += (s, e) => { configForm.Close(); };
                    bottomPanel.Controls.Add(btnCancel);

                    configForm.Controls.Add(dgvQuickProducts);
                    configForm.Controls.Add(bottomPanel);
                    configForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error al configurar productos rápidos", $"{ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al configurar productos rápidos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetProductsForComboBoxForConfig()
        {
            DataTable productsDt = new DataTable();
            productsDt.Columns.Add("Id", typeof(object)); // Para aceptar DBNull.Value
            productsDt.Columns.Add("Name", typeof(string));

            productsDt.Rows.Add(DBNull.Value, "(Ninguno)"); // Opción para desasignar

            List<Product> allActiveProducts = _productService.GetAllProducts(false); // Solo productos activos
            foreach (var product in allActiveProducts)
            {
                productsDt.Rows.Add(product.Id, product.Name);
            }
            return productsDt;
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

        


        // Este método maneja el clic de los botones btnProductQuick1 a btnProductQuick10

        
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



        // --- Necesitas asegurarte de que este método exista y se use ---

        private void LoadProductToSaleUI(Product product)
        {
            string weightFormat = "N" + ConfiguracionUsuario.WeightDecimals;
            string priceFormat = "N" + ConfiguracionUsuario.CurrencyDecimals;

            if (product != null)
            {
                _selectedProduct = product;
                currentFoundProductId = product.Id; // Asumo que estas variables son globales en MainScreen
                currentFoundProductCode = product.Code;
                currentFoundProductName = product.Name;
                currentFoundProductPrice = product.PricePerUnit;

                txtProductCode.Text = product.Id.ToString(); // Esto suele ser el ID del producto, no el código
                txtSearchProductCode.Text = product.Code; // El campo de búsqueda muestra el código del producto encontrado
                txtSaleProductName.Text = product.Name;
                txtSaleProductPrice.Text = product.PricePerUnit.ToString(priceFormat, CultureInfo.InvariantCulture);
                txtStock.Text = product.Stock.ToString(weightFormat, CultureInfo.InvariantCulture);
                txtMinimumStock.Text = product.MinimumStock.ToString(weightFormat, CultureInfo.InvariantCulture);
                cboProductUnit.Text = product.Unit;
                txtRemainingStock.Text = product.Stock.ToString(weightFormat, CultureInfo.InvariantCulture);
                txtRemainingStock.ForeColor = product.Stock <= product.MinimumStock ? Color.Red : Color.Black;
                cboWeightUnit.Text = product.Unit;
                cboWeightUnit.Enabled = false;

                if (decimal.TryParse(txtWeightDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight) && weight > 0)
                {
                    txt1Quantity.Text = weight.ToString(weightFormat, CultureInfo.InvariantCulture);
                }
                else
                {
                    txt1Quantity.Text = 1.ToString(weightFormat, CultureInfo.InvariantCulture);
                }

                Logger.Log("Producto encontrado y cargado en UI de venta", $"Code={product.Code}, Name={product.Name}, Quantity={txt1Quantity.Text}");
            }
            else
            {
                // ⭐ Si el producto es nulo, también debes limpiar _selectedProduct
                _selectedProduct = null;
                Logger.Log("Búsqueda de producto", "Producto no encontrado o nulo.");
                ClearSaleItemFields(); // ⭐ Asegúrate de que este método limpia todos los campos de venta.
                MessageBox.Show("Producto no encontrado.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ARCHIVO: MainScreen.cs

        private void ClearProductDisplayFields()
        {
            txtSaleProductName.Text = "";
            lblSaleProductUnit.Text = "";
            txtSaleProductPrice.Text = "";
            txtRemainingStock.Text = "";
            txt1Quantity.Text = "1"; // O a "0.00" según tu lógica inicial
                                     // ... otros campos de la sección de venta que quieres limpiar CUANDO SE DESSELECCIONA EN LA TABLA DE PRODUCTOS...
        }
        private void LoadProductsToDataGridView()
        {
            try
            {
                List<Product> products = _productService.GetAllProducts(true); // Incluir inactivos si es para administración

                // Convertir List<Product> a DataTable si tu dgvProducts usa DataSource = DataTable
                DataTable dt = new DataTable();
                if (products.Any())
                {
                    dt = ConvertToDataTable(products); // Necesitarás un helper para esto
                }
                else
                {
                    // Si no hay productos, crea la estructura del DataTable vacío
                    dt.Columns.Add("Id", typeof(int));
                    dt.Columns.Add("Code", typeof(string));
                    dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("PricePerUnit", typeof(decimal));
                    dt.Columns.Add("Unit", typeof(string));
                    dt.Columns.Add("Stock", typeof(decimal));
                    dt.Columns.Add("MinimumStock", typeof(decimal));
                    dt.Columns.Add("Active", typeof(bool));
                }

                dgvProducts.DataSource = dt;
                ConfigureProductDataGridViewColumns(); // Configurar columnas después de asignar DataSource
                Logger.Log("UI", "Productos cargados en dgvProducts.");
            }
            catch (Exception ex)
            {
                Logger.Log("Error", $"Error al cargar productos en dgvProducts: {ex.Message}");
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ARCHIVO: MainScreen.cs

        private void ConfigureProductDataGridViewColumns()
        {
            // Limpiar columnas existentes para evitar duplicados si se llama varias veces
            dgvProducts.Columns.Clear();

            // Añadir las columnas necesarias.
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Id", HeaderText = "ID", DataPropertyName = "Id", ReadOnly = true, Visible = false });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Code", HeaderText = "Código", DataPropertyName = "Code", ReadOnly = true });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Name", HeaderText = "Nombre", DataPropertyName = "Name", ReadOnly = true });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn() { Name = "PricePerUnit", HeaderText = "Precio Unitario", DataPropertyName = "PricePerUnit", ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Unit", HeaderText = "Unidad", DataPropertyName = "Unit", ReadOnly = true });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Stock", HeaderText = "Stock", DataPropertyName = "Stock", ReadOnly = true });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn() { Name = "MinimumStock", HeaderText = "Stock Mínimo", DataPropertyName = "MinimumStock", ReadOnly = true });
            dgvProducts.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "Active", HeaderText = "Activo", DataPropertyName = "Active", ReadOnly = true });

            // ⭐ CRÍTICO: Añade la columna de botón aquí
            dgvProducts.Columns.Add(new DataGridViewButtonColumn()
            {
                Name = "colAsignarRapido",      // Nombre para referenciarla en el evento CellContentClick
                HeaderText = "Asignar Rápido",  // Texto del encabezado
                Text = "Asignar",               // Texto que aparecerá en cada botón
                UseColumnTextForButtonValue = true, // Para que el texto anterior sea el del botón
                FlatStyle = FlatStyle.Popup     // Estilo del botón
            });

            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        // Helper method para convertir List<Product> a DataTable
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        private void ConfigureSaleDataGridViewColumns()
        {
            dgvSaleItems.Columns.Clear();
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Code", HeaderText = "Código", DataPropertyName = "Code", ReadOnly = true });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Name", HeaderText = "Producto", DataPropertyName = "Name", ReadOnly = true });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Quantity", HeaderText = "Cantidad", DataPropertyName = "Quantity", ReadOnly = true });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Unit", HeaderText = "Unidad", DataPropertyName = "Unit", ReadOnly = true });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "PricePerUnit", HeaderText = "Precio Unit.", DataPropertyName = "PricePerUnit", ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Subtotal", HeaderText = "Subtotal", DataPropertyName = "Subtotal", ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvSaleItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "StockRemaining", HeaderText = "Stock Rest.", DataPropertyName = "StockRemaining", ReadOnly = true }); // Columna nueva
            dgvSaleItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchName = txtSaleProductName.Text.Trim();
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
                                txtProductCode.Text = currentFoundProductId.ToString();
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

        // ARCHIVO: MainScreen.cs
        private bool _isUpdatingUI = false;
        private void btnAddSaleItem_Click(object sender, EventArgs e)
        {
            // ⭐ BLINDAJE CRÍTICO: Si el método es llamado programáticamente (por ejemplo, desde un TextChanged)
            // mientras estamos procesando una actualización de UI (como limpiar txt1Quantity.Text),
            // la ejecución se detiene aquí para prevenir el bucle.
            if (_isProcessingUIUpdate)
            {
                Logger.Log("Bucle DEBUG", "btnAddSaleItem_Click: Cancelando re-ejecución debido a _isProcessingUIUpdate.");
                return;
            }

            // 1. VALIDACIÓN: Producto seleccionado
            if (_selectedProduct == null)
            {
                MessageBox.Show("Primero busque o seleccione un producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. VALIDACIÓN: Cantidad
            string quantityText = txt1Quantity.Text.Trim();

            if (!decimal.TryParse(quantityText, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show($"Cantidad inválida o cero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 3. LÓGICA DE VENTA: Primera y ÚNICA adición del producto a la venta
                _saleService.AddProductToSale(_selectedProduct, quantity);
                UpdateSaleTable();

                // 4. LOGGING: Usar un solo log para la acción manual
                Logger.Log("Info", $"Ítem añadido/actualizado (Manual): Nombre={_selectedProduct.Name}, Cantidad={quantity}");
                // ⭐ Eliminé el log duplicado que estaba en la línea 1974 de tu código original.

                // 5. MANIPULACIÓN DE UI CON BLINDAJE
                _isProcessingUIUpdate = true;

                if (_selectedProduct.Unit == "kg")
                {
                    // Para productos por peso, generalmente se limpia o se pone a cero el campo
                    // después de agregarlo a la venta, esperando un nuevo peso de la balanza.
                    txt1Quantity.Text = "0";
                }
                else // Para productos por unidad (Unit, ud, etc.)
                {
                    // Se resetea la cantidad a 1 para la siguiente unidad
                    txt1Quantity.Text = 1.ToString("N0", CultureInfo.CurrentCulture);
                }

                _isProcessingUIUpdate = false;
            }
            catch (Exception ex)
            {
                _isProcessingUIUpdate = false; // Asegurar que la bandera se resetee incluso en error
                Logger.Log("Error", $"Error al añadir ítem: {ex.Message}");
                MessageBox.Show($"Error al añadir ítem: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // -----------------------------------------------------
        // ⭐ MÉTODO PARA CARGAR LOS DATOS DE USUARIO CORRECTAMENTE
        // ----------------------------------------------------- 

        private void InitializePaymentMethods()
        {
            // Rellena tu cboPaymentMethod (ejemplo)
            cboPaymentMethod.Items.Add("Efectivo");
            cboPaymentMethod.Items.Add("Tarjeta de Crédito");
            cboPaymentMethod.Items.Add("Transferencia");
            cboPaymentMethod.SelectedIndex = 0; // Selecciona el primero por defecto
        }
        private bool IsWeightBased(string unit)
        {
            return unit.ToLower() == "kg" || unit.ToLower() == "gr" || unit.ToLower() == "lb";
        }

        private void LoadSelectedProductDetails(Product product) // Asume que recibes un objeto Product
        {
            if (product != null)
            {
                txtProductName.Text = product.Name;
                txtSaleProductPrice.Text = product.PricePerUnit.ToString(); // Mostrar el precio
                txtRemainingStock.Text = product.Stock.ToString(); // Mostrar el stock

                // Lógica para txt1Quantity
                if (product.Unit == "kg" || product.Unit == "g" || product.Unit == "L") // Si es un producto pesado/líquido
                {
                    txt1Quantity.Text = "0.000"; // Permitir entrada decimal con 3 decimales
                    txt1Quantity.ReadOnly = false; // Permitir al usuario cambiar la cantidad
                }
                else // Producto por unidad/paquete
                {
                    txt1Quantity.Text = "1"; // Cantidad por defecto de 1
                    txt1Quantity.ReadOnly = false; // Permitir al usuario cambiar la cantidad (ej. comprar 2 paquetes)
                }

                // También podrías enfocar el cursor automáticamente
                txt1Quantity.Focus();
            }
            else
            {
                // Limpiar si no hay producto seleccionado
                txtProductName.Clear();
                txtSaleProductPrice.Clear();
                txtRemainingStock.Clear();
                txt1Quantity.Text = "1"; // Reset a 1
            }
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

        private void LoadProductData(string searchTerm = "", string searchColumn = "") // Agregamos parámetros de búsqueda
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, Code, Name, Unit, PricePerUnit, MinimumStock, Stock, Active FROM Products WHERE Active = 1"; // ⭐ Ahora 'Active' está en SELECT

                    // ⭐ AGREGAR LÓGICA DE BÚSQUEDA
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        if (searchColumn == "Code")
                        {
                            query += " AND Code LIKE @SearchTerm";
                        }
                        else if (searchColumn == "Name")
                        {
                            query += " AND Name LIKE @SearchTerm";
                        }
                        // Si searchColumn está vacío, puedes buscar en ambos o ninguno, según lo que prefieras por defecto
                        // else { query += " AND (Code LIKE @SearchTerm OR Name LIKE @SearchTerm)"; }
                    }

                    query += " ORDER BY Name"; // Siempre ordenar para una vista consistente

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%"); // Uso de % para búsqueda parcial
                        }

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
                lblStatusBalanza.Text = "Modo Demo";
                lblStatusBalanza.ForeColor = Color.Orange;
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
            if (_serialPort != null && _serialPort.IsOpen)
            {
                DisconnectBalanza();
            }
            _serialPort = new SerialPort(selectedPort);

            // ⭐ Cargar los parámetros desde la configuración del usuario
            try
            {
                _serialPort.BaudRate = ConfiguracionUsuario.ScaleBaudRate;
                _serialPort.Parity = ConfiguracionUsuario.ScaleParity;
                _serialPort.DataBits = ConfiguracionUsuario.ScaleDataBits;
                _serialPort.StopBits = ConfiguracionUsuario.ScaleStopBits;
                _serialPort.Handshake = Handshake.None; // Este valor puede seguir fijo si siempre es el mismo

                _serialPort.Open();
                _serialPort.DataReceived += serialPort_DataReceived;
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
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.DataReceived -= serialPort_DataReceived; // Desasociar el evento
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null; // Liberar la instancia
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
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= serialPort_DataReceived;
                    _serialPort.Close();
                    Logger.Log("Puerto serial cerrado", $"Puerto: {ConfiguracionUsuario.ScalePortName}");
                }
                _serialPort?.Dispose();
                _serialPort = null;
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

        

        // Nuevo método para buscar el producto en la base de datos
       

        
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
            Logger.Log("AutoVenta DEBUG", "--> txtSearchProductCode_TextChanged llamado."); // ⭐ AÑADE ESTO
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
                                txtProductCode.Text = currentFoundProductId.ToString();
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

        // ARCHIVO: MainScreen.cs

        private void dgvProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica que no es el encabezado y que es la columna correcta.
            // ⭐ CRÍTICO: "colAsignarRapido" debe coincidir con el nombre de tu DataGridViewButtonColumn
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvProducts.Columns[e.ColumnIndex].Name == "colAsignarRapido")
            {
                // Asegúrate de que tienes un producto válido en esa fila
                if (dgvProducts.Rows[e.RowIndex].Cells["Id"].Value != null &&
                    int.TryParse(dgvProducts.Rows[e.RowIndex].Cells["Id"].Value.ToString(), out int productId))
                {
                    // Ahora sí, llama al método de asignación
                    PromptForQuickButtonAssignment(productId);
                }
            }
        }

        // ARCHIVO: MainScreen.cs

        private void PromptForQuickButtonAssignment(int productId)
        {
            // ⭐ Asegúrate de que FormQuickButtonAssigner existe y tiene el constructor adecuado
            using (FormQuickButtonAssigner assignerForm = new FormQuickButtonAssigner(productId))
            {
                if (assignerForm.ShowDialog() == DialogResult.OK)
                {
                    int buttonNumber = assignerForm.SelectedButtonNumber;
                    try
                    {
                        _quickProductService.AssignProductToQuickButton(buttonNumber, productId);
                        MessageBox.Show($"Producto asignado al botón rápido {buttonNumber}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AssignQuickProductsToButtons(); // Recarga los botones para que se vea el cambio
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Error", $"Error al asignar producto rápido: {ex.Message}");
                        MessageBox.Show($"Error al asignar producto rápido: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // EN BalanzaPOSNuevo\BalanzaPOSNuevo\MainScreen.cs

        private void btnProductQuick_Click(object sender, EventArgs e)
        {

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

        // ⭐ CORRECCIÓN: Firma del método UpdateTotalSaleDisplay
        private void UpdateTotalSaleDisplay(decimal totalAmount) // Acepta un decimal como argumento
        {
            // Asume que tienes un Label o TextBox para mostrar el total (por ejemplo, lblTotalSale)
            // lblTotalSale.Text = totalAmount.ToString("C2", System.Globalization.CultureInfo.CurrentCulture); 
            // O si usas un TextBox:
            // txtTotalSale.Text = totalAmount.ToString("C2", System.Globalization.CultureInfo.CurrentCulture);
        }
        private void btnClosePort_Click(object sender, EventArgs e)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
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

       
        private void UpdateSaleUI()
        {
            // Asigna el DataTable del servicio al DataGridView de ventas
            dgvSaleItems.DataSource = _saleService.CurrentSaleItems;
            txtTotalSale.Text = _saleService.TotalSale.ToString("C2", ConfiguracionUsuario.CurrentCulture); // Formato de moneda
                                                                                                            // ... cualquier otra actualización de UI para la venta ...

            // Si tu dgvSaleItems no tiene autogeneración de columnas, configúralas una vez
            if (dgvSaleItems.Columns.Count == 0)
            {
                ConfigureSaleDataGridViewColumns();
            }
        }

        // EN MainScreen.cs

        // ARCHIVO: MainScreen.cs

        private void btnFinalizeSale_Click(object sender, EventArgs e)
        {
            if (saleItemsDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No hay artículos en la venta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ⭐ Lógica de guardar la venta (ejemplo)
                // int newSaleId = _saleService.SaveSale(saleItemsDataTable); 

                // 1. Limpiar la fuente de datos de la venta
                saleItemsDataTable.Clear();
                Logger.Log("Venta", "Tabla de venta limpiada.");

                // 2. Limpiar la UI de venta y el producto activo
                UpdateSaleTable(); // Actualiza la grilla y el total (a 0)
                ClearProductDisplayFields(); // Limpia campos de nombre/precio y setea _selectedProduct = null
                txtSearchProductCode.Text = "";
                txt1Quantity.Text = "1";

                // También limpia el peso de la balanza si se usa
                txtWeightDisplay.Text = "0.000";

                MessageBox.Show("Venta finalizada con éxito.", "Venta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Log("Error", $"Error al finalizar la venta: {ex.Message}");
                MessageBox.Show($"Error al finalizar la venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnNewSale_Click(object sender, EventArgs e)
        {
            // Verificar si hay ítems para guardar
            if (_saleService.CurrentSaleItems.Rows.Count == 0)
            {
                MessageBox.Show("No hay productos en el carrito de venta para totalizar y guardar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Obtener los datos necesarios para finalizar la venta
                decimal discount = string.IsNullOrEmpty(txtDiscount.Text) ? 0 : Convert.ToDecimal(txtDiscount.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                string paymentMethod = cboPaymentMethod.SelectedItem?.ToString() ?? "Efectivo";
                string username = Session.Username; // Asume que Session.Username está disponible
                int cashRegisterId = 1; // ID de caja registradora, ajustar según tu lógica

                // Llamar al SaleService para finalizar y guardar la venta
                _saleService.FinalizeSale(discount, paymentMethod, username, cashRegisterId);

                // La limpieza de la UI y la preparación para una nueva venta
                // se manejan a través del evento SaleFinalized en HandleSaleFinalized.

                MessageBox.Show("Venta totalizada y guardada con éxito.", "Venta Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex) // Captura excepciones de stock o lógicas del servicio
            {
                Logger.Log("Advertencia", $"Error lógico al totalizar venta: {ex.Message}");
                MessageBox.Show(ex.Message, "Error al Totalizar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Logger.Log("Error", $"Error al totalizar y guardar venta: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al totalizar y guardar la venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // EN MainScreen.cs

       
        private decimal GetStableWeight()
{
    try
    {
        if (_serialPort == null || !_serialPort.IsOpen)
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
            if (_isQuickButtonClick) return;
            // Solo si hay una fila seleccionada
            if (dgvProducts.CurrentRow != null && dgvProducts.CurrentRow.Cells["Id"].Value != null)
            {
                try
                {
                    // 1. Obtener el ID de la fila seleccionada
                    if (!int.TryParse(dgvProducts.CurrentRow.Cells["Id"].Value.ToString(), out int productId))
                    {
                        // Si falla el parseo, salimos
                        return;
                    }

                    // 2. Usar el servicio para obtener los datos completos del producto
                    // ⭐ Esta es la clave para evitar errores de conversión y formatos de DataGridView.
                    Product product = _productService.GetProductById(productId);

                    if (product != null)
                    {
                        _selectedProduct = product; // ⭐ CRÍTICO: Establece la variable
                        // 3. Aplicar formato basado en la Configuración del Usuario (CurrentCulture para UI)
                        string weightFormat = "N" + ConfiguracionUsuario.WeightDecimals;
                        string priceFormat = "N" + ConfiguracionUsuario.CurrencyDecimals;

                        // 4. Cargar los campos de edición
                        txtProductCode.Tag = product.Id; // Usar el Tag para almacenar el ID
                        txtProductCode.Text = product.Code;
                        txtProductName.Text = product.Name;

                        // ⭐ Formatear para la UI con la cultura actual (coma o punto)
                        txtProductPrice.Text = product.PricePerUnit.ToString(priceFormat, CultureInfo.CurrentCulture);
                        txtStock.Text = product.Stock.ToString(weightFormat, CultureInfo.CurrentCulture);
                        txtMinimumStock.Text = product.MinimumStock.ToString(weightFormat, CultureInfo.CurrentCulture);

                        cboProductUnit.Text = product.Unit;
                        chkProductActive.Checked = product.Active;

                        Logger.Log("UI", $"Detalles del producto (ID: {product.Id}) cargados en los campos de edición.");
                    }
                    else
                    {
                        ClearProductDisplayFields(); // <--- LLAMA A UN MÉTODO QUE SÓLO LIMPIA LOS CAMPOS DE EDICIÓN
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Error", $"Error al cargar detalles de producto de dgvProducts: {ex.Message}");
                    // Este log debería disminuir drásticamente con esta nueva lógica.
                }
            }
            else
            {
                // Limpiar los campos si no hay fila seleccionada
                ClearProductControls();
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
            txtProductCode.SelectionStart = txtProductCode.Text.Length;
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
            txtProductCode.Tag = 0;
            txtProductCode.Text = string.Empty;
            txtProductName.Text = string.Empty;
            txtProductPrice.Text = "0,00"; // O el formato inicial que uses
            txtStock.Text = "0,00";
            txtMinimumStock.Text = "0,00";
            cboProductUnit.SelectedIndex = -1;
            chkProductActive.Checked = true;
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
            // ⭐ DECLARA LAS VARIABLES AQUÍ (AL INICIO DEL MÉTODO)
            decimal pricePerUnit = 0m; // Inicializa con un valor por defecto
            decimal stock = 0m;        // Inicializa con un valor por defecto
            decimal minimumStock = 0m; // Inicializa con un valor por defecto

            try
            {
                string productCode = txtProductCode.Text.Trim();
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

                // ⭐ ASIGNA EL VALOR A LAS VARIABLES YA DECLARADAS
                if (!decimal.TryParse(priceText.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out pricePerUnit) || pricePerUnit < 0)
                {
                    Logger.Log("Error al agregar producto", "El precio debe ser un número válido");
                    MessageBox.Show("El precio debe ser un número válido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(stockText.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out stock) || stock < 0)
                {
                    Logger.Log("Error al agregar producto", "El stock debe ser un número válido");
                    MessageBox.Show("El stock debe ser un número válido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(minimumStockText.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out minimumStock) || minimumStock < 0)
                {
                    Logger.Log("Error al agregar producto", "El stock mínimo debe ser un número válido");
                    MessageBox.Show("El stock mínimo debe ser un número válido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                productCode = productCode.PadLeft(6, '0');
                using (SQLiteConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // 1. Verificar si el código ya existe (Este código es correcto)
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

                    // ⭐ Las variables ya existen aquí y pueden ser utilizadas
                    pricePerUnit = Math.Round(pricePerUnit, ConfiguracionUsuario.CurrencyDecimals);
                    stock = Math.Round(stock, ConfiguracionUsuario.WeightDecimals);
                    minimumStock = Math.Round(minimumStock, ConfiguracionUsuario.WeightDecimals);

                    query = @"
                INSERT INTO Products (Code, Name, PricePerUnit, Unit, Stock, MinimumStock, Active)
                VALUES (@Code, @Name, @PricePerUnit, @Unit, @Stock, @MinimumStock, @Active)";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Code", productCode);
                        cmd.Parameters.AddWithValue("@Name", productName);
                        cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit); // ⭐ Ahora funciona
                        cmd.Parameters.AddWithValue("@Unit", unit);
                        cmd.Parameters.AddWithValue("@Stock", stock);               // ⭐ Ahora funciona
                        cmd.Parameters.AddWithValue("@MinimumStock", minimumStock); // ⭐ Ahora funciona
                        cmd.Parameters.AddWithValue("@Active", isActive ? 1 : 0);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        long lastInsertId = conn.LastInsertRowId;

                        Logger.Log("Producto agregado", $"Filas afectadas: {rowsAffected}, Id generado={lastInsertId}, Code={productCode}");
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

        // ARCHIVO: MainScreen.cs

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProducts.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Por favor, seleccione un producto para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Leer el ID de la fila seleccionada
                DataGridViewRow row = dgvProducts.SelectedRows[0];
                if (row.Cells["Id"].Value == null)
                {
                    MessageBox.Show("ID de producto no válido o no seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ⭐ Usamos CurrentCulture para leer el input del usuario (si usa coma o punto según su sistema)
                if (!long.TryParse(row.Cells["Id"].Value.ToString(), out long productId))
                {
                    MessageBox.Show("Error al obtener ID del producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string code = txtProductCode.Text.Trim().PadLeft(6, '0');
                string name = txtProductName.Text.Trim();
                string unit = cboProductUnit.SelectedItem?.ToString().Trim() ?? "";

                // Validación y parseo de decimales desde los TextBoxes de la UI
                if (!decimal.TryParse(txtProductPrice.Text, NumberStyles.Currency | NumberStyles.Float, CultureInfo.CurrentCulture, out decimal pricePerUnit) || pricePerUnit <= 0)
                {
                    MessageBox.Show("Por favor, ingrese un precio válido (ej: 12,50).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!decimal.TryParse(txtStock.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out decimal stock) || stock < 0)
                {
                    MessageBox.Show("Por favor, ingrese un stock válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!decimal.TryParse(txtMinimumStock.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out decimal minimumStock) || minimumStock < 0)
                {
                    MessageBox.Show("Por favor, ingrese un stock mínimo válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Crear objeto Product con los nuevos valores
                Product productToUpdate = new Product
                {
                    Id = (int)productId,
                    Code = code,
                    Name = name,
                    Unit = unit,
                    PricePerUnit = pricePerUnit,
                    Stock = stock,
                    MinimumStock = minimumStock,
                    Active = chkProductActive.Checked // Leer del CheckBox
                };

                // Actualizar usando el ProductService
                _productService.UpdateProduct(productToUpdate);

                MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar el DataGridView para ver los cambios
                LoadProductData();
            }
            catch (Exception ex)
            {
                Logger.Log("Error en btnUpdateProduct_Click", ex.Message);
                MessageBox.Show($"Error al actualizar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearProductFields()
        {
            txtProductCode.Text = string.Empty;
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

        // EN: MainScreen.cs (dentro de ClearSaleItemFields)

        private void ClearSaleItemFields()
        {
            // ⭐ Asegúrate de limpiar el producto seleccionado
            _selectedProduct = null;

            txtProductCode.Text = string.Empty;
            txtSearchProductCode.Text = string.Empty;
            txtSaleProductName.Text = string.Empty;
            txtSaleProductPrice.Text = "0.00";
            txtStock.Text = "0.00";
            txtMinimumStock.Text = "0.00";
            cboProductUnit.Text = string.Empty;
            txtRemainingStock.Text = "0.00";
            txtRemainingStock.ForeColor = Color.Black;
            txt1Quantity.Text = "0.00";
            cboWeightUnit.Text = string.Empty;
            cboWeightUnit.Enabled = true; // Habilitar si se limpia el producto
            lblSaleProductUnit.Text = string.Empty; // Asegúrate de limpiar la unidad

            // También limpiar las variables globales si las usas
            currentFoundProductId = 0;
            currentFoundProductCode = string.Empty;
            currentFoundProductName = string.Empty;
            currentFoundProductPrice = 0m;
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
            txtProductCode.BackColor = string.IsNullOrWhiteSpace(txtProductCode.Text) ? Color.FromArgb(255, 204, 204) : Color.White;
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
                   serialPort, // Argumento 4: string (scalePortName)
                   baudRate, // Argumento 5: int (scaleBaudRate)
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

        // ARCHIVO: MainScreen.cs

        private void btnDisconnectBalanza_Click(object sender, EventArgs e)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = null; // ⭐ CRÍTICO: Liberar la instancia
                    Logger.Log("Balanza", $"Balanza desconectada del puerto {ConfiguracionUsuario.ScalePortName}.");
                    MessageBox.Show("Balanza desconectada correctamente.", "Desconexión", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Logger.Log("Balanza", "Intento de desconexión pero el puerto no estaba abierto.");
                    MessageBox.Show("La balanza ya estaba desconectada.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // ⭐ Detener el temporizador del heartbeat al desconectar
                if (balanzaHeartbeatTimer != null && balanzaHeartbeatTimer.Enabled)
                {
                    balanzaHeartbeatTimer.Stop();
                    Logger.Log("Balanza", "Temporizador de heartbeat detenido por desconexión manual.");
                }

                UpdateBalanzaStatusUI("Desconectada", Color.Red);
                btnConnectBalanza.Enabled = true;
                btnDisconnectBalanza.Enabled = false;
            }
            catch (Exception ex)
            {
                Logger.Log("Error en btnDisconnectBalanza_Click", ex.Message);
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
                lblStatusBalanza.Text = "Modo Demo Activo";
                lblStatusBalanza.ForeColor = Color.Green;
            }
            else
            {
                // Detener simulador y restaurar estado
                balanzaSimulator?.Stop();
                balanzaSimulator = null;
                btnConnectBalanza.Enabled = true;
                btnDisconnectBalanza.Enabled = _serialPort != null && _serialPort.IsOpen;
                lblStatusBalanza.Text = _serialPort != null && _serialPort.IsOpen ? "Conectado" : "Desconectado";
                lblStatusBalanza.ForeColor = _serialPort != null && _serialPort.IsOpen ? Color.Green : Color.Red;
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
           // LoadProductData();
        }

        // En tu formulario MainScreen.cs

        
        // **Nota:** No modifiques LoadUserData, ya que es para usuarios, no para productos.
        private void txtSearchProductCode_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearchProductCode.Text))
            {
                if (int.TryParse(txtSearchProductCode.Text, out int code))
                {
                    txtSearchProductCode.Text = code.ToString("D6");
                }
                else
                {
                    MessageBox.Show("Por favor, introduce un código numérico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSearchProductCode.Clear();
                    txtSearchProductCode.Focus();
                    ClearProductDisplayFields(); // Limpiar UI si el código es inválido
                }
            }
            else
            {
                ClearProductDisplayFields(); // Limpiar UI si el campo queda vacío al salir
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
            txtProductCode.SelectAll();
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
                // Ajusta este nombre, ya que estás modificando currencyDecimals
                int newCurrencyDecimals = (int)numericUpDownDecimalesPrecio.Value;

                ConfiguracionUsuario.SaveSettings(
                    weightDecimals: ConfiguracionUsuario.WeightDecimals, // Asegúrate de que este no se cambie si solo editas precio
                    currencyDecimals: newCurrencyDecimals, // ⭐ Nuevo valor de decimales de moneda
                    currencySymbol: ConfiguracionUsuario.CurrencySymbol,
                    scalePortName: ConfiguracionUsuario.ScalePortName,
                    scaleBaudRate: ConfiguracionUsuario.ScaleBaudRate,
                    scaleParity: ConfiguracionUsuario.ScaleParity,
                    scaleDataBits: ConfiguracionUsuario.ScaleDataBits,
                    scaleStopBits: ConfiguracionUsuario.ScaleStopBits
                );

                // ⭐ NO uses InvariantCulture para TOSTRING en la UI si quieres coma decimal en español.
                // Usa CurrentCulture o déjalo sin especificar para que use la del sistema.
                string priceFormat = "N" + ConfiguracionUsuario.CurrencyDecimals;

                if (txtProductPrice != null && decimal.TryParse(txtProductPrice.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal price)) // ⭐ TryParse con CurrentCulture
                {
                    txtProductPrice.Text = price.ToString(priceFormat, CultureInfo.CurrentCulture); // ⭐ ToString con CurrentCulture
                }
                if (txtTotalSale != null && decimal.TryParse(txtTotalSale.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal total)) // ⭐ TryParse con CurrentCulture
                {
                    txtTotalSale.Text = total.ToString(priceFormat, CultureInfo.CurrentCulture); // ⭐ ToString con CurrentCulture
                }

                // Actualizar formatos de columna en DataGridViews
                if (dgvSaleItems.Columns.Contains("PricePerUnit"))
                {
                    dgvSaleItems.Columns["PricePerUnit"].DefaultCellStyle.Format = priceFormat;
                }
                if (dgvSaleItems.Columns.Contains("TotalPrice"))
                {
                    dgvSaleItems.Columns["TotalPrice"].DefaultCellStyle.Format = priceFormat;
                }
                if (dgvProducts.Columns.Contains("PricePerUnit"))
                {
                    dgvProducts.Columns["PricePerUnit"].DefaultCellStyle.Format = priceFormat;
                }

                // ⭐ NO veo `numericUpDownDecimalesBalanza_ValueChanged` que actualice los decimales de peso
                // Si el numericUpDownDecimalesPrecio_ValueChanged es solo para precios, no debería llamar a UpdateProductDecimals()
                // o UpdateProductDecimals() debe actualizar solo los decimales de PRECIO.
                // Si se debe actualizar el formato de peso, necesitarás un método similar para el numericUpDownDecimalesBalanza
                // Y llama a LoadProductData() para que los formatos se apliquen a los productos recién cargados.
                LoadProductData(); // Esto recargará el dgvProducts y aplicará el formato

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
                LoadProductsToDataGridView();
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

        // ARCHIVO: MainScreen.cs

        // ARCHIVO: MainScreen.cs

        private void btnConnectBalanza_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedPort = cmbPorts.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedPort))
                {
                    MessageBox.Show("Seleccione un puerto serial.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (selectedPort != ConfiguracionUsuario.ScalePortName)
                {
                    ConfiguracionUsuario.SaveSettings(
                        ConfiguracionUsuario.WeightDecimals,
                        ConfiguracionUsuario.CurrencyDecimals,
                        ConfiguracionUsuario.CurrencySymbol,
                        selectedPort,
                        ConfiguracionUsuario.ScaleBaudRate,
                        ConfiguracionUsuario.ScaleParity,
                        ConfiguracionUsuario.ScaleDataBits,
                        ConfiguracionUsuario.ScaleStopBits
                    );
                    Logger.Log("Conexión Balanza", $"Configuración de puerto actualizada a {selectedPort}");
                }

                // ⭐ Ahora, simplemente llamamos a LoadScaleSettingsAndConnect para manejar la conexión
                LoadScaleSettingsAndConnect();

                // La UI ya se actualiza dentro de LoadScaleSettingsAndConnect o en el DataReceived
                // Elimina estas líneas si ya se manejan en LoadScaleSettingsAndConnect
                // lblStatusBalanza.Text = "Conectado"; 
                // lblStatusBalanza.ForeColor = Color.Green; 

                // Los botones se habilitan/deshabilitan en LoadScaleSettingsAndConnect
                // btnConnectBalanza.Enabled = false; 
                // btnDisconnectBalanza.Enabled = true; 

                Logger.Log("Conexión Balanza", $"Balanza conectada exitosamente al puerto {ConfiguracionUsuario.ScalePortName}.");
                MessageBox.Show($"Balanza conectada al puerto {ConfiguracionUsuario.ScalePortName} con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Log("Error en btnConnectBalanza_Click", ex.Message);
                MessageBox.Show($"Error al conectar la balanza: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // El estado y los botones se gestionan en LoadScaleSettingsAndConnect() si hubo error en SetupSerialPort()
                // o manualmente si el error es antes de eso.
                UpdateBalanzaStatusUI("Desconectada", Color.Red);
                btnConnectBalanza.Enabled = true;
                btnDisconnectBalanza.Enabled = false;
                if (balanzaHeartbeatTimer != null && balanzaHeartbeatTimer.Enabled)
                {
                    balanzaHeartbeatTimer.Stop();
                    Logger.Log("Balanza", "Temporizador de heartbeat detenido por error de conexión.");
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
                    using (var cmd = new SQLiteCommand("SELECT ScalePortName, ScaleBaudRate, ScaleParity, ScaleDataBits, ScaleStopBits FROM Settings WHERE Id = 1", conn))
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
               
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = null;
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
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtProductPrice.Text = "";
            cboProductUnit.SelectedIndex = -1;
            txtStock.Text = "";
            txtMinimumStock.Text = "";
            chkProductActive.Checked = true;
            MessageBox.Show("Campos limpiados.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ARCHIVO: MainScreen.cs

        private void numericUpDownDecimalesBalanza_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int newWeightDecimals = (int)numericUpDownDecimalesBalanza.Value;
                ConfiguracionUsuario.SaveSettings(
                    weightDecimals: newWeightDecimals,
                    currencyDecimals: ConfiguracionUsuario.CurrencyDecimals,
                    currencySymbol: ConfiguracionUsuario.CurrencySymbol,
                    scalePortName: ConfiguracionUsuario.ScalePortName,
                    scaleBaudRate: ConfiguracionUsuario.ScaleBaudRate,
                    scaleParity: ConfiguracionUsuario.ScaleParity,
                    scaleDataBits: ConfiguracionUsuario.ScaleDataBits,
                    scaleStopBits: ConfiguracionUsuario.ScaleStopBits
                );

                // ⭐ CORRECCIÓN: Usar CurrentCulture para MOSTRAR en la UI
                // InvariantCulture siempre usa punto decimal, pero tu usuario puede esperar coma.
                if (txtWeightDisplay != null && decimal.TryParse(txtWeightDisplay.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out decimal weight)) // ⭐ Parsear también con CurrentCulture
                {
                    txtWeightDisplay.Text = weight.ToString("N" + ConfiguracionUsuario.WeightDecimals, System.Globalization.CultureInfo.CurrentCulture); // ⭐ Mostrar con CurrentCulture
                }

                // Formato para DataGridViews
                string weightFormat = "N" + ConfiguracionUsuario.WeightDecimals; // Crea el formato una vez

                if (dgvSaleItems.Columns.Contains("Quantity")) // Asumo que Quantity usa decimales de peso
                {
                    dgvSaleItems.Columns["Quantity"].DefaultCellStyle.Format = weightFormat;
                }
                // No encontré "Weight" en tu dgvSaleItems, si existe, también aplícale el formato.
                // if (dgvSaleItems.Columns.Contains("Weight"))
                // {
                //     dgvSaleItems.Columns["Weight"].DefaultCellStyle.Format = weightFormat;
                // }

                if (dgvProducts.Columns.Contains("Stock"))
                {
                    dgvProducts.Columns["Stock"].DefaultCellStyle.Format = weightFormat;
                }
                if (dgvProducts.Columns.Contains("MinimumStock"))
                {
                    dgvProducts.Columns["MinimumStock"].DefaultCellStyle.Format = weightFormat;
                }

                // Es posible que necesites recargar los datos para que el formato se aplique completamente
                // a las celdas ya cargadas en los DataGridViews.
                LoadProductData(); // Para dgvProducts
                                   // Y si tienes un método para recargar dgvSaleItems, llámalo aquí también.

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
                                txtProductCode.Text = reader["Code"].ToString();
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

        private void txt1Quantity_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void txtDiscount_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
    #endregion
}
