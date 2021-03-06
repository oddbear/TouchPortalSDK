using System;
using System.Diagnostics.CodeAnalysis;
using TouchPortalSDK.Models;

namespace TouchPortalSDK.Exceptions
{
    [SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly")]
    public class TouchPortalClosedException : Exception
    {
        private readonly string _stackTrace;
        public override string StackTrace => _stackTrace ?? base.StackTrace;

        public MessageClose MessageClose { get; }

        public TouchPortalClosedException(MessageClose messageClose)
        {
            _stackTrace = Environment.StackTrace;

            MessageClose = messageClose;
        }
    }
}
