
using SpiritMod.NPCs.Reach;
using SpiritMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class GladeWreath : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glade Wreath");
			Tooltip.SetDefault("Summons a Glade Wraith\nCan only be used in the Briar");
		}

		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 99;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = Item.useAnimation = 20;
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = false;
			Item.UseSound = SoundID.Item43;
		}

		public override bool CanUseItem(Player player) => !NPC.AnyNPCs(ModContent.NPCType<ForestWraith>()) && player.ZoneBriar();

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Zombie7, player.position);

				int type = ModContent.NPCType<ForestWraith>();
				int who = NPC.NewNPC(player.GetSource_ItemUse(Item), (int)player.Center.X, (int)player.Center.Y - 200, type);

				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, who);
			}

			return true;
		}
	}
}
