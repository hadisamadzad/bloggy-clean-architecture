namespace Bloggy.Core.BusContracts.Rpc;

public class Error
{
    public Error()
    {
    }

    public Error(Enum error, string message = null)
    {
        ErrorCode = Convert.ToInt32(error);
        Message = message;
    }

    public int ErrorCode { get; set; }
    public string Message { get; set; }

    public bool Is(Enum e)
    {
        return Convert.ToInt32(e) == ErrorCode;
    }
}