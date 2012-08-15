using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TeamSpawn
{
    [Serializable()]
    public class Spawns
    {
        public List<Point> spawns;
        public bool forceSpawn;
        private Dictionary<string, int> GroupIds;

        public Spawns()
        {
            spawns = new List<Point>(4)
            { 
                new Point(-1, -1),
                new Point(-1, -1),
                new Point(-1, -1),
                new Point(-1, -1)
            };

            GroupIds = new Dictionary<string, int>();
            forceSpawn = false;
        }

        public Point GetSpawn( int id )
        {
            return spawns[id];
        }

        public Point GetSpawn(string group)
        {
            return GetSpawn(GroupIds[group]);
        }

        public void SetSpawn(Point p, int id)
        {
            spawns[id] = p;
        }

        public void SetSpawn(Point p, string group)
        {
            SetSpawn(p, GroupIds[group]);
        }

        public void AddGroup(string group)
        {
            if (!GroupIds.ContainsKey(group))
            {
                GroupIds.Add(group, spawns.Count);
                spawns.Add(new Point(-1, -1));
            }
        }
    }
}
