using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.IO;
using SpiritMod.Utilities;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using System.Collections.Generic;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Prim;
using Terraria.DataStructures;

namespace SpiritMod.Items.Sets.OlympiumSet.BetrayersChains
{
	public class BetrayersChains : ModItem
	{
		private byte combo = 0;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Blades of Chaos");

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = Item.useAnimation = 22;
			Item.shootSpeed = 1f;
			Item.knockBack = 4f;
			Item.UseSound = SoundID.Item116;
			Item.shoot = ModContent.ProjectileType<BetrayersChainsProj>();
			Item.value = Item.sellPrice(gold: 2);
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 45;
			Item.rare = ItemRarityID.LightRed;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			float distanceMult = Main.rand.NextFloat(0.8f, 1.2f);

			bool preSlam = combo % 4 == 2;
			bool slam = combo % 4 == 3;

			Vector2 direction = velocity.RotatedByRandom(0.2f);

			for (int i = 0; i < 2; i++)
			{
				float swingTime = player.itemTimeMax;

				if (slam)
					swingTime *= 2f;
				else if (preSlam)
					swingTime *= 2.5f + (float)(i * .25f);
				else
					swingTime *= 1f + (float)(i * .18f);

				bool flip = slam ? (i > 0) : ((combo % 2) == 1);
				Projectile proj = Projectile.NewProjectileDirect(source, position, direction, type, (int)(damage * (slam ? 1f : .5f)), knockback, player.whoAmI);

				if (proj.ModProjectile is BetrayersChainsProj modProj)
				{
					modProj.SwingTime = (int)swingTime;
					modProj.SwingDistance = player.Distance(Main.MouseWorld) * distanceMult;
					modProj.Curvature = 0.231f;
					modProj.Flip = flip;
					modProj.Slam = slam;
					modProj.PreSlam = preSlam;
				}
				proj.localNPCHitCooldown = preSlam ? 15 : -1;

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);
			}
			combo++;

			if (slam)
			{
				if (Main.netMode != NetmodeID.Server)
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/ChainsClanking") with { PitchVariance = 0.5f }, player.position);
			}
			else if (preSlam)
			{
				player.itemTime = player.itemAnimation = 46;
			}

