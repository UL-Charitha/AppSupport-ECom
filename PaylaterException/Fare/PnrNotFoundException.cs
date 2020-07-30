using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaylaterException.Fare
{
    public class PnrNotFoundException:PaylaterCommonException
    {
        public PnrNotFoundException(Int32 ErrorCode, string Message) : base(ErrorCode, Message) { }
    }
}
