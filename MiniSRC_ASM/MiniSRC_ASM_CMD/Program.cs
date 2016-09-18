using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSRC_ASM_CMD
{
    class Program
    {
        static void Main(string[] args)
        {
            bool ShowWarnings = true;
            bool ShowInfo = true;
            bool ShowFile = false;

            int desiredSize = 0;

            List<FileParserException> exceptionList = new List<FileParserException>();

            Console.WriteLine("--MiniSRC Assembler by Mitchell Waite--\n");

            if (args.Length == 0)
            {
                Console.Write("Usage: minisrc_asm.exe [file] [output] [length] [options]\n\n[file] is a relative or absolute path to a MiniSRC asm file\n[output] is the desired location for the generated MIF\n\n[length] is the desired MIF size length, in (32 bit) words of memory.\n\nOptions:\n-showfile : show the MIF output in the console window.\n-noinfo : supress informational messages\n-nowarn : supress warning messages\n\nUse an & symbol to specify a line with data items, separated by spaces or commas -> &0x5,0x6\nUse an @ symbol to specify a subroutine, on its own line.\n\n");
            }
            else if (!File.Exists(args[0]))
            {
                Console.Write("The file specified below does not exist.\n\n" + args[0]);
            }
            else if (!Int32.TryParse(args[2], out desiredSize)) 
            {
                Console.Write("The following integer is not valid for desired MIF size: " + args[2] + "\n\n");
            }
            else
            {
                
                Console.WriteLine("Input: " + args[0]);
                Console.WriteLine("Output: " + args[1]);
                Console.WriteLine("");

                if (args.Contains("-noinfo"))
                {
                    ShowInfo = false;
                }

                if(args.Contains("-nowarn"))
                {
                    ShowWarnings = false;
                }

                if(args.Contains("-showfile"))
                {
                    ShowFile = true;
                }

                try
                {
                    List<String> outList = FileParser.ParseFile(args[0], ref exceptionList, desiredSize);

                    File.WriteAllLines(args[1], outList);

                    if (ShowFile)
                    {
                        foreach (string s in outList)
                        {
                            Console.WriteLine(s);
                        }

                        Console.WriteLine();
                    }

                    if(exceptionList.Count > 0)
                    {
                        foreach (FileParserException ex in exceptionList)
                        {
                            StringBuilder s = new StringBuilder();

                            if(ex.ErrorType == 1 && ShowWarnings)
                            {
                                s.Append("Warning");
                            }
                            else if(ex.ErrorType == 2 && ShowInfo)
                            {
                                s.Append("Information");
                            }
                            else
                            {
                                s.Append("Error");
                            }

                            s.AppendFormat(" in file: {0}\nLine: {1}\nMessage: {2}\n\n",args[0], ex.LineNumber, ex.Message);
                            Console.Error.Write(s);
                        }
                    }
                }
                catch(Exception ex)
                {
                    StringBuilder s = new StringBuilder();
                    s.AppendFormat("There was an error processing the assembly file.\n\nError: {0}\nMessage: {1}", ex.GetType().ToString(), ex.Message);
                    Console.Error.Write(s);
                }
            }

            Console.WriteLine("Assembler Finished. Press any key to continue.");
            Console.ReadKey();

        }
}
    }
