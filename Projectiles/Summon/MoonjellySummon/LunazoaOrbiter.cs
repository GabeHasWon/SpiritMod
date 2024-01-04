using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using SpiritMod.NPCs.Boss.MoonWizard.Projectiles;
using System.Linq;

namespace SpiritMod.Projectiles.Summon.MoonjellySummon
{
	public class LunazoaOrbiter : ModProjectile
	{
		public int ParentIndex { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.width = 16;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.hostile = false;
			Projectile.minion = true;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 300;
		}

		public override void AI()
		{
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
			Lighting.AddLight(Projectile.Center, .075f * 2, .231f * 2, .255f * 2);

			Projectile.spriteDirection = -Projectile.direction;
			if (++Projectile.frameCounter >= 10)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
			}

			if (Main.projectile[ParentIndex] is Projectile parent && parent.active && parent.type == ModContent.ProjectileType<MoonjellySummon>())
			{
				if (parent.oldPos[1] != Vector2.Zero)
					Projectile.position = Projectile.position + parent.position - parent.oldPos[1];

				Vector2 dirToParent = parent.Center - Projectile.Center;

				Projectile.velocity += Vector2.Normalize(dirToParent) * .2f;
				if (Projectile.velocity.Length() > 4f)
					Projectile.velocity *= 4f / Projectile.velocity.Length();
			}
			else if (Projectile.owner == Main.myPlayer)
			{
				Projectile.Kill();
				Projectile.netUpdate = true;
			}
		}

		public override void OnKill(int timeLeft)
		{
			var target = Projectile.OwnerMinionAttackTargetNPC ?? Main.npc.Where(x => x.CanBeChasedBy(Projectile) && (x.Distance(Projectile.Center) / 16) < 30).OrderBy(x => x.Distance(Projectile.Center)).FirstOrDefault();
			if (target != default)
			{
				SoundEngine.PlaySound(SoundID.Item110, Projectile.position);
				Vector2 direction = Projectile.DirectionTo(target.Center) * 15f;

				Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, direction, ModContent.ProjectileType<JellyfishOrbiter_Friendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				p.scale = Projectile.scale;
				p.netUpdate = true;
			}
			for (int k = 0; k < 10; k++)
			{
				Dust d = Dust.NewDustPerfect(Projectile.Center, 226, Vector2.One.RotatedByRandom(3.28f) * Main.rand.NextFloat(5), 0, default, Main.rand.NextFloat(.4f, .8f));
				d.noGravity = true;
			}
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindProjectiles.Add(index);

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, (Projectile.height / Main.projFrames[Projectile.type]) * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * (float)(((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
				Color color1 = Color.White * (float)(((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
				Texture2D glow = Mod.Assets.Request<Texture2D>("Projectiles/Summon/MoonjellySummon/LunazoaOrbiter_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

				Main.spriteBatch.Draw(glow, drawPos, new Microsoft.Xna.Framework.Rectangle?(TextureAssets.Projectile[Projectile.type].Value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color1, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
				Main.spriteBatch.Draw(glow, drawPos, new Microsoft.Xna.Framework.Rectangle?(TextureAssets.Projectile[Projectile.type].Value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color1, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);

				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(TextureAssets.Projectile[Projectile.type].Value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(TextureAssets.Projectile[Projectile.type].Value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame)), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
			}
			return false;
		}
	}
}