using System;

namespace DIContainer.Models;

public enum Lifetime
{
    Singleton,
    Transient
}
    
public class RegisteredType
{
    public Lifetime Lifetime { get; init; }
    public Type Type { get; init; }
}