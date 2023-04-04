using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using SpiritMod.Items.Sets.ClubSubclass.ClubSandwich;
using static Terraria.ModLoader.ModContent;

namespace SpiritMod.Projectiles.Clubs
{
	class ClubSandwichProj : ClubProj
	{
		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Club Sandwich");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void Smash(Vector2 position)
		{
			Player player = Main.player[Projectile.owner];
			for (int k = 0; k <= 50; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<Dusts.EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);

			for (int i = 0; i < 5; i++)
			{
				if (Main.myPlayer == Projectile.owner)
				{
					int type = Main.rand.Next(4) switch
					{
						0 => ItemType<ClubMealOne>(),
						1 => ItemType<ClubMealTwo>(),
						2 => ItemType<ClubMealThree>(),
						_ => ItemType<ClubMealFour>()
					};

					int item = Item.NewItem(Projectile.GetSource_FromAI("ClubSmash"), Projectile.position, type);
					Main.item[item].velocity = Vector2.UnitY.RotatedBy(3.14f + Main.rand.NextFloat(3.14f)) * 8f * player.direction;

					if (Main.netMode != NetmodeID.SinglePlayer)
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item);
				}
			}
			SoundEngine.PlaySound(SoundID.NPCHit20, Projectile.position);
		}

		public ClubSandwichProj() : base(52, new Point(58, 58), 17f) { }
	}
}