using MEC;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Events;
using Qurre.API.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Pets
{
    public class EventHandlers
    {
        public readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;
        public static Dictionary<string, Data> Data = new();
        public static void SpawnBot(Vector3 pos, Vector2 rot, Vector3 scale, RoleType role, string role_text, string role_color,
            ItemType itemType = ItemType.None, string name = "npc", float WalkSpeed = 5f, float RunSpeed = 7f)
        {
            var npc = new Bot(pos, rot, role, name, role_text, role_color)
            {
                Scale = scale,
                WalkSpeed = WalkSpeed,
                RunSpeed = RunSpeed
            };
            if (itemType != ItemType.None) npc.ItemInHand = itemType;
        }
        public void RoundStart()
        {
            Timing.RunCoroutine(CorUpdate(), "CreatingPetsRoundStart");
            IEnumerator<float> CorUpdate()
            {
                var _pls = Player.List.ToArray();
                foreach (Player pl in _pls)
                {
                    if (!Data.TryGetValue(pl.UserId, out var data))
                    {
                        data = Methods.LoadData(pl.UserId);
                        Data.Add(pl.UserId, data);
                    }
                    if (data.Pet_role != RoleType.None || data.Pet_role.GetTeam() != Team.SCP) SpawnBot(pl.Position, Vector3.zero,
                        new(0.5f, 0.5f, 0.5f), data.Pet_role, "npc_pet", "default", ItemType.None, pl.UserId);
                    yield return Timing.WaitForSeconds(0.1f);
                }
                Timing.RunCoroutine(PetWalk(), "PetsWalks");
                yield break;
            }
        }
        public void RoundEnd(RoundEndEvent _)
        {
            Timing.KillCoroutines("PetsWalks");
            Timing.KillCoroutines("CreatingPetsRoundStart");
        }
        public void RoundEnd()
        {
            Timing.KillCoroutines("PetsWalks");
            Timing.KillCoroutines("CreatingPetsRoundStart");
        }
        public void JoinEvent(JoinEvent ev)
        {
            if (!Round.Started) return;
            Timing.CallDelayed(2f, () =>
            {
                if (!Data.TryGetValue(ev.Player.UserId, out var data))
                {
                    data = Methods.LoadData(ev.Player.UserId);
                    Data.Add(ev.Player.UserId, data);
                }
                
            });
        }
        public void RoleChange(RoleChangeEvent ev)
        {
            if (ev.Player.Role != RoleType.Spectator && ev.NewRole == RoleType.Spectator) return;
            if (!plugin.CustomConfig.pet_with_scp && ev.NewRole.GetTeam() == Team.SCP) return;
            Timing.CallDelayed(05f, () =>
            {
                if (Methods.LoadData(ev.Player.UserId).Pet_role != RoleType.None) SpawnBot(ev.Player.Position, Vector3.zero,
                    new(0.5f, 0.5f, 0.5f), Methods.LoadData(ev.Player.UserId).Pet_role, "npc_pet", "default", ItemType.None, ev.Player.UserId);
            });
        }
        public void DiesEvent(DiesEvent ev)
        {
            if (ev.Target.Bot) return;
            ev.Allowed = false;
            foreach (var bot in Map.Bots.Where(x => x.Name == ev.Target.UserId))
            {
                Timing.CallDelayed(0.5f, () => ev.Target.Position = bot.Position + Vector3.up);
            }
        }
        public IEnumerator<float> PetWalk()
        {
            while (Round.Started)
            {
                foreach (var bot in Map.Bots)
                {
                    foreach (var pl in Player.List.Where(x => x.UserId == bot.Name))
                    {
                        try { if (Vector3.Distance(pl.Position, bot.Position) > 1) bot.RotateToPosition(pl.Position); } catch { }
                        try
                        {
                            if (Vector3.Distance(pl.Position, bot.Position) < 1) bot.Direction = MovementDirection.Stop;
                            else
                            {
                                bot.Direction = MovementDirection.Forward;
                                if (Vector3.Distance(pl.Position, bot.Position) < 3) bot.Movement = PlayerMovementState.Sneaking;
                                else if (Vector3.Distance(pl.Position, bot.Position) < 6) bot.Movement = PlayerMovementState.Walking;
                                else if (Vector3.Distance(pl.Position, bot.Position) < 10) bot.Movement = PlayerMovementState.Sprinting;
                                else bot.Position = pl.Position;
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