using RestaurantManagementApp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagementApp
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        //For accessing main
        static Main _obj;
        public static Main Instance
        {
            get { if(_obj == null) { _obj = new Main(); } return _obj; }
        }

        //Method to add controls

        public void AddControls(Form f)
        {
            CenterPanel.Controls.Clear();
            f.Dock= DockStyle.Fill;
            f.TopLevel = false;
            CenterPanel.Controls.Add(f);
            f.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            lblUser.Text = MainClass.USER;
            _obj = this;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            AddControls(new Home());
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            AddControls(new CategoryView());
        }

        private void btnTables_Click(object sender, EventArgs e)
        {
            AddControls(new TableView());
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            AddControls(new StaffView());
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            AddControls(new ProductView());
        }

        private void btnPOS_Click(object sender, EventArgs e)
        {
            Model.POS pos = new Model.POS();
            pos.Show();
        }

        private void btnKitchen_Click(object sender, EventArgs e)
        {
            AddControls(new KitchenView());
        }
    }
}
