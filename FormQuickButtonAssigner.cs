// ARCHIVO: FormQuickButtonAssigner.cs

using System;
using System.Windows.Forms;
using BalanzaPOSNuevo.Models; // Podrías necesitar esto si quieres mostrar el nombre del producto
using BalanzaPOSNuevo.Services; // Podrías necesitar esto para obtener el nombre del producto

namespace BalanzaPOSNuevo // Asegúrate de que el namespace es el correcto
{
    public partial class FormQuickButtonAssigner : Form
    {
        public int SelectedButtonNumber { get; private set; }
        private int _productId;
        private ProductService _productService; // Para obtener el nombre del producto

        public FormQuickButtonAssigner(int productId)
        {
            InitializeComponent(); // Este método ahora existirá
            _productId = productId;
            _productService = new ProductService(); // Instancia el servicio de productos

            this.Text = "Asignar Producto Rápido";

            // Cargar y mostrar el nombre del producto
            Product productToAssign = _productService.GetProductById(_productId);
            if (productToAssign != null)
            {
                lblAssignedProductInfo.Text = $"Asignando: {productToAssign.Name} (ID: {productToAssign.Id})";
            }
            else
            {
                lblAssignedProductInfo.Text = $"Asignando Producto ID: {_productId} (No encontrado)";
            }

            AttachButtonHandlers();
        }

        private void AttachButtonHandlers()
        {
            // ⭐ CRÍTICO: Asegúrate de que este nombre de control (flowLayoutPanelQuickButtons) coincide
            foreach (Control c in flowLayoutPanelQuickButtons.Controls)
            {
                if (c is Button button && button.Tag != null)
                {
                    button.Click += QuickButton_Click;
                }
            }
        }

        private void QuickButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null && int.TryParse(clickedButton.Tag?.ToString(), out int buttonNumber))
            {
                SelectedButtonNumber = buttonNumber;

                DialogResult result = MessageBox.Show(
                    $"¿Desea asignar el producto al botón rápido {buttonNumber}?",
                    "Confirmar Asignación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}