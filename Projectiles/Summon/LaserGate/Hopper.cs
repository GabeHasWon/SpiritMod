using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon.LaserGate
{
	public class Hopper : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private bool Paired => pairWhoAmI > -1;
		public int pairWhoAmI = -1;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Right Gate");

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.hostile = false;
			Projectile.friendly = false;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 8000;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			Projectile.rotation += Projectile.velocity.Length() / 20f;

			if (!Paired) //Find a pair
			{
				float maxDist = 500;
				float drawDist = 1500;

				foreach (Projectile proj in Main.projectile)
				{
					if (proj.whoAmI == Projectile.whoAmI || !proj.active || proj.owner != Projectile.owner || proj.type != Type)
						continue;

					if (proj.Distance(Projectile.Center) <= maxDist)
					{
						pairWhoAmI = proj.whoAmI;
						(proj.ModProjectile as Hopper).pairWhoAmI = Projectile.whoAmI;

						if (Main.netMode != NetmodeID.Server)
							SoundEngine.PlaySound(SoundID.Item93, Projectile.position);

						Projectile.netUpdate = true;
					}
					else if (proj.Distance(Projectile.Center) <= drawDist)
					{
						//Move closer together
						proj.velocity = Vector2.Lerp(Projectile.velocity, proj.DirectionTo(Projectile.Center) * 5f, 0.1f);
						Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(proj.Center) * 5f, 0.1f);
					}
				}
				return;
			}

			Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.05f);

			Projectile other = Main.projectile[pairWhoAmI];

			//rotating
			Vector2 direction9 = other.Center - Projectile.Center;
			int distance = (int)Math.Sqrt((direction9.X * direction9.X) + (direction9.Y * direction9.Y));
			direction9.Normalize();
			//shoot to other guy

			Counter++;
			if (Counter % 5 == 0)
			{
				int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, direction9.X * 15, direction9.Y * 15, ModContent.ProjectileType<GateLaser>(), 14, 0, Main.myPlayer);
				Main.projectile[proj].timeLeft = (distance / 15) - 1;
				DustHelper.DrawElectricity(Projectile.Center, other.Center, 226, 0.3f);
			}

			if (Counter % 30 == 0)
			{
				SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
				Counter = 0;
			}

			if (!other.active)
				ClearPairs();
		}

		private void ClearPairs()
		{
			if (!Paired)
				return;

			(Main.projectile[pairWhoAmI].ModProjectile as Hopper).pairWhoAmI = -1;
			pairWhoAmI = -1;
		}

		public override void Kill(int timeLeft) => ClearPairs();

		public override void PostDraw(Color lightColor)
		{
			if (Paired)
			{
				Texture2D glowMask = TextureAssets.GlowMask[239].Value;
				float num11 = (float)(Main.GlobalTimeWrappedHourly % 1.0 / 1.0);
				float num12 = num11;
				if (num12 > 0.5)
					num12 = 1f - num11;
				if (num12 < 0.0)
					num12 = 0.0f;
				float num13 = (float)((num11 + 0.5) % 1.0);
				float num14 = num13;
				if (num14 > 0.5)
					num14 = 1f - num13;
				if (num14 < 0.0)
					num14 = 0.0f;
				Rectangle r2 = glowMask.Frame(1, 1, 0, 0);
				Vector2 drawOrigin = r2.Size() / 2f;
				Vector2 position3 = (Projectile.Bottom - Main.screenPosition) + new Vector2(3f, -6f);
				Color color3 = new Color(84, 207, 255) * 1.6f;
				Main.spriteBatch.Draw(glowMask, position3, r2, color3, Projectile.rotation, drawOrigin, Projectile.scale * 0.33f, SpriteEffects.FlipHorizontally, 0.0f);
				float num15 = 1f + num11 * 0.75f;
				Main.spriteBatch.Draw(glowMask, position3, r2, color3 * num12, Projectile.rotation, drawOrigin, Projectile.scale * 0.33f * num15, SpriteEffects.FlipHorizontally, 0.0f);
				float num16 = 1f + num13 * 0.75f;
				Main.spriteBatch.Draw(glowMask, position3, r2, color3 * num14, Projectile.rotation, drawOrigin, Projectile.scale * 0.33f * num16, SpriteEffects.FlipHorizontally, 0.0f);
			}
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(pairWhoAmI);

		public override void ReceiveExtraAI(BinaryReader reader) => pairWhoAmI = reader.Read();
	}
}
