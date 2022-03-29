
namespace IDCA.Bll.MDM
{
    public interface IClass : IMDMLabeledObject
    {
        Types? Types { get; }
        Fields? Fields { get; }
        Pages? Pages { get; }
    }
}
