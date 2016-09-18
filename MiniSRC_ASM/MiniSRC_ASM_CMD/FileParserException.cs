using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSRC_ASM_CMD
{
    class FileParserException : ApplicationException
    {
        private int lineNumber;
        private int errType;

        public FileParserException(String message,int theLineNumber,int errtype):base(message)
        {
            lineNumber = theLineNumber;
            errType = errtype;
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public int ErrorType
        {
            get { return errType; }
        }

    }
}
