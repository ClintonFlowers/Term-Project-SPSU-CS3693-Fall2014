// Majority of code by Clinton Flowers, some code by James Tuxbury, support by Adam Liebert
// Written (Clinton's) sophomore year, Fall 2014 for Applications Programming - CS 3693. 
// This is my first real C# program, and probably my second non-super-simple program, for what that implies. 
// Uploading to Github for history/reference; good code not guaranteed; some bad code guaranteed. 
// Comments may or may not reflect reality. 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SQLite;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Data.SqlClient;
using System.Threading;

namespace Term_Project_Testing_Three
{

    //The core components of the program are in MainForm.cs and MainForm.Designer.cs
    //The two classes call methods between each other fairly frequently. 
    //In general, sqlite database access code is stored in MainForm.cs, and code for calculating AND drawing the components on the GUI is stored in MainForm.Designer.cs

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshDatabaseListButtonClick(sender, e);
        }

        public string directory = Directory.GetCurrentDirectory();
        public static string dsrc = "";
        List<Participant> participantList = new List<Participant>();
        public static List<Match> matchList = new List<Match>();
        public static Process proc = Process.GetCurrentProcess();
        public SQLiteConnection con;
        public SQLiteDataAdapter da;
        public SQLiteCommandBuilder builder;
        public static string databaseFilePath;
        public Boolean databaseHasBeenLoadedToGridView;
        public DataTable table;

        private void EnumerateExisting_Click(object sender, EventArgs e)
        {
            //Cause the drawing of the bracket, including reading from the database, creating Match objects, which leads to drawing boxes, bezier curves, buttons, name strings, etc.
            //Reads a lot from the database (IO) and causes a lot of redraws (CPU/threading).
            //Much of this inefficiency comes from the fact it clears the entire list of matches and redraws everything each time this or a ParticipantButton is clicked. 
            if (dbSelectionComboBox.Text != "")
            {
                Console.WriteLine("dbSelectionComboBox: " + dbSelectionComboBox.Text);
                dsrc = "Data Source =" + dbSelectionComboBox.Text;

                using (SQLiteConnection con2 = new SQLiteConnection(@dsrc))
                {
                    string sql2 = "";
                    try
                    {

                        foreach (Button thisButton in Match.buttonList)
                        {
                            thisButton.Dispose();
                        }
                        Match.buttonList.Clear();
                        Match.latestRow = 1;
                        Match.latestColumn = 1;
                        matchList.Clear();
                        boxList.Clear();

                        con2.Open();

                        DataSet ds = new DataSet();
                        sql2 = "SELECT id, team1_id, team2_id, winner_id FROM " + "matches";
                        SQLiteDataAdapter da = new SQLiteDataAdapter(sql2, con2);
                        da.Fill(ds);
                        foreach (DataRow data in ds.Tables[0].Rows)
                        {
                            Participant participant1 = new Participant(Int32.Parse(data["team1_id"].ToString()));
                            Participant participant2 = new Participant(Int32.Parse(data["team2_id"].ToString()));
                            Match match = new Match(matchList.Count() + 1, participant1, participant2, this, matchList, dsrc);
                            match.setWinner(Int32.Parse(data["winner_id"].ToString()));
                        }
                        con2.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write("\nError message: " + ex.Message);
                        MessageBox.Show("ERRAHR: " + ex.Message + "\nSQLCOMMAND: " + sql2 + "\nDSRC: " + dsrc);
                    }
                }

                foreach (Match forMatch in matchList)
                {
                    //Loops for each match in the list of matches and causes a drawing of the box. 
                    forMatch.drawBox(this);
                }


                this.forceRedraw(this);
            }
            else
            {
                MessageBox.Show("DB Selection combobox is detected as empty.");
            }

        }

        #region Load matches and participants buttons
        private void LoadMatchesButtonClick(object sender, EventArgs e)
        {
            databaseHasBeenLoadedToGridView = true;
            loadDatabaseToDataGridView1("matches");
        }

        private void LoadParticipantsButtonClick(object sender, EventArgs e)
        {
            //Loads the data from the database "participants" table onto dataGridView1. 
            databaseHasBeenLoadedToGridView = true;
            loadDatabaseToDataGridView1("participants");
        }

        private void loadDatabaseToDataGridView1(string matchesOrParticipants)
        {
            //Receives a string indicating whether to load the "matches" or "participants" table onto the main datagridview and adds it to the datagridview.
            mainDataGridView.Columns.Clear();
            mainDataGridView.DataSource = new DataSet();
            string sql;
            sql = "SELECT * FROM " + matchesOrParticipants;
            dsrc = "Data Source =" + dbSelectionComboBox.Text;
            con = new SQLiteConnection(@dsrc);
            da = new SQLiteDataAdapter();
            da.SelectCommand = new SQLiteCommand(sql, con);
            using (builder = new SQLiteCommandBuilder(da))
            {
                try
                {
                    table = new DataTable();
                    mainDataGridView.DataSource = table;
                    con.Open();
                    da.Fill(table);
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        public void forceRedraw(MainForm that)
        {
            //Forces the redraw of the passed-in window (which is, I believe, entirely the tabPage). 
            //This leads to the portion of the program that is the least CPU-efficient, hence the stopwatches for determining completion time.
            try
            {
                that.Refresh();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in refreshing: " + e.Message);
            }
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //stopwatch.Stop();
            //System.Diagnostics.Debug.Write("\nStopwatch time:" + stopwatch.ElapsedMilliseconds);
        }

        private void newTournamentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create and show the form used to create new .sqlite database files. 
            DatabaseCreator Form2 = new DatabaseCreator();
            Form2.Show();
        }

        private void createGameTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create and show the form used to create templates for new types of games. 
            GameTemplateCreator GameTemplateCreator = new GameTemplateCreator();
            GameTemplateCreator.Show();
        }

        private void AddNewParticipantsButtonClick(object sender, EventArgs e)
        {
            //Open the (fairly deprecated) form to add new players to the database. Preferred way of doing this is now in the tab dedicated to modifying the database. 
            AddParticipants AddParticipants = new AddParticipants(dbSelectionComboBox.Text);
            AddParticipants.Show();
        }

        private void RefreshDatabaseListButtonClick(object sender, EventArgs e)
        {
            //Refresh the combobox containing the paths to any .sqlite files found in the working directory. 
            var files = Directory.GetFiles(directory + "\\tournaments\\", "*.sqlite").ToList();
            dbSelectionComboBox.Items.Clear();
            foreach (string item in files)
            {
                dbSelectionComboBox.Items.Add(item);
            }
        }

        public string getDsrc()
        {
            return dsrc;
        }

        //private void graphicsWindowToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    //For debug purposes. Unused; can be deleted.
        //    GraphicsWindow gw = new GraphicsWindow();
        //    gw.Show();
        //}

        //private void CreateMatchButtonClick(object sender, EventArgs e)
        //{
        //    //Unused. Can be deleted.
        //    dsrc = "Data Source =" + dbSelectionComboBox.Text;
        //}

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            dsrc = "Data Source =" + dbSelectionComboBox.Text;
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        #region unused, old or debug methods
        //public int getLatestID(string participantsOrMatches)
        //{
        //    //Determine the lowest free distinct UID that can be used in the creation of a player or match. Might be unused. 
        //    int answer = 0;
        //    string sql = "";

        //    using (SQLiteConnection con = new SQLiteConnection(@dsrc))
        //    {
        //        try
        //        {
        //            con.Open();
        //            sql = "SELECT COUNT(DISTINCT id) FROM " + participantsOrMatches;
        //            SQLiteCommand command = new SQLiteCommand(sql, con);
        //            SQLiteDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                answer = reader.GetInt32(0);
        //            }
        //            con.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("ERRAHR: " + ex.Message + ". SQLCOMMAND: " + sql + "..DSRC: " + dsrc);
        //        }
        //    }
        //    return answer;
        //}

        //private void testMathFloorandCeilToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    //Testing code for debug. Can be deleted. 
        //    int latestColumn = 1;
        //    int maxParticipants = 8;
        //    double ColumnDiv2 = (double)latestColumn / 2;
        //    int ColumnDiv2Rounded = (int)Math.Round(ColumnDiv2, MidpointRounding.AwayFromZero);
        //    int numberToReturn = maxParticipants / (int)Math.Pow(2, ColumnDiv2Rounded);
        //    MessageBox.Show(numberToReturn + " : ColumnDiv2: " + ColumnDiv2);
        //}

        //private void testQuickNextMatchToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    //Testing code for debug. Can be deleted. 
        //    int[] hardCodedArrayOfNextMatches = new int[] { -1, 17, 17, 18, 18, 19, 19, 20, 20, 21, 21, 22, 22, 23, 23, 24, 24, 25, 25, 26, 26, 27, 27, 28, 28, 29, 29, 30, 30, 31, 31 };
        //    string stringToPrint = "";
        //    for (int x = 1; x < 31; x++)
        //    {
        //        if (x % 2 == 1)
        //        {
        //            stringToPrint = stringToPrint + (x) + ": " + hardCodedArrayOfNextMatches[x] + " \t " + "would be player 1" + "\n";
        //        }
        //        if (x % 2 == 0)
        //        {
        //            stringToPrint = stringToPrint + (x) + ": " + hardCodedArrayOfNextMatches[x] + " \t " + "would be player 2" + "\n";
        //        }

        //    }
        //    MessageBox.Show(stringToPrint);
        //}

        private void updateButton_Click(object sender, EventArgs e)
        {
            //Button to push changes made to the dataGridView to the database on the DB management tab
            //Won't work unless at least one DB has been loaded on the program before
            if (databaseHasBeenLoadedToGridView == true)
            {
                builder = new SQLiteCommandBuilder(da);
                Console.WriteLine("# of rows updated(?): " + da.Update(table).ToString());
            }
            else
            {
                MessageBox.Show("Please load a database first.");
            }
        }


        #endregion

        private void basketballToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@directory + "\\rulebooks\\" + "NBA_Official-NBA-Rule-Book.pdf");
        }
        private void footballToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@directory + "\\rulebooks\\" + "NFL_2013 - Rule Book.pdf");
        }
        private void baseballToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@directory + "\\rulebooks\\" + "MLB_official_baseball_rules.pdf");
        }

        #region Lots of unused on-click things

        private void saveTournamentToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void calculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("calc.exe");
        }
        private void soundRecorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("soundrecorder");
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.FileName = "C:\\Windows\\System32\\SoundRecorder.exe";
            //Process.Start(startInfo);
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        private void soundboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Creates a soundboard form and shows it. 
            SoundBoard soundBoard = new SoundBoard();
            soundBoard.Show();
        }

        private void randomNumberGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Creates a RNG calculator form and shows it.
            RandomNumberGenerator rng = new RandomNumberGenerator();
            rng.Show();
        }

        private void quickStartRichTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void openTournamentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Opens up a combobox allowong the selection of a sqlite database not listed on the combobox on the first tab
            OpenFileDialog openDatabaseFileDialog = new OpenFileDialog();
            openDatabaseFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\tournaments\\";
            openDatabaseFileDialog.Filter = "SQLite files (*.sqlite)|*.sqlite";
            openDatabaseFileDialog.RestoreDirectory = true;

            if (openDatabaseFileDialog.ShowDialog() == DialogResult.OK)
            {
                databaseFilePath = openDatabaseFileDialog.FileName;
            }

            dbSelectionComboBox.Text = databaseFilePath;
        }
    }

    public class Participant
    {
        //A class representing a participant. Two of these may be passed to a Match object when a Match object is created. 
        //Largely under-utilized (since initially I was passing by ID's of participants rather than passing references to objects representing those records).
        //The getNameString class causes lots of database IO due to repeated accessing of the name strings associated with each button on each enumeration. 
        //A static list of names should be created so this is only done once, or upon pushing updates to the DB.

        private int id;
        private string real_name;
        public Participant(int passed_id, string passed_real_name)
        {
            id = passed_id;
            real_name = passed_real_name;
        }

        public Participant()
        {

        }

        public Participant(int passed_id)
        {
            id = passed_id;
        }

        public int getId()
        {
            return id;
        }

        public static string getNameString(int id)
        {
            //Gets the name string associated with a participant ID from the DB and returns it

            string answer = "";
            string dsrc = MainForm.dsrc;
            string sqlCommand = "SELECT real_name FROM participants WHERE id=" + id + "";

            using (SQLiteConnection con = new SQLiteConnection(@dsrc))
            {
                try
                {
                    con.Open();
                    DataSet ds = new DataSet();
                    SQLiteDataAdapter da = new SQLiteDataAdapter(sqlCommand, dsrc);
                    da.Fill(ds);
                    answer = Convert.ToString(ds.Tables[0].Rows[0]["real_name"]);
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERRAHR: " + ex.Message + "\nSQLCOMMAND: " + sqlCommand + "\nDSRC: " + dsrc + "\nThis might mean there is no winner for the match.");
                }
            }

            return answer;
        }
    }

    public class Match
    {
        //The class representing an individual match. Each time a match instance is created, it gets added to the list of these objects. 
        //Each time the bracket is redrawn, each match is cleared, and they are created anew. This code happens in the EnumerateExisting class. 
        private int id;
        public int participant1_id;
        public int participant2_id;
        public string participant1_name = "";
        public string participant2_name = "";
        Participant participant1;
        Participant participant2;
        private int winner_id = 0;
        public string dataSource;

        public static int latestRow = 1;
        public static int latestColumn = 1;
        public static int maxMatches = 16;
        public static int middleRowNumber = 5; //TODO: Fix this being associated with maxMatches etc (in the whole hard-coded match fiasco I've got going on)
        public static int maxRowsOfAnyColumn = 0;

        public static List<System.Windows.Forms.Button> buttonList = new List<System.Windows.Forms.Button>();

        //TODO: Fix hard-coded bracket?
        public static int numberOfAllowedRows
        {
            //Calculate the number of rows allowed in a certain column -- the number of rows in a column is smaller for the inner columns, and this is the math for that. 
            get
            {
                double ColumnDiv2 = (double)latestColumn / 2;
                int ColumnDiv2Rounded = (int)Math.Round(ColumnDiv2, MidpointRounding.AwayFromZero);
                int numberToReturn = maxMatches / (int)Math.Pow(2, ColumnDiv2Rounded);

                if (maxRowsOfAnyColumn == 0)
                {
                    maxRowsOfAnyColumn = numberToReturn;
                }

                return numberToReturn;
            }
        }

        public Match(int passed_id, Participant passed_participant1, Participant passed_participant2, MainForm that, List<Match> matchList, string passedDataSource)
        {
            //Constructor for a match object. Some of the next few lines may not be strictly necessary anymore. 
            id = passed_id;
            participant1 = passed_participant1;
            participant2 = passed_participant2;
            participant1_id = participant1.getId();
            participant2_id = participant2.getId();
            dataSource = passedDataSource;

            matchList.Add(this);
        }

        public int getId()
        {
            return id;
        }

        public Match(int passed_id)
        {
            id = passed_id;
        }

        public string getParticipantNameString(int participantId)
        {
            //One attempt to decrease the amount of IO with the database: if the name associated with an instance of Participant isn't empty, then we assume we know it and return that, else fetch it from the database. 
            if (participantId == participant1_id && participant1_name != "")
            {
                Console.WriteLine("participant1_name: " + participant1_name);
                return participant1_name;
            }
            else if (participantId == participant2_id && participant2_name != "")
            {
                Console.WriteLine("participant2_name: " + participant2_name);
                return participant2_name;
            }
            else if (participantId == participant1_id)
            {
                participant1_name = getParticipantNameStringFromDB(participantId);
                return participant1_name;
            }
            else if (participantId == participant2_id)
            {
                participant2_name = getParticipantNameStringFromDB(participantId);
                return participant2_name;
            }
            else
            {
                return "NO IDEA YO";
            }
        }


        public static string getParticipantNameStringFromDB(int participantId)
        {
            //A method making sure that the ID we're tring to fetch isn't zero. If the ID is zero, then it has no name. Don't think too hard on it. 
            if (participantId == 0)
            {
                return "";
            }
            else
            {
                return Participant.getNameString(participantId);
            }
        }

        private int determineNewMatchID()
        {
            //This code used to do something useful, like determine the latest ID. It's gone now. I don't remember doing that.
            int answer = 0;
            return answer;
        }

        public int getWinnerId()
        {
            return winner_id;
        }

        public void setWinnerOnDB(int passed_winner_id)
        {
            //Write the participant ID of the winner (passed in by arg) to the database row associated with this match. 
            winner_id = passed_winner_id;
            string sql = "UPDATE matches SET winner_id=" + winner_id + " WHERE id=" + this.id + ";";
            using (SQLiteConnection con = new SQLiteConnection(@dataSource))
            {
                try
                {
                    con.Open();
                    SQLiteCommand sl = new SQLiteCommand(sql, con);
                    System.Diagnostics.Debug.Write("\nSQL: " + sql);
                    sl.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("EXCEPTION: " + ex.Message);
                }
            }
        }

        public void setParticipantOnDB(int passed_winner_id, string fieldString)
        {
            //Set a participant of a match (either field team1 or team2 in the database) to a participant's ID. 
            if (fieldString != "team1_id" & fieldString != "team2_id")
            {
                MessageBox.Show("Invalid setParticipantOnDB fieldString. Exiting to protect DB.");
                Environment.Exit(1);
            }
            string sql = "UPDATE matches SET " + fieldString + "=" + passed_winner_id + " WHERE id=" + this.id + ";";
            using (SQLiteConnection con = new SQLiteConnection(@dataSource))
            {
                try
                {
                    con.Open();
                    SQLiteCommand sl = new SQLiteCommand(sql, con);
                    System.Diagnostics.Debug.Write("\nSQL: " + sql);
                    sl.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("EXCEPTION: " + ex.Message);
                }
            }
        }

        public void setWinner(int passed_winner_id)
        {
            winner_id = passed_winner_id;
        }

        public void setWinnerByMatchId(int matchId, int participantID)
        {
            //Actually doesn't do what the method implies--just sets the winner of this match. Otherwise it could be a Static. Should probably be deleted. 
            MessageBox.Show("matchId: " + matchId + " participantId: " + participantID);
            this.setWinnerOnDB(participantID);
        }

        public void drawBox(MainForm that)
        {
            //Call the big DrawBox method of MainForm.Designer.cs, and pass in the desired row and column for the newly created box.

            //TODO: Fix hard-coded bracket of 16
            //Match.numberOfAllowedRows = 8;
            //int[] displayColumnArray = new int[] { 1, 7, 2, 6, 3, 5, 4, 0, 0, 0, 0, 0 };
            //TODO: Fix hard coded stuff
            int[] displayColumnArray = new int[] { 1, 9, 2, 8, 3, 7, 4, 6, 5, 5 }; // The number by which the column X-spacing is multiplied by. 

            if (Match.latestRow > numberOfAllowedRows)
            {
                Match.latestRow = 1;
                Match.latestColumn++;
            }
            that.DrawMatchBox(this, Match.latestRow, participant1, participant2, displayColumnArray[Match.latestColumn - 1]);
            Match.latestRow++;
        }

        public void calculateMatchup()
        {

        }

        //public void updateDb()
        //{
        //    //Hard-update the DB with a newly-created match. 
        //    //That functionality doesn't work at the moment due to more pressing requirements--instead, databases are added to the DB via the DataGridView editor.
        //    //Currently unused.
        //    string sql = "";

        //    if (winner_id > -1)
        //    {
        //        sql = "INSERT INTO " + "matches" + " (id, team1_id, team2_id, winner_id)" + " VALUES ('" + id + "','";
        //        sql = sql + "" + participant1_id + "','" + participant2_id + "','" + winner_id;
        //        sql = sql + "');";
        //    }
        //    else
        //    {
        //        sql = "INSERT INTO " + "matches" + " (id, team1_id, team2_id)" + " VALUES ('" + id + "','";
        //        sql = sql + "" + participant1_id + "','" + participant2_id;
        //        sql = sql + "');";
        //    }

        //    using (SQLiteConnection con = new SQLiteConnection(@dataSource))
        //    {
        //        try
        //        {
        //            con.Open();
        //            DataSet ds = new DataSet();
        //            SQLiteCommand sl = new SQLiteCommand(sql, con);
        //            System.Diagnostics.Debug.Write("\nSQL: " + sql);
        //            sl.ExecuteNonQuery();
        //            con.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("EXCEPTIONL: " + ex.Message);
        //        }
        //    }
        //}

        //public string getWinnerNameString(string dsrc)
        //{
        //    //This would get the name string of the winner, but now that functionality is done by a method in the Player class: Participant.getNameString(participantId)
        //    //Currently unused.
        //    string answer = "";
        //    string sqlCommand = "SELECT real_name FROM participants WHERE id=" + winner_id + "";

        //    using (SQLiteConnection con = new SQLiteConnection(@dsrc))
        //    {
        //        try
        //        {
        //            con.Open();
        //            DataSet ds = new DataSet();
        //            SQLiteDataAdapter da = new SQLiteDataAdapter(sqlCommand, dsrc);
        //            da.Fill(ds);
        //            answer = Convert.ToString(ds.Tables[0].Rows[0]["real_name"]);
        //            con.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("ERRAHR: " + ex.Message + "..SQLCOMMAND: " + sqlCommand + "..DSRC: " + dsrc + "\nThis might mean there is no winner for the match.");
        //        }
        //    }

        //    return answer;
        //}
    }
}
