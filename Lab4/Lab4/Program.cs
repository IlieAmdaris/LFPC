using System;

namespace Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            Grammar grammar = new("C:\\Users\\ilie.todirascu\\Desktop\\Uni\\LFPC\\LfpcLabs\\Lab4\\Grammar.txt");
            grammar.PrintRules(grammar.Rules);
        }
    }
}
