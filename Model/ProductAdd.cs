using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagementApp.Model
{
    public partial class ProductAdd : SampleAdd
    {
        public ProductAdd()
        {
            InitializeComponent();
        }

        public int id = 0;
        public int cID = 0;

        string filePath;
        Byte[] imageByteArray;

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images(.jpg, .png)|* .png; *.jpg";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                txtImage.Image = new Bitmap(filePath);
            }
        }

        private void ProductAdd_Load(object sender, EventArgs e)
        {
            //For cb fill
            string query = "Select catID 'id' , catName 'name' from category";

            MainClass.CBFill(query, cbCategory);

            if (cID > 0) //For Update
            {
                cbCategory.SelectedValue = cID;
            }

            if(id > 0)
            {
                ForUpdateLoadData();
            }
        }

        public override void btnSave_Click(object sender, EventArgs e)
        {
            string query = "";

            if (id == 0) //Insert
            {
                query = "Insert into products Values(@Name, @Price, @Category, @Image)";
            }
            else //Update
            {
                query = "Update products Set pName = @Name , pPrice = @Price , CategoryID = @Category , pImage = @Image where pID = @ID";
            }

            //For image
            Image temp = new Bitmap(txtImage.Image);
            MemoryStream ms = new MemoryStream();
            temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            imageByteArray= ms.ToArray();

            Hashtable ht = new Hashtable();
            ht.Add("@ID", id);
            ht.Add("@Name", txtName.Text);
            ht.Add("@Price", txtPrice.Text);
            ht.Add("@Category", Convert.ToInt32(cbCategory.SelectedValue));
            ht.Add("@Image", imageByteArray);

            if (MainClass.SQL(query, ht) > 0)
            {
                guna2MessageDialog1.Show("Saved successfully!");
                id = 0;
                cID= 0;
                txtName.Text = "";
                txtPrice.Text = "";
                cbCategory.SelectedIndex = 0;
                cbCategory.SelectedIndex = -1;
                txtImage.Image = RestaurantManagementApp.Properties.Resources.product_pic;
                txtName.Focus();
            }
        }

        private void ForUpdateLoadData()
        {
            string query = @"Select * from products where pID = " + id + "";    
            SqlCommand cmd = new SqlCommand(query, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                txtName.Text = dt.Rows[0]["pName"].ToString();
                txtPrice.Text = dt.Rows[0]["pPrice"].ToString();

                Byte[] imageArray = (byte[])(dt.Rows[0]["pImage"]);
                byte[] imageByteArray = imageArray;
                txtImage.Image = Image.FromStream(new MemoryStream(imageArray));
            }
        }
    }
}
