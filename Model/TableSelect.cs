﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagementApp.Model
{
    public partial class TableSelect : Form
    {
        public TableSelect()
        {
            InitializeComponent();
        }

        public string TableName;

        private void TableSelect_Load(object sender, EventArgs e)
        {
            string query = "Select * from tables";
            SqlCommand cmd = new SqlCommand(query, MainClass.con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                Guna.UI2.WinForms.Guna2Button btn = new Guna.UI2.WinForms.Guna2Button();
                btn.Text = row["tName"].ToString();
                btn.Width = 150;
                btn.Height = 50;
                btn.FillColor = Color.FromArgb(241, 85, 126);
                btn.HoverState.FillColor = Color.FromArgb(50, 55, 89);

                //Event for click
                btn.Click += new EventHandler(btn_Click);
                flowLayoutPanel1.Controls.Add(btn);
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            TableName = (sender as Guna.UI2.WinForms.Guna2Button).Text.ToString();
            this.Close();
        }
    }
}
