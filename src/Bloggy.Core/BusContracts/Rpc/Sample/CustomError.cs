namespace Bloggy.Core.BusContracts.Rpc.Sample;

public class CustomError : Error
{
    public string CustomProperty { get; set; }
}