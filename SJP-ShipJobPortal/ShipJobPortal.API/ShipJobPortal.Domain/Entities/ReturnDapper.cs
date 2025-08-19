namespace SJP.Domain.Entities;

public class ReturnDapper<T>
{
    public List<T> ListResultset { get; set; } = new List<T>();
    public string ReturnStatus { get; set; }
    public string ErrorCode { get; set; }
}
