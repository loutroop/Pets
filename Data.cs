using System;
using System.IO;
namespace Pets
{
	[Serializable]
	public class Data
	{
		public string UserId { get; set; }
		public RoleType Pet_role { get; set; }
	}
	public class Methods
	{
		internal static string StatFilePath = Path.Combine(Qurre.PluginManager.PluginsDirectory, "Pets");
		public static Data LoadData(string userId)
		{
			if (!File.Exists(StatFilePath)) Directory.CreateDirectory(StatFilePath);
			string path = Path.Combine(StatFilePath, $"{userId}.txt");
			if (File.Exists(path)) return DeserializeStats(path);
			var _d = new Data()
			{
				UserId = userId,
				Pet_role = Plugin.CustomConfig.Pet_role,
			};
			string[] write = new[] { _d.UserId, _d.Pet_role.ToString() };
			File.Create(path).Close();
			File.WriteAllLines(path, write);
			return _d;
		}
		private static Data DeserializeStats(string path)
		{
			string[] read = File.ReadAllLines(path);
			Data stats = new()
			{
				UserId = read[0],
				Pet_role = (RoleType)Enum.Parse(typeof(RoleType), read[1]),
			};
			return stats;
		}
	}
}