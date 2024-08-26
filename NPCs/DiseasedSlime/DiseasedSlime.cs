using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.BismiteSet;
using SpiritMod.Items.Consumable.Food;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using SpiritMod.Buffs.DoT;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;

namespace SpiritMod.NPCs.DiseasedSlime
{
	public class DiseasedSlime : ModNPC
	{
		private int pickedType;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Noxious Slime");
			Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
			NPCHelper.ImmuneTo<FesteringWounds>(this, BuffID.Poisoned, BuffID.Venom);
		}

		public override void SetDefaults()
		{
			NPC.width = 44;
			NPC.height = 32;
			NPC.damage = 18;
			NPC.defense = 5;
			NPC.lifeMax = 65;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath22;
			NPC.value = 42f;
			NPC.alpha = 45;
			NPC.knockBackResist = .6f;
			NPC.aiStyle = 1;

			AIType = NPCID.BlueSlime;
			AnimationType = NPCID.BlueSlime;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.DiseasedSlimeBanner>();
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Underground");

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */ => NPC.lifeMax = 130;

		public override void OnSpawn(IEntitySource source)
		{
			NPC.scale = Main.rand.NextFloat(.9f, 1f);
			pickedType = Main.rand.Next(0, 5);
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frame.X = 50 * pickedType;
			NPC.frame.Width = 50;
		}

		public override void SendExtraAI(BinaryWriter writer) => writer.Write(pickedType);

		public override void ReceiveExtraAI(BinaryReader reader) => pickedType = reader.ReadInt32();

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.PlayerSafe || spawnInfo.PlayerInTown)
				return 0f;
			return SpawnCondition.Underground.Chance * 0.27f;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Vector2 extraOffset = new Vector2(-26, -17);
			Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.5f, NPC.height * 0.5f);
			Vector2 drawPos = NPC.Center - screenPos + drawOrigin + extraOffset;
			Color color = NPC.IsABestiaryIconDummy ? Color.White : NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor));
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
			return false;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<BismiteCrystal>(1, 4, 6);
			npcLoot.AddCommon(ItemID.Gel, 1, 2, 3);
			npcLoot.AddCommon(ItemID.SlimeStaff, 10000);
			npcLoot.AddCommon<Cake>(25);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			if (Main.rand.NextBool(3))
				target.AddBuff(ModContent.BuffType<FesteringWounds>(), 240);
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life > 0)
			{
				for (int k = 0; k < 12; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SlimeBunny, hit.HitDirection, -2.5f, 0, Color.Green * .14f, 0.7f);
			}
			else
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, 0f, 0f, ModContent.ProjectileType<NoxiousGas>(), 0, 1, Main.myPlayer, 0, 0);
					Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, 0f, 0f, ModContent.ProjectileType<NoxiousIndicator>(), 0, 1, Main.myPlayer, 0, 0);
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/GasHiss"), NPC.Center);
				}
				for (int k = 0; k < 25; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SlimeBunny, 2 * hit.HitDirection, -2.5f, 0, Color.Green * .14f, 0.7f);
			}
		}
	}
}