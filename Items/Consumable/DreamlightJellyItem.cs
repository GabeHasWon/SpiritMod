using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SpiritMod.NPCs.Boss.MoonWizard;
using Microsoft.Xna.Framework.Graphics;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(1)]
	public class DreamlightJellyItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = Item.useAnimation = 20;
			Item.noMelee = true;
			Item.consumable = false;
			Item.autoReuse = false;
			Item.UseSound = SoundID.Item43;
		}

		public override bool CanUseItem(Player player) => !NPC.AnyNPCs(ModContent.NPCType<MoonWizard>()) && !Main.dayTime && (player.ZoneSkyHeight || player.ZoneOverworldHeight);

		public override bool? UseItem(Player player)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<MoonWizard>());
			else if (Main.netMode == NetmodeID.MultiplayerClient && player == Main.LocalPlayer)
			{
				Vector2 spawnPos = player.Center;
				int tries = 0;
				int maxtries = 300;

				while ((Vector2.Distance(spawnPos, player.Center) <= 200 || WorldGen.SolidTile((int)spawnPos.X / 16, (int)spawnPos.Y / 16) || WorldGen.SolidTile2((int)spawnPos.X / 16, (int)spawnPos.Y / 16) || WorldGen.SolidTile3((int)spawnPos.X / 16, (int)spawnPos.Y / 16)) && tries <= maxtries)
				{
					spawnPos = player.Center + Main.rand.NextVector2Circular(800, 800);
					tries++;
				}

				if (tries >= maxtries)
					return false;

				SpiritMultiplayer.SpawnBossFromClient((byte)player.whoAmI, ModContent.NPCType<MoonWizard>(), (int)spawnPos.X, (int)spawnPos.Y);
			}

			SoundEngine.PlaySound(SoundID.Roar, player.position);
			return true;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.46f, .07f, .64f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}
	}
}