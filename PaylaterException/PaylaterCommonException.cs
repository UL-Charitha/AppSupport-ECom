using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaylaterException
{
    public class PaylaterCommonException:Exception
    {
        public int ErrorCode { get; set; }
        
        public PaylaterCommonException(Int32 ErrorCode, string Message)
            : base(Message)
        {
            this.ErrorCode = ErrorCode;
        }
    }
}
