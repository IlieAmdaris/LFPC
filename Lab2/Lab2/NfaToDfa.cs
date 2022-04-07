using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public class NfaToDfa
    {
        public string[] Vn, Vt, P, F;
        public Dictionary<string, List<KeyValuePair<char, string>>> Rules;
        public Dictionary<string, List<KeyValuePair<char, string>>> DfaAutomaton = new();
        public NfaToDfa(string fileName)
        {
            Vn = GetGrammarData("Vn", fileName);
            Vt = GetGrammarData("Vt", fileName);
            P = GetGrammarData("P", fileName);
            F = GetGrammarData("F", fileName);
            Rules = CreateAutomatonRules();
            CreateDfaRules("Q0");
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
        public void PrintRules(Dictionary<string, List<KeyValuePair<char, string>>> rules)
        {
            foreach (var item in rules)
            {
                Console.Write($"{item.Key}:[");
                foreach (var kvp in item.Value)
                {
                    Console.Write($"[{kvp.Key}: {kvp.Value}]");
                }
                Console.Write("]");
                Console.WriteLine();
            }
        }
        private Dictionary<string, List<KeyValuePair<char, string>>> CreateAutomatonRules()
        {
            var derivationRules = new Dictionary<string, List<KeyValuePair<char, string>>>();
            foreach (string rule in P)
            {
                string[] kvp = rule.Split('-');

                string q = kvp[1].Substring(1);
                char transition = kvp[1].Length > 1 ? kvp[1][0] : kvp[1].ToCharArray()[0];

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

        public string[] CreateArrayOfStates(string state)
        {
            string[] states = new string[state.Length / 2];
            for (int i = 0, iteration = 0; iteration < state.Length / 2; i += 2)
            {
                states[iteration] = new string(state[i..].Take(2).ToArray());
                iteration++;
            }
            return states;
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
            var allowedStateValuePairs = DfaAutomaton["Q0"];
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
                state = GetState(DfaAutomaton[state], word, i);
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
            if (state.Contains(F[0]))
            {
                return true;
            }
            return false;
        }
        public void UpdateDfaRules()
        {
            //c# doesn't support iterating over a list and updating it at the same time this is a workaround
            var DfaAutomatonKeys = DfaAutomaton.Keys.ToArray();
            foreach (var key in DfaAutomatonKeys)
            {
                foreach (var newState in DfaAutomaton[key])
                {
                    if (!DfaAutomaton.ContainsKey(newState.Value) && !string.IsNullOrEmpty(newState.Value))
                    {
                        CreateDfaRules(newState.Value);

                    }
                }
            }
        }
        public void CreateDfaRules(string state)
        {
            if (DfaAutomaton.ContainsKey(state))
            {
                return;
            }
            string[] dividedStates = CreateArrayOfStates(state);
            List<KeyValuePair<char, string>>[] valuesDiff = new List<KeyValuePair<char, string>>[dividedStates.Length];
            int i = 0;
            foreach (var dividedState in dividedStates)
            {

                List<KeyValuePair<char, string>> values = new();
                foreach (var transVar in Vt)
                {
                    string transValueState = "";
                    foreach (var rule in Rules[dividedState])
                    {
                        if (rule.Key == char.Parse(transVar))
                        {
                            transValueState += rule.Value;
                        }
                    }
                    values.Add(new KeyValuePair<char, string>(char.Parse(transVar), transValueState));
                }
                valuesDiff[i] = values;
                i++;
            }
            var tempValue = valuesDiff[0];
            List<KeyValuePair<char, string>> finalRow = tempValue;
            foreach (var value in valuesDiff)
            {
                if (!Enumerable.SequenceEqual(tempValue,value))
                {
                    for (int j = 0; j < value.Count; j++)
                    {
                        if (string.IsNullOrEmpty(tempValue[j].Value) && !string.IsNullOrEmpty(value[j].Value))
                        {
                            finalRow.Remove(tempValue[j]);
                            finalRow.Add(value[j]);
                        }
                        else if(tempValue[j].Key == value[j].Key && !tempValue[j].Value.Equals(value[j].Value))
                        {
                            finalRow[j] = new KeyValuePair<char, string>(tempValue[j].Key, tempValue[j].Value + value[j].Value);
                        }
                    }
                    tempValue = value;
                }
            }
            DfaAutomaton.Add(state, finalRow);
            UpdateDfaRules();
        }
    }
}
