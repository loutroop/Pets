using Qurre.API.Addons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pets
{
    public class Config : IConfig
    {

        public bool IsEnabled { get; set; } = true;
        [Description("Which Role will the player's pet to be normally?It cannot be not a SCP")]
        public RoleType Pet_role { get; set; } = RoleType.None;
        public string Name { get; set; } = nameof(Pets);
    }
}
