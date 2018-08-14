using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Microsoft.VisualBasic.FileIO; 

namespace WindowsFormsApp1
{

    class E2data
    {
        // -------------------------------------------------------- Constructors --------------------------------------------------------
        /// <summary> Constructor for a E2data type </summary>
        /// <remark> Most of the work in this class is done by the constructor, which parses the .csv document and initiliazes a list of elements as well as all extensions.</remark>
        /// <param name="filePath"> the path for the .csv file </param>
        /// <param name="approxMaxStem"> an upper bound on the number of stems, i.e., x coord (can be large, better safe than sorry) </param>
        /// <param name="approxMaxFilt"> an upper bound on the max Adams filtration (can be large, better safe than sorry) </param>
        /// <param name="wordsforNonExtensionAttributes"> In entry n contains all the words for : [0] name, [1] stem, [2] filt, [3] weight, [4] tau torsion, [5] label </param>
        /// <param name="wordsforExtensionAttributes"> In entry n contains all the words for : [0] extension target, [1] extension info, [2] localization </param>
        public E2data(string filePath, int approxMaxStem, int approxMaxFilt, string[][] wordsforNonExtensionAttributes, string[][] wordsforExtensionAttributes, Color extDefColor)
        {
            Elements = new List<Element>[approxMaxStem + 1][];
            for (int i = 0; i <= approxMaxStem; i++)
                Elements[i] = new List<Element>[approxMaxFilt + 1];

            Extensions = new List<Extension>();
            
            #region Parsing the CSV file and load it in Elements

            using (TextFieldParser l_parser = new TextFieldParser(filePath))
            {
                l_parser.TextFieldType = FieldType.Delimited;
                l_parser.SetDelimiters(","); 

                string[] header = l_parser.ReadFields();
                int nbrColumns = header.Length;
                
                IndicesOfHeadersInCSV = MakeTableOfIndices(header, wordsforNonExtensionAttributes, wordsforExtensionAttributes);

                int elemNbr = 0;
                MaxNbrElemPerStemFilt = 0;

                while (!l_parser.EndOfData) // take care of the exception
                {
                    string[] toParse = l_parser.ReadFields();
                    if(toParse.Length == nbrColumns)
                    {
                        try
                        {
                            int stem = int.Parse(toParse[IndicesOfHeadersInCSV[1]]);
                            int filt = int.Parse(toParse[IndicesOfHeadersInCSV[2]]);

                            if (Elements[stem][filt] == null)
                                Elements[stem][filt] = new List<Element>();

                            Elements[stem][filt].Add(new Element(toParse, ref IndicesOfHeadersInCSV, elemNbr, Elements[stem][filt].Count, ref IndicesOfHeadersForExtensionsInCSV));

                            MaxStem = (stem > MaxStem) ? stem : MaxStem;
                            MaxFilt = (filt > MaxFilt) ? filt : MaxFilt;

                            MaxNbrElemPerStemFilt = Math.Max(Elements[stem][filt].Count, MaxNbrElemPerStemFilt);
                        }
                        catch (FormatException e) // take care of this if it there is an exception
                        {
                            
                        }
                    }
                    else
                    {
                        // say that problem
                    }
                    elemNbr++;
                }
            }
            #endregion

            #region Load extensions in Extensions

            // First find the elements by which we extend, create the recipient for extensions by these elements, and find the degrees of the element by which we extend.
            for (int i = 0; i < IndicesOfHeadersForExtensionsInCSV.Count; i++)
            {
                bool foundElem = false;

                for (int x = 0; x <= MaxStem; x++)
                {
                    for (int y = 0; y <= MaxFilt; y++)
                    {
                        if (Elements[x][y] != null)
                        {
                            for (int j = 0; j < Elements[x][y].Count; j++)
                            {
                                // here we check that the string from the header - "target" exists as an element and find it (so doesn't work if the element by which we extend does not exist, in which case we'd have to give its degree by hand)
                                if (Elements[x][y][j].AssembleName() == (string)IndicesOfHeadersForExtensionsInCSV[i][0])
                                {
                                    foundElem = true;

                                    Extensions.Add(new Extension((string)IndicesOfHeadersForExtensionsInCSV[i][0], x, y, Elements[x][y][j].Weight,  ref IndicesOfHeadersForExtensionsInCSV, extDefColor, true));

                                    MaxStemExtensions = Math.Max(MaxStemExtensions, x);
                                    MaxFiltExtensions = Math.Max(MaxFiltExtensions, y);
                                    
                                    break;
                                }
                            }

                        }
                        if (foundElem)
                            break;
                    }
                    if (foundElem)
                        break;
                }
            }

            // Now load the extension source and target in Extensionsx
            for (int x = 0; x <= MaxStem; x++) 
            {
                for (int y = 0; y <= MaxFilt; y++)
                {
                    if (Elements[x][y] != null)
                    {
                        for (int i = 0; i < Elements[x][y].Count; i++)
                        {
                            for (int j=0; j < IndicesOfHeadersForExtensionsInCSV.Count; j++)
                            {
                                if (Elements[x][y][i].NameOfExtTargets[j] != null)
                                {
                                    for (int k = 0; k < wordsforExtensionAttributes[2].Length; k++)
                                    {
                                        if (GetExtTarget(Elements[x][y][i], j, wordsforExtensionAttributes[2][k]) != null)
                                        {
                                            int[] targetCoord = GetExtTarget(Elements[x][y][i], j, wordsforExtensionAttributes[2][k]);

                                            if (targetCoord[2] == -1)
                                            {
                                                if (targetCoord[0] > MaxStem)
                                                    MaxStem = targetCoord[0];
                                                if (targetCoord[1] > MaxFilt)
                                                    MaxFilt = targetCoord[1];

                                                // I should deal better with its name here...
                                                string nameTarget = Elements[x][y][i].AssembleName() + " " + (string)IndicesOfHeadersForExtensionsInCSV[j][0];

                                                if (Elements[targetCoord[0]][targetCoord[1]] == null)
                                                    Elements[targetCoord[0]][targetCoord[1]] = new List<Element>();

                                                int tauTorsion = 0;
                                                if ((string)IndicesOfHeadersForExtensionsInCSV[j][0] == "h1")
                                                    tauTorsion = 1;

                                                Element targetElem = new Element(nameTarget, targetCoord[0], targetCoord[1], Elements[x][y][i].Weight + Extensions[j].WeightElem, tauTorsion, Elements[targetCoord[0]][targetCoord[1]].Count, ref IndicesOfHeadersForExtensionsInCSV);
                                                targetElem.PropertyExtTarget[j] = new List<string>();
                                                targetElem.PropertyExtTarget[j].Add("last");

                                                Elements[targetCoord[0]][targetCoord[1]].Add(targetElem);

                                                targetCoord[2] = Elements[targetCoord[0]][targetCoord[1]].Count - 1;
                                            }

                                            Extensions[j].AddExt(new int[3] { x, y, i }, targetCoord);
                                            break;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Sort them at each spot (s,f), based on the shift info of the .csv file
            for (int x = 0; x <= MaxStem; x++)
            {
                for (int y = 0; y <= MaxFilt; y++)
                {
                    if (Elements[x][y] != null && Elements[x][y].Count > 1)
                    {
                        // first make sure that the points we play with are visible, as we don't care about the ones that we added.
                        List<int> elemNonLastPt = new List<int>();
                        List<int> elemLastPt = new List<int>();

                        for (int i = 0; i < Elements[x][y].Count; i++)
                        {
                            if (!Elements[x][y][i].LastPointOfSomeExt)
                                elemNonLastPt.Add(i);
                            else
                                elemLastPt.Add(i);
                        }

                        // put the elements that are LastPoints at the end. (could be unecessary as they might be created after, but good measure and cheap).

                        for(int j=0; j<elemLastPt.Count;j++)
                        {
                            for(int k= Elements[x][y].Count-1; k> elemLastPt[0]; k--)
                            {
                                if(!Elements[x][y][k].LastPointOfSomeExt)
                                {
                                    Element tempElem = Elements[x][y][k];
                                    Elements[x][y][k] = Elements[x][y][elemLastPt[j]];
                                    Elements[x][y][elemLastPt[j]] = tempElem;
                                }
                            }
                        }

                        if(elemNonLastPt.Count > 1)
                        {
                            bool areOrdered = true;
                            // we check if the order in the .csv file matches with the shifts
                            for (int i = 0; i < elemNonLastPt.Count; i++)
                            {
                                if (Elements[x][y][i].Shift != -(elemNonLastPt.Count - 1) + 2 * i)
                                {
                                    areOrdered = false;
                                    break;
                                }
                            }

                            // if the two orders don't agree (either error, or because we moved some manually), then we try to order them by using the shift of the .csv file, and if it fails (error in shifts) then we just do it automatically in the order of the .csv file
                            if (!areOrdered)
                            {
                                int nbrElem = elemNonLastPt.Count;
                                int[] arrayOfShifts = new int[nbrElem];

                                // the bijection works as Dan -> Comp : k -> (k+nbrElem-1)/2 and the other way Comp -> Dan : k -> -(nbrElem-1) + 2k. So now we order them
                                for (int i = 0; i < nbrElem; i++)
                                    arrayOfShifts[(Elements[x][y][i].Shift + nbrElem - 1) / 2] = Elements[x][y][i].Shift;

                                bool entriesInCSVareGood = true;
                                // if the entries in the .csv file are correct, the entry i should have value Elements[x][y][i].Shift and the boolean is not changed
                                for (int i = 0; i < nbrElem; i++)
                                {
                                    if (arrayOfShifts[i] != -(nbrElem - 1) + 2 * i)
                                        entriesInCSVareGood = false;
                                }

                                if (entriesInCSVareGood) // if the shift entries are good, then we re-order the elements according to those, if no, we leave them in peace and end all this nonsense.
                                {
                                    for (int i = 0; i < nbrElem; i++)
                                    {
                                        if (i != (Elements[x][y][i].Shift + nbrElem - 1) / 2) // if the ith element is not at its correct spot, then put it where it belongs and continue!
                                        {
                                            Element tempElem = Elements[x][y][(Elements[x][y][i].Shift + nbrElem - 1) / 2];
                                            Elements[x][y][(Elements[x][y][i].Shift + nbrElem - 1) / 2] = Elements[x][y][i];
                                            Elements[x][y][i] = tempElem;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

        }

        // -------------------------------------------------------- Properties and Fields --------------------------------------------------------
        public List<Element>[][] Elements;
        public List<Extension> Extensions;

        public int MaxStem { get; private set; }
        public int MaxFilt { get; private set; }
        public int MaxNbrElemPerStemFilt { get; private set; }

        public int MaxStemExtensions;
        public int MaxFiltExtensions;
        
        private int[] IndicesOfHeadersInCSV; 
        public List<object[]> IndicesOfHeadersForExtensionsInCSV;

        // -------------------------------------------------------- Methods --------------------------------------------------------
        /// <summary>
        /// This method parses the header. It identifies the columns of name, stem, filt, weight and tautorsion, identifies the columns of the extenisons, and links the extension info and the extension target of together
        /// </summary>
        /// <param name="header"> Array of strings containing the names of the columns in the .csv</param>
        /// <param name="wordsforNonExtensionAttributes"> In entry n contains all the words for : [0] name, [1] stem, [2] filt, [3] weight, [4] tau torsion, [5] label, [6] angle </param>
        /// <param name="wordsforExtensionAttributes"> In entry n contains all the words for : [0] extension target, [1] extension info, [2] localization </param>
        private int[] MakeTableOfIndices(string[] header, string[][] wordsforNonExtensionAttributes, string[][] wordsforExtensionAttributes) 
        {
            IndicesOfHeadersForExtensionsInCSV = new List<object[]>();
            int[] tableOfIndices = new int[wordsforNonExtensionAttributes.Length];
            int index = 0;

            foreach (string title in header)
            {
                string lowerTitle = title.ToLower();
                lowerTitle = lowerTitle.Replace(" ", "");
                lowerTitle = lowerTitle.Replace("_", "");

                bool identifiedColumnWithExtensionTarget = false;

                for (int i = 0; i < wordsforExtensionAttributes[0].Count(); i++)
                {
                    if (lowerTitle.Contains(wordsforExtensionAttributes[0][i]))
                    {
                        lowerTitle = lowerTitle.Replace(wordsforExtensionAttributes[0][i], "");
                        bool identifiedExt = false;

                        for (int j = 0; j < IndicesOfHeadersForExtensionsInCSV.Count; j++)
                        {
                            if ((string)IndicesOfHeadersForExtensionsInCSV[j][0] == lowerTitle)
                            {
                                IndicesOfHeadersForExtensionsInCSV[j][1] = index;
                                identifiedExt = true;
                                break;
                            }
                        }

                        if (!identifiedExt)
                        {
                            object[] NameAndPos = new object[3];
                            NameAndPos[0] = lowerTitle;
                            NameAndPos[1] = index;
                            IndicesOfHeadersForExtensionsInCSV.Add(NameAndPos);
                        }
                        identifiedColumnWithExtensionTarget = true;
                        break;
                    }
                }

                if (!identifiedColumnWithExtensionTarget)
                {
                    for(int k=0; k < wordsforExtensionAttributes[1].Length; k++)
                    {
                        if (lowerTitle.Contains(wordsforExtensionAttributes[1][k]))
                        {
                            lowerTitle = lowerTitle.Replace(wordsforExtensionAttributes[1][k], "");
                            bool identifiedExt = false;

                            for (int i = 0; i < IndicesOfHeadersForExtensionsInCSV.Count; i++)
                            {
                                if ((string)IndicesOfHeadersForExtensionsInCSV[i][0] == lowerTitle)
                                {
                                    IndicesOfHeadersForExtensionsInCSV[i][2] = index;
                                    identifiedExt = true;
                                    break;
                                }
                            }

                            if (!identifiedExt)
                            {
                                object[] NameAndPos = new object[3];
                                NameAndPos[0] = lowerTitle;
                                NameAndPos[2] = index;
                                IndicesOfHeadersForExtensionsInCSV.Add(NameAndPos);
                            }
                        }
                        else
                        {
                            bool identifiedName = false;

                            for (int i = 0; i < wordsforNonExtensionAttributes.Length; i++)
                            {
                                for (int j = 0; j < wordsforNonExtensionAttributes[i].Length; j++)
                                {
                                    if (lowerTitle == wordsforNonExtensionAttributes[i][j])
                                    {
                                        tableOfIndices[i] = index;
                                        identifiedName = true;
                                        break;
                                    }
                                }
                                if (identifiedName)
                                    break;
                            }

                            if (!identifiedName)
                            {
                                // throw an exception, we couldn't undersand the header
                            }
                        }
                    }
                }
                index++;
            }
            return tableOfIndices;
        }

        /// <summary>
        /// Given an element and the index in Extensions of the type of extension, the method returns the extension of the element
        /// </summary>
        /// <param name="source">Element that we extend</param>
        /// <param name="indexOfExtinExtensions">Index in Extensions of the type of extension</param>
        /// <param name="wordForInfiniteExtension"> The word for an infinite tower of extensions (such as h1-periodic stuff) </param>
        /// <returns>Returns an int[3] containing (stem, filt, indexAtStemFilt) to locate it in Element. If the value is null, it means TODOOOO</returns>
        private int[] GetExtTarget(Element source, int indexOfExtinExtensions, string wordForInfiniteExtension)
        {
            int stemMultiplicand = Extensions[indexOfExtinExtensions].StemElem;
            int filtMultiplicand = Extensions[indexOfExtinExtensions].FiltElem;

            int[] coordTarget = new int[3];
            coordTarget[0] = source.Stem + stemMultiplicand;


            if (source.NameOfExtTargets[indexOfExtinExtensions][0].Symbol.Replace(" ", "").ToLower() == wordForInfiniteExtension)
            {
                coordTarget[1] = source.Filtration + filtMultiplicand;
                coordTarget[2] = -1; // so that when we read we know that this is going to lead to an infinite extension
                return coordTarget;
            }
            else 
            {
                int jumpInFilt = 0;

                while (source.Filtration + filtMultiplicand + jumpInFilt <= MaxFilt && Elements[source.Stem + stemMultiplicand][source.Filtration + filtMultiplicand + jumpInFilt] != null)
                {
                    for (int j = 0; j < Elements[source.Stem + stemMultiplicand][source.Filtration + filtMultiplicand + jumpInFilt].Count; j++)
                    {
                        bool foundTarget = false;

                        if (source.NameOfExtTargets[indexOfExtinExtensions].Count == Elements[source.Stem + stemMultiplicand][source.Filtration + filtMultiplicand + jumpInFilt][j].Name.Count)
                        {
                            for (int k = 0; k < source.NameOfExtTargets[indexOfExtinExtensions].Count; k++)
                            {
                                foundTarget = false;
                                if (source.NameOfExtTargets[indexOfExtinExtensions][k] == Elements[source.Stem + stemMultiplicand][source.Filtration + filtMultiplicand + jumpInFilt][j].Name[k])
                                    foundTarget = true;
                            }

                            if (foundTarget)
                            {
                                coordTarget[1] = source.Filtration + filtMultiplicand + jumpInFilt;
                                coordTarget[2] = j;
                                return coordTarget;
                            }
                        }
                    }
                    jumpInFilt++;
                }
            }
            return null; // that means that we're out of the bounds of the excel spreadsheet, and the target is no described yet (or that there is a hidden ext!!) or just that we didn't find it!
        }

        public Element GetElement(int[] stemFiltIndexAtStemFilt)
        {
            return Elements[stemFiltIndexAtStemFilt[0]][stemFiltIndexAtStemFilt[1]][stemFiltIndexAtStemFilt[2]];
        }
        public void SetElement(int[] stemFiltIndexAtStemFilt, Element element)
        {
            Elements[stemFiltIndexAtStemFilt[0]][stemFiltIndexAtStemFilt[1]][stemFiltIndexAtStemFilt[2]] = element;
        }

        public PointF GetPointF(int[] stemFiltIndexAtStemFilt)
        {
            return Elements[stemFiltIndexAtStemFilt[0]][stemFiltIndexAtStemFilt[1]][stemFiltIndexAtStemFilt[2]].GetPointF();
        }

        public void ShiftXGeomPoint(int[] coord, float dx)
        {
            Elements[coord[0]][coord[1]][coord[2]].ShiftXGeomPoint(dx);
        }
        public void ShiftYGeomPoint(int[] coord, float dy)
        {
            Elements[coord[0]][coord[1]][coord[2]].ShiftYGeomPoint(dy);
        }
    }

    public class Element
    {
        // -------------------------------------------------------- Internal structures --------------------------------------------------------
        public struct Monomial // should be done better, throw exception if not correct syntax, for ex if Letter should have 2 charcter, like Q'
        {
            private Monomial(string c, int? ind = null, int? exp = null)
            {
                Symbol = c;
                Index = ind;
                Exponant = exp;
            }
            public Monomial(string concatName) // throw exception if the parsing fails 
            {
                bool readingIndex = false;
                bool readingExponant = false;

                Symbol = "";
                Index = null;
                Exponant = null;

                foreach (char c in concatName)
                {
                    if (readingIndex)
                    {
                        if (c == '^')
                        {
                            Exponant = 0;
                            readingExponant = true;
                            readingIndex = false;
                        }
                        else if (c >= '0' && c <= '9')
                        {
                            Index *= 10;
                            Index += c - '0';
                        }
                        else if(c == '\'')
                        {
                            Symbol += c;
                        }
                        else
                        {
                            // Problem throw exception!
                        }
                    }
                    else if (readingExponant)
                    {
                        if (c >= '0' && c <= '9')
                        {
                            Exponant *= 10;
                            Exponant += c - '0';
                        }
                        else
                        {
                            // Problem throw exception!
                        }

                    }
                    else
                    {
                        if (c >= '0' && c <= '9')
                        {
                            if (Symbol == "" && c == '1') // the very unique case when the element is 1, and thus does it's symbol is the number 1 (which is not a subscript)
                                Symbol += c;
                            else
                            {
                                Index = c - '0';
                                readingIndex = true;
                                readingExponant = false; // although it should never arrive here, as the index is before the exponant
                            }

                        }
                        else if(c == '_')
                        {
                            Index = 0;
                            readingIndex = true;
                            readingExponant = false; // although it should never arrive here, as the index is before the exponant
                        }
                        else if(c == '^')
                        {
                            Exponant = 0;
                            readingIndex = false;
                            readingExponant = true;
                        }
                        else
                            Symbol += c;
                    }
                        
                }
                 
                // Old name parser
                /*
                var letters = concatName.TakeWhile(Char.IsLetter); // this means that the name has to start with a letter!
                Symbol = new string(letters.ToArray());

                Index = null;
                Exponant = null;

                if (concatName.Length > Symbol.Length && Symbol != "") 
                {
                    int i = Symbol.Length;
                    
                    while(i < concatName.Length)
                    {
                        char c = concatName[i];
                        int number;

                        if (c >= '0' && c <= '9')
                        {
                            number = c - '0';
                            i++;
                            while(i < concatName.Length && concatName[i] >= '0' && concatName[i] <= '9')
                            {
                                number += number * 10 + concatName[i] - '0';
                                i++;
                            }
                            Index = number;

                        }
                        else if (c == '_')
                        {
                            i++;
                            number = 0;
                            while (i < concatName.Length && concatName[i] >= '0' && concatName[i] <= '9')
                            {
                                number += number * 10 + concatName[i] - '0';
                                i++;
                            }
                            Index = number;
                        }   
                        else if (c == '^')
                        {
                            i++;
                            number = 0;
                            while (i < concatName.Length && concatName[i] >= '0' && concatName[i] <= '9')
                            {
                                number += number * 10 + concatName[i] - '0';
                                i++;
                            }
                            Exponant = number;
                        }
                        else
                        {
                            // return an error or something!
                        }
                    }
                }*/


            }

            public string Symbol;
            private int? Index;
            private int? Exponant;

            public static bool operator ==(Monomial name1, Monomial name2)
            {
                return name1.Symbol == name2.Symbol && name1.Index == name2.Index && name1.Exponant == name2.Exponant;
            }
            public static bool operator !=(Monomial name1, Monomial name2)
            {
                return name1.Symbol != name2.Symbol || name1.Index != name2.Index && name1.Exponant != name2.Exponant;
            }
            public override bool Equals(Object obj)
            {
                return obj is Monomial && this == (Monomial)obj;
            }
            public override int GetHashCode()
            {
                return Symbol.GetHashCode() ^ Index.GetHashCode();
            }

            public string GetName()
            {
                string latexName = Symbol;
                latexName += (Index == null) ? "" : Index.ToString();
                latexName += (Exponant == null) ? "" : ("^" + Exponant.ToString());
                return latexName;
            }
            public string GetLatexName()
            {
                string latexName = Symbol;
                latexName += (Index == null) ? "" : ("_" + Index.ToString());
                latexName += (Exponant == null) ? "" : ("^" + Exponant.ToString());
                return latexName;
            }

        };

        // -------------------------------------------------------- Properties and Fields --------------------------------------------------------
        public List<Monomial> Name { get; private set; }
        public string ExcelName { get; } // this is just the name for human reading, as it is in the Excel Spreadsheet, i.e., tg stands for the latex name \tau g

        public int Stem { get; }
        public int Filtration { get; }
        public int Weight { get; }
        public int TauTorsion { get; } = 0;

        public bool LastPointOfSomeExt; // the information for which extension can be found in PropertyExtTarget if there is an entry "last"

        public int? ElemNbrInCSVfile { get; set; }
        public int ElemNbrInElementsAtStemFilt { get; set; }

        public List<Monomial>[] NameOfExtTargets;
        public List<string>[] PropertyExtTarget;

        public int Shift;

        private GeometricPoint GeomPoint;
        public int OffsetX = 0;
        public int OffsetY = 0;

        public string LatexLabel { get; set; }
        public bool IsLabelVisible { get; set; }
        public int OffsetXlabel = 0;
        public int OffsetYlabel = 0;
        public int LabelAngle { get; set; } = -90; // by default it's -90, for example if the parser fails (to do bettr...)

        public bool IsVisible { get; set; } = true;

        public string Notes { get; set; }

        // -------------------------------------------------------- Constructors --------------------------------------------------------
        /// <summary> Constructor for Element whose input is a line from the .csv file as well as a recipe to know what information is where </summary>
        /// <param name="lineFromCSV">The line from the .csv file given as an array of string, cell by cell</param>
        /// <param name="indicesNonExt">The indexes are: [0] name, [1] stem, [2] filtration, [3] weight, [4] tauTorsion</param>
        /// <param name="elemNbrinCSV">The number of the line in the .csv (have to add 2 to recover the line in the .csv because of the header)</param>
        /// <param name="elemNbrinElematStemFilt">The index of the element in the List at Elements[stem][filt]</param>
        /// <param name="indicesHeaderForExt">List where each object contains (string, int), where the string is the name of the extension element and int is its column in the .csv file and thus in <paramref name="lineFromCSV"></paramref> </param>
        /// <param name="isPeriodicAndLastPoint">Boolean indicating wheter or not there is a periodic tower of extensions starting from this element (the extension element should be implicit somehow) <paramref name="lineFromCSV"></paramref> </param>
        public Element(string[] lineFromCSV, ref int[] indicesNonExt, int elemNbrinCSV, int elemNbrinElematStemFilt, ref List<object[]> indicesHeaderForExt)
        {
            Name = new List<Monomial>();
            LastPointOfSomeExt = false;
            IsVisible = true;

            ElemNbrInCSVfile = elemNbrinCSV;
            ElemNbrInElementsAtStemFilt = elemNbrinElematStemFilt;

            ExcelName = lineFromCSV[indicesNonExt[0]];

            string[] eachMonomial = lineFromCSV[indicesNonExt[0]].Split(' ');
            for (int i = 0; i < eachMonomial.Length; i++)
                Name.Add(new Monomial(eachMonomial[i]));

            #region Parse Stem, Filtration, Weight, TauTorsion, Shift, Note
            try
            {
                Stem = int.Parse(lineFromCSV[indicesNonExt[1]]);
                Filtration = int.Parse(lineFromCSV[indicesNonExt[2]]);
                Weight = int.Parse(lineFromCSV[indicesNonExt[3]]);
                TauTorsion = int.Parse(lineFromCSV[indicesNonExt[4]]);
                Shift = int.Parse(lineFromCSV[indicesNonExt[7]]);
                if (lineFromCSV[indicesNonExt[6]] == "")
                    LabelAngle = -90;
                else
                    LabelAngle = int.Parse(lineFromCSV[indicesNonExt[6]]);

                Notes = lineFromCSV[indicesNonExt[8]];

            }
            catch (FormatException e) // take care of this if it there is an exception
            {
            }
            #endregion

            #region Parse the Label and its Angle
        
            
            string label = lineFromCSV[indicesNonExt[5]];
            if (label == "auto")
            {
                IsLabelVisible = true;
               LatexLabel = AssembleLatexName();
            }
            else if (label == "none" )
            {
                IsLabelVisible = false;  // the LatexLabel here is not even instanciated, which allows to make the difference between "none" where we don't want the label and "" where we just didn't provide a label but one will be generated if it's not an extension
            }
            else if (label == "")
            {
                IsLabelVisible = false;
                LatexLabel = ""; // I won't assemble the name here, as there is a big chance that this Label will never get used. I create it when we need later, if we do.
            }
            else
            {
                IsLabelVisible = true;
                LatexLabel = label;
            }

            #endregion

            #region Parse the Extensions

            NameOfExtTargets = new List<Monomial>[indicesHeaderForExt.Count];
            PropertyExtTarget = new List<string>[indicesHeaderForExt.Count];

            for (int i=0; i< indicesHeaderForExt.Count; i++)
            {
                if(lineFromCSV[((int)indicesHeaderForExt[i][1])] != "")
                {
                    NameOfExtTargets[i] = new List<Monomial>();
                    
                    eachMonomial = lineFromCSV[((int)indicesHeaderForExt[i][1])].Split(' ');
                    
                    for (int j = 0; j < eachMonomial.Length; j++)
                        NameOfExtTargets[i].Add(new Monomial(eachMonomial[j]));

                    if (lineFromCSV[((int)indicesHeaderForExt[i][2])] != "")
                    {
                        PropertyExtTarget[i] = new List<string>();
                       
                        string[] eachWord = lineFromCSV[((int)indicesHeaderForExt[i][2])].Split(' ');

                        for (int j = 0; j < eachWord.Length; j++)
                            PropertyExtTarget[i].Add(eachWord[j]);
                           
                    }
                }
            }
            
            if (LastPointOfSomeExt)
                IsVisible = false;

            #endregion

        } // throw exceptions if the parsing does not work

        /// <summary>
        /// This constructor is only used when I have to create an extra point which does not appear in the .csv file. Usually because there is an infinite tower of extensions so we create one more point
        /// </summary>
        /// <param name="concatName"></param>
        /// <param name="stem"></param>
        /// <param name="filt"></param>
        /// <param name="weight"></param>
        /// <param name="tautorsion"></param>
        /// <param name="elemnbrinElemstemfilt"></param>
        /// <param name="indicesHeaderForExt"></param>
        /// <param name="isPeriodicAndLastPoint"></param>
        public Element(string concatName, int stem, int filt, int weight, int tautorsion, int elemnbrinElemstemfilt, ref List<object[]> indicesHeaderForExt)
        {

            Name = new List<Monomial>();
            ElemNbrInCSVfile = null;
            ElemNbrInElementsAtStemFilt = elemnbrinElemstemfilt;

            LastPointOfSomeExt = true;
            IsLabelVisible = false;
            IsVisible = false;

            ExcelName = concatName;

            string[] eachMonomial = concatName.Split(' ');
            for (int i = 0; i < eachMonomial.Length; i++)
                Name.Add(new Monomial(eachMonomial[i]));

            Stem = stem;
            Filtration = filt;
            Weight = weight;
            TauTorsion = tautorsion;

            NameOfExtTargets = new List<Monomial>[indicesHeaderForExt.Count];
            PropertyExtTarget = new List<string>[indicesHeaderForExt.Count];

            LatexLabel = ExcelName;
            LabelAngle = -90;

            Notes = "This point was automatically created as it starts an infinite tower of " + eachMonomial[eachMonomial.Length - 1] + " extensions";

        }

        // -------------------------------------------------------- Methods --------------------------------------------------------
        public void SetGeometricPoint(PointF point)
        {
            GeomPoint = new GeometricPoint(point);
        }

        public PointF GetPointF()
        {
            return GeomPoint.Pointf;
        }
        public int[] GetCoord()
        {
            return new int[3] { Stem, Filtration, ElemNbrInElementsAtStemFilt };
        }

        public float GetGeomXcoord()
        {
            return GeomPoint.Pointf.X;
        }
        public float GetGeomYcoord()
        {
            return GeomPoint.Pointf.Y;
        }

        public void SetGeomXcoord(float xCoord)
        {
            GeomPoint.Pointf.X = xCoord;
        }
        public void SetGeomYcoord(float yCoord)
        {
            GeomPoint.Pointf.Y = yCoord;
        }
        public void ShiftXGeomPoint(float dx)
        {
            GeomPoint.ShiftXGeomPoint(dx);
        }
        public void ShiftYGeomPoint(float dy)
        {
            GeomPoint.ShiftYGeomPoint(dy);
        }
        public void ShiftXYGeomPoint(float dx, float dy)
        {
            GeomPoint.ShiftXGeomPoint(dx);
            GeomPoint.ShiftYGeomPoint(dy);
        }

        public string AssembleName()
        {
            string name = "";

            if (Name.Count == 0)
                return name;

            name = Name[0].GetName();

            for (int i = 1; i < Name.Count; i++)
                name += " " + Name[i].GetName();

            return name;
        }

        public string AssembleLatexName()
        {
            string name = "";

            if (Name.Count == 0)
                return name;

            name = Name[0].GetLatexName();

            for (int i = 1; i < Name.Count; i++)
                name += " " + Name[i].GetLatexName();

            return name;
        }

        public string AssembleExtensionName(int nbrExt)
        {
            string name = System.String.Empty;

            if(NameOfExtTargets[nbrExt] != null)
            {
                foreach (Monomial mono in NameOfExtTargets[nbrExt])
                    name += mono.GetName() + " ";

                return name.Substring(0, name.Length - 1);
            }
            else
                return "";
        }

        public string AssembleExtensionProperties(int nbrExt)
        {
            string name = System.String.Empty;

            if (PropertyExtTarget[nbrExt] != null)
            {
                foreach (string s in PropertyExtTarget[nbrExt])
                    name += s + " ";

                return name.Substring(0, name.Length - 1);
            }
            else
                return "";
        }

    }

    public class GeometricPoint
    {

        public PointF Pointf;

        public GeometricPoint(float x, float y)
        {
            Pointf = new PointF(x, y);
        }
        public GeometricPoint(PointF point) : this(point.X, point.Y)
        {

        }

        public void ShiftXGeomPoint(float dx)
        {
            Pointf.X += dx;
        }
        public void ShiftYGeomPoint(float dy)
        {
            Pointf.Y += dy;
        }
        public void ShiftXYGeomPoint(float dx, float dy)
        {
            Pointf.X += dx;
            Pointf.Y += dy;
        }
    }

    public class Extension
    {
        // -------------------------------------------------------- Constructors --------------------------------------------------------
        public Extension(string nameElem, int stem, int filt, int weight, ref List<object[]> indicesAneNameOfHeadersForExt, Color extdefcolor, bool isVisible)
        {
            NameElem = nameElem;
            StemElem = stem;
            FiltElem = filt;
            WeightElem = weight;

            ExtDefaultColor = extdefcolor;

            IsVisible = isVisible;

            IndicesExtensions = new List<int[][]>();

            for (int i = 0; i < indicesAneNameOfHeadersForExt.Count; i++)
            {
                if (nameElem == (string)indicesAneNameOfHeadersForExt[i][0])
                    NbrElemInHeaderCSV = (int)indicesAneNameOfHeadersForExt[i][1];
            }
        }


        // -------------------------------------------------------- Properties and Fields --------------------------------------------------------
        public string NameElem { get; private set; }
        
        private int NbrElemInHeaderCSV;

        public int StemElem { get; private set; }
        public int FiltElem { get; private set; }
        public int WeightElem { get; private set; }

        public bool IsVisible;

        public List<int[][]> IndicesExtensions;

        public Color ExtDefaultColor;


        // -------------------------------------------------------- Methods --------------------------------------------------------
        public void AddExt(int sourceStem, int sourceFilt, int sourceNbrInElem, int targetStem, int targetFilt, int targetNbrInElem)
        {
            int[] sourceCoord = new int[3] { sourceStem, sourceFilt, sourceNbrInElem };
            int[] targetCoord = new int[3] { targetStem, targetFilt, targetNbrInElem };
            IndicesExtensions.Add(new int[2][] { sourceCoord, targetCoord });
        }
        public void AddExt(int[] sourceStemFiltNbrInElem, int[] targetStemFiltNbrInElem)
        {
            IndicesExtensions.Add(new int[2][] { sourceStemFiltNbrInElem, targetStemFiltNbrInElem });
        }
    }

}
