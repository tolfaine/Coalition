#if false
using asink.util;
using System.IO;
using UnityEngine;

[TestFixture]
public class UnityDirectoryAdapterTest
{
	private string projectPath = Path.GetDirectoryName(Application.dataPath); //Path.GetFullPath(Application.dataPath + Path. "..");

	private UnityDirectoryAdapter adapter = new UnityDirectoryAdapter();

	//happens between each test in the the fixture
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void AssetPathToFullPath()
	{
		assetPathToFullPath(@"Assets/Scripts");
	}

	public void assetPathToFullPath(string partialPath)
	{
		//input Assets\Scripts
		//ouput /Users/alexsink/dev/gamejambase/Assets\Scripts
		//expected: /Users/alexsink/dev/gamejambase/Assets../Assets\Scripts
		string fullPath = adapter.assetPathToFullPath(partialPath);
		string expected = (projectPath + Path.DirectorySeparatorChar + partialPath + Path.DirectorySeparatorChar);
		//Debug.Log(string.Format("input {0}  ouput {1} expected: {2}", partialPath, fullPath,expected));
		Assert.True(fullPath == expected);
	}

	[Test]
	public void FullPathToAssetPath1()
	{
		string fullPath = (projectPath + Path.DirectorySeparatorChar + @"Assets" + Path.DirectorySeparatorChar + "Scripts");
		FullPathToAssetPath(fullPath);
	}

	[Test]
	public void FullPathToAssetPathHandlesMangledSlashes()
	{
		string fullPath = (projectPath + "/" + @"Assets/Scripts");
		FullPathToAssetPath(fullPath);
	}

	[Test]
	public void GetFullPathFromBadSlashesPathTest()
	{
		//no slash between gamejambase and assets
		string fullPath = (projectPath + "Assets\\Scripts"); //note: no slash between project path and scripts
		Assert.Throws(typeof(DirectoryNotInProjectPathException), () => adapter.fullPathToAssetsPath(fullPath));
	}

	[Test]
	public void GetFullPathFromNonProjectPathTest()
	{
		//no slash between gamejambase and assets
		string fullPath = (projectPath + "AssetsCows\\Scripts"); //note: no assetscows
		Assert.Throws(typeof(DirectoryNotInProjectPathException), () => adapter.fullPathToAssetsPath(fullPath));
	}

	public void FullPathToAssetPath(string fullPathIn)
	{
		string partialPath = adapter.fullPathToAssetsPath(fullPathIn);
		string fullPathClean = adapter.cleanPath(fullPathIn);
		Assert.True(fullPathClean.Replace(projectPath + Path.DirectorySeparatorChar, "") == partialPath);
	}
}
#endif