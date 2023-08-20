using Microsoft.Xna.Framework;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.AccessoriesMisc.CrystalFlower
{
	public class CrystalFlowerItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Crystal Flower");
			// Tooltip.SetDefault("Enemies have a chance to explode into damaging crystals on death");
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetSpiritPlayer().crystalFlower = true;

		public static void OnKillEffect(IEntitySource source, Player player, NPC target, int damage)
		{
			SoundEngine.PlaySound(SoundID.Item107, target.Center);

			int numProjectiles = Main.rand.Next(3, 5) + 1;

			int finalDamage = (int)(damage / (numProjectiles * .5f));
			float maxVelocity = 8.0f;
			Vector2 basePosition = target.Center;

			NPC[] storedNPCs = new NPC[numProjectiles];
			int storedCount = -1;

			foreach (NPC npc in Main.npc) //Create an array of nearby NPCs
			{
				if (npc.active && npc.Distance(basePosition) < (maxVelocity * 24))
					storedNPCs[++storedCount] = npc;
				if ((storedCount + 1) >= numProjectiles)
					break;
			}

			if (Main.netMode == NetmodeID.MultiplayerClient) //The projectiles are spawned on the server
				return;
			for (int i = 0; i < numProjectiles; i++)
			{
				Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(maxVelocity * .5f, maxVelocity);

				if (storedCount > -1)
				{
					int index = Math.Min(storedCount, i / (numProjectiles / (storedCount + 1)));
					velocity = (basePosition.DirectionTo(storedNPCs[index].Center) * maxVelocity).RotatedByRandom(.3f);
				}

				Projectile proj = Projectile.NewProjectileDirect(source, basePosition, velocity, ModContent.ProjectileType<Items.Sets.AccessoriesMisc.CrystalFlower.CrystalFlowerProjectile>(), finalDamage, 0, player.whoAmI);
				proj.scale = Main.rand.NextFloat(0.5f, 1.0f);

				if (Main.dedServ)
					continue;
				for (int o = 0; o < 4; o++)
				{
					float randomScale = Main.rand.NextFloat(0.1f, 0.6f);
					ParticleHandler.SpawnParticle(new FireParticle(basePosition, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, maxVelocity),
						Color.White, Color.Magenta, randomScale, (int)(randomScale * 60)));
				}
			}
		}
	}
}
