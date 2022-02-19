using System;
using System.IO;

namespace Pets
{
    public class Plugin : Qurre.Plugin
    {
        public override string Developer => "Loutroop2107 && fydne";
        public override string Name => nameof(Pets);
        public override Version NeededQurreVersion => new(1, 11, 10);
        public override Version Version => new(1, 0, 5);
        public static Config CustomConfig { get; private set; }
        public EventHandlers Handlers;
        public override void Enable()
        {
            CustomConfig = new Config();
            CustomConfigs.Add(CustomConfig);
            if (!CustomConfig.IsEnabled) return;
            Handlers = new EventHandlers();
            Qurre.Events.Round.Start += Handlers.RoundStart;
            Qurre.Events.Round.End += Handlers.RoundEnd;
            Qurre.Events.Round.Restart += Handlers.RoundEnd;
            Qurre.Events.Player.Join += Handlers.JoinEvent;
            Qurre.Events.Scp096.Enrage += Handlers.Enrage;
            Qurre.Events.Scp173.Blink += Handlers.Blink;
            Qurre.Events.Player.Damage += Handlers.Damage;
            Qurre.Events.Player.ScpAttack += Handlers.ScpAttack;
            Qurre.Events.Player.TransmitPlayerData += Handlers.OnTransmitData;
            if (!Directory.Exists(Methods.StatFilePath)) Directory.CreateDirectory(Methods.StatFilePath);
        }

        public override void Disable()
        {
            Handlers = null;
            Qurre.Events.Round.Start -= Handlers.RoundStart;
            Qurre.Events.Round.End -= Handlers.RoundEnd;
            Qurre.Events.Round.Restart -= Handlers.RoundEnd;
            Qurre.Events.Player.Join -= Handlers.JoinEvent;
            Qurre.Events.Scp096.Enrage -= Handlers.Enrage;
            Qurre.Events.Scp173.Blink -= Handlers.Blink;
            Qurre.Events.Player.Damage -= Handlers.Damage;
            Qurre.Events.Player.ScpAttack -= Handlers.ScpAttack;
            Qurre.Events.Player.TransmitPlayerData -= Handlers.OnTransmitData;
        }
    }
}