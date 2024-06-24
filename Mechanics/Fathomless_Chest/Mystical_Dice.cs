using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Tiles;
using Terraria.Localization;

namespace SpiritMod.Mechanics.Fathomless_Chest;

[Sacrifice(3)]
public class Mystical_Dice : ModItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 20;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Orange;
		Item.useAnimation = 45;
		Item.useTime = 45;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.consumable = true;
	}

	public override bool? UseItem(Player player)
	{
		if (Main.netMode == NetmodeID.SinglePlayer)
			FindFathomlessChest(player, Item);
		else
		{
			ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.SearchForFathomless, 2);
			packet.Write((byte)player.whoAmI);
			packet.Write((byte)player.selectedItem);
			packet.Send();
		}

		return true;
	}

	public static bool FindFathomlessChest(Player player, Item item)
	{
		for (int x = 0; x < Main.maxTilesX; ++x)
		{
			for (int y = (int)Main.rockLayer; y < Main.maxTilesY; ++y)
			{
				if (Main.tile[x, y].TileType != ModContent.TileType<Fathomless_Chest>())
					continue;

				RunTeleport(player, new Vector2(x * 16, y * 16));
				return true;
			}
		}

		if (Main.netMode == NetmodeID.SinglePlayer)
			FailFind(player, item);

		return false;
	}

	public static void FailFind(Player player, Item item)
	{
		string text = Language.GetTextValue("Mods.SpiritMod.Misc.NoShrines");
		CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Cyan, text);

		if (player.ItemAnimationJustStarted)
			player.QuickSpawnItem(player.GetSource_ItemUse(item), ModContent.ItemType<Black_Stone_Item>(), 15);

		SoundEngine.PlaySound(SoundID.Item110, player.Center);
	}

	private static void RunTeleport(Player player, Vector2 pos)
	{
		player.velocity = Vector2.Zero;
		SoundEngine.PlaySound(SoundID.Item6, player.Center);

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, pos.X, pos.Y, TeleportationStyleID.RecallPotion);
		else
			player.Teleport(pos, 2, 0);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<Black_Stone_Item>(18);
		recipe.Register();
	}
}