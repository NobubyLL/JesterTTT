using Editor;
using Sandbox.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TerrorTown;
using static System.Net.Mime.MediaTypeNames;

namespace Sandbox
{
    public partial class Jester : BaseTeam
    {
        public override TeamAlignment TeamAlignment => TeamAlignment.Traitor;
        public override string TeamName => "Jester";
        public override Color TeamColour => Color.Magenta;
        public override TeamMemberVisibility TeamMemberVisibility => TeamMemberVisibility.Alignment;
        public override string VictimKillMessage => "You were killed by {0}. They were a Jester, but they shouldn't be able to do damage so you shouldn't see this.";

        public override string RoleDescription => @"You are a Jester! You do no damage and need someone to shoot you!

If anyone kills you, ONLY you win! If no one does, you lose!";

        public override string IdentifyString => "{0} found the body of {1}. They were a Jester. GG";

        public override string OverheadIcon => "ui/jester.png";

        [ConVar.Server(
    "nobodyll_players_per_jester",
    Help = "Assign one Jester per specified player count. For example, setting this to 7 will mean that there will be one Jester per 7 players. The default is 10.",
    Saved = true
)]
        public static int PlayersPerJester { get; set; } = 10;

        public override float TeamPlayerPercentage => 1f / PlayersPerJester;
        public override int TeamPlayerMaximum => 1;

        private static Dictionary<TerrorTown.Player, WorldIndicatorPanel> Jesters = new();

        [Event("Game.Round.Start")]
        public static void OnRoundStart()
        {
            if (Teams.Get<Jester>().Players.Count == 1) return;
            {
                TerrorTown.PopupSystem.DisplayPopup(To.Everyone, $"There is a jester loose! Do not let them trick you into killing them!", Color.Magenta);
            }
        }

            [Event("Player.PreTakeDamage")]
        public static void PreTakeDamage(DamageInfo info, TerrorTown.Player ply)
        {
            if (info.Attacker is TerrorTown.Player attackply && attackply.Team is Jester) ply.PendingDamage.Damage = 0;
        }

        public override bool ShouldWin()
        {
            var jesterTeam = Teams.Get<Jester>();
            if (jesterTeam.Players.Where((TerrorTown.Player i) => i.IsValid && i.LifeState == LifeState.Dead && i.LastAttacker != null).Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
