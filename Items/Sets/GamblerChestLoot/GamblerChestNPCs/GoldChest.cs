using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Mechanics.Fathomless_Chest;
using SpiritMod.Items.Consumable.Food;
using System.Collections.Generic;

namespace SpiritMod.Items.Sets.GamblerChestLoot.GamblerChestNPCs
{
	internal class GoldChestBottom : GamblerChest
	{
		private bool Landed { get => NPC.localAI[0] == 1; set => NPC.localAI[0] = value ? 1 : 0; }
		private float sineAdd = -1;

		public override int TotalValue => Main.rand.Next(40000, 60000);
		public override int CoinRate => 5;
		public override int CounterMax => 200;

		public override List<LootInfo> LootDrops() //Update from silver chances
		{
			return new() {
				new LootInfo(genericVanityDrops,
				() =>
				{
					if (Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(50))
					{
						for (int value = 0; value < 32; value++)
						{
							int num = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y - 20), 50, 50, DustID.ShadowbeamStaff, 0f, -2f, 0, default, 2f);
							Main.dust[num].noGravity = true;
							Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
							Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
							Main.dust[num].scale *= .35f;
							Main.dust[num].fadeIn += .1f;
						}
						return true;
					}
					else return false;
				}),
				new LootInfo(donatorVanityDrops,
				() =>
				{
					if (Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(20))
					{
						for (int value = 0; value < 32; value++)
						{
							int num = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y - 20), 50, 50, DustID.ShadowbeamStaff, 0f, -2f, 0, default, 2f);
							Main.dust[num].noGravity = true;
							Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
							Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
							Main.dust[num].scale *= .35f;
							Main.dust[num].fadeIn += .1f;
						}
						return true;
					}
					else return false;
				}),
				new LootInfo(ModContent.ItemType<Jem.Jem>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(100)),
				new LootInfo(ModContent.ItemType<GoldenCaviar>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(10)),
				new LootInfo(ModContent.ItemType<FunnyFirework.FunnyFirework>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(13), () => (byte)Main.rand.Next(5, 9)),
				new LootInfo(ItemID.AngelStatue, () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(50)),
				new LootInfo(ModContent.ItemType<Champagne.Champagne>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(13), () => (byte)Main.rand.Next(1, 3)),
				new LootInfo(ModContent.ItemType<Mystical_Dice>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(20)),
				new LootInfo(new int[] { ModContent.ItemType<GildedMustache.GildedMustache>(), ModContent.ItemType<RegalCane.RegalCane>() }, () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(20))
			};
		}

		public override void StaticDefaults()
		{
			DisplayName.SetDefault("Gold Chest");
			Main.npcFrameCount[NPC.type] = 2;
		}

		public override void Defaults()
		{
			NPC.width = 36;
			NPC.height = 24;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Vector2 center = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
			#region shader
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			Vector4 colorMod = Color.Gold.ToVector4();
			SpiritMod.StarjinxNoise.Parameters["distance"].SetValue(2.9f - (sineAdd / 10));
			SpiritMod.StarjinxNoise.Parameters["colorMod"].SetValue(colorMod);
			SpiritMod.StarjinxNoise.Parameters["noise"].SetValue(Mod.Assets.Request<Texture2D>("Textures/noise").Value);
			SpiritMod.StarjinxNoise.Parameters["rotation"].SetValue(sineAdd / 5);
			SpiritMod.StarjinxNoise.Parameters["opacity2"].SetValue(0.3f + (sineAdd / 10));
			SpiritMod.StarjinxNoise.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, (NPC.Center - Main.screenPosition) + new Vector2(0, 2), null, Color.White, 0f, new Vector2(50, 50), 1.1f + (sineAdd / 9), SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			#endregion

			if (Landed)
				Main.EntitySpriteDraw(Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49_Top", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, (NPC.Center - Main.screenPosition) + new Vector2(0, 2), null, new Color(200, 200, 200, 0), 0f, new Vector2(50, 50), 0.33f, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, NPC.Center - Main.screenPosition + new Vector2(0, 2), new Rectangle(0, NPC.frame.Y, NPC.width, NPC.height), drawColor, NPC.rotation, center, NPC.scale, SpriteEffects.None, 0);
			
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Vector2 center = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
			if (Landed)
			{
				Color color = Color.White;
				float alpha = Math.Max(0, 50 - (CounterMax - Counter)) / 50f;

				Main.EntitySpriteDraw(Mod.Assets.Request<Texture2D>("Items/Sets/GamblerChestLoot/GamblerChestNPCs/GoldChestTop_White").Value, NPC.Center - Main.screenPosition + new Vector2(0, 2), new Rectangle(0, NPC.frame.Y, NPC.width, NPC.height), color * alpha, NPC.rotation, center, NPC.scale, SpriteEffects.None, 0);
			}
		}

		public override void PostAI()
		{
			sineAdd += 0.03f;

			if (Counter % 10 == 0)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0, 0).velocity = Vector2.Zero;
			if (activated && NPC.velocity.Y == 0 && !Landed)
			{
				Counter = 0;
				Landed = true;

				SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GoldChestTop>(), 0, 0, NPC.target, NPC.position.X, NPC.position.Y);
			}
		}

		public override void OnActivate() => NPC.velocity.Y = -5;

		public override void DeathEffects()
		{
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * 7, Mod.Find<ModGore>("GoldChestGore4").Type, 1f);
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * 7, Mod.Find<ModGore>("GoldChestGore5").Type, 1f);
			
			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(0, 30), Vector2.Zero, ProjectileID.DD2ExplosiveTrapT2Explosion, 0, 0, NPC.target);
			SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
		}

		public override void FindFrame(int frameHeight) => NPC.frame.Y = (int)(NPC.height * (Landed ? 1 : 0));
	}
	internal class GoldChestTop : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gold Chest");
			Main.projFrames[Projectile.type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.hostile = true;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.damage = 0;
			Projectile.timeLeft = 200;
		}

		public override void AI()
		{
			Projectile.velocity.Y = -0.05f;
			Projectile.velocity.X = 0;
			Projectile.frameCounter++;
			if (Projectile.frameCounter > Math.Sqrt(Math.Max(Projectile.timeLeft - 100, 1) / 2) / 2)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= 12)
				{
					Projectile.frame = 0;
				}
			}

			if (Projectile.timeLeft == 1 && Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * 7, Mod.Find<ModGore>("GoldChestGore1").Type, 1f);
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * 7, Mod.Find<ModGore>("GoldChestGore2").Type, 1f);
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextFloat(6.28f).ToRotationVector2() * 7, Mod.Find<ModGore>("GoldChestGore3").Type, 1f);
			}
		}

		public override void PostDraw(Color lightColor)
		{
			Color color = Color.White;
			float alpha = (Math.Max(0, 50 - Projectile.timeLeft)) / 50f;
			Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Sets/GamblerChestLoot/GamblerChestNPCs/GoldChestTop_White").Value, (Projectile.position - Main.screenPosition), new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height), color * alpha, Projectile.rotation, Vector2.Zero, Projectile.scale, SpriteEffects.None, 0f);
		}
	}
}
