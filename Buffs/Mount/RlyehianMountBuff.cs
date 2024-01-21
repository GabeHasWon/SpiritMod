using Terraria.ModLoader;
using Terraria;

namespace SpiritMod.Buffs.Mount
{
    public class RlyehianMountBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
 
        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<Mounts.RlyehianMount.RlyehianMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}