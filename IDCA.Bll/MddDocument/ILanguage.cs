

namespace IDCA.Bll.MddDocument
{
    public interface ILanguage
    {
        string Name { get; }
        string Country { get; }
        string LongName { get; }
        string ItemData { get; }
        IProperties Properties { get; }
        bool IsInstalled { get; }
        string XMLName { get; }
    }
}
