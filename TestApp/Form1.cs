using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(".\\quests"))
                comboBox1.Items.AddRange(
                    Directory.GetFiles(@".\quests\", "*.qst", SearchOption.TopDirectoryOnly).Select(i => i.Substring(9))
                        .Cast<object>().ToArray());
            else
                MessageBox.Show("Папка 'quests' не найдена.\nТесты не загружены!", "Не найдена рабочая папка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var test = new Form2 {  Text = $@"{textBox1.Text} | {textBox2.Text} | {comboBox1.Text}"};
            test.Qp = QuestionPack.FromFile($".\\quests\\{comboBox1.Text}");
            if (test.ShowDialog() != DialogResult.OK) return;

            var sum = test.Answers.Sum(i=>i.Value);
            var cnt = test.Answers.Count;
            var prc = sum * 100f / cnt;
            var scr = prc * 5f / 100f;


            File.AppendAllText($".\\quests\\{comboBox1.Text.Replace(".qst",".ans")}",
                $"Дата    : {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\r\n" +
                $"Студент : {textBox1.Text}\r\n" +
                $"Группа  : {textBox2.Text}\r\n" +
                $"Оценка  : {Math.Round(scr, 0)} ({Math.Round(scr, 4)})\r\n" +
                $"Время   : {(test.Qp.TimeToTest>0 ? $"{(test.Ended- test.Started).TotalSeconds:N0}" : "0" )} сек.\r\n" +
                $"-------------------------------------------------------------------\r\n" +
                $"Вопросов: {Math.Round(sum, 0)} из {cnt} ({Math.Round(prc, 3)}% верно)\r\n" +
                $"Баллы   : ({Math.Round(sum, 2):N2}) {string.Join(" | ", test.Answers.OrderBy(a=>a.Key).Select(a => Math.Round(a.Value, 2)).ToArray())}\r\n" +
                $"===================================================================\r\n",
                Encoding.UTF8
            );

            string wa = "";
            for (int i = 0; i < test.Answers.Count; i++)
                if (test.Answers[i] < 1f)
                    wa += $"{i}] {test.Answers[i]}%\n";


            MessageBox.Show(
                $"Вы прошли тест: {comboBox1.Text}\n\nВаша оценка:  {Math.Round(scr, 0)} ({Math.Round(scr, 4)})\n\nПравильных ответов: {Math.Round(sum, 0)} из {cnt} ({Math.Round(prc, 3)}%){(wa != "" ? $"\n\nСписок неверных ответов:\n{wa.Trim()}" : "")}",
                @"Тест пройден", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void CheckEn(object sender, EventArgs e)
        {
            pictureBox1.Image = textBox1.Text.Split(' ').Length == 3 && textBox1.Text.Split(' ')[2].Length > 0
                ? textBox1.Text.Split(' ').Last().ToCharArray().Last().ToString() == "а"
                    ? Properties.Resources.ppl_female
                    : Properties.Resources.ppl_male
                : Properties.Resources.ppl_unk;

            button1.Enabled = textBox1.TextLength > 0 & textBox2.TextLength > 0 & comboBox1.SelectedIndex > -1;
        }
    }
}
