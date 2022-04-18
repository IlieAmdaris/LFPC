using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FasterBigArr
{
    class Program
    {
        public static string path = @"C:\Users\ilie.todirascu\Desktop\Uni\LFPC\LfpcLabs\BonusBigArr\";
        public static string filesLocation = @"C:\Users\ilie.todirascu\Desktop\Uni\LFPC\LfpcLabs\BonusBigArr\FasterBigArr\Files\";

        static async Task Main(string[] args)
        {

            //for (int i = 0; i < 100; i++)
            //{
            //    await Task.Run(() => GenerateNumbers(filesLocation, i));
            //}
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine(GetNumber(9662, 99999));
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.TotalSeconds);

        }
        private static string GetLine(int line)
        {
            string fileName = filesLocation + $"file{line / 1000}.txt";
            using (var sr = new StreamReader(fileName))
            {
                return File.ReadLines(fileName).ElementAt(line / 100);
            }
        }
        
        public static string GetNumber(int row, int column)
        {
            return GetLine(row).Split(' ')[column];
        }
        public static void GenerateNumbers(string path,int fileNumber)
        {
            string filePath = filesLocation + $"file{fileNumber}.txt";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                Random rnd = new Random();

                for (int i = 0; i < 1000; i++)
                {
                    for (int j = 0; j < 100000; j++)
                    {
                        int number = rnd.Next(1, 101);
                        sw.Write(number + " ");
                    }
                    sw.Write('\n');
                }
            }
        }
    }
}
