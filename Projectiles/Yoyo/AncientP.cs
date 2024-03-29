using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Yoyo
{
	public class AncientP : ModProjectile
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.Ancient.DisplayName");

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.TheEyeOfCthulhu);
			Projectile.damage = 104;
			Projectile.extraUpdates = 1;
			AIType = ProjectileID.TheEyeOfCthulhu;
		}

		public override void AI()
		{
			if (Main.myPlayer == Projectile.owner && ++Projectile.frameCounter % 130 == 0)
			{
				float rotation = (float)(Main.rand.Next(0, 361) * (Math.PI / 180));
				Vector2 velocity = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * 7;
				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ProjectileID.CultistBossIceMist, Projectile.damage, Projectile.owner, 0, 0f);
				proj.friendly = true;
				proj.hostile = false;
				proj.netUpdate = true;
			}
		}
	}
}