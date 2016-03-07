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

namespace Term_Project_Testing_Three
{
    public partial class SoundBoard : Form
    {
        
        public SoundBoard()
        {
            InitializeComponent();
            this.listBox1.MouseDoubleClick += new MouseEventHandler(listBox1_MouseDoubleClick);
        }

        private void SoundBoard_Load(object sender, EventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                System.Diagnostics.Debug.Write("\nSelected Item: " + listBox1.SelectedItem.ToString());
                axWindowsMediaPlayer1.URL = listBox1.SelectedItem.ToString();
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string directory = Directory.GetCurrentDirectory();
            var wavFiles = Directory.GetFiles(directory + "\\sounds\\", "*.wav").ToList();
            var mp3Files = Directory.GetFiles(directory + "\\sounds\\", "*.mp3").ToList();
            //Credit to Daniel White on StackOverflow at http://stackoverflow.com/questions/163162/can-you-call-directory-getfiles-with-multiple-filters
            //var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp3") || s.EndsWith(".wav"));
            foreach (string item in wavFiles)
            {
                listBox1.Items.Add(item);
            }
            foreach (string item in mp3Files)
            {
                listBox1.Items.Add(item);
            }
        }
    }
}
