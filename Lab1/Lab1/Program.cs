using System;
using System.IO;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Grammar grammar = new("C:\\Users\\ilie.todirascu\\Desktop\\Uni\\LFPC\\Lab1\\Grammar.txt");
            Console.WriteLine(grammar.TestWord("dd"));
            grammar.PrintQMapping();
        }
    }
}
