using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Linq;

namespace SpiritMod.Items.Sets.DashSwordSubclass.AnimeSword
{
	public class AnimeSword : DashSwordItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Anime Sword");
			// Tooltip.SetDefault("Hold to slice through nearby enemies");
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.channel = true;
			Item.damage = 40;
			Item.knockBack = 1;
			Item.crit = 4;
			Item.width = Item.height = 60;
			Item.useTime = Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.Orange;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<AnimeSwordProj>();
			Item.shootSpeed = 6f;
			Item.noUseGraphic = true;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
	}

	public class AnimeSwordProj : DashSwordProjectile
	{
		private AnimePrimTrail trail;

		public override int ChargeupTime => 30;
		public override int DashDuration => 5;
		public override int StrikeDelay => 30;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Anime Sword");

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(40);
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 12;
			Projectile.tileCollide = false;
		}

		public override void AbstractAI()
		{
			if (Counter == ChargeupTime && Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/slashdash") with { PitchVariance = 0.4f, Volume = 0.4f }, Projectile.Center);
				SpiritMod.primitives.CreateTrail(trail = new AnimePrimTrail(Projectile));
			} //The dash has just started
			if (Counter == (ChargeupTime + DashDuration + 1) && Main.netMode != NetmodeID.Server)
			{
				if (trail != null)
					trail.OnDestroy();
			} //The dash has just ended a tick before

			if (Counter >= (ChargeupTime + DashDuration) && targets.Any())
			{
				int postCounter = (int)(Counter - (ChargeupTime + DashDuration));
				if (postCounter % (StrikeDelay / targets.Count) == 0)
				{
					int index = (int)MathHelper.Clamp(postCounter / (StrikeDelay / targets.Count), 0, targets.Count - 1);
					if (Main.netMode != NetmodeID.Server && Main.npc[targets[index]].active)
						SpiritMod.primitives.CreateTrail(new AnimePrimTrailTwo(Main.npc[targets[index]]));
				}
			}
		}
	}
}