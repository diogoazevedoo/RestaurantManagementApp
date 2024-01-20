using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagementApp.Model
{
    public partial class CheckOut : SampleAdd
    {
        public CheckOut()
        {
            InitializeComponent();
        }

        public double amount;
        public int MainID = 0;

        private void txtPaymentReceived_TextChanged(object sender, EventArgs e)
        {
            double amount = 0;
            double received = 0;
            double change = 0;

            double.TryParse(txtBillAmount.Text, out amount);
            double.TryParse(txtPaymentReceived.Text, out received);

            change = Math.Abs(amount - received); //convert negative values to positive

            txtChange.Text = change.ToString();

        }

        public override void btnSave_Click(object sender, EventArgs e)
        {
            string query = @"Update tblMain set total = @total, received = @received, change = @change, status = 'Paid' where MainID = @ID";

            Hashtable ht = new Hashtable();
            ht.Add(@"ID", MainID);
            ht.Add(@"total", txtBillAmount.Text);
            ht.Add(@"received", txtPaymentReceived.Text);
            ht.Add(@"change", txtChange.Text);

            if(MainClass.SQL(query, ht) > 0)
            {
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Show("Saved Successfully!");
                this.Close();
            }
        }

        private void CheckOut_Load(object sender, EventArgs e)
        {
            txtBillAmount.Text = amount.ToString();
        }
    }
}
