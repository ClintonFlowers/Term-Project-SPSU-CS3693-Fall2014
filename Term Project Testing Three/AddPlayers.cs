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



    public partial class AddParticipants : Form
    {
        string conString;
        string sql;
        string currentSelected = "";

        public AddParticipants(string currentDb)
        {
            InitializeComponent();
           // conString = @"Data Source = C:\Users\James\Documents\Visual Studio 2013\Projects\TournamentApp\TournamentApp\SPSU_1v1LoLTournament_2014; version = 3";
            
           
           //conString = @"Data Source =" + Path.GetFullPath(@"SPSU_1v1LoLTournament_2014"); ;
            conString = @"Data Source =" + currentDb;

            //MessageBox.Show(conString);
        }



        private void RefreshData(string participantsOrMatches)
        {
  sql = "select * from " + participantsOrMatches;
            using (SQLiteConnection con = new SQLiteConnection(conString))
            {
                try
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conString);
                    da.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0].DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void AddData(string participantsOrMatches)
        {
            int numberOfColumns = 0;
            foreach (DataGridViewColumn tbc in dataGridView1.Columns)
            {
                numberOfColumns = numberOfColumns + 1;
            }
            //MessageBox.Show("NOC: " + numberOfColumns);

            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if ("" + dataGridView1[0, row.Index].Value != "")
                {
                    string sql = "INSERT INTO " + participantsOrMatches + " VALUES (";
                    sql = sql + "\"" + dataGridView1[0, row.Index].Value;
                    for (int loop = 1; loop < numberOfColumns; loop++)
                    {
                        sql = sql + "\",\"" + dataGridView1[loop, row.Index].Value;
                    }
                    sql = sql + "\");";


                    using (SQLiteConnection con = new SQLiteConnection(conString))
                    {
                        try
                        {
                            con.Open();
                            DataSet ds = new DataSet();
                            SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conString);
                            da.Fill(ds);
                            dataGridView1.DataSource = ds.Tables[0].DefaultView;

                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message);
                            //MessageBox.Show("CON: " + con);
                            //MessageBox.Show("sql string: " + sql);
                        }



                    }

                    dataGridView1.Refresh();
                }
            }
        }

        private void btnSub_Click(object sender, EventArgs e)
        {
            AddData(currentSelected);
        }

        private void AddPlayers_Load(object sender, EventArgs e)
        {
            refreshParticipantsButtonClick(sender, e);
        }

        private void refreshMatchesButtonClick(object sender, EventArgs e)
        {
            RefreshData("matches");
            currentSelected = "matches";
        }

        private void refreshParticipantsButtonClick(object sender, EventArgs e)
        {
            RefreshData("participants");
            currentSelected = "participants";
        }
    }
}
