using System.IO;
using System.Reflection;

namespace CyclopsDockingMod
{
	internal static class FilesHelper
	{
		public static string GetSaveFolderPath()
		{
			return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../../SNAppData/SavedGames/", SaveLoadManager.main.GetCurrentSlot(), "CyclopsDockingMod")).Replace('\\', '/');
		}

		public static string GetSaveFolderPath(string saveGame)
		{
			string text = FilesHelper.GetSaveFolderPath();
			if (text.Contains("/test/"))
			{
				if (string.IsNullOrEmpty(saveGame))
					return null;
				text = text.Replace("/test/", "/" + saveGame + "/");
			}
			return text.Replace('\\', '/');
		}

		public static string Combine(string path1, string path2)
		{
			return Path.Combine(path1, path2).Replace('\\', '/');
		}

		public static string Combine(string path1, string path2, string path3)
		{
			return Path.Combine(path1, path2, path3).Replace('\\', '/');
		}
	}
}
