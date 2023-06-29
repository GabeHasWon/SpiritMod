using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
	internal class SilverChestBottom : GamblerChest
	{
		private bool Landed { get => NPC.localAI[0] == 1; set => NPC.localAI[0] = value ? 1 : 0; }

		public override int TotalValue => Main.rand.Next(2800, 7000);
		public override int CoinRate => 3;
		public override int CounterMax => 100;

		public override List<LootInfo> LootDrops()
		{
			return new() {
				new LootInfo(genericVanityDrops,
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
					else return false;
				}),
				new LootInfo(donatorVanityDrops,
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
				new LootInfo(ModContent.ItemType<Jem.Jem>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(200)),
				new LootInfo(ModContent.ItemType<GoldenCaviar>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(14)),
				new LootInfo(ModContent.ItemType<FunnyFirework.FunnyFirework>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(14), () => (byte)Main.rand.Next(5, 9)),
				new LootInfo(ItemID.AngelStatue, () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(33)),
				new LootInfo(ModContent.ItemType<Champagne.Champagne>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(17), () => (byte)Main.rand.Next(1, 3)),
				new LootInfo(ModContent.ItemType<Mystical_Dice>(), () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(25)),
				new LootInfo(new int[] { ModContent.ItemType<GildedMustache.GildedMustache>(), ModContent.ItemType<RegalCane.RegalCane>() }, () => Counter == (int)(CounterMax * .75f) && Main.rand.NextBool(33))
			};
		}

		public override void StaticDefaults()
		{
			DisplayName.SetDefault("Silver Chest");
			Main.npcFrameCount[NPC.type] = 5;
		}

		public override void Defaults()
		{
			NPC.width = 36;
			NPC.height = 34;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			int frame = (int)NPC.frameCounter;

			Vector2 center = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
			Rectangle rect = new Rectangle(0, frame * (TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type]), TextureAssets.Npc[NPC.type].Value.Width, (TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type]));
			
			Main.EntitySpriteDraw(Mod.Assets.Request<Texture2D>("Items/Sets/GamblerChestLoot/GamblerChestNPCs/SilverChestTop").Value, (NPC.Center - Main.screenPosition + new Vector2(0, 4)), rect, drawColor, NPC.rotation, center, NPC.scale, SpriteEffects.None, 0);
			
			if (activated && frame > 0)
				Main.EntitySpriteDraw(Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49_Top").Value, (NPC.Center - Main.screenPosition) + new Vector2(-2, 4), null, new Color(200, 200, 200, 0), 0f, new Vector2(50, 50), 0.375f * NPC.scale, SpriteEffects.None, 0);

			Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, NPC.Center - Main.screenPosition + new Vector2(0, 4), rect, drawColor, NPC.rotation, center, NPC.scale, SpriteEffects.None, 0);
			return false;
		}

		public override void PostAI()
		{
			if (Counter % 10 == 0)
				Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 0, 0).velocity = Vector2.Zero;
			if (activated && NPC.velocity.Y == 0 && !Landed)
			{
				Landed = true;
				NPC.frameCounter = 1;
				SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
			}
		}

		public override void OnActivate() => NPC.velocity.Y = -5;

		public override void DeathEffects()
		{
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 11);
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 12);
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 13);

			SoundEngine.PlaySound(SoundID.DoubleJump, NPC.Center);
		}

		public override void FindFrame(int frameHeight)
		{
			if (Landed)
			{
				NPC.frameCounter = MathHelper.Min((float)NPC.frameCounter + .2f, (Main.npcFrameCount[Type] - 1));
				NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
			}
		}
	}
}
