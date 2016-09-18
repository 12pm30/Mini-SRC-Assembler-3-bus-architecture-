using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSRC_ASM_CMD
{
    class FileParser
    {
        private FileParser()
        {

        }

        public static List<String> ParseFile(string file, ref List<FileParserException> fpx, int desiredMifSize)
        {
            InstructionParser iParse = new InstructionParser();
            Dictionary<string, int> SubroutineTable = new Dictionary<string, int>();

            List<String> outList = new List<string>();

            int memoryLocation = 0;

            string[] inputFile = File.ReadAllLines(file);

            for(int i = 0;i<inputFile.Length;i++)
            {
                string trimmed = TrimWhiteSpace(inputFile[i].Split(';')[0].Trim());
                string result = "";

                if (trimmed.ToLower().StartsWith("org"))
                {
                    try
                    {
                        //memory origin designation
                        string memLoc = trimmed.ToLower().Split(' ')[1];
                        int actualLoc = 0;

                        if (memLoc.StartsWith("$"))
                        {
                            memLoc = memLoc.TrimStart('$');
                            actualLoc = Convert.ToInt32(memLoc, 16);
                        }
                        else if (memLoc.StartsWith("0x"))
                        {
                            actualLoc = Convert.ToInt32(memLoc, 16);
                        }
                        else
                        {
                            actualLoc = Convert.ToInt32(memLoc);
                        }

                      

                        if (actualLoc == memoryLocation)
                        {
                            fpx.Add(new FileParserException("Ignoring org directive '" + trimmed + "' : Erroneous.", i,1));
                        }
                        else if (actualLoc < memoryLocation)
                        {
                            fpx.Add(new FileParserException("Ignoring org directive '" + trimmed + "' : Can't go backwards in memory.", i, 1));
                        }
                        else if (actualLoc - 1 == memoryLocation)
                        {
                            StringBuilder s = new StringBuilder();
                            s.AppendFormat("{0} : 00000000;", String.Format("{0:X}", actualLoc - 1));
                            result = s.ToString();
                            outList.Add(result);
                            memoryLocation = actualLoc;
                        }
                        else
                        {
                            StringBuilder s = new StringBuilder();
                            s.AppendFormat("[{0}..{1}] : 00000000;", String.Format("{0:X}", memoryLocation), String.Format("{0:X}", actualLoc - 1));
                            result = s.ToString();
                            outList.Add(result);
                            memoryLocation = actualLoc;
                        }

                    }
                    catch(Exception ex)
                    {
                        fpx.Add(new FileParserException("Error parsing org directive '" + trimmed +"' : " + ex.Message, i,0));
                    }
                    

                }
                else if (trimmed.StartsWith("@"))//subroutine
                {
                    SubroutineTable.Add(trimmed.Remove(0, 1), memoryLocation);

                    fpx.Add(new FileParserException("Subroutine Found at location " + String.Format("{0:X}",memoryLocation) + " '" + trimmed + "'", i,2));

                }
                else if (trimmed.StartsWith("&"))//data spec
                {
                    string[] dataItems = trimmed.Remove(0, 1).Replace(' ', ',').Split(',');

                    for(int j=0;j<dataItems.Length;j++)
                    {
                        string dataItem ;

                        try
                        {
                            if (dataItems[j].StartsWith("$"))
                            {
                                dataItem = dataItems[j].TrimStart('$');
                                dataItem = Convert.ToInt32(dataItem, 16).ToString("X");
                            }
                            else if (dataItems[j].ToLower().StartsWith("0x"))
                            {
                                dataItem = Convert.ToInt32(dataItems[j], 16).ToString("X");
                            }
                            else
                            {
                                dataItem = Convert.ToInt32(dataItems[j]).ToString("X");
                            }

                            StringBuilder s = new StringBuilder();
                            s.AppendFormat("{0} : {1};", String.Format("{0:X}", memoryLocation++), dataItem);
                            result = s.ToString();
                            outList.Add(result);

                        }
                        catch (Exception ex)
                        {
                            fpx.Add(new FileParserException("Error parsing data item '" + dataItems[j] + "' : " + ex.Message, i, 0));
                            throw;
                        }



                    }


                }
                else if (String.IsNullOrWhiteSpace(trimmed))
                {
                    //do nothing- this is a comment or other such line
                }
                else
                {
                    //intsruction! Yay!
                    try
                    {
                        string tmpResult = iParse.ParseInstruction(trimmed).PadLeft(8,'0');
                        StringBuilder s = new StringBuilder();
                        s.AppendFormat("{0} : {1};", String.Format("{0:X}", memoryLocation++),tmpResult);
                        result = s.ToString();
                        outList.Add(result);
                    }
                    catch(Exception ex)
                    {
                        fpx.Add(new FileParserException("Error parsing instruction '" + trimmed + "' : " + ex.Message, i, 0));
                    }

                }

            }

            //return outList;

            int pow = 0;

            while (desiredMifSize > (int)Math.Pow(2, pow))
            {
                pow = pow + 1;
            }

            if(desiredMifSize < (int)Math.Pow(2, pow))
            {
                desiredMifSize = (int)Math.Pow(2, pow);
                fpx.Add(new FileParserException("MIF extended to a power of two length.", 0, 2));
            }

            if (memoryLocation < desiredMifSize - 1)
            {
                StringBuilder s = new StringBuilder();
                s.AppendFormat("[{0}..{1}] : 00000000;", String.Format("{0:X}", memoryLocation), String.Format("{0:X}", desiredMifSize - 1));
                outList.Add(s.ToString());
                memoryLocation = desiredMifSize;
            }
            else if(memoryLocation > desiredMifSize - 1)
            {
                while (desiredMifSize - 1 < memoryLocation)
                {
                    pow = pow + 1;
                    desiredMifSize = (int)Math.Pow(2, pow);
                }

                if(memoryLocation == desiredMifSize - 1)
                {
                    StringBuilder s = new StringBuilder();
                    s.AppendFormat("{0} : 00000000;", String.Format(String.Format("{0:X}", desiredMifSize - 1)));
                    outList.Add(s.ToString());
                    memoryLocation = desiredMifSize;
                }
                else
                {
                    StringBuilder s = new StringBuilder();
                    s.AppendFormat("[{0}..{1}] : 00000000;", String.Format("{0:X}", memoryLocation), String.Format("{0:X}", desiredMifSize - 1));
                    outList.Add(s.ToString());
                    memoryLocation = desiredMifSize;
                }
                

                fpx.Add(new FileParserException("MIF extended to accommodate code length", 0, 2));

            }

            List<string> FileOutList = new List<string>();

            FileOutList.Add("WIDTH=32;");


            

            FileOutList.Add("DEPTH=" + Convert.ToString((int)Math.Pow(2, pow)) + ";");
            FileOutList.Add("ADDRESS_RADIX=HEX;");
            FileOutList.Add("DATA_RADIX=HEX;");
            FileOutList.Add("");
            FileOutList.Add("CONTENT BEGIN");
            FileOutList.AddRange(outList);
            FileOutList.Add("END;");

            return FileOutList;


        }

        public static string TrimWhiteSpace(string Value)
        {
            StringBuilder sbOut = new StringBuilder();
            if (!string.IsNullOrEmpty(Value))
            {
                bool IsWhiteSpace = false;
                for (int i = 0; i < Value.Length; i++)
                {
                    if (char.IsWhiteSpace(Value[i])) //Comparion with WhiteSpace
                    {
                        if (!IsWhiteSpace) //Comparison with previous Char
                        {
                            sbOut.Append(Value[i]);
                            IsWhiteSpace = true;
                        }
                    }
                    else
                    {
                        IsWhiteSpace = false;
                        sbOut.Append(Value[i]);
                    }
                }
            }
            return sbOut.ToString();
        }
    }



}
