using System;
using System.IO;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = File.ReadAllText("C:\\Users\\ilie.todirascu\\Desktop\\Uni\\LFPC\\LfpcLabs\\Lab3\\Code.txt");
            PrintAndWriteTokens(code, "C:\\Users\\ilie.todirascu\\Desktop\\Uni\\LFPC\\LfpcLabs\\Lab3\\Output.txt");
        }
        public static void PrintAndWriteTokens(string code,string fileName)
        {
            Lexer lexer = new(code);
            var tokens = lexer.GetTokens();
            using (StreamWriter sw = new(fileName))
            {
                int i = 0;
                foreach (var token in tokens)
                {
                    if (i == 3)
                    {
                        Console.WriteLine();
                        sw.Write("\n");
                        i = 0;
                    }
                    Console.Write($"{token} ");
                    sw.Write(token.ToString() + " ");
                    i++;
                }
            }
            
        }
    }
}
