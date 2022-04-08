using MyStemWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WpfCheckStudentWorks
{
    public class Canonizator
    {
        static List<string> stopWords = new List<string>();                         //слова, не несущие смысловую нагрузку 
        public static void FillStopWords()
        {
            string path = @"..\..\stopwords.txt";
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                    stopWords.Add(sr.ReadLine());
            }
        }
        public static string[] TextCanonization(string s)     //канонизация текста и разбиение на слова
        {
            FillStopWords();
            s = Regex.Replace(s.ToLower(), @"[^\w\d\s]", " ").Replace('ё', 'е').Replace("\n", "").Replace("\r", " ").Replace("  ", " ");

            for (int k = 0; k < stopWords.Count; k++)              //может сделать через linq????????????
                s = s.Replace(stopWords[k], " ");

            //s = MyStemMethod(s);

            string[] words = s.Split(' ');
            return words;
        }
        public static string MyStemMethod(string s)
        {
            var lemmatizer = new Lemmatizer();

            return lemmatizer.GetText(s);

        }
    }
}
