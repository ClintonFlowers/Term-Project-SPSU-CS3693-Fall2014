using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace Term_Project_Testing_Three
{   
    

    public partial class GameTemplateCreator : Form
    {
        
        public GameTemplateCreator()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                String dbPathAndName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Tournament_Manager\\Tournaments\\" + textBox1.Text + ".sqlite";
                //String dbName = textBox1.Text + ".sqlite";
                Console.WriteLine("dbPathAndName: " + dbPathAndName); // delete this and preceding line; does nothing
                //MessageBox.Show(dbPathAndName);
                String createMatchesTable;
                String createParticipantsTable;
                List<String> participantFields = new List<String>();
                List<String> matchFields = new List<String>();
                List<String> participantFieldDataTypes = new List<String>();
                List<String> matchFieldsDataTypes = new List<String>();
                participantFields.Add(textBox2.Text);
                participantFields.Add(textBox3.Text);
                participantFields.Add(textBox4.Text);
                participantFields.Add(textBox9.Text);
                participantFields.Add(textBox8.Text);
                participantFieldDataTypes.Add(comboBox1.Text);
                participantFieldDataTypes.Add(comboBox2.Text);
                participantFieldDataTypes.Add(comboBox3.Text);
                participantFieldDataTypes.Add(comboBox8.Text);
                participantFieldDataTypes.Add(comboBox7.Text);
                matchFields.Add(textBox7.Text);
                matchFields.Add(textBox6.Text);
                matchFields.Add(textBox5.Text);
                matchFields.Add(textBox11.Text);
                matchFields.Add(textBox10.Text);
                matchFieldsDataTypes.Add(comboBox6.Text);
                matchFieldsDataTypes.Add(comboBox5.Text);
                matchFieldsDataTypes.Add(comboBox4.Text);
                matchFieldsDataTypes.Add(comboBox10.Text);
                matchFieldsDataTypes.Add(comboBox9.Text);

                String customParticipantFields = "";
                String customMatchFields = "";

                Int32 Index = 0;

                foreach (String aPart in participantFields)
                {
                    
                    if (aPart != "")
                    {
                        customParticipantFields = customParticipantFields + ", " + aPart + " " + participantFieldDataTypes[Index];
                        Index = Index + 1;
                    }
                }
                Index = 0;
                foreach (String aPart in matchFields)
                {
                    
                    if (aPart != "")
                    {
                        customMatchFields = customMatchFields + ", "+ aPart + " " + matchFieldsDataTypes[Index];
                        Index = Index + 1;
                    }
                }
                string directory = Directory.GetCurrentDirectory();
                using(StreamWriter sw = new StreamWriter(directory + "\\templates\\" + textBox1.Text + "_template.txt")){

                    createParticipantsTable = "CREATE TABLE participants (id INTEGER NOT NULL PRIMARY KEY UNIQUE, real_name TEXT" + customParticipantFields + ")";
                    createMatchesTable = "CREATE TABLE matches (id INTEGER NOT NULL PRIMARY KEY UNIQUE, team1_id INTEGER, team2_id INTEGER, winner_id INTEGER" + customMatchFields + ")";
                    sw.WriteLine(createMatchesTable);
                    sw.WriteLine(createParticipantsTable);
                    //SQLiteConnection.CreateFile(dbName);
                    //SQLiteConnection dbConnection;
                    //dbConnection = new SQLiteConnection("Data Source=" + dbName + ";Version=3;");
                    //dbConnection.Open();
                    ////createMatchesTable = "CREATE TABLE matches (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, team1_id INTEGER, team2_id INTEGER, winner_id INTEGER)";
                    ////createParticipantsTable = "CREATE TABLE participants (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, real_name TEXT)";
                    //SQLiteCommand command = new SQLiteCommand(createMatchesTable, dbConnection);
                    //command.ExecuteNonQuery();
                    //command = new SQLiteCommand(createParticipantsTable, dbConnection);
                    //command.ExecuteNonQuery();
                    //dbConnection.Close();
                }

                MessageBox.Show("Template created as txt file.");
            }
            catch (Exception fail)
            {
                Console.WriteLine("FAILURE: " + fail.ToString());
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void GameTemplateCreator_Load(object sender, EventArgs e)
        {

        }
    }
}
