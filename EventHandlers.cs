using MEC;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pets
{
    public class EventHandlers
    {
        public readonly Plugin plugin;
        public EventHandlers(Plugin plugin) =>this.plugin = plugin;
        public static Dictionary<string, Data> Data = new Dictionary<string, Data>();
        public static void SpawnBot(Vector3 pos, Vector2 rot, Vector3 scale, RoleType role, string role_text, string role_color, ItemType itemType = ItemType.None, string name = "npc", float WalkSpeed = 5f, float RunSpeed = 7f)
        {
            var npc = new Qurre.API.Controllers.Bot(pos, rot, role, name, role_text, role_color)
            {
                Scale = scale,
                WalkSpeed = WalkSpeed,
                RunSpeed = RunSpeed
            };
            if (itemType != ItemType.None) npc.ItemInHand = itemType;
        }
        public void RoundStart()
        {
            var _pls = Player.List.ToArray();
            foreach (Player player in _pls)
            {
                if (!Data.ContainsKey(player.UserId)) Data.Add(player.UserId, Methods.LoadData(player.UserId));
                if (Data[player.UserId].Pet_role != RoleType.None || Data[player.UserId].Pet_role.GetTeam() != Team.SCP) SpawnBot(player.Position, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), Data[player.UserId].Pet_role, "npc_pet", "default", ItemType.None, player.UserId);
            }
            Timing.RunCoroutine(PetWalk(), "PetsWalks");
        }
        public void RoundEnd(RoundEndEvent _)
        {
            Timing.KillCoroutines("PetsWalks");
            Map.Bots.Clear(); 
        }
        public void JoinEvent(JoinEvent ev)
        {
            if (!Round.Started) return;
            Timing.CallDelayed(2f, () =>
            {
                if (!Data.ContainsKey(ev.Player.UserId)) Data.Add(ev.Player.UserId, Methods.LoadData(ev.Player.UserId));
                if (Data[ev.Player.UserId].Pet_role != RoleType.None || Data[ev.Player.UserId].Pet_role.GetTeam() != Team.SCP) SpawnBot(ev.Player.Position, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), Data[ev.Player.UserId].Pet_role, "npc_pet", "default", ItemType.None, ev.Player.UserId);
            });
          
        }
        public void DiesEvent(DiesEvent ev)
        {
            if (!ev.Target.Bot) return;
            ev.Allowed = false;
            foreach (var bot in Map.Bots.Where(x => x.Name == ev.Target.UserId))
            {
                Timing.CallDelayed(0.5f, () => ev.Target.Position = bot.Position);
            }
        }
        public IEnumerator<float> PetWalk()
        {
            while (Round.Started)
            {
                foreach (Qurre.API.Controllers.Bot bot in Map.Bots)
                {
                    foreach(Player player in Player.List.Where(x => x.UserId == bot.Name))
                    {
                        try { bot.RotateToPosition(player.Position); } catch { }
                        try
                        {
                            if (Vector3.Distance(player.Position, bot.Position) < 1) bot.Direction = MovementDirection.Stop;
                            else
                            {
                                bot.Direction = MovementDirection.Forward;
                                if (Vector3.Distance(player.Position, bot.Position) < 5) bot.Movement = PlayerMovementState.Sneaking;
                                else if (Vector3.Distance(player.Position, bot.Position) < 15) bot.Movement = PlayerMovementState.Walking;
                                else if (Vector3.Distance(player.Position, bot.Position) < 30) bot.Movement = PlayerMovementState.Sprinting;
                                else bot.Position = player.Position;
                            }
                        }
                        catch { }
                    }
                }
                yield return Timing.WaitForSeconds(0.5f);
            }
            yield break;
        }
        public void ScpAttack(ScpAttackEvent ev)
        {
            if (!ev.Target.Bot) return;
            ev.Allowed = false;
        }
        public void Damage(DamageEvent ev)
        {
            if (!ev.Target.Bot) return;
            ev.Allowed = false;
        }
        public void Enrage(EnrageEvent ev)
        {
            if (!Player.Get(ev.Scp096.Hub).Bot) return;
            ev.Allowed = false;
        }
        public void Blink(BlinkEvent ev)
        {
            if (!Player.Get(ev.Scp.ReferenceHub).Bot) return;
            ev.Allowed = false;
        }
    }
}
