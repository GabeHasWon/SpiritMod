using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.GlobalClasses.Players;
using SpiritMod.Items.Pets;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Pet
{
	public class ShadowPet : ModProjectile
	{
		private readonly int[] frameCounts = new int[] { 4, 5, 4 };

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shadow Pup");
			Main.projFrames[Projectile.type] = 5;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(48, 34);
			Projectile.hostile = false;
			Projectile.penetrate = -1;
		}

		private int State
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private const int STATE_IDLING = 0;
		private const int STATE_RUNNING = 1;
		private const int STATE_FLYING = 2;

		private int pettingPlayer;

		private bool IsBeingPet => pettingPlayer != -1;

		private int Counter
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			owner.GetModPlayer<PetPlayer>().PetFlag(Projectile);

			float gravity = 0;
			if (State == STATE_IDLING)
			{
				if (Projectile.Distance(owner.Center) > 100)
					SwitchState(STATE_RUNNING);

				Projectile.velocity *= .9f;
				gravity = 2.5f;
			}
			if (State == STATE_RUNNING)
			{
				if (Projectile.Distance(owner.Center) > 800 || owner.Center.Y < (Projectile.Center.Y - 100))
					SwitchState(STATE_FLYING);
				if (Projectile.Distance(owner.Center) <= 80)
					SwitchState(STATE_IDLING);

				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(owner.Center) * 4f, 0.1f);
				gravity = 2.5f;

				if ((Counter = ++Counter % 120) == 0)
				{
					if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, owner.position, owner.width, owner.height))
						SwitchState(STATE_FLYING);
				}
			}
			if (State == STATE_FLYING)
			{
				if (Projectile.Distance(owner.Center) <= 20 && Projectile.Center.Y < owner.Center.Y)
					SwitchState(STATE_IDLING);

				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(owner.Center) * 10f, 0.1f);
			}

			Projectile.tileCollide = State != STATE_FLYING;
			Projectile.velocity.Y = MathHelper.Min(10f, Projectile.velocity.Y + gravity); //Gravity

			float stepSpeed = 6f;
			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref stepSpeed, ref Projectile.gfxOffY);

			float directionTo = (Projectile.Center - owner.Center).X;
			if (Math.Abs(directionTo) >= 1)
				Projectile.direction = Projectile.spriteDirection = (int)Math.Sign(directionTo);

			if (++Projectile.frameCounter >= (IsBeingPet ? 8 : 4))
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % frameCounts[State];
			}

			PettingBehaviour();
		}

		private void PettingBehaviour()
		{
			if (State == STATE_IDLING)
			{
				if (Projectile.getRect().Contains(Main.MouseWorld.ToPoint()))
				{
					Player player = Main.LocalPlayer;
					int pettingRange = 70;

					if (player.Distance(Projectile.Center) < pettingRange) //Pet the dog
					{
						if (Main.mouseRight && Main.mouseRightRelease)
						{
							if (!IsBeingPet)
							{
								player.Center = new Vector2(Projectile.Center.X, Projectile.position.Y + Projectile.height) + new Vector2(Math.Sign(Projectile.DirectionTo(player.Center).X) * 34, -(player.height / 2));
								player.direction = Math.Sign(player.DirectionTo(Projectile.Center).X);
								player.velocity = Vector2.Zero;

								if (Main.netMode != NetmodeID.SinglePlayer)
									NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
							}

							if (pettingPlayer == -1)
								pettingPlayer = Main.myPlayer;
							else
								pettingPlayer = -1;

							Projectile.netUpdate = true;
						}
						if (!IsBeingPet)
						{
							player.noThrow = 2;
							player.cursorItemIconEnabled = true;
							player.cursorItemIconID = ModContent.ItemType<ShadowCollar>();
						}
					}
				}
			}

			if (IsBeingPet)
			{
				Player pPlayer = Main.player[pettingPlayer];

				float amount = (float)Math.Sin(Main.timeForVisualEffects / 2f) * 2;
				Player.CompositeArmStretchAmount stretchAmount = (amount > 0) ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.ThreeQuarters;
				pPlayer.SetCompositeArmBack(true, stretchAmount, -2.2f * pPlayer.direction);

				if (pPlayer.velocity != Vector2.Zero)
					pettingPlayer = -1;
			}
		}

		private void SwitchState(int newState)
		{
			Counter = 0;
			Projectile.frameCounter = 0;
			Projectile.frame = 0;

			State = newState;
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Rectangle drawFrame = texture.Frame(3, Main.projFrames[Type], State, Projectile.frame, -2, -2);
			Vector2 drawOffset = Vector2.UnitY * (Projectile.gfxOffY - 6);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Main.EntitySpriteDraw(texture, Projectile.Center + drawOffset - Main.screenPosition, drawFrame, Projectile.GetAlpha(lightColor), Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center + drawOffset - Main.screenPosition, drawFrame, Projectile.GetAlpha(Color.White), Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, effects, 0);

			return false;
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(pettingPlayer);

		public override void ReceiveExtraAI(BinaryReader reader) => pettingPlayer = reader.ReadByte();
	}
}