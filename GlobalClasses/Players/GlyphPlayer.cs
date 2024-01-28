using Microsoft.Xna.Framework;
using SpiritMod.Buffs.Glyph;
using SpiritMod.GlobalClasses.Items;
using SpiritMod.Items.Glyphs;
using SpiritMod.Particles;
using SpiritMod.Projectiles.Glyph;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Players
{
	public class GlyphPlayer : ModPlayer
	{
		public GlyphType Glyph { get; set; }

		public int frenzyDamage;
		public float genericCounter;
		public int unholyCooldown;
		public float veilCounter;
		public bool zephyrStrike;

		public float ChaosCounter => Player.miscCounterNormalized;

		public override void ResetEffects() => zephyrStrike = false;

		public override void PreUpdate()
		{
			if (Player.whoAmI == Main.myPlayer)
			{
				var temp = Glyph; //Store the previous tick glyph type
				if (!Player.HeldItem.IsAir)
				{
					if (ChaosCounter == 0)
					{
						var chaosItems = Player.inventory.Where(x => x != null && x.type != ItemID.None && x.GetGlobalItem<GlyphGlobalItem>().randomGlyph);
						foreach (Item chaosItem in chaosItems)
						{
							chaosItem.GetGlobalItem<GlyphGlobalItem>().SetGlyph(chaosItem, ChaosGlyph.Randomize(Glyph));
							chaosItem.GetGlobalItem<GlyphGlobalItem>().randomGlyph = true;
						}
					} //Chaos glyph effect

					Glyph = Player.HeldItem.GetGlobalItem<GlyphGlobalItem>().Glyph;
					if (Glyph == GlyphType.None && Player.nonTorch >= 0 && Player.nonTorch != Player.selectedItem && !Player.inventory[Player.nonTorch].IsAir)
						Glyph = Player.inventory[Player.nonTorch].GetGlobalItem<GlyphGlobalItem>().Glyph;
				}
				else Glyph = GlyphType.None;

				if (temp != Glyph)
				{
					if (Main.netMode == NetmodeID.MultiplayerClient) //If the glyph type has changed, sync
					{
						ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.PlayerGlyph, 2);
						packet.Write((byte)Main.myPlayer);
						packet.Write((byte)Glyph);
						packet.Send();
					}
					genericCounter = 0;
				}
			}

			if (Glyph == GlyphType.Blaze && Player.velocity.Length() > 1.5f && Main.rand.NextBool(2))
				Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Torch, 0, 0, 0, default, Main.rand.NextFloat(1f, 2f)).noGravity = true;
			if (Glyph == GlyphType.Phase)
				genericCounter = MathHelper.Max(genericCounter - .01f, 0);
			if (veilCounter > 0)
			{
				int shieldType = ModContent.ProjectileType<PhaseShield>();
				if (Player.ownedProjectileCounts[shieldType] < 1) //Spawn a shield visual
					Projectile.NewProjectile(null, Player.Center, Vector2.Zero, shieldType, 0, 0, Player.whoAmI);
			}
			if (Glyph == GlyphType.Radiant)
			{
				if ((genericCounter = MathHelper.Min(genericCounter + (1 / (Player.HeldItem.useTime * 3f)), 1)) == 1)
				{
					int radiantType = ModContent.BuffType<DivineStrike>();

					if (!Player.HasBuff(radiantType))
					{
						ParticleHandler.SpawnParticle(new StarParticle(Player.Center + new Vector2(0, -10 * Player.gravDir), Vector2.Zero, Color.White, Color.Yellow, .2f, 10, 0));
						SoundEngine.PlaySound(SoundID.MaxMana, Player.Center);
					}
					Player.AddBuff(radiantType, 2);
				}
			}

			veilCounter = MathHelper.Max(veilCounter - .001f, 0);
			unholyCooldown = Math.Max(unholyCooldown - 1, 0);
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (veilCounter > 0)
			{
				float resistance = .5f; //Resist 50% damage at full charge
				modifiers.FinalDamage *= 1f - (resistance * veilCounter);
			}
		}

		public override void PostHurt(Player.HurtInfo info)
		{
			if (veilCounter > 0)
			{
				veilCounter = 0;

				for (int i = 0; i < (int)(10 * veilCounter); i++)
					Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Clentaminator_Cyan, 0, 0, 100).velocity = Vector2.UnitY * -2;
			}
		}
	}
}