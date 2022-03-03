
using System.Collections;
using System.Runtime.InteropServices;

namespace IDCA.Bll.MddDocument
{
    public interface ILanguages : IEnumerable
    {
        ILanguage this[[In] object index] { get; }
        string Current { get; set; }
        short Count { get; }
        string Base { get; set; }
        IProperties Properties { get; }
        ObjectTypesConstants ObjectTypeValue { get; }
        bool Exist { get; }
        void Add(string newVal);
        void Remove(string name);
        new IEnumerator GetEnumerator();
    }
}
