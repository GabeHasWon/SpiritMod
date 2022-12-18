using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Sets.BowsMisc.GemBows.Emerald_Bow
{
	public class Emerald_Arrow : GemArrow
	{
		public Emerald_Arrow() : base(Color.Green, DustID.GemEmerald) { }
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Emerald Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
	}
}
