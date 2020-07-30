using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;

namespace Util
{
    public class PaylaterLogger
    {
        public static void Error(String message)
        {
            StackFrame frame = new StackFrame(1);
            var method = frame.GetMethod();
            var type = method.DeclaringType;
            var name = method.Name;
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append(type);
            messageBuilder.Append(".");
            messageBuilder.Append(name);
            messageBuilder.Append("()");
            messageBuilder.Append(" -> ");
            messageBuilder.Append(message);
            Logger.Write(messageBuilder.ToString(), "Error");
        }

        public static void Info(String message)
        {
            StackFrame frame = new StackFrame(1);
            var method = frame.GetMethod();
            var type = method.DeclaringType;
            var name = method.Name;
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append(type);
            messageBuilder.Append(".");
            messageBuilder.Append(name);
            messageBuilder.Append("()");
            messageBuilder.Append(" -> ");
            messageBuilder.Append(message);
            Logger.Write(messageBuilder.ToString(), "Info");
        }
    }
}
