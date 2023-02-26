using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.Fathomless_Chest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest
{
	public abstract class ChanceEffect
	{
		public virtual bool Unlucky => false;

		public virtual bool Selectable(Point16 tileCoords) => true;

		public void Trigger(Player player, Point16 tileCoords)
		{
			var source = new EntitySource_TileBreak(tileCoords.X, tileCoords.Y);
			if (!Selectable(tileCoords))
				return;

			Effects(player, tileCoords, new EntitySource_TileBreak(tileCoords.X, tileCoords.Y));

			OnKillVase(player, tileCoords, source);

			//Do mandantory luck VFX
			for (int d = 0; d < 9; d++)
			{
				Vector2 speed = new Vector2((float)(-1 * Main.rand.Next(40, 70) * 0.00999999977648258 + Main.rand.Next(-20, 21) * 0.4f));

				Projectile proj = Projectile.NewProjectileDirect(source, new Vector2((float)(tileCoords.X * 16) + 8 + speed.X, (float)(tileCoords.Y * 16) + 12 + speed.Y), speed, ModContent.ProjectileType<Visual_Projectile>(), 0, 0f, player.whoAmI, 0.0f, 0.0f);
				proj.scale = Main.rand.Next(30, 150) * 0.01f;
			}

			Player localP = Main.LocalPlayer;

			Color color = Unlucky ? new Color(255, 150, 150) : new Color(150, 255, 150);
			string text = Unlucky ? "Bad Luck!" : "Good Luck!";

			CombatText combatText = Main.combatText[CombatText.NewText(new Rectangle(tileCoords.X * 16, tileCoords.Y * 16, localP.width, localP.height), color, text, false, false)];
			NetMessage.SendData(MessageID.CombatTextInt, -1, -1, NetworkText.FromLiteral(combatText.text), (int)combatText.color.PackedValue, combatText.position.X, combatText.position.Y, 0.0f, 0, 0, 0);
		}

		public virtual void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
		}

		/// <summary>
		/// Plays a sound and spawns gores when the Fathomless Vase is destroyed
		/// </summary>
		/// <param name="player">The player who interacted with the vase</param>
		/// <param name="tileCoords">The tile coordinates of the vase</param>
		/// <param name="source">The entity source relevant to this tile interaction</param>
		public virtual void OnKillVase(Player player, Point16 tileCoords, IEntitySource source)
		{
			SoundEngine.PlaySound(SoundID.DD2_SkeletonDeath, tileCoords.ToVector2() * 16);

			for (int g = 0; g < 6; g++)
			{
				Vector2 tilePos = new Vector2(tileCoords.X * 16, tileCoords.Y * 16);
				Gore gore = Gore.NewGoreDirect(source, tileCoords.ToVector2() * 16, Vector2.Zero, 99, 1.1f);
				gore.velocity *= 0.6f;

				if (g < 5)
				{
					gore = Gore.NewGoreDirect(source, tilePos + new Vector2(0, 4), Vector2.Zero, SpiritMod.Instance.Find<ModGore>("FathomlessChest" + (g + 1)).Type, 1f);
					gore.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 1.8f);
				}
			}

		}
	}

	public static class ChanceEffectManager
	{
		public static readonly List<ChanceEffect> effectIndex = new();

		public static void Load()
		{
			IEnumerable<Type> effects = typeof(Fathomless_Chest).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ChanceEffect)));
			foreach (Type type in effects)
			{
				ChanceEffect chanceEffect = (ChanceEffect)Activator.CreateInstance(type);
				effectIndex.Add(chanceEffect); //These may be added asynchronously between clients
			}
		}
	}
}
