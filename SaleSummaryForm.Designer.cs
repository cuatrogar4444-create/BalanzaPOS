

namespace BalanzaPOSNuevo
{
    partial class SaleSummaryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvSummaryItems;
        private System.Windows.Forms.Label lblSummaryTotal;
        private System.Windows.Forms.Label lblDiscount;
        private System.Windows.Forms.Label lblPaymentMethod;
        private System.Windows.Forms.Button btnAccept;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvSummaryItems = new System.Windows.Forms.DataGridView();
            this.lblSummaryTotal = new System.Windows.Forms.Label();
            this.lblDiscount = new System.Windows.Forms.Label();
            this.lblPaymentMethod = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSummaryItems)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSummaryItems
            // 
            this.dgvSummaryItems.ColumnHeadersHeight = 29;
            this.dgvSummaryItems.Location = new System.Drawing.Point(15, -5);
            this.dgvSummaryItems.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.dgvSummaryItems.Name = "dgvSummaryItems";
            this.dgvSummaryItems.RowHeadersWidth = 51;
            this.dgvSummaryItems.Size = new System.Drawing.Size(744, 646);
            this.dgvSummaryItems.TabIndex = 0;
            // 
            // lblSummaryTotal
            // 
            this.lblSummaryTotal.AutoSize = true;
            this.lblSummaryTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSummaryTotal.Location = new System.Drawing.Point(582, 648);
            this.lblSummaryTotal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblSummaryTotal.Name = "lblSummaryTotal";
            this.lblSummaryTotal.Size = new System.Drawing.Size(206, 30);
            this.lblSummaryTotal.TabIndex = 1;
            this.lblSummaryTotal.Text = "                                ";
            // 
            // lblDiscount
            // 
            this.lblDiscount.AutoSize = true;
            this.lblDiscount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDiscount.Location = new System.Drawing.Point(20, 674);
            this.lblDiscount.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDiscount.Name = "lblDiscount";
            this.lblDiscount.Size = new System.Drawing.Size(200, 30);
            this.lblDiscount.TabIndex = 2;
            this.lblDiscount.Text = "                               ";
            // 
            // lblPaymentMethod
            // 
            this.lblPaymentMethod.AutoSize = true;
            this.lblPaymentMethod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPaymentMethod.Location = new System.Drawing.Point(20, 723);
            this.lblPaymentMethod.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPaymentMethod.Name = "lblPaymentMethod";
            this.lblPaymentMethod.Size = new System.Drawing.Size(200, 30);
            this.lblPaymentMethod.TabIndex = 3;
            this.lblPaymentMethod.Text = "                               ";
            this.lblPaymentMethod.Click += new System.EventHandler(this.lblPaymentMethod_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(258, 688);
            this.btnAccept.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(338, 65);
            this.btnAccept.TabIndex = 4;
            this.btnAccept.Text = "SUBTOTAL O VOLVER A VENTAS";
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // SaleSummaryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 765);
            this.Controls.Add(this.dgvSummaryItems);
            this.Controls.Add(this.lblSummaryTotal);
            this.Controls.Add(this.lblDiscount);
            this.Controls.Add(this.lblPaymentMethod);
            this.Controls.Add(this.btnAccept);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "SaleSummaryForm";
            this.Text = "Resumen de Venta";
            this.Load += new System.EventHandler(this.SaleSummaryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSummaryItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}