using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Accessory.TalismanTree.GildedScarab;
using SpiritMod.Items.Accessory.TalismanTree.SlagMedallion;
using SpiritMod.GlobalClasses.Players;
using Terraria.DataStructures;

namespace SpiritMod.DrawLayers
{
	internal class ScarabFrontLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ProjectileOverArm);
		protected override void Draw(ref PlayerDrawSet drawInfo) //reference sharkbonesExplosion and use DrawAnimationVerticalRect or reference how its done in inferno potion
		{
			if (drawInfo.shadow != 0f)
				return;
			Player drawPlayer = drawInfo.drawPlayer;

			if (drawPlayer.HasBuff(ModContent.BuffType<GildedScarab_buff>()) && ((drawPlayer.GetModPlayer<TalismanPlayer>().scarabTimer <= 12) || (drawPlayer.GetModPlayer<TalismanPlayer>().scarabTimer >= 24)))
			{
				Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/Accessory/TalismanTree/GildedScarab/GildedScarab_player").Value;
				Vector2 size = new Vector2(68, 54);
				Vector2 origin = size / 2;
				Vector2 drawPos = drawPlayer.Center;
				Point tileLocation = Main.LocalPlayer.Center.ToTileCoordinates();

				DrawData drawData = new DrawData(texture, drawPos - Main.screenPosition, new Rectangle(0, 56 * (drawPlayer.GetModPlayer<TalismanPlayer>().scarabTimer / 4), 68, 54), Lighting.GetColor(tileLocation), 0, origin, 1, SpriteEffects.None, 0);
				drawInfo.DrawDataCache.Add(drawData);
			}
		}
	}

	internal class ScarabBackLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ForbiddenSetRing);
		protected override void Draw(ref PlayerDrawSet drawInfo) //reference sharkbonesExplosion and use DrawAnimationVerticalRect or reference how its done in inferno potion
		{
			if (drawInfo.shadow != 0f)
				return;

			Player drawPlayer = drawInfo.drawPlayer;
			if (drawPlayer.HasBuff(ModContent.BuffType<GildedScarab_buff>()) && ((drawPlayer.GetModPlayer<TalismanPlayer>().scarabTimer >= 13) && drawPlayer.GetModPlayer<TalismanPlayer>().scarabTimer <= 23))
			{
				Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/Accessory/TalismanTree/GildedScarab/GildedScarab_player").Value;
				Vector2 size = new Vector2(68, 54); //frame size
				Vector2 origin = size / 2;
				Vector2 drawPos = drawPlayer.Center;
				Point tileLocation = Main.LocalPlayer.Center.ToTileCoordinates();

				DrawData drawData = new DrawData(texture, drawPos - Main.screenPosition, new Rectangle(0, 56 * (drawPlayer.GetModPlayer<TalismanPlayer>().scarabTimer / 4), 68, 54), Lighting.GetColor(tileLocation), 0, origin, 1, SpriteEffects.None, 0);
				drawInfo.DrawDataCache.Add(drawData);
			}
		}
	}

	internal class SlagLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);
		protected override void Draw(ref PlayerDrawSet drawInfo) //reference sharkbonesExplosion and use DrawAnimationVerticalRect or reference how its done in inferno potion
		{
			if (drawInfo.shadow != 0f)
				return;

			Player drawPlayer = drawInfo.drawPlayer;
			if (drawPlayer.HasBuff(ModContent.BuffType<SlagFury_buff>()))
			{
				Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/Accessory/TalismanTree/SlagMedallion/SlagFury_Player").Value;
				Vector2 size = new Vector2(70, 48);
				Vector2 origin = size / 2;
				Vector2 offset = new Vector2(0, 48);
				Vector2 drawPos = drawPlayer.Center - offset;

				DrawData drawData = new DrawData(texture, drawPos - Main.screenPosition, new Rectangle(0, 50 * (drawPlayer.GetModPlayer<TalismanPlayer>().slagTimer / 4), 70, 48), Color.White, 0, origin, 1, SpriteEffects.None, 0);
				drawInfo.DrawDataCache.Add(drawData);

				if (Main.rand.NextBool(4))
				{
					int dust = Dust.NewDust(drawPlayer.position - offset, 70, 48, DustID.Torch, 0f, 0f, 200, Color.Red, 1.15f);
				}
			}
		}
	}
}