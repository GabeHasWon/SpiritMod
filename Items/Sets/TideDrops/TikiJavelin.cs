using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Thrown.Charge;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.TideDrops
{
	public class TikiJavelin : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Tiki Javelin");
			// Tooltip.SetDefault("Hold and release to throw\nHold it longer for more velocity and damage");
		}

		public override void SetDefaults()
		{
			Item.damage = 40;
			Item.knockBack = 8;
			Item.crit = 6;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.channel = true;
			Item.autoReuse = false;
			Item.rare = ItemRarityID.Orange;
			Item.width = Item.height = 18;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 24;
			Item.DamageType = DamageClass.Melee;
			Item.shoot = ModContent.ProjectileType<TikiJavelinProj>();
			Item.shootSpeed = 1f;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, .42f, .29f, .10f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			StatModifier meleeStat = Main.LocalPlayer.GetTotalDamage(DamageClass.Melee);

			foreach (TooltipLine line in tooltips)
			{
				if (line.Mod == "Terraria" && line.Name == "Damage") //Replace the vanilla text with our own
					line.Text = $"{(int)meleeStat.ApplyTo(Item.damage)}-{(int)meleeStat.ApplyTo(Item.damage * JavelinProj.maxDamageMult)} melee damage";
			}
		}
	}
}