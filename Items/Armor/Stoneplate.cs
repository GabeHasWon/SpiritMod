using Microsoft.Xna.Framework;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class Stoneplate : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stoneplate");
			Tooltip.SetDefault("Encumbers the wearer");
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 18;
			Item.value = Item.buyPrice(0, 0, 10, 0);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 15;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetModPlayer<MyPlayer>().stoneplate = true;

			player.noKnockback = true;

			player.gravity *= 2;
			player.maxFallSpeed *= 2;
			player.jumpSpeedBoost *= .5f;

			if (player.GetModPlayer<MyPlayer>().justLanded)
			{
				for (int i = 0; i < 5; i++)
				{
					Vector2 position = player.position + new Vector2(player.width * Main.rand.NextFloat(), player.height);
					Vector2 velocity = -(Vector2.UnitY * Main.rand.NextFloat());
					float scale = Main.rand.NextFloat(0.3f, 1.0f);

					if (!Main.dedServ)
						ParticleHandler.SpawnParticle(new SmokeParticle(position, velocity, Color.Lerp(Color.LightGray, Color.WhiteSmoke, scale) * .5f, scale / 2, (int)(scale * 15)));

					Dust.NewDustPerfect(position, ModContent.DustType<Dusts.BoneDust>(), (velocity * 3f).RotatedByRandom(1.57f));
					Dust.NewDustPerfect(position, DustID.Smoke, velocity, 120, default, Main.rand.NextFloat(0.5f, 1.2f));

					SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = .75f }, player.position);
				}
			}
			if (player.velocity.X != 0 && Main.timeForVisualEffects % 50 == 0)
				for (int i = 0; i < 4; i++)
					Dust.NewDust(player.position, player.width, player.width, DustID.Water, (Vector2.Normalize(player.velocity).X * Main.rand.NextFloat(0.0f, 3.0f)), Main.rand.NextFloat(-3.0f, 0.0f), 0, default, Main.rand.NextFloat(0.8f, 1.2f));
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.StoneBlock, 250);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}