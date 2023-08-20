using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.TideDrops.Whirltide
{
	public class WhirltideScouter : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Whirltide Spout");

		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.hide = true;
			Projectile.scale = 1f;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 120;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

		public override void AI()
		{
			int padTime = 10;
			if ((++Counter % 10) == 0 && Counter > padTime)
			{
				int style = (Counter - padTime) / 10;
				float bonus = style / 2f;

				Vector2 position = new Vector2(Projectile.position.X, Projectile.position.Y - 8);
				Vector2 velocity = new Vector2(0.1f * Math.Sign(Projectile.velocity.X), -(1.5f + bonus));

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<WhirltideSpout>(), Projectile.damage, Projectile.knockBack + bonus, Main.player[Projectile.owner].whoAmI);

				if (style > 4)
					Projectile.Kill();
			}
		}

		public override bool? CanDamage() => false;
	}
}