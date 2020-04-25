using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
class Program
{
static void Main(string[] args)
{
var cadena = string.Empty;
var resultTokenList = new List<string>();
var estadoPresente = 1;
var estado = 1;
var posicion = 1;
var aceptacion = true;
var error = false;
var tokensDictionary = new Dictionary<int, string>()
{
{Convert.ToInt32(1), "DIGITO.DIGITO*"},
{Convert.ToInt32(2), "'='"},
{Convert.ToInt32(3), "':'.'='"},
{Convert.ToInt32(4), "LETRA.(LETRA|DIGITO)*"}
};
 var reservadasDictionary = new Dictionary<int, string>()
{
{Convert.ToInt32(5), "PROGRAM"},
{Convert.ToInt32(6), "INCLUDE"},
{Convert.ToInt32(7), "CONST"},
{Convert.ToInt32(8), "TYPE"}
};
 var estadosAceptacion = new List<string>();
estadosAceptacion.Add("Estado2");
estadosAceptacion.Add("Estado3");
estadosAceptacion.Add("Estado5");
 var setsDictionary = new Dictionary<string, List<string>>();
 var listTemporal = new List<string>();
listTemporal = new List<string>();
listTemporal.Add("A");
listTemporal.Add("B");
listTemporal.Add("C");
listTemporal.Add("D");
listTemporal.Add("E");
listTemporal.Add("F");
listTemporal.Add("G");
listTemporal.Add("H");
listTemporal.Add("I");
listTemporal.Add("J");
listTemporal.Add("K");
listTemporal.Add("L");
listTemporal.Add("M");
listTemporal.Add("N");
listTemporal.Add("Ñ");
listTemporal.Add("O");
listTemporal.Add("P");
listTemporal.Add("Q");
listTemporal.Add("R");
listTemporal.Add("S");
listTemporal.Add("T");
listTemporal.Add("U");
listTemporal.Add("V");
listTemporal.Add("W");
listTemporal.Add("X");
listTemporal.Add("Y");
listTemporal.Add("Z");
listTemporal.Add("a");
listTemporal.Add("b");
listTemporal.Add("c");
listTemporal.Add("d");
listTemporal.Add("e");
listTemporal.Add("f");
listTemporal.Add("g");
listTemporal.Add("h");
listTemporal.Add("i");
listTemporal.Add("j");
listTemporal.Add("k");
listTemporal.Add("l");
listTemporal.Add("m");
listTemporal.Add("n");
listTemporal.Add("ñ");
listTemporal.Add("o");
listTemporal.Add("p");
listTemporal.Add("q");
listTemporal.Add("r");
listTemporal.Add("s");
listTemporal.Add("t");
listTemporal.Add("u");
listTemporal.Add("v");
listTemporal.Add("w");
listTemporal.Add("x");
listTemporal.Add("y");
listTemporal.Add("z");
listTemporal.Add("_");
setsDictionary.Add("LETRA", listTemporal);
listTemporal = new List<string>();
listTemporal.Add("0");
listTemporal.Add("1");
listTemporal.Add("2");
listTemporal.Add("3");
listTemporal.Add("4");
listTemporal.Add("5");
listTemporal.Add("6");
listTemporal.Add("7");
listTemporal.Add("8");
listTemporal.Add("9");
setsDictionary.Add("DIGITO", listTemporal);
var transicionesTotal = new Dictionary<string, List<string>>();
listTemporal = new List<string>();
listTemporal.Add("DIGITO|Estado2");
listTemporal.Add("'='|Estado3");
listTemporal.Add("':'|Estado4");
listTemporal.Add("LETRA|Estado5");
transicionesTotal.Add("1", listTemporal);
listTemporal = new List<string>();
listTemporal.Add("DIGITO|Estado2");
transicionesTotal.Add("2", listTemporal);
listTemporal = new List<string>();
transicionesTotal.Add("3", listTemporal);
listTemporal = new List<string>();
listTemporal.Add("'='|Estado3");
transicionesTotal.Add("4", listTemporal);
listTemporal = new List<string>();
listTemporal.Add("DIGITO|Estado5");
listTemporal.Add("LETRA|Estado5");
transicionesTotal.Add("5", listTemporal);
Console.WriteLine("Ingrese la ruta en la que se encuentra el archivo:");
var ruta = Console.ReadLine();
Console.WriteLine("Ingrese el nombre del archivo a evaluar");
var nombreArchivo = Console.ReadLine();
var path = Path.Combine(ruta, nombreArchivo);
using (var stream = new FileStream(path, FileMode.Open))
{
using (var reader = new StreamReader(stream))
{
cadena = reader.ReadToEnd();
}
}
cadena = Regex.Replace(cadena, @"\s+", "");
var nameSet = "";
var palabra = "";
for (int i = 0; i <= cadena.Length; i++)
{
if (error == false)
{
switch (posicion)
{
case 1:
nameSet = string.Empty;
foreach (var set in setsDictionary)
{
if (set.Value.Contains(cadena[i].ToString()))
{
nameSet = set.Key;
}
}
var tok = tokensDictionary.FirstOrDefault(x => x.Value.Contains("'" + cadena[i].ToString() + "'"));
if (nameSet == string.Empty)
{
nameSet = "'" + cadena[i].ToString() + "'"; 
var temporalList = new List<string>();
temporalList.Add(cadena[i].ToString());
setsDictionary.Add(nameSet, temporalList);
}
if (transicionesTotal.ContainsKey(estadoPresente.ToString()))
{
var estadoActual = transicionesTotal.FirstOrDefault(x => x.Key == estadoPresente.ToString());
var sigTransicion = estadoActual.Value.FirstOrDefault(x => x.Split('|')[0] == nameSet);
var separador = sigTransicion.Split('|');
estado = Convert.ToInt32(separador[1].Substring(6, 1));
posicion = 2;
}
else
{
error = true;
}
break;
case 2:
var e = "Estado" + estado;
if (estadosAceptacion.Contains(e))
{
if (palabra == string.Empty)
{
i = i - 1;
}
if (aceptacion == false)
{
i = i - 1;
aceptacion = true;
}
var Set = setsDictionary.FirstOrDefault(x => x.Key == nameSet);
var maspequeño = reservadasDictionary.Values.OrderBy(s => s.Length).First();
if (Set.Value.Contains(cadena[i].ToString()))
{
palabra += cadena[i].ToString();
if (reservadasDictionary.Values.Contains(palabra.ToUpper()))
{
var reservada = reservadasDictionary.FirstOrDefault(x => x.Value == palabra.ToUpper());
resultTokenList.Add(reservada.Key.ToString() + " | " + reservada.Value);
estadoPresente = estado;
posicion = 1;
palabra = "";
}
else if (palabra.Length > maspequeño.Length)
{
foreach (var item in reservadasDictionary.Values)
{
if (palabra.ToUpper().Contains(item))
{
var arrayOne = palabra.ToUpper().ToCharArray();
var arrayTwo = item.ToCharArray();
var differentChars = arrayOne.Except(arrayTwo);
var token = tokensDictionary.FirstOrDefault(x => x.Value.Contains(nameSet));
foreach (var character in differentChars)
{
resultTokenList.Add(token.Key.ToString() + " | " + character.ToString().ToLower());
}
var reservada = reservadasDictionary.FirstOrDefault(x => x.Value == item.ToUpper());
resultTokenList.Add(reservada.Key.ToString() + " | " + reservada.Value);
estadoPresente = estado;
posicion = 1;
palabra = ""; 
}
}
}
else
{
if (i == cadena.Length - 1)
{
cadena = cadena + "#"; 
}
else
{
posicion = 2;
}
}
}
else
{
if (palabra.Length > 1)
{
if (Set.Value.Contains(palabra[0].ToString()))
{
var token = tokensDictionary.FirstOrDefault(x => x.Value.Contains(nameSet));
foreach (var item in palabra)
{
resultTokenList.Add(token.Key.ToString() + " | " + item.ToString());
}
}
else
{
var palabraConComas = "";
foreach (var item in palabra)
{
palabraConComas += "'" + item + "'" + ".";
}
palabraConComas = palabraConComas.Substring(0, palabraConComas.Length - 1);
var token = tokensDictionary.FirstOrDefault(x => x.Value.Contains(palabraConComas));
resultTokenList.Add(token.Key.ToString() + " | " + palabra);
}
}
else
{
var token = tokensDictionary.FirstOrDefault(x => x.Value.Contains(nameSet));
resultTokenList.Add(token.Key.ToString() + " | " + palabra);
if (i == cadena.Length - 1 && cadena[i].ToString() != "#")
{
cadena = cadena + "#"; 
}
}
if (cadena[i].ToString() != cadena[cadena.Length - 1].ToString())
{
i -= 1;
posicion = 1;
estado = 1;
estadoPresente = 1;
palabra = "";
}
else
{
i++;
}
}
}
else
{
i = i - 1;
palabra += cadena[i].ToString();
estadoPresente = estado;
posicion = 1;
aceptacion = false;
}
break;
}
}
else
{
break;
}
}
if (error == false)
{
foreach (var item in resultTokenList)
{
Console.WriteLine(item);
}
}
else
{
Console.WriteLine("Error");
}
Console.ReadLine();
}
}
