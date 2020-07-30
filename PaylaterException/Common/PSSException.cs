using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaylaterException.Common
{
    /// <summary>
    /// throws any exception occured in PSS system
    /// </summary>
    public class PSSException: PaylaterCommonException
    {
        public PSSException(Int32 ErrorCode, string Message) : base(ErrorCode, Message) { }
    }
}
