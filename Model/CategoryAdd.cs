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
    public partial class CategoryAdd : SampleAdd
    {
        public CategoryAdd()
        {
            InitializeComponent();
        }

        public int id = 0;

        public override void btnSave_Click(object sender, EventArgs e)
        {
            string query = "";

            if(id == 0) //Insert
            {
                query = "Insert into category Values(@Name)";
            }
            else //Update
            {
                query = "Update category Set catName = @Name where catID = @ID";
            }

            Hashtable ht = new Hashtable();
            ht.Add("@ID", id);
            ht.Add("@Name", txtName.Text);

            if(MainClass.SQL(query, ht) > 0)
            {
                guna2MessageDialog1.Show("Saved successfully!");
                id = 0;
                txtName.Text = "";
                txtName.Focus();
            }
        }
    }
}
