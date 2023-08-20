using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.SummonTag;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.BaseProj
{
	public abstract class BaseWhipProj : ModProjectile
	{
		private ref float Timer => ref Projectile.ai[0];

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
			StaticDefaults();
		}

		public virtual void StaticDefaults() { }

		public override void SetDefaults()
		{
			Projectile.DefaultToWhip();
			Defaults();
		}

		public virtual void Defaults() { }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<SummonTag3>(), 180, true);
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(Projectile.damage * .5f);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) { }

		public override bool PreDraw(ref Color lightColor)
		{
			List<Vector2> list = new List<Vector2>();
			//Main.DrawWhip_WhipBland(Projectile, list);
			Projectile.FillWhipControlPoints(Projectile, list);

			SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++)
			{
				Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type]);
				ModifyDraw(i, list.Count - 1, ref frame);

				Vector2 origin = frame.Size() / 2;
				float scale = 1;

				if (i == list.Count - 2) //Draw head
				{
					frame.Y = frame.Height * (Main.projFrames[Type] - 1);

					Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
					float t = Timer / timeToFlyOut;
					scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
				}
				else if (i > 0) //Draw body segments
				{
					frame.Y = frame.Height * ((int)((float)i / (list.Count - 1) * (Main.projFrames[Type] - 2)) + 1);
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

				pos += diff;
			}
			return false;
		}

		/// <summary>
		/// Called for every whip segment drawn.
		/// </summary>
		/// <param name="segment"></param>
		/// <param name="numSegments"></param>
		/// <param name="frame"></param>
		public virtual void ModifyDraw(int segment, int numSegments, ref Rectangle frame) { }
	}
}