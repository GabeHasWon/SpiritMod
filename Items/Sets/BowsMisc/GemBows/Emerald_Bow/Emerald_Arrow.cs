using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Sets.BowsMisc.GemBows.Emerald_Bow
{
	public class Emerald_Arrow : GemArrow
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Emerald Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		protected override void SafeSetDefaults()
		{
			dustType = DustID.GemEmerald;
			glowColor = Color.Green;
		}
	}
}
