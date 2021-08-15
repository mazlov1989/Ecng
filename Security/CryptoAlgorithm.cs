namespace Ecng.Security
{
	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography;

	using Ecng.Common;
	using Ecng.Reflection;
	using Ecng.Serialization;

	using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;

	public enum AlgorithmTypes
	{
		Symmetric,
		Asymmetric,
		Dpapi,
		Hash,
	}

	[Serializable]
	public class CryptoAlgorithm : Serializable<CryptoAlgorithm>, IDisposable
	{
		#region Private Fields

		private static readonly IDictionary<string, Type> _types;

		private readonly AlgorithmTypes _type;
		private readonly SymmetricCryptographer _symmetric;
		private readonly AsymmetricCryptographer _asymmetric;
		private readonly DpapiCryptographer _dpapi;
		private readonly HashCryptographer _hash;

		#endregion

		#region CryptoAlgorithm.cctor()

		static CryptoAlgorithm()
		{
			_types = new Dictionary<string, Type>();

			foreach (var entry in typeof(CryptoConfig).GetValue<VoidType, Dictionary<string, object>>("DefaultNameHT", null))
			{
				if (entry.Value is Type value)
					_types.Add(entry.Key, value);
			}

			_types.Add(DefaultDpapiAlgoName, typeof(DpapiCryptographer));
		}

		#endregion

		#region CryptoAlgorithm.ctor()

		public CryptoAlgorithm(SymmetricCryptographer symmetric)
		{
			_type = AlgorithmTypes.Symmetric;
			_symmetric = symmetric ?? throw new ArgumentNullException(nameof(symmetric));
		}

		public CryptoAlgorithm(AsymmetricCryptographer asymmetric)
		{
			_type = AlgorithmTypes.Asymmetric;
			_asymmetric = asymmetric ?? throw new ArgumentNullException(nameof(asymmetric));
		}

		public CryptoAlgorithm(DpapiCryptographer dpapi)
		{
			_type = AlgorithmTypes.Dpapi;
			_dpapi = dpapi ?? throw new ArgumentNullException(nameof(dpapi));
		}

		public CryptoAlgorithm(HashCryptographer hash)
		{
			_type = AlgorithmTypes.Hash;
			_hash = hash ?? throw new ArgumentNullException(nameof(hash));
		}

		#endregion

		#region Public Constants

		public const string DefaultSymmetricAlgoName = "Rijndael";
		public const string DefaultAsymmetricAlgoName = "RSA";
		public const string DefaultDpapiAlgoName = "Dpapi";
		public const string DefaultHashAlgoName = "SHA";

		public const DataProtectionScope DefaultScope = DataProtectionScope.LocalMachine;

		#endregion

		#region GetAlgType

		public static AlgorithmTypes GetAlgType(string name)
		{
			if (name.IsEmpty())
				throw new ArgumentNullException(nameof(name));

			return GetAlgType(_types[name]);
		}

		public static AlgorithmTypes GetAlgType(Type type)
		{
			if (type is null)
				throw new ArgumentNullException(nameof(type));

			if (type == typeof(DpapiCryptographer))
				return AlgorithmTypes.Dpapi;
			else if (typeof(SymmetricAlgorithm).IsAssignableFrom(type))
				return AlgorithmTypes.Symmetric;
			else if (typeof(AsymmetricAlgorithm).IsAssignableFrom(type))
				return AlgorithmTypes.Asymmetric;
			else if (typeof(HashAlgorithm).IsAssignableFrom(type))
				return AlgorithmTypes.Hash;
			else
				throw new ArgumentException("Type {0} doesnt't supported.".Put(type), nameof(type));
		}

		#endregion

		public static Type GetDefaultAlgo(AlgorithmTypes type)
		{
			var name = type switch
			{
				AlgorithmTypes.Symmetric => DefaultSymmetricAlgoName,
				AlgorithmTypes.Asymmetric => DefaultAsymmetricAlgoName,
				AlgorithmTypes.Dpapi => DefaultDpapiAlgoName,
				AlgorithmTypes.Hash => DefaultHashAlgoName,
				_ => throw new ArgumentOutOfRangeException(nameof(type)),
			};
			return GetAlgo(name);
		}

		public static Type GetAlgo(string name)
		{
			if (_types.TryGetValue(name, out var type))
				return type;

			if (name == "RSA")
				return typeof(RSACryptoServiceProvider);
			else if (name == "SHA")
				return typeof(SHA1Managed);

			throw new ArgumentException("Algorithm {0} not found".Put(name), nameof(name));
		}

		#region Create

		public static CryptoAlgorithm CreateAssymetricVerifier(byte[] publicKey) => new(new AsymmetricCryptographer(GetDefaultAlgo(AlgorithmTypes.Asymmetric), publicKey));

		public static CryptoAlgorithm Create(AlgorithmTypes type, params ProtectedKey[] keys)
		{
			return Create(GetDefaultAlgo(type), keys);
		}

		public static CryptoAlgorithm Create(Type type, params ProtectedKey[] keys)
		{
			if (keys is null)
				throw new ArgumentNullException(nameof(keys));

			var name = GetAlgType(type);

			return name switch
			{
				AlgorithmTypes.Symmetric => new CryptoAlgorithm(new SymmetricCryptographer(type, keys[0])),
				AlgorithmTypes.Asymmetric => new CryptoAlgorithm(new AsymmetricCryptographer(type, keys[0], keys[1])),
				AlgorithmTypes.Dpapi => new CryptoAlgorithm(new DpapiCryptographer(DefaultScope)),
				AlgorithmTypes.Hash => new CryptoAlgorithm(keys.Length == 0 ? new HashCryptographer(type) : new HashCryptographer(type, keys[0])),
				_ => throw new ArgumentOutOfRangeException(nameof(name), name.To<string>()),
			};
		}

		#endregion

		#region Encrypt

		public byte[] Encrypt(byte[] data)
		{
			return _type switch
			{
				AlgorithmTypes.Symmetric => _symmetric.Encrypt(data),
				AlgorithmTypes.Asymmetric => _asymmetric.Encrypt(data),
				AlgorithmTypes.Dpapi => _dpapi.Encrypt(data),
				AlgorithmTypes.Hash => _hash.ComputeHash(data),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		#endregion

		#region Decrypt

		public byte[] Decrypt(byte[] data)
		{
			return _type switch
			{
				AlgorithmTypes.Symmetric => _symmetric.Decrypt(data),
				AlgorithmTypes.Asymmetric => _asymmetric.Decrypt(data),
				AlgorithmTypes.Dpapi => _dpapi.Decrypt(data),
				AlgorithmTypes.Hash => throw new NotSupportedException(),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		#endregion

		public byte[] CreateSignature(byte[] data)
		{
			return _type switch
			{
				AlgorithmTypes.Asymmetric => _asymmetric.CreateSignature(data),
				_ => throw new NotSupportedException(),
			};
		}

		public bool VerifySignature(byte[] data, byte[] signature)
		{
			return _type switch
			{
				AlgorithmTypes.Asymmetric => _asymmetric.VerifySignature(data, signature),
				_ => throw new NotSupportedException(),
			};
		}

		#region Disposable Members

		public void Dispose()
		{
			if (_symmetric != null)
				_symmetric.Dispose();

			if (_asymmetric != null)
				_asymmetric.Dispose();
		}

		#endregion

		protected override bool OnEquals(CryptoAlgorithm other)
		{
			return _type == other._type;
		}

		protected override void Serialize(ISerializer serializer, FieldList fields, SerializationItemCollection source)
		{
			throw new NotImplementedException();
		}

		protected override void Deserialize(ISerializer serializer, FieldList fields, SerializationItemCollection source)
		{
			throw new NotImplementedException();
		}
	}
}