using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.CoilSet;
using Microsoft.Xna.Framework;
using System.Linq;

namespace SpiritMod.Items.Accessory.UnstableTeslaCoil
{
	[AutoloadEquip(EquipType.Back)]
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

			if (player.whoAmI == Main.myPlayer && player.miscCounter % 100 == 0)
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
			var targets = Main.npc.Where(x => x.active && player.DistanceSQ(x.Center) <= 300f * 300f && x.CanBeChasedBy());

			foreach (NPC target in targets)
			{
				Projectile.NewProjectile(player.GetSource_FromThis(), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Unstable_Tesla_Coil_Projectile>(), 18, 0f, player.whoAmI, target.position.X, target.position.Y);

				if (npcsHit == 0)
				{
					for (int i = 0; i < 10; i++)
					{
						Vector2 pos = player.position + new Vector2(player.width * Main.rand.NextFloat(), player.height);
						float magnitude = Main.rand.NextFloat();

						Dust.NewDustPerfect(pos, DustID.Electric, magnitude * -3f * Vector2.UnitY, 0, default, MathHelper.Max(1f - magnitude, .3f));
					} //Spawn additional dusts
				}
				if (++npcsHit > 3)
					break;
			}
		}
	}
}