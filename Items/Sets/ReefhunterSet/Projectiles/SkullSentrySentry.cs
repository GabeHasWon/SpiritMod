using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.IO;
using SpiritMod.NPCs.BloodGazer;

namespace SpiritMod.Items.Sets.ReefhunterSet.Projectiles
{
	public class SkullSentrySentry : ModProjectile
	{
		const int EyeCount = 3;

		int[] eyeWhoAmIs = null;

		public override void SetDefaults()
		{
			Projectile.width = 46;
			Projectile.height = 64;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.aiStyle = -1;
			Projectile.sentry = true;
			Projectile.timeLeft = Projectile.SentryLifeTime;
			Projectile.scale = 0.75f;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			if (eyeWhoAmIs is null) //Init
			{
				DoDust();
				eyeWhoAmIs = new int[EyeCount];
				Vector2[] offsets = [new Vector2(-10, -8) * Projectile.scale, new Vector2(10, -8) * Projectile.scale, new Vector2(0, 18) * Projectile.scale];

				for (int i = 0; i < EyeCount; ++i)
				{
					Vector2 pos = Projectile.Center + offsets[i];
					int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<SkullSentryEye>(), Main.player[Projectile.owner].HeldItem.damage, 0f, Projectile.owner, -1, i * SkullSentryEye.SHOOT_TIME / 3);

					Projectile eye = Main.projectile[p];
					(eye.ModProjectile as SkullSentryEye).anchor = offsets[i];
					(eye.ModProjectile as SkullSentryEye).parent = Projectile;
					eye.scale = Projectile.scale;
					eye.netUpdate = true;

					eyeWhoAmIs[i] = p;
				}
			}

			foreach (Projectile p in GetEyeList()) //Refresh all eye targets at once per tick
				if (p.ModProjectile is SkullSentryEye eye)
					eye.Target = -1;

			Projectile.velocity.Y = (float)System.Math.Sin(Projectile.timeLeft / 30f) / 6; //Subtle hovering
		}

		public override bool? CanDamage() => false;

		public override void OnKill(int timeLeft)
		{
			DoDust();

			if (eyeWhoAmIs is null)
				return;

			foreach (Projectile p in GetEyeList())
				p.Kill();
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.QuickDraw(Main.spriteBatch);

			if (eyeWhoAmIs is null)
				return false;

			//Manually call drawing for eyes after this projectile's drawing is called (eyes have hide = true, so they don't automatically get drawn)
			List<Projectile> eyes = GetEyeList();
			foreach (Projectile p in eyes) //Draw all the chains before all the eyes
				(p.ModProjectile as SkullSentryEye).DrawChain(Main.spriteBatch);

			foreach (Projectile p in eyes)
				(p.ModProjectile as SkullSentryEye).Draw(Main.spriteBatch, lightColor);

			return false;
		}

		public List<Projectile> GetEyeList()
		{
			List<Projectile> temp = [];

			foreach (int i in eyeWhoAmIs)
			{
				Projectile proj = Main.projectile[i];
				if (proj.active && proj.type == ModContent.ProjectileType<SkullSentryEye>() && proj != null)
					if ((proj.ModProjectile as SkullSentryEye).parent == Projectile)
						temp.Add(proj);
			}

			return temp;
		}

		private void DoDust()
		{
			const int DustAmount = 25;

			for (int i = 0; i < DustAmount; i++)
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Main.rand.NextFloat(-0.2f, 0.2f),
					Main.rand.NextFloat(-0.2f, 0.2f), 0, new Color(98, 180, 162), Main.rand.NextFloat(0.8f, 1.1f));
				Main.dust[d].noGravity = true;
			}
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(eyeWhoAmIs is not null);

			if (eyeWhoAmIs is null)
				return;

			for (int i = 0; i < EyeCount; ++i)
				writer.Write(eyeWhoAmIs[i]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			bool read = reader.ReadBoolean();

			if (read)
			{
				if (eyeWhoAmIs is null)
					eyeWhoAmIs = new int[EyeCount];

				for (int i = 0; i < EyeCount; ++i)
					eyeWhoAmIs[i] = reader.ReadInt32();
			}
		}
	}
}