using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using SpiritMod.Items.Sets.BowsMisc.GemBows;

namespace SpiritMod.Items.Sets.BowsMisc.OrnamentBow
{
	public class Ornament_Arrow : GemArrow
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ornament Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		protected override void SafeSetDefaults() => glowColor = Main.DiscoColor;

		public override void AI()
		{
			for (int i = 0; i < 2; i++)
			{
				int num = Projectile.frameCounter;
				Projectile.frameCounter = num + 1;
				Projectile.localAI[0] += 1f;
				for (int num41 = 0; num41 < 4; num41 = num + 1)
				{
					Vector2 value8 = -Vector2.UnitY.RotatedBy(Projectile.localAI[0] * 0.1308997f + num41 * MathHelper.Pi) * new Vector2(2f, 10f) - Projectile.rotation.ToRotationVector2();
					int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowTorch, 0f, 0f, 160, glowColor, 1f);
					Main.dust[dust].scale = 0.7f;
					Main.dust[dust].noGravity = true;
					Main.dust[dust].position = Projectile.Center + value8 + Projectile.velocity * 2f;
					Main.dust[dust].velocity = Vector2.Normalize(Projectile.Center + Projectile.velocity * 2f - Main.dust[dust].position) / 8f + Projectile.velocity;
					num = num41;
				}
			}
		}

		private void SpawnArrows(IEntitySource src)
		{
			for (int index = 0; index < 10; ++index) {
				int i = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.RainbowTorch, 0.0f, 0.0f, 0, glowColor, 1f);
				Main.dust[i].noGravity = true;
			}
			SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.4f });

			Player player = Main.player[Projectile.owner];
			int extraArrows = Main.rand.Next(5);

			for (int i = 0; i < 1 + extraArrows; i++) {
				int arrowType = Main.rand.Next(6);
				int dustType = DustID.Stone;
				switch (arrowType) {
					case 0:
						arrowType = ModContent.ProjectileType<GemBows.Amethyst_Bow.Amethyst_Arrow>();
						dustType = DustID.GemAmethyst;
						break;
					case 1:
						arrowType = ModContent.ProjectileType<GemBows.Topaz_Bow.Topaz_Arrow>();
						dustType = DustID.GemTopaz;
						break;
					case 2:
						arrowType = ModContent.ProjectileType<GemBows.Sapphire_Bow.Sapphire_Arrow>();
						dustType = DustID.GemSapphire;
						break;
					case 3:
						arrowType = ModContent.ProjectileType<GemBows.Emerald_Bow.Emerald_Arrow>();
						dustType = DustID.GemEmerald;
						break;
					case 4:
						arrowType = ModContent.ProjectileType<GemBows.Ruby_Bow.Ruby_Arrow>();
						dustType = DustID.GemRuby;
						break;
					case 5:
						arrowType = ModContent.ProjectileType<GemBows.Diamond_Bow.Diamond_Arrow>();
						dustType = DustID.GemDiamond;
						break;
					default:
						break;
				}
				float x = Main.rand.Next(-80, 80);
				float y = Main.rand.Next(-60, -20);
				int a = Projectile.NewProjectile(src, player.Center.X + x, player.Center.Y + y, 0f, 0f, arrowType, (int)(player.GetDamage(DamageClass.Ranged).ApplyTo(player.HeldItem.damage * 0.7f)), 2f, player.whoAmI);
				Vector2 vector2_2 = Vector2.Normalize(new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.projectile[a].Center) * Main.rand.Next(14, 18);
				Main.projectile[a].velocity = vector2_2;
				for (int j = 0; j < 16f; ++j) {
					Vector2 v = (Vector2.UnitX * 0.1f + -Vector2.UnitY.RotatedBy(j * (MathHelper.TwoPi / 16f), new Vector2()) * new Vector2(4f)).RotatedBy(Projectile.velocity.ToRotation());
					int dust = Dust.NewDust(new Vector2(player.Center.X + x, player.Center.Y + y), 8, 8, dustType, 0.0f, 0.0f, 0, default, 1f);
					Main.dust[dust].scale = 0.9f;
					Main.dust[dust].alpha = 200;
					Main.dust[dust].noGravity = true;
					Main.dust[dust].noLight = false;
					Main.dust[dust].position = new Vector2(player.Center.X + x, player.Center.Y + y) + v;
					Main.dust[dust].velocity = new Vector2(Projectile.velocity.X, Projectile.velocity.Y) * 0.0f + v.SafeNormalize(Vector2.UnitY) * 1f;
				}
			}
		}

		public override void Kill(int timeLeft) => SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.4f });

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => SpawnArrows(Projectile.GetSource_OnHit(target));

		public override void OnHitPvp(Player target, int damage, bool crit) => SpawnArrows(Projectile.GetSource_OnHit(target));
	}
}
