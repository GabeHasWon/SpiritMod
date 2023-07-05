using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Summon;
using SpiritMod.Projectiles.BaseProj;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace SpiritMod.Items.Sets.SummonsMisc.RodofDunes
{
	[AutoloadMinionBuff("Sand Warrior", "The sand warrior will fight for you")]
	public class SandWarriorMinion : BaseMinion
	{
		private ref float AiState => ref Projectile.ai[0];

		private const int STATE_IDLING = 0;
		private const int STATE_STILL = 1;
		private const int STATE_AGGROED = 2;
		private const int STATE_SAND = 3;

		public SandWarriorMinion() : base(600, 1200, new Vector2(30, 38)) { }

		public override void AbstractSetStaticDefaults()
		{
			DisplayName.SetDefault("Sand Warrior");
			Main.projFrames[Type] = 4;
		}

		public override void AbstractSetDefaults() => Projectile.tileCollide = true;

		public override bool DoAutoFrameUpdate(ref int framespersecond, ref int startframe, ref int endframe)
		{
			framespersecond = 15;
			return true;
		}

		public override bool MinionContactDamage() => AiState == STATE_AGGROED;

		public override bool PreAI()
		{
			if (Projectile.Distance(Player.Center) > 900)
				AiState = STATE_SAND;
			if (AiState == STATE_SAND)
			{
				Projectile.alpha = 255;
				Projectile.tileCollide = false;
				SandBehaviour();
			}
			else
			{
				if (Projectile.velocity != Vector2.Zero)
				{
					float throwaway = 6;
					Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref throwaway, ref Projectile.gfxOffY);
				}
				Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0) ? 1 : -1;

				int dustAmt = (int)Math.Min(Projectile.velocity.Length(), 11);
				if (Main.rand.NextBool(12 - dustAmt) && Projectile.velocity.Length() > 1.5f)
					Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, Projectile.velocity.X / 3, Projectile.velocity.Y / 3, 0, default, Main.rand.NextFloat(.8f, 1.3f)).noGravity = true;
			}
			Projectile.rotation = Projectile.velocity.X / 20;

			return AiState != STATE_SAND;
		}

		public override void PostAI()
		{
			if (AiState != STATE_SAND)
				Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 8f, .1f); //Gravity effects
		}

		private void SandBehaviour()
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, Projectile.velocity.X / 3, Projectile.velocity.Y / 3, 0, default, Main.rand.NextFloat(.8f, 1.5f)).noGravity = true;

			Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Player.Center) * 8f, .08f);
			if (Projectile.Distance(Player.Center) < 80 && Projectile.Center.Y <= Player.Center.Y)
			{
				Projectile.alpha = 0;
				Projectile.tileCollide = true;
				AiState = STATE_IDLING;
			}
		}

		public override void IdleMovement(Player player)
		{
			if ((int)Projectile.velocity.X == 0)
				AiState = STATE_STILL;
			else
				AiState = STATE_IDLING;
			if (Math.Abs((Player.Center - Projectile.Center).Y) > 80 || !Collision.CanHit(Projectile, Player))
				AiState = STATE_SAND;

			if (Projectile.Distance(player.Center) > 80)
			{
				const float maxSpeed = 5f;
				Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, Projectile.DirectionTo(player.Center).X * maxSpeed, .05f);
			}
			else Projectile.velocity *= Main.rand.NextFloat(.80f, .98f);
		}

		public override void TargettingBehavior(Player player, NPC target)
		{
			AiState = STATE_AGGROED;

			const float maxSpeed = 10f;
			Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, Projectile.DirectionTo(target.Center).X * maxSpeed, .05f);

			if (Projectile.Distance(target.Center) < 15f) //Charge forward when nearby an enemy
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 10f;
			if (Projectile.velocity.Y == 0 && target.Center.Y < (Projectile.Center.Y - (Projectile.height / 2))) //Jump
				Projectile.velocity.Y = -Math.Min((Projectile.Center - target.Center).Y, 20f);
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			int maxFrames = (AiState == STATE_STILL) ? 1 : Main.projFrames[Type];
			Rectangle frame = texture.Frame(2, Main.projFrames[Type], (AiState == STATE_STILL) ? 0 : 1, Projectile.frame % maxFrames, -2, -2);
			SpriteEffects effects = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);
			return false;
		}
	}
}