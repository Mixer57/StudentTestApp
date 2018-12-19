using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TestApp;

namespace TestBuilder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var q = new Question(qstTitle.Text, qstText.Text,
                valType.SelectedIndex == 0 ? Question.QValueEnum.Text : Question.QValueEnum.Image,
                ansType.SelectedIndex == 0 ? Question.QAnswerEnum.Radio : Question.QAnswerEnum.Check);

            for (var i = 1; i < 7; i++)
            {
                var val = groupBox3.Controls.Find($"ans{i}", true).OfType<RichTextBox>().FirstOrDefault()?.Rtf;
                var ira = groupBox3.Controls.Find($"en{i}", true).OfType<CheckBox>().FirstOrDefault()?.Checked ?? false;

                if (string.IsNullOrEmpty(val)) continue;

                if (q.ValueType == Question.QValueEnum.Text)
                    q.AddAnswer(val);
                else
                    q.AddAnswer(Image.FromFile(val));

                if (ira)
                    q.RightAnswerId = q.RightAnswerId.Concat(new[] {(byte)i}).ToArray();
            }


            qpQuestions.Items.Add(q);

            ans1.Text = ans2.Text = ans3.Text = ans4.Text = ans5.Text = ans6.Text = "";
            en1.Checked = en2.Checked = en3.Checked = en4.Checked = en5.Checked = en6.Checked = false;
            qstTitle.Text = qstText.Text = "";
            valType.SelectedIndex = ansType.SelectedIndex = -1;

            groupBox2.Text = $@"Список вопросов теста: {qpQuestions.Items.Count}";
            numericUpDown2.Value = numericUpDown1.Value * qpQuestions.Items.Count;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            qpQuestions.Items.RemoveAt(qpQuestions.SelectedIndex);
            groupBox2.Text = $@"Список вопросов теста: {qpQuestions.Items.Count}";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var qp = new QuestionPack
            {
                Author = qpAuthor.Text,
                Title = qpTitle.Text,
                Discipline = qpDis.Text,
                Theme = qpTheme.Text,
                Questions = new List<Question>(qpQuestions.Items.Cast<Question>().ToArray()),
                TimeToAnswer = (int) numericUpDown1.Value,
                TimeToTest = (int) numericUpDown2.Value
            };

            using (var sf = new SaveFileDialog { Filter = @"Question pack|*.qst", InitialDirectory = Directory.GetCurrentDirectory() })
                if (sf.ShowDialog() == DialogResult.OK)
                    qp.ToFile(sf.FileName);
        }

        private void цветТекстаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                Enumerable.Range(1, 6).ToList().ForEach(i =>
                {
                    var rtb = groupBox3.Controls.Find("ans" + i, true).OfType<RichTextBox>()
                        .FirstOrDefault(c => c.SelectionLength > 0);
                    if (rtb != null)
                        rtb.SelectionColor = colorDialog1.Color;
                });
            }
        }

        private void цветФонаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                Enumerable.Range(1, 6).ToList().ForEach(i =>
                {
                    var rtb = groupBox3.Controls.Find("ans" + i, true).OfType<RichTextBox>()
                        .FirstOrDefault(c => c.SelectionLength > 0);
                    if (rtb != null)
                        rtb.SelectionBackColor = colorDialog1.Color;
                });
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
}
