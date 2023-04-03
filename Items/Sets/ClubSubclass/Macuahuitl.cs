using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Projectiles.Clubs;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class Macuahuitl : ClubItem
    {
		internal override int MinDamage => 113;
		internal override int MaxDamage => 288;
		internal override float MinKnockback => 5f;
		internal override float MaxKnockback => 10f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Macuahuitl");
            Tooltip.SetDefault("Greatly increases armor penetration based on charge time");
        }

        public override void Defaults()
        {
			Item.damage = 115;
            Item.width = 66;
            Item.height = 66;
            Item.crit = 8;
			Item.value = Item.buyPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<MacuahuitlProj>();
        }
	}
}