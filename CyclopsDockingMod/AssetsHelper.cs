using System.IO;
using System.Reflection;
using UnityEngine;

namespace CyclopsDockingMod
{
	internal static class AssetsHelper
    {
        public static readonly AssetBundle Assets = AssetBundle.LoadFromFile(FilesHelper.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets/models.assets"));

        public static FMODAsset CreateAsset(string id, string name, string path)
		{
			FMODAsset fmodasset = ScriptableObject.CreateInstance<FMODAsset>();
			fmodasset.name = name;
			fmodasset.id = id;
			fmodasset.path = path;
			return fmodasset;
		}
	}
}
