
namespace IDCA.Bll.MDMDocument
{
    public interface IPage : IMDMObject
    {
        string Id { get; }
        string Name { get; }
        string Reference { get; }
    }

    public interface IPages<T> : IMDMObjectCollection<T>
    {
        string Name { get; }
        bool GlobalNamespace { get; }
    }

}
