using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SpiritMod.Mechanics.OceanWavesSystem;
using SpiritMod.Utilities;
using SpiritMod.Utilities.Helpers;
using System;
using System.Diagnostics;
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

namespace SpiritMod.Effects.SurfaceWaterModifications
{
	public class SurfaceWaterModifications
	{
		internal static Effect transparencyEffect = null;
		internal static Texture2D rippleTex = null;
		internal static int leftOceanHeight = 0;
		internal static int rightOceanHeight = 0;

		private static FieldInfo animationFrameField;

		public static ILog Logger => ModContent.GetInstance<SpiritMod>().Logger;

		public static void Load()
		{
			animationFrameField = typeof(LiquidRenderer).GetField("_animationFrame", BindingFlags.Instance | BindingFlags.NonPublic);

			if (ModContent.GetInstance<SpiritClientConfig>().SurfaceWaterTransparency)
			{
				IL.Terraria.Main.DoDraw += AddWaterShader; //Transparency shader
				On.Terraria.GameContent.Drawing.TileDrawing.DrawPartialLiquid += RenderRealLiquidInPlaceOfPartial;
			}

			IL.Terraria.GameContent.Shaders.WaterShaderData.QueueRipple_Vector2_Color_Vector2_RippleShape_float += IncreaseRippleSize; //Makes ripple bigger
			IL.Terraria.GameContent.Shaders.WaterShaderData.DrawWaves += WaterShaderData_DrawWaves;

			if (!Main.dedServ)
			{
				transparencyEffect = ModContent.Request<Effect>("SpiritMod/Effects/SurfaceWaterModifications/SurfaceWaterFX", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				rippleTex = ModContent.Request<Texture2D>("Terraria/Images/Misc/Ripples", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			}
		}

		public static void Unload()
		{
			transparencyEffect = null;
			rippleTex = null;
		}

		private static void RenderRealLiquidInPlaceOfPartial(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawPartialLiquid orig, TileDrawing self, Tile tileCache, Vector2 position, Rectangle liquidSize, int liquidType, Color aColor)
		{
			if (TileID.Sets.BlocksWaterDrawingBehindSelf[(int)tileCache.BlockType] || tileCache.Slope == SlopeType.Solid)
			{
				orig(self, tileCache, position, liquidSize, liquidType, aColor);
				return;
			}

			var drawPos = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			var drawOffset = drawPos - Main.screenPosition;
			var tileCoords = (position + Main.screenPosition - drawPos).ToTileCoordinates();

			if (Main.LocalPlayer.ZoneBeach)
			{
				transparencyEffect.Parameters["transparency"].SetValue(GetTransparency());
				Main.tileBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, transparencyEffect);
			}
			else
				Main.tileBatch.Begin();

			Main.DrawTileInWater(drawOffset, tileCoords.X, tileCoords.Y);

			var tile = Framing.GetTileSafely(tileCoords);

			if (tile.LiquidType != LiquidID.Water)
				return;

			int liquidStyle = Main.waterStyle;
			float opacity = 0.8f * 0.8f;

			opacity = Math.Min(opacity, 1f);
			if (tile.WallType != 0) 
				opacity = 0.5f;

			Lighting.GetCornerColors(tileCoords.X, tileCoords.Y, out var vertices);
			vertices.BottomLeftColor *= opacity;
			vertices.BottomRightColor *= opacity;
			vertices.TopLeftColor *= opacity;
			vertices.TopRightColor *= opacity;

			Main.DrawTileInWater(drawOffset, tileCoords.X, tileCoords.Y);

			int frameYOffset = tile.LiquidAmount == 0 && !tile.HasTile ? 0 : 48;
			var srcRect = new Rectangle(16, frameYOffset + (int)(animationFrameField!.GetValue(LiquidRenderer.Instance) ?? 0) * 80, 16, 16);

			Main.tileBatch.Draw(TextureAssets.Liquid[liquidStyle].Value, position + new Vector2(-2f, tile.IsHalfBlock ? 8f : 0f), srcRect, vertices, Vector2.Zero, 1f, SpriteEffects.None);
			Main.tileBatch.End();
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
				if (sides.Item1 && Main.GameUpdateCount % 50 == 0)
					OceanWaveManager.AddWave(new OceanWaveManager.Wave(new Vector2(600, leftOceanHeight), new Vector2(10, 30), Main.rand.NextFloat(0.6f, 1f), Speed()));
				if (sides.Item2 && Main.GameUpdateCount % 50 == 0)
					OceanWaveManager.AddWave(new OceanWaveManager.Wave(new Vector2(Main.maxTilesX * 16 - 550, rightOceanHeight), new Vector2(10, 30), Main.rand.NextFloat(0.6f, 1f), Speed()));
			}

			OceanWaveManager.UpdateWaves(sides.Item1, sides.Item2, offset);
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
			c.Emit(OpCodes.Ldc_R4, 2f);
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

		private static bool IsWaterTransparent() => (Lighting.Mode == LightMode.Color || Lighting.Mode == LightMode.White) && Main.LocalPlayer.ZoneBeach && !Main.bloodMoon && !Main.LocalPlayer.ZoneSkyHeight;

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
			Main.spriteBatch.Begin(default, default, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);
		}

		// <summary>Just a little test I did. Don't mind this. :)</summary>
		//private static void DrawReflectedPlayer()
		//{
		//	Player plr = Main.LocalPlayer;
		//	plr.direction = plr.direction == -1 ? 1 : -1;
		//
		//	Main.instance.DrawPlayer(plr, plr.position, MathHelper.Pi, new Vector2(plr.width / 2f, plr.height), 0);
		//
		//	plr.direction = plr.direction == -1 ? 1 : -1;
		//}

		private static void SetShader(bool back)
		{
			transparencyEffect.Parameters["transparency"].SetValue(GetTransparency());

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, transparencyEffect, Main.GameViewMatrix.ZoomMatrix);
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
