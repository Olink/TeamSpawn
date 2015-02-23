using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TeamSpawn
{
    [Serializable()]
    public class Spawns
    {
        public List<Point> spawns = new List<Point>();
        public bool forceSpawn;
        public Dictionary<string, int> GroupIds = new Dictionary<string, int>();

        public Spawns()
        {
        }

        public Point GetSpawn( int id )
        {
            if(id >= spawns.Count)
                return new Point(-1,-1);

            return spawns[id];
        }

        public Point GetSpawn(string group)
        {
            if(GroupIds.ContainsKey(group))
                return GetSpawn(GroupIds[group]);

            return new Point(-1,-1);
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
