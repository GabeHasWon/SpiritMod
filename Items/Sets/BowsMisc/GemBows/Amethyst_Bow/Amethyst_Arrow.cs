using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Sets.BowsMisc.GemBows.Amethyst_Bow
{
	public class Amethyst_Arrow : GemArrow
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Amethyst Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		protected override void SafeSetDefaults()
		{
			dustType = DustID.GemAmethyst;
			glowColor = Color.Purple;
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (Main.rand.NextBool(10)) //10% extra chance for a crit
				crit = true;
		}
	}
}
