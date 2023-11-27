using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using SpiritMod.Dusts;
using Terraria.Localization;

namespace SpiritMod.Projectiles.Clubs
{
	class BlasphemerProj : ClubProj
	{
		public BlasphemerProj() : base(new Vector2(104)) { }

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.Blasphemer.DisplayName");

		public override void SafeSetStaticDefaults()
		{
			// DisplayName.SetDefault("Blasphemer");
			Main.projFrames[Projectile.type] = 3;
        }

		public override void Smash(Vector2 position)
		{
			Player player = Main.player[Projectile.owner];
			for (int k = 0; k <= 100; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<BoneDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);

            for (int k = 0; k <= 100; k++)
                Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<FireClubDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);

			int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 16, 0, -12, ProjectileType<Magic.Firespike>(), Projectile.damage / 3, Projectile.knockBack / 2, Projectile.owner, 0, player.direction);
            Main.projectile[a].DamageType = DamageClass.Melee;
            SoundEngine.PlaySound(SoundID.NPCHit20, Projectile.position);
        }

        public override void SafeDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (Projectile.ai[0] >= ChargeTime)
			{
				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
				Rectangle drawFrame = texture.Frame(1, Main.projFrames[Type], 0, 2, 0, 0);

				spriteBatch.Draw(texture, Main.player[Projectile.owner].Center - Main.screenPosition, drawFrame, Color.White * 0.9f, TrueRotation, Origin, Projectile.scale, Effects, 0);
            }
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 180);
        }
	}
}
