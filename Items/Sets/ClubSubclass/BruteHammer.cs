using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Clubs.BruteHammer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
	public class BruteHammer : ClubItem
	{
		internal override int ChargeTime => 54;
		internal override Vector2 Size => new(50, 80);
		internal override float Acceleration => 17f;
		internal override int MinDamage => 26;
		internal override int MaxDamage => 90;
		internal override float MinKnockback => 4;
		internal override float MaxKnockback => 12;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brute Hammer");
			Tooltip.SetDefault("Spins rapidly around the player, dealing a devastating blow on release");
		}

		public override void Defaults()
		{
			Item.width = 40;
			Item.height = 32;
			Item.UseSound = SoundID.Item1;
			Item.value = Item.sellPrice(0, 0, 60, 0);
			Item.rare = ItemRarityID.Orange;
			Item.shoot = ModContent.ProjectileType<BruteHammerProj>();
		}
	}
}