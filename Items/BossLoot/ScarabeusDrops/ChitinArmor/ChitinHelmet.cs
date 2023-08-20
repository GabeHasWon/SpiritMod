using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Dusts;
using SpiritMod.GlobalClasses.Players;

namespace SpiritMod.Items.BossLoot.ScarabeusDrops.ChitinArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class ChitinHelmet : ModItem
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Chitin Faceguard");

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = Item.sellPrice(silver: 14);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 3;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<ChitinChestplate>() && legs.type == ModContent.ItemType<ChitinLeggings>();

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "Double tap in a direction to dash and envelop yourself in a tornado";
			player.GetModPlayer<DashPlayer>().chitinSet = true;

			if (player.velocity.X != 0f && player.velocity.Y == 0f && !player.mount.Active)
			{
				int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + player.height - 4f), player.width, 0, DustID.Dirt);
				Main.dust[dust].velocity = (Vector2.UnitX * Main.rand.NextFloat(0.0f, 2.0f) * -player.direction).RotatedBy(Main.rand.NextFloat(0.0f, 0.8f) * player.direction);
				Main.dust[dust].noGravity = true;
			}

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<Chitin>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

		public static void ChitinDashVisuals(Player player, out float speedCap, out float decayCapped, out float speedMax, out float decayMax, out int delay)
		{
			for (int k = 0; k < 2; k++)
			{
				int dust;
				if (player.velocity.Y == 0f)
					dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + player.height - 4f), player.width, 8, ModContent.DustType<SandDust>(), 0f, 0f, 100, default, 1.4f);
				else
					dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + (player.height >> 1) - 8f), player.width, 16, ModContent.DustType<SandDust>(), 0f, 0f, 100, default, 1.4f);

				Main.dust[dust].velocity *= 0.1f;
				Main.dust[dust].scale *= 1f + Main.rand.Next(20) * 0.01f;
			}

			player.GetModPlayer<DashPlayer>().chitinDashTicks = 15;
			player.noKnockback = true;
			speedCap = 30;
			speedMax = 11f;
			decayCapped = 0.95f;
			decayMax = decayCapped;
			delay = 25;
		}
	}
}
