using Microsoft.Xna.Framework;
using SpiritMod.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles
{
	class MiscEffectProjectile : GlobalProjectile
	{
		public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			Player player = Main.player[projectile.owner];
			MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();

			// Ace of Spades
			if (modPlayer.AceOfSpades && crit)
			{
				damage = (int)(damage * 1.2f);
				for (int i = 0; i < 3; i++)
					Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<SpadeDust>(), 0, -0.8f);
			}

			// Ace of Clubs
			if (modPlayer.AceOfClubs && crit && !target.friendly && target.lifeMax > 15 && !target.SpawnedFromStatue && target.type != NPCID.TargetDummy)
			{
				//int money = (int)(300 * MathHelper.Clamp(damage / target.lifeMax, 1 / 295f, 1f));
				int money = (int)(damage);
				for (int i = 0; i < 3; i++)
					Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<ClubDust>(), 0, -0.8f);

				if (money / 1000000 > 0)
					ItemUtils.NewItemWithSync(projectile.GetSource_OnHit(target), projectile.owner, (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.PlatinumCoin, money / 1000000);

				money %= 1000000;
				if (money / 10000 > 0)
					ItemUtils.NewItemWithSync(projectile.GetSource_OnHit(target), projectile.owner, (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.GoldCoin, money / 10000);

				money %= 10000;
				if (money / 100 > 0)
					ItemUtils.NewItemWithSync(projectile.GetSource_OnHit(target), projectile.owner, (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.SilverCoin, money / 100);

				money %= 100;
				if (money > 0)
					ItemUtils.NewItemWithSync(projectile.GetSource_OnHit(target), projectile.owner, (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.CopperCoin, money);
			}

			// Briar Set Bonus
			if (modPlayer.reachSet && target.life <= target.life / 2 && projectile.IsThrown() && crit)
				damage = (int)(damage * 2.25f);
		}

		public int storedUseTime = 0;
		public override bool InstancePerEntity => true;
		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			if(source is EntitySource_ItemUse item)
			{
				storedUseTime = item.Item.useTime; 
			}
		}
		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[projectile.owner];
			MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();

			// Jellynaut Helmet
			if (modPlayer.jellynautHelm && modPlayer.jellynautStacks < 4 && projectile.IsMagic() && (target.life <= 0 || Main.rand.NextBool(8)) && !target.friendly && !target.SpawnedFromStatue)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int p = Projectile.NewProjectile(projectile.GetSource_OnHit(target), player.position.X + Main.rand.Next(-20, 20), player.position.Y + Main.rand.Next(-20, 0), 1, -1, ModContent.ProjectileType<Projectiles.Magic.JellynautOrbiter>(), 0, 0, Main.myPlayer);
					Main.projectile[p].scale = Main.rand.NextFloat(.5f, 1f);
					modPlayer.jellynautStacks++;
				}
			}

			//Ace of Hearts
			if (modPlayer.AceOfHearts && Main.rand.NextFloat() < (storedUseTime / 75f) && crit && !target.friendly && target.lifeMax > 15 && !target.SpawnedFromStatue && target.type != NPCID.TargetDummy)
			{
				ItemUtils.NewItemWithSync(projectile.GetSource_OnHit(target), projectile.owner, (int)target.position.X, (int)target.position.Y, target.width, target.height, Main.halloween ? ItemID.CandyApple : ItemID.Heart);
				for (int i = 0; i < 3; i++)
					Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<HeartDust>(), 0, -0.8f);
			}

			// Ace of Diamonds
			if (modPlayer.AceOfDiamonds && Main.rand.NextFloat() < (storedUseTime / 75f) && crit && !target.friendly && target.lifeMax > 15 && !target.SpawnedFromStatue && target.type != NPCID.TargetDummy)
			{
				ItemUtils.NewItemWithSync(projectile.GetSource_OnHit(target), projectile.owner, (int)target.position.X, (int)target.position.Y, target.width, target.height, ModContent.ItemType<Items.Accessory.AceCardsSet.DiamondAce>());
				for (int i = 0; i < 3; i++)
					Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<DiamondDust>(), 0, -0.8f);
			}

			// Geode Set
			if (projectile.friendly && projectile.IsThrown() && Main.rand.NextBool(4) && modPlayer.geodeSet)
				target.AddBuff(Main.rand.NextBool() ? 24 : 44, 150);
		}
	}
}
