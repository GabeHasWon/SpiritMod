using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.FlailsMisc
{
	public abstract class BaseFlailItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Melee;
			Item.channel = true;
			Item.UseSound = SoundID.Item19;
			SafeSetDefaults();
		}

		public virtual void SafeSetDefaults() { }

		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
	}

	public abstract class BaseFlailProj : ModProjectile
	{
		internal int State
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		internal const int SPINNING = 0;
		internal const int LAUNCHING = 1;
		internal const int RETRACTING = 2;
		internal const int FALLING = 3;

		internal bool struckTile = false;

		internal int Timer
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public int launchDist;
		public float spinDist;
		public float spinSpeed;
		public int momentum;

		internal int LaunchTime => (int)(launchDist / GetLaunchSpeed(Owner));

		protected Player Owner => Main.player[Projectile.owner];

		/// <summary>
		/// 
		/// </summary>
		/// <param name="launchDist">The maximum distance (in pixels) that the projectile can travel before returning</param>
		/// <param name="spinDist">Controls the distance of the projectile from the player while spinning</param>
		/// <param name="spinSpeed">Controls how quickly the projectile spins around the player</param>
		/// <param name="momentum">Determines how long (in ticks) the projectile will take to reach max spinning speed</param>
		public BaseFlailProj(int launchDist = 150, float spinDist = 40f, float spinSpeed = 32f, int momentum = 0)
		{
			this.launchDist = launchDist;
			this.spinDist = spinDist;
			this.spinSpeed = spinSpeed;
			this.momentum = momentum;
		}

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.netImportant = true;
			Projectile.timeLeft = 2;
			Projectile.penetrate = -1;
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(struckTile);

		public override void ReceiveExtraAI(BinaryReader reader) => struckTile = reader.ReadBoolean();

		public override void AI()
		{
			if (!Owner.dead && Owner.active && !Owner.CCed)
			{
				Owner.itemTime = 2;
				Owner.itemAnimation = 2;
				Owner.heldProj = Projectile.whoAmI;
				Projectile.timeLeft = 2;
			}

			Timer++;
			//modify multiplier effects when momentum is greater than 0
			float multiplier = (momentum > 0) ? MathHelper.Min((float)Timer / momentum, 1.0f) : 1.0f;

			if (State == SPINNING) //spinning around the player in an ellipse
			{
				Projectile.velocity = Vector2.Zero;
				Projectile.tileCollide = false;

				float radians = MathHelper.ToRadians(Timer * (spinSpeed * multiplier)) * Owner.direction;
				Vector2 spinningoffset = new Vector2(spinDist, 0).RotatedBy(radians);
				spinningoffset.Y *= .5f;

				Projectile.Center = Owner.MountedCenter + spinningoffset;
				if (Owner.whoAmI == Main.myPlayer)
					Owner.ChangeDir(Math.Sign(Main.MouseWorld.X - Owner.Center.X));

				SpinExtras(Owner);

				if (!Owner.channel) //check to see if player stops channelling
				{
					if (Owner.whoAmI == Main.myPlayer) //initial launch
					{
						SoundEngine.PlaySound(SoundID.Item19, Projectile.Center);
						Projectile.Center = Owner.MountedCenter;
						Projectile.velocity = Owner.DirectionTo(Main.MouseWorld) * (GetLaunchSpeed(Owner) * multiplier) + Owner.velocity;

						OnLaunch(Owner);
					}

					launchDist = (int)(launchDist * multiplier);
					Timer = 0;
					State = LAUNCHING;

					Projectile.netUpdate = true;
				}
			}
			else
			{
				bool hasGravity = false;
				if (State == LAUNCHING) //basic flail launch, returns after a while
				{
					Projectile.tileCollide = true;
					hasGravity = struckTile;

					if (Projectile.Distance(Owner.MountedCenter) >= launchDist || Timer >= LaunchTime) //return to the player
					{
						State = RETRACTING;
						Projectile.velocity *= 0.3f;

						Projectile.netUpdate = true;
					}

					LaunchExtras(Owner);
				}
				else if (State == RETRACTING)
				{
					Projectile.tileCollide = false;

					Projectile.velocity *= 0.98f;
					Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(Owner.MountedCenter) * GetLaunchSpeed(Owner), 3f);

					if (Projectile.Hitbox.Intersects(Owner.Hitbox)) //kill the projectile on contact with the player
						Projectile.Kill();

					ReturnExtras(Owner);
				}

				if (State == FALLING)
				{
					FallingExtras(Owner);
					Projectile.tileCollide = true;
					hasGravity = true;

					bool outOfRange = Owner.Distance(Projectile.Center) > (launchDist * 2);
					if (!Owner.controlUseItem || outOfRange)
					{
						if (outOfRange)
							Projectile.velocity *= 0.5f;

						State = RETRACTING;
						struckTile = true;

						Projectile.netUpdate = true;
					}
				}
				else if (Owner.controlUseItem && !struckTile) //prompt the flail to fall
				{
					State = FALLING;
					Projectile.velocity *= 0.2f;
					Timer = LaunchTime;

					Projectile.netUpdate = true;
				}

				if (hasGravity)
				{
					Projectile.velocity.Y += 0.8f;
					Projectile.velocity.X *= 0.95f;
				}

				Owner.ChangeDir(Math.Sign(Projectile.Center.X - Owner.Center.X));
				NotSpinningExtras(Owner);
			}

			//handle projectile rotation
			if (State != FALLING)
				Projectile.rotation = Projectile.AngleFrom(Owner.MountedCenter) - 1.57f;
			else
			{
				int clamp = (int)MathHelper.Clamp(Projectile.velocity.Length(), 0, 1);
				Projectile.rotation = (Projectile.rotation * (1 - clamp)) + (Projectile.velocity.ToRotation() * clamp) + Projectile.velocity.X * 0.1f;
			}

			Owner.itemRotation = MathHelper.WrapAngle(Projectile.AngleFrom(Owner.MountedCenter) - ((Owner.direction < 0) ? MathHelper.Pi : 0));
		}

		#region extra hooks
		public virtual void SpinExtras(Player player) { }

		public virtual void NotSpinningExtras(Player player) { }

		public virtual void OnLaunch(Player player) { }

		public virtual void LaunchExtras(Player player) { }

		public virtual void FallingExtras(Player player) { }

		public virtual void ReturnExtras(Player player) { }

		public virtual void FallingTileCollide(Vector2 oldVelocity) { }

		public virtual void SafeTileCollide(Vector2 oldVelocity, bool highImpact)
		{
			if (highImpact)
			{
				Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
				SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			}
		}

		public virtual float GetLaunchSpeed(Player player) => player.HeldItem.shootSpeed * (1f / player.GetAttackSpeed(DamageClass.Melee));
		#endregion

		public override void ModifyDamageScaling(ref float damageScale)
		{
			damageScale *= State switch
			{
				SPINNING => 1.2f,
				FALLING => 1.0f,
				_ => 2.0f
			};
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			knockback *= State switch
			{
				SPINNING => 0.35f,
				_ => 1.0f
			};

			hitDirection = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);
		}

		public override bool? CanDamage() => (State == SPINNING && Timer <= 12f) ? false : base.CanDamage(); //Don't hit targets in the first 12 ticks

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (State == SPINNING)
			{
				Vector2 toTarget = (targetHitbox.ClosestPointInRect(Owner.MountedCenter) - Owner.MountedCenter) / 0.8f;
				float hitRadius = spinDist + 15f;
				return toTarget.Length() <= hitRadius;
			}
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (State == FALLING)
				FallingTileCollide(oldVelocity);

			float bounceFactor = (State == FALLING) ? 0.0f : 0.4f;
			if (oldVelocity.X != Projectile.velocity.X)
				Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
			if (oldVelocity.Y != Projectile.velocity.Y)
				Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;

			struckTile = true;

			SafeTileCollide(oldVelocity, oldVelocity.Length() > 4f);
			return false;
		}

		public override bool PreDrawExtras()
		{
			Texture2D ChainTexture = Mod.Assets.Request<Texture2D>(Texture.Remove(0, Mod.Name.Length + 1) + "_Chain").Value;

			Vector2 armPos = Main.GetPlayerArmPosition(Projectile);
			armPos.Y -= Owner.gfxOffY;

			int numDraws = Math.Max((int)(Projectile.Distance(armPos) / ChainTexture.Height), 1);
			for (int i = 0; i < numDraws; i++)
			{
				Vector2 chaindrawpos = Vector2.Lerp(armPos, Projectile.Center, i / (float)numDraws);
				Color lightColor = Lighting.GetColor((int)chaindrawpos.X / 16, (int)chaindrawpos.Y / 16);

				Main.EntitySpriteDraw(ChainTexture, chaindrawpos - Main.screenPosition, null, lightColor, Projectile.AngleFrom(Owner.MountedCenter) + 1.57f, ChainTexture.Size() / 2, 1f, SpriteEffects.None, 0);
			}
			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
