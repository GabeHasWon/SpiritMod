using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Localization;

namespace SpiritMod.Items.Accessory.MoonlightSack
{
	public class Moonlight_Sack : ModItem
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Moonlight Sack");

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 34;
			Item.value = Item.sellPrice(gold: 3);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.player[Item.playerIndexTheItemIsReservedFor];
			float num7 = 5E-06f;
			float num8 = player.GetDamage(DamageClass.Summon).ApplyTo(15);
			var aga = new TooltipLine(Mod, "summonDamageText", (int)(num8+num7) + "" + Language.GetText("LegacyTooltip.53"));
			tooltips.Add(aga);
			var aga2 = new TooltipLine(Mod, "summonDamageText2", Language.GetTextValue("Mods.SpiritMod.Items.Moonlight_Sack.CustomTooltip"));
			tooltips.Add(aga2);
		}

		public override void UpdateEquip(Player player) => player.GetSpiritPlayer().moonlightSack = true;

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) 
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			SpriteEffects spriteEffects = SpriteEffects.None;
			Lighting.AddLight(new Vector2(Item.Center.X, Item.Center.Y), 0.075f, 0.231f, 0.255f);
			var vector2_3 = new Vector2(texture.Width / 2, texture.Height / 1 / 2);
			spriteBatch.Draw(texture, Item.Center - Main.screenPosition, texture.Frame(), lightColor, rotation, vector2_3, Item.scale, spriteEffects, 0);

			Vector2 drawOff = new Vector2(0, -2f);
			
			int num7 = 5;
			float num9 = (float) (Math.Cos(Main.GlobalTimeWrappedHourly % 2.40000009536743 / 2.40000009536743 * 6.28318548202515) / 1.0 + 0.5);
			Vector2 bb = Item.Center - Main.screenPosition - new Vector2(texture.Width, texture.Height / 1) * Item.scale / 2f + vector2_3 * Item.scale + drawOff;
			Color color2 = new Color(sbyte.MaxValue - Item.alpha, sbyte.MaxValue - Item.alpha, sbyte.MaxValue - Item.alpha, 0).MultiplyRGBA(Color.White);
			
			for (int index2 = 0; index2 < num7; ++index2)
			{
				Vector2 position2 = Item.Center + ((float) (index2 / (double)num7 * 6.28318548202515) + rotation).ToRotationVector2() * (float) (2.0 * (double)2.0) - Main.screenPosition - new Vector2(texture.Width, texture.Height / 1) * Item.scale / 2f + vector2_3 * Item.scale + drawOff;
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Accessory/MoonlightSack/Moonlight_Sack_Glow").Value, position2, texture.Frame(), Item.GetAlpha(color2), rotation, vector2_3, Item.scale, spriteEffects, 0.0f);
			}
			
			for (int index2 = 0; index2 < 4; ++index2)
			{
				Vector2 pos2 = Item.Center + ((float) (index2 / (double) 4 * 6.28318548202515) + rotation).ToRotationVector2() * (float) (2.0 * (double) num9 + 2.0) - Main.screenPosition - new Vector2(texture.Width, texture.Height / 1) * Item.scale / 2f + vector2_3 * Item.scale + drawOff;
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Accessory/MoonlightSack/Moonlight_Sack_Glow").Value, pos2, texture.Frame(), color2, rotation, vector2_3, Item.scale, spriteEffects, 0.0f);
			}
			
			Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Accessory/MoonlightSack/Moonlight_Sack_Glow").Value, bb, texture.Frame(), color2, rotation, vector2_3, Item.scale, spriteEffects, 0.0f);
			return false;
		}
	}
}