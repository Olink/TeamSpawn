using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TeamSpawn;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;


namespace TeamSpawn
{
	[ApiVersion(1, 16)]
	public class TeamSpawn : TerrariaPlugin
	{
        private string savepath = Path.Combine(TShock.SavePath, "TeamSpawn.cfg");
		private Config config;

        private Dictionary<int, int> deadplayers; 

        public override Version Version
        {
            get { return new Version("1.1"); }
        }

        public override string Name
        {
            get { return "TeamSpawn"; }
        }

        public override string Author
        {
            get { return "Zach Piispanen"; }
        }

        public override string Description
        {
            get { return "Allows you to set spawn for the different teams, and groups!"; }
        }

        public TeamSpawn(Main game) : base(game)
        {
	        config = Config.Read(savepath);
			config.Write(savepath);
            deadplayers = new Dictionary<int,int>(255);
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("teamspawn", HandleCommand, "teamspawn"));
            TShockAPI.GetDataHandlers.PlayerTeam += ChangeTeam;
            GetDataHandlers.KillMe += HandleDeath;
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        private void OnUpdate(EventArgs e)
        {
            foreach( TSPlayer ply in TShock.Players )
            {
                if( ply != null && ply.Active)
                if( !ply.TPlayer.dead )
                {
                    if( deadplayers.ContainsKey(ply.Index) )
                    {
                        int team = deadplayers[ply.Index];
                        deadplayers.Remove(ply.Index);
                        Point spawn;
						spawn = config.Spawns.GetSpawn(ply.Group.Name, team);
						if (spawn.X == -1 || spawn.Y == -1)
						{
							ply.Spawn();
						}
						else
						{
							ply.Teleport(spawn.X * 16, spawn.Y * 16);
						}
                    }
                }
            }
        }

        private void HandleDeath( object sender, GetDataHandlers.KillMeEventArgs args )
        {
            if (args.Handled)
                return;

            byte PlayerID = args.PlayerId;
            byte hitDirection = args.Direction;
            Int16 Damage = args.Damage;
            bool PVP = args.Pvp;

            if( !deadplayers.ContainsKey(PlayerID) )
            {
                deadplayers.Add(PlayerID, TShock.Players[PlayerID].Team);
            }
            else
            {
                deadplayers.Remove(PlayerID);
                deadplayers.Add(PlayerID, TShock.Players[PlayerID].Team);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if( disposing )
            {
				config.Write(savepath);
                TShockAPI.GetDataHandlers.PlayerTeam -= ChangeTeam;
                GetDataHandlers.KillMe -= HandleDeath;
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        private void HandleCommand(CommandArgs args)
        {
            TSPlayer ply = args.Player;
            if (ply == null)
                return;

            if( args.Parameters.Count < 2 )
            {
                args.Player.SendMessage("Usage: /teamspawn [set|reset] [group name | 0-3]", Color.Red);
                args.Player.SendMessage("       /teamspawn [force] [true|false]", Color.Red);
                return;
            }

            if( args.Parameters[0] == "set" )
            {
                int id = -1;
                if( !int.TryParse(args.Parameters[1], out id) )
                {
                    //group name
                    if (TShock.Groups.GroupExists(args.Parameters[1]))
                    {
						config.Spawns.AddGroup(args.Parameters[1]);
                        Point p = new Point(args.Player.TileX, args.Player.TileY);
						config.Spawns.SetSpawn(p, args.Parameters[1]);
                        args.Player.SendMessage(String.Format("Group {0}'s spawn has been set to {1}, {2}", args.Parameters[1], p.X, p.Y), Color.Green);
                    }
                    else
                    {
                        args.Player.SendMessage(String.Format("Error: Group {0} does not exist", args.Parameters[1]), Color.Red);
                    }
                    return;
                }

                if( id >= 0 && id <= 3)
                {
                    Point p = new Point( args.Player.TileX, args.Player.TileY );
					config.Spawns.SetSpawn(p, id);
                    args.Player.SendMessage(String.Format("Team {0}'s spawn has been set to {1}, {2}", id, p.X, p.Y), Color.Green);
                }
                else
                {
                    args.Player.SendMessage("Error: Valid team ids are 0, 1, 2, and 3", Color.Red);
                    return;
                }
            }
            else if (args.Parameters[0] == "reset")
            {
                int id = -1;
                if (!int.TryParse(args.Parameters[1], out id))
                {
                    //group name
                    if (TShock.Groups.GroupExists(args.Parameters[1]))
                    {
						config.Spawns.AddGroup(args.Parameters[1]);
                        Point p = new Point(-1, -1);
						config.Spawns.SetSpawn(p, args.Parameters[1]);
                        args.Player.SendMessage(String.Format("Group {0}'s spawn has been reset", args.Parameters[1]), Color.Green);
                    }
                    else
                    {
                        args.Player.SendMessage(String.Format("Error: Group {0} does not exist", args.Parameters[1]), Color.Red);
                    }
                    return;
                }

                if (id >= 0 && id <= 3)
                {
                    Point p = new Point(-1, -1);
					config.Spawns.SetSpawn(p, id);
                    args.Player.SendMessage(String.Format("Team {0}'s spawn has been reset", id), Color.Green);
                }
                else
                {
                    args.Player.SendMessage("Error: Valid team ids are 0, 1, 2, and 3", Color.Red);
                    return;
                }
            }
            else if (args.Parameters[0] == "force")
            {
				bool force = config.Spawns.forceSpawn;
                if( !bool.TryParse(args.Parameters[1], out force))
                {
                    args.Player.SendMessage("Error: Valid force options are 'true' and 'false'", Color.Red);
                    return;
                }
				config.Spawns.forceSpawn = force;
                args.Player.SendMessage(String.Format("Team spawn force has been set to {0}", force), Color.Green);
            }
            else
            {
				args.Player.SendMessage("Usage: /teamspawn [set|reset] [group name | 0-3]", Color.Red);
                args.Player.SendMessage("       /teamspawn [force] [true|false]", Color.Red);
                return;
            }
        }

        private void ChangeTeam( object sender, GetDataHandlers.PlayerTeamEventArgs args )
        {
            TSPlayer player = TShock.Players[args.PlayerId];
			Point spawn = config.Spawns.GetSpawn(player.Group.Name, args.Team - 1);
            if( spawn.X == -1 || spawn.Y == -1 )
            {
				if (config.Spawns.forceSpawn)
                {
                    player.Spawn();
                }
            }
            else
            {
	            if (config.Spawns.forceSpawn)
	            {
		            player.Teleport(spawn.X*16, spawn.Y*16);
	            }
            }
        }
    }
}
