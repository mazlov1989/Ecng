namespace Ecng.Interop
{
	using System;
	using System.Diagnostics;

	using Ecng.Common;

	public abstract class DllLibrary : Disposable
	{
		protected DllLibrary(string dllPath)
		{
			DllPath = dllPath.ThrowIfEmpty(nameof(dllPath));
			Handler = Marshaler.LoadLibrary(dllPath);
		}

		public string DllPath { get; private set; }

		private Version _dllVersion;
		public Version DllVersion => _dllVersion ??= FileVersionInfo.GetVersionInfo(DllPath).ProductVersion?.Replace(',', '.')?.RemoveSpaces()?.To<Version>();

		protected IntPtr Handler { get; }

		protected T GetHandler<T>(string procName) => Handler.GetHandler<T>(procName);

		protected T TryGetHandler<T>(string procName)
			where T : Delegate
			=> Handler.TryGetHandler<T>(procName);

		protected override void DisposeNative()
		{
			Handler.FreeLibrary();
			base.DisposeNative();
		}
	}
}