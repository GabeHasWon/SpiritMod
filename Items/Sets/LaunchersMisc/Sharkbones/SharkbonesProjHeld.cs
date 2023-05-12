using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.LaunchersMisc.Sharkbones
{
	public class SharkbonesProjHeld : Projectiles.BaseProj.BaseHeldProj
	{
		public override string Texture => "SpiritMod/Items/Sets/LaunchersMisc/Sharkbones/Sharkbones";

		public override void SetStaticDefaults() => DisplayName.SetDefault("Sharkbones");

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(26, 26);
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public override bool? CanDamage() => false;

		private ref float AiState => ref Projectile.ai[0];

		private const int STATE_CHARGEUP = 0;
		private const int STATE_LAUNCH = 1;
		private const int STATE_COOLDOWN = 2;

		private const int CHARGEUPTIME = 50;
		private const int COOLDOWNTIME = 30;
		private ref float AiTimer => ref Projectile.ai[1];
		public override void AbstractAI()
		{
			float recoilStrength = .5f;

			switch (AiState)
			{
				case STATE_CHARGEUP:
					if (ChannelKillCheck())
						break;

					if (AiTimer < CHARGEUPTIME * .7f && !Main.dedServ)
					{
						float quoteant = 1f - ((float)AiTimer / CHARGEUPTIME);
						Vector2 dustPos = Projectile.Center + (Projectile.velocity * (quoteant * 80f * Main.rand.NextFloat(0.7f, 1.0f))).RotatedByRandom(quoteant * 1.3f);
						Vector2 Velocity(int timeLeft) => dustPos.DirectionTo(Projectile.Center) * (dustPos.Distance(Projectile.Center) / timeLeft);

						ParticleHandler.SpawnParticle(new FireParticle(dustPos, Velocity(25), Color.DarkCyan, Color.Cyan, Main.rand.NextFloat(0.35f, 0.5f), 25, delegate (Particle particle) { particle.Position += Owner.velocity; }));
						Dust.NewDustPerfect(dustPos, DustID.Electric, Velocity(10), 0, default, .5f).noGravity = true;
					}

					if (AiTimer > CHARGEUPTIME)
						SetAIState(STATE_LAUNCH);
					break;
				case STATE_LAUNCH:
					bool canShoot = Owner.PickAmmo(ContentSamples.ItemsByType[ItemID.RocketLauncher], out int shoot, out float speed, out int damage, out float knockback, out int ammo); //first pickammo to find stats and actually consume the ammo

					if (!Main.dedServ)
					{
						for (int i = 0; i < 12; i++)
						{
							float strength = Main.rand.NextFloat();
							ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, (Projectile.velocity * 8f * strength).RotatedByRandom(.25f), Color.Lerp(new Color(144, 80, 240), Color.DarkCyan, strength) with { A = 0 }, 1f - strength, 30));
							if (i < 6)
								ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center, (Projectile.velocity * Main.rand.NextFloat(8f)).RotatedByRandom(.4f), Color.LightBlue, Color.Purple, Main.rand.NextFloat(0.2f, 0.5f), 20));

							Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, (Projectile.velocity * Main.rand.NextFloat(10f)).RotatedByRandom(.5f), 100, Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat()), .8f).noGravity = true;
						}

						SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
						SoundEngine.PlaySound(SoundID.Item14 with { PitchVariance = 0.4f, Volume = 0.4f }, Projectile.Center);
					}

					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * (speed + Owner.HeldItem.shootSpeed), ModContent.ProjectileType<SharkbonesRocket>(), damage + Projectile.damage, knockback + Projectile.knockBack, Owner.whoAmI);

					Projectile.velocity = Projectile.velocity.RotatedBy(recoilStrength * -Projectile.direction);
					
					canShoot = Owner.PickAmmo(Owner.HeldItem, out _, out _, out _, out _, out int _, true); //second to determine if another shot can be fired
					if (!canShoot) //if no ammo left, stop channelling, so weapon use can end when wanted
						Owner.channel = false;

					SetAIState(STATE_COOLDOWN);
					break;
				case STATE_COOLDOWN:
					if (AiTimer > COOLDOWNTIME)
						SetAIState(STATE_CHARGEUP);

					float readjustment = recoilStrength * 1.12f;
					Projectile.velocity = Projectile.velocity.RotatedBy(readjustment / COOLDOWNTIME * Projectile.direction); //readjust back to what direction was before shot
					
					if (!Main.dedServ && Main.rand.NextBool())
						ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, (-Vector2.UnitY).RotatedByRandom(.5f), new Color(60, 60, 60) * 0.5f, Main.rand.NextFloat(0.2f, 0.25f), 25));

					break;
			}
			++AiTimer;

			float rotation = Projectile.rotation - (1.57f * Projectile.direction) + (.5f * Owner.direction);
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
			Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, rotation);
		}

		public override bool AutoAimCursor() => AiState != STATE_COOLDOWN;
		public override Vector2 HoldoutOffset() => new Vector2(38 - ((AiState == STATE_COOLDOWN) ? (1f - ((float)AiTimer / COOLDOWNTIME)) * 20f : 0), 0).RotatedBy(Projectile.velocity.ToRotation());

		private void SetAIState(int State)
		{
			AiState = State;
			AiTimer = 0;
			Projectile.netUpdate = true;
		}

		public override float CursorLerpSpeed() => .75f;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 origin = (effects == SpriteEffects.FlipHorizontally) ? new Vector2(Projectile.width / 2, texture.Height - (Projectile.height / 2)) : texture.Size() - (Projectile.Size / 2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, effects, 0);

			return false;
		}
	}
}