using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaylaterException.Common
{
   public class InvalidRequestInputException : PaylaterCommonException
    {
        public InvalidRequestInputException(Int32 ErrorCode, string Message) : base(ErrorCode, Message) { }
    }
}
