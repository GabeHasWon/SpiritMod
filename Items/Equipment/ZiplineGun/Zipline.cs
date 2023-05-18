using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Equipment.ZiplineGun
{
	public class Zipline : ModProjectile
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Zipline");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.Size = new Vector2(16);
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.alpha = 255;
			Projectile.extraUpdates = 4;
		}

		public bool Right
		{
			get => Projectile.ai[0] > 0;
			set => Projectile.ai[0] = value ? 1 : 0;
		}
		public int PartnerIndex
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private const int prLength = 4;
		private readonly float[] playerProgress = new float[prLength];
		public float vFadeout;

		private bool Deployed => Projectile.velocity == Vector2.Zero;

		public bool isHovering;

		public override void OnSpawn(IEntitySource source) => PartnerIndex = -1;

		public override void AI()
		{
			if (!Deployed)
				return;

			Projectile.timeLeft = 2;

			if (Projectile.owner == Main.myPlayer) //Hover-over projectile behaviour
			{
				Vector2 zoom = Main.GameViewMatrix.Zoom;

				Rectangle projBox = Projectile.getRect();
				projBox.Inflate((int)zoom.X, (int)zoom.Y);

				Player player = Main.player[Projectile.owner];

				bool oldIsHovering = isHovering;
				isHovering = projBox.Contains(Main.MouseWorld.ToPoint()) && player.HeldItem.type == ModContent.ItemType<ZiplineGun>();

				if (oldIsHovering != isHovering)
					Projectile.netUpdate = true;
			}

			if (PartnerIndex != -1)
			{
				Projectile pair = Main.projectile[PartnerIndex];

				if (pair.ModProjectile is Zipline zipline)
				{
					if (pair.Center.X > Projectile.Center.X) //Pick the leftmost Zipline of the pair
					{
						Player player = Main.LocalPlayer;

						Vector2 direction = pair.Center - Projectile.Center;
						Vector2 newDims = new Vector2(18);
						Rectangle playerRect = new Rectangle((int)(player.Center.X - (newDims.X / 2)), (int)(player.position.Y + (player.height - newDims.Y)), (int)newDims.X, (int)newDims.Y);

						int loopLength = (int)(direction.Length() / 10);
						for (int i = 0; i < loopLength; i++)
						{
							Vector2 snapshot = Vector2.Lerp(Projectile.Center, Projectile.Center + direction, (float)i / loopLength);
							if (playerRect.Contains(snapshot.ToPoint())) //A valid player is found
							{
								player.GetModPlayer<ZiplinePlayer>().directionUnit = Vector2.Normalize(direction);

								float progress = (float)i / loopLength;
								if (progress > .5) //The second half of the chain is drawn by the partner projectile
								{
									zipline.HandleChainVisuals(1f - (float)progress);

									zipline.vFadeout = 1f;

									if (vFadeout > .5f)
										vFadeout = .5f;
								}
								else
								{
									HandleChainVisuals(progress);

									vFadeout = 1f;

									if (zipline.vFadeout > .5f)
										zipline.vFadeout = .5f;
								}

								return;
							}
						}
					}

					if (!pair.active || !zipline.Deployed)
						PartnerIndex = -1;
				}
			}

			Projectile.localAI[1] = Math.Min(1, Projectile.localAI[1] + 0.1f);
			vFadeout = Math.Max(0, vFadeout - 0.05f);
		}

		public void HandleChainVisuals(float progress)
		{
			for (int i = playerProgress.Length - 1; i > 0; i--)
				playerProgress[i] = playerProgress[i - 1];

			playerProgress[0] = progress;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (!Deployed)
			{
				if (TryPair())
				{
					SoundEngine.PlaySound(SoundID.Item101, Projectile.Center);
					Projectile.localAI[1] = 0;
				}

				Projectile.alpha = 0;
				Projectile.extraUpdates = 0;
				Projectile.position += Vector2.Normalize(Projectile.velocity) * 18f;
				Projectile.velocity = Vector2.Zero;

				if (!Main.dedServ)
					ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, Right ? Color.Orange : Color.Cyan, 50, 12));
				for (int i = 0; i < 12; i++)
					Dust.NewDustPerfect(Projectile.Center, Right ? DustID.FireworkFountain_Yellow : DustID.Electric, Main.rand.NextVector2Unit() * 2.2f, 0, default, .5f).noGravity = true;
			}
			return false;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 12; i++)
			{
				if (isHovering)
					SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

				Dust.NewDustPerfect(Projectile.Center, Right ? DustID.FireworkFountain_Yellow : DustID.Electric, Main.rand.NextVector2Unit() * 2.2f).noGravity = Main.rand.NextBool(2);
			}
		}

		private bool TryPair()
		{
			int maxRange = 1800;

			foreach (Projectile proj in Main.projectile)
			{
				if (proj.ModProjectile is not Zipline zipline)
					continue;

				if (proj.owner == Projectile.owner && proj.active && zipline.Deployed && Right != zipline.Right && proj.type == Type && Projectile.Distance(proj.Center) <= maxRange)
				{
					PartnerIndex = proj.whoAmI;
					zipline.PartnerIndex = Projectile.whoAmI;

					Projectile.rotation = Projectile.AngleTo(proj.Center);
					proj.rotation = proj.AngleTo(Projectile.Center);

					for (int i = 0; i < 6; i++)
					{
						Vector2 position = (i > 2) ? proj.Center : Projectile.Center;
						Gore.NewGore(Entity.GetSource_FromAI(), position, Vector2.Zero, 99, .75f);
					}

					return true;
				}
			}
			return false;
		}

		public override bool PreDrawExtras()
		{
			if (PartnerIndex != -1)
			{
				Projectile pair = Main.projectile[PartnerIndex];
				Vector2 direction = Projectile.DirectionTo(pair.Center);

				Vector2 scale = new Vector2(Projectile.localAI[1], 1);

				Texture2D chainTexture = ModContent.Request<Texture2D>(Texture + "_Chain").Value;
				Texture2D hotChainTexture = ModContent.Request<Texture2D>(Texture + "_Chain_Hot").Value;

				int loops = ((int)Projectile.Distance(pair.Center) / 2 / chainTexture.Height) + 2;

				for (int i = 0; i < loops; i++)
				{
					Vector2 position = Projectile.Center + (direction * chainTexture.Height * i);
					Color lightColor = Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16));

					Main.EntitySpriteDraw(chainTexture, position - Main.screenPosition, null, lightColor, direction.ToRotation() + 1.57f, chainTexture.Size() / 2, scale, SpriteEffects.None, 0);

					bool Contains(out int index)
					{
						for (int o = 0; o < prLength; o++)
						{
							if (i == (int)(playerProgress[o] * loops * 2))
							{
								index = prLength - 1 - o;
								return true;
							}
						}

						index = 0;
						return false;
					}

					if (Contains(out int index))
					{
						Color color = Color.White * ((index + 1) * (float)(1f / prLength)) * vFadeout;

						Main.EntitySpriteDraw(hotChainTexture, position - Main.screenPosition, null, color, direction.ToRotation() + 1.57f, chainTexture.Size() / 2, scale, SpriteEffects.None, 0);
					}
				}
			}
			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow" + (Right ? "Right" : "Left")).Value;
			Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			Texture2D whiteTexture = ModContent.Request<Texture2D>(Texture + "_White").Value;
			Main.EntitySpriteDraw(whiteTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * (float)(1f - Projectile.localAI[1]), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			if (isHovering)
			{
				Texture2D cancelTexture = ModContent.Request<Texture2D>(Texture + "_Cancel").Value;
				Projectile.localAI[0] = Math.Min(1, Projectile.localAI[0] + .2f);

				Vector2 scale = new Vector2(2 - Projectile.localAI[0], Projectile.localAI[0]) * Projectile.scale;

				float sin = (float)Math.Sin(Main.timeForVisualEffects / 30);
				Color color = ((Right ? Color.Orange : Color.Cyan) * (.7f - sin * .3f)) with { A = 0 };

				Main.EntitySpriteDraw(cancelTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color), sin / 10, cancelTexture.Size() / 2, scale, SpriteEffects.None, 0);
			}
			else
			{
				Projectile.localAI[0] = 0;
			}
		}

		public override bool? CanDamage() => false;

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(isHovering);

		public override void ReceiveExtraAI(BinaryReader reader) => isHovering = reader.ReadBoolean();
	}
}
