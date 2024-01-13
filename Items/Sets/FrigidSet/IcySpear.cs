using SpiritMod.Projectiles.Thrown.Charge;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritMod.Items.Sets.FrigidSet
{
	public class IcySpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frigid Javelin");
			// Tooltip.SetDefault("Hold and release to throw\nHold longer for more velocity and damage\nInflicts Frostburn");
		}

		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.knockBack = 6f;
			Item.crit = 6;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.channel = true;
			Item.autoReuse = false;
			Item.rare = ItemRarityID.Blue;
			Item.width = Item.height = 18;
			Item.value = 16000;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 24;
			Item.DamageType = DamageClass.Melee;
			Item.shoot = ModContent.ProjectileType<FrigidJavelinProj>();
			Item.shootSpeed = 1f;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			StatModifier meleeStat = Main.LocalPlayer.GetTotalDamage(DamageClass.Melee);

			foreach (TooltipLine line in tooltips)
			{
				if (line.Mod == "Terraria" && line.Name == "Damage") //Replace the vanilla text with our own
					line.Text = $"{(int)meleeStat.ApplyTo(Item.damage)}-{(int)meleeStat.ApplyTo(Item.damage * JavelinProj.maxDamageMult)}" + Language.GetText("LegacyTooltip.2");
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrigidFragment>(), 9);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
