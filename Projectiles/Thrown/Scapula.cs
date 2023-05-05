using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Prim;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Thrown
{
	public class Scapula : ModProjectile, IDrawAdditive
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int counterMax = 10;

		private int hitNPCIndex = -1;

		/// <summary>
		/// Used whenever we want the projectile to visually embed in the target without dealing its unique damage effect in PreAI
		/// </summary>
		private bool NoDamage
		{
			get => (int)Projectile.ai[1] != 0;
			set => Projectile.ai[1] = value ? 1 : 0;
		}

		public override string Texture => "SpiritMod/Items/BossLoot/AvianDrops/SoaringScapula";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soaring Scapula");
			ProjectileID.Sets.TrailCacheLength[Type] = 2;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Shuriken);
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.penetrate = 1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = true;
			Projectile.hostile = false;
		}

		public override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];

			if (hitNPCIndex > -1) //The projectile is embedded
			{
				Projectile.timeLeft = 2;
				Projectile.velocity = Vector2.Zero;
				NPC npc = Main.npc[hitNPCIndex];

				bool shouldKill = false;

				if (++Counter > counterMax)
				{
					if (Counter > (30 + counterMax) || ((Projectile.alpha >= 255) && NoDamage))
						shouldKill = true;

					Projectile.alpha += 255 / 10;
				}

				if (!shouldKill && !NoDamage)
				{
					Vector2 bottomLeft = npc.position + new Vector2(0, npc.height);
					if (GroundCollision(npc)) //The NPC hits a surface
					{
						shouldKill = true;

						player.GetSpiritPlayer().Shake += 5;
						npc.StrikeNPC(Projectile.damage, 0f, Math.Sign(npc.velocity.X));
						npc.velocity.Y = -3f; //Send the NPC upwards again

						SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, npc.Center);

						Collision.HitTiles(bottomLeft, npc.velocity, npc.width, npc.height);
						for (int i = 0; i < 18; i++)
						{
							Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, Main.rand.NextBool(2) ? DustID.Ash : DustID.TreasureSparkle, Main.rand.NextFloat(-5.0f, 5.0f), -3f);
							dust.noGravity = true;
							dust.fadeIn = 1.2f;
						}

						Projectile.netUpdate = true;
					}
					else
					{
						npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 10, 0.8f);

						int loops = (npc.getRect().Width / 10) + 2;
						if (npc.velocity.Y > 1f)
						{
							for (int i = 0; i < loops; i++)
							{
								Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, Main.rand.NextBool(2) ? DustID.Ash : DustID.TreasureSparkle, 0f, -2f, 80);
								dust.noGravity = true;
							}
						}
					}
				}
				if (shouldKill)
					Projectile.Kill();

				if (npc.active)
					Projectile.position += npc.velocity; //Appear to remain stuck to the NPC
				else
					NoDamage = true;

				return false;
			}
			else if (Counter < counterMax) //Throwing animation
			{
				player.heldProj = Projectile.whoAmI;
				Projectile.tileCollide = false;

				float addRot = (float)((counterMax - Counter) * 0.2f) + 0.15f;
				Projectile.Center = player.MountedCenter + new Vector2(24, 0).RotatedBy(Projectile.velocity.ToRotation() - (addRot * player.direction)) - Projectile.velocity;

				player.itemRotation = MathHelper.WrapAngle(player.AngleTo(Projectile.Center) + ((player.direction < 0) ? MathHelper.Pi : 0));
				Projectile.rotation = player.AngleTo(Projectile.Center) + 1.57f;
				Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0) ? 1 : -1;

				if (++Counter >= counterMax)
					Projectile.netUpdate = true;

				return false;
			}
			else
			{
				Projectile.tileCollide = true;
				return true;
			}
		}

		public override bool? CanDamage() => ((Counter < counterMax) || (hitNPCIndex > -1)) ? false : null;

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if ((!target.boss && target.velocity != Vector2.Zero && target.knockBackResist != 0) || target.type == NPCID.TargetDummy)
			{
				Projectile.penetrate++; //Prevent the projectile from dying

				NoDamage = GroundCollision(target); //If the target is already grounded, don't deal the damage effect in PreAI
				hitNPCIndex = target.whoAmI;

				Counter = 0;
			}
		}

		private static bool GroundCollision(NPC npc)
		{
			Vector2 bottomLeft = npc.position + new Vector2(0, npc.height);
			return WorldGen.SolidOrSlopedTile(Main.tile[(bottomLeft / 16).ToPoint()]);
		}

		public override void Kill(int timeLeft)
		{
			if (hitNPCIndex > -1)
				return;
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			for (int i = 0; i < 10; i++)
			{
				Vector2 velocity = (Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 3.0f)) + -(Projectile.oldVelocity * Main.rand.NextFloat(0.1f, 0.8f));
				Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Bone, velocity.X, velocity.Y);

				if (i < 4 && Main.rand.NextBool(2))
					Gore.NewGorePerfect(Entity.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.0f, 3.0f), Mod.Find<ModGore>("Scapula" + (i + 1)).Type);
			}
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			if (hitNPCIndex > -1)
				behindNPCs.Add(index);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			SpriteEffects effects = (Projectile.direction < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);

			Texture2D glowTex = Mod.Assets.Request<Texture2D>("Projectiles/Thrown/Scapula_Glow").Value;
			Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White * .5f), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);

			if (hitNPCIndex > -1)
			{
				Texture2D glow2Tex = Mod.Assets.Request<Texture2D>("Projectiles/Thrown/Scapula_Glow2").Value;
				float quoteant = (float)Counter / counterMax;
				Color color = Color.Lerp(Color.White, Color.Yellow, 1f - quoteant) * quoteant;

				Main.EntitySpriteDraw(glow2Tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);
			}
			return false;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			if ((hitNPCIndex < 0) || (Counter > 5))
				return;
			Vector2 direction = Projectile.DirectionTo(Main.npc[hitNPCIndex].Center);

			float scaleMod = 1f - (float)((float)Counter / counterMax);
			Vector2 position = Projectile.Center + (direction * 14);
			float rotation = direction.ToRotation();

			Texture2D bloom = Mod.Assets.Request<Texture2D>("Effects/Masks/CircleGradient", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Main.spriteBatch.Draw(bloom, position - Main.screenPosition, null, Color.Yellow * .4f, 0, bloom.Size() / 2, 0.5f * scaleMod, SpriteEffects.None, 0);

			Effect blurEffect = ModContent.Request<Effect>("SpiritMod/Effects/BlurLine").Value;
			SquarePrimitive blurLine = new SquarePrimitive()
			{
				Position = position - Main.screenPosition,
				Height = 150 * scaleMod,
				Length = 12 * scaleMod,
				Rotation = rotation,
				Color = Color.White
			};
			PrimitiveRenderer.DrawPrimitiveShape(blurLine, blurEffect);
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(hitNPCIndex);
		public override void ReceiveExtraAI(BinaryReader reader) => hitNPCIndex = reader.ReadInt32();
	}
}
