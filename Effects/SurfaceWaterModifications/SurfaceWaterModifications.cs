using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using SpiritMod.Effects.Waters;
using SpiritMod.Mechanics.OceanWavesSystem;
using SpiritMod.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.Shaders;
using Terraria.Graphics;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.Effects.SurfaceWaterModifications
{
	public class SurfaceWaterModifications
	{
		internal static Effect transparencyEffect = null;
		internal static Texture2D rippleTex = null;
		internal static int leftOceanHeight = 0;
		internal static int rightOceanHeight = 0;

		static Hook LoadGlobalHook;

		private static bool ImproperWaterStyle => Main.waterStyle != WaterStyleID.Count && Main.waterStyle != WaterStyleID.Purity;

		public static ILog Logger => ModContent.GetInstance<SpiritMod>().Logger;
		public static bool PlayerInValidOcean => Main.LocalPlayer.ZoneBeach && !ImproperWaterStyle;// && Main.waterStyle == WaterStyleID.Purity

		public static void Load()
		{
			if (ModContent.GetInstance<SpiritClientConfig>().SurfaceWaterTransparency)
			{
				IL_Main.DoDraw += AddWaterShader; //Transparency shader

				//Slope fix garbage
				IL_LiquidRenderer.DrawNormalLiquids += LiquidRenderer_InternalDraw; //Draw over slopes
				On_TileDrawing.DrawPartialLiquid += On_TileDrawing_DrawPartialLiquid; //Only draw vanilla slopes in certain cases
				IL_Main.DrawBlack += HideSlopedBlack;
			}

			IL_WaterShaderData.QueueRipple_Vector2_Color_Vector2_RippleShape_float += IncreaseRippleSize; //Makes ripple bigger
			IL_WaterShaderData.DrawWaves += WaterShaderData_DrawWaves;

			if (!Main.dedServ)
			{
				transparencyEffect = ModContent.Request<Effect>("SpiritMod/Effects/SurfaceWaterModifications/SurfaceWaterFX", AssetRequestMode.ImmediateLoad).Value;
				rippleTex = ModContent.Request<Texture2D>("Terraria/Images/Misc/Ripples", AssetRequestMode.ImmediateLoad).Value;
			}

#if DEBUG
			// This is for testing around an issue that happened with a couple of my characters, bricking them.
			// This fixes the problem but shouldn't be run in production since it's hacky and may hide other issues.
			LoadGlobalHook = new Hook(typeof(ItemIO).GetMethod("LoadGlobals", BindingFlags.NonPublic | BindingFlags.Static), HijackLoadGlobals);
			LoadGlobalHook.Apply();
#endif
		}
		
		private static void HijackLoadGlobals(Action<Item, IList<TagCompound>> orig, Item item, IList<TagCompound> list)
		{
			try
			{
				orig(item, list);
			}
			catch (Exception e)
			{
				SpiritMod.Instance.Logger.Warn($"SurfaceWaterModifications.HijackLoadGlobals caught {e.Message} error at {e.StackTrace}. This should only run in DEBUG.");
			}
		}

		private static void HideSlopedBlack(ILContext il)
		{
			ILCursor c = new(il);

			if (!c.TryGotoNext(x => x.MatchCall(typeof(TimeLogger).GetMethod(nameof(TimeLogger.DrawTime), BindingFlags.Public | BindingFlags.Static))))
				return;

			if (!c.TryGotoPrev(x => x.MatchLdsfld<Main>(nameof(Main.spriteBatch))))
				return;

			c.Emit(OpCodes.Ldloca_S, (byte)14); //num7 (startX)
			c.Emit(OpCodes.Ldloc_S, (byte)11); //i (y)
			c.Emit(OpCodes.Ldloca_S, (byte)13); //j (endX)
			c.EmitDelegate(ModifyDrawBlack);
		}

		private static void ModifyDrawBlack(ref int startX, int y, ref int endX)
		{
			if (!PlayerInValidOcean || ImproperWaterStyle)
				return;

			Tile tile = Main.tile[startX, y];

			if (tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
				startX++;

			tile = Main.tile[endX - 1, y];

			if (tile.Slope != SlopeType.Solid || tile.IsHalfBlock)
				endX--;
		}

		private static void On_TileDrawing_DrawPartialLiquid(On_TileDrawing.orig_DrawPartialLiquid orig, TileDrawing self, bool behindBlocks, Tile tileCache, ref Vector2 position, ref Rectangle liquidSize, int liquidType, ref VertexColors colors)
		{
			if (!PlayerInValidOcean || tileCache.LiquidType != LiquidID.Water || ImproperWaterStyle)
				orig(self, behindBlocks, tileCache, ref position, ref liquidSize, liquidType, ref colors);
		}

		private static void LiquidRenderer_InternalDraw(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			if (!c.TryGotoNext(MoveType.After, x => x.MatchCall<Main>(nameof(Main.DrawTileInWater))))
				return;

			c.Emit(OpCodes.Ldloc_3); //i
			c.Emit(OpCodes.Ldloc_S, (byte)4); //j
			c.Emit(OpCodes.Ldloc_S, (byte)9); //vertex colours
			c.Emit(OpCodes.Ldarg_S, (byte)5); //isBackgroundDraw

			c.EmitDelegate(DrawPartialWater);
		}

		public static void DrawPartialWater(int i, int j, VertexColors colours, bool isBackgroundDraw)
		{
			if (ImproperWaterStyle || !PlayerInValidOcean)
				return;

			Tile tile = Main.tile[i, j];
			Tile right = Main.tile[i + 1, j];
			Tile left = Main.tile[i - 1, j];
			Vector2 drawOffset = (Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange)) - Main.screenPosition;

			bool openTile = (!tile.HasTile || !Main.tileSolid[tile.TileType]);
			float factor = tile.LiquidAmount / 255f;
			bool vanillaWaterStyle = Main.waterStyle < WaterStyleID.Count;

			int style = 0;

			if (tile.LiquidType != LiquidID.Water)
				style = tile.LiquidType == LiquidID.Lava ? WaterStyleID.Lava : WaterStyleID.Honey;
			else if (vanillaWaterStyle)
				style = Main.waterStyle;

			Asset<Texture2D> texture = TextureAssets.LiquidSlope[style];

			if (openTile)
			{
				DrawSlopedLiquid(right, 1, true, factor, drawOffset, texture, i, j);
				DrawSlopedLiquid(left, -1, false, factor, drawOffset, texture, i, j);

				DrawHalfBrickLiquid(right, 1, drawOffset, colours, i, j);
				DrawHalfBrickLiquid(left, -1, drawOffset, colours, i, j);

				if (!isBackgroundDraw)
					return;

				if (WorldGen.SolidTile(i, j - 1) && tile.LiquidAmount == byte.MaxValue && !Main.tile[i, j - 1].BottomSlope)
				{
					var pos = new Vector2((i) << 4, (j - 1) * 16f + 8) + drawOffset;
					var source = new Rectangle(16, 64, 16, 8);
					var tex = LiquidRenderer.Instance._liquidTextures[style].Value;

					Main.tileBatch.Draw(tex, pos, source, colours, Vector2.Zero, 1f, SpriteEffects.None);
				}

				if (WorldGen.SolidOrSlopedTile(i, j + 1) && !Main.tile[i, j + 1].TopSlope)
				{
					var pos = new Vector2((i) << 4, (j + 1) * 16f) + drawOffset;
					var source = new Rectangle(16, 8, 16, 4);
					var tex = LiquidRenderer.Instance._liquidTextures[style].Value;

					Main.tileBatch.Draw(tex, pos, source, colours, Vector2.Zero, 1f, SpriteEffects.None);
				}

				if (WorldGen.SolidOrSlopedTile(right) && !right.LeftSlope)
				{
					var pos = new Vector2((i << 4) + 16, j * 16f) + drawOffset;
					var source = new Rectangle(16, 64, 2, 16);
					var tex = LiquidRenderer.Instance._liquidTextures[style].Value;

					Main.tileBatch.Draw(tex, pos, source, colours, Vector2.Zero, 1f, SpriteEffects.None);
				}

				if (WorldGen.SolidOrSlopedTile(left) && !left.RightSlope)
				{
					var pos = new Vector2((i << 4) - 2, j * 16f) + drawOffset;
					var source = new Rectangle(16, 64, 2, 16);
					var tex = LiquidRenderer.Instance._liquidTextures[style].Value;

					Main.tileBatch.Draw(tex, pos, source, colours, Vector2.Zero, 1f, SpriteEffects.None);
				}
			}
		}

		static void DrawSlopedLiquid(Tile tile, int offset, bool left, float factor, Vector2 drawOffset, Asset<Texture2D> texture, int i, int j)
		{
			if (tile.HasTile && ((left && tile.LeftSlope) || tile.RightSlope))
			{
				Lighting.GetCornerColors(i, j, out VertexColors colours);
				colours.PerColor((ref Color col) => col = Color.Lerp(col, new Color(210, 210, 210, 255), 0.1f));
				var pos = new Vector2((i + offset) << 4, (j + (1 - factor)) * 16f) + drawOffset;
				var source = new Rectangle((int)(tile.Slope - 1) * 18, (int)(16 * (1 - factor)), 16, (int)(16 * factor));

				Main.tileBatch.Draw(texture.Value, pos, source, colours, Vector2.Zero, 1f, SpriteEffects.None);
			}
		}

		private static void DrawHalfBrickLiquid(Tile tile, int offset, Vector2 drawOffset, VertexColors colours, int i, int j)
		{
			if (tile.HasTile && tile.IsHalfBlock)
			{
				byte targetAmount = Math.Max(Main.tile[i + offset - 1, j].LiquidAmount, Main.tile[i + offset + 1, j].LiquidAmount);

				if (Main.tile[i + offset, j - 1].LiquidAmount > 0)
					return;

				if (targetAmount <= 128)
					return;

				int leftType = Main.tile[i + offset - 1, j].LiquidType;
				int rightType = Main.tile[i + offset + 1, j].LiquidType;
				int useType = Main.waterStyle;

				if (Main.tile[i + offset - 1, j].LiquidAmount == 0)
				{
					if (rightType == LiquidID.Lava)
						useType = WaterStyleID.Lava;
					else if (rightType == LiquidID.Honey)
						useType = WaterStyleID.Honey;
				}
				else if (Main.tile[i + offset + 1, j].LiquidAmount == 0)
					useType = leftType;
				else
				{
					if (leftType == rightType)
					{
						if (leftType == LiquidID.Lava)
							useType = WaterStyleID.Lava;
						else if (leftType == LiquidID.Honey)
							useType = WaterStyleID.Honey;
					}
					else
					{
						if (leftType == LiquidID.Water || rightType == LiquidID.Water)
							useType = Main.waterStyle;
						else if (leftType == LiquidID.Lava || rightType == LiquidID.Lava)
							useType = WaterStyleID.Lava;
						else
							useType = WaterStyleID.Honey;
					}
				}

				float heightFactor = (targetAmount - 128) / 127f;// 126.5f;

				var pos = new Vector2((i + offset) << 4, j * 16f - 4) + drawOffset;
				var source = new Rectangle(16, 0, 16, (int)(8 * heightFactor));
				var tex = LiquidRenderer.Instance._liquidTextures[useType].Value;

				Main.tileBatch.Draw(tex, pos + new Vector2(0, 8 * (heightFactor)), source, colours, Vector2.Zero, 1f, SpriteEffects.None);
			}
		}

		public static void Unload()
		{
			transparencyEffect = null;
			rippleTex = null;
		}

		private static void WaterShaderData_DrawWaves(ILContext il)
		{
			var c = new ILCursor(il);

			if (!c.TryGotoNext(x => x.MatchLdfld<WaterShaderData>("_useRippleWaves")))
			{
				Logger.Debug("FAILED _useRippleWaves GOTO [SpiritMod.WaterShaderData.DrawWaves]");
				return;
			}

			if (!c.TryGotoNext(x => x.MatchCallvirt(typeof(TileBatch).GetMethod("End"))))
			{
				Logger.Debug("FAILED End GOTO [SpiritMod.WaterShaderData.DrawWaves]");
				return;
			}

			c.Emit(OpCodes.Ldloc_2);
			c.EmitDelegate(DoWaves);
		}

		private static void DoWaves(Vector2 offset)
		{
			bool validPlayer = false;
			(bool, bool) sides = (false, false);

			for (int i = 0; i < Main.maxPlayers; ++i)
			{
				Player p = Main.player[i];
				if (p.active && p.ZoneBeach)
				{
					validPlayer = true;
					CheckBeachHeightsSet();

					if (p.position.X / 16f < Main.maxTilesX / 2)
						sides.Item1 = true;
					else
						sides.Item2 = true;

					if (sides.Item1 && sides.Item2)
						break;
				}
			}

			static float Speed() => 0.75f + Main.rand.NextFloat(0, 0.25f); 

			if (validPlayer) //Draw here to draw stuff only when there's a player at a beach
			{
				if (sides.Item1 && Main.GameUpdateCount % 80 == 0)
					OceanWaveManager.AddWave(new OceanWaveManager.Wave(new Vector2(600, leftOceanHeight), new Vector2(10, 30), Main.rand.NextFloat(0.6f, 1f), Speed()));
				if (sides.Item2 && Main.GameUpdateCount % 80 == 0)
					OceanWaveManager.AddWave(new OceanWaveManager.Wave(new Vector2(Main.maxTilesX * 16 - 550, rightOceanHeight), new Vector2(10, 30), Main.rand.NextFloat(0.6f, 1f), Speed()));
			}

			OceanWaveManager.UpdateWaves(sides.Item1, sides.Item2, offset);

			//PlayerRipples(offset);
		}

		private static void PlayerRipples(Vector2 offset)
		{
			if (Main.LocalPlayer.wet && Main.GameUpdateCount % 12 == 0) //Draw player ripples
			{
				Player player = Main.LocalPlayer;
				float speed = player.velocity.Length();

				if (speed > 0.2f)
				{
					player.GetSpiritPlayer().Submerged(6, out int actualDepth);
					actualDepth++;

					float g = speed * 0.25f + 0.5f;
					float mult = Math.Min(Math.Abs(speed) * 0.65f, 0.25f);
					Color c = new Color(0.5f, g, 0f, 1f) * mult;

					Vector2 drawPos = Main.LocalPlayer.Center - offset;
					Rectangle src = new Rectangle(1, 1, 40, 40);
					Main.tileBatch.Draw(rippleTex, new Vector4(drawPos.X, drawPos.Y, player.width, player.height) * 0.25f, src, new VertexColors(c * (actualDepth / 6f)), src.Size() / 2f, SpriteEffects.None, 0f);
				}
			}
		}

		private static void CheckBeachHeightsSet()
		{
			int maxYAllowed = Main.maxTilesY - 200;

			if (leftOceanHeight == 0 || leftOceanHeight / 16f > Main.worldSurface)
			{
				var start = new Point(60, (int)(Main.maxTilesY * 0.35f / 16f));
				while (Framing.GetTileSafely(start.X, start.Y).LiquidAmount < 255)
				{
					start.Y++;
					if (start.Y > maxYAllowed) 
						break;
				}

				leftOceanHeight = start.Y * 16 - (int)(18 * (Framing.GetTileSafely(start.X, start.Y).LiquidAmount / 255f));
			}

			if (rightOceanHeight == 0 || rightOceanHeight / 16f > Main.worldSurface)
			{
				var start = new Point(Main.maxTilesX - 60, (int)(Main.maxTilesY * 0.35f / 16f));
				while (Framing.GetTileSafely(start.X, start.Y).LiquidAmount < 255)
				{
					start.Y++;
					if (start.Y > maxYAllowed) 
						break;
				}

				rightOceanHeight = start.Y * 16 - (int)(18 * (Framing.GetTileSafely(start.X, start.Y).LiquidAmount / 255f));
			}
		}

		private static void IncreaseRippleSize(ILContext il)
		{
			var c = new ILCursor(il);

			c.Emit(OpCodes.Ldarg_3);
			c.Emit(OpCodes.Ldc_R4, 1.5f);
			var vec2Mul = typeof(Vector2).GetMethod("op_Multiply", new Type[2] { typeof(Vector2), typeof(float) }, new ParameterModifier[] { new ParameterModifier(3) });
			c.Emit(OpCodes.Call, vec2Mul);
			c.Emit(OpCodes.Starg, 3);
		}

		/// <summary>MASSIVE thanks to Starlight River for the base of this IL edit.</summary>
		private static void AddWaterShader(ILContext il)
		{
			var c = new ILCursor(il);

			//BACK TARGET
			c.TryGotoNext(n => n.MatchLdfld<Main>("backWaterTarget"));

			c.TryGotoNext(n => n.MatchCallvirt<SpriteBatch>("Draw"));
			c.Index++;
			ILLabel postDrawLabel = c.MarkLabel();

			c.TryGotoPrev(MoveType.Before, n => n.MatchLdfld<Main>("backWaterTarget"));
			c.TryGotoNext(MoveType.Before, n => n.MatchLdsfld<Main>("sceneBackgroundPos"));

			ILLabel exitLabel = c.MarkLabel();

			c.TryGotoPrev(MoveType.Before, n => n.MatchLdfld<Main>("backWaterTarget"));
			c.Index++;

			c.EmitDelegate(IsWaterTransparent);
			c.Emit(OpCodes.Brfalse, exitLabel);
			c.Emit(OpCodes.Nop);

			c.Index--;
			c.Emit(OpCodes.Pop);
			c.Emit(OpCodes.Pop);
			c.Emit(OpCodes.Ldc_I4_0); //Push 0 because this is the back
			int v = c.EmitDelegate(NewDraw);
			c.Emit(OpCodes.Br, postDrawLabel);

			//FRONT TARGET
			c.TryGotoNext(n => n.MatchLdsfld<Main>("waterTarget"));

			c.TryGotoNext(n => n.MatchCallvirt<SpriteBatch>("Draw"));
			c.Index++;
			ILLabel label2 = c.MarkLabel();

			c.TryGotoPrev(n => n.MatchLdsfld<Main>("waterTarget"));
			c.TryGotoNext(MoveType.Before, n => n.MatchLdsfld<Main>("sceneWaterPos"));

			ILLabel exitLabelFront = c.MarkLabel();

			c.TryGotoPrev(n => n.MatchLdsfld<Main>("waterTarget"));
			c.Index++;

			c.EmitDelegate(IsWaterTransparent);
			c.Emit(OpCodes.Brfalse, exitLabelFront);
			c.Emit(OpCodes.Nop);

			c.Index--;
			c.Emit(OpCodes.Pop);
			c.Emit(OpCodes.Pop);
			c.Emit(OpCodes.Ldc_I4_1); //Push 1 since this is the front
			c.EmitDelegate(NewDraw);
			c.Emit(OpCodes.Br, label2);
		}

		private static bool IsWaterTransparent() => (Lighting.Mode == LightMode.Color || Lighting.Mode == LightMode.White) && PlayerInValidOcean && !Main.bloodMoon && !Main.LocalPlayer.ZoneSkyHeight;

		private static void NewDraw(bool back)
		{
			Main.spriteBatch.End();

			SetShader(back);

			if (back)
			{
				Main.spriteBatch.Draw(Main.instance.backWaterTarget, Main.sceneBackgroundPos - Main.screenPosition, Color.White);
				Terraria.Graphics.Effects.Overlays.Scene.Draw(Main.spriteBatch, Terraria.Graphics.Effects.RenderLayers.BackgroundWater);
			}
			else
			{
				Main.spriteBatch.Draw(Main.waterTarget, Main.sceneWaterPos - Main.screenPosition, Color.White * Main.liquidAlpha[Main.waterStyle]);
				Terraria.Graphics.Effects.Overlays.Scene.Draw(Main.spriteBatch, Terraria.Graphics.Effects.RenderLayers.ForegroundWater);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}

		private static void SetShader(bool back)
		{
			transparencyEffect.Parameters["transparency"].SetValue(GetTransparency());

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, transparencyEffect, Main.GameViewMatrix.TransformationMatrix);
		}

		private static float GetTransparency()
		{
			bool aboveGround = Main.LocalPlayer.Center.Y / 16f < Main.worldSurface;
			if (aboveGround && Main.LocalPlayer.ZoneBeach && ModContent.GetInstance<SpiritClientConfig>().SurfaceWaterTransparency)
				return 1.25f;

			return aboveGround ? 1f : 0.5f;
		}
	}
}
