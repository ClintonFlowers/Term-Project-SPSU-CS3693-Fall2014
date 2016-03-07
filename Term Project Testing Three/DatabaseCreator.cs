using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Term_Project_Testing_Three
{
    public partial class DatabaseCreator : Form
    {
        public DatabaseCreator()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //var files = Directory.GetFiles("\");
            string directory = Directory.GetCurrentDirectory();
            //System.Windows.Forms.MessageBox.Show("Directory: " + directory);
            var files = Directory.GetFiles(directory + "\\templates\\", "*_template.txt").ToList();
            foreach (string item in files){
                //printout = printout + item;
                comboBox1.Items.Add(item);
            }
            //System.Windows.Forms.MessageBox.Show(printout);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
            string createMatchesTable;
            string createParticipantsTable;

            using (StreamReader sr = new StreamReader(comboBox1.Text)){
                createMatchesTable = sr.ReadLine();
                createParticipantsTable = sr.ReadLine();
                //System.Windows.Forms.MessageBox.Show(createMatchesTable + " :FLOCKA: " + createParticipantsTable);
                sr.Close();
            }

            string directory = Directory.GetCurrentDirectory();
            String dbName = textBox1.Text + ".sqlite";
            SQLiteConnection.CreateFile(directory + "\\tournaments\\" + dbName);
            SQLiteConnection dbConnection;
            dbConnection = new SQLiteConnection("Data Source=" + directory + "\\tournaments\\" + dbName + ";Version=3;");
            dbConnection.Open();
            SQLiteCommand command = new SQLiteCommand(createMatchesTable, dbConnection);
            command.ExecuteNonQuery();
            command = new SQLiteCommand(createParticipantsTable, dbConnection);
            command.ExecuteNonQuery();
            dbConnection.Close();
            MessageBox.Show("Tournament database created.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
