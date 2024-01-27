using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Glyph;
using SpiritMod.GlobalClasses.Items;
using SpiritMod.GlobalClasses.Players;
using SpiritMod.Items.Glyphs;
using SpiritMod.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Projectiles
{
	public class GlyphGlobalProjectile : GlobalProjectile
	{
		public int rarity;

        public GlyphType Glyph { get; set; }

        public override bool InstancePerEntity => true;

		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			void Share(Entity entity)
			{
				if (entity is Item item)
				{
					Glyph = item.GetGlobalItem<GlyphGlobalItem>().Glyph;
					rarity = item.OriginalRarity;
				}
				else if (entity is Projectile parent)
				{
					Glyph = parent.GetGlobalProjectile<GlyphGlobalProjectile>().Glyph;
					rarity = parent.GetGlobalProjectile<GlyphGlobalProjectile>().rarity;
				}
				rarity = Math.Max(rarity, 1);
			}

			//Whether the projectile was spawned from item use, or another projectile
			if (source is EntitySource_ItemUse item)
				Share(item.Item);
			else if (Main.ProjectileUpdateLoopIndex >= 0 && Main.projectile[Main.ProjectileUpdateLoopIndex] is Projectile newProj && newProj.active && newProj.owner == Main.myPlayer)
				Share(newProj);
			else
				Glyph = GlyphType.None;

			if (Main.netMode != NetmodeID.SinglePlayer && Glyph != GlyphType.None)
			{
				ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.ProjGlyph, 3);
				packet.Write(projectile.whoAmI);
				packet.Write((byte)Glyph);
				packet.Write(rarity);
				packet.Send();
			}
		}

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[projectile.owner];
			GlyphPlayer mPlayer = player.GetModPlayer<GlyphPlayer>();

			int useTime = (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]) 
				? 25 : projectile.GetGlobalProjectile<SpiritGlobalProjectile>().storedUseTime;
			//Item useTime isn't accurate for minions

			if (Glyph == GlyphType.Frost && Main.rand.NextBool((int)MathHelper.Clamp(30 - (useTime / 2f), 2, 12)))
				FrostGlyph.FreezeEffect(player, target, projectile);
			if (Glyph == GlyphType.Void && Main.rand.NextBool((int)MathHelper.Clamp(30 - (useTime / 2f), 2, 12)))
				VoidGlyph.VoidCollapse(player, target, projectile, damageDone / 2 * rarity);
			if (Glyph == GlyphType.Radiant)
			{
				mPlayer.genericCounter = 0;

				if (player.HasBuff(ModContent.BuffType<DivineStrike>()))
				{
					RadiantGlyph.RadiantStrike(player, target);
					player.ClearBuff(ModContent.BuffType<DivineStrike>());
				}
			}

			if (!target.CanDropLoot()) //Don't let useless NPCs trigger widely beneficial effects
				return;

			if (Glyph == GlyphType.Unholy && mPlayer.unholyCooldown <= 0 && target.life <= 0)
			{
				UnholyGlyph.Erupt(player, target, 10 * rarity);
				mPlayer.unholyCooldown = 2;
			}
			if (Glyph == GlyphType.Sanguine)
				SanguineGlyph.DrainEffect(player, target);
			if (Glyph == GlyphType.Blaze)
				player.AddBuff(ModContent.BuffType<BurningRage>(), 120);
			if (Glyph == GlyphType.Bee)
			{
				if ((mPlayer.genericCounter = MathHelper.Clamp(mPlayer.genericCounter + (useTime / 60f), 0, 1)) == 1)
				{
					BeeGlyph.ReleaseBees(player, target, (int)(damageDone * .4f));
					mPlayer.genericCounter = 0;
				}
				if (target.life <= 0)
					BeeGlyph.HoneyEffect(player);
			}
			if (Glyph == GlyphType.Phase)
			{
				if ((mPlayer.genericCounter = MathHelper.Clamp(mPlayer.genericCounter + (useTime / 60f), 0, 1)) == 1)
					player.AddBuff(ModContent.BuffType<TemporalShift>(), (int)MathHelper.Clamp(useTime * 2f, 30, 60));
			}
			if (Glyph == GlyphType.Rage)
			{
				bool notCascading = mPlayer.frenzyDamage == 0;
				mPlayer.frenzyDamage = 0;

				if (target.life <= 0 && notCascading)
					mPlayer.frenzyDamage = Math.Abs(target.life);
			}
			if (Glyph == GlyphType.Veil)
				mPlayer.veilCounter = MathHelper.Clamp(mPlayer.veilCounter + (useTime / 300f), 0, 1);
		}

		public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
		{
			Player player = Main.player[projectile.owner];
			GlyphPlayer mPlayer = player.GetModPlayer<GlyphPlayer>();

			if (Glyph == GlyphType.Phase)
			{
				float boost = MathHelper.Clamp(.006f * player.GetModPlayer<MyPlayer>().SpeedMPH, 0, .7f);
				modifiers.FinalDamage *= .8f + boost;
			}
			if (Glyph == GlyphType.Rage && mPlayer.frenzyDamage > 0)
			{
				modifiers.FinalDamage.Base += mPlayer.frenzyDamage = Math.Min(mPlayer.frenzyDamage, 1000);
				RageGlyph.RageEffect(player, target, projectile);
			}
			if (Glyph == GlyphType.Radiant && player.HasBuff(ModContent.BuffType<DivineStrike>()))
				modifiers.FinalDamage *= 2.5f;
		}
	}
}
