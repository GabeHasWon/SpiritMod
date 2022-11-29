using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable.Potion
{
	public class BottomlessAle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bottomless Ale");
			Tooltip.SetDefault("Non-consumable\nMinor improvements to melee stats & lowered defense\n'Down the hatch!'");
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 34;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 1;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.holdStyle = ItemHoldStyleID.HoldFront;
			Item.useTime = Item.useAnimation = 20;
			Item.consumable = false;
			Item.autoReuse = false;
			Item.buffType = BuffID.Tipsy;
			Item.buffTime = 7200;
			Item.UseSound = SoundID.Item3;
		}

		public override void HoldStyle(Player player, Rectangle heldItemFrame)
		{
			player.itemLocation.X -= 8 * player.direction;
			player.itemLocation.Y += 10 * (int)player.gravDir;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

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
	}
}
