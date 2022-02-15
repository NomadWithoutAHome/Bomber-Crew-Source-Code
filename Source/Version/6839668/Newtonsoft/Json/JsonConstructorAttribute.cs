using System;

namespace Newtonsoft.Json;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property, AllowMultiple = false)]
public sealed class JsonConstructorAttribute : Attribute
{
}
