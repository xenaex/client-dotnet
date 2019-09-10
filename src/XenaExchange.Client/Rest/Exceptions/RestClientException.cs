using System;
using System.Net;
using System.Runtime.Serialization;

namespace XenaExchange.Client.Rest.Exceptions
{
    /// <summary>
    /// Is thrown in case of any HTTP status code other than 200 OK.
    /// </summary>
    public class RestClientException : Exception
    {
        public readonly HttpStatusCode StatusCode;

        public RestClientException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        protected RestClientException(SerializationInfo info, StreamingContext context, HttpStatusCode statusCode)
            : base(info, context)
        {
            StatusCode = statusCode;
        }

        public RestClientException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public RestClientException(string message, Exception innerException, HttpStatusCode statusCode)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public override string Message => $"Status code: {StatusCode}, Message: {base.Message}";
    }
}