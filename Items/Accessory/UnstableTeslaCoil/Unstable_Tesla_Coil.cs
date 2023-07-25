using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.CoilSet;

namespace SpiritMod.Items.Accessory.UnstableTeslaCoil
{
	public class Unstable_Tesla_Coil : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Unstable Tesla Coil");
			Tooltip.SetDefault("Electrocutes up to 3 nearby enemies\nIncreases pickup range for ores");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 30;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 2;
			Item.accessory = true;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetSpiritPlayer().teslaCoil = true;

			if (player.whoAmI == Main.myPlayer)
				TeslaStrike(player);
		}

		public override void AddRecipes()
		{
			var recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<TechDrive>(), 14);
			recipe.AddIngredient(ItemID.Wire, 4);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

		private static void TeslaStrike(Player player)
		{
			int npcsHit = 0;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				NPC npc = Main.npc[i];

				if (npc.active && player.DistanceSQ(npc.Center) <= 300f * 300f && npc.CanBeChasedBy())
				{
					if (player.miscCounter % 100 == 0)
					{
						int p = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Unstable_Tesla_Coil_Projectile>(), 18, 0f, player.whoAmI);
						Main.projectile[p].ai[0] = npc.position.X;
						Main.projectile[p].ai[1] = npc.position.Y;
						Main.projectile[p].netUpdate = true;
					}

					if (npcsHit++ > 3)
						break;
				}
			}
		}
	}
}