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
using Terraria.Localization;

namespace SpiritMod.Items.Consumable.Potion
{
	public class MoonJellyDonut : FoodItem
	{
		internal override Point Size => new(34, 20);

		public override void Defaults()
		{
			Item.rare = ItemRarityID.Pink;
			Item.maxStack = Item.CommonMaxStack;
			Item.potion = true;
			Item.healLife = 180;
		}

		public override bool CanUseItem(Player player) => player.FindBuffIndex(BuffID.PotionSickness) == -1;
		public override void UpdateInventory(Player player) => Item.healLife = 180; //update the heal life back to 180 for tooltip and quick heal purposes

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) //pulsating glow effect in world
		{
            Lighting.AddLight(Item.position, 0.08f, .4f, .28f);
            Texture2D texture;
            texture = TextureAssets.Item[Item.type].Value;
            spriteBatch.Draw
            (
                Mod.Assets.Request<Texture2D>("Items/Consumable/Potion/MoonJellyDonut_Glow").Value,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height / 3 * 0.5f + 2f
                ),
				null,
                Color.White,
                rotation,
				Item.Size * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Consumable/Potion/MoonJellyDonut_Glow").Value,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height / 3 * 0.5f + 2f
                ),
                null, 
				Color.Lerp(Color.White, Color.Transparent, 0.75f), 
				rotation, 
				Item.Size / 2, 
				MathHelper.Lerp(1f, 1.2f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3) / 2 + 0.5f), 
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

			player.AddBuff(ModContent.BuffType<MoonBlessingDonut>(), 900);
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			foreach (TooltipLine line in tooltips.Where(x => x.Mod == "Terraria" && x.Name == "HealLife"))
				line.Text = Language.GetTextValue("Mods.SpiritMod.Items.MoonJellyDonut.CustomTooltip");
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient(ModContent.ItemType<MoonJelly>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Sets.SeraphSet.MoonStone>(), 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
