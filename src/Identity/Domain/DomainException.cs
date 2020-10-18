using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Identity.Domain
{
    public class DomainException : Exception
    {
        protected DomainException()
        {
        }

        public DomainException(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0) 
            : base(FormatMessage(message, memberName, sourceFilePath, sourceLineNumber))
        {
        }

        private static string FormatMessage(
            string message,
            string memberName,
            string sourceFilePath,
            int sourceLineNumber)
        {
            var error = new ExceptionMessage
            {
                Message = message,
                ClassName = memberName,
                FilePath = sourceFilePath,
                FileNumber = sourceLineNumber
            };

            return JsonConvert.SerializeObject(error);
        }
    }

    public class ExceptionMessage
    {
        public string Message { get; set; }
        public string ClassName { get; set; }
        public string FilePath { get; set; }
        public int FileNumber { get; set; }
    }
}