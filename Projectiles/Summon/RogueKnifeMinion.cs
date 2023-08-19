using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Accessory;
using SpiritMod.Items.Sets.DashSwordSubclass.AnimeSword;
using SpiritMod.Projectiles.BaseProj;
using SpiritMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace SpiritMod.Projectiles.Summon
{
	public class RogueKnifeMinion : BaseMinion
	{
		public RogueKnifeMinion() : base(500, 900, new Vector2(12, 12)) { }

		public override void AbstractSetStaticDefaults()
		{
			DisplayName.SetDefault("Rusted Sword");
			Main.projFrames[Type] = 6;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void AbstractSetDefaults()
		{
			Projectile.minionSlots = 0f;
			Projectile.localNPCHitCooldown = attackCooldown;
			Projectile.extraUpdates = 1;
		}

		public override bool PreAI()
		{
			Player mp = Main.player[Projectile.owner];

			if (Projectile.damage == 0) //This shouldn't happen
				Projectile.damage = (int)Main.player[Projectile.owner].GetDamage(Projectile.DamageType).ApplyTo(5);

			if (mp.HasAccessory<RogueCrest>())
				Projectile.timeLeft = 2;

			return true;
		}

		public override void AI()
		{
			animate = false;

			base.AI();

			if (animate || Projectile.frame != 0)
			{
				if (++Projectile.frameCounter >= 6)
				{
					Projectile.frameCounter = 0;
					Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
				}
			}

			if (AiTimer <= (attackCooldown - 5) && trail != null && !trail.Destroyed)
				trail.Destroyed = true;

			AiTimer = Math.Max(0, AiTimer - 1);
		}

		private bool Trailing => (Projectile.velocity.Length() >= ProjectileID.Sets.TrailCacheLength[Projectile.type]) && (AiState == Attacking);
		private bool animate = false;

		private AnimePrimTrail trail;

		private const int Returning = 0;
		private const int Attacking = 1;
		private const int LockedToPlayer = 2;

		private readonly int attackCooldown = 30;

		private ref float AiTimer => ref Projectile.ai[0];
		private ref float AiState => ref Projectile.ai[1];

		public override void IdleMovement(Player player)
		{
			Vector2 desiredPos = player.Center + new Vector2(0, -60 + (float)Math.Sin(Main.GameUpdateCount / 30f) * 5);

			AiTimer = 0;

			Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0, 0.07f);

			if (AiState != LockedToPlayer && Projectile.Distance(desiredPos) > 25)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(desiredPos) * 6, 0.1f);

				AiState = Returning;
			}
			else
			{
				AiState = LockedToPlayer;
				Projectile.extraUpdates = 0;
				Projectile.velocity = Vector2.Zero;
				Projectile.Center = desiredPos;
			}
		}

		public override void TargettingBehavior(Player player, NPC target)
		{
			AiState = Attacking;

			Projectile.extraUpdates = 1;
			Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.AngleTo(target.Center) + 1.57f, 0.2f);

			if (Projectile.Distance(target.Center) > 120) //Move closer to the target
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 8, 0.08f);
			}
			else
			{
				CanRetarget = AiTimer <= 1;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionFrom(target.Center) * 5, 0.04f);

				if (AiTimer <= 0 && (Projectile.Distance(target.Center) > 30)) //Lunge, then go on cooldown
				{
					if (trail != null && !trail.Destroyed)
						trail.Destroyed = true;
					if (Main.netMode != NetmodeID.Server)
						SpiritMod.primitives.CreateTrail(trail = new AnimePrimTrail(Projectile));

					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/SwordSlash1") with { Pitch = 1.25f }, Projectile.Center);

					AiTimer = attackCooldown;
					Projectile.velocity = (Projectile.DirectionTo(target.Center) * 28).RotatedByRandom(0.1f);

					for (int i = 0; i < 10; i++)
					{
						Dust dust = Dust.NewDustPerfect(Projectile.Center + (Projectile.velocity * Main.rand.NextFloat(0.5f, 1.0f)) + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.0f, 4.0f)), DustID.SilverFlame, Projectile.velocity * Main.rand.NextFloat(0.1f, 0.5f), 100, default, 2f);
						dust.noGravity = true;
						dust.fadeIn = 1.2f;
					}

					Projectile.netUpdate = true;
				}
				animate = true;
			}
		}

		public override bool DoAutoFrameUpdate(ref int framespersecond, ref int startframe, ref int endframe) => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle drawFrame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Type]);
			Vector2 origin = new Vector2(drawFrame.Width / 2, Projectile.height / 2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawFrame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			if (Trailing)
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
					Main.EntitySpriteDraw(texture, drawPos, drawFrame, color * .6f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
				}
			}

			return false;
		}
	}
}