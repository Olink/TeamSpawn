using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace TeamSpawn
{
    public static class Config
    {
        public static Spawns WriteFile(String file)
        {
            TextWriter tw = new StreamWriter(file);

            Spawns spawns = new Spawns();

            tw.Write(JsonConvert.SerializeObject(spawns, Newtonsoft.Json.Formatting.Indented));
            tw.Close();

            return spawns;
        }

        public static void Save(String file, Spawns spawns)
        {
            /*if( File.Exists(file))
            {
                File.Delete(file);
            }*/
            TextWriter tw = new StreamWriter(file);
            tw.Write(JsonConvert.SerializeObject(spawns, Newtonsoft.Json.Formatting.Indented));
            tw.Close();
        }

        public static Spawns ReadFile(String file)
        {
            Spawns spawns;
            using (var tr = new StreamReader(file))
            {
                String raw = tr.ReadToEnd();
                spawns = JsonConvert.DeserializeObject<Spawns>(raw);
            }
            return spawns;
        }
    }
}
