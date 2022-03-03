
using System.Collections;
using System.Runtime.InteropServices;

namespace IDCA.Bll.MddDocument
{
    public interface IContexts : IEnumerable
    {

        IContext this[[In] object index] { get; set; }

        int Count { get; }

        string Current { get; set; }

        string Base { get; set; }

        IProperties Properties { get; }

        ObjectTypesConstants ObjectTypeValue { get; }

        void Add([In] string context);

        void Remove([In] string context);

        new IEnumerator GetEnumerator();

        void AddEx([In] string context, bool unversioned);

        string MakeName([In] string text);
    }
}
