using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.VinewrathDrops.VinewrathPet
{
	public class VinewrathPetProjectile : ModProjectile
	{
		private Player Owner => Main.player[Projectile.owner];
		private ref float MoveSpeedX => ref Projectile.ai[0];
		private ref float MoveSpeedY => ref Projectile.ai[1];

		private int _state = 0;
		private int _charge = 0;

		private readonly int _exhaustionMax = 100;
		private int _exhaustion = 0;


		private int targetIndex = -1;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wrathful Seedling");
			Main.projFrames[Projectile.type] = 7;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Truffle);
			Projectile.aiStyle = 0;
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.light = 0f;
			Projectile.tileCollide = false;

			AIType = 0;
		}

		public override void AI()
		{
			Owner.GetModPlayer<GlobalClasses.Players.PetPlayer>().PetFlag(Projectile);

			if (_state == 0)
				NearbyMovement();
			else if (_state == 1)
				AggressiveCharge();
			else
				FollowPlayerFlight();

			if (Projectile.velocity.X > 0)
				Projectile.spriteDirection = -1;
			else if (Projectile.velocity.X < 0)
				Projectile.spriteDirection = 1;
			Projectile.rotation = Projectile.velocity.X * 0.04f;
			//Increment draw frame
			if (++Projectile.frameCounter >= 6)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
			}
			if (_exhaustion > 0)
				_exhaustion--;
		}

		private void NearbyMovement()
		{
			GeneralMovement();

			NPC target = FindTarget(Owner, 220 * 220);
			if (target != null)
			{
				targetIndex = target.whoAmI;
				ResetState(1);
			}
			if (Projectile.DistanceSQ(Owner.Center) > 700 * 700)
				ResetState(2);
		}

		/// <summary>Literally stolen from ReachBoss1.cs</summary>
		/// <param name="player"></param>
		public void GeneralMovement()
		{
			Player player = Owner;

			if (Projectile.Center.X >= player.Center.X && MoveSpeedX >= -30) //Flies to players x position
				MoveSpeedX--;
			else if (Projectile.Center.X <= player.Center.X && MoveSpeedX <= 30)
				MoveSpeedX++;

			Projectile.velocity.X = MoveSpeedX * 0.08f;

			float sine = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) * 60f;
			if (Projectile.Center.Y >= player.Center.Y - 60 + sine && MoveSpeedY >= -14) //Flies to players Y position
				MoveSpeedY--;
			else if (Projectile.Center.Y <= player.Center.Y - 60f + sine && MoveSpeedY <= 14)
				MoveSpeedY++;
			Projectile.velocity.Y = MoveSpeedY * 0.1f;
		}

		private void AggressiveCharge()
		{
			if (!Main.npc[targetIndex].active)
			{
				ResetState(0);
				targetIndex = -1;
				return;
			}
			if (_exhaustion <= 0)
			{
				int magnitude = 12;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[targetIndex].Center) * magnitude, 0.1f);
				if (Projectile.Hitbox.Intersects(Main.npc[targetIndex].getRect()))
				{
					Projectile.velocity = Projectile.DirectionFrom(Main.npc[targetIndex].Center) * 5;
					Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI("Bash Attack"), Main.npc[targetIndex].getRect().ClosestPointInRect(Projectile.Center), 
						Vector2.Zero, ModContent.ProjectileType<VinewrathPetProjectile_Bash>(), 0, 0f, Owner.whoAmI);
					proj.rotation = Projectile.velocity.ToRotation();
					for (int i = 0; i < 6; i++)
					{
						int type = Main.rand.NextBool(2) ? DustID.Sunflower : DustID.Grass;
						Vector2 velocity = Projectile.velocity * Main.rand.NextFloat(0.8f, 1.2f);
						Dust.NewDust(proj.position, proj.width, proj.height, type, velocity.X, velocity.Y, 0, default, Main.rand.NextFloat(1.0f, 1.3f));
					}

					SoundEngine.PlaySound(SoundID.GlommerBounce, Projectile.position); //Item171, GlommerBounce
					ResetState(0);
					targetIndex = -1;
					_exhaustion = _exhaustionMax;
					Projectile.netUpdate = true;
				}
				if (Main.rand.NextBool(2))
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 
						Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(1.0f, 2.0f));
					dust.noGravity = true;
				}
			}
			else Projectile.velocity *= 0.98f;
		}

		private void FollowPlayerFlight()
		{
			const float MaxSpeedDistance = 800;
			const float DefaultSpeed = 7f;
			const int StartCharge = 40;
			const int MaxCharge = 50;

			_charge++;

			float dist = Projectile.DistanceSQ(Main.player[Projectile.owner].Center);
			float magnitude = DefaultSpeed;

			if (dist > MaxSpeedDistance * MaxSpeedDistance)
				magnitude = DefaultSpeed + (((float)Math.Sqrt(dist) - MaxSpeedDistance) * 0.05f);

			if (_charge < StartCharge)
				Projectile.velocity *= 0.8f;
			else if (_charge > StartCharge && _charge < MaxCharge)
			{
				float mult = (_charge - (float)StartCharge) / MaxCharge;
				Projectile.velocity = Projectile.DirectionTo(Owner.Center) * mult * magnitude;
			}
			else if (_charge >= MaxCharge)
				Projectile.velocity = Projectile.DirectionTo(Owner.Center) * magnitude;

			if (dist <= 100 * 100)
			{
				_charge = 0;

				MoveSpeedX = Projectile.velocity.X / 0.08f;
				MoveSpeedY = Projectile.velocity.Y / 0.1f;
				ResetState(0);
			}
		}

		private NPC FindTarget(Player player, int detectRange)
		{
			foreach (NPC npc in Main.npc)
				if (npc.active && !npc.friendly && npc.CanBeChasedBy(Projectile) && 
					Collision.CanHitLine(npc.Center, 0, 0, player.Center, 0, 0) && Projectile.DistanceSQ(npc.Center) <= detectRange)
					return npc;
			return null;
		}

		private void ResetState(int newState)
		{
			_state = newState;

			Projectile.frameCounter = 0;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Asset<Texture2D> texture = TextureAssets.Projectile[Projectile.type];
			Rectangle rect = new Rectangle(0, texture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, texture.Width(), texture.Height() / Main.projFrames[Projectile.type] - 2);
			SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			float scalar = (float)((float)_exhaustion / _exhaustionMax * .3f);
			Vector2 scale = new Vector2(1f + scalar, 1f - scalar) * Projectile.scale;
			Vector2 offset = scalar * rect.Size() * .5f;
			if (_state != 0)
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + offset + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * (float)((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
					Main.EntitySpriteDraw(texture.Value, drawPos, rect, color, Projectile.rotation, Vector2.Zero, scale, effect, 0);
				}
			}
			Main.EntitySpriteDraw(texture.Value, Projectile.position + offset - Main.screenPosition, rect, lightColor, Projectile.rotation, Vector2.Zero, scale, effect, 0);
			//Draw a glowmask
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.position + offset - Main.screenPosition, rect, Projectile.GetAlpha(Color.White), Projectile.rotation, Vector2.Zero, scale, effect, 0);
			return false;
		}
	}
}
