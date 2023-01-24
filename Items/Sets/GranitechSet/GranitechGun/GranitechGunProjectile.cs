using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GranitechSet.GranitechGun
{
	public class GranitechGunProjectile : ModProjectile
	{
		private int _charge = 0;
		private int _endCharge = -1;
		private float _finalRotation = 0f;
		private SpriteEffects _effect = SpriteEffects.None; //So there's no wackies with the semiauto mode

		const int ChargeUp = 16; //How long it takes to start up

		public override void SetStaticDefaults() => DisplayName.SetDefault("Vector .109");

		public override void SetDefaults()
		{
			Projectile.width = 56;
			Projectile.height = 36;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = -1;
			DrawHeldProjInFrontOfHeldItemAndArms = false;
		}

		public override bool? CanDamage() => false;

		public override void AI()
		{
			Player p = Main.player[Projectile.owner];
			p.heldProj = Projectile.whoAmI;

			if (p == Main.LocalPlayer)
			{
				if (p.channel) //Use turn functionality
					p.ChangeDir(Main.MouseWorld.X >= p.MountedCenter.X ? 1 : -1);

				Projectile.timeLeft++; //dont die
				Projectile.rotation = Vector2.Normalize(p.MountedCenter - Main.MouseWorld).ToRotation() - MathHelper.Pi; //So it looks like the player is holding it properly
				Projectile.netUpdate = true;
			}
			Projectile.spriteDirection = p.direction;
			p.itemRotation = MathHelper.WrapAngle(Projectile.rotation + ((Projectile.spriteDirection < 0) ? MathHelper.Pi : 0));

			_charge++; //Increase charge timer...

			if (_endCharge == -1) //Wait until the player has fired to let go & set position
			{
				p.itemTime = 2;
				p.itemAnimation = 2;
				Projectile.Center = p.Center - new Vector2(27, 0).RotatedBy(Projectile.rotation - MathHelper.Pi) + new Vector2(28, 18 + p.gfxOffY);

				if (!p.channel) //the player has stopped shooting
				{
					_endCharge = _charge;
					_finalRotation = Projectile.rotation - MathHelper.Pi;
					_endCharge += p.HeldItem.useAnimation;
					_effect = Main.MouseWorld.X >= p.MountedCenter.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

					Projectile.netUpdate = true;
				}
			}
			else
				Projectile.Center = p.Center - (new Vector2(1, 0).RotatedBy(_finalRotation) * 27) + new Vector2(28, 18 + p.gfxOffY);

			if (_charge > _endCharge && _endCharge != -1) //Kill projectile when done shooting
				Projectile.Kill();

			if (_charge > ChargeUp && (_charge - ChargeUp) % p.HeldItem.useTime == 1)
				Fire(p);
		}

		private void Fire(Player player)
		{
			if (player.PickAmmo(player.HeldItem, out int _, out float _, out int _, out float _, out int _))
			{
				if (Main.myPlayer == Projectile.owner)
				{
					var baseVel = player.DirectionTo(Main.MouseWorld).RotatedByRandom(0.02f) * player.HeldItem.shootSpeed;

					Vector2 pos = player.Center;
					Vector2 muzzleOffset = Vector2.Normalize(baseVel) * (player.HeldItem.width / 2f);
					if (Collision.CanHit(pos, 0, 0, pos + muzzleOffset, 0, 0))
						pos += muzzleOffset;

					var proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, baseVel, ModContent.ProjectileType<GranitechGunBullet>(), player.HeldItem.damage, 0f, player.whoAmI);
					var p = Main.projectile[proj];

					if (p.ModProjectile is GranitechGunBullet bullet)
						bullet.spawnRings = true;

					if (Main.netMode != NetmodeID.SinglePlayer)
						NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);

					if (!Main.dedServ)
						SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/EnergyShoot") with { PitchVariance = 0.1f, Volume = 0.25f }, pos);

					VFX(pos + muzzleOffset, baseVel * 0.2f);
				}
			}
			else
			{
				player.channel = false;
				Projectile.Kill();
			}
		}

		private static void VFX(Vector2 position, Vector2 velocity)
		{
			for (int i = 0; i < 6; ++i)
			{
				Vector2 vel = velocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(2f, 6f);
				ParticleHandler.SpawnParticle(new GranitechParticle(position, vel, Main.rand.NextBool(2) ? new Color(222, 111, 127) : new Color(239, 241, 80), Main.rand.NextFloat(2.5f, 3f), 20));
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player p = Main.player[Projectile.owner];
			Texture2D t = TextureAssets.Projectile[Projectile.type].Value;

			_effect = Main.MouseWorld.X >= p.MountedCenter.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			float realRot = GetRotation();

			Vector2 drawPos = Projectile.position - Main.screenPosition; //Draw position + charge shaking

			const int Width = 82;
			const int Height = 38;
			const int MuzzleFlashDuration = 2;

			var frame = new Rectangle(0, 0, Width, Height);

			if (_charge > ChargeUp && (_charge - ChargeUp) % p.HeldItem.useTime < MuzzleFlashDuration)
			{
				int offset = (_charge - ChargeUp) % (p.HeldItem.useTime * 3);
				int variation = offset / p.HeldItem.useTime;

				frame = new Rectangle(0, Height * (4 + (variation * 2)), Width, Height);
			}
			else if (_charge > ChargeUp && (_charge - ChargeUp) % p.HeldItem.useTime >= MuzzleFlashDuration)
			{
				int offset = (_charge - ChargeUp) % (p.HeldItem.useTime * 3);
				int variation = offset / p.HeldItem.useTime;

				frame = new Rectangle(0, Height * (5 + (variation * 2)), Width, Height);
			}

			if (_charge < ChargeUp / 4)
				frame = new Rectangle(0, 0, Width, Height);
			else if (_charge < ChargeUp / 2)
				frame = new Rectangle(0, Height, Width, Height);
			else if (_charge < ChargeUp / 1.5f)
				frame = new Rectangle(0, Height * 2, Width, Height);
			else if (_charge <= ChargeUp)
				frame = new Rectangle(0, Height * 3, Width, Height);

			Main.spriteBatch.Draw(t, drawPos, frame, lightColor, realRot, new Vector2(42, 22), 1f, _effect, 1f);

			Texture2D glowmask = ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Main.spriteBatch.Draw(glowmask, drawPos, frame, Color.White, realRot, new Vector2(42, 22), 1f, _effect, 1f);

			return false;
		}

		private float GetRotation()
		{
			float rot = Projectile.rotation; //Rotate towards mouse
			if (_endCharge != -1) rot = _finalRotation + MathHelper.Pi;
			if (_effect == SpriteEffects.FlipHorizontally)
				rot -= MathHelper.Pi;
			return rot;
		}

		public override void Kill(int timeLeft) //paranoia - I don't know if this is necessary
		{
			_endCharge = -1;
			_finalRotation = 0;
		}
	}
}
