using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Term_Project_Testing_Three
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        Graphics g;
        Pen p = new Pen(Color.Black, 7);
        Rectangle rect = new Rectangle();
        List<BoxCoords> boxList = new List<BoxCoords>();

        //private void GraphicsWindow_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        //{
        //    //For debugging drawing a simple line.
        //    //Currently unused.

        //    g = this.CreateGraphics();
        //    // Make a big red pen.
        //    p = new Pen(Color.Red, 7);
        //    //Other drawing code can go here
        //}

        public void DrawMatchBox(Match match, int rowOfColumn, Participant participant1, Participant participant2, int abstractedColumn = 1)
        {
            //Acts as the main class for determining the coordinates for the corners of the boxes, with each box representing a match containing two buttons, each button representing a player.
            //Creates and manages the box struct. 

            int participant1ID = participant1.getId();
            int participant2ID = participant2.getId();

            //Primary math for spacing. 
            int yPadding = 6 + ((Match.maxRowsOfAnyColumn - Match.numberOfAllowedRows) * 2);
            int buttonHeight = 35 + ((Match.maxRowsOfAnyColumn - Match.numberOfAllowedRows) * 4);
            int buttonWidth = 70;
            int interBoxPadding = 25 + ((Match.maxRowsOfAnyColumn - Match.numberOfAllowedRows) * 5);
            int interColumnPadding = buttonWidth + 60;
            int totalBoxHeight = buttonHeight * 2 + yPadding + interBoxPadding;

            //Add Y spacing to the boxes as they near the middle column:
            BoxCoords box = new BoxCoords();
            box.TLX = (abstractedColumn * interColumnPadding) - 50;
            box.TLY = totalBoxHeight * rowOfColumn + ((Match.maxRowsOfAnyColumn - Match.numberOfAllowedRows) * 15);

            //Create button for participant 1 of the match.
            System.Windows.Forms.Button newButton = new System.Windows.Forms.Button();
            newButton.Location = new Point(box.TLX, box.TLY);
            newButton.Height = buttonHeight;
            newButton.Width = buttonWidth;
            //newButton.Text = Match.getPlayerNameStringFromDB(passedTeam1id);
            newButton.Text = match.getParticipantNameString(participant1ID);
            Match.buttonList.Add(newButton);
            this.workingVisualizationTab.Controls.Add(newButton);

            //Create button for participant 2 of the match.
            System.Windows.Forms.Button newerButton = new System.Windows.Forms.Button();
            newerButton.Location = new Point(box.TLX, box.TLY + newButton.Height + yPadding);
            newerButton.Height = buttonHeight;
            newerButton.Width = buttonWidth;
            //newerButton.Text = Match.getPlayerNameStringFromDB(passedTeam2id);
            newerButton.Text = match.getParticipantNameString(participant2ID);
            Match.buttonList.Add(newerButton);
            this.workingVisualizationTab.Controls.Add(newerButton);

            //Create events for each button, to be called when the associated button is clicked. 
            newButton.Click += (sender, e) => ParticipantButton_Click(sender, e, participant1ID, match);
            newerButton.Click += (sender, e) => ParticipantButton_Click(sender, e, participant2ID, match);

            box.TRX = box.TLX + newButton.Width;
            box.TRY = box.TLY;
            box.BLX = box.TLX;
            box.BLY = box.TLY + newButton.Height + newerButton.Height + yPadding;
            box.BRX = box.TRX;
            box.BRY = box.BLY;
            box.participant1ID = participant1ID;
            box.participant2ID = participant2ID;
            box.associatedMatchID = match.getId();

            //Determine which direction the arrow should go
            if (abstractedColumn < Match.middleRowNumber)
            {
                box.goDirection = 1;
            }
            else if (abstractedColumn > Match.middleRowNumber)
            {
                box.goDirection = 3;
            }
            else if (abstractedColumn == Match.middleRowNumber)
            {
                box.goDirection = 2;
            }
            else
            {
                box.goDirection = 4;
            }

            box.width = buttonWidth;
            box.height = newButton.Height + newerButton.Height + yPadding;

            boxList.Add(box);
            addPaintHandler();
        }

        public struct BoxCoords
        {
            //Primary struct containing data for coordinates associated with each matchbox.
            //The corners are used to calculate where on the edge of the box the bezier curves should stop. 
            public int TLX, TLY, TRX, TRY, BLX, BLY, BRX, BRY, height, width, associatedMatchID, participant1ID, participant2ID;
            public int goDirection; // 1 = right, 2 = up, 3 = left, 4 = down
            public int winnerID;
            public int winnerBoxY
            {
                get
                {
                    winnerID = MainForm.matchList[this.associatedMatchID - 1].getWinnerId();
                    if (winnerID == this.participant1ID && this.participant1ID != 0)
                    {
                        return this.TRY + (this.height / 4);
                    }
                    else if (winnerID == this.participant2ID && this.participant2ID != 0)
                    {
                        return this.TRY + (3 * (this.height / 4));
                    }
                    else
                    {
                        return this.TRY + (this.height / 2);
                    }
                }
                set
                {
                    winnerBoxY = value;
                }
            }

            public int nextMatch
            {
                get
                {
                    //The ID of the next match that a winner should be moved to. 
                    //This can probably be made way more efficient, and not hard-coded. 
                    int[] hardCodedArrayOfNextMatches = new int[] { -1, 17, 17, 18, 18, 19, 19, 20, 20, 21, 21, 22, 22, 23, 23, 24, 24, 25, 25, 26, 26, 27, 27, 28, 28, 29, 29, 30, 30, 31, 31, 32, 1, 1, 1, 1, 1, 1, 1 };
                    return hardCodedArrayOfNextMatches[associatedMatchID];
                }
            }
        }

        Int32 numberOfTabPagePaintHandlersThatExist = 0;
        public void addPaintHandler()
        {
            //Adds a handler for the painting of the match box once. If handlers are added repeatedly, the program will slow to a crawl, necessitating the following if statement.
            if (numberOfTabPagePaintHandlersThatExist < 1)
            {
                workingVisualizationTab.Paint += TabPage_Paint;
                numberOfTabPagePaintHandlersThatExist++;
            }

        }

        private void ParticipantButton_Click(object sender, EventArgs e, int participantId, Match match)
        {
            //Event handler for a participant button being clicked. 

            string whichTeam = "";
            match.setWinnerOnDB(participantId); //Set the winner of the passed match to the passed ID.
            if ((match.getId() - 1) % 2 == 1) //Determine which match, and which position, the winner should be moved to.
            {
                whichTeam = "team2_id";
            }
            if ((match.getId() - 1) % 2 == 0)
            {
                whichTeam = "team1_id";
            }
            //Update the associated match object. This line is unnecessary since the method called immediately afterwards wipes all matches. Eh whatever. 
            MainForm.matchList[boxList[match.getId() - 1].nextMatch - 1].setParticipantOnDB(participantId, whichTeam);
            EnumerateExisting_Click(sender, e);
        }

        private void TabPage_Paint(object sender, PaintEventArgs e)
        {
            //RECALCULATES (and repaints) LIKE EVERYTHING. Really inefficient. Seriously. It needs double buffering and, you know, to check if components have even changed.
            //It also needs to verify that the coordinates are within range of being drawn, so it doesn't crash due to an OutOfRange exception.

            g = e.Graphics;
            p = new Pen(Color.Black, 4);
            SolidBrush aBrush = new SolidBrush(Color.Black);
            PointF aPoint;
            PointF anotherPoint;
            PointF aPointBetweenBoxes;
            Font aFont = new Font("Arial", 8);
            Font anotherFont = new Font("Arial", 15);
            SolidBrush anotherBrush = new SolidBrush(Color.Orange);
            Font vsFont = new Font("Courier", 8, FontStyle.Italic);

            foreach (BoxCoords thisBox in boxList)
            {
                p.Color = Color.Black;
                p.Width = 4;
                p.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                aPoint = new PointF(thisBox.TLX, thisBox.TLY - 15);


                aPointBetweenBoxes = new PointF((thisBox.TLX + (thisBox.width / 2) - 10), thisBox.TLY + (thisBox.height / 2) - 8);


                Point pointA, pointB, pointC, pointD;

                rect = new Rectangle(thisBox.TLX, thisBox.TLY, thisBox.width, thisBox.height);

                if (MainForm.matchList[thisBox.associatedMatchID - 1].getWinnerId() != 0)
                {
                    //Determines if there is a winner of the match, and if so, changes the pen to be all fancy. 
                    p.Color = Color.Orange;
                    p.Width = 8;
                    p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    p.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                }

                if (thisBox.goDirection == 1)
                {
                    if (thisBox.associatedMatchID % 2 == 1)
                    {
                        //Drawing right-oriented arrow from participant 1 of a match
                        pointA = new Point(thisBox.TRX, thisBox.winnerBoxY);
                        pointB = new Point(boxList[thisBox.nextMatch - 1].TLX - 1, thisBox.winnerBoxY);
                        pointC = new Point(thisBox.TRX, boxList[thisBox.nextMatch - 1].TLY + (boxList[thisBox.nextMatch - 1].height / 4));
                        pointD = new Point(boxList[thisBox.nextMatch - 1].TLX - 1, boxList[thisBox.nextMatch - 1].TLY + (boxList[thisBox.nextMatch - 1].height / 4));
                        g.DrawBezier(p, pointA, pointB, pointC, pointD);
                        //g.DrawLine(p, thisBox.TRX, thisBox.winnerBoxY, boxList[thisBox.nextMatch - 1].TLX - 3, boxList[thisBox.nextMatch - 1].TLY + (thisBox.height / 4));
                    }
                    if (thisBox.associatedMatchID % 2 == 0)
                    {
                        //Drawing right-oriented arrow from participant 2 of a match
                        pointA = new Point(thisBox.BRX, thisBox.winnerBoxY);
                        pointB = new Point(boxList[thisBox.nextMatch - 1].BLX - 1, thisBox.winnerBoxY);
                        pointC = new Point(thisBox.BRX, boxList[thisBox.nextMatch - 1].BLY - (boxList[thisBox.nextMatch - 1].height / 4));
                        pointD = new Point(boxList[thisBox.nextMatch - 1].BLX - 1, boxList[thisBox.nextMatch - 1].BLY - (boxList[thisBox.nextMatch - 1].height / 4));
                        g.DrawBezier(p, pointA, pointB, pointC, pointD);
                        //g.DrawLine(p, thisBox.TRX, thisBox.winnerBoxY, boxList[thisBox.nextMatch - 1].BLX - 3, boxList[thisBox.nextMatch - 1].BLY - (thisBox.height / 4));
                    }

                }
                else if (thisBox.goDirection == 3)
                {
                    if (thisBox.associatedMatchID % 2 == 1)
                    {
                        //Drawing left-oriented arrow from participant 1 of a match
                        pointA = new Point(thisBox.TLX, thisBox.winnerBoxY);
                        pointB = new Point(boxList[thisBox.nextMatch - 1].TRX + 1, thisBox.winnerBoxY);
                        pointC = new Point(thisBox.TLX, boxList[thisBox.nextMatch - 1].TRY + (boxList[thisBox.nextMatch - 1].height / 4));
                        pointD = new Point(boxList[thisBox.nextMatch - 1].TRX + 1, boxList[thisBox.nextMatch - 1].TRY + (boxList[thisBox.nextMatch - 1].height / 4));
                        g.DrawBezier(p, pointA, pointB, pointC, pointD);
                        //g.DrawLine(p, thisBox.TLX, thisBox.winnerBoxY, boxList[thisBox.nextMatch - 1].TRX, boxList[thisBox.nextMatch - 1].TRY + (thisBox.height / 4));
                    }
                    if (thisBox.associatedMatchID % 2 == 0)
                    {
                        //Drawing left-oriented arrow from participant 2 of a match
                        pointA = new Point(thisBox.BLX, thisBox.winnerBoxY);
                        pointB = new Point(boxList[thisBox.nextMatch - 1].BRX + 1, thisBox.winnerBoxY);
                        pointC = new Point(thisBox.BLX, boxList[thisBox.nextMatch - 1].BRY - (boxList[thisBox.nextMatch - 1].height / 4));
                        pointD = new Point(boxList[thisBox.nextMatch - 1].BRX + 1, boxList[thisBox.nextMatch - 1].BRY - (boxList[thisBox.nextMatch - 1].height / 4));
                        g.DrawBezier(p, pointA, pointB, pointC, pointD);
                        //g.DrawLine(p, thisBox.TLX, thisBox.winnerBoxY, boxList[thisBox.nextMatch - 1].BRX, boxList[thisBox.nextMatch - 1].BRY - (thisBox.height / 4));
                    }
                }
                else if (thisBox.goDirection == 2)// && (thisBox.winnerID == thisBox.participant1ID || thisBox.winnerID == thisBox.participant2ID))
                {
                    //Drawing a fairly ridiculous line from the winner box to the championship cup image
                    Int32 cupTLXCoord = thisBox.TLX + (thisBox.width / 2) - 150;
                    Int32 cupTLYCoord = thisBox.TLY - 250;
                    p.Width = 16;
                    g.DrawLine(p, thisBox.TLX + (thisBox.width / 2), thisBox.winnerBoxY, thisBox.TLX + (thisBox.width / 2), thisBox.TLY - 100);
                    anotherPoint = new PointF(thisBox.TLX - 10, thisBox.TLY - 125);
                    //g.DrawString(Participant.getNameString(thisBox.winnerID), anotherFont, anotherBrush, anotherPoint);
                    //g.DrawImage(Image.FromFile(directory + "\\other\\" + "champions_cup_2.png"), cupTLXCoord, cupTLYCoord);
                    //g.DrawBezier(p, pointA, pointB, pointC, pointD);
                }

                //Draw the match ID and vs. text.
                p.Color = Color.Black;
                p.Width = 4;
                g.DrawRectangle(p, rect);
                aBrush.Color = Color.Black;
                g.DrawString("id: " + thisBox.associatedMatchID, aFont, aBrush, aPoint);
                aBrush.Color = Color.Red;
                g.DrawString("vs.", vsFont, aBrush, aPointBetweenBoxes);

            }
        }

        protected override void OnPaint(PaintEventArgs paintEvnt)
        {
            //Update the unreliable little debug textbox at the top of the form which shows the RAM usage and CPU time total. 
            this.textBox1.Text = "MEM: " + MainForm.proc.PrivateMemorySize64 / 1024 + " KiB" + "\tTOTALPROCTIME: " + proc.TotalProcessorTime;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.dataEntryTab = new System.Windows.Forms.TabPage();
            this.quickStartLabel = new System.Windows.Forms.Label();
            this.quickStartRichTextBox = new System.Windows.Forms.RichTextBox();
            this.RefreshDatabaseListButton = new System.Windows.Forms.Button();
            this.dbSelectionComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AddNewPlayersButton = new System.Windows.Forms.Button();
            this.workingVisualizationTab = new System.Windows.Forms.TabPage();
            this.EnumerateExisting = new System.Windows.Forms.Button();
            this.databaseManagementTab = new System.Windows.Forms.TabPage();
            this.updateButton = new System.Windows.Forms.Button();
            this.LoadParticipantsButton = new System.Windows.Forms.Button();
            this.LoadMatchesButton = new System.Windows.Forms.Button();
            this.mainDataGridView = new System.Windows.Forms.DataGridView();
            this.namen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Wins = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Losses = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Organization = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Contact = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTournamentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTournamentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createGameTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomNumberGenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundRecorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.officialPlayerDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameRulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basketballToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.baseballToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.footballToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leagueOfLegendsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dotAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dotA2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applicationGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.dataEntryTab.SuspendLayout();
            this.workingVisualizationTab.SuspendLayout();
            this.databaseManagementTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainDataGridView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.dataEntryTab);
            this.tabControl1.Controls.Add(this.workingVisualizationTab);
            this.tabControl1.Controls.Add(this.databaseManagementTab);
            this.tabControl1.Location = new System.Drawing.Point(9, 26);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(820, 376);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // dataEntryTab
            // 
            this.dataEntryTab.Controls.Add(this.quickStartLabel);
            this.dataEntryTab.Controls.Add(this.quickStartRichTextBox);
            this.dataEntryTab.Controls.Add(this.RefreshDatabaseListButton);
            this.dataEntryTab.Controls.Add(this.dbSelectionComboBox);
            this.dataEntryTab.Controls.Add(this.label1);
            this.dataEntryTab.Controls.Add(this.AddNewPlayersButton);
            this.dataEntryTab.Location = new System.Drawing.Point(4, 22);
            this.dataEntryTab.Name = "dataEntryTab";
            this.dataEntryTab.Size = new System.Drawing.Size(812, 350);
            this.dataEntryTab.TabIndex = 2;
            this.dataEntryTab.Text = "Data Entry";
            this.dataEntryTab.UseVisualStyleBackColor = true;
            this.dataEntryTab.Click += new System.EventHandler(this.tabPage3_Click);
            // 
            // quickStartLabel
            // 
            this.quickStartLabel.AutoSize = true;
            this.quickStartLabel.Location = new System.Drawing.Point(8, 80);
            this.quickStartLabel.Name = "quickStartLabel";
            this.quickStartLabel.Size = new System.Drawing.Size(94, 13);
            this.quickStartLabel.TabIndex = 22;
            this.quickStartLabel.Text = "Quick Start Guide:";
            // 
            // quickStartRichTextBox
            // 
            this.quickStartRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.quickStartRichTextBox.Location = new System.Drawing.Point(11, 96);
            this.quickStartRichTextBox.Name = "quickStartRichTextBox";
            this.quickStartRichTextBox.Size = new System.Drawing.Size(788, 242);
            this.quickStartRichTextBox.TabIndex = 21;
            this.quickStartRichTextBox.Text = resources.GetString("quickStartRichTextBox.Text");
            this.quickStartRichTextBox.TextChanged += new System.EventHandler(this.quickStartRichTextBox_TextChanged);
            // 
            // RefreshDatabaseListButton
            // 
            this.RefreshDatabaseListButton.Location = new System.Drawing.Point(11, 33);
            this.RefreshDatabaseListButton.Margin = new System.Windows.Forms.Padding(2);
            this.RefreshDatabaseListButton.Name = "RefreshDatabaseListButton";
            this.RefreshDatabaseListButton.Size = new System.Drawing.Size(131, 24);
            this.RefreshDatabaseListButton.TabIndex = 20;
            this.RefreshDatabaseListButton.Text = "Refresh Database List";
            this.RefreshDatabaseListButton.UseVisualStyleBackColor = true;
            this.RefreshDatabaseListButton.Click += new System.EventHandler(this.RefreshDatabaseListButtonClick);
            // 
            // dbSelectionComboBox
            // 
            this.dbSelectionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbSelectionComboBox.FormattingEnabled = true;
            this.dbSelectionComboBox.Location = new System.Drawing.Point(74, 8);
            this.dbSelectionComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.dbSelectionComboBox.Name = "dbSelectionComboBox";
            this.dbSelectionComboBox.Size = new System.Drawing.Size(725, 21);
            this.dbSelectionComboBox.TabIndex = 19;
            this.dbSelectionComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Current DB:";
            // 
            // AddNewPlayersButton
            // 
            this.AddNewPlayersButton.Location = new System.Drawing.Point(146, 33);
            this.AddNewPlayersButton.Margin = new System.Windows.Forms.Padding(2);
            this.AddNewPlayersButton.Name = "AddNewPlayersButton";
            this.AddNewPlayersButton.Size = new System.Drawing.Size(131, 24);
            this.AddNewPlayersButton.TabIndex = 14;
            this.AddNewPlayersButton.Text = "Add New Players";
            this.AddNewPlayersButton.UseVisualStyleBackColor = true;
            this.AddNewPlayersButton.Click += new System.EventHandler(this.AddNewParticipantsButtonClick);
            // 
            // workingVisualizationTab
            // 
            this.workingVisualizationTab.AutoScroll = true;
            this.workingVisualizationTab.Controls.Add(this.EnumerateExisting);
            this.workingVisualizationTab.Location = new System.Drawing.Point(4, 22);
            this.workingVisualizationTab.Name = "workingVisualizationTab";
            this.workingVisualizationTab.Size = new System.Drawing.Size(812, 350);
            this.workingVisualizationTab.TabIndex = 3;
            this.workingVisualizationTab.Text = "Bracket Visualization";
            this.workingVisualizationTab.UseVisualStyleBackColor = true;
            // 
            // EnumerateExisting
            // 
            this.EnumerateExisting.Location = new System.Drawing.Point(14, 13);
            this.EnumerateExisting.Margin = new System.Windows.Forms.Padding(2);
            this.EnumerateExisting.Name = "EnumerateExisting";
            this.EnumerateExisting.Size = new System.Drawing.Size(112, 23);
            this.EnumerateExisting.TabIndex = 5;
            this.EnumerateExisting.Text = "Enumerate Existing (CAUTION)";
            this.EnumerateExisting.UseVisualStyleBackColor = true;
            this.EnumerateExisting.Click += new System.EventHandler(this.EnumerateExisting_Click);
            // 
            // databaseManagementTab
            // 
            this.databaseManagementTab.Controls.Add(this.updateButton);
            this.databaseManagementTab.Controls.Add(this.LoadParticipantsButton);
            this.databaseManagementTab.Controls.Add(this.LoadMatchesButton);
            this.databaseManagementTab.Controls.Add(this.mainDataGridView);
            this.databaseManagementTab.Location = new System.Drawing.Point(4, 22);
            this.databaseManagementTab.Margin = new System.Windows.Forms.Padding(2);
            this.databaseManagementTab.Name = "databaseManagementTab";
            this.databaseManagementTab.Padding = new System.Windows.Forms.Padding(2);
            this.databaseManagementTab.Size = new System.Drawing.Size(812, 350);
            this.databaseManagementTab.TabIndex = 1;
            this.databaseManagementTab.Text = "View/Manage Stats";
            this.databaseManagementTab.UseVisualStyleBackColor = true;
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(224, 5);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(125, 23);
            this.updateButton.TabIndex = 0;
            this.updateButton.Text = "Push Changes to DB";
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // LoadParticipantsButton
            // 
            this.LoadParticipantsButton.Location = new System.Drawing.Point(103, 5);
            this.LoadParticipantsButton.Name = "LoadParticipantsButton";
            this.LoadParticipantsButton.Size = new System.Drawing.Size(115, 23);
            this.LoadParticipantsButton.TabIndex = 6;
            this.LoadParticipantsButton.Text = "Load Participants";
            this.LoadParticipantsButton.UseVisualStyleBackColor = true;
            this.LoadParticipantsButton.Click += new System.EventHandler(this.LoadParticipantsButtonClick);
            // 
            // LoadMatchesButton
            // 
            this.LoadMatchesButton.Location = new System.Drawing.Point(4, 5);
            this.LoadMatchesButton.Margin = new System.Windows.Forms.Padding(2);
            this.LoadMatchesButton.Name = "LoadMatchesButton";
            this.LoadMatchesButton.Size = new System.Drawing.Size(94, 23);
            this.LoadMatchesButton.TabIndex = 5;
            this.LoadMatchesButton.Text = "Load Matches";
            this.LoadMatchesButton.UseVisualStyleBackColor = true;
            this.LoadMatchesButton.Click += new System.EventHandler(this.LoadMatchesButtonClick);
            // 
            // mainDataGridView
            // 
            this.mainDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.mainDataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.mainDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mainDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.namen,
            this.Wins,
            this.Losses,
            this.Organization,
            this.Contact});
            this.mainDataGridView.Location = new System.Drawing.Point(4, 33);
            this.mainDataGridView.Name = "mainDataGridView";
            this.mainDataGridView.Size = new System.Drawing.Size(803, 312);
            this.mainDataGridView.TabIndex = 0;
            this.mainDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // namen
            // 
            this.namen.HeaderText = "Name";
            this.namen.Name = "namen";
            // 
            // Wins
            // 
            this.Wins.HeaderText = "Wins";
            this.Wins.Name = "Wins";
            // 
            // Losses
            // 
            this.Losses.HeaderText = "Losses";
            this.Losses.Name = "Losses";
            // 
            // Organization
            // 
            this.Organization.HeaderText = "Organization";
            this.Organization.Name = "Organization";
            // 
            // Contact
            // 
            this.Contact.HeaderText = "Contact (Email)";
            this.Contact.Name = "Contact";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(836, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTournamentToolStripMenuItem,
            this.openTournamentToolStripMenuItem,
            this.createGameTemplateToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // newTournamentToolStripMenuItem
            // 
            this.newTournamentToolStripMenuItem.Name = "newTournamentToolStripMenuItem";
            this.newTournamentToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.newTournamentToolStripMenuItem.Text = "New Tournament";
            this.newTournamentToolStripMenuItem.Click += new System.EventHandler(this.newTournamentToolStripMenuItem_Click);
            // 
            // openTournamentToolStripMenuItem
            // 
            this.openTournamentToolStripMenuItem.Name = "openTournamentToolStripMenuItem";
            this.openTournamentToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.openTournamentToolStripMenuItem.Text = "Open Tournament";
            this.openTournamentToolStripMenuItem.Click += new System.EventHandler(this.openTournamentToolStripMenuItem_Click);
            // 
            // createGameTemplateToolStripMenuItem
            // 
            this.createGameTemplateToolStripMenuItem.Name = "createGameTemplateToolStripMenuItem";
            this.createGameTemplateToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.createGameTemplateToolStripMenuItem.Text = "Create Game Template";
            this.createGameTemplateToolStripMenuItem.Click += new System.EventHandler(this.createGameTemplateToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calculatorToolStripMenuItem,
            this.soundboardToolStripMenuItem,
            this.randomNumberGenToolStripMenuItem,
            this.soundRecorderToolStripMenuItem,
            this.timerToolStripMenuItem,
            this.officialPlayerDataToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 22);
            this.toolsToolStripMenuItem.Text = "Tools";
            this.toolsToolStripMenuItem.Click += new System.EventHandler(this.toolsToolStripMenuItem_Click);
            // 
            // calculatorToolStripMenuItem
            // 
            this.calculatorToolStripMenuItem.Name = "calculatorToolStripMenuItem";
            this.calculatorToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.calculatorToolStripMenuItem.Text = "Calculator";
            this.calculatorToolStripMenuItem.Click += new System.EventHandler(this.calculatorToolStripMenuItem_Click);
            // 
            // soundboardToolStripMenuItem
            // 
            this.soundboardToolStripMenuItem.Name = "soundboardToolStripMenuItem";
            this.soundboardToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.soundboardToolStripMenuItem.Text = "Soundboard";
            this.soundboardToolStripMenuItem.Click += new System.EventHandler(this.soundboardToolStripMenuItem_Click);
            // 
            // randomNumberGenToolStripMenuItem
            // 
            this.randomNumberGenToolStripMenuItem.Name = "randomNumberGenToolStripMenuItem";
            this.randomNumberGenToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.randomNumberGenToolStripMenuItem.Text = "Random Number Generator";
            this.randomNumberGenToolStripMenuItem.Click += new System.EventHandler(this.randomNumberGenToolStripMenuItem_Click);
            // 
            // soundRecorderToolStripMenuItem
            // 
            this.soundRecorderToolStripMenuItem.Name = "soundRecorderToolStripMenuItem";
            this.soundRecorderToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.soundRecorderToolStripMenuItem.Text = "Sound Recorder (NYI)";
            this.soundRecorderToolStripMenuItem.Click += new System.EventHandler(this.soundRecorderToolStripMenuItem_Click);
            // 
            // timerToolStripMenuItem
            // 
            this.timerToolStripMenuItem.Name = "timerToolStripMenuItem";
            this.timerToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.timerToolStripMenuItem.Text = "Timer (NYI)";
            // 
            // officialPlayerDataToolStripMenuItem
            // 
            this.officialPlayerDataToolStripMenuItem.Name = "officialPlayerDataToolStripMenuItem";
            this.officialPlayerDataToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.officialPlayerDataToolStripMenuItem.Text = "Official Participant Data (NYI)";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameRulesToolStripMenuItem,
            this.applicationGuideToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // gameRulesToolStripMenuItem
            // 
            this.gameRulesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basketballToolStripMenuItem,
            this.baseballToolStripMenuItem,
            this.footballToolStripMenuItem,
            this.leagueOfLegendsToolStripMenuItem,
            this.dotAToolStripMenuItem,
            this.dotA2ToolStripMenuItem});
            this.gameRulesToolStripMenuItem.Name = "gameRulesToolStripMenuItem";
            this.gameRulesToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.gameRulesToolStripMenuItem.Text = "Game Rules";
            // 
            // basketballToolStripMenuItem
            // 
            this.basketballToolStripMenuItem.Name = "basketballToolStripMenuItem";
            this.basketballToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.basketballToolStripMenuItem.Text = "Basketball";
            this.basketballToolStripMenuItem.Click += new System.EventHandler(this.basketballToolStripMenuItem_Click);
            // 
            // baseballToolStripMenuItem
            // 
            this.baseballToolStripMenuItem.Name = "baseballToolStripMenuItem";
            this.baseballToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.baseballToolStripMenuItem.Text = "Baseball";
            this.baseballToolStripMenuItem.Click += new System.EventHandler(this.baseballToolStripMenuItem_Click);
            // 
            // footballToolStripMenuItem
            // 
            this.footballToolStripMenuItem.Name = "footballToolStripMenuItem";
            this.footballToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.footballToolStripMenuItem.Text = "Football";
            this.footballToolStripMenuItem.Click += new System.EventHandler(this.footballToolStripMenuItem_Click);
            // 
            // leagueOfLegendsToolStripMenuItem
            // 
            this.leagueOfLegendsToolStripMenuItem.Name = "leagueOfLegendsToolStripMenuItem";
            this.leagueOfLegendsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.leagueOfLegendsToolStripMenuItem.Text = "League of Legends (NYI)";
            // 
            // dotAToolStripMenuItem
            // 
            this.dotAToolStripMenuItem.Name = "dotAToolStripMenuItem";
            this.dotAToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.dotAToolStripMenuItem.Text = "DotA (NYI)";
            // 
            // dotA2ToolStripMenuItem
            // 
            this.dotA2ToolStripMenuItem.Name = "dotA2ToolStripMenuItem";
            this.dotA2ToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.dotA2ToolStripMenuItem.Text = "DotA 2 (NYI)";
            // 
            // applicationGuideToolStripMenuItem
            // 
            this.applicationGuideToolStripMenuItem.Name = "applicationGuideToolStripMenuItem";
            this.applicationGuideToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.applicationGuideToolStripMenuItem.Text = "Application Guide (NYI)";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(339, 1);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(490, 20);
            this.textBox1.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(836, 408);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "SPSU Tournament Manager 2014 Pre-Alpha";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.dataEntryTab.ResumeLayout(false);
            this.dataEntryTab.PerformLayout();
            this.workingVisualizationTab.ResumeLayout(false);
            this.databaseManagementTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainDataGridView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage databaseManagementTab;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newTournamentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTournamentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calculatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem officialPlayerDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TabPage dataEntryTab;
        private System.Windows.Forms.DataGridView mainDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn namen;
        private System.Windows.Forms.DataGridViewTextBoxColumn Wins;
        private System.Windows.Forms.DataGridViewTextBoxColumn Losses;
        private System.Windows.Forms.DataGridViewTextBoxColumn Organization;
        private System.Windows.Forms.DataGridViewTextBoxColumn Contact;
        private System.Windows.Forms.ToolStripMenuItem timerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soundboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gameRulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem basketballToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem baseballToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem footballToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leagueOfLegendsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dotAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dotA2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applicationGuideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createGameTemplateToolStripMenuItem;
        private System.Windows.Forms.Button AddNewPlayersButton;
        private System.Windows.Forms.ComboBox dbSelectionComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button RefreshDatabaseListButton;
        private System.Windows.Forms.ToolStripMenuItem soundRecorderToolStripMenuItem;
        private System.Windows.Forms.Button LoadMatchesButton;
        private System.Windows.Forms.TabPage workingVisualizationTab;
        private System.Windows.Forms.Button LoadParticipantsButton;
        private System.Windows.Forms.Button EnumerateExisting;
        private TextBox textBox1;
        private Button updateButton;
        private ToolStripMenuItem randomNumberGenToolStripMenuItem;
        private Label quickStartLabel;
        private RichTextBox quickStartRichTextBox;
    }
}

