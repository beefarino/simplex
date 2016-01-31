using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Text;

namespace CodeOwls.ScriptProvider.Persistence
{
    [Serializable]
    public class ScriptProviderException : ApplicationException
    {
        public ScriptProviderException()
        {
        }

        public ScriptProviderException(string message) : base(message)
        {
        }

        public ScriptProviderException(string message, IEnumerable<ErrorRecord> errors) : base( FormatMessage(message, errors) )
        {
        }

        static string FormatMessage(string message, IEnumerable<ErrorRecord> errors)
        {
            var builder = new StringBuilder(message);
            builder.AppendLine();
            errors.ToList().ForEach( 
                error =>
                    {
                        builder.AppendLine();
                        builder.AppendFormat(error.ToString());
                        builder.AppendLine();
                        builder.AppendFormat("At line {0} char {1} of ScriptProvider script {2}",
                                             error.InvocationInfo.ScriptLineNumber,
                                             error.InvocationInfo.OffsetInLine,
                                             error.InvocationInfo.ScriptName);                        
                        builder.AppendLine();
                        builder.AppendLine("---------------------------------------");
                    });
            
            
            return builder.ToString();
        }
        protected ScriptProviderException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}