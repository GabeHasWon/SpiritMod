using SpiritMod.Projectiles.Thrown.Charge;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritMod.Items.Weapon.Thrown
{
	public class ClatterSpear : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.knockBack = 8f;
			Item.crit = 6;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.channel = true;
			Item.autoReuse = false;
			Item.rare = ItemRarityID.Blue;
			Item.width = Item.height = 18;
            Item.value = 22000;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 24;
			Item.DamageType = DamageClass.Melee;
			Item.shoot = ModContent.ProjectileType<ClatterJavelinProj>();
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
	}
}