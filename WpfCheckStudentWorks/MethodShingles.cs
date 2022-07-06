using SimMetrics.Net.Metric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace WpfCheckStudentWorks
{
    public class MethodShingles
    {
        static List<CheckSumInformation> checkSum = new List<CheckSumInformation>();
        public static void ShinglCreate(List<TextInformation> inf)
        {
            checkSum.Clear();
            int shingleLen = 5;
            Canonizator canon = new Canonizator();
            for (int i = 0; i < inf.Count; i++)
            {
                string[] words = canon.TextCanonization(inf[i].Text);

                checkSum.Add(new CheckSumInformation());
                checkSum[i].Text = inf[i].Text;

                for (int j = 0; j <= words.Length - shingleLen; j++)
                {
                    string shingleString = string.Join(" ", words, j, shingleLen);
                    string[] shingleWords = shingleString.Split(' ');
                    var shingleStringSort = string.Join(" ", shingleWords.OrderBy(c => c));

                    checkSum[i].HashShingle.Add(ShingleEncode(shingleStringSort));
                    checkSum[i].OriginShingle.Add(shingleString);
                }
            }
        }
        static string ShingleEncode(string shingle)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(shingle));
            string res = Convert.ToBase64String(hash);

            return res;
        }
        public static double CheckSumCompair(int text_1, int text_2)
        {
            double same = 0;
            for (int j = 0; j < checkSum[text_1].HashShingle.Count; j++)
                if (checkSum[text_2].HashShingle.Exists(o => o == checkSum[text_1].HashShingle[j]))
                {
                    same++;
                }
            int A = checkSum[text_1].HashShingle.Count;
            int B = checkSum[text_2].HashShingle.Count;
            return Math.Round(2 * same / (A + B) * 100, 1);
        }
        public static List<string> SearchMatch(int text_1, int text_2)
        {
            List<string> match = new List<string>();
            for (int j = 0; j < checkSum[text_1].HashShingle.Count; j++)
                if (checkSum[text_2].HashShingle.Exists(o => o == checkSum[text_1].HashShingle[j]))
                {
                    match.Add(checkSum[text_1].OriginShingle[j]);
                }

            return match.Distinct().ToList();
        }

        // returns list of the original matched strings
        public static List<string> OutputMatchedText(string text, List<string> match)
        {
            string[] txtWords = text.Replace("\n", "").Replace("\r", " ").Split(' ');

            List<string> txtShingle = new List<string>();

            for (int j = 0; j <= txtWords.Length - 9; j++)
            {
                string shingleString = string.Join(" ", txtWords, j, 9);
                txtShingle.Add(shingleString);
            }

            List<string> resultText = new List<string>();

            int tmp = 0;
            Thread[] thr = new Thread[4];
            for (int i = 0; i < 4; i++)
            {
                var tuple = new Tuple<int, List<string>, List<string>, List<string>>(tmp, match, txtShingle, resultText);
                thr[i] = new Thread(ThreadSearchMatch);
                thr[i].Start(tuple);
                tmp += 1;
            }
            for (int i = 0; i < 4; i++)
            {
                if (thr[i].IsAlive) thr[i].Join();

            }
            return resultText.Distinct().ToList();
        }
        static void ThreadSearchMatch(object o)
        {
            var allinf = (Tuple<int, List<string>, List<string>, List<string>>)o;
            int start = allinf.Item1;
            List<string> match = allinf.Item2;
            List<string> txtShingle = allinf.Item3;
            List<string> resultText = allinf.Item4;

            JaroWinkler jaroW = new JaroWinkler();
            double temp;
           List<int> indexes = new List<int>();
            for (int i = start; i < match.Count; i += 4)
            {
                double max = -1;
                for (int j = 0; j < txtShingle.Count; j++)
                {
                    temp = jaroW.GetSimilarity(match[i], txtShingle[j]);
                    if (temp > max)
                    {
                        max = temp;
                    }
                }
                for (int j = 0; j < txtShingle.Count; j++)
                {
                    temp = jaroW.GetSimilarity(match[i], txtShingle[j]);
                    if (temp == max)
                    {
                        max = temp;
                        indexes.Add(j);
                    }
                }
                lock ("write")
                {
                    foreach (int index in indexes)
                    resultText.Add(txtShingle[index]);
                }
            }
        }
    }
}
