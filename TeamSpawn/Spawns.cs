using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TeamSpawn
{
    [Serializable()]
    public class Spawns
    {
        public Point[] spawns;
        public bool forceSpawn;

        public Spawns()
        {
            spawns = new Point[4]
            { 
                new Point(-1, -1),
                new Point(-1, -1),
                new Point(-1, -1),
                new Point(-1, -1)
            };

            forceSpawn = false;
        }

        public Point GetSpawn( int id )
        {
            return spawns[id];
        }

        public void SetSpawn(Point p, int id)
        {
            spawns[id] = p;
        }
    }
}
