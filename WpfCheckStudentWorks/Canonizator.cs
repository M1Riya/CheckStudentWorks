using MyStemWrapper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WpfCheckStudentWorks
{
    public class Canonizator
    {
        List<string> stopWords = new List<string>();                         //слова, не несущие смысловую нагрузку 
        public void FillStopWords()
        {
            string path = @"stopwords.txt";
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                    stopWords.Add(sr.ReadLine());
            }
        }
        public string[] TextCanonization(string s)     //канонизация текста и разбиение на слова
        {
            FillStopWords();
            s = Regex.Replace(s.ToLower(), @"[^\w\d\s]", " ").Replace('ё', 'е').Replace('o', 'о').Replace('a', 'а').Replace('y', 'у').Replace('c', 'с').Replace('e', 'е')
                .Replace("\n", "").Replace("\r", " ").Replace("  ", " ");

            for (int k = 0; k < stopWords.Count; k++)              
                s = s.Replace(stopWords[k], " ");

            string[] words = MyStemMethod(s);
            return words;
        }
        public string[] MyStemMethod(string s)
        {

            var stemmer = new MyStem();
            stemmer.PathToMyStem = @"mystem.exe";
            string canon = stemmer.Analysis(s);

            string pattern = @"\{(\w+)[(\?*\})(\?*\|)]";
            Regex rgx = new Regex(pattern);
            var words = rgx.Matches(canon).Cast<Match>()
                                          .Select(m => m.Value.Substring(1, m.Value.Length - 2))
                                          .ToArray();

            return words;

        }
    }
}
