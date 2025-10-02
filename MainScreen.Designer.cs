using System;
using System.IO; // Agregado para File
using System.Windows.Forms;



namespace BalanzaPOSNuevo
{
    partial class MainScreen
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSales = new System.Windows.Forms.TabPage();
            this.btnSetTare = new System.Windows.Forms.Button();
            this.btnClearTare = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.btnTestQuickProduct = new System.Windows.Forms.Button();
            this.cboPaymentMethod = new System.Windows.Forms.ComboBox();
            this.btnFinalizeSale = new System.Windows.Forms.Button();
            this.btnClearAllItems = new System.Windows.Forms.Button();
            this.btnNewSale = new System.Windows.Forms.Button();
            this.btnDevolucion = new System.Windows.Forms.Button();
            this.btnRemoveSaleItem = new System.Windows.Forms.Button();
            this.txtDiscount = new System.Windows.Forms.TextBox();
            this.productPanelVentas = new System.Windows.Forms.Panel();
            this.label23 = new System.Windows.Forms.Label();
            this.txtRemainingStock = new System.Windows.Forms.TextBox();
            this.txtSearchProductCode = new System.Windows.Forms.MaskedTextBox();
            this.lblSaleProductUnit = new System.Windows.Forms.Label();
            this.txt1Quantity = new System.Windows.Forms.TextBox();
            this.btnAddSaleItem = new System.Windows.Forms.Button();
            this.txtSaleProductPrice = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSaleProductName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblStatusBalanza = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnProductQuick8 = new System.Windows.Forms.Button();
            this.btnProductQuick10 = new System.Windows.Forms.Button();
            this.btnProductQuick9 = new System.Windows.Forms.Button();
            this.btnProductQuick7 = new System.Windows.Forms.Button();
            this.btnProductQuick6 = new System.Windows.Forms.Button();
            this.btnProductQuick5 = new System.Windows.Forms.Button();
            this.btnProductQuick4 = new System.Windows.Forms.Button();
            this.btnProductQuick3 = new System.Windows.Forms.Button();
            this.btnProductQuick2 = new System.Windows.Forms.Button();
            this.btnProductQuick1 = new System.Windows.Forms.Button();
            this.txtTotalSale = new System.Windows.Forms.TextBox();
            this.labelTimeout = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dgvSaleItems = new System.Windows.Forms.DataGridView();
            this.panelWeightInput = new System.Windows.Forms.Panel();
            this.txtWeightDisplay = new System.Windows.Forms.TextBox();
            this.cboWeightUnit = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageProducts = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgvProducts = new System.Windows.Forms.DataGridView();
            this.productPanel = new System.Windows.Forms.Panel();
            this.txtStock = new System.Windows.Forms.MaskedTextBox();
            this.txtProductPrice = new System.Windows.Forms.MaskedTextBox();
            this.txtMinimumStock = new System.Windows.Forms.MaskedTextBox();
            this.txtProductCode = new System.Windows.Forms.MaskedTextBox();
            this.btnClearProductFields = new System.Windows.Forms.Button();
            this.btnDeleteProduct = new System.Windows.Forms.Button();
            this.btnUpdateProduct = new System.Windows.Forms.Button();
            this.btnAddProduct = new System.Windows.Forms.Button();
            this.chkProductActive = new System.Windows.Forms.CheckBox();
            this.cboProductUnit = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPageUsers = new System.Windows.Forms.TabPage();
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.panelUserDetails = new System.Windows.Forms.Panel();
            this.btnClearUserFields = new System.Windows.Forms.Button();
            this.btnDeleteUser = new System.Windows.Forms.Button();
            this.btnUpdateUser = new System.Windows.Forms.Button();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.chkUserExpires = new System.Windows.Forms.CheckBox();
            this.chkActiveUser = new System.Windows.Forms.CheckBox();
            this.chkIsAdminUser = new System.Windows.Forms.CheckBox();
            this.txtPasswordUser = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtUsernameUser = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPageReports = new System.Windows.Forms.TabPage();
            this.btnExportStockReport = new System.Windows.Forms.Button();
            this.cmbProductFilter = new System.Windows.Forms.ComboBox();
            this.btnGenerateSalesReport = new System.Windows.Forms.Button();
            this.dgvReports = new System.Windows.Forms.DataGridView();
            this.btnGenerateHourlyReport = new System.Windows.Forms.Button();
            this.dgvHourlyReports = new System.Windows.Forms.DataGridView();
            this.btnGenerateStockReport = new System.Windows.Forms.Button();
            this.dgvStockReports = new System.Windows.Forms.DataGridView();
            this.btnDeleteAllSalesData = new System.Windows.Forms.Button();
            this.lblStatusMessage = new System.Windows.Forms.Label();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.btnClearFilters = new System.Windows.Forms.Button();
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.dgvSalesReports = new System.Windows.Forms.DataGridView();
            this.dtpReportEndDate = new System.Windows.Forms.DateTimePicker();
            this.dtpReportStartDate = new System.Windows.Forms.DateTimePicker();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtCurrencySymbol = new System.Windows.Forms.ComboBox();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.numericUpDownDecimalesBalanza = new System.Windows.Forms.NumericUpDown();
            this.chkDemoMode = new System.Windows.Forms.CheckBox();
            this.btnConnectBalanza = new System.Windows.Forms.Button();
            this.btnDisconnectBalanza = new System.Windows.Forms.Button();
            this.cboStopBits = new System.Windows.Forms.ComboBox();
            this.cboParity = new System.Windows.Forms.ComboBox();
            this.cboBaudRate = new System.Windows.Forms.ComboBox();
            this.cboDataBits = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnRefreshPorts = new System.Windows.Forms.Button();
            this.cmbPorts = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.numericUpDownDecimalesPrecio = new System.Windows.Forms.NumericUpDown();
            this.labelDecimalesPrecio = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPageSales.SuspendLayout();
            this.panel5.SuspendLayout();
            this.productPanelVentas.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaleItems)).BeginInit();
            this.panelWeightInput.SuspendLayout();
            this.tabPageProducts.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            this.productPanel.SuspendLayout();
            this.tabPageUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.panelUserDetails.SuspendLayout();
            this.tabPageReports.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHourlyReports)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockReports)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalesReports)).BeginInit();
            this.tabPageSettings.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDecimalesBalanza)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDecimalesPrecio)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageSales);
            this.tabControl1.Controls.Add(this.tabPageProducts);
            this.tabControl1.Controls.Add(this.tabPageUsers);
            this.tabControl1.Controls.Add(this.tabPageReports);
            this.tabControl1.Controls.Add(this.tabPageSettings);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1582, 853);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageSales
            // 
            this.tabPageSales.Controls.Add(this.btnSetTare);
            this.tabPageSales.Controls.Add(this.btnClearTare);
            this.tabPageSales.Controls.Add(this.panel5);
            this.tabPageSales.Controls.Add(this.productPanelVentas);
            this.tabPageSales.Controls.Add(this.panel4);
            this.tabPageSales.Controls.Add(this.panel2);
            this.tabPageSales.Controls.Add(this.txtTotalSale);
            this.tabPageSales.Controls.Add(this.labelTimeout);
            this.tabPageSales.Controls.Add(this.label6);
            this.tabPageSales.Controls.Add(this.dgvSaleItems);
            this.tabPageSales.Controls.Add(this.panelWeightInput);
            this.tabPageSales.Location = new System.Drawing.Point(4, 37);
            this.tabPageSales.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageSales.Name = "tabPageSales";
            this.tabPageSales.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageSales.Size = new System.Drawing.Size(1574, 812);
            this.tabPageSales.TabIndex = 0;
            this.tabPageSales.Text = "Ventas";
            this.tabPageSales.UseVisualStyleBackColor = true;
            this.tabPageSales.Click += new System.EventHandler(this.tabPageSales_Click);
            // 
            // btnSetTare
            // 
            this.btnSetTare.Location = new System.Drawing.Point(1288, 3);
            this.btnSetTare.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetTare.Name = "btnSetTare";
            this.btnSetTare.Size = new System.Drawing.Size(175, 55);
            this.btnSetTare.TabIndex = 4;
            this.btnSetTare.Text = "Fijar Tara";
            this.btnSetTare.UseVisualStyleBackColor = true;
            this.btnSetTare.Click += new System.EventHandler(this.btnSetTare_Click);
            // 
            // btnClearTare
            // 
            this.btnClearTare.Location = new System.Drawing.Point(1299, 102);
            this.btnClearTare.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearTare.Name = "btnClearTare";
            this.btnClearTare.Size = new System.Drawing.Size(175, 61);
            this.btnClearTare.TabIndex = 5;
            this.btnClearTare.Text = "Limpiar Tara";
            this.btnClearTare.UseVisualStyleBackColor = true;
            this.btnClearTare.Click += new System.EventHandler(this.btnClearTare_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label25);
            this.panel5.Controls.Add(this.label24);
            this.panel5.Controls.Add(this.btnTestQuickProduct);
            this.panel5.Controls.Add(this.cboPaymentMethod);
            this.panel5.Controls.Add(this.btnFinalizeSale);
            this.panel5.Controls.Add(this.btnClearAllItems);
            this.panel5.Controls.Add(this.btnNewSale);
            this.panel5.Controls.Add(this.btnDevolucion);
            this.panel5.Controls.Add(this.btnRemoveSaleItem);
            this.panel5.Controls.Add(this.txtDiscount);
            this.panel5.Location = new System.Drawing.Point(0, 645);
            this.panel5.Margin = new System.Windows.Forms.Padding(4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1578, 163);
            this.panel5.TabIndex = 10;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(3, 55);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(168, 28);
            this.label25.TabIndex = 3;
            this.label25.Text = "Método de Pago";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(12, 14);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(112, 28);
            this.label24.TabIndex = 2;
            this.label24.Text = "Descuento";
            // 
            // btnTestQuickProduct
            // 
            this.btnTestQuickProduct.Location = new System.Drawing.Point(115, 100);
            this.btnTestQuickProduct.Margin = new System.Windows.Forms.Padding(4);
            this.btnTestQuickProduct.Name = "btnTestQuickProduct";
            this.btnTestQuickProduct.Size = new System.Drawing.Size(139, 40);
            this.btnTestQuickProduct.TabIndex = 6;
            this.btnTestQuickProduct.Text = "ADMIN.";
            this.btnTestQuickProduct.UseVisualStyleBackColor = true;
            this.btnTestQuickProduct.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // cboPaymentMethod
            // 
            this.cboPaymentMethod.FormattingEnabled = true;
            this.cboPaymentMethod.Items.AddRange(new object[] {
            "EFECTIVO",
            "TARJETA"});
            this.cboPaymentMethod.Location = new System.Drawing.Point(178, 56);
            this.cboPaymentMethod.Margin = new System.Windows.Forms.Padding(4);
            this.cboPaymentMethod.Name = "cboPaymentMethod";
            this.cboPaymentMethod.Size = new System.Drawing.Size(159, 36);
            this.cboPaymentMethod.TabIndex = 1;
            // 
            // btnFinalizeSale
            // 
            this.btnFinalizeSale.Location = new System.Drawing.Point(372, 44);
            this.btnFinalizeSale.Name = "btnFinalizeSale";
            this.btnFinalizeSale.Size = new System.Drawing.Size(199, 71);
            this.btnFinalizeSale.TabIndex = 1;
            this.btnFinalizeSale.Text = "SUB-TOTAL";
            this.btnFinalizeSale.UseVisualStyleBackColor = true;
            this.btnFinalizeSale.Click += new System.EventHandler(this.btnFinalizeSale_Click);
            // 
            // btnClearAllItems
            // 
            this.btnClearAllItems.Location = new System.Drawing.Point(1028, 45);
            this.btnClearAllItems.Name = "btnClearAllItems";
            this.btnClearAllItems.Size = new System.Drawing.Size(184, 69);
            this.btnClearAllItems.TabIndex = 3;
            this.btnClearAllItems.Text = "Limpiar Venta";
            this.btnClearAllItems.UseVisualStyleBackColor = true;
            this.btnClearAllItems.Click += new System.EventHandler(this.btnClearAllItems_Click);
            // 
            // btnNewSale
            // 
            this.btnNewSale.Location = new System.Drawing.Point(596, 44);
            this.btnNewSale.Name = "btnNewSale";
            this.btnNewSale.Size = new System.Drawing.Size(211, 71);
            this.btnNewSale.TabIndex = 4;
            this.btnNewSale.Text = "Totalizador";
            this.btnNewSale.UseVisualStyleBackColor = true;
            this.btnNewSale.Click += new System.EventHandler(this.btnNewSale_Click);
            // 
            // btnDevolucion
            // 
            this.btnDevolucion.Location = new System.Drawing.Point(1238, 45);
            this.btnDevolucion.Name = "btnDevolucion";
            this.btnDevolucion.Size = new System.Drawing.Size(186, 69);
            this.btnDevolucion.TabIndex = 9;
            this.btnDevolucion.Text = "Devolución";
            this.btnDevolucion.UseVisualStyleBackColor = true;
            this.btnDevolucion.Click += new System.EventHandler(this.btnDevolucion_Click);
            // 
            // btnRemoveSaleItem
            // 
            this.btnRemoveSaleItem.Location = new System.Drawing.Point(828, 44);
            this.btnRemoveSaleItem.Name = "btnRemoveSaleItem";
            this.btnRemoveSaleItem.Size = new System.Drawing.Size(184, 68);
            this.btnRemoveSaleItem.TabIndex = 5;
            this.btnRemoveSaleItem.Text = "Eliminar Item";
            this.btnRemoveSaleItem.UseVisualStyleBackColor = true;
            this.btnRemoveSaleItem.Click += new System.EventHandler(this.btnRemoveSaleItem_Click);
            // 
            // txtDiscount
            // 
            this.txtDiscount.Location = new System.Drawing.Point(178, 14);
            this.txtDiscount.Margin = new System.Windows.Forms.Padding(4);
            this.txtDiscount.Name = "txtDiscount";
            this.txtDiscount.Size = new System.Drawing.Size(159, 34);
            this.txtDiscount.TabIndex = 0;
            this.txtDiscount.TextChanged += new System.EventHandler(this.txtDiscount_TextChanged_1);
            // 
            // productPanelVentas
            // 
            this.productPanelVentas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.productPanelVentas.Controls.Add(this.label23);
            this.productPanelVentas.Controls.Add(this.txtRemainingStock);
            this.productPanelVentas.Controls.Add(this.txtSearchProductCode);
            this.productPanelVentas.Controls.Add(this.lblSaleProductUnit);
            this.productPanelVentas.Controls.Add(this.txt1Quantity);
            this.productPanelVentas.Controls.Add(this.btnAddSaleItem);
            this.productPanelVentas.Controls.Add(this.txtSaleProductPrice);
            this.productPanelVentas.Controls.Add(this.label5);
            this.productPanelVentas.Controls.Add(this.txtSaleProductName);
            this.productPanelVentas.Controls.Add(this.label4);
            this.productPanelVentas.Controls.Add(this.label3);
            this.productPanelVentas.Location = new System.Drawing.Point(1210, 166);
            this.productPanelVentas.Margin = new System.Windows.Forms.Padding(4);
            this.productPanelVentas.Name = "productPanelVentas";
            this.productPanelVentas.Size = new System.Drawing.Size(360, 474);
            this.productPanelVentas.TabIndex = 1;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Segoe UI", 13.2F, System.Drawing.FontStyle.Bold);
            this.label23.Location = new System.Drawing.Point(22, 316);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(88, 30);
            this.label23.TabIndex = 10;
            this.label23.Text = "STOCK:";
            // 
            // txtRemainingStock
            // 
            this.txtRemainingStock.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.txtRemainingStock.Location = new System.Drawing.Point(181, 325);
            this.txtRemainingStock.Margin = new System.Windows.Forms.Padding(4);
            this.txtRemainingStock.Name = "txtRemainingStock";
            this.txtRemainingStock.ReadOnly = true;
            this.txtRemainingStock.Size = new System.Drawing.Size(170, 38);
            this.txtRemainingStock.TabIndex = 9;
            // 
            // txtSearchProductCode
            // 
            this.txtSearchProductCode.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.txtSearchProductCode.Location = new System.Drawing.Point(165, 59);
            this.txtSearchProductCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtSearchProductCode.Mask = "000000";
            this.txtSearchProductCode.Name = "txtSearchProductCode";
            this.txtSearchProductCode.PromptChar = '0';
            this.txtSearchProductCode.Size = new System.Drawing.Size(184, 38);
            this.txtSearchProductCode.TabIndex = 9;
            this.txtSearchProductCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSearchProductCode.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.txtSearchProductCode_MaskInputRejected_1);
            this.txtSearchProductCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchProductCode_KeyDown);
            // 
            // lblSaleProductUnit
            // 
            this.lblSaleProductUnit.AutoSize = true;
            this.lblSaleProductUnit.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblSaleProductUnit.Location = new System.Drawing.Point(4, 258);
            this.lblSaleProductUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSaleProductUnit.Name = "lblSaleProductUnit";
            this.lblSaleProductUnit.Size = new System.Drawing.Size(106, 31);
            this.lblSaleProductUnit.TabIndex = 8;
            this.lblSaleProductUnit.Text = "UNIDAD";
            // 
            // txt1Quantity
            // 
            this.txt1Quantity.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.txt1Quantity.Location = new System.Drawing.Point(188, 258);
            this.txt1Quantity.Margin = new System.Windows.Forms.Padding(4);
            this.txt1Quantity.Name = "txt1Quantity";
            this.txt1Quantity.Size = new System.Drawing.Size(163, 38);
            this.txt1Quantity.TabIndex = 6;
            this.txt1Quantity.TextChanged += new System.EventHandler(this.txt1Quantity_TextChanged_1);
            // 
            // btnAddSaleItem
            // 
            this.btnAddSaleItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAddSaleItem.Location = new System.Drawing.Point(10, 371);
            this.btnAddSaleItem.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddSaleItem.Name = "btnAddSaleItem";
            this.btnAddSaleItem.Size = new System.Drawing.Size(331, 60);
            this.btnAddSaleItem.TabIndex = 7;
            this.btnAddSaleItem.Text = "Agregar Item";
            this.btnAddSaleItem.UseVisualStyleBackColor = true;
            this.btnAddSaleItem.Click += new System.EventHandler(this.btnAddSaleItem_Click);
            // 
            // txtSaleProductPrice
            // 
            this.txtSaleProductPrice.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.txtSaleProductPrice.Location = new System.Drawing.Point(181, 192);
            this.txtSaleProductPrice.Margin = new System.Windows.Forms.Padding(4);
            this.txtSaleProductPrice.Name = "txtSaleProductPrice";
            this.txtSaleProductPrice.ReadOnly = true;
            this.txtSaleProductPrice.Size = new System.Drawing.Size(170, 38);
            this.txtSaleProductPrice.TabIndex = 6;
            this.txtSaleProductPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(6, 192);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 33);
            this.label5.TabIndex = 5;
            this.label5.Text = "Precio/Kg:";
            // 
            // txtSaleProductName
            // 
            this.txtSaleProductName.AutoSize = true;
            this.txtSaleProductName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSaleProductName.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.txtSaleProductName.Location = new System.Drawing.Point(165, 8);
            this.txtSaleProductName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtSaleProductName.Name = "txtSaleProductName";
            this.txtSaleProductName.Size = new System.Drawing.Size(268, 33);
            this.txtSaleProductName.TabIndex = 4;
            this.txtSaleProductName.Text = "                                          ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(9, 8);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 33);
            this.label4.TabIndex = 2;
            this.label4.Text = "Producto:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(11, 64);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 33);
            this.label3.TabIndex = 0;
            this.label3.Text = "Código:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblStatusBalanza);
            this.panel4.Controls.Add(this.label21);
            this.panel4.Controls.Add(this.lblDateTime);
            this.panel4.Location = new System.Drawing.Point(8, 4);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(329, 165);
            this.panel4.TabIndex = 9;
            // 
            // lblStatusBalanza
            // 
            this.lblStatusBalanza.AutoSize = true;
            this.lblStatusBalanza.ForeColor = System.Drawing.Color.Red;
            this.lblStatusBalanza.Location = new System.Drawing.Point(116, 36);
            this.lblStatusBalanza.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusBalanza.Name = "lblStatusBalanza";
            this.lblStatusBalanza.Size = new System.Drawing.Size(145, 28);
            this.lblStatusBalanza.TabIndex = 10;
            this.lblStatusBalanza.Text = "Desconectado";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(4, 36);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(80, 28);
            this.label21.TabIndex = 9;
            this.label21.Text = "Estado:";
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold);
            this.lblDateTime.Location = new System.Drawing.Point(5, 6);
            this.lblDateTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(100, 23);
            this.lblDateTime.TabIndex = 8;
            this.lblDateTime.Text = "fecha/Hora";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnProductQuick8);
            this.panel2.Controls.Add(this.btnProductQuick10);
            this.panel2.Controls.Add(this.btnProductQuick9);
            this.panel2.Controls.Add(this.btnProductQuick7);
            this.panel2.Controls.Add(this.btnProductQuick6);
            this.panel2.Controls.Add(this.btnProductQuick5);
            this.panel2.Controls.Add(this.btnProductQuick4);
            this.panel2.Controls.Add(this.btnProductQuick3);
            this.panel2.Controls.Add(this.btnProductQuick2);
            this.panel2.Controls.Add(this.btnProductQuick1);
            this.panel2.Location = new System.Drawing.Point(0, 171);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(364, 469);
            this.panel2.TabIndex = 4;
            // 
            // btnProductQuick8
            // 
            this.btnProductQuick8.Location = new System.Drawing.Point(199, 285);
            this.btnProductQuick8.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick8.Name = "btnProductQuick8";
            this.btnProductQuick8.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick8.TabIndex = 7;
            this.btnProductQuick8.UseVisualStyleBackColor = true;
            this.btnProductQuick8.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // btnProductQuick10
            // 
            this.btnProductQuick10.Location = new System.Drawing.Point(200, 373);
            this.btnProductQuick10.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick10.Name = "btnProductQuick10";
            this.btnProductQuick10.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick10.TabIndex = 6;
            this.btnProductQuick10.UseVisualStyleBackColor = true;
            this.btnProductQuick10.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // btnProductQuick9
            // 
            this.btnProductQuick9.Location = new System.Drawing.Point(13, 373);
            this.btnProductQuick9.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick9.Name = "btnProductQuick9";
            this.btnProductQuick9.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick9.TabIndex = 5;
            this.btnProductQuick9.UseVisualStyleBackColor = true;
            this.btnProductQuick9.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // btnProductQuick7
            // 
            this.btnProductQuick7.Location = new System.Drawing.Point(13, 285);
            this.btnProductQuick7.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick7.Name = "btnProductQuick7";
            this.btnProductQuick7.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick7.TabIndex = 6;
            this.btnProductQuick7.UseVisualStyleBackColor = true;
            this.btnProductQuick7.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // btnProductQuick6
            // 
            this.btnProductQuick6.Location = new System.Drawing.Point(200, 197);
            this.btnProductQuick6.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick6.Name = "btnProductQuick6";
            this.btnProductQuick6.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick6.TabIndex = 5;
            this.btnProductQuick6.UseVisualStyleBackColor = true;
            this.btnProductQuick6.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // btnProductQuick5
            // 
            this.btnProductQuick5.Location = new System.Drawing.Point(13, 197);
            this.btnProductQuick5.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick5.Name = "btnProductQuick5";
            this.btnProductQuick5.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick5.TabIndex = 4;
            this.btnProductQuick5.UseVisualStyleBackColor = true;
            this.btnProductQuick5.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // btnProductQuick4
            // 
            this.btnProductQuick4.Location = new System.Drawing.Point(200, 109);
            this.btnProductQuick4.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick4.Name = "btnProductQuick4";
            this.btnProductQuick4.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick4.TabIndex = 3;
            this.btnProductQuick4.UseVisualStyleBackColor = true;
            this.btnProductQuick4.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // btnProductQuick3
            // 
            this.btnProductQuick3.Location = new System.Drawing.Point(13, 109);
            this.btnProductQuick3.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick3.Name = "btnProductQuick3";
            this.btnProductQuick3.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick3.TabIndex = 2;
            this.btnProductQuick3.UseVisualStyleBackColor = true;
            this.btnProductQuick3.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // btnProductQuick2
            // 
            this.btnProductQuick2.Location = new System.Drawing.Point(200, 20);
            this.btnProductQuick2.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick2.Name = "btnProductQuick2";
            this.btnProductQuick2.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick2.TabIndex = 1;
            this.btnProductQuick2.UseVisualStyleBackColor = true;
            this.btnProductQuick2.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // btnProductQuick1
            // 
            this.btnProductQuick1.Location = new System.Drawing.Point(13, 20);
            this.btnProductQuick1.Margin = new System.Windows.Forms.Padding(4);
            this.btnProductQuick1.Name = "btnProductQuick1";
            this.btnProductQuick1.Size = new System.Drawing.Size(150, 80);
            this.btnProductQuick1.TabIndex = 0;
            this.btnProductQuick1.UseVisualStyleBackColor = true;
            this.btnProductQuick1.Click += new System.EventHandler(this.btnQuickProduct_Click);
            // 
            // txtTotalSale
            // 
            this.txtTotalSale.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold);
            this.txtTotalSale.ForeColor = System.Drawing.Color.Green;
            this.txtTotalSale.Location = new System.Drawing.Point(968, 597);
            this.txtTotalSale.Margin = new System.Windows.Forms.Padding(4);
            this.txtTotalSale.Name = "txtTotalSale";
            this.txtTotalSale.Size = new System.Drawing.Size(244, 43);
            this.txtTotalSale.TabIndex = 5;
            this.txtTotalSale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelTimeout
            // 
            this.labelTimeout.AutoSize = true;
            this.labelTimeout.Location = new System.Drawing.Point(1344, 109);
            this.labelTimeout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTimeout.Name = "labelTimeout";
            this.labelTimeout.Size = new System.Drawing.Size(0, 28);
            this.labelTimeout.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1294, 52);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 28);
            this.label6.TabIndex = 0;
            this.label6.Text = "Total Venta:";
            // 
            // dgvSaleItems
            // 
            this.dgvSaleItems.AllowUserToAddRows = false;
            this.dgvSaleItems.AllowUserToDeleteRows = false;
            this.dgvSaleItems.AllowUserToOrderColumns = true;
            this.dgvSaleItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSaleItems.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvSaleItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSaleItems.Location = new System.Drawing.Point(372, 168);
            this.dgvSaleItems.Margin = new System.Windows.Forms.Padding(4);
            this.dgvSaleItems.Name = "dgvSaleItems";
            this.dgvSaleItems.RowHeadersWidth = 51;
            this.dgvSaleItems.RowTemplate.Height = 24;
            this.dgvSaleItems.Size = new System.Drawing.Size(835, 472);
            this.dgvSaleItems.TabIndex = 2;
            // 
            // panelWeightInput
            // 
            this.panelWeightInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelWeightInput.Controls.Add(this.txtWeightDisplay);
            this.panelWeightInput.Controls.Add(this.cboWeightUnit);
            this.panelWeightInput.Controls.Add(this.label2);
            this.panelWeightInput.Controls.Add(this.label1);
            this.panelWeightInput.Location = new System.Drawing.Point(422, 10);
            this.panelWeightInput.Margin = new System.Windows.Forms.Padding(4);
            this.panelWeightInput.Name = "panelWeightInput";
            this.panelWeightInput.Size = new System.Drawing.Size(785, 158);
            this.panelWeightInput.TabIndex = 0;
            // 
            // txtWeightDisplay
            // 
            this.txtWeightDisplay.Font = new System.Drawing.Font("Segoe Script", 42F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWeightDisplay.Location = new System.Drawing.Point(4, 29);
            this.txtWeightDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.txtWeightDisplay.Name = "txtWeightDisplay";
            this.txtWeightDisplay.Size = new System.Drawing.Size(559, 120);
            this.txtWeightDisplay.TabIndex = 3;
            this.txtWeightDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cboWeightUnit
            // 
            this.cboWeightUnit.Font = new System.Drawing.Font("Segoe UI", 42F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboWeightUnit.FormattingEnabled = true;
            this.cboWeightUnit.Items.AddRange(new object[] {
            "KG",
            "G",
            "LB",
            "OZ"});
            this.cboWeightUnit.Location = new System.Drawing.Point(566, 41);
            this.cboWeightUnit.Margin = new System.Windows.Forms.Padding(4);
            this.cboWeightUnit.Name = "cboWeightUnit";
            this.cboWeightUnit.Size = new System.Drawing.Size(150, 101);
            this.cboWeightUnit.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(561, -6);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 28);
            this.label2.TabIndex = 1;
            this.label2.Text = "Unidad:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(168, -1);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "Peso Actual";
            // 
            // tabPageProducts
            // 
            this.tabPageProducts.Controls.Add(this.panel3);
            this.tabPageProducts.Controls.Add(this.productPanel);
            this.tabPageProducts.Location = new System.Drawing.Point(4, 37);
            this.tabPageProducts.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageProducts.Name = "tabPageProducts";
            this.tabPageProducts.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageProducts.Size = new System.Drawing.Size(1574, 812);
            this.tabPageProducts.TabIndex = 1;
            this.tabPageProducts.Text = "Productos";
            this.tabPageProducts.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.dgvProducts);
            this.panel3.Location = new System.Drawing.Point(415, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1145, 816);
            this.panel3.TabIndex = 2;
            // 
            // dgvProducts
            // 
            this.dgvProducts.AllowUserToAddRows = false;
            this.dgvProducts.AllowUserToDeleteRows = false;
            this.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProducts.Location = new System.Drawing.Point(0, 0);
            this.dgvProducts.Margin = new System.Windows.Forms.Padding(4);
            this.dgvProducts.Name = "dgvProducts";
            this.dgvProducts.ReadOnly = true;
            this.dgvProducts.RowHeadersWidth = 51;
            this.dgvProducts.RowTemplate.Height = 24;
            this.dgvProducts.Size = new System.Drawing.Size(1145, 816);
            this.dgvProducts.TabIndex = 1;
            this.dgvProducts.SelectionChanged += new System.EventHandler(this.dgvProducts_SelectionChanged);
            // 
            // productPanel
            // 
            this.productPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.productPanel.Controls.Add(this.txtStock);
            this.productPanel.Controls.Add(this.txtProductPrice);
            this.productPanel.Controls.Add(this.txtMinimumStock);
            this.productPanel.Controls.Add(this.txtProductCode);
            this.productPanel.Controls.Add(this.btnClearProductFields);
            this.productPanel.Controls.Add(this.btnDeleteProduct);
            this.productPanel.Controls.Add(this.btnUpdateProduct);
            this.productPanel.Controls.Add(this.btnAddProduct);
            this.productPanel.Controls.Add(this.chkProductActive);
            this.productPanel.Controls.Add(this.cboProductUnit);
            this.productPanel.Controls.Add(this.label10);
            this.productPanel.Controls.Add(this.label9);
            this.productPanel.Controls.Add(this.txtProductName);
            this.productPanel.Controls.Add(this.label8);
            this.productPanel.Controls.Add(this.label7);
            this.productPanel.Location = new System.Drawing.Point(0, 0);
            this.productPanel.Margin = new System.Windows.Forms.Padding(4);
            this.productPanel.Name = "productPanel";
            this.productPanel.Size = new System.Drawing.Size(407, 812);
            this.productPanel.TabIndex = 0;
            // 
            // txtStock
            // 
            this.txtStock.Location = new System.Drawing.Point(199, 283);
            this.txtStock.Margin = new System.Windows.Forms.Padding(4);
            this.txtStock.Name = "txtStock";
            this.txtStock.Size = new System.Drawing.Size(93, 34);
            this.txtStock.TabIndex = 19;
            this.txtStock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtProductPrice
            // 
            this.txtProductPrice.Location = new System.Drawing.Point(201, 167);
            this.txtProductPrice.Margin = new System.Windows.Forms.Padding(4);
            this.txtProductPrice.Name = "txtProductPrice";
            this.txtProductPrice.Size = new System.Drawing.Size(92, 34);
            this.txtProductPrice.TabIndex = 18;
            this.txtProductPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMinimumStock
            // 
            this.txtMinimumStock.Location = new System.Drawing.Point(199, 343);
            this.txtMinimumStock.Margin = new System.Windows.Forms.Padding(4);
            this.txtMinimumStock.Name = "txtMinimumStock";
            this.txtMinimumStock.Size = new System.Drawing.Size(92, 34);
            this.txtMinimumStock.TabIndex = 19;
            this.txtMinimumStock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(201, 121);
            this.txtProductCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtProductCode.Mask = "000000";
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.PromptChar = '0';
            this.txtProductCode.Size = new System.Drawing.Size(104, 34);
            this.txtProductCode.TabIndex = 17;
            // 
            // btnClearProductFields
            // 
            this.btnClearProductFields.Location = new System.Drawing.Point(70, 621);
            this.btnClearProductFields.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearProductFields.Name = "btnClearProductFields";
            this.btnClearProductFields.Size = new System.Drawing.Size(160, 71);
            this.btnClearProductFields.TabIndex = 15;
            this.btnClearProductFields.Text = "Limpiar";
            this.btnClearProductFields.UseVisualStyleBackColor = true;
            this.btnClearProductFields.Click += new System.EventHandler(this.btnClearProductFields_Click);
            // 
            // btnDeleteProduct
            // 
            this.btnDeleteProduct.Location = new System.Drawing.Point(70, 725);
            this.btnDeleteProduct.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteProduct.Name = "btnDeleteProduct";
            this.btnDeleteProduct.Size = new System.Drawing.Size(160, 78);
            this.btnDeleteProduct.TabIndex = 11;
            this.btnDeleteProduct.Text = "Eliminar";
            this.btnDeleteProduct.UseVisualStyleBackColor = true;
            this.btnDeleteProduct.Click += new System.EventHandler(this.btnDeleteProduct_Click);
            // 
            // btnUpdateProduct
            // 
            this.btnUpdateProduct.Location = new System.Drawing.Point(70, 530);
            this.btnUpdateProduct.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdateProduct.Name = "btnUpdateProduct";
            this.btnUpdateProduct.Size = new System.Drawing.Size(160, 66);
            this.btnUpdateProduct.TabIndex = 10;
            this.btnUpdateProduct.Text = "Actualizar";
            this.btnUpdateProduct.UseVisualStyleBackColor = true;
            this.btnUpdateProduct.Click += new System.EventHandler(this.btnUpdateProduct_Click);
            // 
            // btnAddProduct
            // 
            this.btnAddProduct.Location = new System.Drawing.Point(70, 438);
            this.btnAddProduct.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddProduct.Name = "btnAddProduct";
            this.btnAddProduct.Size = new System.Drawing.Size(160, 64);
            this.btnAddProduct.TabIndex = 9;
            this.btnAddProduct.Text = "Agregar";
            this.btnAddProduct.UseVisualStyleBackColor = true;
            this.btnAddProduct.Click += new System.EventHandler(this.btnAddProduct_Click);
            // 
            // chkProductActive
            // 
            this.chkProductActive.AutoSize = true;
            this.chkProductActive.Location = new System.Drawing.Point(97, 398);
            this.chkProductActive.Margin = new System.Windows.Forms.Padding(4);
            this.chkProductActive.Name = "chkProductActive";
            this.chkProductActive.Size = new System.Drawing.Size(95, 32);
            this.chkProductActive.TabIndex = 8;
            this.chkProductActive.Text = "Activo";
            this.chkProductActive.UseVisualStyleBackColor = true;
            this.chkProductActive.CheckedChanged += new System.EventHandler(this.chkProductActive_CheckedChanged);
            // 
            // cboProductUnit
            // 
            this.cboProductUnit.FormattingEnabled = true;
            this.cboProductUnit.Items.AddRange(new object[] {
            "KG",
            "UNIDAD",
            "PAQUETE",
            "LITRO"});
            this.cboProductUnit.Location = new System.Drawing.Point(200, 221);
            this.cboProductUnit.Margin = new System.Windows.Forms.Padding(4);
            this.cboProductUnit.Name = "cboProductUnit";
            this.cboProductUnit.Size = new System.Drawing.Size(93, 36);
            this.cboProductUnit.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Location = new System.Drawing.Point(2, 221);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(190, 28);
            this.label10.TabIndex = 6;
            this.label10.Text = "Unidad de Medida:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 167);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(152, 28);
            this.label9.TabIndex = 4;
            this.label9.Text = "Precio/Unidad:";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(199, 67);
            this.txtProductName.Margin = new System.Windows.Forms.Padding(4);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(166, 34);
            this.txtProductName.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 67);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 28);
            this.label8.TabIndex = 2;
            this.label8.Text = "Nombre:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 121);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 28);
            this.label7.TabIndex = 0;
            this.label7.Text = "Código:";
            // 
            // tabPageUsers
            // 
            this.tabPageUsers.Controls.Add(this.dgvUsers);
            this.tabPageUsers.Controls.Add(this.panelUserDetails);
            this.tabPageUsers.Location = new System.Drawing.Point(4, 37);
            this.tabPageUsers.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageUsers.Name = "tabPageUsers";
            this.tabPageUsers.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageUsers.Size = new System.Drawing.Size(1574, 812);
            this.tabPageUsers.TabIndex = 2;
            this.tabPageUsers.Text = "Usuarios";
            this.tabPageUsers.UseVisualStyleBackColor = true;
            // 
            // dgvUsers
            // 
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.AllowUserToDeleteRows = false;
            this.dgvUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsers.Location = new System.Drawing.Point(642, -2);
            this.dgvUsers.Margin = new System.Windows.Forms.Padding(4);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.RowHeadersWidth = 51;
            this.dgvUsers.RowTemplate.Height = 24;
            this.dgvUsers.Size = new System.Drawing.Size(891, 742);
            this.dgvUsers.TabIndex = 1;
            this.dgvUsers.SelectionChanged += new System.EventHandler(this.dgvUsers_SelectionChanged);
            // 
            // panelUserDetails
            // 
            this.panelUserDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUserDetails.Controls.Add(this.btnClearUserFields);
            this.panelUserDetails.Controls.Add(this.btnDeleteUser);
            this.panelUserDetails.Controls.Add(this.btnUpdateUser);
            this.panelUserDetails.Controls.Add(this.btnAddUser);
            this.panelUserDetails.Controls.Add(this.dateTimePicker1);
            this.panelUserDetails.Controls.Add(this.chkUserExpires);
            this.panelUserDetails.Controls.Add(this.chkActiveUser);
            this.panelUserDetails.Controls.Add(this.chkIsAdminUser);
            this.panelUserDetails.Controls.Add(this.txtPasswordUser);
            this.panelUserDetails.Controls.Add(this.label13);
            this.panelUserDetails.Controls.Add(this.txtUsernameUser);
            this.panelUserDetails.Controls.Add(this.label12);
            this.panelUserDetails.Controls.Add(this.txtId);
            this.panelUserDetails.Controls.Add(this.label11);
            this.panelUserDetails.Location = new System.Drawing.Point(-5, 0);
            this.panelUserDetails.Margin = new System.Windows.Forms.Padding(4);
            this.panelUserDetails.Name = "panelUserDetails";
            this.panelUserDetails.Size = new System.Drawing.Size(640, 740);
            this.panelUserDetails.TabIndex = 0;
            // 
            // btnClearUserFields
            // 
            this.btnClearUserFields.Location = new System.Drawing.Point(358, 552);
            this.btnClearUserFields.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearUserFields.Name = "btnClearUserFields";
            this.btnClearUserFields.Size = new System.Drawing.Size(226, 84);
            this.btnClearUserFields.TabIndex = 13;
            this.btnClearUserFields.Text = "Limpiar";
            this.btnClearUserFields.UseVisualStyleBackColor = true;
            this.btnClearUserFields.Click += new System.EventHandler(this.btnClearUserFields_Click);
            // 
            // btnDeleteUser
            // 
            this.btnDeleteUser.Location = new System.Drawing.Point(46, 552);
            this.btnDeleteUser.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteUser.Name = "btnDeleteUser";
            this.btnDeleteUser.Size = new System.Drawing.Size(226, 84);
            this.btnDeleteUser.TabIndex = 12;
            this.btnDeleteUser.Text = "Eliminar";
            this.btnDeleteUser.UseVisualStyleBackColor = true;
            this.btnDeleteUser.Click += new System.EventHandler(this.btnDeleteUser_Click);
            // 
            // btnUpdateUser
            // 
            this.btnUpdateUser.Location = new System.Drawing.Point(358, 428);
            this.btnUpdateUser.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdateUser.Name = "btnUpdateUser";
            this.btnUpdateUser.Size = new System.Drawing.Size(226, 84);
            this.btnUpdateUser.TabIndex = 11;
            this.btnUpdateUser.Text = "Actualizar";
            this.btnUpdateUser.UseVisualStyleBackColor = true;
            this.btnUpdateUser.Click += new System.EventHandler(this.btnUpdateUser_Click);
            // 
            // btnAddUser
            // 
            this.btnAddUser.Location = new System.Drawing.Point(46, 428);
            this.btnAddUser.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(226, 84);
            this.btnAddUser.TabIndex = 10;
            this.btnAddUser.Text = "Agregar";
            this.btnAddUser.UseVisualStyleBackColor = true;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(36, 281);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(509, 34);
            this.dateTimePicker1.TabIndex = 9;
            // 
            // chkUserExpires
            // 
            this.chkUserExpires.AutoSize = true;
            this.chkUserExpires.Location = new System.Drawing.Point(406, 209);
            this.chkUserExpires.Margin = new System.Windows.Forms.Padding(4);
            this.chkUserExpires.Name = "chkUserExpires";
            this.chkUserExpires.Size = new System.Drawing.Size(93, 32);
            this.chkUserExpires.TabIndex = 8;
            this.chkUserExpires.Text = "Expira";
            this.chkUserExpires.UseVisualStyleBackColor = true;
            // 
            // chkActiveUser
            // 
            this.chkActiveUser.AutoSize = true;
            this.chkActiveUser.Checked = true;
            this.chkActiveUser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveUser.Location = new System.Drawing.Point(406, 138);
            this.chkActiveUser.Margin = new System.Windows.Forms.Padding(4);
            this.chkActiveUser.Name = "chkActiveUser";
            this.chkActiveUser.Size = new System.Drawing.Size(95, 32);
            this.chkActiveUser.TabIndex = 7;
            this.chkActiveUser.Text = "Activo";
            this.chkActiveUser.UseVisualStyleBackColor = true;
            // 
            // chkIsAdminUser
            // 
            this.chkIsAdminUser.AutoSize = true;
            this.chkIsAdminUser.Location = new System.Drawing.Point(406, 70);
            this.chkIsAdminUser.Margin = new System.Windows.Forms.Padding(4);
            this.chkIsAdminUser.Name = "chkIsAdminUser";
            this.chkIsAdminUser.Size = new System.Drawing.Size(122, 32);
            this.chkIsAdminUser.TabIndex = 6;
            this.chkIsAdminUser.Text = "Es Admin";
            this.chkIsAdminUser.UseVisualStyleBackColor = true;
            // 
            // txtPasswordUser
            // 
            this.txtPasswordUser.Location = new System.Drawing.Point(191, 206);
            this.txtPasswordUser.Margin = new System.Windows.Forms.Padding(4);
            this.txtPasswordUser.Name = "txtPasswordUser";
            this.txtPasswordUser.Size = new System.Drawing.Size(172, 34);
            this.txtPasswordUser.TabIndex = 5;
            this.txtPasswordUser.UseSystemPasswordChar = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(54, 206);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(123, 28);
            this.label13.TabIndex = 4;
            this.label13.Text = "Contraseña:";
            // 
            // txtUsernameUser
            // 
            this.txtUsernameUser.Location = new System.Drawing.Point(191, 135);
            this.txtUsernameUser.Margin = new System.Windows.Forms.Padding(4);
            this.txtUsernameUser.Name = "txtUsernameUser";
            this.txtUsernameUser.Size = new System.Drawing.Size(172, 34);
            this.txtUsernameUser.TabIndex = 3;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(54, 135);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 28);
            this.label12.TabIndex = 2;
            this.label12.Text = "Usuario:";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(191, 68);
            this.txtId.Margin = new System.Windows.Forms.Padding(4);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(172, 34);
            this.txtId.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(54, 68);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(116, 28);
            this.label11.TabIndex = 0;
            this.label11.Text = "ID Usuario:";
            // 
            // tabPageReports
            // 
            this.tabPageReports.Controls.Add(this.btnExportStockReport);
            this.tabPageReports.Controls.Add(this.cmbProductFilter);
            this.tabPageReports.Controls.Add(this.btnGenerateSalesReport);
            this.tabPageReports.Controls.Add(this.dgvReports);
            this.tabPageReports.Controls.Add(this.btnGenerateHourlyReport);
            this.tabPageReports.Controls.Add(this.dgvHourlyReports);
            this.tabPageReports.Controls.Add(this.btnGenerateStockReport);
            this.tabPageReports.Controls.Add(this.dgvStockReports);
            this.tabPageReports.Controls.Add(this.btnDeleteAllSalesData);
            this.tabPageReports.Controls.Add(this.lblStatusMessage);
            this.tabPageReports.Controls.Add(this.btnExportToExcel);
            this.tabPageReports.Controls.Add(this.btnClearFilters);
            this.tabPageReports.Controls.Add(this.btnGenerateReport);
            this.tabPageReports.Controls.Add(this.dgvSalesReports);
            this.tabPageReports.Controls.Add(this.dtpReportEndDate);
            this.tabPageReports.Controls.Add(this.dtpReportStartDate);
            this.tabPageReports.Controls.Add(this.label15);
            this.tabPageReports.Controls.Add(this.label14);
            this.tabPageReports.Location = new System.Drawing.Point(4, 37);
            this.tabPageReports.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageReports.Name = "tabPageReports";
            this.tabPageReports.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageReports.Size = new System.Drawing.Size(1574, 812);
            this.tabPageReports.TabIndex = 3;
            this.tabPageReports.Text = "Reportes";
            this.tabPageReports.UseVisualStyleBackColor = true;
            // 
            // btnExportStockReport
            // 
            this.btnExportStockReport.Location = new System.Drawing.Point(14, 758);
            this.btnExportStockReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnExportStockReport.Name = "btnExportStockReport";
            this.btnExportStockReport.Size = new System.Drawing.Size(140, 80);
            this.btnExportStockReport.TabIndex = 23;
            this.btnExportStockReport.Text = "Exportar Reportes";
            this.btnExportStockReport.UseVisualStyleBackColor = true;
            this.btnExportStockReport.Click += new System.EventHandler(this.btnExportStockReport_Click);
            // 
            // cmbProductFilter
            // 
            this.cmbProductFilter.FormattingEnabled = true;
            this.cmbProductFilter.Location = new System.Drawing.Point(200, 792);
            this.cmbProductFilter.Margin = new System.Windows.Forms.Padding(4);
            this.cmbProductFilter.Name = "cmbProductFilter";
            this.cmbProductFilter.Size = new System.Drawing.Size(150, 36);
            this.cmbProductFilter.TabIndex = 22;
            // 
            // btnGenerateSalesReport
            // 
            this.btnGenerateSalesReport.Location = new System.Drawing.Point(224, 701);
            this.btnGenerateSalesReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateSalesReport.Name = "btnGenerateSalesReport";
            this.btnGenerateSalesReport.Size = new System.Drawing.Size(251, 84);
            this.btnGenerateSalesReport.TabIndex = 21;
            this.btnGenerateSalesReport.Text = "Generar Reportes";
            this.btnGenerateSalesReport.UseVisualStyleBackColor = true;
            this.btnGenerateSalesReport.Click += new System.EventHandler(this.btnGenerateSalesReport_Click);
            // 
            // dgvReports
            // 
            this.dgvReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReports.Location = new System.Drawing.Point(548, 4);
            this.dgvReports.Margin = new System.Windows.Forms.Padding(4);
            this.dgvReports.Name = "dgvReports";
            this.dgvReports.RowHeadersWidth = 51;
            this.dgvReports.RowTemplate.Height = 24;
            this.dgvReports.Size = new System.Drawing.Size(981, 188);
            this.dgvReports.TabIndex = 20;
            // 
            // btnGenerateHourlyReport
            // 
            this.btnGenerateHourlyReport.Location = new System.Drawing.Point(308, 484);
            this.btnGenerateHourlyReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateHourlyReport.Name = "btnGenerateHourlyReport";
            this.btnGenerateHourlyReport.Size = new System.Drawing.Size(168, 106);
            this.btnGenerateHourlyReport.TabIndex = 19;
            this.btnGenerateHourlyReport.Text = "Reportes por Horas";
            this.btnGenerateHourlyReport.UseVisualStyleBackColor = true;
            this.btnGenerateHourlyReport.Click += new System.EventHandler(this.btnGenerateHourlyReport_Click);
            // 
            // dgvHourlyReports
            // 
            this.dgvHourlyReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHourlyReports.Location = new System.Drawing.Point(548, 650);
            this.dgvHourlyReports.Margin = new System.Windows.Forms.Padding(4);
            this.dgvHourlyReports.Name = "dgvHourlyReports";
            this.dgvHourlyReports.RowHeadersWidth = 51;
            this.dgvHourlyReports.RowTemplate.Height = 24;
            this.dgvHourlyReports.Size = new System.Drawing.Size(991, 188);
            this.dgvHourlyReports.TabIndex = 18;
            // 
            // btnGenerateStockReport
            // 
            this.btnGenerateStockReport.Location = new System.Drawing.Point(302, 301);
            this.btnGenerateStockReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateStockReport.Name = "btnGenerateStockReport";
            this.btnGenerateStockReport.Size = new System.Drawing.Size(160, 81);
            this.btnGenerateStockReport.TabIndex = 17;
            this.btnGenerateStockReport.Text = "Stock Reportes";
            this.btnGenerateStockReport.UseVisualStyleBackColor = true;
            this.btnGenerateStockReport.Click += new System.EventHandler(this.btnGenerateStockReport_Click);
            // 
            // dgvStockReports
            // 
            this.dgvStockReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockReports.Location = new System.Drawing.Point(548, 216);
            this.dgvStockReports.Margin = new System.Windows.Forms.Padding(4);
            this.dgvStockReports.Name = "dgvStockReports";
            this.dgvStockReports.RowHeadersWidth = 51;
            this.dgvStockReports.RowTemplate.Height = 24;
            this.dgvStockReports.Size = new System.Drawing.Size(981, 188);
            this.dgvStockReports.TabIndex = 16;
            // 
            // btnDeleteAllSalesData
            // 
            this.btnDeleteAllSalesData.Location = new System.Drawing.Point(29, 614);
            this.btnDeleteAllSalesData.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteAllSalesData.Name = "btnDeleteAllSalesData";
            this.btnDeleteAllSalesData.Size = new System.Drawing.Size(224, 80);
            this.btnDeleteAllSalesData.TabIndex = 15;
            this.btnDeleteAllSalesData.Text = "Borrar Reportes de Prueba";
            this.btnDeleteAllSalesData.UseVisualStyleBackColor = true;
            this.btnDeleteAllSalesData.Click += new System.EventHandler(this.btnDeleteAllSalesData_Click);
            // 
            // lblStatusMessage
            // 
            this.lblStatusMessage.AutoSize = true;
            this.lblStatusMessage.Location = new System.Drawing.Point(36, 719);
            this.lblStatusMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusMessage.Name = "lblStatusMessage";
            this.lblStatusMessage.Size = new System.Drawing.Size(75, 28);
            this.lblStatusMessage.TabIndex = 14;
            this.lblStatusMessage.Text = "Estado";
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Location = new System.Drawing.Point(14, 378);
            this.btnExportToExcel.Margin = new System.Windows.Forms.Padding(4);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(239, 86);
            this.btnExportToExcel.TabIndex = 13;
            this.btnExportToExcel.Text = "Exportar a Excel";
            this.btnExportToExcel.UseVisualStyleBackColor = true;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // btnClearFilters
            // 
            this.btnClearFilters.Location = new System.Drawing.Point(14, 492);
            this.btnClearFilters.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearFilters.Name = "btnClearFilters";
            this.btnClearFilters.Size = new System.Drawing.Size(239, 89);
            this.btnClearFilters.TabIndex = 12;
            this.btnClearFilters.Text = "Limpiar Filtros";
            this.btnClearFilters.UseVisualStyleBackColor = true;
            this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
            // 
            // btnGenerateReport
            // 
            this.btnGenerateReport.Location = new System.Drawing.Point(224, 118);
            this.btnGenerateReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(239, 86);
            this.btnGenerateReport.TabIndex = 5;
            this.btnGenerateReport.Text = "Generar Reporte de Ventas";
            this.btnGenerateReport.UseVisualStyleBackColor = true;
            this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);
            // 
            // dgvSalesReports
            // 
            this.dgvSalesReports.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSalesReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSalesReports.Location = new System.Drawing.Point(548, 436);
            this.dgvSalesReports.Margin = new System.Windows.Forms.Padding(4);
            this.dgvSalesReports.Name = "dgvSalesReports";
            this.dgvSalesReports.ReadOnly = true;
            this.dgvSalesReports.RowHeadersWidth = 51;
            this.dgvSalesReports.RowTemplate.Height = 24;
            this.dgvSalesReports.Size = new System.Drawing.Size(981, 188);
            this.dgvSalesReports.TabIndex = 4;
            // 
            // dtpReportEndDate
            // 
            this.dtpReportEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpReportEndDate.Location = new System.Drawing.Point(186, 68);
            this.dtpReportEndDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpReportEndDate.Name = "dtpReportEndDate";
            this.dtpReportEndDate.Size = new System.Drawing.Size(208, 34);
            this.dtpReportEndDate.TabIndex = 3;
            // 
            // dtpReportStartDate
            // 
            this.dtpReportStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpReportStartDate.Location = new System.Drawing.Point(186, 8);
            this.dtpReportStartDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpReportStartDate.Name = "dtpReportStartDate";
            this.dtpReportStartDate.Size = new System.Drawing.Size(208, 34);
            this.dtpReportStartDate.TabIndex = 2;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 68);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(122, 28);
            this.label15.TabIndex = 1;
            this.label15.Text = "Fecha Final:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 8);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(129, 28);
            this.label14.TabIndex = 0;
            this.label14.Text = "Fecha Inicio:";
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.panel1);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 37);
            this.tabPageSettings.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageSettings.Size = new System.Drawing.Size(1574, 812);
            this.tabPageSettings.TabIndex = 4;
            this.tabPageSettings.Text = "Configuración";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtCurrencySymbol);
            this.panel1.Controls.Add(this.btnLogout);
            this.panel1.Controls.Add(this.btnGuardar);
            this.panel1.Controls.Add(this.numericUpDownDecimalesBalanza);
            this.panel1.Controls.Add(this.chkDemoMode);
            this.panel1.Controls.Add(this.btnConnectBalanza);
            this.panel1.Controls.Add(this.btnDisconnectBalanza);
            this.panel1.Controls.Add(this.cboStopBits);
            this.panel1.Controls.Add(this.cboParity);
            this.panel1.Controls.Add(this.cboBaudRate);
            this.panel1.Controls.Add(this.cboDataBits);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.btnRefreshPorts);
            this.panel1.Controls.Add(this.cmbPorts);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.numericUpDownDecimalesPrecio);
            this.panel1.Controls.Add(this.labelDecimalesPrecio);
            this.panel1.Location = new System.Drawing.Point(8, 8);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1548, 832);
            this.panel1.TabIndex = 0;
            // 
            // txtCurrencySymbol
            // 
            this.txtCurrencySymbol.FormattingEnabled = true;
            this.txtCurrencySymbol.Items.AddRange(new object[] {
            "S/",
            "$",
            "€",
            "£"});
            this.txtCurrencySymbol.Location = new System.Drawing.Point(508, 217);
            this.txtCurrencySymbol.Margin = new System.Windows.Forms.Padding(4);
            this.txtCurrencySymbol.Name = "txtCurrencySymbol";
            this.txtCurrencySymbol.Size = new System.Drawing.Size(84, 36);
            this.txtCurrencySymbol.TabIndex = 18;
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(825, 189);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(4);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(188, 62);
            this.btnLogout.TabIndex = 17;
            this.btnLogout.Text = "Cerrar Sesión";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(400, 280);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(4);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(188, 62);
            this.btnGuardar.TabIndex = 16;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // numericUpDownDecimalesBalanza
            // 
            this.numericUpDownDecimalesBalanza.Location = new System.Drawing.Point(508, 137);
            this.numericUpDownDecimalesBalanza.Margin = new System.Windows.Forms.Padding(4);
            this.numericUpDownDecimalesBalanza.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDownDecimalesBalanza.Name = "numericUpDownDecimalesBalanza";
            this.numericUpDownDecimalesBalanza.Size = new System.Drawing.Size(84, 34);
            this.numericUpDownDecimalesBalanza.TabIndex = 13;
            this.numericUpDownDecimalesBalanza.ValueChanged += new System.EventHandler(this.numericUpDownDecimalesBalanza_ValueChanged);
            // 
            // chkDemoMode
            // 
            this.chkDemoMode.AutoSize = true;
            this.chkDemoMode.Location = new System.Drawing.Point(701, 22);
            this.chkDemoMode.Margin = new System.Windows.Forms.Padding(4);
            this.chkDemoMode.Name = "chkDemoMode";
            this.chkDemoMode.Size = new System.Drawing.Size(151, 32);
            this.chkDemoMode.TabIndex = 12;
            this.chkDemoMode.Text = "Modo Demo";
            this.chkDemoMode.UseVisualStyleBackColor = true;
            // 
            // btnConnectBalanza
            // 
            this.btnConnectBalanza.Location = new System.Drawing.Point(150, 291);
            this.btnConnectBalanza.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnectBalanza.Name = "btnConnectBalanza";
            this.btnConnectBalanza.Size = new System.Drawing.Size(188, 62);
            this.btnConnectBalanza.TabIndex = 11;
            this.btnConnectBalanza.Text = "Conectar Balanza";
            this.btnConnectBalanza.UseVisualStyleBackColor = true;
            this.btnConnectBalanza.Click += new System.EventHandler(this.btnConnectBalanza_Click);
            // 
            // btnDisconnectBalanza
            // 
            this.btnDisconnectBalanza.Location = new System.Drawing.Point(152, 382);
            this.btnDisconnectBalanza.Margin = new System.Windows.Forms.Padding(4);
            this.btnDisconnectBalanza.Name = "btnDisconnectBalanza";
            this.btnDisconnectBalanza.Size = new System.Drawing.Size(188, 62);
            this.btnDisconnectBalanza.TabIndex = 10;
            this.btnDisconnectBalanza.Text = "Desconectar Balanza";
            this.btnDisconnectBalanza.UseVisualStyleBackColor = true;
            this.btnDisconnectBalanza.Click += new System.EventHandler(this.btnDisconnectBalanza_Click);
            // 
            // cboStopBits
            // 
            this.cboStopBits.FormattingEnabled = true;
            this.cboStopBits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this.cboStopBits.Location = new System.Drawing.Point(155, 183);
            this.cboStopBits.Margin = new System.Windows.Forms.Padding(4);
            this.cboStopBits.Name = "cboStopBits";
            this.cboStopBits.Size = new System.Drawing.Size(124, 36);
            this.cboStopBits.TabIndex = 9;
            // 
            // cboParity
            // 
            this.cboParity.FormattingEnabled = true;
            this.cboParity.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even"});
            this.cboParity.Location = new System.Drawing.Point(155, 139);
            this.cboParity.Margin = new System.Windows.Forms.Padding(4);
            this.cboParity.Name = "cboParity";
            this.cboParity.Size = new System.Drawing.Size(124, 36);
            this.cboParity.TabIndex = 8;
            // 
            // cboBaudRate
            // 
            this.cboBaudRate.FormattingEnabled = true;
            this.cboBaudRate.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cboBaudRate.Location = new System.Drawing.Point(155, 88);
            this.cboBaudRate.Margin = new System.Windows.Forms.Padding(4);
            this.cboBaudRate.Name = "cboBaudRate";
            this.cboBaudRate.Size = new System.Drawing.Size(124, 36);
            this.cboBaudRate.TabIndex = 7;
            // 
            // cboDataBits
            // 
            this.cboDataBits.FormattingEnabled = true;
            this.cboDataBits.Items.AddRange(new object[] {
            "8",
            "7"});
            this.cboDataBits.Location = new System.Drawing.Point(155, 230);
            this.cboDataBits.Margin = new System.Windows.Forms.Padding(4);
            this.cboDataBits.Name = "cboDataBits";
            this.cboDataBits.Size = new System.Drawing.Size(124, 36);
            this.cboDataBits.TabIndex = 6;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Location = new System.Drawing.Point(43, 189);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(102, 28);
            this.label20.TabIndex = 5;
            this.label20.Text = "Stop Bits:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(43, 142);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(89, 28);
            this.label19.TabIndex = 4;
            this.label19.Text = "Paridad:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(33, 91);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(114, 28);
            this.label18.TabIndex = 3;
            this.label18.Text = "Baud Rate:";
            // 
            // btnRefreshPorts
            // 
            this.btnRefreshPorts.Location = new System.Drawing.Point(301, 42);
            this.btnRefreshPorts.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefreshPorts.Name = "btnRefreshPorts";
            this.btnRefreshPorts.Size = new System.Drawing.Size(309, 48);
            this.btnRefreshPorts.TabIndex = 2;
            this.btnRefreshPorts.Text = "Actualizar Puertos";
            this.btnRefreshPorts.UseVisualStyleBackColor = true;
            this.btnRefreshPorts.Click += new System.EventHandler(this.btnRefreshPorts_Click);
            // 
            // cmbPorts
            // 
            this.cmbPorts.FormattingEnabled = true;
            this.cmbPorts.Location = new System.Drawing.Point(155, 42);
            this.cmbPorts.Margin = new System.Windows.Forms.Padding(4);
            this.cmbPorts.Name = "cmbPorts";
            this.cmbPorts.Size = new System.Drawing.Size(124, 36);
            this.cmbPorts.TabIndex = 1;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(63, 45);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(81, 28);
            this.label17.TabIndex = 0;
            this.label17.Text = "Puerto:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(395, 215);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 28);
            this.label16.TabIndex = 19;
            this.label16.Text = "Moneda:";
            // 
            // numericUpDownDecimalesPrecio
            // 
            this.numericUpDownDecimalesPrecio.Location = new System.Drawing.Point(508, 177);
            this.numericUpDownDecimalesPrecio.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownDecimalesPrecio.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDownDecimalesPrecio.Name = "numericUpDownDecimalesPrecio";
            this.numericUpDownDecimalesPrecio.Size = new System.Drawing.Size(84, 34);
            this.numericUpDownDecimalesPrecio.TabIndex = 14;
            // 
            // labelDecimalesPrecio
            // 
            this.labelDecimalesPrecio.AutoSize = true;
            this.labelDecimalesPrecio.Location = new System.Drawing.Point(325, 183);
            this.labelDecimalesPrecio.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelDecimalesPrecio.Name = "labelDecimalesPrecio";
            this.labelDecimalesPrecio.Size = new System.Drawing.Size(179, 28);
            this.labelDecimalesPrecio.TabIndex = 20;
            this.labelDecimalesPrecio.Text = "Decimales Precio:";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1582, 853);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainScreen";
            this.Text = " ";
            this.Load += new System.EventHandler(this.MainScreen_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageSales.ResumeLayout(false);
            this.tabPageSales.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.productPanelVentas.ResumeLayout(false);
            this.productPanelVentas.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaleItems)).EndInit();
            this.panelWeightInput.ResumeLayout(false);
            this.panelWeightInput.PerformLayout();
            this.tabPageProducts.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            this.productPanel.ResumeLayout(false);
            this.productPanel.PerformLayout();
            this.tabPageUsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.panelUserDetails.ResumeLayout(false);
            this.panelUserDetails.PerformLayout();
            this.tabPageReports.ResumeLayout(false);
            this.tabPageReports.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHourlyReports)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockReports)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalesReports)).EndInit();
            this.tabPageSettings.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDecimalesBalanza)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDecimalesPrecio)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSales;
        private System.Windows.Forms.Panel panelWeightInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboWeightUnit;
        private System.Windows.Forms.TextBox txtWeightDisplay;
        private System.Windows.Forms.Button btnSetTare;
        private System.Windows.Forms.Button btnClearTare;
        private System.Windows.Forms.DataGridView dgvSaleItems;
        private System.Windows.Forms.Label label6;
       
        private System.Windows.Forms.Button btnFinalizeSale;
        private System.Windows.Forms.Button btnClearAllItems;
        private System.Windows.Forms.Button btnNewSale;
        private System.Windows.Forms.Button btnDevolucion;
        private System.Windows.Forms.Button btnRemoveSaleItem;
        private System.Windows.Forms.Label labelTimeout;
        private System.Windows.Forms.TextBox txtTotalSale;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnProductQuick1;
        private System.Windows.Forms.Button btnProductQuick2;
        private System.Windows.Forms.Button btnProductQuick3;
        private System.Windows.Forms.Button btnProductQuick4;
        private System.Windows.Forms.Button btnProductQuick5;
        private System.Windows.Forms.Button btnProductQuick6;
        private System.Windows.Forms.Button btnProductQuick7;
        private System.Windows.Forms.Button btnProductQuick8;
        private System.Windows.Forms.Button btnProductQuick9;
        private System.Windows.Forms.Button btnProductQuick10;
        private System.Windows.Forms.Button btnTestQuickProduct;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lblStatusBalanza;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox txtDiscount;
        private System.Windows.Forms.ComboBox cboPaymentMethod;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TabPage tabPageProducts;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.TabPage tabPageUsers;
        private System.Windows.Forms.Panel panelUserDetails;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtUsernameUser;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtPasswordUser;
        private System.Windows.Forms.CheckBox chkIsAdminUser;
        private System.Windows.Forms.CheckBox chkActiveUser;
        private System.Windows.Forms.CheckBox chkUserExpires;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.Button btnUpdateUser;
        private System.Windows.Forms.Button btnDeleteUser;
        private System.Windows.Forms.Button btnClearUserFields;
        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.TabPage tabPageReports;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DateTimePicker dtpReportStartDate;
        private System.Windows.Forms.DateTimePicker dtpReportEndDate;
        private System.Windows.Forms.DataGridView dgvSalesReports;
        private System.Windows.Forms.Button btnGenerateReport;
        private System.Windows.Forms.Button btnClearFilters;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.Label lblStatusMessage;
        private System.Windows.Forms.Button btnDeleteAllSalesData;
        private System.Windows.Forms.DataGridView dgvStockReports;
        private System.Windows.Forms.Button btnGenerateStockReport;
        private System.Windows.Forms.DataGridView dgvHourlyReports;
        private System.Windows.Forms.Button btnGenerateHourlyReport;
        private System.Windows.Forms.DataGridView dgvReports;
        private System.Windows.Forms.Button btnGenerateSalesReport;
        private System.Windows.Forms.ComboBox cmbProductFilter;
        private System.Windows.Forms.Button btnExportStockReport;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox cmbPorts;
        private System.Windows.Forms.Button btnRefreshPorts;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cboDataBits;
        private System.Windows.Forms.ComboBox cboBaudRate;
        private System.Windows.Forms.ComboBox cboParity;
        private System.Windows.Forms.ComboBox cboStopBits;
        private System.Windows.Forms.Button btnDisconnectBalanza;
        private System.Windows.Forms.Button btnConnectBalanza;
        private System.Windows.Forms.CheckBox chkDemoMode;
        private System.Windows.Forms.NumericUpDown numericUpDownDecimalesBalanza;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox txtCurrencySymbol;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NumericUpDown numericUpDownDecimalesPrecio;
        private System.Windows.Forms.Label labelDecimalesPrecio;
        private Panel productPanel;
        private MaskedTextBox txtStock;
        private MaskedTextBox txtProductPrice;
        private MaskedTextBox txtMinimumStock;
        private MaskedTextBox txtProductCode;
        private Button btnClearProductFields;
        private Button btnDeleteProduct;
        private Button btnUpdateProduct;
        private Button btnAddProduct;
        private CheckBox chkProductActive;
        private ComboBox cboProductUnit;
        private Label label10;
        private Label label9;
        private TextBox txtProductName;
        private Label label8;
        private Label label7;
        private Panel productPanelVentas;
        private Label label23;
        private TextBox txtRemainingStock;
        private MaskedTextBox txtSearchProductCode;
        private Label lblSaleProductUnit;
        private TextBox txt1Quantity;
        private Button btnAddSaleItem;
        private TextBox txtSaleProductPrice;
        private Label label5;
        private Label txtSaleProductName;
        private Label label4;
        private Label label3;
    }
}