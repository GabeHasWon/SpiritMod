using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public abstract class SubtypeProj : ModProjectile
	{
		public enum Subtypes : byte
		{
			Fire = 0,
			Poison = 1,
			Frost = 2,
			Plasma = 3,
			Count
		}

		public byte Subtype { get; set; }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			int? debuffType = Debuff;
			if (debuffType != null)
				target.AddBuff(debuffType.Value, 200);
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(Subtype);
		public override void ReceiveExtraAI(BinaryReader reader) => Subtype = reader.ReadByte();

		internal static Color GetColor(int index)
		{
			return index switch
			{
				(int)Subtypes.Poison => new Color(22, 245, 140),
				(int)Subtypes.Frost => new Color(138, 158, 225),
				(int)Subtypes.Plasma => new Color(250, 60, 225),
				_ => new Color(255, 230, 65)
			};
		}

		protected int? Debuff
			=> Subtype switch
			{
				(int)Subtypes.Poison => BuffID.Poisoned,
				(int)Subtypes.Frost => BuffID.Frostburn,
				(int)Subtypes.Plasma => null,
				_ => BuffID.OnFire
			};

		protected int[] Dusts
			=> Subtype switch
			{
				(int)Subtypes.Poison => new int[] { DustID.FartInAJar, DustID.GreenTorch },
				(int)Subtypes.Frost => new int[] { DustID.FrostHydra, DustID.IceTorch },
				(int)Subtypes.Plasma => new int[] { DustID.Pixie, DustID.PinkTorch },
				_ => new int[] { DustID.SolarFlare, DustID.Torch }
			};
	}
}