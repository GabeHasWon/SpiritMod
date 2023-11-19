using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ThrownMisc.FlaskofGore
{
	[Sacrifice(0)]
	public class CrimsonSkull : ModItem
	{
		private int timeLeft = 8 * 60;

		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 7));
			ItemID.Sets.AnimatesAsSoul[Type] = true;
			ItemID.Sets.ItemNoGravity[Type] = true;
			ItemID.Sets.IgnoresEncumberingStone[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 32;
			Item.maxStack = 1;
			Item.alpha = 50;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			timeLeft--;
			if (timeLeft <= 0)
				Item.active = false;

			int fadeTime = 20;
			if (timeLeft < fadeTime)
				Item.alpha = Math.Min(Item.alpha + (255 / fadeTime), 255);
		}

		public override bool ItemSpace(Player player) => true;

		public override void GrabRange(Player player, ref int grabRange) => grabRange = 0;

		public override bool OnPickup(Player player)
		{
			player.AddBuff(Mod.Find<ModBuff>("CrimsonSkullBuff").Type, 240);
			SoundEngine.PlaySound(SoundID.DD2_DrakinDeath with { Volume = 0.8f }, player.Center);
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			return true;
		}
	}
}
