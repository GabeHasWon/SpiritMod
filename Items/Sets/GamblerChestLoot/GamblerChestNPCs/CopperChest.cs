using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Mechanics.Fathomless_Chest;
using System.Collections.Generic;
using SpiritMod.Items.Consumable.Food;
using Terraria.Utilities;

namespace SpiritMod.Items.Sets.GamblerChestLoot.GamblerChestNPCs
{
	public class CopperChestBottom : GamblerChest
	{
		public override int TotalValue => Main.rand.Next(300, 500);
		public override int CoinRate => 10;
		public override int CounterMax => 100;

		public override List<LootInfo> LootDrops()
		{
			return new() {
				new LootInfo(genericVanityDrops,
				() =>
				{
					if (Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(250))
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
					else 
						return false;
				}),
				new LootInfo(donatorVanityDrops,
				() =>
				{
					if (Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(100))
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
					else 
						return false;
				}),
				new LootInfo(ModContent.ItemType<Jem.Jem>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(400)),
				new LootInfo(ModContent.ItemType<GoldenCaviar>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(20)),
				new LootInfo(ModContent.ItemType<FunnyFirework.FunnyFirework>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(20), () => (byte)Main.rand.Next(5, 9)),
				new LootInfo(ItemID.AngelStatue, () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(20)),
				new LootInfo(ModContent.ItemType<Champagne.Champagne>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(25), () => (byte)Main.rand.Next(1, 3)),
				new LootInfo(ModContent.ItemType<Mystical_Dice>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(100)),
				new LootInfo(new int[] { ModContent.ItemType<GildedMustache.GildedMustache>(), ModContent.ItemType<RegalCane.RegalCane>() }, () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(100)),
				new LootInfo(ItemID.None,
				() =>
				{
					if (Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(30))
					{
						WeightedRandom<int> pins = new WeightedRandom<int>(Main.rand);
						pins.Add(ModContent.ItemType<Pins.PinCopperCoin>(), 0.80f);
						pins.Add(ModContent.ItemType<Pins.PinSilverCoin>(), 0.175f);
						pins.Add(ModContent.ItemType<Pins.PinGoldCoin>(), 0.025f);

						NPC.DropItem(pins, NPC.GetSource_FromAI());
						return true;
					}
					else 
						return false;
				})
			};
		}

		public override void StaticDefaults()
		{
			//DisplayName.SetDefault("Copper Chest");
			Main.npcFrameCount[NPC.type] = 4;
		}

		public override void Defaults()
		{
			NPC.width = 30;
			NPC.height = 26;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			int frame = (int)NPC.frameCounter;

			Vector2 center = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
			Rectangle rect = new Rectangle(0, frame * (TextureAssets.Npc[NPC.type].Value.Height / 4), TextureAssets.Npc[NPC.type].Value.Width, TextureAssets.Npc[NPC.type].Value.Height / 4);
			
			Main.EntitySpriteDraw(Mod.Assets.Request<Texture2D>("Items/Sets/GamblerChestLoot/GamblerChestNPCs/CopperChestTop", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, NPC.Center - Main.screenPosition + new Vector2(0, 4), rect, drawColor, NPC.rotation, center, NPC.scale, SpriteEffects.None, 0);
			
			if (activated && frame > 0)
				Main.EntitySpriteDraw(TextureAssets.Extra[49].Value, NPC.Center - Main.screenPosition + new Vector2(-2, 4), null, new Color(200, 200, 200, 0), 0f, new Vector2(50, 50), 0.3f * NPC.scale, SpriteEffects.None, 0);

			Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, NPC.Center - Main.screenPosition + new Vector2(0, 4), rect, drawColor, NPC.rotation, center, NPC.scale, SpriteEffects.None, 0);
			return false;
		}

		public override void PostAI()
		{
			if (Counter % 10 == 0)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, 0, 0).velocity = Vector2.Zero;
		}

		public override void DeathEffects()
		{
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 11);
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 12);
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 13);

			SoundEngine.PlaySound(SoundID.DoubleJump, NPC.Center);
		}
	}
}