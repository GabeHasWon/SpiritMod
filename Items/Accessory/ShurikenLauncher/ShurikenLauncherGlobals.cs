using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;

namespace SpiritMod.Items.Accessory.ShurikenLauncher
{
	public class ShurikenLauncherPlayer : ModPlayer
	{
		public bool throwerGlove = false;
		private float throwerStacks;

		public override void ResetEffects()
		{
			if (!throwerGlove)
				throwerStacks = 0;
			else
				throwerStacks = Math.Max(throwerStacks - .0005f, 0); //Decay over time

			throwerGlove = false;
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Projectile, consider using ModifyHitNPC instead */
		{
			if (throwerGlove && proj.IsRanged())
			{
				Player player = Main.player[proj.owner];
				float damageBonusMult = 1f;

				if (player.Distance(target.Center) >= ShurikenLauncher.EffectiveDistance)
				{
					if (proj.type != ProjectileID.ChlorophyteBullet)
					{
						const float throwerStacksMax = .25f; //+25% bonus damage for sustaining consecutive hits
						throwerStacks = Math.Min(throwerStacks + MathHelper.Clamp(modifiers.SourceDamage.Base * .0005f, 0, .05f), throwerStacksMax);
					}
					damageBonusMult += .2f + throwerStacks; //Allows 145% total bonus damage

					for (int i = 0; i < 3; i++)
					{
						Dust dust = Dust.NewDustPerfect(proj.Center, DustID.Electric, Scale: .5f);
						dust.noGravity = true;
						dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat( .5f, 2f);
					}
					ParticleHandler.SpawnParticle(new HyperlightParticle(target.Center, Main.rand.NextFloat(1.57f), Main.rand.NextFloat(.75f, 1f), target));
				}

				modifiers.FinalDamage *= damageBonusMult;
				target.GetGlobalNPC<ShurikenLauncherNPC>().hitDelay = 1f;
			}
		}
	}

	public class ShurikenLauncherNPC : GlobalNPC
	{
		public float hitDelay;
		private float opacity;

		public override bool InstancePerEntity => true;

		public override bool PreAI(NPC npc)
		{
			if (!Main.dedServ)
			{
				Vector2 zoom = Main.GameViewMatrix.Zoom;

				Rectangle hoverBox = npc.getRect();
				hoverBox.Inflate((int)zoom.X, (int)zoom.Y);

				opacity = 1f - MathHelper.Clamp(Main.MouseWorld.Distance(hoverBox.ClosestPointInRect(Main.MouseWorld)) / 100f, 0, 1);
			}
			hitDelay = Math.Max(hitDelay - .1f, 0);

			return base.PreAI(npc);
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Player player = Main.LocalPlayer;
			if (!player.GetModPlayer<ShurikenLauncherPlayer>().throwerGlove || !player.HeldItem.IsRanged() || !npc.CanDamage() || (player.Distance(npc.Center) < ShurikenLauncher.EffectiveDistance))
				return;

			Texture2D texture = Mod.Assets.Request<Texture2D>("Items/Accessory/ShurikenLauncher/ShurikenLauncher_Reticle").Value;

			float lerp = (float)Math.Sin(Main.timeForVisualEffects / 40f);
			float scale = 1 + (lerp * .2f);

			for (int i = 0; i < 5; i++)
			{
				int frame = i / 4;
				Rectangle drawFrame = texture.Frame(1, 2, 0, frame);
				float distance = (lerp * 2f) + (hitDelay * 8f);
				if (i == 4)
					distance = 0;

				float rotation = ((float)(Main.timeForVisualEffects / 40f % MathHelper.Pi) + (MathHelper.PiOver2 * i)) * (1f - frame);
				Vector2 pos = npc.Center + (new Vector2(-1) * distance).RotatedBy(rotation);

				spriteBatch.Draw(texture, pos - screenPos, drawFrame, Color.White * Math.Max(opacity, hitDelay * 2), rotation, drawFrame.Size() / 2, scale, SpriteEffects.None, 0);
			}
		}
	}
}
