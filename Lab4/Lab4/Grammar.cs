using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class Grammar
    {

        public List<string> Vn, Vt, P;
        public Dictionary<string, List<KeyValuePair<char, string>>> Rules;
        public List<string> ProductiveStates = new();
        public Dictionary<char, string> XStates = new();
        public Dictionary<string, string> YStates = new();
        public Grammar(string fileName)
        {
            Vn = GetGrammarData("Vn", fileName);
            Vt = GetGrammarData("Vt", fileName);
            P = GetGrammarData("P", fileName);
            ReplaceEps();
            RemoveUnitProductions();
            RemoveNonProductiveSymbols();
            RemoveUnreachable();
            CreateX();
            CreateYStates();
            ReplaceY();
            Rules = CreateAutomatonRules();
        }
        private List<string> GetGrammarData(string notation, string fileName)
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
            return resultData.ToList();
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
        private void ReplaceEps()
        {
            List<string> newP = new();
            var epsStates = FindEpsStates();
            foreach (var epsState in epsStates)
            {
                foreach (string rule in P)
                {
                    List<string> newStates = new();
                    string[] kvp = rule.Split('-');
                    string derivation = kvp[1];

                    for (int i = 0; i < derivation.Length; i++)
                    {
                        if (derivation[i].Equals(char.Parse(epsState)))
                        {
                            newStates.Add(i + 1 < derivation.Length ? new string(derivation[..i] + derivation[(i + 1)..]) :
                               new string(derivation.AsSpan()[..i]));
                        }
                    }
                    if (newStates.Count != 0)
                    {
                        //more than one State that leads to empty string in derivation
                        string edgeCase = new string(derivation.Where(x => !x.Equals(char.Parse(epsState))).ToArray());
                        if (!newStates.Contains(edgeCase))
                        {
                            newStates.Add(edgeCase);
                        }
                        foreach (var state in newStates)
                        {
                            if (!newP.Contains($"{kvp[0]}-{state}"))
                            {
                                newP.Add($"{kvp[0]}-{state}");
                            }
                        }
                    }
                    if (!kvp[1][0].Equals('e'))
                    {
                        if (!newP.Contains($"{ kvp[0]}-{derivation}"))
                        {
                            newP.Add($"{ kvp[0]}-{derivation}");
                        }
                    }
                }
                P.AddRange(newP);
            }
            P = newP.Count == 0 ? P : newP;
        }
        private List<string> FindEpsStates()
        {
            List<string> epsStates = new();
            foreach (string rule in P)
            {
                string[] kvp = rule.Split('-');
                if (kvp[1][0].Equals('e'))
                {
                    epsStates.Add(kvp[0]);
                }
            }
            return epsStates;
        }
        private void RemoveUnitProductions()
        {
            var unitProductions = GetUnitProductions();
            List<string> tempP = new(P);
            if (unitProductions.Count > 0)
            {
                foreach (var unit in unitProductions)
                {
                    string[] kvp = unit.Split('-');
                    string sourceState = kvp[0];
                    string derivation = kvp[1];
                    foreach (var rule in tempP)
                    {
                        var ruleKvp = rule.Split('-');
                        string sourceRule = ruleKvp[0];
                        string derivationRule = ruleKvp[1];
                        if (sourceRule.Equals(derivation) && Vn.Contains(derivation))
                        {
                            P.Remove($"{sourceState}-{derivation}");
                            if (!P.Contains($"{sourceState}-{derivationRule}"))
                            {
                                P.Add($"{sourceState}-{derivationRule}");

                            }
                        }
                    }
                }
                RemoveUnitProductions();
            }
        }
        private void RemoveUnreachable()
        {
            var tempP = new List<string>(P);
            foreach (var rule in tempP)
            {
                var ruleKvp = rule.Split('-');
                string sourceRule = ruleKvp[0];
                bool isUnreachable = true;
                foreach (var unreachableRule in tempP)
                {
                    var kvp = unreachableRule.Split('-');
                    string unreachableDerivation = kvp[1];
                    if (unreachableDerivation.Contains(sourceRule))
                    {
                        isUnreachable = false;
                        break;
                    }
                }
                if (isUnreachable)
                {
                    P.Remove($"{sourceRule}-{ruleKvp[1]}");
                }
            }
        }
        private void RemoveNonProductiveSymbols()
        {
            var tempP = new List<string>(P);
            foreach (var nonTerminalSymbol in Vn.Except(ProductiveStates))
            {

                foreach (var rule in P.Where(x =>
                {
                    var kvp = x.Split('-');
                    return (kvp[0].Equals(nonTerminalSymbol) && Vt.Contains(kvp[1]) && !ProductiveStates.Contains(kvp[0]));
                }))
                {
                    ProductiveStates.Add(nonTerminalSymbol);
                }
            }

            foreach (var rule in tempP.Where(j => { var kvp = j.Split('-'); return !ProductiveStates.Contains(kvp[0]); }).Where(x =>
            {
                var kvp = x.Split('-');
                return (ProductiveStates.Any(x => kvp[1].Where(n => Vn.Contains(n + "")).Any(y => x.Equals(y + ""))));
            }))
            {
                ProductiveStates.Add(rule.Split('-')[0]);
            }
            var unproductive = String.Join(',', Vn.Except(ProductiveStates).ToList());
            if (!string.IsNullOrWhiteSpace(unproductive))
            {
                tempP.ForEach(x => { var kvp = x.Split('-'); if (kvp[0].Equals(unproductive) || kvp[1].Contains(unproductive)) P.Remove($"{kvp[0]}-{kvp[1]}"); });
            }
            Vn.Remove(unproductive);
        }

        private List<string> GetUnitProductions()
        {
            List<string> unitProductions = new();
            foreach (string rule in P)
            {
                string[] kvp = rule.Split('-');
                string sourceState = kvp[0];
                string derivation = kvp[1];
                if (Vn.Contains(sourceState) && Vn.Contains(derivation))
                {
                    unitProductions.Add($"{sourceState}-{derivation}");
                }
            }
            return unitProductions;
        }
        private void CreateX()
        {
            foreach (var terminalSymbol in Vt)
            {
                XStates.Add(char.Parse(terminalSymbol), $"X{XStates.Count + 1}");
                Vn.Add($"X{XStates.Count}");
                P.Add($"X{XStates.Count}-{terminalSymbol}");
            }
            //ReplaceX();

        }
        private void ReplaceX()
        {
            var tempP = new List<string>(P);
            foreach (var rule in tempP.Where(x =>
            {
                var kvp = x.Split('-');
                return kvp[1].Length > 1 &&  Vt.Any(x => kvp[1].Any(y => y.Equals(char.Parse(x))));
            }))
            {
                var kvp = rule.Split('-');
                string derivation = kvp[1];
                P.Remove($"{kvp[0]}-{derivation}");
                string finalDerivation = "";
                foreach (var ch in derivation)
                {
                    if (Vt.Contains(ch+""))
                    {
                        finalDerivation += XStates[ch];
                    }
                    else
                    {
                        finalDerivation += ch;
                    }
                }
                
                P.Add($"{kvp[0]}-{finalDerivation}");
            }
        }
        private string SwitchTerminalToX(string twoStates)
        {
            string finalTwoStates = "";
            foreach (var ch in twoStates)
            {
                if (Vt.Contains(ch + ""))
                {
                    finalTwoStates += XStates[ch];
                }
                else
                {
                    finalTwoStates += ch;
                }
            }
            return finalTwoStates;
        }
        
        private string GetSpecificState(string state,int numberOfTheState)
        {
            int countTimesEncounteredAState = 0;

            for (int i = 0; i < state.Length; i++)
            {
                if (i + 1 < state.Length)
                {
                    var xState = "" + state[i] + "" + state[i + 1];
                    if (Vn.Contains(xState))
                    {
                        countTimesEncounteredAState++;
                        if (countTimesEncounteredAState == numberOfTheState)
                        {
                            return xState;
                        }
                    }
                }
                var regState = state[i] + "";
                if (Vn.Contains(regState))
                {
                    countTimesEncounteredAState++;
                    if (countTimesEncounteredAState == numberOfTheState)
                    {
                        return regState;
                    }
                }
                
            }
            return "";
        }
        private int GetIndexOfSpecificNumberOfState(string state,int nrOfState)
        {
            int countTimesEncounteredAState = 0;
           
            for (int i = 0; i < state.Length; i++)
            {
                if (i+1 < state.Length)
                {
                    var xState = ""+state[i] + ""+state[i + 1];
                    if (Vn.Contains(xState))
                    {
                        countTimesEncounteredAState++;
                    }
                }
                var regState = state[i] + "";
                if (Vn.Contains(regState))
                {
                    countTimesEncounteredAState++;
                }
                if (countTimesEncounteredAState == nrOfState)
                {
                    return i;
                }
            }
            return 0;
        }
        private void CreateYStates()
        {
            var tempP = new List<string>(P);
            foreach (var rule in tempP.Where(x => {
                var kvp = x.Split('-');
                return kvp[1].Length > 2;
            }))
            {
                var kvp = rule.Split('-');
                var derivation = kvp[1][1..];
                int i = 0;
                do
                {
                    bool isEven = derivation.Length % 2 == 0;
                    string twoStates = isEven ? new string(derivation.Skip(i).Take(2).ToArray()) : new string(derivation.Skip(i + 1).Take(2).ToArray());
                    string finalTwoStates = SwitchTerminalToX(twoStates);
                    if (!YStates.ContainsKey(finalTwoStates))
                    {
                        YStates.Add(finalTwoStates, $"Y{YStates.Count + 1}");
                        P.Add($"Y{YStates.Count}-{YStates.FirstOrDefault(x => x.Value.Equals($"Y{YStates.Count}")).Key}");
                        Vn.Add($"Y{YStates.Count}");
                    }
                    i += 2;
                    if (derivation.Length > i)
                    {
                        var oddState = isEven ? new string(derivation.Skip(i).ToArray()) : new string(derivation.Skip(i-2).Take(1).ToArray());
                        if (oddState.Length == 1)
                        {
                            string firstTwoStates = isEven ?  new string(derivation.Skip(i - 2).Take(2).ToArray()) : new string(derivation.Skip(i - 2).Take(1).ToArray());
                            finalTwoStates = isEven ?  SwitchTerminalToX(finalTwoStates) + SwitchTerminalToX(oddState) : SwitchTerminalToX(oddState) + SwitchTerminalToX(finalTwoStates);
                            if (!YStates.ContainsKey(finalTwoStates))
                            {
                                if (finalTwoStates.Length > 2)
                                {
                                    var firstState = GetSpecificState(finalTwoStates,1);
                                    var secondSetOfStates = finalTwoStates[GetIndexOfSpecificNumberOfState(finalTwoStates[1..],2)..];
                                    if (!YStates.ContainsKey(secondSetOfStates))
                                    {
                                        YStates.Add($"{secondSetOfStates}",$"Y{YStates.Count + 1}");
                                        P.Add($"Y{YStates.Count}-{YStates.FirstOrDefault(x => x.Value.Equals($"Y{YStates.Count}")).Key}");
                                        Vn.Add($"Y{YStates.Count}");
                                    }
                                    else
                                    {
                                        if (!YStates.ContainsKey($"{firstState}{YStates[secondSetOfStates]}"))
                                        {
                                            YStates.Add($"{firstState}{YStates[secondSetOfStates]}", $"Y{YStates.Count + 1}");
                                            P.Add($"Y{YStates.Count}-{YStates.FirstOrDefault(x => x.Value.Equals($"Y{YStates.Count}")).Key}");
                                            Vn.Add($"Y{YStates.Count}");
                                        }
                                    }
                                }
                                else
                                {
                                    YStates.Add($"{finalTwoStates}", $"Y{YStates.Count + 1}");
                                    P.Add($"Y{YStates.Count}-{YStates.FirstOrDefault(x => x.Value.Equals($"Y{YStates.Count}")).Key}");
                                    Vn.Add($"Y{YStates.Count}");
                                }
                            }
                        }
                    }
                    
                } while (i < derivation.Length - 2);
            }
            ReplaceX();
        }
        private void ReplaceY()
        {
            Dictionary<string, string> YStatesClone = new();
            var yStatesKeys = YStates.Keys.ToList();
            var yStatesValues = YStates.Values.ToList();
            foreach (var yStateKey in yStatesKeys)
            {
                var tempYStateKey = yStateKey;
                for (int i = 0; i < yStateKey.Length; i++)
                {
                    if (i + 1 <  yStateKey.Length)
                    {
                        var possibleY = "" + yStateKey[i] + "" + yStateKey[i + 1];
                        if (yStatesValues.Contains(possibleY))
                        {
                            tempYStateKey = tempYStateKey.Replace(possibleY, YStates.FirstOrDefault(x => x.Value.Equals(possibleY)).Key);
                            YStatesClone.Add(tempYStateKey, possibleY);
                        }
                    }
                }
                YStatesClone.Add(yStateKey, YStates[yStateKey]);
            }
            var tempP = new List<string>(P);
            foreach (var rule in tempP)
            {
                var kvp = rule.Split('-');
                var derivation = kvp[1];
                var stateCount = CountNumberOfStates(derivation);
                if (stateCount > 2)
                {
                    var firstDerivationState = GetSpecificState(kvp[1], 1);
                    P.Remove($"{kvp[0]}-{derivation}");
                    var YVal = YStatesClone[derivation[firstDerivationState.Length..]];
                    P.Add($"{kvp[0]}-{firstDerivationState}{YVal}");
                }
            }
        }
        public int CountNumberOfStates(string state)
        {
            int countTimesEncounteredAState = 0;

            for (int i = 0; i < state.Length; i++)
            {
                if (i + 1 < state.Length)
                {
                    var xState = "" + state[i] + "" + state[i + 1];
                    if (Vn.Contains(xState) )
                    {
                        countTimesEncounteredAState++;
                    }
                }
                var regState = state[i] + "";
                if (Vn.Contains(regState))
                {
                    countTimesEncounteredAState++;
                }

            }
            return countTimesEncounteredAState;
        }
        private Dictionary<string, List<KeyValuePair<char, string>>> CreateAutomatonRules()
        {
            var derivationRules = new Dictionary<string, List<KeyValuePair<char, string>>>();
            foreach (string rule in P)
            {
                string[] kvp = rule.Split('-');

                string q = kvp[1].Length == 1 ? kvp[1] : Vt.Any(x => x.Equals(kvp[1][0] + "")) ? kvp[1][1..] : kvp[1][0..];
                char transition = kvp[1].Length == 1 ? '\0' : Vt.Any(x => x.Equals(kvp[1][0] + "")) ? kvp[1][0] : '\0';

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
    }
}
