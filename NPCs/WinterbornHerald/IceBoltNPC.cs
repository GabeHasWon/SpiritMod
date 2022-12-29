using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.WinterbornHerald
{
	public class IceBoltNPC : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ice Bolt");
			NPCID.Sets.TrailCacheLength[NPC.type] = 5;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
		}

		public override void SetDefaults()
		{
			NPC.friendly = false;
			NPC.width = 14;
			NPC.height = 14;
			NPC.lifeMax = 1;
			NPC.value = 0f;
			NPC.scale = 1f;
			NPC.dontCountMe = true;
			NPC.npcSlots = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.timeLeft = 500;
			NPC.aiStyle = -1;
		}

		public override void AI()
		{
			NPC.rotation = NPC.velocity.ToRotation();
			NPC.spriteDirection = NPC.direction;

			if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
					ImpactFX();
				NPC.active = false;
			}

			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.BlueCrystalShard, 0f, 0f, 200, new Color(), 0.7f);
				dust.fadeIn = 1f;
				dust.velocity = NPC.velocity / 2.5f;
				dust.noGravity = true;
			}
		}

		public override void HitEffect(int hitDirection, double damage) => ImpactFX();

		private void ImpactFX()
		{
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, Main.rand.NextBool(2) ? DustID.IceTorch : DustID.BlueCrystalShard, 0f, 0f, 200, new Color(), 0.5f);
				dust.fadeIn = 1.3f;
				dust.velocity = new Vector2(Main.rand.NextFloat(0.5f, 2.4f)).RotatedByRandom(MathHelper.TwoPi);
				dust.noGravity = true;
			}
			SoundEngine.PlaySound(SoundID.Item27 with { PitchVariance = 0.2f }, NPC.position);
			NPC.netUpdate = true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D texture = TextureAssets.Npc[NPC.type].Value;
			drawColor = Color.White;

			for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
			{
				float opacityMod = (NPCID.Sets.TrailCacheLength[NPC.type] - i) / (float)NPCID.Sets.TrailCacheLength[Type];
				Vector2 drawPosition = NPC.oldPos[i] + (NPC.Size / 2) - Main.screenPosition;
				Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, NPC.GetAlpha(drawColor) * opacityMod,
					NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			}

			Texture2D glowTex = ModContent.Request<Texture2D>("SpiritMod/Effects/Masks/CircleGradient").Value;
			spriteBatch.Draw(glowTex, NPC.Center - screenPos, null, new Color(50, 130, 220, 0) * .7f, 0f, glowTex.Size() / 2, NPC.scale / 4, SpriteEffects.None, 0);

			Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(drawColor),
				NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(drawColor * .3f),
				NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.15f, SpriteEffects.None, 0);
			return false;
		}
	}
}