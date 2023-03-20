using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class SwiftShot : ModProjectile
	{
		private const int timeLeftMax = 24;
		private Vector2 origin;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Energy Beam");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.timeLeft = timeLeftMax;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.extraUpdates = 1;
		}

		public override void OnSpawn(IEntitySource source) => origin = Projectile.Center;

		public override void Kill(int timeLeft)
		{
			if (timeLeft <= 0)
				return;

			for (int i = 0; i < 12; i++)
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(2) ? DustID.Torch : DustID.TreasureSparkle, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.7f, 1.0f));
				dust.velocity = -(Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f)).RotatedByRandom(0.8f);
				dust.noGravity = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			float quoteant = (float)Projectile.timeLeft / timeLeftMax;
			float initScaleY = 15;

			for (int i = 0; i < 3; i++)
			{
				float shotLength = origin.Distance(Projectile.Center);

				Texture2D texture = (i > 1) ? Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_1").Value : Mod.Assets.Request<Texture2D>("Textures/GlowTrail").Value;
				Vector2 scale = new Vector2(shotLength, MathHelper.Lerp(initScaleY, 5, 1f - quoteant)) / texture.Size();

				Color color = i switch
				{
					0 => Color.Red * .4f,
					1 => Color.Yellow * .8f,
					_ => Color.White * 1.5f
				};
				color = (color with { A = 0 }) * quoteant;

				Main.EntitySpriteDraw(texture, origin - Main.screenPosition, null, color, Projectile.velocity.ToRotation(), new Vector2(0, texture.Height / 2), scale, SpriteEffects.None, 0);

				Texture2D bloom = Mod.Assets.Request<Texture2D>("Effects/Masks/CircleGradient").Value;
				Vector2 endPos = origin + (Vector2.UnitX * shotLength).RotatedBy(Projectile.velocity.ToRotation());
				Main.spriteBatch.Draw(bloom, endPos - Main.screenPosition, null, color, 0, bloom.Size() / 2, (0.1f - (i * 0.03f)) * quoteant, SpriteEffects.None, 0);
			}

			return false;
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(origin);

		public override void ReceiveExtraAI(BinaryReader reader) => origin = reader.ReadVector2();
	}
}