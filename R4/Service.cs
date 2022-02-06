using System;

namespace R4;

public interface ISingletonService { }

[AttributeUsage(AttributeTargets.Class)]
public class DisableAutoDiscoverAttribute : Attribute { }
