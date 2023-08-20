using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.TideDrops.Whirltide
{
	public class WhirltideSpout : ModProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Whirltide Spout");

		public override void SetDefaults()
		{
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.hide = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.scale = 1f;
			Projectile.extraUpdates = 1;
			Projectile.timeLeft = 60;
			AIType = ProjectileID.WoodenArrowFriendly;
			Projectile.aiStyle = 1;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

		public override void OnSpawn(IEntitySource source)
		{
			for (int i = 0; i < 30; i++) //Scan only 30 tiles down
			{
				Point position = new Point((int)(Projectile.Center.X / 16), (int)((Projectile.Center.Y / 16) + i));
				if (WorldGen.SolidOrSlopedTile(Framing.GetTileSafely(position)))
				{
					Projectile.Center = new Vector2(Projectile.Center.X, (position.Y * 16) + 8); //Don't modify X
					break;
				}
			}
		}

		public override void AI()
		{
			for (int index = 0; index < 12; ++index)
			{
				if (!Main.rand.NextBool(5))
				{
					int Type = Utils.SelectRandom(Main.rand, new int[2] { 4, 157 });

					Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Type, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(), 1f)];
					dust.velocity = dust.velocity / 4f + Projectile.velocity / 2f;
					dust.scale = (float)(0.800000011920929 + (double)Main.rand.NextFloat() * 0.400000005960464);
					dust.position = Projectile.Center;
					dust.position += new Vector2((float)(Projectile.width / 3), 0.0f).RotatedBy(6.28318548202515 * (double)Main.rand.NextFloat(), new Vector2()) * Main.rand.NextFloat();
					dust.noLight = true;
					dust.noGravity = true;
					if (dust.type == 4)
					{
						dust.color = new Color(80, 170, 40, 120);
					}
				}
			}
			float num2 = 60f;

			++Projectile.ai[1];

			float num3 = Projectile.ai[1] / num2;
			Vector2 spinningpoint = new Vector2(0.0f, -30f);
			spinningpoint = spinningpoint.RotatedBy((double)num3 * 1.5 * 6.28318548202515, new Vector2()) * new Vector2(1f, 0.4f);
			for (int index1 = 0; index1 < 4; ++index1)
			{
				Vector2 vector2 = Vector2.Zero;
				float num4 = 1f;
				if (index1 == 0)
				{
					vector2 = Vector2.UnitY * -15f;
					num4 = 0.6f;
				}
				if (index1 == 1)
				{
					vector2 = Vector2.UnitY * -5f;
					num4 = 0f;
				}
				if (index1 == 2)
				{
					vector2 = Vector2.UnitY * 5f;
					num4 = 0f;
				}
				if (index1 == 3)
				{
					vector2 = Vector2.UnitY * 20f;
					num4 = 0.6f;
				}
				int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.ChlorophyteWeapon, 0.0f, 0.0f, 100, new Color(), 1f);
				Main.dust[index2].noGravity = true;
				Main.dust[index2].position = Projectile.Center + spinningpoint * num4 + vector2;
				Main.dust[index2].velocity = Vector2.Zero;
				spinningpoint *= -1f;
				int index3 = Dust.NewDust(Projectile.Center, 0, 0, DustID.ChlorophyteWeapon, 0.0f, 0.0f, 100, new Color(), 1f);
				Main.dust[index3].noGravity = true;
				Main.dust[index3].position = Projectile.Center + spinningpoint * num4 + vector2;
				Main.dust[index3].velocity = Vector2.Zero;
			}
		}
	}
}