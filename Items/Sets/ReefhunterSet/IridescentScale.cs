using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ReefhunterSet
{
	public class IridescentScale : ModItem
	{
		private int subID = -1; //Controls the in-world sprite for this item

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iridescent Scale");
			Tooltip.SetDefault("'Glints beautifully under the water'");
		}

		public override void SetDefaults()
		{
			Item.value = 100;
			Item.maxStack = 999;
			Item.rare = ItemRarityID.Blue;
			Item.width = 26;
			Item.height = 28;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			if (subID != -1)
			{
				Item.height = subID switch
				{
					0 => 20,
					1 => 32,
					_ => 28
				};
			}
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (subID == -1)
				subID = Main.rand.Next(3);

			Texture2D tex = ModContent.Request<Texture2D>(Texture + "_World").Value;
			spriteBatch.Draw(tex, Item.position - Main.screenPosition, new Rectangle(0, 34 * subID, 26, 32), GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
			return false;
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			subID = -1;
			return true;
		}
	}
}
