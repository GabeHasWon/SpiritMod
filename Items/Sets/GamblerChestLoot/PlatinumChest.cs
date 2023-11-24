using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.GamblerChestLoot.GamblerChestNPCs;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Sets.GamblerChestLoot
{
	public class PlatinumChest : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.value = Item.buyPrice(gold: 50);
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = Item.CommonMaxStack;
			Item.autoReuse = true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			int npcType = ModContent.NPCType<PlatinumChestBottom>();
			Vector2 position = player.Center + (Vector2.UnitX * player.direction * 30);

			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				NPC.NewNPC(player.GetSource_OpenItem(Item.type, "RightClick"), (int)position.X, (int)position.Y, npcType);
			}
			else if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
			{
				ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.SpawnNPCFromClient, 3);
				packet.Write(npcType);
				packet.Write((int)position.X);
				packet.Write((int)position.Y);
				packet.Send();
			}
		}
	}
}
