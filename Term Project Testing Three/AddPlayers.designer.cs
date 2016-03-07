namespace Term_Project_Testing_Three
{
    partial class AddParticipants
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.refreshParticipantsButton = new System.Windows.Forms.Button();
            this.btnSub = new System.Windows.Forms.Button();
            this.refreshMatchesButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(13, 13);
            this.dataGridView1.Name = "mainDataGridView";
            this.dataGridView1.Size = new System.Drawing.Size(742, 415);
            this.dataGridView1.TabIndex = 0;
            // 
            // refreshParticipantsButton
            // 
            this.refreshParticipantsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.refreshParticipantsButton.Location = new System.Drawing.Point(13, 434);
            this.refreshParticipantsButton.Name = "refreshParticipantsButton";
            this.refreshParticipantsButton.Size = new System.Drawing.Size(116, 23);
            this.refreshParticipantsButton.TabIndex = 1;
            this.refreshParticipantsButton.Text = "Refresh Participants";
            this.refreshParticipantsButton.UseVisualStyleBackColor = true;
            this.refreshParticipantsButton.Click += new System.EventHandler(this.refreshParticipantsButtonClick);
            // 
            // btnSub
            // 
            this.btnSub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSub.Location = new System.Drawing.Point(680, 434);
            this.btnSub.Name = "btnSub";
            this.btnSub.Size = new System.Drawing.Size(75, 23);
            this.btnSub.TabIndex = 2;
            this.btnSub.Text = "Submit";
            this.btnSub.UseVisualStyleBackColor = true;
            this.btnSub.Click += new System.EventHandler(this.btnSub_Click);
            // 
            // refreshMatchesButton
            // 
            this.refreshMatchesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.refreshMatchesButton.Location = new System.Drawing.Point(135, 434);
            this.refreshMatchesButton.Name = "refreshMatchesButton";
            this.refreshMatchesButton.Size = new System.Drawing.Size(116, 23);
            this.refreshMatchesButton.TabIndex = 3;
            this.refreshMatchesButton.Text = "Refresh Matches";
            this.refreshMatchesButton.UseVisualStyleBackColor = true;
            this.refreshMatchesButton.Click += new System.EventHandler(this.refreshMatchesButtonClick);
            // 
            // AddPlayers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 465);
            this.Controls.Add(this.refreshMatchesButton);
            this.Controls.Add(this.btnSub);
            this.Controls.Add(this.refreshParticipantsButton);
            this.Controls.Add(this.dataGridView1);
            this.Name = "AddParticipants";
            this.Text = "Add Players";
            this.Load += new System.EventHandler(this.AddPlayers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button refreshParticipantsButton;
        private System.Windows.Forms.Button btnSub;
        private System.Windows.Forms.Button refreshMatchesButton;
    }
}

