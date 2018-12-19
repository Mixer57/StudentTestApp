using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form2 : Form
    {
        private int _toAnswerCounter;
        private int _idx;
        private readonly Timer _toTest = new Timer();
        private readonly Timer _toAnswer = new Timer();
        public Dictionary<int,double> Answers =  new Dictionary<int, double>();
        public QuestionPack Qp { get; set; }
        public int ToTestCounter;
        public DateTime Started;
        public DateTime Ended;

        public Form2()
        {
            InitializeComponent();
            Started = DateTime.Now;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            groupBox3.Visible = Qp.TimeToAnswer > 0 | Qp.TimeToTest > 0;
            titled.Text = $"Тест: \"{Qp.Title}\" по \"{Qp.Discipline}\"  (Автор: {Qp.Author})";
            
            PlaceQuestionButtons();

            LoadQuestion(_idx);

            _toAnswer.Interval = 1000;

            if (Qp.TimeToTest <= 0) return;

            _toTest.Interval = 1000;
            _toTest.Tick += ToTestOnTick;
            ToTestCounter = Qp.TimeToTest;
            tmrTest.Maximum = ToTestCounter;
            tmrTest.Value = ToTestCounter;
            _toTest.Start();
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Ended = DateTime.Now;
        }

        private void Rdm(object sender, EventArgs e)
        {
            var vars = Enumerable.Range(0, Qp.Questions.Count).Except(Answers.Keys.ToArray()).ToArray();
            var rnd = new Random();
            LoadQuestion(vars[rnd.Next(0,vars.Length)]);
        }
        private void next_Click(object sender, EventArgs e)
        {
            _toTest.Stop();
            _toAnswer.Stop();


            var ans= ansBox.Controls.OfType<Answer>().Where(i => i.Checked).Select(z => (byte)(z.Idx+1)).ToArray();
            var scr = Qp.Questions[_idx].GetAnswerScores(ans);
            Answers.Add(_idx, scr);

            RescanAnswerButtons();

            ansBox.Controls.Clear();

            if (Answers.Count < Qp.Questions.Count)
            {
             var idz= Enumerable.Range(0, Qp.Questions.Count).Except(Answers.Keys.ToArray()).FirstOrDefault();
                if (idz != -1)
                    LoadQuestion(idz);
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            next.Enabled = false;
            _toTest.Start();
        }
        private void ToTestOnTick(object sender, EventArgs e)
        {
            if (ToTestCounter == 0)
            {
                Enumerable.Range(Answers.Count, Qp.Questions.Count - Answers.Count).ToList()
                    .ForEach(i => Answers.Add(i,0f));

                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                ToTestCounter--;
                tmrTest.Value = ToTestCounter;
            }
            label3.Text = $@"{ToTestCounter}c.";
        }
        private void ToAnswerOnTick(object sender, EventArgs e)
        {
            if (_toAnswerCounter == 0)
                next_Click(null,null);
            else
            {
                _toAnswerCounter--;
                tmrAns.Value = _toAnswerCounter;
            }
            label2.Text = $@"{_toAnswerCounter}c.";
        }

        private void RescanAnswerButtons()
        {
            Enumerable.Range(0, Qp.Questions.Count).ToList().ForEach(i =>
            {
                var btn = qBtns.Controls.Find("qstID" + i, true).FirstOrDefault();
                if (btn != null)
                    btn.Enabled = !Answers.ContainsKey(i);
            });
        }
        private void PlaceQuestionButtons()
        {
            Enumerable.Range(0, Qp.Questions.Count).ToList().ForEach(i =>
            {
                var btn = new Button
                {
                    Text = (i+1).ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font= new Font("Segoe UI Semibold", 9.75f),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = {BorderSize = 0},
                    Dock = DockStyle.Left,
                    Name = "qstID" + i,
                    Tag = i,
                    Width=30,
                    Height=26,
                };
                btn.Click += (s, a) => { if(!Answers.ContainsKey((int)((Button)s).Tag)) LoadQuestion((int)((Button)s).Tag); };
                qBtns.Controls.Add(btn);
                btn.BringToFront();
            });
        }
        private void LoadQuestion(int id)
        {
            var q = Qp.Questions[id];
            qstTitle.Text = q.Title;
            qstText.Text = q.Text;
            qstIDl.Text = $@"Вопрос № {id+1} :";
            _idx = id;
            ansBox.Visible = false;
            ansBox.Controls.Clear();

            for (var i = 0; i < q.Answers.Length; i++)
            {
                var ans = new Answer
                {
                    Idx = i,
                    Dock = DockStyle.Fill,
                    ValueType = q.ValueType,
                    AnswerType = q.AnswerType,
                    Font = ansBox.Font
                };

                if (q.ValueType == Question.QValueEnum.Text)
                    ans.ValueTxt = q.Answers[i].ToString();
                else
                    ans.ValueImg = (Image) q.Answers[i];

                ansBox.Controls.Add(ans);

                ans.CheckedChanged += Ans_CheckedChanged;
            }

            ansBox.Visible = true;
            ansBox.Select();

            if (Qp.TimeToAnswer <= 0) return;

            _toAnswer.Stop();
            _toAnswer.Tick -= ToAnswerOnTick;
            _toAnswer.Interval = 1000;
            _toAnswerCounter = Qp.TimeToAnswer;
            _toAnswer.Tick += ToAnswerOnTick;
            tmrAns.Maximum = _toAnswerCounter;
            tmrAns.Value = _toAnswerCounter;
            _toAnswer.Start();
        }

        private void Ans_CheckedChanged(int idx, bool check)
        {
            next.Enabled = ansBox.Controls.OfType<Answer>().Count(i => i.Checked) > 0;

            if (Qp.Questions[_idx].AnswerType == Question.QAnswerEnum.Radio)
            {
                ansBox.Controls.OfType<Answer>().ToList().ForEach(i=>
                {
                    if (i.Idx != idx)
                        i.Checked = false;
                });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void fx1_Click(object sender, EventArgs e)
        {
            qstIDl.Font = qstTitle.Font = qstText.Font = new Font("Segoe UI", 8.25f, FontStyle.Bold);
            ansBox.Font = new Font("Segoe UI", 8.25f);
            ansBox.Controls.OfType<Answer>().ToList().ForEach(c => c.Font = ansBox.Font);
        }

        private void fx2_Click(object sender, EventArgs e)
        {
            qstIDl.Font = qstTitle.Font = qstText.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            ansBox.Font = new Font("Segoe UI", 9.75f);
            ansBox.Controls.OfType<Answer>().ToList().ForEach(c => c.Font = ansBox.Font);
        }

        private void fx3_Click(object sender, EventArgs e)
        {
            qstIDl.Font = qstTitle.Font = qstText.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            ansBox.Font = new Font("Segoe UI", 12f);
            ansBox.Controls.OfType<Answer>().ToList().ForEach(c => c.Font = ansBox.Font);
        }

    }
}
