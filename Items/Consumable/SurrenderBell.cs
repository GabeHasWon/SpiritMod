using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class SurrenderBell : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 30;
			Item.rare = ItemRarityID.Cyan;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.buyPrice(gold: 40);
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = Item.useAnimation = 20;
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = false;
		}

		public override bool CanUseItem(Player player) => Main.invasionType != 0;

		public override bool? UseItem(Player player)
		{
			SoundEngine.PlaySound(SoundID.CoinPickup, player.Center);
			Main.invasionType = 0;
			Main.invasionSize = 0;

			NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, null, Main.invasionProgress, Main.invasionProgressMax, Main.invasionProgressIcon, 0f, 0, 0, 0);

			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText("The invaders have called off their attack!", Color.MediumPurple);
			else if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The invaders have called off their attack!"), Color.MediumPurple, -1);
			return true;
		}
	}
}
