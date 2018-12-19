using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace TestApp
{

    public class QuestionPack
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Discipline { get; set; }
        public string Theme { get; set; }
        public List<Question> Questions { get; set; }
        public int TimeToAnswer { get; set; }
        public int TimeToTest { get; set; }

        public override string ToString() => $"{Title} ({Author})";

        public bool ToFile(string fileName)
        {
            if(string.IsNullOrEmpty(fileName)) return false;

            using (var f = new FileStream(fileName, FileMode.OpenOrCreate))
            using (var w = new BinaryWriter(f))
            {
                w.Write(Author);
                w.Write(Title);
                w.Write(Discipline);
                w.Write(Theme);
                w.Write(TimeToAnswer);
                w.Write(TimeToTest);

                w.Write(Questions.Count);
                Questions.ForEach(q =>
                {
                    var bin = q.ToBinary();
                    w.Write(bin.Length);
                    w.Write(bin);
                });
            }

            return true;
        }

        public static QuestionPack FromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var q = new QuestionPack();
            try
            {

                using (var f = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                using (var r = new BinaryReader(f))
                {
                    q.Author = r.ReadString();
                    q.Title = r.ReadString();
                    q.Discipline = r.ReadString();
                    q.Theme = r.ReadString();
                    q.Questions = new List<Question>();
                    q.TimeToAnswer = r.ReadInt32();
                    q.TimeToTest = r.ReadInt32();

                    var qc = r.ReadInt32();
                    for (var i = 0; i < qc; i++)
                    {
                            var bin = r.ReadBytes(r.ReadInt32());
                            var qt = Question.FromBinary(bin);
                            if(qt != null)
                                q.Questions.Add(qt);
                    }
                }
            }
            catch
            {
                return null;
            }

            return q;
        }
    }

    public class Question
    {

        /// <summary>
        /// Тип построения ответов
        /// </summary>
        public enum QAnswerEnum
        {
            /// <summary>
            /// Возможен выбор одного верного ответа
            /// </summary>
            Radio,

            /// <summary>
            /// Возможен выбор нескольких ответов
            /// </summary>
            Check
        }

        /// <summary>
        /// Тип значений вопроса
        /// </summary>
        public enum QValueEnum
        {
            /// <summary>
            /// Представление вопроса на основе текстов
            /// </summary>
            Text,

            /// <summary>
            /// Представление вопроса на основе изображений
            /// </summary>
            Image,
        }

        public Question()
        {
            Answers = new object[0];
            RightAnswerId = new byte[0];
        }
        public Question(string title, string text, QValueEnum valType, QAnswerEnum ansType)
        {
            Title = title;
            Text = text;
            AnswerType = ansType;
            ValueType = valType;
            Answers = new object[0];
            RightAnswerId = new byte[0];
        }

        /// <summary>
        /// Тип выбора ответов
        /// </summary>
        public QAnswerEnum AnswerType { get; set; }

        /// <summary>
        /// Тип построения вопроса
        /// </summary>
        public QValueEnum ValueType { get; set; }

        /// <summary>
        /// Заголовок вопроса
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание вопроса
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Варианты ответов
        /// </summary>
        public object[] Answers { get; set; }

        /// <summary>
        /// Номера верноых ответов
        /// </summary>
        public byte[] RightAnswerId { get; set; }

        public void AddAnswer(object value)
        {
            Answers = Answers.Concat(new[] {value}).ToArray();
        }

        public double GetAnswerScores(params byte[] answers)
        {
            var x1 = RightAnswerId.Intersect(answers).Count();
            return x1 * 100f / RightAnswerId.Length / 100f;
        }

        public override string ToString() => Title;

        public byte[] ToBinary()
        {
            var mem = new MemoryStream();
            var w = new BinaryWriter(mem);

            w.Write(Title);
            w.Write(Text);
            w.Write(ValueType.ToString());
            w.Write(AnswerType.ToString());
            
            w.Write(RightAnswerId.Length);
            Array.ForEach(RightAnswerId, i => w.Write(i));

            w.Write(Answers.Length);
            Array.ForEach(Answers, i =>
            {
                if(ValueType== QValueEnum.Text)
                    w.Write(i.ToString());
                else
                {
                    var bin = new MemoryStream();
                    ((Image) i).Save(bin , ImageFormat.Png);
                    w.Write( Convert.ToBase64String(bin.ToArray()) );
                }
            });

            return mem.ToArray();
        }

        public static Question FromBinary(byte[] bin)
        {
            var mem = new MemoryStream(bin);
            var r = new BinaryReader(mem);
            var q = new Question();

            try
            {
                q.Title = r.ReadString();
                q.Text = r.ReadString();
                q.ValueType = (QValueEnum) Enum.Parse(typeof(QValueEnum), r.ReadString());
                q.AnswerType = (QAnswerEnum) Enum.Parse(typeof(QAnswerEnum), r.ReadString());

                var rac = r.ReadBytes(r.ReadInt32());
                q.RightAnswerId = rac;

                var tac = r.ReadInt32();
                for (int i = 0; i < tac; i++)
                {
                    var str = r.ReadString();
                    if (q.ValueType == QValueEnum.Text)
                        q.AddAnswer(str);
                    else
                        q.AddAnswer(Image.FromStream(new MemoryStream(Convert.FromBase64String(str))));
                }
            }
            catch
            {
                q = null;
            }

            return q;
        }
    }
}
