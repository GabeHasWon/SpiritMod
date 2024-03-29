using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Snaptrapper
{
	public class SnaptrapperSpore : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Snaptrapper Spore");
		}

		public override void SetDefaults()
		{
			Projectile.hostile = true;
			Projectile.height = 30;
			Projectile.width = 26;
			Projectile.friendly = false;
			Projectile.aiStyle = 2;
			Projectile.penetrate = 4;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
            SoundEngine.PlaySound(SoundID.NPCHit7, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item2, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item95, Projectile.Center);
            return true;
		}

		public override void AI()
		{
			Projectile.rotation += 0.3f;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (Main.rand.NextBool(6))
				target.AddBuff(BuffID.Poisoned, 180);
		}
        public override void OnKill(int timeLeft)
		{
			for (int num621 = 0; num621 < 8; num621++) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, 0f, 100, default, .7f);
                Dust dust = Main.dust[Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height, ModContent.DustType<Dusts.PoisonGas>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 5f)];
                dust.noGravity = true;
                dust.velocity.X *= 0.3f;
                dust.velocity.Y = (dust.velocity.Y * 0.2f) - 1;
            }
            int amountOfProjectiles = Main.rand.Next(2, 5);
            for (int i = 0; i < amountOfProjectiles; ++i)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-4, -1), ModContent.ProjectileType<SnaptrapperGas>(), 12, 1, Main.myPlayer, 0, 0);
                Main.projectile[p].hostile = true;
                Main.projectile[p].friendly = false;
            }
        }

	}
}