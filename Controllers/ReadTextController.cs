using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using AnalizadorDeLexico.Models;
using AnalizadorDeLexico.Entities;
using AnalizadorDeLexico.ExpressionsTree;
using System.Web.Routing;

namespace AnalizadorDeLexico.Controllers
{
    public class ReadTextController : Controller
    {
        public string FilePath = "";
        public static string Message;
        public static ExpressionTree tree = new ExpressionTree();
        // GET: ReadText
        public string regularExpression = "";
        public List<Node> nList;
        public static Dictionary<int, string> reservadasDictionary;
        public static Dictionary<string, List<string>> setsDictionary;
        public static Dictionary<string, List<string>> transicionesTotal;
        public static List<string> EstadosAceptacion;
        public static Dictionary<int, string> tokensDictionary = new Dictionary<int, string>();
        public ActionResult Read(string filename)
        {
            regularExpression = "";
            var path = Path.Combine(Server.MapPath("~/Archivo"), filename);
            FilePath = Server.MapPath("~/Archivo");
            var numberLine = 0;
            var palabrasReservadas = new List<string>();
            var specialCharsSets = new List<string>() { "=", "'", ".", "+", "(", ")" };
            var specialCharsTokens = new List<string>() { "*", "|", "+", "?", "'", "\"", "(", ")" };
            var specialCharsActions = new List<string>() { "{", "RESERVADAS", "()", "'" };
            var dictionaryMistakes = new Dictionary<int, string>();
            var rangeList = new List<string>();
            tokensDictionary = new Dictionary<int, string>();
            reservadasDictionary = new Dictionary<int, string>();
            setsDictionary = new Dictionary<string, List<string>>();
            var listCharset = new List<string>();
            var lettersM = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
            var lettersm = "abcdefghijklmnñopqrstuvwxyz";
            var space = "_";
            var digits = "0123456789";
            var nextChar = "=";
            var setName = "";
            var rangeL = new List<string>();
            var rangel = new List<string>();
            var rangeD = new List<string>();
            var range_ = new List<string>();
            var rangeCharset = new List<string>();
            var listCharacters = new List<string>();
            var character = "";
            var noToken = 0;
            var error = false;
            var cadena = "";
            var token = "";
            var tokenValue = "";
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    var line = reader.ReadLine();
                    numberLine++;
                    line = Regex.Replace(line, @"\s+", "");
                    //SETS
                    if (line.Substring(0, 4).ToUpper() == "SETS")
                    {
                        line = reader.ReadLine();
                        numberLine++;
                        while (line.Substring(0, line.Length).ToUpper() != "TOKENS")
                        {
                            line = Regex.Replace(line, @"\s+", "");
                            //ValidateSets(line, specialCharsSets);
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (error == true)
                                {
                                    nextChar = "=";
                                }
                                else
                                {
                                    switch (nextChar)
                                    {
                                        case "=":
                                            if (line.Contains("="))
                                            {
                                                if (line[i].ToString() == "=")
                                                {
                                                    if (setName != string.Empty)
                                                    {
                                                        setsDictionary.Add(setName, rangeList);
                                                        setName = string.Empty;
                                                        var newRangeList = new List<string>();
                                                        rangeList = newRangeList;
                                                    }
                                                    setName = line.Substring(0, i);
                                                    nextChar = "'";
                                                }
                                            }
                                            else if (!specialCharsSets.Contains(line[i].ToString()))
                                            {
                                                setName += line[i];
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "'":
                                            if (line[i].ToString() == "'")
                                            {
                                                nextChar = "caracter";
                                            }
                                            else if (line[i].ToString() == "C")
                                            {
                                                nextChar = "H";
                                            }
                                            else
                                            {
                                                if (line != "")
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    error = true;
                                                }
                                            }
                                            break;
                                        case "caracter":
                                            if ((i + 1) >= line.Length)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (!specialCharsSets.Contains(line[i].ToString()) && specialCharsSets.Contains(line[i + 1].ToString()))
                                                {
                                                    //if(LM Lm E D o C) Contains character
                                                    if (lettersM.Contains(line[i].ToString()))
                                                    {
                                                        if (rangeL.Count == 0)
                                                        {
                                                            rangeL.Add(line[i].ToString());
                                                        }
                                                        else
                                                        {
                                                            rangeL.Add(line[i].ToString());
                                                            if (rangeL[0].CompareTo(rangeL[1]) != -1)
                                                            {
                                                                dictionaryMistakes.Add(numberLine, line);
                                                                error = true;
                                                            }
                                                            else
                                                            {
                                                                var indexFirts = lettersM.IndexOf(lettersM.FirstOrDefault(x => x.ToString() == rangeL[0]));
                                                                var indexLast = lettersM.IndexOf(lettersM.FirstOrDefault(x => x.ToString() == rangeL[1]));
                                                                for (int k = indexFirts; k <= indexLast; k++)
                                                                {
                                                                    rangeList.Add(lettersM[k].ToString());
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lettersm.Contains(line[i].ToString()))
                                                    {
                                                        if (rangel.Count == 0)
                                                        {
                                                            rangel.Add(line[i].ToString());
                                                        }
                                                        else
                                                        {
                                                            rangel.Add(line[i].ToString());
                                                            if (rangel[0].CompareTo(rangel[1]) != -1)
                                                            {
                                                                dictionaryMistakes.Add(numberLine, line);
                                                                error = true;
                                                            }
                                                            else
                                                            {
                                                                var indexFirts = lettersm.IndexOf(lettersm.FirstOrDefault(x => x.ToString() == rangel[0]));
                                                                var indexLast = lettersm.IndexOf(lettersm.FirstOrDefault(x => x.ToString() == rangel[1]));
                                                                for (int k = indexFirts; k <= indexLast; k++)
                                                                {
                                                                    rangeList.Add(lettersm[k].ToString());
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (space.Contains(line[i].ToString()))
                                                    {
                                                        rangeList.Add(line[i].ToString());
                                                    }
                                                    if (digits.Contains(line[i].ToString()))
                                                    {
                                                        if (rangeD.Count == 0)
                                                        {
                                                            rangeD.Add(line[i].ToString());
                                                        }
                                                        else
                                                        {
                                                            rangeD.Add(line[i].ToString());
                                                            if (rangeD[0].CompareTo(rangeD[1]) != -1)
                                                            {
                                                                dictionaryMistakes.Add(numberLine, line);
                                                                error = true;
                                                            }
                                                            else
                                                            {
                                                                var indexFirts = digits.IndexOf(digits.FirstOrDefault(x => x.ToString() == rangeD[0]));
                                                                var indexLast = digits.IndexOf(digits.FirstOrDefault(x => x.ToString() == rangeD[1]));
                                                                for (int k = indexFirts; k <= indexLast; k++)
                                                                {
                                                                    rangeList.Add(digits[k].ToString());
                                                                }
                                                            }
                                                        }
                                                    }
                                                    nextChar = "'";
                                                }
                                                else if (line[i].ToString() == ".")
                                                {
                                                    nextChar = ".";
                                                }
                                                else if (line[i].ToString() == "+")
                                                {
                                                    nextChar = "'";
                                                }
                                                else if (specialCharsSets.Contains(line[i].ToString()) && specialCharsSets.Contains(line[i + 1].ToString()))
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    error = true;
                                                }
                                                else
                                                {
                                                    nextChar = "=";
                                                }
                                            }
                                            break;
                                        case ".":
                                            if (line[i].ToString() == ".")
                                            {
                                                nextChar = "'";
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "C":
                                            if (line[i].ToString().ToUpper() != "C")
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            nextChar = "H";
                                            break;
                                        case "H":
                                            if (line[i].ToString().ToUpper() != "H")
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            nextChar = "R";
                                            break;
                                        case "R":
                                            if (line[i].ToString().ToUpper() != "R")
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            nextChar = "(";
                                            break;
                                        case "(":
                                            if (line[i].ToString() == "(")
                                            {
                                                nextChar = "charset";
                                            }
                                            else
                                            {
                                                if (line != "")
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    error = true;
                                                }
                                            }
                                            break;
                                        case "charset":
                                            if (!specialCharsSets.Contains(line[i].ToString()))
                                            {
                                                character += line[i].ToString();
                                            }
                                            else if (line[i].ToString() == ")")
                                            {
                                                if (rangeCharset.Count == 0)
                                                {
                                                    rangeCharset.Add(character);
                                                    character = "";
                                                    nextChar = "punto";
                                                }
                                                else
                                                {
                                                    rangeCharset.Add(character);
                                                    for (int pos = Convert.ToInt32(rangeCharset[0]); pos <= Convert.ToInt32(rangeCharset[1]); pos++)
                                                    {
                                                        var enByte = Convert.ToByte(pos);
                                                        var caracter = Convert.ToChar(enByte);
                                                        rangeList.Add(caracter.ToString());
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "punto":
                                            if (line[i].ToString() == ".")
                                            {
                                                nextChar = "punto2";
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "punto2":
                                            if (line[i].ToString() == ".")
                                            {
                                                nextChar = "C";
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            line = reader.ReadLine();
                            numberLine++;
                            error = false;
                        }
                        setsDictionary.Add(setName, rangeList);
                    }
                    //TOKENS
                    if (line.Substring(0, 6).ToUpper() == "TOKENS")
                    {
                        line = reader.ReadLine();
                        numberLine++;
                        nextChar = "=";
                        while (line.Substring(0, line.Length).ToUpper() != "ACTIONS")
                        {
                            line = line.ToUpper();
                            line = Regex.Replace(line, @"\s+", "");
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (error == true)
                                {
                                    nextChar = "=";
                                }
                                else
                                {
                                    switch (nextChar)
                                    {
                                        case "=":
                                            if (line.Contains("="))
                                            {
                                                if (line[i].ToString() == "=")
                                                {
                                                    if (tokenValue != string.Empty)
                                                    {
                                                        tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                        if (!tokensDictionary.Keys.Contains(noToken))
                                                        {
                                                            tokensDictionary.Add(noToken, tokenValue);
                                                            tokenValue = string.Empty;
                                                        }
                                                        else
                                                        {
                                                            dictionaryMistakes.Add(numberLine, line);
                                                            error = true;
                                                        }
                                                    }
                                                    if (line.Substring(0, i - 1) == "TOKEN")
                                                    {
                                                        noToken = Convert.ToInt32(line.Substring(5, 1));
                                                    }
                                                    else if (line.Substring(0, i - 2) == "TOKEN")
                                                    {
                                                        noToken = Convert.ToInt32(line.Substring(5, 2));
                                                    }
                                                    else if (token.Substring(0, 5) == "TOKEN")
                                                    {
                                                        noToken = Convert.ToInt32(line.Substring(5, 1));
                                                    }
                                                    else
                                                    {
                                                        dictionaryMistakes.Add(numberLine, line);
                                                        error = true;
                                                    }
                                                    nextChar = "set";
                                                }
                                            }
                                            else if (!specialCharsTokens.Contains(line[i].ToString()))
                                            {
                                                token += line[i];
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "set":
                                            if ((i + 1) >= line.Length)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (!specialCharsTokens.Contains(line[i].ToString()))
                                                {
                                                    cadena += line[i].ToString();
                                                    if (cadena == "TOKEN")
                                                    {
                                                        nextChar = "=";
                                                        cadena = "";
                                                        break;
                                                    }
                                                    if (setsDictionary.ContainsKey(cadena))
                                                    {
                                                        listCharacters.Add(cadena);
                                                        cadena = "";
                                                    }
                                                    if (specialCharsTokens.Contains(line[i + 1].ToString()))
                                                    {
                                                        foreach (var item in listCharacters)
                                                        {
                                                            tokenValue = tokenValue + item + ".";
                                                        }
                                                        listCharacters = new List<string>();
                                                        if (line[i + 1].ToString() == "'")
                                                        {
                                                            nextChar = "set";
                                                        }
                                                        else
                                                        {
                                                            nextChar = "simbol";
                                                        }
                                                    }
                                                }
                                                else if (line[i].ToString() == "'")
                                                {
                                                    nextChar = "character";
                                                }
                                                else if (line[i].ToString() == "(")
                                                {
                                                    tokenValue += line[i].ToString();
                                                    nextChar = "set";
                                                }
                                                else if (line[i].ToString() == "|")
                                                {
                                                    tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                    tokenValue += line[i].ToString();
                                                    nextChar = "set";
                                                }
                                                else
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    error = true;
                                                }
                                            }
                                            break;
                                        case "simbol":
                                            if (cadena != string.Empty)
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            else
                                            {
                                                if (line[i].ToString() == "|")
                                                {
                                                    tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                    tokenValue += line[i].ToString();
                                                    nextChar = "set";
                                                }
                                                else if (line[i].ToString() == "(")
                                                {
                                                    tokenValue += line[i].ToString();
                                                    nextChar = "set";
                                                }
                                                else if (line[i].ToString() == ")")
                                                {
                                                    tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                    tokenValue += line[i].ToString() + ".";
                                                    nextChar = "pow";
                                                }
                                                else if (line[i].ToString() == "*" || line[i].ToString() == "+" || line[i].ToString() == "?")
                                                {
                                                    tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                    tokenValue += line[i].ToString() + ".";
                                                    nextChar = "set";
                                                }
                                            }
                                            break;
                                        case "character":
                                            if (line[i].ToString() == "'")
                                            {
                                                if (cadena != string.Empty)
                                                {
                                                    listCharacters.Add(cadena);
                                                    cadena = "";
                                                }
                                                else
                                                {
                                                    listCharacters.Add(line[i].ToString());
                                                }
                                                nextChar = "'";
                                            }
                                            else if (line[i].ToString() == "\"")
                                            {
                                                listCharacters.Add(line[i].ToString());
                                                nextChar = "'";
                                            }
                                            else
                                            {
                                                cadena += line[i].ToString();
                                                if (setsDictionary.ContainsKey(cadena))
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    cadena = "";
                                                }
                                                if (specialCharsTokens.Contains(line[i + 1].ToString()))
                                                {
                                                    listCharacters.Add(cadena);
                                                    cadena = "";
                                                    nextChar = "'";
                                                }
                                            }
                                            break;
                                        case "'":
                                            if (line[i].ToString() != "'")
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            else
                                            {
                                                foreach (var item in listCharacters)
                                                {
                                                    tokenValue = tokenValue + "'" + item + "'" + ".";
                                                }
                                                listCharacters = new List<string>();
                                                nextChar = "set";
                                            }
                                            break;
                                        case "pow":
                                            if (line[i].ToString() == "*" || line[i].ToString() == "+" || line[i].ToString() == "?")
                                            {
                                                tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                tokenValue += line[i].ToString() + ".";
                                            }
                                            nextChar = "set";
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            line = reader.ReadLine();
                            line = Regex.Replace(line, @"\s+", "");
                            numberLine++;
                        }
                        tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                        if (error == false)
                        {
                            if (!tokensDictionary.Keys.Contains(noToken))
                            {
                                tokensDictionary.Add(noToken, tokenValue);
                                tokenValue = string.Empty;
                            }
                            else
                            {
                                dictionaryMistakes.Add(numberLine, line);
                                error = true;
                            }
                            foreach (var item in tokensDictionary)
                            {
                                regularExpression += "(" + item.Value + ")" + "|";
                            }
                            regularExpression = regularExpression.Substring(0, regularExpression.Length - 1) + "#";
                        }
                    }
                    //ACTIONS
                    if (line.Substring(0, 7).ToUpper() == "ACTIONS")
                    {
                        line = reader.ReadLine();
                        numberLine++;
                        line = Regex.Replace(line, @"\s+", "");
                        if (line.Substring(0, line.Length).ToUpper() == "RESERVADAS()")
                        {
                            line = reader.ReadLine();
                            numberLine++;
                            line = Regex.Replace(line, @"\s+", "");
                            if (line == "{")
                            {
                                line = reader.ReadLine();
                                numberLine++;
                                line = Regex.Replace(line, @"\s+", "");
                                while (!line.Contains("}") && error == false)
                                {
                                    line = line.ToUpper();
                                    line = Regex.Replace(line, @"\s+", "");
                                    if (line.Contains("="))
                                    {
                                        var valores = line.Split('=');
                                        if (valores[0] == "ERROR")
                                        {
                                            dictionaryMistakes.Add(numberLine, line);
                                            error = true;
                                        }
                                        else
                                        {
                                            noToken = Convert.ToInt32(valores[0]);
                                        }
                                        var palabra = valores[1];
                                        if (palabra.Substring(0, 1) == "'" && palabra.Substring(palabra.Length - 1, 1) == "'")
                                        {
                                            tokenValue = palabra.Substring(1, palabra.Length - 2);
                                            if (!reservadasDictionary.ContainsKey(noToken))
                                            {
                                                reservadasDictionary.Add(noToken, tokenValue);
                                                palabrasReservadas.Add(tokenValue);
                                                tokenValue = "";
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                        }
                                        else
                                        {
                                            dictionaryMistakes.Add(numberLine, line);
                                            error = true;
                                        }
                                    }
                                    else
                                    {
                                        dictionaryMistakes.Add(numberLine, line);
                                        error = true;
                                    }
                                    line = reader.ReadLine();
                                    numberLine++;
                                }
                            }
                            else
                            {
                                dictionaryMistakes.Add(numberLine, line);
                                error = true;
                            }
                        }
                        else
                        {
                            dictionaryMistakes.Add(numberLine, line);
                            error = true;
                        }
                    }
                    else
                    {
                        dictionaryMistakes.Add(numberLine, line);
                    }
                }
            }
            if (dictionaryMistakes.Count > 0)
            {
                MistakesListViewModel modelList = new MistakesListViewModel();
                modelList.ListMistakes = new List<MistakesEntity>();
                foreach (var item in dictionaryMistakes)
                {
                    MistakesEntity model = new MistakesEntity();
                    model.Line = item.Key;
                    model.Mistake = item.Value;
                    modelList.ListMistakes.Add(model);
                }
                TempData["Mistakes"] = modelList;
                return RedirectToAction("Mistakes");
            }
            else
            {
                var postList = new List<string>();
                var nodesList = new List<Node>();
                postList = tree.Calculator(regularExpression, setsDictionary);
                nodesList = tree.Tree(postList);
                nList = nodesList;
                ArbolViewModel arbol = new ArbolViewModel();
                arbol.DictionarySets = new Dictionary<string, string>();
                foreach (var item in setsDictionary)
                {
                    var temporalList = "";
                    foreach (var ch in item.Value)
                    {
                        temporalList += ch;
                    }
                    arbol.DictionarySets.Add(item.Key, temporalList);
                }
                arbol.ListNodes = new List<NodeEntity>();
                foreach (var item in nodesList)
                {
                    NodeEntity model = new NodeEntity();
                    model.EsHoja = item.EsHoja;
                    model.IsNull = item.IsNull;
                    model.Value = item.Value;
                    if (item.Number != null)
                    {
                        model.Number = item.Number;
                    }
                    foreach (var number in item.First)
                    {
                        model.First = model.First + number.Number + ",";
                    }
                    foreach (var number1 in item.Last)
                    {
                        model.Last = model.Last + number1.Number + ",";
                    }
                    if (item.Follow != null)
                    {
                        foreach (var number2 in item.Follow)
                        {
                            model.Follow = model.Follow + number2 + ",";
                        }
                    }
                    arbol.ListNodes.Add(model);
                }
                TempData["Tree"] = arbol;
                return RedirectToAction("TableOfTree");
            }
        }
        public ActionResult Mistakes()
        {
            var modelList = TempData["Mistakes"] as MistakesListViewModel;
            return View(modelList);
        }
        public ActionResult TableOfTree()
        {
            var arbol = TempData["Tree"] as ArbolViewModel;
            return View(arbol);
        }
        public ActionResult Automata()
        {
            var statesTable = new Dictionary<string, string>();
            var terminalsWithTeam = new Dictionary<string, List<string>>();
            var listConjuntos = new List<List<string>>();
            var nodesList = new List<Node>();
            nodesList = tree.GetList();
            var listNoTerminals = new List<string>();
            var listNoTerminalsNumbers = new List<string>();
            listNoTerminals = tree.GetNoTerminalList();
            foreach (var item in listNoTerminals)
            {
                terminalsWithTeam.Add(item, new List<string>());
            }
            var firstRaiz = new List<string>();
            var firstRaizString = "";
            foreach (var item in nodesList[nodesList.Count - 1].First)
            {
                firstRaiz.Add(item.Number);
                firstRaizString += item.Number + ",";
            }
            listConjuntos.Add(firstRaiz);
            statesTable.Add("Estado1", firstRaizString);
            var conjunto = firstRaiz;
            var posicion = 0;
            var pos = 1;
            var table = new List<Dictionary<string, List<string>>>();
            var tableTransicions = CreateTransitions(listConjuntos, nodesList, terminalsWithTeam, statesTable, posicion, pos, table);
            var model = new AutomataViewModel();
            model.Conjuntos = new List<string>();
            model.EstadosAceptacion = new List<string>();
            model.NoTerminales = new List<string>();
            model.NombreConjuntos = new List<List<string>>();
            model.NoTerminalesConjunto = new List<List<string>>();
            var conjuntosDictionary = new Dictionary<string, string>();
            foreach (var item in tableTransicions)
            {
                model.Conjuntos.Add(item.Key);
                var separador = item.Key.Split('=');
                conjuntosDictionary.Add(separador[0], separador[1]);
                var num = separador[1].Split(',');
                for (int i = 0; i < num.Length; i++)
                {
                    if (num[i] == tree.GetFollowHashtag())
                    {
                        model.EstadosAceptacion.Add(item.Key);
                    }
                }
            }
            foreach (var item in tableTransicions.Values)
            {
                var listTemporal = new List<string>();
                foreach (var i in item.Keys)
                {
                    if (!model.NoTerminales.Contains(i))
                    {
                        model.NoTerminales.Add(i);
                    }
                }
                foreach (var i in item.Values)
                {
                    var temporalJuntar = "";
                    foreach (var e in i)
                    {
                        temporalJuntar = temporalJuntar + e + ",";
                    }
                    if (conjuntosDictionary.ContainsValue(temporalJuntar))
                    {
                        var c = conjuntosDictionary.FirstOrDefault(x => x.Value == temporalJuntar);
                        listTemporal.Add(c.Key);
                    }
                    else
                    {
                        listTemporal.Add(temporalJuntar);
                    }
                }
                model.NoTerminalesConjunto.Add(listTemporal);
            }
            EstadosAceptacion = new List<string>();
            EstadosAceptacion = model.EstadosAceptacion;
            //Estados y direcciones
            transicionesTotal = new Dictionary<string, List<string>>();
            for (int noEstado = 0; noEstado < model.NoTerminalesConjunto.Count; noEstado++)
            {
                var numEstado = noEstado + 1;
                var transicionPorEstado = new List<string>();
                for (int noTerminal = 0; noTerminal < model.NoTerminales.Count; noTerminal++)
                {
                    if (model.NoTerminalesConjunto[noEstado][noTerminal] != string.Empty)
                    {
                        var noTerminalEstado = model.NoTerminales[noTerminal] + "|" +
                            model.NoTerminalesConjunto[noEstado][noTerminal];
                        transicionPorEstado.Add(noTerminalEstado);
                    }
                }
                transicionesTotal.Add(numEstado.ToString(), transicionPorEstado);
            }
            return View(model);
        }

        public Dictionary<string, Dictionary<string, List<string>>> CreateTransitions(List<List<string>> listConjuntos, List<Node> nodesList, Dictionary<string, List<string>> terminalsWithTeam, Dictionary<string, string> statesTable, int posicion, int pos, List<Dictionary<string, List<string>>> noTerminalsFiles)
        {
            if (posicion < listConjuntos.Count)
            {
                var nodosConjunto = new List<Node>();
                var temporalList = new List<string>();
                foreach (var number in listConjuntos[posicion])
                {
                    foreach (var nodo in nodesList)
                    {
                        if (nodo.Number == number)
                        {
                            nodosConjunto.Add(nodo);
                        }
                    }
                }

                foreach (var nodo in nodosConjunto)
                {
                    //En lugar de comparar los numeros comparar que el valor sea igual
                    if (nodo.Value != "#")
                    {
                        var noTerminal = terminalsWithTeam.FirstOrDefault(x => x.Key == nodo.Value);
                        foreach (var follow in nodo.Follow)
                        {
                            if (!noTerminal.Value.Contains(follow))
                            {
                                noTerminal.Value.Add(follow);
                            }
                        }
                    }
                }
                foreach (var noTerminal in terminalsWithTeam)
                {
                    var conjuntoString = "";
                    foreach (var item in noTerminal.Value)
                    {
                        conjuntoString += item + ",";
                    }
                    if (!statesTable.ContainsValue(conjuntoString) && noTerminal.Value.Count > 0)
                    {
                        pos++;
                        listConjuntos.Add(noTerminal.Value);
                        statesTable.Add("Estado" + pos, conjuntoString);
                    }
                }
                noTerminalsFiles.Add(terminalsWithTeam);
                posicion++;
                var noTerminalsWithTeam = new Dictionary<string, List<string>>();
                foreach (var item in terminalsWithTeam)
                {
                    noTerminalsWithTeam.Add(item.Key, new List<string>());
                }
                return CreateTransitions(listConjuntos, nodesList, noTerminalsWithTeam, statesTable, posicion, pos, noTerminalsFiles);
            }
            else
            {
                //Diccionario donde la llave sea por ejemplo "Estado1 = 1234"
                //Y el valor sea una lista que va a tener todos los conjuntos
                var cont = 0;
                var tableTransitions = new Dictionary<string, Dictionary<string, List<string>>>();
                foreach (var item in statesTable)
                {
                    tableTransitions.Add(item.Key + "=" + item.Value, noTerminalsFiles[cont]);
                    cont++;
                }
                return tableTransitions;
            }
        }

        public ActionResult Archivo(string filename)
        {
            var estadosAceptacion = new List<string>();
            foreach (var item in EstadosAceptacion)
            {
                var valores = item.Split('=');
                estadosAceptacion.Add(valores[0]);
            }

            var path2 = Path.Combine(Server.MapPath("~/Archivo"), "Codigo.cs");
            using (var writeStream = new FileStream(path2, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(writeStream))
                {
                    writer.WriteLine("using System;");
                    writer.WriteLine("using System.IO;");
                    writer.WriteLine("using System.Collections.Generic;");
                    writer.WriteLine("using System.Linq;");
                    writer.WriteLine("using System.Text.RegularExpressions;");
                    writer.WriteLine("class Program");
                    writer.WriteLine("{");
                    writer.WriteLine("static void Main(string[] args)");
                    writer.WriteLine("{");
                    writer.WriteLine("var cadena = string.Empty;");
                    writer.WriteLine("var resultTokenList = new List<string>();");
                    writer.WriteLine("var estadoPresente = 1;");
                    writer.WriteLine("var estado = 1;");
                    writer.WriteLine("var posicion = 1;");
                    writer.WriteLine("var aceptacion = true;");
                    writer.WriteLine("var error = false;");
                    writer.WriteLine("var tokensDictionary = new Dictionary<int, string>()");
                    writer.WriteLine("{");
                    var cont = 0;
                    foreach (var item in tokensDictionary)
                    {
                        cont++;
                        if (cont == tokensDictionary.Count)
                        {
                            if (item.Value.Contains("\""))
                            {
                                item.Value.Replace("\"", "\\\"");
                            }
                            if (item.Value.Contains("\'"))
                            {
                                item.Value.Replace("\'", "\\\'");
                            }
                            writer.WriteLine("{Convert.ToInt32(" + item.Key + "), \"" + item.Value + "\"}");
                        }
                        else
                        {
                            writer.WriteLine("{Convert.ToInt32(" + item.Key + "), \"" + item.Value + "\"}" + ",");
                        }
                    }
                    writer.WriteLine("};");
                    writer.WriteLine(" var reservadasDictionary = new Dictionary<int, string>()");
                    writer.WriteLine("{");
                    cont = 0;
                    foreach (var item in reservadasDictionary)
                    {
                        cont++;
                        if(cont== reservadasDictionary.Count)
                        {
                            writer.WriteLine("{Convert.ToInt32(" + item.Key + "), \"" + item.Value + "\"}");
                        }
                        else
                        {
                            writer.WriteLine("{Convert.ToInt32(" + item.Key + "), \"" + item.Value + "\"}" + ",");
                        }
                    }
                    writer.WriteLine("};");

                    writer.WriteLine(" var estadosAceptacion = new List<string>();");
                    foreach (var item in estadosAceptacion)
                    {
                        writer.WriteLine("estadosAceptacion.Add(\"" + item + "\");");
                    }

                    writer.WriteLine(" var setsDictionary = new Dictionary<string, List<string>>();");
                    writer.WriteLine(" var listTemporal = new List<string>();");
                    foreach (var item in setsDictionary)
                    {
                        writer.WriteLine("listTemporal = new List<string>();");
                        foreach (var c in item.Value)
                        {
                            writer.WriteLine("listTemporal.Add(\"" + c + "\");");
                        }
                        writer.WriteLine("setsDictionary.Add(\"" + item.Key + "\", listTemporal);");
                    }
                    writer.WriteLine("var transicionesTotal = new Dictionary<string, List<string>>();");
                    foreach (var item in transicionesTotal)
                    {
                        writer.WriteLine("listTemporal = new List<string>();");
                        foreach (var c in item.Value)
                        {
                            writer.WriteLine("listTemporal.Add(\"" + c + "\");");
                        }
                        writer.WriteLine("transicionesTotal.Add(\"" + item.Key + "\", listTemporal);");
                    }
                    writer.WriteLine("Console.WriteLine(\"Ingrese la ruta en la que se encuentra el archivo:\");");
                    writer.WriteLine("var ruta = Console.ReadLine();");
                    writer.WriteLine("Console.WriteLine(\"Ingrese el nombre del archivo a evaluar\");");
                    writer.WriteLine("var nombreArchivo = Console.ReadLine();");
                    writer.WriteLine("var path = Path.Combine(ruta, nombreArchivo);");
                    writer.WriteLine("using (var stream = new FileStream(path, FileMode.Open))");
                    writer.WriteLine("{");
                    writer.WriteLine("using (var reader = new StreamReader(stream))");
                    writer.WriteLine("{");
                    writer.WriteLine("cadena = reader.ReadToEnd();");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("cadena = Regex.Replace(cadena, @\"\\s+\", \"\");");
                    writer.WriteLine("var nameSet = \"\";");
                    writer.WriteLine("var palabra = \"\";");
                    writer.WriteLine("for (int i = 0; i <= cadena.Length; i++)");
                    writer.WriteLine("{");
                    writer.WriteLine("if (error == false)");
                    writer.WriteLine("{");
                    writer.WriteLine("switch (posicion)");
                    writer.WriteLine("{");
                    writer.WriteLine("case 1:");
                    writer.WriteLine("nameSet = string.Empty;");
                    writer.WriteLine("foreach (var set in setsDictionary)");
                    writer.WriteLine("{");
                    writer.WriteLine("if (set.Value.Contains(cadena[i].ToString()))");
                    writer.WriteLine("{");
                    writer.WriteLine("nameSet = set.Key;");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("var tok = tokensDictionary.FirstOrDefault(x => x.Value.Contains(\"'\" + cadena[i].ToString() + \"'\"));");
                    writer.WriteLine("if (nameSet == string.Empty)");
                    writer.WriteLine("{");
                    writer.WriteLine("nameSet = \"'\" + cadena[i].ToString() + \"'\"; ");
                    writer.WriteLine("var temporalList = new List<string>();");
                    writer.WriteLine("temporalList.Add(cadena[i].ToString());");
                    writer.WriteLine("setsDictionary.Add(nameSet, temporalList);");
                    writer.WriteLine("}");
                    writer.WriteLine("if (transicionesTotal.ContainsKey(estadoPresente.ToString()))");
                    writer.WriteLine("{");
                    writer.WriteLine("var estadoActual = transicionesTotal.FirstOrDefault(x => x.Key == estadoPresente.ToString());");
                    writer.WriteLine("var sigTransicion = estadoActual.Value.FirstOrDefault(x => x.Split('|')[0] == nameSet);");
                    writer.WriteLine("var separador = sigTransicion.Split('|');");
                    writer.WriteLine("estado = Convert.ToInt32(separador[1].Substring(6, 1));");
                    writer.WriteLine("posicion = 2;");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("error = true;");
                    writer.WriteLine("}");
                    writer.WriteLine("break;");

                    writer.WriteLine("case 2:");
                    writer.WriteLine("var e = \"Estado\" + estado;");
                    writer.WriteLine("if (estadosAceptacion.Contains(e))");
                    writer.WriteLine("{");
                    writer.WriteLine("if (palabra == string.Empty)");
                    writer.WriteLine("{");
                    writer.WriteLine("i = i - 1;");
                    writer.WriteLine("}");
                    writer.WriteLine("if (aceptacion == false)");
                    writer.WriteLine("{");
                    writer.WriteLine("i = i - 1;");
                    writer.WriteLine("aceptacion = true;");
                    writer.WriteLine("}");
                    writer.WriteLine("var Set = setsDictionary.FirstOrDefault(x => x.Key == nameSet);");
                    writer.WriteLine("var maspequeño = reservadasDictionary.Values.OrderBy(s => s.Length).First();");
                    writer.WriteLine("if (Set.Value.Contains(cadena[i].ToString()))");
                    writer.WriteLine("{");
                    writer.WriteLine("palabra += cadena[i].ToString();");
                    writer.WriteLine("if (reservadasDictionary.Values.Contains(palabra.ToUpper()))");
                    writer.WriteLine("{");
                    writer.WriteLine("var reservada = reservadasDictionary.FirstOrDefault(x => x.Value == palabra.ToUpper());");
                    writer.WriteLine("resultTokenList.Add(reservada.Key.ToString() + \" | \" + reservada.Value);");
                    writer.WriteLine("estadoPresente = estado;");
                    writer.WriteLine("posicion = 1;");
                    writer.WriteLine("palabra = \"\";");
                    writer.WriteLine("}");
                    writer.WriteLine("else if (palabra.Length > maspequeño.Length)");
                    writer.WriteLine("{");
                    writer.WriteLine("foreach (var item in reservadasDictionary.Values)");
                    writer.WriteLine("{");
                    writer.WriteLine("if (palabra.ToUpper().Contains(item))");
                    writer.WriteLine("{");
                    writer.WriteLine("var arrayOne = palabra.ToUpper().ToCharArray();");
                    writer.WriteLine("var arrayTwo = item.ToCharArray();");
                    writer.WriteLine("var differentChars = arrayOne.Except(arrayTwo);");
                    writer.WriteLine("var token = tokensDictionary.FirstOrDefault(x => x.Value.Contains(nameSet));");
                    writer.WriteLine("foreach (var character in differentChars)");
                    writer.WriteLine("{");
                    writer.WriteLine("resultTokenList.Add(token.Key.ToString() + \" | \" + character.ToString().ToLower());");
                    writer.WriteLine("}");
                    writer.WriteLine("var reservada = reservadasDictionary.FirstOrDefault(x => x.Value == item.ToUpper());");
                    writer.WriteLine("resultTokenList.Add(reservada.Key.ToString() + \" | \" + reservada.Value);");
                    writer.WriteLine("estadoPresente = estado;");
                    writer.WriteLine("posicion = 1;");
                    writer.WriteLine("palabra = \"\"; ");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("if (i == cadena.Length - 1)");
                    writer.WriteLine("{");
                    writer.WriteLine("cadena = cadena + \"#\"; ");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("posicion = 2;");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("if (palabra.Length > 1)");
                    writer.WriteLine("{");
                    writer.WriteLine("if (Set.Value.Contains(palabra[0].ToString()))");
                    writer.WriteLine("{");
                    writer.WriteLine("var token = tokensDictionary.FirstOrDefault(x => x.Value.Contains(nameSet));");
                    writer.WriteLine("foreach (var item in palabra)");
                    writer.WriteLine("{");
                    writer.WriteLine("resultTokenList.Add(token.Key.ToString() + \" | \" + item.ToString());");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("var palabraConComas = \"\";");
                    writer.WriteLine("foreach (var item in palabra)");
                    writer.WriteLine("{");
                    writer.WriteLine("palabraConComas += \"'\" + item + \"'\" + \".\";");
                    writer.WriteLine("}");
                    writer.WriteLine("palabraConComas = palabraConComas.Substring(0, palabraConComas.Length - 1);");
                    writer.WriteLine("var token = tokensDictionary.FirstOrDefault(x => x.Value.Contains(palabraConComas));");
                    writer.WriteLine("resultTokenList.Add(token.Key.ToString() + \" | \" + palabra);");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("var token = tokensDictionary.FirstOrDefault(x => x.Value.Contains(nameSet));");
                    writer.WriteLine("resultTokenList.Add(token.Key.ToString() + \" | \" + palabra);");
                    writer.WriteLine("if (i == cadena.Length - 1 && cadena[i].ToString() != \"#\")");
                    writer.WriteLine("{");
                    writer.WriteLine("cadena = cadena + \"#\"; ");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("if (cadena[i].ToString() != cadena[cadena.Length - 1].ToString())");
                    writer.WriteLine("{");
                    writer.WriteLine("i -= 1;");
                    writer.WriteLine("posicion = 1;");
                    writer.WriteLine("estado = 1;");
                    writer.WriteLine("estadoPresente = 1;");
                    writer.WriteLine("palabra = \"\";");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("i++;");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("i = i - 1;");
                    writer.WriteLine("palabra += cadena[i].ToString();");
                    writer.WriteLine("estadoPresente = estado;");
                    writer.WriteLine("posicion = 1;");
                    writer.WriteLine("aceptacion = false;");
                    writer.WriteLine("}");
                    writer.WriteLine("break;");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("break;");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("if (error == false)");
                    writer.WriteLine("{");
                    writer.WriteLine("foreach (var item in resultTokenList)");
                    writer.WriteLine("{");
                    writer.WriteLine("Console.WriteLine(item);");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.WriteLine("else");
                    writer.WriteLine("{");
                    writer.WriteLine("Console.WriteLine(\"Error\");");
                    writer.WriteLine("}");
                    writer.WriteLine("Console.ReadLine();");
                    writer.WriteLine("}");
                    writer.WriteLine("}");
                    writer.Close();
                }
                writeStream.Close();
            }

            return View();
        }
    }
}