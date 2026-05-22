namespace BizKotei.Client.Services.Interfaces;

public interface ICacheable
{
    public string? _id { get; set; }
    public DateTime _expire { get; set; }
}