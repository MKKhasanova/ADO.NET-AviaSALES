using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {

        public SqlConnection sqlConnection = null;
        public SqlCommand cmdTickets, cmdClient, cmdHistory;
        public SqlDataAdapter adapTickets, adapClient, adapHistory;
        public string strTickets, strClient, strHystory;
        public DataTable Tickets, Client, History;
        string filterField;
        public BindingSource bsTickets, bsClient, bsHistory;
        Thread th;
        public int Option;

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Фильтрация по столбцу "Destination" с использованием выбранного значения из ComboBox2
            bsTickets.Filter = CombineFilters(GetFilter1(), GetFilter2());
            /*string filter1;
            filterField = "Kuda";
            filter1 = filterField + " LIKE '%" + comboBox1.Text + "%'";
            bsTickets.Filter = filter1;*/
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView3.CurrentRow;

            if (selectedRow != null)
            {
                // Получаем значение ключевого поля (например, ID) выбранной строки
                int keyFieldValue = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                // Создаем SQL-запрос для удаления строки с указанным ключевым полем
                string deleteQuery = $"DELETE FROM History WHERE ID = {keyFieldValue}";
                using (SqlCommand DELcommand = new SqlCommand(deleteQuery, sqlConnection))
                {
                    // Выполняем команду удаления
                    DELcommand.ExecuteNonQuery();
                }
            }
           
            //insertCmd1.ExecuteNonQuery();
            History.Clear();
            adapHistory.Fill(History);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем выбранное значение в ComboBox
            // Фильтрация по столбцу "Otkuda" с использованием выбранного значения из ComboBox1
            string filter1 = $"Otkuda LIKE '%{comboBox1.Text}%'";
            bsTickets.Filter = CombineFilters(filter1, GetFilter2());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Option = 1;
            Form2 form2 = new Form2();
            form2.form1 = this;
            form2.Show();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow curRow = dataGridView1.CurrentRow;
           // textBox1.Text = curRow.Cells["Name"].Value.ToString();
            textBox2.Text = curRow.Cells["Otkuda"].Value.ToString();
            textBox3.Text = curRow.Cells["Kuda"].Value.ToString();
            textBox4.Text = curRow.Cells["Vremya_otpravleniya"].Value.ToString();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow curRow1 = dataGridView2.CurrentRow;
            textBox1.Text = curRow1.Cells["Name"].Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e) // оформить билет
        {
            DataGridViewRow curRow = dataGridView1.CurrentRow;
            DataGridViewRow curRow1 = dataGridView2.CurrentRow;
            string insertStr = "INSERT INTO History (Client_ID, Flight_number, Otkuda, Kuda) VALUES (@Client_ID, @Flight_number, @Otkuda, @Kuda)";
            //string insertStr1 = "INSERT INTO History (Client_ID) VALUES (@Client_ID)";
            SqlCommand insertCmd = new SqlCommand(insertStr, sqlConnection);
           // SqlCommand insertCmd1 = new SqlCommand(insertStr1, sqlConnection);
            
            // Добавление параметров к команде
            insertCmd.Parameters.AddWithValue("@Flight_number", curRow.Cells["Flight_number"].Value.ToString());
            insertCmd.Parameters.AddWithValue("@Otkuda", curRow.Cells["Otkuda"].Value.ToString());
            insertCmd.Parameters.AddWithValue("@Kuda", curRow.Cells["Kuda"].Value.ToString());
           // insertCmd.Parameters.AddWithValue("@Vremya_otpravleniya", Convert.ToDateTime(curRow.Cells["Vremya_otpravleniya"].Value).ToString("HH:mm:ss"));
           // insertCmd.Parameters.AddWithValue("@Price", curRow.Cells["Price"].Value);
            insertCmd.Parameters.AddWithValue("@Client_ID", curRow1.Cells["Client_ID"].Value);

            insertCmd.ExecuteNonQuery();
            //insertCmd1.ExecuteNonQuery();
            History.Clear();
            adapHistory.Fill(History);
          


        }
        private string GetFilter1()
        {
            // Получение текущего фильтра для столбца "Otkuda"
            return $"Otkuda LIKE '%{comboBox1.Text}%'";
        }

        private string GetFilter2()
        {
            // Получение текущего фильтра для столбца "Destination"
            return $"Kuda LIKE '%{comboBox2.Text}%'";
        }

        private string CombineFilters(string filter1, string filter2)
        {
            // Функция для объединения двух фильтров с использованием оператора AND
            if (string.IsNullOrEmpty(filter1) || string.IsNullOrEmpty(filter2))
            {
                return filter1 + filter2;
            }
            else
            {
                return $"({filter1}) AND ({filter2})";
            }
        }

        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["StationDB2"].ConnectionString);
            sqlConnection.Open();
            /*if (sqlConnection.State == ConnectionState.Open) 
            {
                MessageBox.Show("Подключение установлено!!");
            }*/
            strTickets = "SELECT * FROM Tickets";
            cmdTickets = new SqlCommand(strTickets, sqlConnection);
            adapTickets = new SqlDataAdapter(cmdTickets);
            Tickets = new DataTable();
            adapTickets.Fill(Tickets);
            dataGridView1.DataSource = Tickets;
            bsTickets = new BindingSource();
             bsTickets.DataSource = Tickets;
             dataGridView1.DataSource = bsTickets;
            //DataSet
            strClient = "SELECT * FROM Client";
            cmdClient = new SqlCommand(strClient, sqlConnection);

            // Копирование данных в локальную таблицу
            adapClient = new SqlDataAdapter(cmdClient);
            Client = new DataTable();
            adapClient.Fill(Client);
            dataGridView2.DataSource = Client;
            // cmdFirms = new SqlCommand();
            cmdTickets.Connection = sqlConnection;


            strHystory = "SELECT * FROM History";
            cmdHistory = new SqlCommand(strHystory, sqlConnection);
            adapHistory = new SqlDataAdapter(cmdHistory);
            History = new DataTable();
            adapHistory.Fill(History);
            dataGridView3.DataSource = History;
            bsHistory = new BindingSource();
            bsHistory.DataSource = History;
            dataGridView3.DataSource = bsHistory;


            // Привязка данных на DataGridView
            // bsKorzina = new BindingSource();
            // bsKorzina.DataSource = Korzina;
            //dataGridView2.DataSource = Client;


        }
    }
}
