using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace RestaurantManagementApp.Model
{
    public partial class AddCostumer : Form
    {
        public AddCostumer()
        {
            InitializeComponent();
        }

        public string orderType = "";
        public int driverID = 0;
        public string cusName = "";
        public int MainID = 0;

        private void AddCostumer_Load(object sender, EventArgs e)
        {
            if(orderType == "Take-Away")
            {
                lblDriver.Visible = false;
                cbDriver.Visible = false;
            }

            string query = "Select staffID 'id', sName 'name' from staff where sRole like 'Driver'";
            MainClass.CBFill(query, cbDriver);

            if(MainID > 0)
            {
                cbDriver.SelectedValue = driverID;
            }
        }

        private void cbDriver_SelectedIndexChanged(object sender, EventArgs e)
        {
            driverID = Convert.ToInt32(cbDriver.SelectedValue);
        }
    }
}
