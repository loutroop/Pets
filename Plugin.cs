using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pets
{
    public class Plugin : Qurre.Plugin
    {
        public override string Developer => "Loutroop2107";
        public override string Name => nameof(Pets);
        public override Version NeededQurreVersion => new Version(1, 11, 10);
        public override Version Version => new Version(1, 0, 0);
        public Config CustomConfig { get; private set; }
        public EventHandlers Handlers;
        public override void Enable()
        {
            CustomConfig = new Config();
            CustomConfigs.Add(CustomConfig); 
            if (!CustomConfig.IsEnabled) return;
            Handlers = new EventHandlers(this);
            Qurre.Events.Round.Start += Handlers.RoundStart;
            Qurre.Events.Round.End += Handlers.RoundEnd;
            Qurre.Events.Player.Join += Handlers.JoinEvent;
            Qurre.Events.Player.Dies += Handlers.DiesEvent;
            Qurre.Events.Scp096.Enrage += Handlers.Enrage;
            Qurre.Events.Scp173.Blink += Handlers.Blink;
            Qurre.Events.Player.ScpAttack += Handlers.ScpAttack;
            Qurre.Events.Player.Damage += Handlers.Damage;
            Qurre.Events.Player.RoleChange += Handlers.RoleChange;
        }

        public override void Disable()
        {
            Handlers = null;
            Qurre.Events.Round.Start -= Handlers.RoundStart;
            Qurre.Events.Round.End -= Handlers.RoundEnd;
            Qurre.Events.Player.Join -= Handlers.JoinEvent;
            Qurre.Events.Player.Dies -= Handlers.DiesEvent;
            Qurre.Events.Scp096.Enrage -= Handlers.Enrage;
            Qurre.Events.Scp173.Blink -= Handlers.Blink;
            Qurre.Events.Player.ScpAttack -= Handlers.ScpAttack;
            Qurre.Events.Player.Damage -= Handlers.Damage;
            Qurre.Events.Player.RoleChange -= Handlers.RoleChange;
        }
    }
}
