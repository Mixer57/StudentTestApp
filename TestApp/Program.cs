using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace TestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //var x = new XmlDocument();
            //var q = new QuestionPack();
            //x.Load("test.xml");
            //var info = x.SelectSingleNode("//Info") as XmlElement;
            //q.Author = info.Attributes["author"].Value;
            //q.Title = info.Attributes["title"].Value;
            //q.Discipline = info.Attributes["subject"].Value;
            //q.TimeToAnswer = int.Parse(info.Attributes["timeans"].Value);
            //q.TimeToTest = int.Parse(info.Attributes["timetst"].Value);

            //var qsts = x.SelectNodes("//Question");
            //foreach (XmlElement qst in qsts)
            //{
            //    var qi = new Question
            //    {
            //        RightAnswerId = qst.Attributes["answer"].Value.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(byte.Parse).ToArray(),
            //        Text = qst.Attributes["text"].Value,
            //        Title = qst.Attributes["title"].Value,
            //        AnswerType = qst.Attributes["selection"].Value == "one" ? Question.QAnswerEnum.Radio : Question.QAnswerEnum.Check,
            //        ValueType = qst.Attributes["type"].Value == "text" ? Question.QValueEnum.Text : Question.QValueEnum.Image
            //    };

            //    foreach (XmlElement a in qst.SelectNodes("Answer"))
            //    {
            //        if (qi.ValueType== Question.QValueEnum.Image)
            //            qi.AddAnswer((Bitmap)Image.FromStream(new MemoryStream(Convert.FromBase64String(a.FirstChild.Value.Substring(4)))));
            //        else
            //            qi.AddAnswer(a.FirstChild.Value);
            //    }
            //    q.Questions.Add(qi);
            //}


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0 && args[0].Replace("\"","").ToLower().EndsWith(".ans"))
                Application.Run(new Form3(args[0]));
            else
                Application.Run(new Form1());
        }
    }
}
