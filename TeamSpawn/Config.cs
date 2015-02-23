using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace TeamSpawn
{
    public class Config
    {
	    public Spawns Spawns = new Spawns();

		/// <summary>
		/// Reads a configuration file from a given path
		/// </summary>
		/// <param name="path">string path</param>
		/// <returns>ConfigFile object</returns>
		public static Config Read(string path)
		{
			if (!File.Exists(path))
			{
				var c = new Config();
				c.Spawns.spawns = new List<Point>(4)
				{ 
					new Point(-1, -1),
					new Point(-1, -1),
					new Point(-1, -1),
					new Point(-1, -1)
				};
				c.Spawns.GroupIds = new Dictionary<string, int>();
				c.Spawns.forceSpawn = false;

				return c;
			}
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return Read(fs);
			}
		}

		/// <summary>
		/// Reads the configuration file from a stream
		/// </summary>
		/// <param name="stream">stream</param>
		/// <returns>ConfigFile object</returns>
		public static Config Read(Stream stream)
		{
			using (var sr = new StreamReader(stream))
			{
				var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
				return cf;
			}
		}

		/// <summary>
		/// Writes the configuration to a given path
		/// </summary>
		/// <param name="path">string path - Location to put the config file</param>
		public void Write(string path)
		{
			using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				Write(fs);
			}
		}

		/// <summary>
		/// Writes the configuration to a stream
		/// </summary>
		/// <param name="stream">stream</param>
		public void Write(Stream stream)
		{
			var str = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
			using (var sw = new StreamWriter(stream))
			{
				sw.Write(str);
			}
		}
    }
}
