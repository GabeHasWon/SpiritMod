using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GranitechSet
{
	public class GranitechMaterial : ModItem
	{
		private int subID = -1; //Controls the in-world sprite for this item

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("G-TEK Components");
			Tooltip.SetDefault("'An impressive combination of magic and science'");
		}

		public override void SetDefaults()
		{
			Item.value = Item.sellPrice(silver: 20);
			Item.maxStack = 999;
			Item.rare = ItemRarityID.Pink;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			if (subID != -1)
			{
				if (subID == 0)
					Item.width = 24;
				else
					Item.width = 28;

				if (subID == 2)
					Item.height = 36;
				else
					Item.height = 20;
			}
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (subID == -1)
				subID = Main.rand.Next(3);

			Texture2D tex = ModContent.Request<Texture2D>(Texture + "_World").Value;
			spriteBatch.Draw(tex, Item.position - Main.screenPosition, new Rectangle(0, 38 * subID, 28, 36), lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
			return false;
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			subID = -1;
			return true;
		}
	}
}
