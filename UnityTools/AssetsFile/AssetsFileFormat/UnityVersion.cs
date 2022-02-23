using System;
using System.IO;
using UnityTools.Utils;

namespace UnityTools
{
    public class UnityVersion
    {
        public int Major;
        public int Minor;
        public int Build;
        public UnityVersionType Type;
        public int TypeNumber;

        public UnityVersion(string version)
        {
            if (version is null)
            {
                throw new ArgumentNullException(nameof(version));
            }
            if (version == string.Empty)
            {
                throw new ArgumentException(nameof(version));
            }

            var splitVersion = version.Split('.');
            Major = int.Parse(splitVersion[0]);
            Minor = int.Parse(splitVersion[1]);

            var build = true;
            foreach (var c in splitVersion[2])
            {
                if (char.IsDigit(c))
                {
                    if (build)
                        Build = c.ParseDigit();
                    else
                        TypeNumber = c.ParseDigit();
                }
                else
                {
                    Type = ToUnityVersionType(c);
                    build = false;
                }
            }
        }

        public UnityVersion(int major)
        {
            Major = major;
        }

        public UnityVersion(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        public UnityVersion(int major, int minor, int build)
        {
            Major = major;
            Minor = minor;
            Build = build;
        }

        public UnityVersion(int major, int minor, int build, UnityVersionType type)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Type = type;
        }

        public UnityVersion(int major, int minor, int build, UnityVersionType type, int typeNumber)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Type = type;
            TypeNumber = typeNumber;
        }

        /// <summary>
        /// Serialize the version as a string
        /// </summary>
        /// <returns>A new string like 2019.4.3f1</returns>
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}{ToCharacter(Type)}{TypeNumber}";
        }

        /// <summary>
        /// Converts Unity version type to the relevant character
        /// </summary>
        /// <param name="versionType">A Unity version type</param>
        /// <returns>The character this value represents</returns>
        /// <exception cref="ArgumentOutOfRangeException">The type is not a valid value</exception>
        public string ToCharacter(UnityVersionType versionType)
        {
            return versionType switch
            {
                UnityVersionType.Alpha => "a",
                UnityVersionType.Beta => "b",
                UnityVersionType.China => "c",
                UnityVersionType.Final => "f",
                UnityVersionType.Patch => "p",
                UnityVersionType.Experimental => "x",
                _ => throw new Exception($"Unsupported version type {versionType}"),
            };
        }

        /// <summary>
		/// Parse a character into a Unity Version Type
		/// </summary>
		/// <param name="c">The character</param>
		/// <returns>The Unity Version Type this value represents</returns>
		/// <exception cref="ArgumentException">No version type for character</exception>
        public UnityVersionType ToUnityVersionType(char c)
        {
            return c switch
            {
                'a' => UnityVersionType.Alpha,
                'b' => UnityVersionType.Beta,
                'c' => UnityVersionType.China,
                'f' => UnityVersionType.Final,
                'p' => UnityVersionType.Patch,
                'x' => UnityVersionType.Experimental,
                _ => throw new ArgumentException($"There is no version type {c}", nameof(c)),
            };
        }
    }
}
