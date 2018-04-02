using CsvHelper;
using CsvHelper.Configuration.Attributes;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TweetPuller
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var doc = new HtmlDocument();
            doc.Load("twitterHtml.txt", Encoding.UTF8);

            var output = doc.DocumentNode.Descendants();

            var classOutput = output.Where(x => x.Attributes["class"]?.Value == "TweetTextSize TweetTextSize--normal js-tweet-text tweet-text").ToList();

            var allQuestionsAndAnswers = new List<QuestionAndAnswers>();

            foreach (var block in classOutput)
            {
                var questionAndAnswers = new QuestionAndAnswers();
                var text = block.InnerText;

                questionAndAnswers.Question = text.Split('\n')[0];
                questionAndAnswers.Answer1 = text.Split('\n')[1];
                questionAndAnswers.Answer2 = text.Split('\n')[2];
                questionAndAnswers.Answer3 = text.Split('\n')[3];

                if (questionAndAnswers.Answer1.Contains("✓"))
                {
                    if (questionAndAnswers.CorrectAnswer != 0) throw new Exception("2 correct answers!");
                    questionAndAnswers.CorrectAnswer = 1;
                    questionAndAnswers.Answer1 = questionAndAnswers.Answer1.Replace("✓", "");
                }
                if (questionAndAnswers.Answer2.Contains("✓"))
                {
                    if (questionAndAnswers.CorrectAnswer != 0) throw new Exception("2 correct answers!");
                    questionAndAnswers.CorrectAnswer = 2;
                    questionAndAnswers.Answer2 = questionAndAnswers.Answer2.Replace("✓", "");
                }
                if (questionAndAnswers.Answer3.Contains("✓"))
                {
                    if (questionAndAnswers.CorrectAnswer != 0) throw new Exception("2 correct answers!");
                    questionAndAnswers.CorrectAnswer = 3;
                    questionAndAnswers.Answer3 = questionAndAnswers.Answer3.Replace("✓", "");
                }

                allQuestionsAndAnswers.Add(questionAndAnswers);
            }

            var config = new CsvHelper.Configuration.Configuration
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                Encoding = Encoding.UTF8
            };

            Directory.CreateDirectory(@"C:\TweetPullerOutput\");
            using (var writer = new StreamWriter(@"C:\TweetPullerOutput\QuestionsAndAnswers.csv", false, Encoding.UTF8))
            {
                var csv = new CsvWriter(writer, config);

                csv.WriteRecords(allQuestionsAndAnswers);
            }
        }
    }

    public class QuestionAndAnswers
    {
        [Name("Question")]
        public string Question { get; set; }

        [Name("Answer1")]
        public string Answer1 { get; set; }

        [Name("Answer2")]
        public string Answer2 { get; set; }

        [Name("Answer3")]
        public string Answer3 { get; set; }

        [Name("CorrectAnswer")]
        public int CorrectAnswer { get; set; }
    }
}
