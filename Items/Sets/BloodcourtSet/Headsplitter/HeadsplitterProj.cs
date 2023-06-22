using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using SpiritMod.NPCs.Boss.Occultist.Particles;
using SpiritMod.Particles;
using SpiritMod.Projectiles.BaseProj;
using SpiritMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BloodcourtSet.Headsplitter
{
	public class HeadsplitterProj : BaseHeldProj
	{
		private int AiState
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private const int STATE_NORMAL = 0;
		private const int STATE_HEAVY = 1;
		private const int STATE_SUPER = 2;

		private bool collided = false;

		private ref float Timer => ref Projectile.ai[1];

		private Vector2 initialVelocity = Vector2.Zero;
		private int swingDirection = 1;

		private readonly Vector2 initialSize = new(60);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Headsplitter");
			ProjectileID.Sets.TrailCacheLength[Type] = 5;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Melee;
			Projectile.Size = initialSize;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override bool AutoAimCursor() => false;

		public override Vector2 HoldoutOffset() => Projectile.velocity * Projectile.Size.Length() / 2 * Math.Min(1, Projectile.scale);

		private float SwingRadians => (AiState != STATE_NORMAL) ? ((AiState == STATE_SUPER) ? 8f : MathHelper.TwoPi) : MathHelper.Pi;

		public override bool PreAI()
		{
			bool firstTick = Projectile.timeLeft > 2;
			if (firstTick)
			{
				swingDirection = Math.Sign(Projectile.velocity.X);
				initialVelocity = (Vector2.UnitX * Owner.direction).RotatedBy(swingDirection * -.3f);
				Projectile.scale = 0f;
			}
			Projectile.scale = Math.Min(1, Projectile.scale + .1f);

			if (Timer < (Owner.itemTimeMax - 8)) //Do fancy dusts
			{
				float magnitude = (AiState == STATE_NORMAL) ? 1f : 3f;

				Vector2 dustPos = Projectile.Center + (Vector2.UnitX * 30 * magnitude).RotatedBy(Projectile.velocity.ToRotation()) + (Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 8f * magnitude);
				Dust dust = Dust.NewDustPerfect(dustPos, DustID.LavaMoss, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.5f, 1.0f));
				dust.velocity = (Vector2.UnitX * Main.rand.NextFloat() * 2f * magnitude).RotatedBy(Projectile.velocity.ToRotation() + (1.57f * swingDirection)).RotatedByRandom(.3f * magnitude);
				dust.noGravity = true;
				dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
			}

			if (AiState != STATE_NORMAL)
			{
				float quoteant = Math.Min(1, Timer / (Owner.itemTimeMax * .4f));
				Projectile.Size = Vector2.Lerp(initialSize, initialSize * 1.5f, quoteant);
				Projectile.scale = quoteant + 1;

				if (AiState == STATE_SUPER && !collided)
				{
					if (Timer == 0 && Main.netMode != NetmodeID.Server)
						SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/PowerSlash1"), Projectile.Center);
					if (Timer >= (Owner.itemTimeMax / 2) && Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height, true))
					{
						collided = true;
						Timer = 0;
						Owner.GetModPlayer<MyPlayer>().Shake = (int)MathHelper.Min(30, Owner.GetModPlayer<MyPlayer>().Shake + 15);

						Vector2 dustPos = Projectile.Center + (Vector2.UnitX * ((Projectile.Size.Length() / 2) - 30)).RotatedBy(Projectile.velocity.ToRotation()) - (Vector2.UnitY * 20);
						for (int k = 0; k <= 80; k++)
							Dust.NewDustPerfect(dustPos, ModContent.DustType<Dusts.EarthDust>(), Vector2.UnitY.RotatedByRandom(1) * Main.rand.NextFloat(-1.0f, 1.0f) * 5f);

						SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
						SoundEngine.PlaySound(SoundID.NPCHit42, Projectile.Center);
					}
				}
				if (AiState == STATE_HEAVY && Timer == 0 && Main.netMode != NetmodeID.Server)
					SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, Projectile.Center);
			}

			float progress = ++Timer / Owner.itemTimeMax;
			progress = EaseFunction.EaseCircularInOut.Ease(progress);

			if (!collided)
				Projectile.velocity = initialVelocity.RotatedBy(MathHelper.Lerp(SwingRadians / 2 * swingDirection, -SwingRadians / 2 * swingDirection, progress));

			float rotation = Projectile.velocity.ToRotation() - 1.57f * Owner.gravDir;
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
			Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, rotation);
			return true;
		}

		public override void AbstractAI()
		{
			if (initialVelocity.X == Owner.direction)
				Projectile.direction = Projectile.spriteDirection *= -1;

			Owner.reuseDelay = Owner.HeldItem.reuseDelay;

			if (Timer > Owner.itemTimeMax)
				Projectile.Kill();
		}

		public override bool? CanDamage() => collided ? false : null;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center, Projectile.Center) ? true : base.Colliding(projHitbox, targetHitbox);

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			hitDirection = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);

			if (AiState != STATE_NORMAL && target.HasBuff(ModContent.BuffType<SurgingAnguish>()))
				damage = (int)(damage * 1.5f);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Vector2 hitPos = target.getRect().ClosestPointInRect(Projectile.Center);

			if (AiState != STATE_NORMAL)
			{
				Owner.GetModPlayer<MyPlayer>().Shake = (int)MathHelper.Min(20, Owner.GetModPlayer<MyPlayer>().Shake + 5);

				ParticleHandler.SpawnParticle(new OccultistDeathBoom(hitPos, Main.rand.NextFloat(0.5f, 1.0f), Main.rand.NextFloatDirection()));
				for (int i = 0; i < 15; i++)
				{
					Dust dust = Dust.NewDustPerfect(hitPos, DustID.LavaMoss, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.5f, 3.0f));
					dust.velocity = (Vector2.UnitX * Main.rand.NextFloat() * 12f).RotatedBy(Projectile.velocity.ToRotation() + (1.57f * swingDirection)).RotatedByRandom(.8f);
					dust.noGravity = true;
					dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
				}
				if (Main.netMode != NetmodeID.Server)
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Slash1") with { PitchVariance = .5f, Volume = .5f }, Projectile.Center);

				if (AiState == STATE_HEAVY)
					Timer -= 2; //Stagger
			}
			else
			{
				for (int i = 0; i < 15; i++)
				{
					Vector2 velocity = (Vector2.UnitX * Main.rand.NextFloatDirection() * 8f).RotatedBy(Projectile.velocity.ToRotation() + (1.57f * swingDirection)).RotatedByRandom(.3f);
					Dust dust = Dust.NewDustPerfect(hitPos, Main.rand.NextBool() ? DustID.GemRuby : DustID.LavaMoss, velocity, 0, Color.White, Main.rand.NextFloat(0.5f, 1.5f));
					dust.noGravity = true;
					dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
				}
				if (Main.rand.NextBool(3))
				{
					if (!Main.dedServ)
						ParticleHandler.SpawnParticle(new SplitterImpact(hitPos, 1f, ((swingDirection == -1) ? MathHelper.Pi : 0f) - (hitPos.DirectionTo(Main.player[Projectile.owner].Center).X / 5)));

					target.AddBuff(ModContent.BuffType<SurgingAnguish>(), 180);
				}

				Owner.GetModPlayer<HeadsplitterPlayer>().charge = Math.Min(Owner.GetModPlayer<HeadsplitterPlayer>().chargeMax, Owner.GetModPlayer<HeadsplitterPlayer>().charge + (damage / 2));
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D trail = Mod.Assets.Request<Texture2D>("Items/Sets/BloodcourtSet/Headsplitter/Trail").Value;
			SpriteEffects effects = (swingDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			float quoteant = (float)((float)Timer / Owner.itemAnimationMax);
			Color color = Color.Lerp(Color.Magenta, Color.Red, quoteant) with { A = 0 } * Math.Min(1, Timer / 10);
			Vector2 scale = new Vector2(.27f, (1f - quoteant) * .3f) * Projectile.scale;

			if (AiState != STATE_NORMAL)
				lightColor = color;

			float rotation = Projectile.velocity.ToRotation() + ((effects == SpriteEffects.FlipHorizontally) ? 3.14f : 0);
			Main.EntitySpriteDraw(trail, Projectile.Center + (Vector2.UnitX * 10).RotatedBy(Projectile.velocity.ToRotation()) - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, new Vector2(trail.Width / 2, trail.Height), scale, effects, 0);

			rotation += .785f * ((effects == SpriteEffects.FlipHorizontally) ? -1 : 1);
			Projectile.QuickDraw(null, rotation, effects, Projectile.GetAlpha(lightColor));
			Projectile.QuickDrawGlowTrail(null, 1f, Color.White, rotation, effects);

			return false;
		}
	}
}