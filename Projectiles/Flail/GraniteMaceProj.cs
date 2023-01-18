using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using ReLogic.Content;
using SpiritMod.Projectiles.Magic;

namespace SpiritMod.Projectiles.Flail
{
	public class GraniteMaceProj : ModProjectile
	{
		private const string ChainTexturePath = "SpiritMod/Projectiles/Flail/GraniteMace_Chain";
		private enum AIState
		{
			Spinning,
			LaunchingForward,
			Retracting,
			UnusedState,
			ForcedRetracting,
			Ricochet,
			Dropping
		}
		private AIState CurrentAIState
		{
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}
		public ref float StateTimer => ref Projectile.ai[1];
		public ref float CollisionCounter => ref Projectile.localAI[0];
		public ref float SpinningStateTimer => ref Projectile.localAI[1];

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clatter Mace");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.width = 30;
			Projectile.height = 28;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		// Terraria.Projectile.AI_015_Flails() 
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f)
			{
				Projectile.Kill();
				return;
			}
			if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
			{
				Projectile.Kill();
				return;
			}

			Vector2 mountedCenter = player.MountedCenter;
			int launchTimeLimit = 25;  // How much time the projectile can go before retracting (speed and shootTimer will set the flail's range)
			float launchSpeed = 10f; // How fast the projectile can move
			float maxLaunchLength = 1200f; // How far the projectile's chain can stretch before being forced to retract when in launched state
			float retractAcceleration = 3f; // How quickly the projectile will accelerate back towards the player while retracting
			float maxRetractSpeed = 10f; // The max speed the projectile will have while retracting
			float forcedRetractAcceleration = 6f; // How quickly the projectile will accelerate back towards the player while being forced to retract
			float maxForcedRetractSpeed = 15f; // The max speed the projectile will have while being forced to retract
			float unusedRetractAcceleration = 1f;
			float unusedMaxRetractSpeed = 14f;
			int unusedChainLength = 60;
			int defaultHitCooldown = 10; // How often your flail hits when resting on the ground, or retracting
			int spinHitCooldown = 15; // How often your flail hits when spinning
			int movingHitCooldown = 10; // How often your flail hits when moving
			int ricochetTimeLimit = launchTimeLimit + 5;

			// Scaling these speeds and accelerations by the players meleeSpeed make the weapon more responsive if the player boosts their meleeSpeed
			float meleeSpeed = player.GetAttackSpeed(DamageClass.Melee);
			float meleeSpeedMultiplier = 1f / meleeSpeed;
			launchSpeed *= meleeSpeedMultiplier;
			unusedRetractAcceleration *= meleeSpeedMultiplier;
			unusedMaxRetractSpeed *= meleeSpeedMultiplier;
			retractAcceleration *= meleeSpeedMultiplier;
			maxRetractSpeed *= meleeSpeedMultiplier;
			forcedRetractAcceleration *= meleeSpeedMultiplier;
			maxForcedRetractSpeed *= meleeSpeedMultiplier;
			float launchRange = launchSpeed * launchTimeLimit;
			float maxDroppedRange = launchRange + 160f;
			Projectile.localNPCHitCooldown = defaultHitCooldown;

