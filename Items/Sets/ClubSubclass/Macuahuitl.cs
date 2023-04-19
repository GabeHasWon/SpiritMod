using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Projectiles.Clubs;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class Macuahuitl : ClubItem
    {
		internal override int MinDamage => 65;
		internal override int MaxDamage => 250;
		internal override float MinKnockback => 4f;
		internal override float MaxKnockback => 12f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Macuahuitl");
            Tooltip.SetDefault("Charging increases armor penetration");
        }

        public override void Defaults()
        {
			Item.damage = 115;
            Item.width = 66;
            Item.height = 66;
            Item.crit = 6;
			Item.value = Item.buyPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<MacuahuitlProj>();
        }
	}
}