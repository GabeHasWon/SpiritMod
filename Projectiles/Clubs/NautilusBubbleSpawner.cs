using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Clubs
{
	public class NautilusBubbleSpawner : ModProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Nautilus Bubble");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.damage = 1;
			Projectile.penetrate = -1;
            Projectile.hide = true;
			Projectile.alpha = 255;
            Projectile.timeLeft = 75;
			Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
		}

		public override bool PreAI()
        {
            Projectile.velocity = Vector2.Zero;

			if (Projectile.timeLeft % 15 == 0)
			{
				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(Main.rand.Next(-15, 15), 6), Vector2.UnitY * Main.rand.NextFloat(-2f, -1f), ModContent.ProjectileType<NautilusBubbleProj>(), Projectile.damage / 4, Projectile.owner, 0, 0f);
				proj.scale = Main.rand.NextFloat(.8f, 1f);
				proj.timeLeft = Main.rand.Next(90, 110);
			}

			return false;
		}

		public override bool? CanHitNPC(NPC target) => false;
	}
}