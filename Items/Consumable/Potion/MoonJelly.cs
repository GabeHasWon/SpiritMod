using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Potion;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Potion
{
	public class MoonJelly : FoodItem
	{
		internal override Point Size => new(34, 26);

		public override void StaticDefaults()
		{
			DisplayName.SetDefault("Moon Jelly");

			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

		public override void Defaults()
		{
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 30;
			Item.potion = true;
			Item.healLife = 120;
			Item.UseSound = SoundID.Item3;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;
		public override bool CanUseItem(Player player) => player.FindBuffIndex(BuffID.PotionSickness) < 0;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) //pulsating glow effect in world
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;
			Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height / 3);

			spriteBatch.Draw(texture, 
				Item.Center - Main.screenPosition,
				frame, 
				Color.Lerp(Color.White, Color.Transparent, 0.75f), 
				rotation,
				frame.Size() / 2, 
				MathHelper.Lerp(1f, 1.3f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3) / 2 + 0.5f), 
				SpriteEffects.None, 
				0);
		}

		public override bool? UseItem(Player player)
		{
			Item.healLife = 0; //set item's heal life to 0 when actually used, so it doesnt heal player
			if (!player.pStone)
				player.AddBuff(BuffID.PotionSickness, 3600);
			else
				player.AddBuff(BuffID.PotionSickness, 2700);

			player.AddBuff(ModContent.BuffType<MoonBlessing>(), 600);
			return true;
		}

		public override void UpdateInventory(Player player) => Item.healLife = 120; //update the heal life back to 120 for tooltip and quick heal purposes

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			foreach(TooltipLine line in tooltips.Where(x => x.Mod == "Terraria" && x.Name == "HealLife")) {
				line.Text = "Restores 150 life over 10 seconds\nCauses Potion Sickness";
			}
		}
	}
}
