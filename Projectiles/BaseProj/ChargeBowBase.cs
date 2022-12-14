using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.IO;
using Terraria.Audio;

namespace SpiritMod.Projectiles.BaseProj
{
	public abstract class ChargeBowProj : ModProjectile
	{
		public sealed override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
			Projectile.timeLeft = 999999;
			SafeSetDefaults();
			SetBowDefaults();
		}

		protected int AmmoType
		{
			get
			{
				return (int)Projectile.ai[0];
			}
			set
			{
				Projectile.ai[0] = value;
			}
		}
		protected virtual void SafeSetDefaults() {}
		protected virtual void SetBowDefaults() {}
		protected virtual void SafeAI() {}
		protected virtual void Shoot(bool firstFire) {}
		protected virtual void Charging() => AdjustDirection();
		
		protected int minDamage;
		protected int maxDamage;
		protected int minVelocity;
		protected int maxVelocity;
		protected int predictor;
		protected float chargeRate;
		protected float dechargeRate;
		protected SoundStyle soundtype = SoundID.Item5 with { PitchVariance = 0.2f };

		protected float charge = 0;
		protected bool firing = false;
		protected Vector2 direction = Vector2.Zero;
		int counter = 0;

		public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(direction);

		public override void ReceiveExtraAI(BinaryReader reader) => direction = reader.ReadVector2();

		public sealed override void AI()
		{
			Projectile.velocity = Vector2.Zero;
			counter++;
			SafeAI();
			AdjustDirection();
			Player player = Main.player[Projectile.owner];
			player.ChangeDir(Main.MouseWorld.X > player.position.X ? 1 : -1);
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			Projectile.timeLeft = Math.Min(Projectile.timeLeft, player.HeldItem.useAnimation);
			Projectile.position = player.MountedCenter;
			if (player.channel && !firing) 
			{
				Projectile.timeLeft = Math.Max(Projectile.timeLeft, 2);
				if (charge < 1)
				{
					if ((charge + chargeRate) >= 1)
						SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
					charge+=chargeRate;
				}
				Charging();
				if (predictor != -1 && counter % 5 == 0)
				{
					float velocity = LerpFloat(minVelocity, maxVelocity, charge);
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, direction * velocity, predictor, 0, 0, Projectile.owner);
				}
			}
			else
			{
				Shoot(!firing);
				firing = true;
			}
		}

		//helpers
		protected void AdjustDirection(float deviation = 0f)
		{
			Player player = Main.player[Projectile.owner];
			if (Main.myPlayer == player.whoAmI) {
				direction = Main.MouseWorld - (player.Center - new Vector2(4, 4));
				direction.Normalize();
				direction = direction.RotatedBy(deviation);
				Projectile.netUpdate = true;
			}
			player.itemRotation = direction.ToRotation();
			if (player.direction != 1)
			{
				player.itemRotation -= 3.14f;
			}
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
		}

		protected int CreateArrow()
		{
			Player player = Main.player[Projectile.owner];
			float velocity = LerpFloat(minVelocity, maxVelocity, charge);
			int damage = (int)(player.GetDamage(DamageClass.Ranged).ApplyTo(LerpFloat(minDamage, maxDamage, charge))) + Projectile.damage;
			SoundEngine.PlaySound(soundtype, Projectile.Center);
			return Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.Center, direction * velocity, AmmoType, damage, Projectile.knockBack, Projectile.owner, (charge >= 1) ? 1 : 0);
		}
		protected static float LerpFloat(float min, float max, float val)
		{
			float difference = max-min;
			return min + (difference * val);
		}
	}
}
