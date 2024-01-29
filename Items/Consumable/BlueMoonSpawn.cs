using Microsoft.Xna.Framework;
using SpiritMod.Items.BossLoot.StarplateDrops;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(3)]
	public class BlueMoonSpawn : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.Pink;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = Item.useAnimation = 20;
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = false;
			Item.UseSound = SoundID.Item43;
		}

		public override bool CanUseItem(Player player)
		{
			if ((player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && !Main.pumpkinMoon && !Main.snowMoon)
				return false;

			if (Main.dayTime)
				Main.NewText(Language.GetTextValue("Mods.SpiritMod.Events.BlueMoon.IsDay"), 80, 80, 150);

			if (MyWorld.calmNight)
				Main.NewText(Language.GetTextValue("Mods.SpiritMod.Events.BlueMoon.IsCalmNight"), 80, 80, 150);

			return !MyWorld.blueMoon && !Main.dayTime && !MyWorld.calmNight;
		}

		public override bool? UseItem(Player player)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(Language.GetTextValue("Mods.SpiritMod.Events.BlueMoon.OnStart"), 61, 255, 142);
			else if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.SpiritMod.Events.BlueMoon.OnStart"), new Color(61, 255, 142));

			SoundEngine.PlaySound(SoundID.Roar, player.Center);
			
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				MyWorld.blueMoon = true;

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.WorldData);
			}
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.CrystalShard, 6);
			recipe.AddIngredient(ModContent.ItemType<Placeable.Tiles.AsteroidBlock>(), 30);
			recipe.AddIngredient(ItemID.SoulofLight, 10);
			recipe.AddIngredient(ModContent.ItemType<CosmiliteShard>(), 4);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
