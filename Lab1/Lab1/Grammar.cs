using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Grammar
    {
        public string[] Vn, Vt, P;
        public Dictionary<string, string> QMapping = new();
        public Dictionary<string, List<KeyValuePair<char, string>>> rules;
        public Grammar(string fileName)
        {
            Vn = GetGrammarData("Vn", fileName);
            for (int i = 0; i < Vn.Length; i++)
            {
                QMapping.Add(Vn[i], "Q" + i);
            }
            QMapping.Add("", "Qf");
            Vt = GetGrammarData("Vt", fileName);
            P = GetGrammarData("P", fileName);
            rules = CreateAutomatonRules();
        }
        public void PrintQMapping()
        {
            foreach (var item in rules)
            {
                Console.Write($"{QMapping[item.Key]}:[");
                foreach (var kvp in item.Value)
                {
                    Console.Write($"[{kvp.Key}: {QMapping[string.IsNullOrEmpty(kvp.Value) ? "" : kvp.Value]}]");
                }
                Console.Write("]");
                Console.WriteLine();
            }   
        }
        private string[] GetGrammarData(string notation, string fileName)
        {
            string[] resultData = new string[] { "no Value" };
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains(notation))
                    {
                        int startIndex = line.IndexOf('{') + 1;
                        int endIndex = line.IndexOf('}') - startIndex;
                        line = line.Substring(startIndex, endIndex);
                        resultData = line.Split(',');
                    }
                }
            }
            return resultData;
        }
        public Dictionary<string, List<KeyValuePair<char,string>>> CreateAutomatonRules()
        {
            var derivationRules = new Dictionary<string, List<KeyValuePair<char, string>>>();
            foreach (string rule in P)
            {
                string[] kvp = rule.Split('-');
                
                string q = kvp[1].Length == 1 ? null : kvp[1][1] + "";
                char transition = kvp[1].Length == 1 ? kvp[1].ToCharArray()[0] : kvp[1][0];
                
                if (derivationRules.ContainsKey(kvp[0]))
                {
                    derivationRules[kvp[0]].Add(new KeyValuePair<char, string>(transition, q));
                }
                else
                {
                    derivationRules.Add(kvp[0], new List<KeyValuePair<char, string>> { new KeyValuePair<char, string>(transition, q) });
                }
            }
            return derivationRules;
        }
        public string GetState(List<KeyValuePair<char, string>> allowedStateValuePairs, string word, int i)
        {
            foreach (var item in allowedStateValuePairs)
            {
                if (item.Key == word[i]) return item.Value;
            }
            return "invalidString";
        }
        public bool TestWord(string word)
        {
            if (word.Length == 0)
            {
                return false;
            }
            var allowedStateValuePairs = rules["S"];
            string state = GetState(allowedStateValuePairs, word, 0);
            if (state.Equals("invalidString"))
            {
                return false;
            }
            if (word.Length == 1)
            {
                if (String.IsNullOrEmpty(state))
                {
                    return true;
                }
                return false;
            }
            for (int i = 1; i < word.Length; i++)
            {
                state = GetState(rules[state], word, i);
                if (String.IsNullOrEmpty(state) && i != word.Length - 1)
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(state))
                {
                    if (state.Equals("invalidString"))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
