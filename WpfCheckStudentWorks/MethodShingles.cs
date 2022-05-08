using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCheckStudentWorks
{
    public class MethodShingles
    {
        static List<CheckSumInformation> checkSum = new List<CheckSumInformation>();          //контрольная сумма всех шинглов для каждого текста
        public static void ShinglCreate(List<TextInformation> inf)
        {
            checkSum.Clear();
            int shingleLen = 5;                                     // или 6?
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
            List<string> match = new List<string>();
            double same = 0;
            for (int j = 0; j < checkSum[text_1].HashShingle.Count; j++)
                if (checkSum[text_2].HashShingle.Exists(o => o == checkSum[text_1].HashShingle[j]))
                {
                    same++;
                    match.Add(checkSum[text_1].OriginShingle[j]);
                }

            //if (Math.Round(same / different * 100, 1)>2)
             // OutputMatchedText(checkSum[text_1].Text, checkSum[text_2].Text, match);

            return Math.Round(same / different * 100, 1);
        }

        static void OutputMatchedText(string text1, string text2, List<string> match)           //может вытащить во viewmodel?
        {

            string[] txt1Words = text1.Replace("\n", "").Replace("\r", " ").Split(' ');
            string[] txt2Words = text2.Replace("\n", "").Replace("\r", " ").Split(' ');

            List<string> txt1Shingle = new List<string>();
            List<string> txt2Shingle = new List<string>();

            for (int j = 0; j <= txt1Words.Length - 10; j++)
            {
                string shingleString = string.Join(" ", txt1Words, j, 10);
                txt1Shingle.Add(shingleString);
            }

            for (int j = 0; j <= txt2Words.Length - 10; j++)
            {
                string shingleString = string.Join(" ", txt2Words, j, 10);
                txt2Shingle.Add(shingleString);
            }

            List<string> resultText1 = new List<string>();
            List<string> resultText2 = new List<string>();
            int temp;
            int index = 0;
            for (int i = 0; i < match.Count; i++)
            {
                int min = int.MaxValue;
                for (int j = 0; j < txt1Shingle.Count; j++)
                {
                    temp = DamerauLevenshteinDistance(match[i], txt1Shingle[j]);
                    if (temp < min)
                    {
                        min = temp;
                        index = j;
                    }
                }
                resultText1.Add(txt1Shingle[index]);

                int min2 = int.MaxValue;
                for (int j = 0; j < txt2Shingle.Count; j++)
                {
                    temp = DamerauLevenshteinDistance(match[i], txt2Shingle[j]);
                    if (temp < min2)
                    {
                        min2 = temp;
                        index = j;
                    }
                }
                resultText2.Add(txt2Shingle[index]);
            }

            resultText1 = resultText1.Distinct().ToList();
            resultText2 = resultText2.Distinct().ToList();

            foreach (var x in resultText1)
                Console.WriteLine(x);
            Console.WriteLine("=====================================================");
            foreach (var x in resultText2)
                Console.WriteLine(x);
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

                    arrayD[i, j] = Minimum(arrayD[i - 1, j] + 1,          // удаление
                                            arrayD[i, j - 1] + 1,         // вставка
                                            arrayD[i - 1, j - 1] + cost); // замена

                    if (i > 1 && j > 1
                        && firstText[i - 1] == secondText[j - 2]
                        && firstText[i - 2] == secondText[j - 1])
                    {
                        arrayD[i, j] = Minimum(arrayD[i, j],
                                           arrayD[i - 2, j - 2] + cost); // перестановка
                    }
                }
            }
            return arrayD[n - 1, m - 1];
        }
    }
}
