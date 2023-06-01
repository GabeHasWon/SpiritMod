
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops
{
	public class InfernalPact : ModItem
	{
		private int sineTimer = 0; //maybe there's a better way to do this idk

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Infernal Pact");
			Tooltip.SetDefault("4% increased damage\nReduces your defense to 0\nIncreases damage by 0.75% per defense point lost");
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
			Item.expert = true; 
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Generic) *= 1 + (player.statDefense * 0.0075f);
			player.statDefense = 0;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}