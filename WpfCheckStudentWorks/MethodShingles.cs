using SimMetrics.Net.Metric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WpfCheckStudentWorks
{
    public class MethodShingles
    {
        static List<CheckSumInformation> checkSum = new List<CheckSumInformation>();          //контрольная сумма всех шинглов для каждого текста
        public static void ShinglCreate(List<TextInformation> inf)
        {
            checkSum.Clear();
            int shingleLen = 5;                                     
            for (int i = 0; i < inf.Count; i++)
            {
                string[] words = Canonizator.TextCanonization(inf[i].Text);

                checkSum.Add(new CheckSumInformation());
                checkSum[i].FileName = inf[i].FileName;
                checkSum[i].Text = inf[i].Text;

                for (int j = 0; j <= words.Length - shingleLen; j++)
                {
                    string shingleString = string.Join(" ", words, j, shingleLen);
                    string[] shingleWords = shingleString.Split(' ');
                    var shingleStringSort = string.Join(" ", shingleWords.OrderBy(c => c));         //попытка отловить перестановку слов

                    checkSum[i].HashShingle.Add(ShingleEncode(shingleStringSort));
                    checkSum[i].OriginShingle.Add(shingleString);
                }
            }
        }
        static string ShingleEncode(string shingle)
        {
            /*var md5 = MD5.Create();                                                 //MD5 алгоритм
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(shingle));
            string res = Convert.ToBase64String(hash);*/

            var arrayOfBytes = Encoding.UTF8.GetBytes(shingle);         //Crc32 алгоритм. НИЖЕ ВЕРОЯТНОСТЬ КОЛЛИЗИЙ
            var crc32 = new Crc32();
            string res = crc32.Get(arrayOfBytes).ToString("X");

            return res;
        }
        public static double CheckSumCompair(int text_1, int text_2)
        {
            double different = checkSum[text_1].HashShingle.Count;
            for (int j = 0; j < checkSum[text_2].HashShingle.Count; j++)
                if (!checkSum[text_1].HashShingle.Exists(o => o == checkSum[text_2].HashShingle[j]))
                    different++;
            double same = 0;
            for (int j = 0; j < checkSum[text_1].HashShingle.Count; j++)
                if (checkSum[text_2].HashShingle.Exists(o => o == checkSum[text_1].HashShingle[j]))
                {
                    same++;
                }
            int A = checkSum[text_1].HashShingle.Count;
            int B = checkSum[text_2].HashShingle.Count;
            //return Math.Round(same / different * 100, 1);
            return Math.Round((2*same) /(A + B - Math.Abs(A - B)) * 100, 1);
        }
        public static List<string> SearchMatch (int text_1, int text_2)
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
        public static List<string> OutputMatchedText(string text,  List<string> match)           
        {
            string[] txtWords = text.Replace("\n", "").Replace("\r", " ").Split(' ');

            List<string> txtShingle = new List<string>();

            for (int j = 0; j <= txtWords.Length - 10; j++)
            {
                string shingleString = string.Join(" ", txtWords, j, 10);
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
                if (thr[i].IsAlive)
                {
                    thr[i].Join();
                }
            }
            /*JaroWinkler jaroW = new JaroWinkler();
            double temp;
            int index = 0;
            for (int i = 0; i < match.Count; i++)
            {
                //double min = int.MaxValue;
                double max = -1;
                for (int j = 0; j < txtShingle.Count; j++)
                {
                    //temp = DamerauLevenshteinDistance(match[i], txt1Shingle[j]);
                    temp = jaroW.GetSimilarity(match[i], txtShingle[j]);
                    if (temp > max)
                    {
                        max = temp;
                        index = j;
                    }
                }
                resultText.Add(txtShingle[index]);
            }*/

            return resultText.Distinct().ToList();
        }
        static void ThreadSearchMatch (object o)
        {
            var allinf = (Tuple<int, List<string>, List<string>, List<string>>)o;
            int start = allinf.Item1;
            List<string> match = allinf.Item2;
            List<string> txtShingle = allinf.Item3;
            List<string> resultText = allinf.Item4;

            JaroWinkler jaroW = new JaroWinkler();
            double temp;
            int index = 0;
            for (int i = start; i < match.Count; i+=4)
            {
                //double min = int.MaxValue;
                double max = -1;
                for (int j = 0; j < txtShingle.Count; j++)
                {
                    //temp = DamerauLevenshteinDistance(match[i], txtShingle[j]);
                    temp = jaroW.GetSimilarity(match[i], txtShingle[j]);
                    if (temp > max)
                    {
                        max = temp;
                        index = j;
                    }
                }
                lock ("write")
                {
                    resultText.Add(txtShingle[index]);
                }
            }
        }

        static int Minimum(int a, int b) => a < b ? a : b;
        static int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;

        static int DamerauLevenshteinDistance(string firstText, string secondText)
        {
            var n = firstText.Length + 1;
            var m = secondText.Length + 1;
            var arrayD = new int[n, m];

            for (var i = 0; i < n; i++)
            {
                arrayD[i, 0] = i;
            }

            for (var j = 0; j < m; j++)
            {
                arrayD[0, j] = j;
            }

            for (var i = 1; i < n; i++)
            {
                for (var j = 1; j < m; j++)
                {
                    var cost = firstText[i - 1] == secondText[j - 1] ? 0 : 1;

                    arrayD[i, j] = Minimum(arrayD[i - 1, j] + 1,        
                                            arrayD[i, j - 1] + 1,       
                                            arrayD[i - 1, j - 1] + cost);

                    if (i > 1 && j > 1
                        && firstText[i - 1] == secondText[j - 2]
                        && firstText[i - 2] == secondText[j - 1])
                    {
                        arrayD[i, j] = Minimum(arrayD[i, j],
                                           arrayD[i - 2, j - 2] + cost);
                    }
                }
            }
            return arrayD[n - 1, m - 1];
        }
    }
}
