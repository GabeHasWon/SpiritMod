using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using System;

namespace SpiritMod.NPCs.Critters.Ocean;

public class SandPiper : ModNPC
{
	private ref float State => ref NPC.ai[0];
	private ref float Timer => ref NPC.ai[1];
	private ref float WalkState => ref NPC.ai[2];
	private ref float WalkTimer => ref NPC.ai[3];

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 1;
		Main.npcCatchable[Type] = true;

		NPCID.Sets.CountsAsCritter[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.dontCountMe = true;
		NPC.width = 24;
		NPC.height = 20;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 5;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = 1f;
		NPC.aiStyle = -1;
		NPC.npcSlots = 0;
		NPC.catchItem = (short)ModContent.ItemType<SandPiperItem>();
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		bestiaryEntry.UIInfoProvider = new CritterUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type]);

		bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
			new FlavorTextBestiaryInfoElement("A small, brave bird that patrols the shorelines looking for food uncovered by the waves"),
		});
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.direction = -1;
		NPC.netUpdate = true;
	}

	public override void AI()
	{
		NPC.noGravity = true;

		if (Math.Abs(NPC.velocity.X) > 0.001f)
			NPC.spriteDirection = NPC.direction = Math.Sign(NPC.velocity.X);

		if (State == 0)
		{
			NPC.noGravity = NPC.wet;

			if (NPC.wet)
			{
				NPC.velocity.Y *= 0.7f;

				if (Collision.WetCollision(NPC.position, NPC.width, 18))
					NPC.velocity.Y -= 0.2f;
			}

			if (NPC.collideX)
				NPC.direction *= -1;

			if (WalkState != 0)
				NPC.velocity.X = WalkState * 2f;
			else
				NPC.velocity.X *= 0.92f;

			if (WalkTimer == 0 || ++Timer % WalkTimer == 0)
			{
				WalkState = NPC.direction = WalkState == 0 ? (Main.rand.NextBool(2) ? -1 : 1) : 0;
				WalkTimer = Main.rand.Next(80, 160);
				NPC.netUpdate = true;
			}

			for (int i = 0; i < Main.maxPlayers; ++i) //Scare check
			{
				Player player = Main.player[i];

				if (player.active && !player.dead && player.DistanceSQ(NPC.Center) < 300 * 300)
				{
					float dist = player.Distance(NPC.Center);

					if (dist > 150)
						NPC.velocity.X = Math.Sign(NPC.Center.X - player.Center.X) * (1 - ((dist - 150f) / 150f)) * 5;
					else 
					{ 
						State = 1;
						Timer = 0;

						NPC.direction = NPC.Center.X < player.Center.X ? -1 : 1;
						NPC.netUpdate = true;
						break;
					}
				}
			}
		}
		else if (State == 1) //Almost entirely cleaned up vanilla AI
		{
			if (Main.player[NPC.target].dead)
				return;

			for (int i = 0; i < Main.maxPlayers; ++i) //Scare check
			{
				Player player = Main.player[i];

				if (player.active && !player.dead && player.DistanceSQ(NPC.Center) < 400 * 400)
				{
					Timer--;
					break;
				}
			}

			if (++Timer >= 130f) //Fall down and switch states when landing
			{
				if (((NPC.velocity.Y == 0f || NPC.collideY) && Collision.SolidCollision(NPC.BottomLeft, NPC.width, 6)) || NPC.wet)
				{
					NPC.velocity.X = 0f;
					NPC.velocity.Y = 0f;
					State = 0f;
					Timer = 0f;
				}
				else
				{
					NPC.velocity.X *= 0.98f;
					NPC.velocity.Y += 0.15f;
					if (NPC.velocity.Y > 2.5f)
						NPC.velocity.Y = 2.5f;
				}
				return;
			}

			if (NPC.collideX)
			{
				NPC.direction *= -1;
				NPC.velocity.X = NPC.oldVelocity.X * -0.5f;
				NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -2, 2);
			}

			if (NPC.collideY)
			{
				NPC.velocity.Y = NPC.oldVelocity.Y * -0.5f;
				NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -1, 1);
			}

			if (NPC.direction == -1 && NPC.velocity.X > -3f)
			{
				NPC.velocity.X -= 0.15f;

				if (NPC.velocity.X > 4f)
					NPC.velocity.X -= 0.1f;
				else if (NPC.velocity.X > 0f)
					NPC.velocity.X -= 0.05f;

				if (NPC.velocity.X < -4f)
					NPC.velocity.X = -4f;
			}
			else if (NPC.direction == 1 && NPC.velocity.X < 3f)
			{
				NPC.velocity.X += 0.15f;

				if (NPC.velocity.X < -4f)
					NPC.velocity.X += 0.1f;
				else if (NPC.velocity.X < 0f)
					NPC.velocity.X += 0.05f;

				if (NPC.velocity.X > 4f)
					NPC.velocity.X = 4f;
			}

			const int ScanCheck = 15;

			int tileX = (int)(NPC.Center.X / 16f) + NPC.direction;
			int tileY = (int)(NPC.Bottom.Y / 16f);
			bool closeGround = true;
			bool veryCloseGround = false;

			for (int y = tileY; y < tileY + ScanCheck; y++)
			{
				if ((Main.tile[tileX, y].HasUnactuatedTile && Main.tileSolid[Main.tile[tileX, y].TileType]) || Main.tile[tileX, y].LiquidAmount > 0)
				{
					if (y < tileY + 5)
						veryCloseGround = true;

					closeGround = false;
					break;
				}
			}

			if (closeGround)
				NPC.velocity.Y += 0.08f;
			else
				NPC.velocity.Y -= 0.08f;

			if (veryCloseGround)
				NPC.velocity.Y -= 0.15f;

			NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -4.5f, 4);
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{
			
		}
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneBeach && Main.dayTime ? (spawnInfo.PlayerInTown ? 2f : 1f) : 0;
}

[Sacrifice(1)]
public class SandPiperItem : ModItem
{
	public override string Texture => base.Texture.Replace("Item", "");

	// public override void SetStaticDefaults() => DisplayName.SetDefault("Sand Piper");

	public override void SetDefaults()
	{
		Item.width = Item.height = 20;
		Item.rare = ItemRarityID.White;
		Item.maxStack = 99;
		Item.value = Item.sellPrice(0, 0, 5, 0);
		Item.noUseGraphic = true;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 20;
		Item.noMelee = true;
		Item.consumable = true;
		Item.autoReuse = true;
		Item.makeNPC = ModContent.NPCType<SandPiper>();
	}
}