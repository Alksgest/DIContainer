namespace DIContainer.Factories
{
    public static class CollectionFactory
    {
        public static IDependencyCollection CreateCollection()
        {
            return new DependencyCollection();;
        }
    }
}
