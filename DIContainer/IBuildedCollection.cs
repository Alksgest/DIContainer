using System.Text;

namespace DIContainer
{
    public interface IBuildedCollection
    {
        T Get<T>() where T : class;
    }
}
