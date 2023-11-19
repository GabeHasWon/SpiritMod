using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Prim;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using SpiritMod.Projectiles;
using SpiritMod.Buffs;
using Terraria.Audio;

namespace SpiritMod.Items.Weapon.Magic.ShadowbreakWand
{
	public class ShadowbreakOrb : ModProjectile, IDrawAdditive, ITrailProjectile
	{
		public void DoTrailCreation(TrailManager trailManager)
			=> trailManager.CreateTrail(Projectile, new StandardColorTrail(Color.Purple with { A = 0 } * .75f), new RoundCap(), new DefaultTrailPosition(), 50, 100, new ImageShader(Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_4", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, 0.05f, 1f, 1f));

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30; 
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(30);
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.hide = true;
			Projectile.extraUpdates = 1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 1;
			Projectile.alpha = 100;
			Projectile.timeLeft = 30;
		}

		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public const int CounterMax = 60;

		private bool Released
		{
			get => Projectile.ai[1] == 1;
			set => Projectile.ai[1] = value ? 1 : 0;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.Distance(targetHitbox.Center.ToVector2()) < (50 * (Projectile.scale + 0.4f)); //circular collision
		
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			float maxScale = .5f;

			Projectile.rotation += 1f - ((float)Counter / CounterMax) + 0.05f;
			Lighting.AddLight(Projectile.Center, Color.Magenta.ToVector3());

			if (!player.channel)
			{
				if (!Released)
					SoundEngine.PlaySound(SoundID.Item45, Projectile.Center);
				Released = true;
			}

			if (!Released)
			{
				Projectile.timeLeft++;

				if (Projectile.numUpdates == 0)
					Counter = Math.Min(Counter + 1, CounterMax);

				player.itemAnimation = player.itemTime = 2;
				player.reuseDelay = player.itemTimeMax;

				if (player.whoAmI == Main.myPlayer)
				{
					float magnitude = 12;
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, player.DirectionTo(Main.MouseWorld) * magnitude, .05f);
					Projectile.netUpdate = true;
				}

				int sign = Math.Sign(Projectile.Center.X < player.Center.X ? -1 : 1);
				player.ChangeDir(sign);

				if ((Counter + 1) == CounterMax)
					SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
				if (Counter >= CounterMax)
				{
					Projectile.localAI[0] = MathHelper.Lerp(Projectile.localAI[0], 1, 0.05f);
					Projectile.alpha = Math.Max(Projectile.alpha - 10, 0);
				}

				float shrinkDegree = .2f;
				float distance = Math.Min(Counter * 3, 50);

				Projectile.Center = player.Center + (Vector2.UnitX * distance).RotatedBy(Projectile.velocity.ToRotation());
				Projectile.scale = MathHelper.Lerp(0, maxScale + shrinkDegree - (Projectile.localAI[0] * shrinkDegree), (float)Counter / CounterMax);

				if (Main.rand.NextBool(2))
				{
					Vector2 randomPos = Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.25f, 1.0f) * 50 * ((maxScale * 2) - Projectile.scale));
					Vector2 velocity = (randomPos.DirectionTo(Projectile.Center) * Main.rand.NextFloat(0.5f, 3.0f) * ((float)Counter / CounterMax)).RotatedBy(1.57f);

					Dust.NewDustPerfect(randomPos, Main.rand.NextBool() ? DustID.ShadowbeamStaff : DustID.PurpleTorch, velocity, 100, default, Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
				}
			}
			else
			{
				Projectile.alpha = 0;
				Projectile.scale = maxScale;

				if (Projectile.timeLeft > 8)
				{
					float magnitude = Main.rand.NextFloat();
					ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 30), Projectile.velocity * (magnitude / 4), Color.White, Color.Purple, magnitude / 3, (int)(magnitude * 30f)));
				}
				if (Projectile.timeLeft > 25)
				{
					for (int i = 0; i < 2; i++)
					{
						Vector2 randomPos = Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.25f, 1.0f) * 15);

						Dust dust = Dust.NewDustPerfect(randomPos, DustID.PurpleTorch, Projectile.velocity * Main.rand.NextFloat(0.85f, 1.1f), Main.rand.Next(100, 200), default, Main.rand.NextFloat(1.5f, 5.0f));
						dust.noGravity = true;
					}
				}
			}
		}

		public override bool ShouldUpdatePosition() => Released;

		public override bool? CanDamage() => Released;

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (!(Counter < CounterMax || Projectile.timeLeft > 10))
			{
				modifiers.FinalDamage *= 2;
				modifiers.Knockback.Base = 10;
				target.AddBuff(ModContent.BuffType<Shadowbroken>(), 300);
			}
		}

		public override void OnKill(int timeLeft)
		{
			if (Counter >= CounterMax)
			{
				ProjectileExtras.Explode(Projectile.whoAmI, 200, 200, delegate
				{
					if (Main.dedServ)
						return;

					SoundEngine.PlaySound(SoundID.Item103, Projectile.Center);
					SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);

					ParticleHandler.SpawnParticle(new Shatter(Projectile.Center, 30) { Rotation = Main.rand.NextFloatDirection() * .2f });

					for (int i = 0; i < 35; i++)
					{
						float magnitude = Main.rand.NextFloat();
						Vector2 velocity = Main.rand.NextVector2Unit() * magnitude * 10;

						Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.ShadowbeamStaff : DustID.PurpleTorch, velocity, 100, default, Main.rand.NextFloat(1.0f, 4.0f)).noGravity = true;
					}
				});
			}
		}

		public void AdditiveCall(SpriteBatch sB, Vector2 screenPos)
		{
			float rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

			Vector2 scale = new Vector2(1f - Projectile.velocity.Length() / 50, 1f + Projectile.velocity.Length() / 50) * Projectile.scale;
			if (!Released && Counter > (CounterMax * .3f))
				scale = new Vector2(Projectile.scale);

			Vector2 drawCenter = Projectile.Center - screenPos;
			Texture2D bloom2 = ModContent.Request<Texture2D>(Texture + "_Bloom1").Value;
			Vector2 bloom2Scale = scale * .5f * (float)Math.Sin(Main.timeForVisualEffects / 25);
			sB.Draw(bloom2, drawCenter, null, Color.DeepPink * Projectile.Opacity, Projectile.rotation, bloom2.Size() / 2, bloom2Scale, SpriteEffects.None, 0);

			sB.End(); sB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

			//Set parameters for radial noise shader
			Effect effect = ModContent.Request<Effect>("SpiritMod/Effects/PortalShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			effect.Parameters["PortalNoise"].SetValue(Mod.Assets.Request<Texture2D>("Utilities/Noise/SpiralNoise", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
			effect.Parameters["Timer"].SetValue(MathHelper.WrapAngle(Main.GlobalTimeWrappedHourly / 3));
			effect.Parameters["DistortionStrength"].SetValue(0);
			effect.Parameters["Rotation"].SetValue(Projectile.rotation);
			effect.CurrentTechnique.Passes[0].Apply();

			sB.Draw(tex, drawCenter, null, Color.Purple * Projectile.Opacity, rotation, tex.Size() / 2, scale * 0.75f, SpriteEffects.None, 0);
			sB.Draw(tex, drawCenter, null, Color.Pink * Projectile.Opacity * 1.25f, rotation, tex.Size() / 2, scale * 0.7f, SpriteEffects.None, 0);

			sB.End(); sB.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

			CirclePrimitive[] circles = new CirclePrimitive[]
			{
				new CirclePrimitive()
				{
					Color = new Color(253, 46, 250) * Projectile.Opacity,
					Radius = 20,
					Position = Projectile.Center - Main.screenPosition,
					ScaleModifier = scale,
					MaxRadians = MathHelper.TwoPi,
					Rotation = Projectile.velocity.ToRotation()
				},
				new CirclePrimitive()
				{
					Color = new Color(47, 42, 453) * Projectile.Opacity,
					Radius = 19 + (float)(Math.Sin(Main.timeForVisualEffects / 30) * 4f),
					Position = Projectile.Center - Main.screenPosition,
					ScaleModifier = scale,
					MaxRadians = MathHelper.TwoPi,
					Rotation = Projectile.velocity.ToRotation()
				}
			};

			Effect circleAA = ModContent.Request<Effect>("SpiritMod/Effects/CirclePrimitiveAA", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			PrimitiveRenderer.DrawPrimitiveShapeBatched(circles, circleAA);

			Texture2D bloom = ModContent.Request<Texture2D>(Texture + "_Bloom").Value;
			sB.Draw(bloom, drawCenter, null, new Color(164, 56, 253) * Projectile.Opacity, Projectile.rotation, bloom.Size() / 2, scale * .7f, SpriteEffects.None, 0);
		}
	}
}