using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StudiTest
{
    public partial class Form2 : Form
    {
        public Test Test { get; private set; }
        private int curid = 0;

        public Form2(string testFile)
        {
            InitializeComponent();
            Test = TestProvider.LoadTest(testFile);
            titled.Text = Test.ToString();
            CreateButtons();
            LoadQuestion(0);
        }

        private void CreateButtons()
        {
            qBtns.Controls.Clear();
            Enumerable.Range(0, Test.Questions.Count).ToList().ForEach(i =>
            {
                var btn = new Button
                {
                    Text = (i + 1).ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI Semibold", 9.75f),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Dock = DockStyle.Left,
                    Name = "qstID" + i,
                    Tag = i,
                    Width = 30,
                    Height = 26,
                };
                btn.Click += (s, a) => { if (Test.Answers[(int)((Button)s).Tag]?.Length == 0) LoadQuestion((int)((Button)s).Tag); };
                qBtns.Controls.Add(btn);
                btn.BringToFront();
            });
        }

        private void LoadQuestion(int tag)
        {
            curid = tag;
            var q = Test.Questions[tag];
            qstIDl.Text = $"Вопрос №: {tag + 1}";
            qstTitle.Text = q.Title;
            qstText.Text = q.Text;
            var mtw = 0f;
            var g = listViewEx1.CreateGraphics();
            var itms = q.Answers.Select(i =>
            {
                mtw = Math.Max(mtw, g.MeasureString((string)i.Value, listViewEx1.Font).Height);
                return new ListViewItem((string)i.Value);
            }).ToArray();
            listViewEx1 = new ListViewEx
            {
                RowHeight = Math.Max(20, (int)mtw),
                CheckBoxes = q.Selection == Test.Question.SelectionEnum.many,
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                HideSelection = false,
                View  = View.Details,
                HeaderStyle = ColumnHeaderStyle.None,
                BackColor = BackColor,
                ForeColor = ForeColor
            };
            listViewEx1.Columns.Add((ColumnHeader)columnHeader1.Clone());
            listViewEx1.Items.AddRange(itms);
            panel5.Controls.Clear();
            panel5.Controls.Add(listViewEx1);
            Application.DoEvents();
            listViewEx1.Columns[0].Width = listViewEx1.Width - SystemInformation.VerticalScrollBarWidth;
            listViewEx1.SizeChanged += (a, b) => listViewEx1.Columns[0].Width = listViewEx1.Width - SystemInformation.VerticalScrollBarWidth;
            next.Enabled = true;
        }

        private void listViewEx1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void next_Click(object sender, EventArgs e)
        {
            next.Enabled = false;
            Test.AddAnswer(curid, listViewEx1.CheckBoxes ? listViewEx1.CheckedItems.Cast<int>().ToArray() : listViewEx1.SelectedIndices.Cast<int>().ToArray());
            var nxt = -1;
            Enumerable.Range(0, Test.Answers.Length).ToList().ForEach(i => { if ((Test.Answers[i] == null || Test.Answers[i].Length == 0) & nxt == -1) nxt = i; });
            if (nxt == -1)
                Close();
            else
                LoadQuestion(nxt);
        }
    }
}
