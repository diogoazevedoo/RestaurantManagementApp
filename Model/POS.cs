using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RestaurantManagementApp.Model
{
    public partial class POS : Form
    {
        public POS()
        {
            InitializeComponent();
        }

        public int MainID = 0;
        public string OrderType = "";
        public int driverID = 0;
        public string customerName = "";
        public string customerPhone = "";

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void POS_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.BorderStyle= BorderStyle.FixedSingle;
            AddCategory();

            ProductPanel.Controls.Clear();
            LoadProducts();
        }

        private void AddCategory()
        {
            string query = "Select * from category";
            SqlCommand cmd = new SqlCommand(query, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            CategoryPanel.Controls.Clear();

            Guna.UI2.WinForms.Guna2Button btn2 = new Guna.UI2.WinForms.Guna2Button();
            btn2.FillColor = Color.FromArgb(50, 55, 89);
            btn2.Size = new Size(161, 45);
            btn2.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            btn2.Text = "All Categories";
            btn2.CheckedState.FillColor = Color.FromArgb(241, 85, 126);
            btn2.Click += new EventHandler(btn_Click);
            CategoryPanel.Controls.Add(btn2);

            if(dt.Rows.Count > 0 )
            {
                foreach (DataRow row in dt.Rows)
                {
                    Guna.UI2.WinForms.Guna2Button btn = new Guna.UI2.WinForms.Guna2Button();
                    btn.FillColor = Color.FromArgb(50, 55, 89);
                    btn.Size = new System.Drawing.Size(161, 45);
                    btn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
                    btn.Text = row["catName"].ToString();
                    btn.CheckedState.FillColor = Color.FromArgb(241, 85, 126);

                    //Event for click
                    btn.Click += new EventHandler(btn_Click);

                    CategoryPanel.Controls.Add(btn);
                }             
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button btn = (Guna.UI2.WinForms.Guna2Button)sender;

            if(btn.Text == "All Categories")
            {
                txtSearch.Text = "1";
                txtSearch.Text = "";
                return;
            }

            foreach (var item in ProductPanel.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = RemoveDiacritics(pro.pCategory).ToLower().Contains(RemoveDiacritics(btn.Text.Trim().ToLower()));
            }
        }

        private void AddItems(string id, string proID, string name, string category, string price, Image image)
        {
            var ucProduct = new ucProduct()
            {
                id = Convert.ToInt32(proID),
                pName = name,
                pPrice = price,
                pCategory = category,              
                pImage = image
            };

            ProductPanel.Controls.Add(ucProduct);

            ucProduct.onSelect += (ss, ee) =>
            {
                var wdg = (ucProduct)ss;

                foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                {
                    //check if product already added and add one to quantity and update price
                    if (Convert.ToInt32(item.Cells["proID"].Value) == wdg.id)
                    {
                        item.Cells["dgvQty"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) + 1;
                        item.Cells["dgvAmount"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) *
                                                        double.Parse(item.Cells["dgvPrice"].Value.ToString());
                        return;
                    }                   
                }

                //this line add new product
                guna2DataGridView1.Rows.Add(new object[] {0, 0, wdg.id, wdg.pName, 1, wdg.pPrice, wdg.pPrice });
                GetTotal();
            };
        }

        //Getting product from database
        private void LoadProducts()
        {
            string query = "Select * from products inner join category on catID = CategoryID";
            SqlCommand cmd = new SqlCommand(query, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach(DataRow row in dt.Rows)
            {
                Byte[] imageArray = (byte[])row["pImage"];
                byte[] imageByteArray = imageArray;

                AddItems("0", row["pID"].ToString(), row["pName"].ToString(), row["catName"].ToString(), row["pPrice"].ToString(), Image.FromStream(new MemoryStream(imageArray)));
            }
        }

        //Ignore diacritics on search
        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            foreach (var item in ProductPanel.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = RemoveDiacritics(pro.pName).ToLower().Contains(RemoveDiacritics(txtSearch.Text.Trim().ToLower()));
            }
        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //for serial no
            int count = 0;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        private void GetTotal()
        {
            double total = 0;
            lblTotal.Text = "";

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                total += double.Parse(row.Cells["dgvAmount"].Value.ToString());
            }

            lblTotal.Text = total.ToString("N2");
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            guna2DataGridView1.Rows.Clear();
            MainID = 0;
            lblTotal.Text = "0.00";
        }

        private void btnDelivery_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Delivery";

            AddCostumer addCostumer = new AddCostumer();
            addCostumer.MainID = MainID;
            addCostumer.orderType = OrderType;
            MainClass.BlurBackground(addCostumer);

            if (addCostumer.txtName.Text != "")
            {
                driverID = addCostumer.driverID;
                lblDriver.Text = "Customer Name: " + addCostumer.txtName.Text + " Phone: " + addCostumer.txtPhone.Text + " Driver: " + addCostumer.cbDriver.Text;
                lblDriver.Visible = true;
                customerName = addCostumer.txtName.Text;
                customerPhone = addCostumer.txtPhone.Text;
            }
        }

        private void btnTakeAway_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Take-Away";

            AddCostumer addCostumer = new AddCostumer();
            addCostumer.MainID = MainID;
            addCostumer.orderType = OrderType;
            MainClass.BlurBackground(addCostumer);

            if(addCostumer.txtName.Text != "")
            {
                driverID = addCostumer.driverID;
                lblDriver.Text = "Customer Name: " + addCostumer.txtName.Text + " Phone: " + addCostumer.txtPhone.Text;
                lblDriver.Visible = true;
                customerName = addCostumer.txtName.Text;
                customerPhone = addCostumer.txtPhone.Text;
            }
        }

        private void btnDinIn_Click(object sender, EventArgs e)
        {
            OrderType = "Din In";

            lblDriver.Visible = false;

            TableSelect tableSelect = new TableSelect();
            MainClass.BlurBackground(tableSelect);

            if(tableSelect.TableName != "")
            {
                lblTable.Text = tableSelect.TableName;
                lblTable.Visible = true;
            }
            else
            {
                lblTable.Text = "";
                lblTable.Visible = false;
            }

            WaiterSelect waiterSelect = new WaiterSelect();
            MainClass.BlurBackground(waiterSelect);

            if (waiterSelect.WaiterName != "")
            {
                lblWaiter.Text = waiterSelect.WaiterName;
                lblWaiter.Visible = true;
            }
            else
            {
                lblWaiter.Text = "";
                lblWaiter.Visible = false;
            }
        }

        private void btnKOT_Click(object sender, EventArgs e)
        {
            string query1 = ""; //Main table
            string query2 = ""; //detail table

            int detailID = 0;

            if(MainID == 0) //Insert
            {
                query1 = @"Insert into tblMain Values(@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change, @driverID, @CustName, @CustPhone);
                           Select SCOPE_IDENTITY()";
                //this line will get recent add id value
            }
            else //Update
            {
                query1 = @"Update tblMain Set status = @status, total = @total, received = @received, change = @change where MainID = @ID";
            }

            SqlCommand cmd = new SqlCommand(query1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainID);
            cmd.Parameters.AddWithValue("@aDate",Convert.ToDateTime(DateTime.Now.Date));
            cmd.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Pending");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text));
            cmd.Parameters.AddWithValue("@received", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@change", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@driverID", driverID);
            cmd.Parameters.AddWithValue("@CustName", customerName);
            cmd.Parameters.AddWithValue("@CustPhone", customerPhone);

            if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
            if (MainID == 0) { MainID = Convert.ToInt32(cmd.ExecuteScalar()); } else { cmd.ExecuteNonQuery(); }
            if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                detailID = Convert.ToInt32(row.Cells["dgvID"].Value);

                if(detailID == 0) //Insert
                {
                    query2 = @"Insert into tblDetails Values(@MainID, @proID, @qty, @price, @amount)";
                }
                else //Update
                {
                    query2 = @"Update tblDetails Set proID = @proID, qty = @qty, price = @price, amount = @amount where DetailID = @ID";
                }

                SqlCommand cmd2 = new SqlCommand(query2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID", Convert.ToInt32(row.Cells["proID"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
                cmd2.ExecuteScalar();
                if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }
            }
            guna2MessageDialog1.Show("Saved Successfully!");
            MainID = 0;
            detailID = 0;
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "0.00";
            guna2DataGridView1.Rows.Clear();
            lblDriver.Text = "";
        }

        public int id = 0;

        private void btnBillList_Click(object sender, EventArgs e)
        {
            BillList billList = new BillList();
            MainClass.BlurBackground(billList);

            if(billList.MainID > 0)
            {
                id = billList.MainID;
                MainID = billList.MainID;
                LoadEntries();
            }
        }

        private void LoadEntries()
        {
            string query = @"Select * from tblMain m 
                                    inner join tblDetails d on m.MainID = d.MainID 
                                    inner join products p on p.pID = d.proID 
                                        Where m.MainID = "+ id +"";

            SqlCommand cmd = new SqlCommand(query, MainClass.con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if (dt.Rows[0]["orderType"].ToString() == "Delivery")
            {
                btnDelivery.Checked = true;
                lblWaiter.Visible = false;
                lblTable.Visible = false;
            }
            else if(dt.Rows[0]["orderType"].ToString() == "Take-Away")
            {
                btnTakeAway.Checked = true;
                lblWaiter.Visible = false;
                lblTable.Visible = false;
            }
            else
            {
                btnDinIn.Checked = true;
                lblWaiter.Visible = true;
                lblTable.Visible = true;
            }

            guna2DataGridView1.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                lblTable.Text = row["TableName"].ToString();
                lblWaiter.Text = row["WaiterName"].ToString();

                string detailID = row["DetailID"].ToString();
                string proName = row["pName"].ToString();
                string proID = row["proID"].ToString();
                string qty = row["qty"].ToString();
                string price = row["price"].ToString();
                string amount = row["amount"].ToString();

                object[] obj = { 0, detailID, proID, proName, qty, price, amount };
                guna2DataGridView1.Rows.Add(obj);
            }
            GetTotal();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            CheckOut checkOut = new CheckOut();
            checkOut.MainID = id;
            checkOut.amount = Convert.ToDouble(lblTotal.Text);
            MainClass.BlurBackground(checkOut);

            MainID = 0;
            guna2DataGridView1.Rows.Clear();
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "0.00";
        }

        private void btnHold_Click(object sender, EventArgs e)
        {
            string query1 = ""; //Main table
            string query2 = ""; //detail table

            int detailID = 0;

            if(OrderType == "")
            {
                guna2MessageDialog1.Show("Please select order type");
                return;
            }

            if (MainID == 0) //Insert
            {
                query1 = @"Insert into tblMain Values(@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change, @driverID, @CustName, @CustPhone);
                           Select SCOPE_IDENTITY()";
                //this line will get recent add id value
            }
            else //Update
            {
                query1 = @"Update tblMain Set status = @status, total = @total, received = @received, change = @change where MainID = @ID";
            }

            SqlCommand cmd = new SqlCommand(query1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainID);
            cmd.Parameters.AddWithValue("@aDate", Convert.ToDateTime(DateTime.Now.Date));
            cmd.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Hold");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text));
            cmd.Parameters.AddWithValue("@received", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@change", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@driverID", driverID);
            cmd.Parameters.AddWithValue("@CustName", customerName);
            cmd.Parameters.AddWithValue("@CustPhone", customerPhone);

            if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
            if (MainID == 0) { MainID = Convert.ToInt32(cmd.ExecuteScalar()); } else { cmd.ExecuteNonQuery(); }
            if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                detailID = Convert.ToInt32(row.Cells["dgvID"].Value);

                if (detailID == 0) //Insert
                {
                    query2 = @"Insert into tblDetails Values(@MainID, @proID, @qty, @price, @amount)";
                }
                else //Update
                {
                    query2 = @"Update tblDetails Set proID = @proID, qty = @qty, price = @price, amount = @amount where DetailID = @ID";
                }

                SqlCommand cmd2 = new SqlCommand(query2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID", Convert.ToInt32(row.Cells["proID"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
                cmd2.ExecuteScalar();
                if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }
            }
            guna2MessageDialog1.Show("Saved Successfully!");
            MainID = 0;
            detailID = 0;
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "0.00";
            guna2DataGridView1.Rows.Clear();
            lblDriver.Text = "";
        }
    }
}