			return false;
		}

		public override float UseTimeMultiplier(Player player) => MathHelper.Max(1.12f, 2f - player.GetTotalAttackSpeed(DamageClass.Melee)); //Scale with melee speed buffs, like whips

		public override void NetSend(BinaryWriter writer) => writer.Write(combo);
		public override void NetReceive(BinaryReader reader) => combo = reader.ReadByte();
	}

	public class BetrayersChainsProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blades of Chaos");

			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.Size = new Vector2(80, 80);
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
			Projectile.localNPCHitCooldown = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.DamageType = DamageClass.Melee;
		}

		private Player Owner => Main.player[Projectile.owner];

		public int SwingTime;
		public float SwingDistance;
		public float Curvature;

		public ref float Timer => ref Projectile.ai[0];

		public const float THROW_RANGE = 250; //Peak distance from player when thrown out, in pixels

		public bool Flip = false;
		public bool Slam = false;
		public bool PreSlam = false;

		private readonly List<Vector2> oldBase = new();

		public Vector2 CurrentBase = Vector2.Zero;

		private float slamTimer = 0;

		public FireChainPrimTrail trail;
		Projectile phantomProj;

		public override void AI()
		{
			if (Projectile.timeLeft > 2) //Initialize chain control points on first tick, in case of projectile hooking in on first tick
			{
				_chainMidA = Projectile.Center;
				_chainMidB = Projectile.Center;
				CurrentBase = Owner.Center;
			}

			bool shouldDrawTrail = Slam || PreSlam;
			int startTime = 2;

			if (Timer >= startTime && shouldDrawTrail)
			{
				if (Timer == startTime)
				{
					float lengthMult = Slam ? 1f : 0.7f;

					trail = new FireChainPrimTrail(Projectile, 24, (int)(12 * lengthMult), lengthMult);
					SpiritMod.primitives.CreateTrail(trail);

					//not using itrail interface for reasons that make sense but i dont feel like explaining

					phantomProj = new()
					{
						Size = Projectile.Size,
						active = true,
						Center = CurrentBase
					};
					SpiritMod.TrailManager?.CreateTrail(phantomProj, new GradientTrail(new Color(252, 73, 3) * 0.6f, new Color(255, 160, 40) * 0.3f), new RoundCap(), new DefaultTrailPosition(), 40f, 400f * lengthMult, default);
				}

				if (slamTimer <= 20)
				{
					phantomProj.Center = CurrentBase;
					trail?.AddPoints();
				}
				else if (phantomProj != null)
				{
					phantomProj.active = false;
				}
			}

			if (Collision.CanHit(Owner.Center, 1, 1, CurrentBase, 1, 1))
				Lighting.AddLight(CurrentBase, Color.Orange.ToVector3());
			Projectile.timeLeft = 2;

			if (Slam)
			{
				Owner.itemTime = Owner.itemAnimation = 34;
				Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Projectile.Center) - (Owner.direction < 0 ? MathHelper.Pi : 0));
			}
			else
			{
				Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Main.MouseWorld) - (Owner.direction < 0 ? MathHelper.Pi : 0));
			}

			ThrowOutAI();
		}

		private Vector2 GetSwingPosition(float progress, float distanceMod = 0)
		{
			//Starts at owner center, goes to peak range, then returns to owner center
			float distance = MathHelper.Clamp(SwingDistance, THROW_RANGE * 0.1f, THROW_RANGE) * (MathHelper.Lerp((float)Math.Sin(progress * MathHelper.Pi), 1, 0.04f) + distanceMod);
			distance = Math.Max(distance, 2);

			float angleMaxDeviation = PreSlam ? (MathHelper.TwoPi * 1.7f) : MathHelper.Pi;
			float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(-angleMaxDeviation, angleMaxDeviation, progress); //Moves clockwise if player is facing right, counterclockwise if facing left
			return Projectile.velocity.RotatedBy(angleOffset) * distance;
		}

		private void ThrowOutAI()
		{
			Projectile.rotation = Projectile.AngleFrom(Owner.Center);
			Vector2 position = Owner.MountedCenter + (Vector2.UnitY * 20f * (Flip ? -1 : 1) * -Owner.direction).RotatedBy(Projectile.velocity.ToRotation());
			float progress = ++Timer / SwingTime; //How far the projectile is through its swing

			float halfway = .505f;
			if (Slam)
			{
				if (progress >= halfway)
				{
					progress = halfway - (float)(slamTimer / SwingTime * 0.1f);

					//Impact once
					if (slamTimer <= 0)
					{
						Vector2 effectPos = Owner.Center + (Projectile.velocity * MathHelper.Clamp(SwingDistance, THROW_RANGE * 0.1f, THROW_RANGE)).RotatedBy(0.1f);

						if (!Main.dedServ)
						{
							for (int i = 0; i < 25; i++)
							{
								Vector2 dirFactor = Projectile.velocity * Main.rand.NextFloat();

								if (Main.rand.NextBool(2))
								{
									Dust dust = Dust.NewDustPerfect(effectPos, DustID.Torch, (dirFactor * 8.0f).RotatedByRandom(1f), 0, default, Main.rand.NextFloat(0.5f, 1.5f));
									if (Main.rand.NextBool(2))
										dust.noGravity = true;
									if (Main.rand.NextBool(3))
										dust.fadeIn = 1f;
								}
								else
								{
									ParticleHandler.SpawnParticle(new FireParticle(effectPos, (dirFactor * 18.0f).RotatedByRandom(.6f), Color.White, Color.Yellow, Main.rand.NextFloat(0.2f, 0.9f), Main.rand.Next(10, 30)));
								}

								if (i < 3)
								{
									Vector2 randomVel = (dirFactor * 10.0f).RotatedByRandom(2f);
									ParticleHandler.SpawnParticle(new ImpactLine(effectPos + randomVel, Vector2.Normalize(randomVel), Color.Yellow, Vector2.One * Main.rand.NextFloat(0.5f, 1.2f), Main.rand.Next(8, 16)));

									ParticleHandler.SpawnParticle(new SmokeParticle(effectPos, Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 3.4f), Color.Lerp(Color.Black, Color.Goldenrod, Main.rand.NextFloat()), Main.rand.NextFloat(0.8f, 2.0f), Main.rand.Next(12, 28)));
								}
							}

							ParticleHandler.SpawnParticle(new PulseCircle(effectPos, Color.Lerp(Color.White, Color.OrangeRed, Main.rand.NextFloat(0.0f, 1.0f)) * 0.5f, 100, 8));
						}

						Vector2 oldSize = Projectile.Size;
						float sizeMult = 3f;

						Projectile.Size *= sizeMult;
						Projectile.position -= oldSize;

						Projectile.Damage();

						Projectile.Size = oldSize;

						SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MetalImpact") with { PitchVariance = 0.5f }, Projectile.position);
					}

					slamTimer++;
				}

				progress = EaseFunction.EaseCubicInOut.Ease(progress);
				if (progress > 0.15f && progress < 0.85f)
					Dust.NewDustPerfect(Projectile.Center + Projectile.velocity + Main.rand.NextVector2Circular(15, 15), 6, Main.rand.NextVector2Circular(2, 2), 0, default, 1.15f).noGravity = true;
			}
			else if (PreSlam)
			{
				for (int i = 0; i < 5; i++)
					Dust.NewDustPerfect(Projectile.Center + Projectile.velocity + Main.rand.NextVector2Circular(15, 15), 6, -(Vector2.UnitY * Main.rand.NextFloat(2.5f, 6.0f)).RotatedBy(Projectile.rotation + Main.rand.NextFloat(0.09f)), 0, default, 1.15f).noGravity = true;
			}

			Projectile.Center = position + GetSwingPosition(progress, -(slamTimer * 0.035f));
			Projectile.direction = Projectile.spriteDirection = -Owner.direction * (Flip ? -1 : 1);

			if (Timer >= SwingTime + 1)
				Projectile.Kill();
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (Projectile.timeLeft > 2)
				return false;

			Texture2D projTexture = TextureAssets.Projectile[Projectile.type].Value;

			//End control point for the chain
			Vector2 projBottom = Projectile.Center + new Vector2(0, projTexture.Height / 2).RotatedBy(Projectile.rotation) * 0.75f;
			DrawChainCurve(projBottom, out Vector2[] chainPositions);

			//Adjust rotation to face from the last point in the bezier curve
			float newRotation = (projBottom - chainPositions[^2]).ToRotation() + MathHelper.PiOver2;

			//Draw from bottom center of texture
			Vector2 origin = new Vector2(projTexture.Width / 2, projTexture.Height);
			SpriteEffects flip = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));

			Main.EntitySpriteDraw(projTexture, projBottom - Main.screenPosition, null, lightColor, newRotation, origin, Projectile.scale, flip, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, projBottom - Main.screenPosition, null, Color.White, newRotation, origin, Projectile.scale, flip, 0);

			CurrentBase = projBottom + (newRotation - 1.57f).ToRotationVector2() * (projTexture.Height / 2);

			oldBase.Add(projBottom - Main.screenPosition);

			if (oldBase.Count > 8)
				oldBase.RemoveAt(0);

			if (Slam && slamTimer < 20)
				DrawGlow(ModContent.Request<Texture2D>(Texture + "_White").Value, projBottom, newRotation, Vector2.One * Projectile.scale, origin, flip, pulse: true);

			return false;
		}

		private void DrawGlow(Texture2D texture, Vector2 position, float rotation, Vector2 scale, Vector2 origin, SpriteEffects effects, float rate = 15f, bool pulse = false)
		{
			float progress = MathHelper.Clamp(slamTimer / rate, 0f, 1.2f);
			float transparency = (float)Math.Pow(1 - progress, 2);

			if (pulse)
				scale *= (float)(1f + progress);

			Main.EntitySpriteDraw(texture, position - Main.screenPosition, null, Color.White * transparency, rotation, origin, scale, effects, 0);
		}

		//Control points for drawing chain bezier, update slowly when hooked in
		private Vector2 _chainMidA;
		private Vector2 _chainMidB;
		private void DrawChainCurve(Vector2 projBottom, out Vector2[] chainPositions)
		{
			Texture2D chainTex = ModContent.Request<Texture2D>(Texture + "_Chain", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			float progress = Timer / SwingTime;

			if (Slam)
				progress = EaseFunction.EaseCubicInOut.Ease(progress);

			float angleMaxDeviation = MathHelper.Pi * 0.85f;
			float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(angleMaxDeviation, -angleMaxDeviation / 4, progress);

			_chainMidA = Owner.MountedCenter + GetSwingPosition(progress).RotatedBy(angleOffset) * Curvature;
			_chainMidB = Owner.MountedCenter + GetSwingPosition(progress).RotatedBy(angleOffset / 2) * Curvature * 2.5f;

			BezierCurve curve = new BezierCurve(new Vector2[] { Owner.MountedCenter, _chainMidA, _chainMidB, projBottom });

			int numPoints = 20; //Should make dynamic based on curve length, but I'm not sure how to smoothly do that while using a bezier curve
			chainPositions = curve.GetPoints(numPoints).ToArray();

			//Draw each chain segment, skipping the very first one, as it draws partially behind the player
			for (int i = 1; i < numPoints; i++)
			{
				Vector2 position = chainPositions[i];

				float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
				float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

				Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
				Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
				Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
				
				Main.EntitySpriteDraw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);

				if (Slam && slamTimer < 10)
					DrawGlow(ModContent.Request<Texture2D>(Texture + "_Chain_White").Value, position, rotation, scale, origin, SpriteEffects.None, 10f);
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			BezierCurve curve = new BezierCurve(new Vector2[] { Owner.MountedCenter, _chainMidA, _chainMidB, Projectile.Center });

			int numPoints = 32;
			Vector2[] chainPositions = curve.GetPoints(numPoints).ToArray();
			float collisionPoint = 0;

			for (int i = 1; i < numPoints; i++)
			{
				Vector2 position = chainPositions[i];
				Vector2 previousPosition = chainPositions[i - 1];

				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
					return true;
			}
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override bool? CanDamage() => base.CanDamage();

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (Slam || PreSlam)
				target.AddBuff(BuffID.OnFire3, 180);

			if (Collision.CheckAABBvAABBCollision(target.position, target.Size, Projectile.position, Projectile.Size))
			{
				modifiers.FinalDamage *= 1.3f;
				for (int i = 0; i < 8; i++)
				{
					Vector2 vel = Main.rand.NextFloat(6.28f).ToRotationVector2();
					vel *= Main.rand.NextFloat(2, 5);

					ImpactLine line = new(target.Center - (vel * 10), vel, Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat()), new Vector2(0.25f, Main.rand.NextFloat(0.35f, 0.55f) * 1.5f), 70)
					{ TimeActive = 30 };
					ParticleHandler.SpawnParticle(line);
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			if (phantomProj != null)
				phantomProj.active = false;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(SwingTime);
			writer.Write(SwingDistance);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			SwingTime = reader.ReadInt32();
			SwingDistance = reader.ReadSingle();
		}
	}
}