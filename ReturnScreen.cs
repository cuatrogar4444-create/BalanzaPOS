using System;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BalanzaPOSNuevo.Helpers;

namespace BalanzaPOSNuevo
{
    public partial class ReturnScreen : Form
    {
        private int currentLoggedInId;

        public ReturnScreen(int loggedInId)
        {
            InitializeComponent();
            currentLoggedInId = loggedInId;
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Name FROM Products WHERE Active = 1", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cboProduct.Items.Add(reader["Name"].ToString());
                    }
                }
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (cboProduct.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("Por favor, seleccione un producto y especifique la cantidad.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtQuantity.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número positivo.", "Entrada Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string productName = cboProduct.SelectedItem.ToString();
            long productId = DatabaseHelper.GetProductIdFromName(productName);

            if (productId == -1)
            {
                MessageBox.Show("Producto no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string username = Session.Username ?? "default_user";

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        DatabaseHelper.UpdateStockWithHistory(productId, quantity, "Devolución", username, conn, transaction);
                        using (var cmd = new SQLiteCommand("INSERT INTO Returns (ProductId, Quantity, ReturnDate, UserId) VALUES (@ProductId, @Quantity, @ReturnDate, @UserId)", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ProductId", productId);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@ReturnDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@UserId", currentLoggedInId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        Logger.Log("Devolución procesada", $"Producto ID: {productId}, Cantidad: {quantity}");
                        MessageBox.Show("Devolución procesada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error al procesar la devolución: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Logger.Log("Error al procesar devolución", ex.Message);
                    }
                }
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }
    }
}