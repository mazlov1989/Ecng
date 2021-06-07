namespace Ecng.Common
{
	using System;
	using System.Collections.Generic;
	using System.Dynamic;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Security.Cryptography;

	public static class TypeHelper
	{
		private static readonly FieldInfo _remoteStackTraceString;

		static TypeHelper()
		{
			// Get the _remoteStackTraceString of the Exception class
			_remoteStackTraceString = typeof(Exception)
				.GetField("_remoteStackTraceString",
					BindingFlags.Instance | BindingFlags.NonPublic); // MS.Net

			if (_remoteStackTraceString == null)
				_remoteStackTraceString = typeof(Exception)
				.GetField("remote_stack_trace",
					BindingFlags.Instance | BindingFlags.NonPublic); // Mono
		}

		private static readonly Type _enumType = typeof(Enum);

		public static T CreateInstance<T>(this Type type, params object[] args)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (args == null)
				throw new ArgumentNullException(nameof(args));

			return Activator.CreateInstance(type, args).To<T>();
		}

		public static T CreateInstanceArgs<T>(this Type type, object[] args)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (args == null)
				throw new ArgumentNullException(nameof(args));

			Func<Type, object[], object> func = Activator.CreateInstance;
			return func(type, args).To<T>();
		}

		public static Type Make(this Type type, params Type[] args)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (args == null)
				throw new ArgumentNullException(nameof(args));

			return type.MakeGenericType(args);
		}

		public static Type Make(this Type type, IEnumerable<Type> args)
		{
			return type.Make(args.ToArray());
		}

		public static bool IsPrimitive(this Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return (
						type.IsPrimitive ||
						type.IsEnum() ||
						type == typeof(decimal) ||
						type == typeof(string) ||
						type == typeof(DateTime) ||
						type == typeof(DateTimeOffset) ||
						type == typeof(Guid) ||
						type == typeof(byte[]) ||
						type == typeof(TimeSpan) ||
						type == typeof(TimeZoneInfo)
					);
		}

		public static bool IsNumeric(this Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}

		public static string GetTypeAsString(this Type type, bool isAssemblyQualifiedName)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return Converter.GetAlias(type) ?? type.GetTypeName(isAssemblyQualifiedName) /*"{0}, {1}".Put(type.FullName, type.Assembly.GetName().Name)*/;
		}

		public static bool IsStruct(this Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return type.IsValueType && !type.IsEnum();
		}

		public static bool IsEnum(this Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			//
			// 2 times faster than Type.IsEnum
			//
			return type.BaseType == _enumType;
		}

		public static bool IsAttribute(this Type type)
		{
			return typeof(Attribute).IsAssignableFrom(type);
		}

		public static bool IsDelegate(this Type type)
		{
			return typeof(Delegate).IsAssignableFrom(type);
		}

#if !SILVERLIGHT
		public static bool IsWinColor(this Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return type == typeof(System.Drawing.Color);
		}
#endif

		public static TEntity CreateUnitialized<TEntity>()
		{
			return (TEntity)typeof(TEntity).CreateUnitialized();
		}

		public static object CreateUnitialized(this Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return FormatterServices.GetUninitializedObject(type);
		}

		[Obsolete("Use pattern matching.")]
		public static void DoIf<TSource, TDestination>(this TSource source, Action<TDestination> handler)
			where TDestination : class
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			var destination = source as TDestination;

			if (destination == null)
				return;

			handler(destination);
		}

		[Obsolete("Use pattern matching.")]
		public static void DoIfElse<T>(this object value, Action<T> ifAction, Action elseAction)
			where T : class
		{
			if (ifAction == null)
				throw new ArgumentNullException(nameof(ifAction));

			if (elseAction == null)
				throw new ArgumentNullException(nameof(elseAction));

			if (value is T typedValue)
				ifAction(typedValue);
			else
				elseAction();
		}

		public static void DoDispose<TSource>(this TSource source)
		{
			if (source is IDisposable disposable)
				disposable.Dispose();
		}

#if !SILVERLIGHT
		private static readonly Lazy<string> _applicationName = new Lazy<string>(() =>
		{
			var asm = Assembly.GetEntryAssembly();
			if (asm == null)
				return "None";
			var attr = asm.GetAttribute<AssemblyTitleAttribute>();
			return attr != null ? attr.Title : asm.GetName().Name;
		});

		public static string ApplicationName => _applicationName.Value;

		private static readonly Lazy<string> _applicationNameWithVersion = new Lazy<string>(() =>
		{
			var asm = Assembly.GetEntryAssembly();

			if (asm == null)
				return "None";

			return ApplicationName + " v" + asm.GetName().Version;
		});

		public static string ApplicationNameWithVersion => _applicationNameWithVersion.Value;
