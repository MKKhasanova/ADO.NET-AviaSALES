using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp5
{
    public partial class Form2 : Form
    {
        public Form1 form1;
        public SqlConnection sqlConnection;
        public SqlDataAdapter adapClient;
        public DataTable Client;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string editStr = "INSERT INTO Client (Id, Name, Address, Passport, YearOfBirth) VALUES (@Id, @Name, @Address, @Passport, @YearOfBirth)";

            SqlCommand insertCmd = new SqlCommand(editStr, form1.sqlConnection);

            if (int.TryParse(textBox1.Text, out int id))
            {
                insertCmd.Parameters.AddWithValue("@Id", id);
            }
            else
            {
                MessageBox.Show("ID введен некорректно!!");
                return;
            }

            insertCmd.Parameters.AddWithValue("@Name", textBox2.Text);
            insertCmd.Parameters.AddWithValue("@Address", textBox3.Text);
            insertCmd.Parameters.AddWithValue("@Passport", textBox4.Text);

            if (DateTime.TryParseExact(textBox5.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateDeparture))
            {
                insertCmd.Parameters.AddWithValue("@YearOfBirth", dateDeparture);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите дату в формате дд.ММ.гггг");
                return;
            }


            insertCmd.ExecuteNonQuery();
            form1.Client.Clear();
            form1.adapClient.Fill(form1.Client);
            Close();
        }
    }
}
