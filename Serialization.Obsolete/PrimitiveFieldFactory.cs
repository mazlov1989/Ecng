namespace Ecng.Serialization
{
	#region Using Directives

	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using Ecng.Common;

	#endregion

	public class PrimitiveFieldFactory<I, S> : FieldFactory<I, S>
	{
		public PrimitiveFieldFactory(Field field, int order)
			: base(field, order)
		{
		}

		protected internal override ValueTask<I> OnCreateInstance(ISerializer serializer, S source, CancellationToken cancellationToken)
		{
			return new(source.To<I>());
		}

		protected internal override ValueTask<S> OnCreateSource(ISerializer serializer, I instance, CancellationToken cancellationToken)
		{
			return new(instance.To<S>());
		}

		public override FieldFactory Clone()
		{
			return new PrimitiveFieldFactory<I, S>(Field, Order);
		}
	}

	public class PrimitiveAttribute : ReflectionFieldFactoryAttribute
	{
		#region ReflectionFieldFactoryAttribute Members

		protected override Type GetFactoryType(Field field)
		{
			return typeof(PrimitiveFieldFactory<,>).Make(field.Type, field.Type);
		}

		#endregion
	}

	class PrimitiveEntityFactory<TEntity> : EntityFactory<TEntity>
	{
		public PrimitiveEntityFactory(string name)
		{
			if (name.IsEmpty())
				throw new ArgumentNullException(nameof(name));

			Name = name;
		}

		public string Name { get; }

		public override bool FullInitialize => true;

		public override ValueTask<TEntity> CreateEntity(ISerializer serializer, SerializationItemCollection source, CancellationToken cancellationToken)
		{
			return new(source[Name].Value.To<TEntity>());
		}
	}
}