#endif

		// http://stackoverflow.com/questions/8517159/how-to-detect-at-runtime-that-net-version-4-5-currently-running-your-code
		public static bool IsNet45OrNewer()
		{
			// Class "ReflectionContext" exists from .NET 4.5 onwards.
			return Type.GetType("System.Reflection.ReflectionContext", false) != null;
		}

		public static string GetTypeName(this Type type, bool isAssemblyQualifiedName)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return isAssemblyQualifiedName ? type.AssemblyQualifiedName : "{0}, {1}".Put(type.FullName,
#if SILVERLIGHT
				new AssemblyName(type.Assembly.FullName).Name
#else
				type.Assembly.GetName().Name
#endif
			);
		}

		public static object GetDefaultValue(this Type type)
		{
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}

		// http://stackoverflow.com/questions/57383/in-c-how-can-i-rethrow-innerexception-without-losing-stack-trace
		public static void Throw(this Exception ex)
		{
			if (ex == null)
				throw new ArgumentNullException(nameof(ex));

			_remoteStackTraceString.SetValue(ex, ex.StackTrace + Environment.NewLine);

			throw ex;
		}

		public static byte[] GenerateSalt(int saltSize)
		{
			var salt = new byte[saltSize];

			using (var saltGen = new RNGCryptoServiceProvider())
				saltGen.GetBytes(salt);

			return salt;
		}

		public static Scope<T> ToScope<T>(this T value, bool ownInstance = true)
		{
			return new Scope<T>(value, ownInstance);
		}

		public static Exception SingleOrAggr(this IList<Exception> errors)
		{
			if (errors == null)
				throw new ArgumentNullException(nameof(errors));

			return errors.Count == 1 ? errors[0] : new AggregateException(errors);
		}

		public static T CheckOnNull<T>(this T value)
			where T : class
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			return value;
		}

		private static readonly HashSet<Type> TupleTypes = new HashSet<Type>
		{
			typeof(Tuple<>),
			typeof(Tuple<,>),
			typeof(Tuple<,,>),
			typeof(Tuple<,,,>),
			typeof(Tuple<,,,,>),
			typeof(Tuple<,,,,,>),
			typeof(Tuple<,,,,,,>),
			typeof(Tuple<,,,,,,,>),

			typeof(ValueTuple<>),
			typeof(ValueTuple<,>),
			typeof(ValueTuple<,,>),
			typeof(ValueTuple<,,,>),
			typeof(ValueTuple<,,,,>),
			typeof(ValueTuple<,,,,,>),
			typeof(ValueTuple<,,,,,,>),
			typeof(ValueTuple<,,,,,,,>),
		};

		public static bool IsTuple(this Type tupleType)
		{
			return
				tupleType != null &&
				tupleType.IsGenericType &&
				TupleTypes.Contains(tupleType.GetGenericTypeDefinition());
		}

		public static object[] TryTupleToValues(this object tuple)
		{
			var type = tuple?.GetType();

			if(type?.IsTuple() != true)
				return null;

			var numValues = type.GetProperties().Count(pi => pi.Name.StartsWith("Item"));

			var arr = new object[numValues];

			for (var i = 1; i <= numValues; ++i)
				arr[i - 1] = type.GetProperty("Item" + i).GetValue(tuple);

			return arr;
		}

		public static IEnumerable<object> ToValues<T>(this Tuple<T> tuple)
		{
			yield return tuple.Item1;
		}

		public static IEnumerable<object> ToValues<T1, T2>(this Tuple<T1, T2> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
		}

		public static IEnumerable<object> ToValues<T1, T2, T3>(this Tuple<T1, T2, T3> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
			yield return tuple.Item3;
		}

		public static IEnumerable<object> ToValues<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
			yield return tuple.Item3;
			yield return tuple.Item4;
		}

		public static IEnumerable<object> ToValues<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
			yield return tuple.Item3;
			yield return tuple.Item4;
			yield return tuple.Item5;
		}

		public static IEnumerable<object> ToValues<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
			yield return tuple.Item3;
			yield return tuple.Item4;
			yield return tuple.Item5;
			yield return tuple.Item6;
		}

		public static IEnumerable<object> ToValues<T>(this ValueTuple<T> tuple)
		{
			yield return tuple.Item1;
		}

		public static IEnumerable<object> ToValues<T1, T2>(this ValueTuple<T1, T2> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
		}

		public static IEnumerable<object> ToValues<T1, T2, T3>(this ValueTuple<T1, T2, T3> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
			yield return tuple.Item3;
		}

		public static IEnumerable<object> ToValues<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3, T4> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
			yield return tuple.Item3;
			yield return tuple.Item4;
		}

		public static IEnumerable<object> ToValues<T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
			yield return tuple.Item3;
			yield return tuple.Item4;
			yield return tuple.Item5;
		}

		public static IEnumerable<object> ToValues<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> tuple)
		{
			yield return tuple.Item1;
			yield return tuple.Item2;
			yield return tuple.Item3;
			yield return tuple.Item4;
			yield return tuple.Item5;
			yield return tuple.Item6;
		}

		public static object ToTuple(this IEnumerable<object> values)
		{
			var types = new List<Type>();
			var args = new List<object>();

			foreach (var value in values)
			{
				types.Add(value.GetType());
				args.Add(value);
			}

			var genericType = ("System.Tuple`" + types.Count).To<Type>();
			var specificType = genericType.Make(types);

			return specificType.CreateInstance<object>(args.ToArray());
		}

		public static Platforms GetPlatform(this Type type) => type.GetAttribute<TargetPlatformAttribute>()?.Platform ?? Platforms.AnyCPU;

		public static int HiWord(this int iValue)
		{
			return (iValue >> 16) & 0xFFFF;
		}

		public static int LoWord(this int iValue)
		{
			return iValue & 0xFFFF;
		}

		// https://stackoverflow.com/a/30528667
		public static bool HasProperty(this object settings, string name)
		{
			if (settings is ExpandoObject)
				return ((IDictionary<string, object>)settings).ContainsKey(name);

			return settings.GetType().GetProperty(name) != null;
		}
	}
}