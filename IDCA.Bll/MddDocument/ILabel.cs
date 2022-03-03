using System.Runtime.InteropServices;

namespace IDCA.Bll.MddDocument
{
    public interface ILabel : IMDMObject
    {
        string this[[Optional][In] object context, [Optional][In] object language] { get; set; }
        string TextAt { get; }
        ILabel Reference { get; set; }
        void Clear([Optional][In] object context, [Optional][In] object language);
    }
}
