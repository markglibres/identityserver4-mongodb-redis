using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Identity.Common
{
    public class RequiredArgumentException : Exception
    {
        public RequiredArgumentException(string field,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
            : base(GetMessage($"Argument {nameof(field)} is required", memberName, sourceFilePath, sourceLineNumber))
        {
        }

        private static string GetMessage(
            string message,
            string memberName,
            string sourceFilePath,
            int sourceLineNumber)
        {
            var error = new RequiredArgumentMessage
            {
                Message = message,
                ClassName = memberName,
                FilePath = sourceFilePath,
                FileNumber = sourceLineNumber
            };

            return JsonConvert.SerializeObject(error);
        }
    }

    internal class RequiredArgumentMessage
    {
        public string Message { get; set; }
        public string ClassName { get; set; }
        public string FilePath { get; set; }
        public int FileNumber { get; set; }
    }
}