using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Projectiles.Clubs;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class BassSlapper : ClubItem
    {
		internal override int MinDamage => 33;
		internal override int MaxDamage => 65;
		internal override float MinKnockback => 13f;
		internal override float MaxKnockback => 27f;
		

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