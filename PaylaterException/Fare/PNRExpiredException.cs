using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaylaterException.Fare
{
    public class PnrExpiredException:PaylaterCommonException
    {
        public PnrExpiredException(Int32 ErrorCode, string Message) : base(ErrorCode, Message) { }
    }
}
