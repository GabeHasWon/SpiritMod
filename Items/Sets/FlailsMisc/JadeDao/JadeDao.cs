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
using Terraria.DataStructures;

namespace SpiritMod.Items.Sets.FlailsMisc.JadeDao
{
	public class JadeDao : ModItem
	{
		private bool reversed = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Jade Daos");
			Tooltip.SetDefault("Pulls enemies inward on release");
		}

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = Item.useAnimation = 30;
			Item.shootSpeed = 1f;
			Item.knockBack = 4f;
			Item.UseSound = SoundID.Item116;
			Item.shoot = ModContent.ProjectileType<JadeDaoProj>();
			Item.value = Item.sellPrice(gold: 10);
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 170;
			Item.rare = ItemRarityID.LightPurple;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			for (int i = 0; i < 2; i++)
			{
				Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, (int)(damage * .5f), knockback, player.whoAmI);
				if (proj.ModProjectile is JadeDaoProj modProj)
				{
					modProj.Flip = reversed;
					modProj.secondary = i > 0;
				}

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);
			}
			reversed = !reversed;

			return false;
		}

		public override float UseTimeMultiplier(Player player) => player.GetAttackSpeed(DamageClass.Melee); //Scale with melee speed buffs, like whips
	}

	public class JadeDaoProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Jade Daos");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.Size = new Vector2(60, 60);
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 1;

			Projectile.localNPCHitCooldown = -1;
			Projectile.usesLocalNPCImmunity = true;
		}

		private Player Owner => Main.player[Projectile.owner];

		public int SwingTime;
		public float SwingDistance;
		public float Curvature;

		public ref float Timer => ref Projectile.ai[0];
		public ref float AiState => ref Projectile.ai[1];

		private const int STATE_SWING = 0;
		private const int STATE_THRUST = 1;
		private const int STATE_RETRACT = 2;

		private NPC hookNPC; //The npc the projectile is hooked into

		public const float THROW_RANGE = 320; //Peak distance from player when thrown out, in pixels
		public const float THRUST_RANGE = 580; //Peak distance from player when thrust out, in pixels

		public bool Flip = false;

		internal List<Vector2> oldBase = new();

		public Vector2 CurrentBase = Vector2.Zero;

		public bool secondary = false;

		public JadeDaoBasicTrail trail;

		public override void AI()
		{
			if (Projectile.timeLeft > 2) //Initialize the projectile
				Recalculate();

			if (Collision.CanHit(Owner.Center, 1, 1, CurrentBase, 1, 1))
				Lighting.AddLight(CurrentBase, new Color(54, 192, 98).ToVector3());

			Projectile.timeLeft = 2;
			Owner.itemTime = Owner.itemAnimation = 2;
			Owner.heldProj = Projectile.whoAmI;

			if (hookNPC == null)
				Projectile.rotation = Projectile.AngleFrom(Owner.Center) - 1.57f;

			if (AiState != STATE_SWING)
				ThrustAI();
			else
				ThrowOutAI();

			if (!Owner.channel && AiState == STATE_SWING)
			{
				AiState = STATE_THRUST;
				Recalculate();
			}
			if (Timer >= SwingTime)
			{
				if (AiState != STATE_SWING)
					Projectile.Kill();

				Recalculate();
			}
		}

		private void Recalculate()
		{
			Timer = 0;
			Item item = Owner.HeldItem;

			if (Owner.whoAmI == Main.myPlayer)
			{
				SwingDistance = Owner.Distance(Main.MouseWorld) * Main.rand.NextFloat(0.8f, 1.8f);

				float inaccuracy = (AiState != STATE_SWING) ? Main.rand.NextFloat(-0.1f, 0.1f) : (secondary ? 0.2f : 0f);	
				Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).RotatedBy(inaccuracy);

				Projectile.netUpdate = true;
			}
			SwingTime = (int)(item.useTime * item.ModItem.UseTimeMultiplier(Owner) * (float)((AiState == STATE_THRUST) ? 2.4f : 2.8f));
			Curvature = 0.29f;

			Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
			Projectile.ResetLocalNPCHitImmunity();

			//Handle trail and chain drawing variables
			_chainMidA = Projectile.Center;
			_chainMidB = Projectile.Center;

			if (trail != null)
				trail.Destroyed = true;

			SpiritMod.primitives.CreateTrail(trail = new JadeDaoBasicTrail(Projectile));
		}

		private Vector2 GetPosition(float progress)
		{
			if (AiState != STATE_SWING)
			{
				float distance = MathHelper.Clamp(SwingDistance, 0, THRUST_RANGE) * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.Pi), 1, 0.04f);
				return Projectile.velocity * distance;
			}
			else
			{
				float angleDeviation = MathHelper.Pi;

				//Starts at owner center, goes to peak range, then returns to owner center
				float distance = MathHelper.Clamp(SwingDistance, THROW_RANGE * 0.1f, THROW_RANGE) * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.Pi), 1, 0.04f);

				float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(-angleDeviation, angleDeviation, progress); //Moves clockwise if player is facing right, counterclockwise if facing left
				return Projectile.velocity.RotatedBy(angleOffset) * distance;
			}
		}

		private void ThrowOutAI()
		{
			Vector2 basePos = Owner.MountedCenter;
			float progress = ++Timer / SwingTime; //How far the projectile is through its swing
			progress = EaseFunction.EaseCubicInOut.Ease(progress);

			Projectile.Center = basePos + GetPosition(progress);
			Projectile.direction = Projectile.spriteDirection = -Owner.direction * (Flip ? -1 : 1);

			if (Timer == (SwingTime / 2) && Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/SwordSlash1") with { PitchVariance = 0.6f, Volume = 0.6f }, Projectile.position);

			Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Projectile.Center) - (Owner.direction < 0 ? MathHelper.Pi : 0));
		}

		private void ThrustAI()
		{
			Vector2 basePos = Owner.MountedCenter;
			float progress = ++Timer / SwingTime; //How far the projectile is through its swing

			if (progress > 0.15f && progress < 0.85f)
			{
				int timeLeft = Main.rand.Next(40, 100);

				StarParticle particle = new StarParticle(CurrentBase, Main.rand.NextVector2Circular(1, 1), Color.Lerp(new Color(106, 255, 35), new Color(18, 163, 85), Main.rand.NextFloat()), Main.rand.NextFloat(0.1f, 0.2f), timeLeft)
				{ TimeActive = (uint)(timeLeft / 2) };
				ParticleHandler.SpawnParticle(particle);
			}

			if (hookNPC != null) //Owner has an NPC hooked
			{
				Projectile.velocity = hookNPC.velocity;
				Vector2 direction = Owner.DirectionTo(Projectile.Center);

				if (!hookNPC.active)
				{
					Timer = SwingTime / 2;
					Projectile.velocity = direction;

					hookNPC = null;
				}
				else if (progress >= .5f) //Unhook the NPC
				{
					Curvature = Main.rand.NextFloat(-0.35f, 0.35f);

					SwingDistance = Owner.Distance(Projectile.Center);
					Projectile.velocity = direction;

					if (hookNPC.knockBackResist > 0f && hookNPC.CanBeChasedBy(Projectile))
						hookNPC.velocity = -(direction * 20 * hookNPC.knockBackResist);

					Vector2 pos = hookNPC.getRect().ClosestPointInRect(Projectile.Center);
					for (int i = 0; i < 30; i++)
					{
						Dust dust = Dust.NewDustPerfect(pos + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.0f, 8.0f)), DustID.Blood, (-direction * Main.rand.NextFloat(0.5f, 7.0f)).RotatedByRandom(.8f), Scale: Main.rand.NextFloat(0.5f, 1.5f));
						
						if (Main.rand.NextBool(2))
						{
							dust.noGravity = true;
							dust.fadeIn = 1.2f;
						}
					}

					hookNPC = null;

					if (Main.netMode != NetmodeID.Server)
					{
						SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Stab") with { PitchVariance = 0.3f, Pitch = 0.5f }, Projectile.position);
						SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/ChainsClanking") with { PitchVariance = 0.5f }, Owner.position);
					}
				}
			}
			else
			{
				Projectile.Center = basePos + GetPosition(progress);

				if (Timer == 1) //Spawn one-time effects
				{
					Vector2 dirUnit = Vector2.UnitX.RotatedBy(Projectile.velocity.ToRotation());
					for (int i = 0; i < 12; i++)
						Dust.NewDustPerfect(Owner.Center + (dirUnit * 18) + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.0f, 10.0f)), DustID.GemEmerald, dirUnit * Main.rand.NextFloat(1.0f, 7.0f), 80, default, Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;

					Curvature = 0;
				}
			}

			Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Main.MouseWorld) - (Owner.direction < 0 ? MathHelper.Pi : 0));
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (Projectile.timeLeft > 2)
				return false;

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			//End control point for the chain
			Vector2 projBottom = Projectile.Center + new Vector2(0, texture.Height / 2).RotatedBy(Projectile.rotation) * 0.75f;
			DrawChainCurve(projBottom, out Vector2[] chainPositions);

			//Adjust rotation to face from the last point in the bezier curve
			float rotation = (projBottom - chainPositions[^2]).ToRotation();

			Vector2 origin = new Vector2(texture.Width * (.5f + (.25f * Projectile.spriteDirection)), texture.Height);
			SpriteEffects effects = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));

			for (int i = 0; i < 2; i++)
			{
				var drawTexture = (i > 0) ? ModContent.Request<Texture2D>(Texture + "_Glow").Value : texture;
				Main.EntitySpriteDraw(drawTexture, projBottom - Main.screenPosition, null, Projectile.GetAlpha(lightColor), rotation + MathHelper.PiOver2, origin, Projectile.scale, effects, 0);
			}

			CurrentBase = projBottom + rotation.ToRotationVector2() * (texture.Height / 2);
			oldBase.Add(projBottom + rotation.ToRotationVector2() * (texture.Height / 2));
			if (oldBase.Count > 8)
				oldBase.RemoveAt(0);

			if (AiState == STATE_THRUST && Timer < 30)
				DrawGlow(ModContent.Request<Texture2D>(Texture + "_White").Value, projBottom, rotation + MathHelper.PiOver2, Vector2.One * Projectile.scale, origin, effects, pulse: true);

			return false;
		}

		private void DrawGlow(Texture2D texture, Vector2 position, float rotation, Vector2 scale, Vector2 origin, SpriteEffects effects, float rate = 30f, bool pulse = false)
		{
			float progress = MathHelper.Clamp(Timer / rate, 0f, 1.2f);
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
			Texture2D chainTex = ModContent.Request<Texture2D>(Texture + "_Chain").Value;

			float progress = Timer / SwingTime;

			progress = EaseFunction.EaseCubicInOut.Ease(progress);

			float angleMaxDeviation = MathHelper.Pi * 0.85f;
			float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(angleMaxDeviation, -angleMaxDeviation / 4, progress);

			_chainMidA = Owner.MountedCenter + GetPosition(progress).RotatedBy(angleOffset) * Curvature;
			_chainMidB = Owner.MountedCenter + GetPosition(progress).RotatedBy(angleOffset / 2) * Curvature * 2.5f;

			BezierCurve curve = new BezierCurve(new Vector2[] { Owner.MountedCenter, _chainMidA, _chainMidB, projBottom });

			int numPoints = (AiState != STATE_SWING) ? (int)(SwingDistance / 10) : 30;
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

				if (AiState == STATE_THRUST && Timer < 30)
					DrawGlow(ModContent.Request<Texture2D>(Texture + "_Chain_White").Value, position, rotation, scale, origin, SpriteEffects.None, 20f);
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

		public override bool? CanDamage() => AiState != STATE_RETRACT;

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			float progress = Timer / SwingTime; //How far the projectile is through its swing
			progress = EaseFunction.EaseCubicInOut.Ease(progress);

			if (AiState == STATE_THRUST && progress < 0.6f)
			{
				Vector2 projTop = Projectile.Center + new Vector2(0, TextureAssets.Projectile[Type].Height() / 2).RotatedBy(Projectile.velocity.ToRotation() - 1.57f) * 0.75f;
				Vector2 stabDims = new Vector2(30);

				if (Collision.CheckAABBvAABBCollision(target.position, target.Size, projTop - (stabDims / 2), stabDims)) //Hook an NPC
				{
					if (Owner.GetModPlayer<MyPlayer>().Shake < 5)
						Owner.GetModPlayer<MyPlayer>().Shake += 5;

					for (int j = 0; j < 14; j++)
					{
						int timeLeft = Main.rand.Next(20, 40);

						StarParticle particle = new StarParticle(
							target.Center,
							Main.rand.NextVector2Circular(10, 7),
							Color.Lerp(new Color(106, 255, 35), new Color(18, 163, 85), Main.rand.NextFloat()),
							Main.rand.NextFloat(0.15f, 0.3f),
							timeLeft);
						ParticleHandler.SpawnParticle(particle);
					}

					Timer = 0;
					hookNPC = target;

					AiState = STATE_RETRACT;
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Stab") with { PitchVariance = 0.3f, Pitch = -0.5f, Volume = 0.8f }, Projectile.position);

					if (trail != null)
						trail.Destroyed = true;

					crit = true;
				}
			}
			else if (Collision.CheckAABBvAABBCollision(target.position, target.Size, Projectile.position, Projectile.Size))
			{
				damage = (int)(damage * 1.5f);

				for (int i = 0; i < 8; i++)
				{
					Vector2 vel = Main.rand.NextFloat(6.28f).ToRotationVector2();
					vel *= Main.rand.NextFloat(2, 5);

					ImpactLine line = new ImpactLine(target.Center - (vel * 10), vel, Color.Green, new Vector2(0.25f, Main.rand.NextFloat(0.75f, 1.75f) * 1.5f), 70)
					{ TimeActive = 30 };
					ParticleHandler.SpawnParticle(line);
				}
			}
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(SwingTime);
			writer.Write(SwingDistance);
			writer.Write(Flip);
			writer.Write(Curvature);

			if (hookNPC == default(NPC)) //Write a -1 instead if the npc isnt set
				writer.Write(-1);
			else
				writer.Write(hookNPC.whoAmI);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			SwingTime = reader.ReadInt32();
			SwingDistance = reader.ReadSingle();
			Flip = reader.ReadBoolean();
			Curvature = reader.ReadSingle();

			int whoAmI = reader.ReadInt32(); //Read the whoami value sent
			if (whoAmI == -1) //If its a -1, sync that the npc hasn't been set yet
				hookNPC = default;
			else
				hookNPC = Main.npc[whoAmI];
		}
	}
}