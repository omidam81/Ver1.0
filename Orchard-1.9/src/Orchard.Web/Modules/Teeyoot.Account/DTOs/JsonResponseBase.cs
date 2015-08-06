namespace Teeyoot.Account.DTOs
{
    public abstract class JsonResponseBase
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}