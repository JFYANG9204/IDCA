
namespace IDCA.Bll.MddDocument
{
    public interface IContext
    {
        string Name { get; set; }

        IProperties Properties { get; }

        string Description { get; set; }

        ContextUsageConstants Usage { get; set; }
    }
}
