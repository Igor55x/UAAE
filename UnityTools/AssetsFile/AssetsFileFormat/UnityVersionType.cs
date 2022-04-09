namespace UnityTools
{
	/// <summary>
	/// An enumeration representing the letter in a Unity Version string
	/// </summary>
	public enum UnityVersionType
	{
		/// <summary>
		/// Alpha version 'a'
		/// </summary>
		Alpha = 0,
		/// <summary>
		/// Beta version 'b'
		/// </summary>
		Beta,
		/// <summary>
		/// China version 'c'
		/// </summary>
		China,
		/// <summary>
		/// Final version 'f'
		/// </summary>
		Final,
		/// <summary>
		/// Patch version 'p'
		/// </summary>
		Patch,
		/// <summary>
		/// Experimental version 'x'
		/// </summary>
		Experimental,

		/// <summary>
		/// The minimum valid value for <see cref="UnityVersionType"/>
		/// </summary>
		MinValue = Alpha,
		/// <summary>
		/// The maximum valid value for <see cref="UnityVersionType"/>
		/// </summary>
		MaxValue = Experimental
	}
}
