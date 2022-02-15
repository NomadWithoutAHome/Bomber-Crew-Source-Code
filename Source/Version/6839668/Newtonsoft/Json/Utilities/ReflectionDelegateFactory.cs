using System;
using System.Globalization;
using System.Reflection;

namespace Newtonsoft.Json.Utilities;

internal abstract class ReflectionDelegateFactory
{
	public Func<T, object> CreateGet<T>(MemberInfo memberInfo)
	{
		if (memberInfo is PropertyInfo propertyInfo)
		{
			return CreateGet<T>(propertyInfo);
		}
		if (memberInfo is FieldInfo fieldInfo)
		{
			return CreateGet<T>(fieldInfo);
		}
		throw new Exception("Could not create getter for {0}.".FormatWith(CultureInfo.InvariantCulture, memberInfo));
	}

	public Action<T, object> CreateSet<T>(MemberInfo memberInfo)
	{
		if (memberInfo is PropertyInfo propertyInfo)
		{
			return CreateSet<T>(propertyInfo);
		}
		if (memberInfo is FieldInfo fieldInfo)
		{
			return CreateSet<T>(fieldInfo);
		}
		throw new Exception("Could not create setter for {0}.".FormatWith(CultureInfo.InvariantCulture, memberInfo));
	}

	public abstract MethodCall<T, object> CreateMethodCall<T>(MethodBase method);

	public abstract Func<T> CreateDefaultConstructor<T>(Type type);

	public abstract Func<T, object> CreateGet<T>(PropertyInfo propertyInfo);

	public abstract Func<T, object> CreateGet<T>(FieldInfo fieldInfo);

	public abstract Action<T, object> CreateSet<T>(FieldInfo fieldInfo);

	public abstract Action<T, object> CreateSet<T>(PropertyInfo propertyInfo);
}
