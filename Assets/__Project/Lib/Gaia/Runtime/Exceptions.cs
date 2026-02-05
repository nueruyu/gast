using System;
using System.Net;

namespace Gast.Lib.Gaia
{
    public class GaiaException : Exception
    {
        public GaiaException(string message) : base(message) { }
        public GaiaException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class GaiaConnectionException : GaiaException
    {
        public GaiaConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class GaiaTimeoutException : GaiaConnectionException
    {
        public GaiaTimeoutException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class GaiaServerException : GaiaException
    {
        public HttpStatusCode StatusCode { get; }
        public string ResponseContent { get; }

        public GaiaServerException(HttpStatusCode statusCode, string responseContent, string message) : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }
}
