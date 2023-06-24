using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Effects;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class GoldBlasterGNPC : GlobalNPC
	{
		public readonly int maxNumMarks = 25;
		public int numMarks;

		private bool isHovering;

		public override bool InstancePerEntity => true;

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (numMarks <= 0 || Main.dedServ)
				return;

			Texture2D texture = Mod.Assets.Request<Texture2D>("Items/Sets/GunsMisc/Blaster/Effects/GoldCasing").Value;
			int frames = 7;
			float frame = Main.GlobalTimeWrappedHourly * 10 % frames;

			Rectangle rect = new Rectangle(0, texture.Height / frames * (int)frame, texture.Width, (texture.Height / frames) - 2);
			Vector2 position = npc.Hitbox.TopLeft() - new Vector2(20) - screenPos;

			spriteBatch.Draw(texture, position, rect, new Color(180, 180, 180) * (isHovering ? 1f : 0.2f), 0f, rect.Size() / 2, 1f, SpriteEffects.None, 0f);
			if (isHovering)
				Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, $"x{numMarks}", position.X + 8, position.Y - 4, Color.White * .8f, Color.Black * .8f, Vector2.Zero, 0.9f);
		}

		public override bool PreAI(NPC npc)
		{
			if (!Main.dedServ)
			{
				Vector2 zoom = Main.GameViewMatrix.Zoom;

				Rectangle hoverBox = npc.getRect();
				hoverBox.Inflate((int)zoom.X, (int)zoom.Y);

				isHovering = hoverBox.Contains(Main.MouseWorld.ToPoint());
			}

			return base.PreAI(npc);
		}

		public void TryDetonate(NPC npc, int baseDamage, out bool success, Player player = null)
		{
			if (numMarks <= 0)
			{
				success = false;
				return;
			}

			Player _player = player ?? Main.LocalPlayer;
			Vector2 velocity = npc.DirectionTo(player.Center);

			int loops = (int)MathHelper.Clamp(numMarks, 1, 7);
			for (int i = 0; i < loops; i++)
			{
				Vector2 position = npc.Center + new Vector2(Main.rand.NextFloat(2.0f, 5.0f), 0).RotatedBy(npc.AngleTo(_player.Center));

				for (int o = 0; o < 5; o++)
				{
					Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.IchorTorch : DustID.GemTopaz,
						(velocity * Main.rand.NextFloat(2.0f, 7.0f)).RotatedByRandom(1f), 0, default, Main.rand.NextFloat(0.8f, 1.5f));
					dust.noGravity = true;
				}
			}
			if (Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Shotgun2") with { Volume = 0.7f, PitchVariance = 0.3f }, npc.Center);

			success = true;
			npc.StrikeNPC(numMarks * baseDamage, 0, 1);

			numMarks = 0;
		}

		public void TryVoidMarks(NPC npc)
		{
			if (numMarks <= 0)
				return;

			int loops = (int)MathHelper.Clamp(numMarks, 1, 7);
			for (int i = 0; i < loops; i++)
			{
				Vector2 position = npc.Hitbox.TopLeft() + new Vector2(Main.rand.Next(npc.Hitbox.Width), Main.rand.Next(npc.Hitbox.Height));

				Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(Main.rand.NextFloat(0.5f, 3.0f), Main.rand.NextFloat(0.5f, 3.0f)), ModContent.GoreType<GoldCasing>());

				for (int o = 0; o < 4; o++)
				{
					Dust dust = Dust.NewDustPerfect(position, DustID.GoldCoin, (position.DirectionFrom(npc.Center) * Main.rand.NextFloat(1.0f, 3.0f)).RotatedByRandom(1f), 50, default, Main.rand.NextFloat(0.5f, 1.0f));
					dust.noGravity = true;
				}
			}
			if (Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/Ricochet") with { PitchVariance = 0.5f }, npc.Center);

			numMarks = 0;
		}
	}
}