			switch (CurrentAIState)
			{
				case AIState.Spinning:
					{
						if (Projectile.owner == Main.myPlayer)
						{
							Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
							player.ChangeDir((unitVectorTowardsMouse.X > 0f) ? 1 : (-1));
							if (!player.channel) // If the player releases then change to moving forward mode
							{
								CurrentAIState = AIState.LaunchingForward;
								StateTimer = 0f;
								Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
								Projectile.Center = mountedCenter;
								Projectile.netUpdate = true;
								Projectile.ResetLocalNPCHitImmunity();
								Projectile.localNPCHitCooldown = movingHitCooldown;
								break;
							}
						}
						SpinningStateTimer += 1f;
						// This line creates a unit vector that is constantly rotated around the player. 10f controls how fast the projectile visually spins around the player
						Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);

						offsetFromPlayer.Y *= 1.0f;
						if (offsetFromPlayer.Y * player.gravDir > 0f)
						{
							offsetFromPlayer.Y *= 0.5f;
						}
						Projectile.Center = mountedCenter + offsetFromPlayer * 30f;
						Projectile.velocity = Vector2.Zero;
						Projectile.localNPCHitCooldown = spinHitCooldown; // set the hit speed to the spinning hit speed
						break;
					}
				case AIState.LaunchingForward:
					{
						bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
						shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
						if (player.controlUseItem) // If the player clicks, transition to the Dropping state
						{
							CurrentAIState = AIState.Dropping;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
							break;
						}
						if (shouldSwitchToRetracting)
						{
							CurrentAIState = AIState.Retracting;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.3f;
							// This is also where Drippler Crippler spawns its projectile, see above code.
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						Projectile.localNPCHitCooldown = movingHitCooldown;
						break;
					}
				case AIState.Retracting:
					{
						Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= maxRetractSpeed)
						{
							Projectile.Kill(); // Kill the projectile once it is close enough to the player
							return;
						}
						if (player.controlUseItem) // If the player clicks, transition to the Dropping state
						{
							CurrentAIState = AIState.Dropping;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
						}
						else
						{
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
							player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						}
						break;
					}
				case AIState.UnusedState: // Projectile.ai[0] == 3; This case is actually unused, but maybe a Terraria update will add it back in, or maybe it is useless, so I left it here.
					{
						if (!player.controlUseItem)
						{
							CurrentAIState = AIState.ForcedRetracting; // Move to super retracting mode if the player taps
							StateTimer = 0f;
							Projectile.netUpdate = true;
							break;
						}
						float currentChainLength = Projectile.Distance(mountedCenter);
						Projectile.tileCollide = StateTimer == 1f;
						bool flag3 = currentChainLength <= launchRange;
						if (flag3 != Projectile.tileCollide)
						{
							Projectile.tileCollide = flag3;
							StateTimer = Projectile.tileCollide ? 1 : 0;
							Projectile.netUpdate = true;
						}
						if (currentChainLength > unusedChainLength)
						{

							if (currentChainLength >= launchRange)
							{
								Projectile.velocity *= 0.5f;
								Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedMaxRetractSpeed);
							}
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedRetractAcceleration);
						}
						else
						{
							if (Projectile.velocity.Length() < 6f)
							{
								Projectile.velocity.X *= 0.96f;
								Projectile.velocity.Y += 0.2f;
							}
							if (player.velocity.X == 0f)
							{
								Projectile.velocity.X *= 0.96f;
							}
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						break;
					}
				case AIState.ForcedRetracting:
					{
						Projectile.tileCollide = false;
						Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed)
						{
							Projectile.Kill(); // Kill the projectile once it is close enough to the player
							return;
						}
						Projectile.velocity *= 0.98f;
						Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
						Vector2 target = Projectile.Center + Projectile.velocity;
						Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
						if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f)
						{
							Projectile.Kill(); // Kill projectile if it will pass the player
							return;
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						break;
					}
				case AIState.Ricochet:
					if (StateTimer++ >= ricochetTimeLimit)
					{
						CurrentAIState = AIState.Dropping;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else
					{
						Projectile.localNPCHitCooldown = movingHitCooldown;
						Projectile.velocity.Y += 0.6f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
					}
					break;
				case AIState.Dropping:
					if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange)
					{
						CurrentAIState = AIState.ForcedRetracting;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else
					{
						Projectile.velocity.Y += 0.8f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
					}
					break;
			}

			bool freeRotation = CurrentAIState == AIState.Ricochet || CurrentAIState == AIState.Dropping;
			if (freeRotation)
			{
				if (Projectile.velocity.Length() > 1f)
					Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
				else
					Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			}
			else
			{
				Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
				Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;
			}

			Projectile.timeLeft = 2; // Makes sure the flail doesn't die (good when the flail is resting on the ground)
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2); //Add a delay so the player can't button mash the flail
			player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
			if (Projectile.Center.X < mountedCenter.X)
			{
				player.itemRotation += (float)Math.PI;
			}
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

			int dustRate = 10; 
			if (Main.rand.NextBool(dustRate))
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 150, default(Color), 1.3f);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			int defaultLocalNPCHitCooldown = 10;
			int impactIntensity = 0;
			Vector2 velocity = Projectile.velocity;
			float bounceFactor = 0.2f;
			if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet)
				bounceFactor = 0.4f;

			if (CurrentAIState == AIState.Dropping)
				bounceFactor = 0f;

			if (oldVelocity.X != Projectile.velocity.X)
			{
				if (Math.Abs(oldVelocity.X) > 4f)
					impactIntensity = 1;

				Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
				CollisionCounter += 1f;
			}

			if (oldVelocity.Y != Projectile.velocity.Y)
			{
				if (Math.Abs(oldVelocity.Y) > 4f)
					impactIntensity = 1;

				Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
				CollisionCounter += 1f;
			}

			// If in the Launched state, spawn sparks
			if (CurrentAIState == AIState.LaunchingForward)
			{
				CurrentAIState = AIState.Ricochet;
				Projectile.localNPCHitCooldown = defaultLocalNPCHitCooldown;
				Projectile.netUpdate = true;
				Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
				Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
				impactIntensity = 2;
				Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);
				Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
				Projectile.position -= velocity;
			}

			// Here the tiles spawn dust indicating they've been hit
			if (impactIntensity > 0)
			{
				Projectile.netUpdate = true;
				for (int i = 0; i < impactIntensity; i++)
				{
					Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
				}

				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			}

			// Force retraction if stuck on tiles while retracting
			if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f)
			{
				CurrentAIState = AIState.ForcedRetracting;
				Projectile.netUpdate = true;
			}

			return false;
		}

		public override bool? CanDamage()
		{
			// Flails in spin mode won't damage enemies within the first 12 ticks. Visually this delays the first hit until the player swings the flail around for a full spin before damaging anything.
			if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f)
				return false;
			return base.CanDamage();
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			// Flails do special collision logic that serves to hit anything within an ellipse centered on the player when the flail is spinning around the player. For example, the projectile rotating around the player won't actually hit a bee if it is directly on the player usually, but this code ensures that the bee is hit. This code makes hitting enemies while spinning more consistant and not reliant of the actual position of the flail projectile.
			if (CurrentAIState == AIState.Spinning)
			{
				Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
				Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
				shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
				float hitRadius = 55f; // The length of the semi-major radius of the ellipse (the long end)
				return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
			}
			// Regular collision logic happens otherwise.
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override void ModifyDamageScaling(ref float damageScale)
		{
			// Flails do 20% more damage while spinning
			if (CurrentAIState == AIState.Spinning)
				damageScale *= 1.2f;

			// Flails do 100% more damage while launched or retracting. This is the damage the item tooltip for flails aim to match, as this is the most common mode of attack. This is why the item has ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
			if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting)
				damageScale *= 2f;
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			// The hitDirection is always set to hit away from the player, even if the flail damages the npc while returning
			hitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);

			// Knockback is only 35% as powerful when in spin mode in 1.4.4
			if (CurrentAIState == AIState.Spinning)
				knockback *= 0.35f;
			// Knockback is only 50% as powerful when in drop down mode
			if (CurrentAIState == AIState.Dropping)
				knockback *= 0.5f;

			base.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0)
			{
				if (Projectile.friendly && !Projectile.hostile)
					ProjectileExtras.Explode(Projectile.whoAmI, 30, 30, () => { });

				SoundEngine.PlaySound(SoundID.Item109);

				for (int i = 0; i < 20; i++)
				{
					int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, -2f, 0, default, 2f);
					Main.dust[num].noGravity = true;
					Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].scale *= .25f;
					if (Main.dust[num].position != Projectile.Center)
						Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
				}

				int proj = Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center.X, target.Center.Y, 0, 0, ModContent.ProjectileType<GraniteSpike1>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
				Main.projectile[proj].timeLeft = 2;
				Projectile.Kill();
			}
		}

		// PreDraw is used to draw a chain and trail before the projectile is drawn normally.
		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

			// This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
			playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

			Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);

			Rectangle? chainSourceRectangle = null;
			// Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
			float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
			Vector2 chainDrawPosition = Projectile.Center;
			Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
			Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
			if (chainSegmentLength == 0)
				chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
			float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
			int chainCount = 0;
			float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

			// Chain drawing from the projectile to the player
			while (chainLengthRemainingToDraw > 0f)
			{
				// This code gets the lighting at the current tile coordinates
				Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

				// Here, we draw the chain texture at the coordinates
				Main.spriteBatch.Draw(chainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				// chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
				chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

			if (CurrentAIState == AIState.LaunchingForward)
			{
				Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
				Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
				SpriteEffects spriteEffects = SpriteEffects.None;
				if (Projectile.spriteDirection == -1)
					spriteEffects = SpriteEffects.FlipHorizontally;
				for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
				{
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
					Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
				}
			}
			return true;
		}
	}
}
