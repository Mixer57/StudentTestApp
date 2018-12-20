using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;

namespace StudiTest
{
    internal static class TestProvider
    {
        /// <summary>
        /// Путь к текущей папке с тестами. Устанавливается вызовом метода <see cref="InitTestDir(string)"/>
        /// </summary>
        public static string TestsDir { get; private set; }

        /// <summary>
        /// Список тестов. Путь и Тема теста. Обновляется вызовом метода <see cref="InitTestDir(string)"/>
        /// </summary>
        public static Dictionary<string, string> Tests { get; private set; }

        /// <summary>
        /// Метод инициализации папки с тестами.<para/>
        /// Обновляет свойства <see cref="Tests"/> и <see cref="TestsDir"/>
        /// </summary>
        /// <param name="dir">Путь к папке с тестами формата .mtx (Mixer57 Test Xml)</param>
        public static void InitTestDir(string dir = @".\tests\")
        {
            var doc = new XmlDocument();
            Tests = new Dictionary<string, string>();
            if (Directory.Exists(dir))
            {
                Directory.GetFiles(dir, "*.mtx", SearchOption.TopDirectoryOnly)
                    .ToList()
                    .ForEach(mtx =>
                   {
                       try
                       {
                           doc.Load(mtx);
                           var i = doc.SelectSingleNode("//Info");
                           Tests.Add(mtx, $"[{i.Attributes["lang"].Value}] {i.Attributes["lesson"].Value} -> {i.Attributes["title"].Value}");
                       }
                       catch { }
                   });
                TestsDir = dir;
            }
        }

        public static Test LoadTest(string testFileName) => new Test(testFileName);
    }
    public class Test
    {
        public Test(string testFileName)
        {
            try
            {
                LoadFailed = false;
                var doc = new XmlDocument();
                doc.Load(testFileName);

                var i = doc.SelectSingleNode("//Info");
                Author = i.Attributes["author"].Value;
                Language = i.Attributes["lang"].Value;
                Title = i.Attributes["title"].Value;
                Lesson = i.Attributes["lesson"].Value;
                TimeToAnswer = int.Parse(i.Attributes["answertime"].Value);
                TimeToTest = int.Parse(i.Attributes["testtime"].Value);
                Questions = new List<Question>();

                doc.SelectNodes("//Question")
                    .Cast<XmlElement>()
                    .OrderBy(qa => int.Parse(qa.Attributes["id"].Value))
                    .ToList()
                    .ForEach(q =>
                    {
                        var qx = new Question
                        {
                            ID = int.Parse(q.Attributes["id"].Value),
                            AnswerIDs = q.Attributes["answer"].Value.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                            Title = q.Attributes["title"].Value,
                            Type = (Question.TypeEnum)Enum.Parse(typeof(Question.TypeEnum), q.Attributes["type"].Value),
                            Selection = (Question.SelectionEnum)Enum.Parse(typeof(Question.SelectionEnum), q.Attributes["selection"].Value),
                            Text = q.ChildNodes[0].InnerText.Trim(),
                            Answers = new List<Question.Answer>()
                        };

                        doc.SelectNodes($"//Question[@id='{qx.ID}']/Answer")
                        .Cast<XmlElement>()
                        .OrderBy(qa => int.Parse(qa.Attributes["id"].Value))
                        .ToList()
                        .ForEach(a =>
                        {
                            var qa = new Question.Answer()
                            {
                                ID = int.Parse(a.Attributes["id"].Value),
                                Value = qx.Type == Question.TypeEnum.txt ? (object)a.ChildNodes[0].Value : (object)Image.FromStream(new MemoryStream(Convert.FromBase64String(a.ChildNodes[0].Value.Substring(4))))
                            };
                            qx.Answers.Add(qa);
                        });
                        Questions.Add(qx);
                    });

                TimeOfStart = DateTime.Now;
                Answers = new int[Questions.Count][];
                doc = null;
            }
            catch (Exception ex)
            {
                LoadFailed = true;
                LoadFailedWithError = ex.Message;
            }
        }

        public bool LoadFailed { get; private set; }
        public string LoadFailedWithError { get; private set; }

        public string Title { get; private set; }
        public string Lesson { get; private set; }
        public string Language { get; private set; }
        public string Author { get; private set; }
        public int TimeToTest { get; private set; }
        public int TimeToAnswer { get; private set; }
        public List<Question> Questions { get; private set; }
        public int[][] Answers { get; private set; }
        public DateTime TimeOfStart { get; private set; }


        private TimeSpan StopTest() => DateTime.Now - TimeOfStart;
        public void AddAnswer(int id, params int[] answers) => Answers[id] = answers;
        public Scores GetScore()
        {
            var tm = StopTest();
            //var an = new List<double>();
            var an = Enumerable.Range(0, Answers.Length).ToList().Select(i => 
            {
                var rightAnswers = Answers[i].Intersect(Questions[i].AnswerIDs).Count();
                var rightAnswersValue = rightAnswers* 1f / Questions[i].AnswerIDs.Length ;
                var totalAnswersValue = rightAnswers * 1f / Answers[i].Length;
                var result = totalAnswersValue * rightAnswersValue;
                return result;
            }).ToArray();
            return new Scores(tm, an);

        }
        public override string ToString() => $"[{Language}] {Lesson} -> {Title} © {Author}  <{Questions.Count}>";

        public class Question
        {
            public enum TypeEnum { txt, img }
            public enum SelectionEnum { one, many }

            public int ID { get; internal set; }
            public int[] AnswerIDs { get; internal set; }
            public string Title { get; internal set; }
            public string Text { get; internal set; }
            public TypeEnum Type { get; internal set; }
            public SelectionEnum Selection { get; internal set; }
            public List<Answer> Answers { get; internal set; }

            public override string ToString() => $"[ID:{ID}] {Type}/{Selection} {Title} @ {Answers.Count} answers";

            public class Answer
            {

                public int ID { get; internal set; }
                public object Value { get; internal set; }

                public override string ToString() => $"[ID:{ID}] {Value}";
            }
        }
        public class Scores
        {
            public Scores(TimeSpan time, float[] answersValues)
            {
                TestTime = time;
                Answers = answersValues;
            }
            public TimeSpan TestTime { get; }
            public float[] Answers { get; }
            public float ScorePercents => Answers.Sum() * 100f / Answers.Length;
            public float ScoreNum => Answers.Sum() * 5f / Answers.Length;
            public int ScoreNumRounded => (int)ScoreNum;

            public override string ToString() => $"Оценка: {ScoreNumRounded} ({ScoreNum:N3}) Верно: {Answers.Where(i => i == 1f).Sum()}/{Answers.Length} ({ScorePercents:N3}%)";
        }
    }
}
