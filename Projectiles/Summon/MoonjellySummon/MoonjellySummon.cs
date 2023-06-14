using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Summon;
using SpiritMod.Projectiles.BaseProj;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon.MoonjellySummon
{
	[AutoloadMinionBuff("Moonlight Preserver", "This moonlight preserver summons tiny Lunazoa to fight for you!")]
	public class MoonjellySummon : BaseMinion
    {
		private readonly float baseScale = .8f;

		public MoonjellySummon() : base(920, 1500, new Vector2(40, 40)) { }

		public override void AbstractSetStaticDefaults()
		{
			DisplayName.SetDefault("Moonlight Preserver");
			Main.projFrames[Projectile.type] = 10;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

		public override void AbstractSetDefaults() => Projectile.scale = baseScale;

		private ref float AiTimer => ref Projectile.ai[0];

		public override void PostAI()
		{
			int summonTime = (int)MathHelper.Clamp(34 / (.33f * Projectile.minionSlots), 0, 110);

			if ((AiTimer = ++AiTimer % summonTime) == 0)
			{
				Vector2 vel = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * new Vector2(5f, 3f);

				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.Next(-50, 50)), vel, ModContent.ProjectileType<LunazoaOrbiter>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0.0f, (float)Projectile.whoAmI);
				proj.scale = Main.rand.NextFloat(.4f, 1f);
				proj.timeLeft = (int)(62 / (.33f * Projectile.minionSlots));
			}

			Projectile.scale = MathHelper.Min(1.3f, baseScale + ((Projectile.minionSlots - 1) * .062f));
			Projectile.rotation = Projectile.velocity.X * 0.025f;
		}

		public override void IdleMovement(Player player)
		{
			Vector2 desiredPos = Player.Center + new Vector2(-30 * Player.direction, -50);
			if (Projectile.Distance(desiredPos) > 12)
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(desiredPos) * 10f, MathHelper.Min(100, Projectile.Distance(Player.Center)) / 3000);
		}

		public override void TargettingBehavior(Player player, NPC target)
		{
			Vector2 desiredVel = ((Projectile.Distance(player.Center) < 100) ? Projectile.DirectionTo(target.Center) : Projectile.DirectionTo(player.Center)) * 2f;
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVel, 0.1f);
		}

		public override bool DoAutoFrameUpdate(ref int framespersecond, ref int startframe, ref int endframe)
		{
			framespersecond = 8;
			return true;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Color color = new Color(44, 168, 67) * 0.75f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

				float scale = Projectile.scale;
				Texture2D tex = Mod.Assets.Request<Texture2D>("Projectiles/Summon/Zones/StaminaZone", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

				spriteBatch.Draw(tex, Projectile.oldPos[k] + Projectile.Size / 2 - screenPos, null, color, Projectile.rotation, tex.Size() / 2, scale, default, default);
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle drawFrame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

			Vector2 drawOrigin = drawFrame.Size() / 2;
            
			for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);

				float alphaMod = (float)(((float)Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);

				Main.EntitySpriteDraw(texture, drawPos, drawFrame, Projectile.GetAlpha(lightColor) * alphaMod, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
				Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, drawPos, drawFrame, Projectile.GetAlpha(Color.White) * alphaMod, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

				float sineAdd = (float)Math.Sin(Main.timeForVisualEffects / 80) + 2;

				Texture2D ripple = Mod.Assets.Request<Texture2D>("Effects/Masks/Extra_49").Value;
                Main.EntitySpriteDraw(ripple, Projectile.Center - (Vector2.UnitY * 10) - Main.screenPosition, null, new Color((int)(7.5f * sineAdd), (int)(16.5f * sineAdd), (int)(18f * sineAdd), 0), Projectile.rotation, ripple.Size() / 2, Projectile.scale * .7f, SpriteEffects.None, 0);
			}

			Lighting.AddLight(new Vector2(Projectile.Center.X, Projectile.Center.Y), 0.075f * 2, 0.231f * 2, 0.255f * 2);

			return false;
		}
	}
}
