using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Town
{
	public class Ghost : ModNPC
	{
		public int npcTypeToCopy = 0;

		private int FramesMax => Main.npcFrameCount[npcTypeToCopy];

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ghost");
			NPCID.Sets.ActsLikeTownNPC[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.friendly = true;
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.lifeMax = 250;
			NPC.dontTakeDamage = true;
			NPC.width = 20;
			NPC.height = 54;
			NPC.alpha = 255;
		}

		public override bool CheckActive() => false;

		public override void PostAI()
		{
			const int fadeoutTime = 180;

			if (Main.dayTime)
			{
				if ((NPC.alpha += 255 / fadeoutTime) >= 255)
					NPC.active = false;
			}
			else
			{
				Player player = Main.LocalPlayer;
				int desiredAlpha = 255 - (int)MathHelper.Min(player.Distance(NPC.Center) * 1.5f, 255);
				NPC.alpha = (int)MathHelper.Lerp(NPC.alpha, desiredAlpha, .05f);
			}

			int dir = (int)Math.Sin(NPC.velocity.X);
			NPC.direction = NPC.spriteDirection = (dir == 0) ? NPC.oldDirection : dir;
		}

		//Drawing assumes that the copied NPC uses the default town NPC sprite format, and will otherwise create artifacts
		public override void FindFrame(int frameHeight)
		{
			const int walkFrames = 15;
			if (NPC.velocity.X != 0)
				NPC.frameCounter = ((int)NPC.frameCounter == 0) ? 1 : (NPC.frameCounter + .2f) % walkFrames;
			else
				NPC.frameCounter = 0;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			SpiritMod.GhostShader.Parameters["progress"].SetValue(FramesMax);
			SpiritMod.GhostShader.CurrentTechnique.Passes[0].Apply();

			Texture2D texture = TextureAssets.Npc[npcTypeToCopy].Value;
			SpriteEffects effects = (NPC.spriteDirection == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Rectangle frame = texture.Frame(1, FramesMax, 0, (int)NPC.frameCounter, 0, -2);

			spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), frame, NPC.GetAlpha(drawColor), NPC.rotation, frame.Size() / 2, NPC.scale, effects, 0);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(npcTypeToCopy);

		public override void ReceiveExtraAI(BinaryReader reader) => npcTypeToCopy = reader.ReadInt32();
	}

	internal static class GhostHandler
	{
		/// <summary>
		/// Contains all NPCs who should become ghosts during the night in valid biomes, as well as their home positions.
		/// </summary>
		public static IDictionary<int, Point> toBeGhosts = new Dictionary<int, Point>();

		/// <summary>
		/// Spawns town NPC ghosts; should not be called on multiplayer clients.
		/// </summary>
		/// <param name="player"></param>
		public static void TrySpawnGhosts(Player player)
		{
			if (!toBeGhosts.Any())
				return;

			var ghostNPCs = toBeGhosts.Where(x => x.Value.ToWorldCoordinates().Distance(player.Center) < 800);
			if (ghostNPCs.Any())
			{
				var ghostNPC = ghostNPCs.First();
				NPC npc = NPC.NewNPCDirect(Entity.GetSource_NaturalSpawn(), ghostNPC.Value.X * 16, ghostNPC.Value.Y * 16, ModContent.NPCType<Ghost>());
				if (npc.ModNPC is Ghost ghost)
					ghost.npcTypeToCopy = ghostNPC.Key;

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);

				toBeGhosts.Remove(ghostNPC.Key);
			}
		}
	}
}