using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Projectiles.Clubs;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class BassSlapper : ClubItem
    {
		internal override int ChargeTime => 66;
		internal override Point Size => new(76, 84);
		internal override float Acceleration => 17f;
		internal override int MinDamage => 28;
		internal override int MaxDamage => 90;
		internal override float MinKnockback => 10f;
		internal override float MaxKnockback => 20f;
		

		public override void SetStaticDefaults() => DisplayName.SetDefault("Bass Slapper");

		public override void Defaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.crit = 4;
            Item.value = Item.sellPrice(0, 1, 42, 0);
            Item.rare = ItemRarityID.Orange;
			Item.shoot = ModContent.ProjectileType<BassSlapperProj>();
        }
	}
}