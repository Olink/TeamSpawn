﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace TeamSpawn
{
    [APIVersion(1, 11)]
    public class TeamSpawn : TerrariaPlugin
    {
        private Spawns spawns;
        private string savepath = Path.Combine(TShock.SavePath, "TeamSpawn.cfg");
        private Dictionary<string, int> teamsColors = new Dictionary<string, int>()
                                                          {
                                                              {"red", 0}, 
                                                              {"green", 1}, 
                                                              {"blue", 2}, 
                                                              {"yellow", 3}
                                                          };
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
            get { return "Allows you to set spawn for the different teams."; }
        }

        public TeamSpawn(Main game) : base(game)
        {
            if (File.Exists(savepath))
            {
                try
                {
                    spawns = Config.ReadFile(savepath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                spawns = Config.WriteFile(savepath);
            }
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("TeamSpawn", HandleCommand, "teamspawn"));
            TShockAPI.GetDataHandlers.PlayerTeam += ChangeTeam;
        }

        protected void Dispose(bool disposing)
        {
            
            base.Dispose(disposing);
        }

        private void HandleCommand(CommandArgs args)
        {
            TSPlayer ply = args.Player;
            if (ply == null)
                return;

            if( args.Parameters.Count < 2 )
            {
                args.Player.SendMessage("Usage: /teamspawn [set|reset] [0-3]", Color.Red);
                args.Player.SendMessage("       /teamspawn [force] [true|false]", Color.Red);
                return;
            }

            if( args.Parameters[0] == "set" )
            {
                int id = -1;
                if( !int.TryParse(args.Parameters[1], out id) )
                {
                    if( teamsColors.ContainsKey(args.Parameters[1].ToLower() ) )
                    {
                        id = teamsColors[args.Parameters[1].ToLower()];
                    }
                }

                if( id >= 0 && id <= 3)
                {
                    Point p = new Point( args.Player.TileX, args.Player.TileY );
                    spawns.SetSpawn( p, id );
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
                    if (teamsColors.ContainsKey(args.Parameters[1].ToLower()))
                    {
                        id = teamsColors[args.Parameters[1].ToLower()];
                    }
                }

                if (id >= 0 && id <= 3)
                {
                    Point p = new Point(-1, -1);
                    spawns.SetSpawn(p, id);
                    args.Player.SendMessage(String.Format("Team {0}'s spawn has been set to {1}, {2}", id, p.X, p.Y), Color.Green);
                }
                else
                {
                    args.Player.SendMessage("Error: Valid team ids are 0, 1, 2, and 3", Color.Red);
                    return;
                }
            }
            else if (args.Parameters[0] == "force")
            {
                bool force = spawns.forceSpawn;
                if( !bool.TryParse(args.Parameters[1], out force))
                {
                    args.Player.SendMessage("Error: Valid force options are 'true' and 'false'", Color.Red);
                    return;
                }
                spawns.forceSpawn = force;
                args.Player.SendMessage(String.Format("Team spawn force has been set to {0}", force), Color.Green);
                
            }
            else
            {
                args.Player.SendMessage("Usage: /teamspawn [set|reset] [0-3]", Color.Red);
                args.Player.SendMessage("       /teamspawn [force] [true|false]", Color.Red);
                return;
            }

            Config.Save(savepath, spawns);
        }

        private void ChangeTeam( object sender, GetDataHandlers.PlayerTeamEventArgs args )
        {
            TSPlayer player = TShock.Players[args.PlayerId];
            if( args.Team == 0 )
            {
                if( spawns.forceSpawn )
                {
                    player.Spawn();
                }
            }
            else
            {
                Point spawn = spawns.GetSpawn(args.Team - 1);
                if( spawn.X == -1 || spawn.Y == -1 )
                {
                    if (spawns.forceSpawn)
                    {
                        player.Spawn();
                    }
                }
                else
                {
                    player.Teleport(spawn.X, spawn.Y);
                }
            }
        }
    }
}