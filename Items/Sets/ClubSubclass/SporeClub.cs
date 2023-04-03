using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class SporeClub : ClubItem
    {
		internal override int MinDamage => 55;
		internal override int MaxDamage => 120;
		internal override float MinKnockback => 6f;
		internal override float MaxKnockback => 10f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sporebreaker");
            Tooltip.SetDefault("Charged strikes release a field of toxins");
        }

        public override void Defaults()
        {
            Item.width = 54;
            Item.height = 62;
            Item.crit = 4;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.SporeClubProj>();
        }
	}
}