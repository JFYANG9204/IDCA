
namespace IDCA.Bll.MDMDocument
{
    public interface IScript : IMDMObject
    {
        string Name { get; }
        bool Default { get; }
        string Text { get; }
    }

    public interface IScriptType : IMDMObject
    {
        string Type { get; }
        string Context { get; }
        InterviewModes InterviewMode { get; }
        bool UseKeyCodes { get; }
    }

    public enum InterviewModes
    {
        Default = -1,
        Web = 0,
        Phone = 1,
        Local = 2,
        DataEntry = 3,
    }

    public interface IRoutingItem : IMDMObject
    {
        string Name { get; }
        string Item { get; }
    }

    public interface IRouting : IMDMObject, IMDMObjectCollection<RoutingItem>
    {
        string Context { get; }
        InterviewModes InterviewMode { get; }
        bool UseKeyCodes { get; }
    }

    public interface IRoutings<T, S> : IMDMObject, IMDMObjectCollection<T>
    {
        IMDMObjectCollection<S>? Scripts { get; }
    }

}
