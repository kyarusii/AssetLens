using AssetLens.Reference;
using NUnit.Framework;
using UnityEngine;

namespace AssetLens.Tests
{
	public class VersionTest
	{
		[Test]
		public void CreateVersionFromUInt()
		{
			const uint version_1_12_8 = 11208;

			Version version = new Version(version_1_12_8);
			
			Debug.Log(version.ToString());
            
			Assert.True(version.major == 1U);
			Assert.True(version.minor == 12U);
			Assert.True(version.maintenance == 8U);
		}

		[Test]
		public void VersionToUInt()
		{
			Version version = new Version(38201);

			Assert.True(version == 38201);
		}
	}
}