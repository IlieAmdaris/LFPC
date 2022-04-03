using System;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            NfaToDfa nfaToDfa = new("C:\\Users\\ilie.todirascu\\Desktop\\Uni\\LFPC\\Lab2\\Grammar.txt");
            nfaToDfa.PrintRules(nfaToDfa.Rules);
            Console.WriteLine();
            nfaToDfa.CreateDfaRules("Q0");
            nfaToDfa.PrintRules(nfaToDfa.DfaAutomaton);
            Console.WriteLine(nfaToDfa.TestWord("aaabbaaa"));
        }
    }
}
