
namespace IDCA.Bll.MDMDocument
{
    public interface IClass : IMDMNamedObject
    {
        Types? Types { get; }
        Fields? Fields { get; }
        Pages? Pages { get; }
    }
}
