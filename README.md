# DIContainer
Simple dependency injection container.

Description: 
  - Simple DI container which can register classes and return singltones or new instance of registered class.
  - If class or interface registered as singlton it will be used for all dependencies, you registered after if it is needed.
  - If class was registered as scoped and it is used in injection to other constructors, it will be created new instances for every injection.

Usage: 
  - Create new dependency collection by colling: CollectionFactory.CreateCollection();
  - Register your classes like singleton or scoped: collection
                                                    .RegisterSingleton<TInterface, TClass>()
                                                    .RegisterScoped<TClass>();
  - Build dependencyCollection by calling: collection.Build();
  - Get any class yoy`ve been register to collection by calling: buildedCollection.Get<TInterface || TClass>();
