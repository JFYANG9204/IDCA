
namespace IDCA.Model.MDM
{
    public interface IPage : IMDMLabeledObject
    {
        string Reference { get; }
        Variables? Items { get; }
    }

    public interface IPages<T> : IMDMNamedCollection<T>
    {
        bool GlobalNamespace { get; }
    }

}
