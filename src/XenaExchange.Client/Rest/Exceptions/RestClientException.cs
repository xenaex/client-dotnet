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

        public readonly string RequestAbsoluteUri;

        public RestClientException(HttpStatusCode statusCode, string requestAbsoluteUri)
        {
            StatusCode = statusCode;
            RequestAbsoluteUri = requestAbsoluteUri;
        }

        protected RestClientException(SerializationInfo info, StreamingContext context, HttpStatusCode statusCode, string requestAbsoluteUri)
            : base(info, context)
        {
            StatusCode = statusCode;
            RequestAbsoluteUri = requestAbsoluteUri;
        }

        public RestClientException(string message, HttpStatusCode statusCode, string requestAbsoluteUri = null) : base(message)
        {
            StatusCode = statusCode;
            RequestAbsoluteUri = requestAbsoluteUri;
        }

        public RestClientException(string message, Exception innerException, HttpStatusCode statusCode, string requestAbsoluteUri)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            RequestAbsoluteUri = requestAbsoluteUri;
        }

        public override string Message => $"Uri: {RequestAbsoluteUri}, Status code: {StatusCode}, Message: {base.Message}";
    }
}