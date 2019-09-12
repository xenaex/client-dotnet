using Api;

namespace XenaExchange.Client.Rest
{
    /// <summary>
    /// Is used as a container for accounts response deserialization.
    /// </summary>
    public class ListAccountsResponse
    {
        public AccountInfo[] Accounts { get; set; }
    }
}