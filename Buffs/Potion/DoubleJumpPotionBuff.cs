using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Buffs.Potion
{
	public class DoubleJumpPotionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex) => player.GetJumpState<AetherJump>().Enable();
	}

	public class AetherJump : ExtraJump
	{
		public override Position GetDefaultPosition() => new Before(CloudInABottle);

		public override float GetDurationMultiplier(Player player) => .75f;

		public override void OnStarted(Player player, ref bool playSound)
		{
			static void SpawnGore(Player player, Vector2 position)
			{
				Gore gore = Gore.NewGoreDirect(player.GetSource_FromThis(), position, -player.velocity, Main.rand.Next(11, 14));
				gore.velocity.X = gore.velocity.X * 0.1f - player.velocity.X * 0.1f;
				gore.velocity.Y = gore.velocity.Y * 0.1f - player.velocity.Y * 0.05f;
			}

			int offsetY = ((player.gravDir == -1f) ? 0 : player.height) - 16;
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(player.position + new Vector2(-34f, offsetY), 102, 32, DustID.Cloud, -player.velocity.X * .5f, player.velocity.Y * .5f, 100, default, 1.5f);
				dust.velocity = dust.velocity * 0.5f - player.velocity * new Vector2(0.1f, 0.3f);
			}

			SpawnGore(player, player.Top + new Vector2(-16f, offsetY));
			SpawnGore(player, player.position + new Vector2(-36f, offsetY));
			SpawnGore(player, player.TopRight + new Vector2(4f, offsetY));
		}

		public override void ShowVisuals(Player player)
		{
			int offsetY = (player.gravDir == -1f) ? -6 : player.height;

			Dust dust = Dust.NewDustDirect(player.position + new Vector2(-4, offsetY), player.width + 8, 4, DustID.Cloud, -player.velocity.X * .5f, player.velocity.Y * .5f, 100, default, 1.5f);
			dust.velocity = dust.velocity * 0.5f - player.velocity * new Vector2(0.1f, 0.3f);
		}
	}
}