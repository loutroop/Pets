using Qurre.API.Addons;
using System.ComponentModel;
namespace Pets
{
    public class Config : IConfig
    {

        public bool IsEnabled { get; set; } = true;
        [Description("Which Role will the player's pet to be normally?")]
        public RoleType Pet_role { get; set; } = RoleType.None;
        [Description("Will SCP players have pets?")]
        public bool pet_with_scp { get; set; } = false;
        [Description("what is pets'names?")]
        public string pet_displaynickname { get; set; } = "pet of {owner}";
        [Description("Are you using WaitAndChil?")]
        public bool WaitAndChilUsing { get; set; } = true;
        public string Name { get; set; } = nameof(Pets);
    }
}