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
            npc.Player.DisplayNickname = Plugin.CustomConfig.pet_displaynickname.Replace("{owner}", Player.Get(npc.Name).Nickname);
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
            foreach (var bot in Map.Bots)
                bot.Destroy();
        }
        public void RoundEnd()
        {
            Timing.KillCoroutines("PetsWalks");
            Timing.KillCoroutines("CreatingPetsRoundStart");
        }
        public void JoinEvent(JoinEvent ev)
        {
            if (!Round.Started) return;
            Timing.CallDelayed(1f, () =>
            {
                if (!Data.TryGetValue(ev.Player.UserId, out var data))
                {
                    data = Methods.LoadData(ev.Player.UserId);
                    Data.Add(ev.Player.UserId, data);
                }
            });
        }
        public void OnTransmitData(TransmitPlayerDataEvent ev)
        {
            if (ev.Player.Bot) return;
            if (!ev.PlayerToShow.Bot) return;

            Player pl = Player.Get(ev.PlayerToShow.Nickname);
            if (pl is null) return;

            if (pl.Team == Team.CDP || pl.Team == Team.CHI)
            {
                if (ev.Player.Team == Team.SCP || ev.Player.Team == Team.RSC || ev.Player.Team == Team.MTF) ev.Invisible = true;
            }

            else if (pl.Team == Team.RSC || pl.Team == Team.MTF)
            {
                if (ev.Player.Team == Team.SCP || ev.Player.Team == Team.CHI || ev.Player.Team == Team.CDP) ev.Invisible = true;
            }

            else if (pl.Team == Team.SCP && ev.Player.Team != Team.SCP) ev.Invisible = true;
        }
        public IEnumerator<float> PetWalk()
        {
            while (Round.Started)
            {
                foreach (var bot in Map.Bots)
                {
                    foreach (var pl in Player.List.Where(x => x.UserId == bot.Name))
                    {
                        var distance = Vector3.Distance(pl.Position, bot.Position);
                        try { if (distance > 1) bot.RotateToPosition(pl.Position); } catch { }
                        try
                        {
                            if (distance < 1) bot.Direction = MovementDirection.Stop;
                            else
                            {
                                bot.Direction = MovementDirection.Forward;
                                if (distance < 10)
                                {
                                    bot.Movement = PlayerMovementState.Sprinting;
                                    bot.RunSpeed = distance * 2;
                                }
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