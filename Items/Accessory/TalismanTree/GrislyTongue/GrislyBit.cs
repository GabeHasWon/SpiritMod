using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.GlobalClasses.Players;


namespace SpiritMod.Items.Accessory.TalismanTree.GrislyTongue
{
	[Sacrifice(0)]
	internal class GrislyBit : ModItem
	{
		private int subID = 0; //Controls the in-world sprite for this item
		private int timeLeft = 180;

		public override void SetStaticDefaults()
		{
			ItemID.Sets.IgnoresEncumberingStone[Type] = true;
		}
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 14;
			Item.maxStack = 1;
			subID = Main.rand.Next(3);
		}
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D tex = ModContent.Request<Texture2D>(Texture + "_World").Value;
			spriteBatch.Draw(tex, Item.position - Main.screenPosition, new Rectangle(0, 16 * subID, 20, 14), GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
			return false;
		}
		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			timeLeft--;
			if (timeLeft <= 5)
			{
				for (int i = 0; i < 5; ++i)
					Dust.NewDust(Item.position, Item.width, Item.height, DustID.Blood, 1f, 1f, 100, default, 1.5f);
				SoundEngine.PlaySound(SoundID.Item167, Item.Center);
				Item.active = false;
			}
		}
		public override bool CanPickup(Player player)
		{
			if (timeLeft >= 150)
				return false;
			else
				return true;
		}
		public override bool ItemSpace(Player player) => true;
		public override bool OnPickup(Player player)
		{
			SoundEngine.PlaySound(SoundID.Item171);
			player.Heal(player.GetModPlayer<TalismanPlayer>().totemHealthToRecover);
			return false;
		}
	}
}