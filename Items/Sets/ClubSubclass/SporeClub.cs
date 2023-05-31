using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class SporeClub : ClubItem
    {
		internal override int ChargeTime => 52;
		internal override Vector2 Size => new(86, 82);
		internal override float Acceleration => 19f;
		internal override int MinDamage => 36;
		internal override int MaxDamage => 90;
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
            Item.crit = 6;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.SporeClubProj>();
        }
	}
}