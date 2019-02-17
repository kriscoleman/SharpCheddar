using System;
using System.Runtime.CompilerServices;

namespace SharpCheddar.Core
{
    public class SharpCheddarInitializationException : InvalidOperationException
    {
        public SharpCheddarInitializationException([CallerFilePath] string filePath = "", [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMemberName = "") : base(
            $"The repository is not initialized. The action {callerMemberName} cannot be completed. Filepath = {filePath} Linenumber = {callerLineNumber}")
        {
        }
    }
}