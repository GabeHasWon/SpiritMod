using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritMod.Items.Consumable.Potion
{
	[Sacrifice(1)]
	public class BottomlessHealingPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BottomlessAle>();
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 34;
			Item.rare = ItemRarityID.LightRed;
			Item.maxStack = 1;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = Item.useAnimation = 20;
			Item.consumable = false;
			Item.autoReuse = false;
			Item.potion = true;
			Item.healLife = 120;
			Item.UseSound = SoundID.Item3;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;
		public override bool CanUseItem(Player player) => player.FindBuffIndex(BuffID.PotionSickness) < 0;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) //pulsating glow effect in world
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;
			Rectangle rect = new Rectangle(0, texture.Height / Main.itemAnimations[Item.type].FrameCount * Main.itemAnimations[Item.type].Frame, 
				texture.Width, texture.Height / Main.itemAnimations[Item.type].FrameCount);

			spriteBatch.Draw(TextureAssets.Item[Item.type].Value, 
				Item.Center - Main.screenPosition,
				rect, 
				Color.Lerp(Color.White, Color.Transparent, 0.75f), 
				rotation,
				rect.Size() / 2,
				MathHelper.Lerp(1f, 1.3f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3) / 2 + 0.5f), 
				SpriteEffects.None, 
				0);
		}

		public override bool? UseItem(Player player)
		{
			if (!player.pStone)
				player.AddBuff(BuffID.PotionSickness, 3600);
			else
				player.AddBuff(BuffID.PotionSickness, 2700);

            if (player.statLife == player.statLifeMax2)
                return false;
			return true;
		}

		public override void GetHealLife(Player player, bool quickHeal, ref int healValue) => healValue = 100;
		public override bool ConsumeItem(Player player) => false;

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			foreach (TooltipLine line in tooltips.Where(x => x.Mod == "Terraria" && x.Name == "HealLife")) {
				line.Text = Language.GetTextValue("Mods.SpiritMod.Items.BottomlessHealingPotion.CustomTooltip");
			}
		}
	}
}
