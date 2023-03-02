using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Consumable;
using SpiritMod.Items.Placeable;
using SpiritMod.NPCs.Boss.SteamRaider;
using SpiritMod.Particles;
using SpiritMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient
{
	[TileTag(TileTags.Indestructible)]
	public class StarBeacon : ModTile
	{
		private float alphaCounter = 1;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Astralite Beacon");
			AddMapEntry(new Color(50, 70, 150), name);
			TileID.Sets.DisableSmartCursor[Type] = true;
			DustType = DustID.Electric;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = .12f;
			g = .3f;
			b = 0.5f;
		}

		public override bool CanExplode(int i, int j) => false;

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			TileUtilities.BlockActuators(i, j);
			return base.TileFrame(i, j, ref resetFrame, ref noBreak);
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
				zero = Vector2.Zero;

			if (tile.TileFrameX % 36 == 0 && tile.TileFrameY % 36 == 0)
				DrawSpecialFX(new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero + new Vector2(16), spriteBatch);

			return true;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
				zero = Vector2.Zero;

			int height = tile.TileFrameY == 36 ? 18 : 16;
			spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Ambient/StarBeacon_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			if (Main.rand.NextBool(10) && !Main.dedServ && tile.TileFrameX % 36 == 0 && tile.TileFrameY % 36 == 0)
			{
				if (Mechanics.EventSystem.EventManager.IsPlaying<Mechanics.EventSystem.Events.StarplateBeaconIntroEvent>())
					return;

				Vector2 position = (new Vector2(i, j) * 16) + new Vector2(16 + Main.rand.NextFloat(-8.0f, 8.0f), 16);
				ParticleHandler.SpawnParticle(new GlowParticle(position, Vector2.UnitY * -Main.rand.NextFloat(0.3f, 1.8f), PulseColor, Main.rand.NextFloat(0.03f, 0.1f), Main.rand.Next(25, 50)));
			}
		}

		private void DrawSpecialFX(Vector2 position, SpriteBatch spriteBatch) //WIP
		{
			Texture2D extra = Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_60").Value;
			Texture2D ray = Mod.Assets.Request<Texture2D>("Textures/Ray").Value;
			Texture2D head = Mod.Assets.Request<Texture2D>("Tiles/Ambient/StarBeacon_Head").Value;

			bool playingIntro = Mechanics.EventSystem.EventManager.IsPlaying<Mechanics.EventSystem.Events.StarplateBeaconIntroEvent>();

			float flickerTime = ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 20) % .25f) + 1f;

			Color color = PulseColor * flickerTime;
			if (playingIntro)
			{
				if (alphaCounter > 0)
					alphaCounter -= 0.025f;

				color = Color.Red * 0.8f * alphaCounter;
			}
			else if (alphaCounter < 1)
				alphaCounter += 0.1f;

			for (int i = 0; i < 3; i++)
				spriteBatch.Draw(extra, position, null, (color * .5f) with { A = 0 }, 0f, extra.Size() / 2, 0.45f, SpriteEffects.None, 0);
			spriteBatch.Draw(ray, position, null, color * .5f, 3.14f, new Vector2(ray.Width / 2, 0), new Vector2(1, .5f), SpriteEffects.None, 0);

			int columns = 2;
			int rows = 8;
			Rectangle frame = new Rectangle(0, head.Height / rows * ((int)(Main.timeForVisualEffects / 4)) % head.Height, (head.Width / columns) - 2, (head.Height / rows) - 2);
			if (Main.rand.NextBool(45) || playingIntro) //Display a "glitched" frame
				frame.X = head.Width / columns;

			DrawAberration.DrawChromaticAberration(Vector2.UnitX, 1f, delegate (Vector2 offset, Color colorMod)
			{
				spriteBatch.Draw(head, position - new Vector2(0, 40) + offset, frame, color.MultiplyRGBA(colorMod), 0f, frame.Size() / 2, 1f, SpriteEffects.None, 0);
			});

			for (int i = 0; i < 4; i++)
			{
				(float, byte, float) stats = i switch
				{
					1 => (2.2f, 20, 0.1f),
					2 => (1, 30, -0.15f),
					3 => (1.2f, 34, 0.3f),
					_ => (0.5f, 25, 0)
				};

				DrawSatellites(position, spriteBatch, color, stats.Item1, stats.Item2, stats.Item3);
			}
		}

		private void DrawSatellites(Vector2 position, SpriteBatch spriteBatch, Color color, float rate = 1f, int range = 30, float rotation = 0)
		{
			Texture2D texture = Mod.Assets.Request<Texture2D>("Effects/Masks/CircleGradient").Value;

			float angle = Main.GlobalTimeWrappedHourly * rate;
			Vector2 offset = new Vector2(range, 0).RotatedBy(angle);
			offset.Y *= .2f;

			float quoteant = (float)((position + offset).Y - position.Y) / range;
			float scale = .03f + (.07f * quoteant);
			position += offset.RotatedBy(rotation) - new Vector2(0, 40);

			DrawAberration.DrawChromaticAberration(Vector2.UnitX, 1f, delegate (Vector2 offset, Color colorMod)
			{
				spriteBatch.Draw(texture, position + offset, null, color.MultiplyRGBA(colorMod) * quoteant * 2, angle + 3.14f, texture.Size() / 2, scale, SpriteEffects.None, 0);
			});
		}

		private static Color PulseColor => Color.Lerp(new Color(0, 100, 255), Color.Orange, (float)Math.Sin(Main.GlobalTimeWrappedHourly) / 1f);

		//public override bool CanKillTile(int i, int j, ref bool blockDamaged) => MyWorld.downedRaider; //The tile is indestructable
		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 48, ModContent.ItemType<StarBeaconItem>());
			SoundEngine.PlaySound(SoundID.NPCHit4);
		}

		public override void MouseOver(int i, int j)
		{
			Main.player[Main.myPlayer].cursorItemIconEnabled = true;
			Main.player[Main.myPlayer].cursorItemIconID = ModContent.ItemType<StarWormSummon>();
		}

		public override bool RightClick(int i, int j)
		{
			for (int k = 0; k < Main.npc.Length; k++)
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<SteamRaiderHead>()) return false;

			if (Mechanics.EventSystem.EventManager.IsPlaying<Mechanics.EventSystem.Events.StarplateBeaconIntroEvent>())
				return false;

			Player player = Main.player[Main.myPlayer];
			if (player.HasItem(ModContent.ItemType<StarWormSummon>()))
			{
				int x = i;
				int y = j;
				while (Main.tile[x, y].TileType == Type) x--;
				x++;
				while (Main.tile[x, y].TileType == Type) y--;
				y++;

				Mechanics.EventSystem.EventManager.PlayEvent(new Mechanics.EventSystem.Events.StarplateBeaconIntroEvent(new Vector2(x * 16f + 16f, y * 16f + 12f)));
			}
			return false;
		}
	}
}