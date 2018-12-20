using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StudiTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            TestProvider.Tests.Select(i => i).ToList().ForEach(i =>
            {
                var cms = new ToolStripMenuItem(i.Value, null, (a, b) => { textBox3.Text = i.Value; textBox3.Tag = i.Key; })
                {
                    BackColor = Color.FromArgb(0x34495e),
                    Font = Font
                };
                contextMenuStrip1.Items.Add(cms);
            });
            contextMenuStrip1.Show(MousePosition);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Tag != null)
            {
                var f = new Form2(textBox3.Tag.ToString()).ShowDialog();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = textBox3.TextLength > 0;
        }
    }
}
