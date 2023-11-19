using SpiritMod.NPCs.Tides.Tide;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class BlackPearl : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.reuseDelay = 10;
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = false;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.UseSound = SoundID.Item43;
		}

		public override bool CanUseItem(Player player)
		{
			if ((player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && !Main.pumpkinMoon && !Main.snowMoon)
				return false;

			if (!player.ZoneBeach)
			{
				Main.NewText(Language.GetTextValue("Mods.SpiritMod.Events.TheTide.NotOcean"), 85, 172, 247);
				return false;
			}
			else if (TideWorld.TheTide)
			{
				Main.NewText(Language.GetTextValue("Mods.SpiritMod.Events.TheTide.AlreadyActive"), 85, 172, 247);
				return false;
			}
			return true;
		}

		public override bool? UseItem(Player player)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
				SpiritMod.WriteToPacket(SpiritMod.Instance.GetPacket(), (byte)MessageType.StartTide).Send();
			else
			{
				TideWorld.TheTide = true;
				TideWorld.TideWaveIncrease();
			}
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Coral, 5);
			recipe.AddIngredient(ItemID.Bone, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}