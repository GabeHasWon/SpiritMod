using Microsoft.Xna.Framework;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.Vulture_Matriarch.Matriarch_Wings
{
	[AutoloadEquip(EquipType.Wings)]
	public class Matriarch_Wings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Matriarch Wings");
			Tooltip.SetDefault("Hold down to dive");
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(90, 9.4f, 1.15f);
		}

		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 20;
			Item.value = Item.sellPrice(0, 3, 50, 0);
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			Matriarch_Wings_Visuals modplayer = player.GetModPlayer<Matriarch_Wings_Visuals>();
			modplayer.wingsEquipped = true;

			player.wingTimeMax = 100;

			if (player.controlDown)
			{
				if ((int)player.velocity.Y != 0)
				{
					if (Main.rand.NextBool(3))
						Dust.NewDust(player.position - (Vector2.UnitX * (player.width / 2)), player.width * 2, 30, DustID.Sandnado, Main.rand.NextFloat(-.1f, .1f), player.velocity.Y * Main.rand.NextFloat(0.8f, 1.2f), 0, default, Main.rand.NextFloat(0.8f, 1.35f));
					if (Main.rand.NextBool(6))
						Dust.NewDust(player.position - (Vector2.UnitX * (player.width / 2)), player.width * 2, 30, ModContent.DustType<Matriarch_Wings_Dust>(), Main.rand.NextFloat(-.1f, .1f), player.velocity.Y * -Main.rand.NextFloat(0.05f, 0.20f), 0, default, Main.rand.NextFloat(0.8f, 1.35f));

					player.gravity *= 2.5f;
					player.maxFallSpeed *= 2.5f;
				}

				if (modplayer.storedVelocityY != 0 && player.velocity.Y == 0) //Impact
				{
					modplayer.impact = MathHelper.Min(modplayer.storedVelocityY * 3, 30);

					for (int i = 0; i < (int)(modplayer.impact / 4); i++)
					{
						Vector2 position = player.position + new Vector2(player.width * Main.rand.NextFloat(), player.height);
						Vector2 velocity = -(Vector2.UnitY * Main.rand.NextFloat());
						float scale = Main.rand.NextFloat(0.3f, 1.0f);

						if (!Main.dedServ)
							ParticleHandler.SpawnParticle(new SmokeParticle(position, velocity, Color.Lerp(Color.Yellow, Color.WhiteSmoke, scale) * .5f, scale / 2, (int)(scale * 15)));

						Dust.NewDustPerfect(position, ModContent.DustType<Dusts.BoneDust>(), (velocity * 3f).RotatedByRandom(1.57f));
						Dust.NewDustPerfect(position, DustID.Smoke, velocity, 120, default, Main.rand.NextFloat(0.5f, 1.2f));

						SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown with { Volume = .5f }, player.position);
						SoundEngine.PlaySound(SoundID.NPCHit2 with { Volume = .25f, Pitch = -.5f }, player.position);
					}
				}

				modplayer.storedVelocityY = player.velocity.Y;
			}
			else modplayer.storedVelocityY = 0;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.65f;
			ascentWhenRising = 0.08f;
			maxCanAscendMultiplier = 1.25f;
			maxAscentMultiplier = 2.3f;
			constantAscend = 0.09f;
		}
	}
}