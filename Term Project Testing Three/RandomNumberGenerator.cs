using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Term_Project_Testing_Three
{
    public partial class RandomNumberGenerator : Form
    {
        public RandomNumberGenerator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 val1 = Int32.Parse(textBox1.Text);
                Int32 val2 = Int32.Parse(textBox2.Text) + 1;
                Random random = new Random();
                Int32 result = random.Next(val1, val2);
                listBox1.Items.Insert(0, textBox3.Text);
                textBox3.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
