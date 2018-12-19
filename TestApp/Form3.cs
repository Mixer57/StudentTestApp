using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form3 : Form
    {
        private string file;
        public Form3()
        {
            InitializeComponent();
        }
        public Form3(string path)
        {
            InitializeComponent();
            file = path;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            if (!File.Exists(file) || Path.GetExtension(file) != ".ans") { Close(); return; }

            File.ReadAllText(file)
                .Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .ForEach(a =>
            {
                if (string.IsNullOrEmpty(a)) return;
                var spl = a.Trim().Split('\n').Select(c => c.Trim()).ToArray();
                if (spl.Length != 8) return;

                var i = "";
                for (int z = 0; z < 7; z++)
                {
                    if (z == 5) continue;
                    var v = spl[z].Split(new[] { ':' }, 2)[1].Trim();
                    i += v + "\0";
                }
                listView1.Items.Add(new ListViewItem(i.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries)));
            });

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
}
