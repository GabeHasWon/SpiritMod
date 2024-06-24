using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Items.Consumable.Food;
using SpiritMod.Mechanics.Fathomless_Chest;
using Terraria.Audio;
using System.Collections.Generic;
using Terraria.Utilities;

namespace SpiritMod.Items.Sets.GamblerChestLoot.GamblerChestNPCs
{
	internal class PlatinumChestBottom : GamblerChest
	{
		private bool Landed { get => NPC.localAI[0] == 1; set => NPC.localAI[0] = value ? 1 : 0; }
		private float sineAdd = -1;

		public override int TotalValue => Main.rand.Next(50000, 200000);
		public override int CoinRate => CounterMax;
		public override int CounterMax => 250;

		public override List<LootInfo> LootDrops() //Update from silver chances
		{
			bool isTime = Counter == CounterMax;

			return new() {
				new LootInfo(genericVanityDrops,
				() =>
				{
					if (isTime && Main.rand.NextBool(20))
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
					if (isTime && Main.rand.NextBool(15))
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
				new LootInfo(ModContent.ItemType<Jem.Jem>(), () => isTime && Main.rand.NextBool(50)),
				new LootInfo(ModContent.ItemType<GoldenCaviar>(), () => isTime && Main.rand.NextBool(7)),
				new LootInfo(ModContent.ItemType<FunnyFirework.FunnyFirework>(), () => isTime && Main.rand.NextBool(10), () => (byte)Main.rand.Next(5, 9)),
				new LootInfo(ItemID.AngelStatue, () => isTime && Main.rand.NextBool(100)),
				new LootInfo(ModContent.ItemType<Champagne.Champagne>(), () => isTime && Main.rand.NextBool(10), () => (byte)Main.rand.Next(1, 3)),
				new LootInfo(ModContent.ItemType<Mystical_Dice>(), () => isTime && Main.rand.NextBool(17)),
				new LootInfo(new int[] { ModContent.ItemType<GildedMustache.GildedMustache>(), ModContent.ItemType<RegalCane.RegalCane>() }, () => isTime && Main.rand.NextBool(13)),
				new LootInfo(ItemID.None,
				() =>
				{
					if (isTime && Main.rand.NextBool(15))
					{
						WeightedRandom<int> pins = new WeightedRandom<int>(Main.rand);
						pins.Add(ModContent.ItemType<Pins.PinCopperCoin>(), 0.2f);
						pins.Add(ModContent.ItemType<Pins.PinSilverCoin>(), 0.3f);
						pins.Add(ModContent.ItemType<Pins.PinGoldCoin>(), 0.5f);

						NPC.DropItem(pins, NPC.GetSource_FromAI());
						return true;
					}
					else return false;
				})
			};
		}

		public override void StaticDefaults()
		{
			//DisplayName.SetDefault("Platinum Chest");
			Main.npcFrameCount[NPC.type] = 9;
		}

		public override void Defaults()
		{
			NPC.width = 40;
			NPC.height = 34;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Vector2 center = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
			#region shader
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			Vector4 colorMod = Color.Silver.ToVector4();
			SpiritMod.StarjinxNoise.Parameters["distance"].SetValue(2.9f - (sineAdd / 10));
			SpiritMod.StarjinxNoise.Parameters["colorMod"].SetValue(colorMod);
			SpiritMod.StarjinxNoise.Parameters["noise"].SetValue(Mod.Assets.Request<Texture2D>("Textures/noise").Value);
			SpiritMod.StarjinxNoise.Parameters["rotation"].SetValue(sineAdd / 5);
			SpiritMod.StarjinxNoise.Parameters["opacity2"].SetValue(0.3f + (sineAdd / 10));
			SpiritMod.StarjinxNoise.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49").Value, NPC.Center - Main.screenPosition + new Vector2(0, 2), null, Color.White, 0f, new Vector2(50, 50), 1.1f + (sineAdd / 9), SpriteEffects.None, 0f);
			
			if (Landed)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
				float breakCounter = 1 - ((CounterMax - Counter) / 200f);
				SpiritMod.CircleNoise.Parameters["breakCounter"].SetValue(breakCounter);
				SpiritMod.CircleNoise.Parameters["rotation"].SetValue(breakCounter);
				SpiritMod.CircleNoise.Parameters["colorMod"].SetValue(colorMod);
				SpiritMod.CircleNoise.Parameters["noise"].SetValue(Mod.Assets.Request<Texture2D>("Textures/noise").Value);
				SpiritMod.CircleNoise.CurrentTechnique.Passes[0].Apply();
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49").Value, (NPC.Center - Main.screenPosition) + new Vector2(0, 2), null, Color.White, 0f, new Vector2(50, 50), 3 + (breakCounter / 2), SpriteEffects.None, 0f);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			#endregion

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
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Sets/GamblerChestLoot/GamblerChestNPCs/PlatinumChestBottom_White").Value, (NPC.Center - Main.screenPosition) + new Vector2(0, 2), new Rectangle(0, NPC.frame.Y, NPC.width, NPC.height), color * alpha, NPC.rotation, center, NPC.scale, SpriteEffects.None, 0f);
			}
		}

		public override void PostAI()
		{
			sineAdd += 0.03f;

			if (Counter % 10 == 0)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PlatinumCoin, 0, 0).velocity = Vector2.Zero;
			if (activated && NPC.velocity.Y == 0 && !Landed)
			{
				Counter = 0;
				Landed = true;

				NPC.noGravity = true;
				NPC.velocity.Y = -0.09f;
				SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
			}
			if (Landed)
				NPC.rotation = 0;
		}

		public override void OnActivate() => NPC.velocity.Y = -5;

		public override void DeathEffects()
		{
			SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

			Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 7, Mod.Find<ModGore>("PlatinumChestGore1").Type, 1f);
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 7, Mod.Find<ModGore>("PlatinumChestGore2").Type, 1f);
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 7, Mod.Find<ModGore>("PlatinumChestGore3").Type, 1f);
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 7, Mod.Find<ModGore>("PlatinumChestGore4").Type, 1f);
		}

		public override void FindFrame(int frameHeight)
		{
			if (!Landed)
				return;

			NPC.frameCounter = (NPC.frameCounter + ((float)Counter / CounterMax)) % Main.npcFrameCount[Type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * NPC.height;

			if (Counter % 10 == 0 && Counter > (CounterMax - 70))
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, ProjectileID.LunarFlare, 0, 0, NPC.target).timeLeft = 2;
		}
	}
}
