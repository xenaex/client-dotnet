namespace XenaExchange.Client.Rest
{
    /// <summary>
    /// Is just a container for 400 BadRequest body deserialization.
    /// </summary>
    public class ErrorResponse
    {
        public string Error { get; set; }
    }
}