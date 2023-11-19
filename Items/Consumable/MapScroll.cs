using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(2)]
	public class MapScroll : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 40;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 1000;
			Item.rare = ItemRarityID.White;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
		}

		public override bool? UseItem(Player player)
		{
			if (Main.myPlayer == player.whoAmI && Main.netMode != NetmodeID.Server)
			{
				CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 10, player.width, player.height), new Color(255, 255, 255, 100), "Map Revealed");

				Point center = (Main.LocalPlayer.Center / 16).ToPoint();

				int range = 180;

				for (int i = center.X - range / 2; i < center.X + range / 2; i++)
				{
					for (int j = center.Y - range / 2; j < center.Y + range / 2; j++)
					{
						bool inWorldBorder = i < (Main.maxTilesX - 40) && i >= 40 && j < (Main.maxTilesY - 40) && j >= 40;
						if (WorldGen.InWorld(i, j) && inWorldBorder)
							Main.Map.Update(i, j, 255);
					}
				}

				Main.refreshMap = true;

				for (int k = 0; k < 10; k++)
				{
					int dust = Dust.NewDust(player.Center, player.width, player.height, DustID.PortalBolt);
					Vector2 vector2_1 = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * (Main.rand.Next(50, 100) * 0.04f);
					Main.dust[dust].velocity = vector2_1;
					Main.dust[dust].noGravity = true;
					Main.dust[dust].position = player.Center - Vector2.Normalize(vector2_1) * 34f;
				}
			}
			return true;
		}
	}
}