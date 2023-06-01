using Microsoft.Xna.Framework;
using System;
using Terraria;
using static Terraria.ModLoader.ModContent;
using SpiritMod.Dusts;

namespace SpiritMod.Projectiles.Clubs
{
	class BoneClubProj : ClubProj
	{
		public BoneClubProj() : base(new Vector2(76, 82)) { }

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Bone Club");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void Smash(Vector2 position)
		{
			for (int k = 0; k <= 100; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + (Projectile.Size / 2f), DustType<BoneDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);

            for (int i = 0; i < 6; i++)
            {
                float rotation = (float)(Main.rand.Next(180, 361) * (Math.PI / 180));
                float rotation2 = (float)(Main.rand.Next(180, 270) * (Math.PI / 180));
                Vector2 velocity = new Vector2((float)Math.Cos(rotation2), (float)Math.Sin(rotation));
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI("ClubSmash"), Projectile.Center - new Vector2(0, 32), velocity * new Vector2(-4 * Main.player[Projectile.owner].direction, 4), ProjectileType<BoneShard>(), (int)(Projectile.damage / 4f), Projectile.knockBack, Projectile.owner);
                Main.projectile[proj].scale *= Main.rand.NextFloat(.6f, 1f);
            }
        }
	}
}
