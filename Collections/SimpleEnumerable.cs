namespace Ecng.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class SimpleEnumerable<T> : IEnumerable<T>
	{
		private readonly Func<IEnumerator<T>> _createEnumerator;

		public SimpleEnumerable(Func<IEnumerator<T>> createEnumerator)
		{
			_createEnumerator = createEnumerator ?? throw new ArgumentNullException(nameof(createEnumerator));
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _createEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}