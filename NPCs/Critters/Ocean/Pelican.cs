using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.Utilities;
using SpiritMod.Items.Sets.FloatingItems;
using Terraria.GameContent;
using Terraria.UI;
using System;
using System.IO;

namespace SpiritMod.NPCs.Critters.Ocean;

public class Pelican : ModNPC
{
	/// <summary>
	/// The item choices for this pelican. Set once on load, and unloaded.
	/// </summary>
	static WeightedRandom<int> choice;

	private int _heldItemType = ItemID.None;

	private ref float State => ref NPC.ai[0];
	private ref float Timer => ref NPC.ai[1];
	private ref float WalkState => ref NPC.ai[2];
	private ref float WalkTimer => ref NPC.ai[3];

	private Vector2 HeldItemPosition => NPC.position + new Vector2(NPC.spriteDirection == 1 ? 25 : 0, -4);

	public override void Load()
	{
		choice = new(Main.rand);
		choice.Add(ItemID.None, 6);
		choice.Add(ModContent.ItemType<Kelp>(), 0.25f);
		choice.Add(ModContent.ItemType<Items.Placeable.FishCrate>(), 0.05f);
		choice.Add(ModContent.ItemType<Items.Sets.FloatingItems.Driftwood.DriftwoodTileItem>(), 0.1f);
		choice.Add(ItemID.RedSnapper, 1f);
		choice.Add(ItemID.Shrimp, 0.5f);
		choice.Add(ItemID.Trout, 1.5f);
		choice.Add(ItemID.Tuna, 1f);
		choice.Add(ItemID.GoldenCarp, 0.01f);
	}

	public override void Unload() => choice = null;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 1;
		NPCID.Sets.CountsAsCritter[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.dontCountMe = true;
		NPC.width = 22;
		NPC.height = 22;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 5;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = 1f;
		NPC.aiStyle = -1;
		NPC.npcSlots = 0;
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		bestiaryEntry.UIInfoProvider = new CritterUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type]);
		bestiaryEntry.AddInfo(this, "Ocean");
	}

	public override void OnSpawn(IEntitySource source)
	{
		_heldItemType = choice;

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
				NPC.velocity.X = WalkState * 1.2f;
			else
				NPC.velocity.X *= 0.95f;

			if (WalkTimer == 0 || ++Timer % WalkTimer == 0)
			{
				WalkState = NPC.direction = WalkState == 0 ? (Main.rand.NextBool(2) ? -1 : 1) : 0;
				WalkTimer = Main.rand.Next(120, 240);
				NPC.netUpdate = true;
			}

			for (int i = 0; i < Main.maxPlayers; ++i) //Scare check
			{
				Player player = Main.player[i];

				if (player.active && !player.dead && player.DistanceSQ(NPC.Center) < 200 * 200 && player.velocity.LengthSquared() > 5 * 5)
				{
					State = 1;
					Timer = 0;

					NPC.direction = Math.Sign(NPC.Center.X - player.Center.X);
					NPC.netUpdate = true;

					if (_heldItemType != ItemID.None)
					{
						Item.NewItem(new EntitySource_Loot(NPC), NPC.Center, _heldItemType);
						_heldItemType = ItemID.None;
					}
					return;
				}
			}
		}
		else if (State == 1) //Almost entirely cleaned up vanilla AI
		{
			if (Main.player[NPC.target].dead)
				return;

			if (++Timer >= 300f) //Fall down and switch states when landing
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
				NPC.velocity.X -= 0.1f;

				if (NPC.velocity.X > 3f)
					NPC.velocity.X -= 0.1f;
				else if (NPC.velocity.X > 0f)
					NPC.velocity.X -= 0.05f;

				if (NPC.velocity.X < -3f)
					NPC.velocity.X = -3f;
			}
			else if (NPC.direction == 1 && NPC.velocity.X < 3f)
			{
				NPC.velocity.X += 0.1f;

				if (NPC.velocity.X < -3f)
					NPC.velocity.X += 0.1f;
				else if (NPC.velocity.X < 0f)
					NPC.velocity.X += 0.05f;

				if (NPC.velocity.X > 3f)
					NPC.velocity.X = 3f;
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
				NPC.velocity.Y += 0.15f;
			else
				NPC.velocity.Y -= 0.15f;

			if (veryCloseGround)
				NPC.velocity.Y -= 0.25f;

			NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -4.5f, 4);
		}
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (_heldItemType != ItemID.None)
		{
			Main.instance.LoadItem(_heldItemType);

			Item theItem = ContentSamples.ItemsByType[_heldItemType];
			Texture2D value = TextureAssets.Item[theItem.type].Value;
			Rectangle frame = (Main.itemAnimations[theItem.type] == null) ? value.Frame() : Main.itemAnimations[theItem.type].GetFrame(value);
			frame.Height /= 2;
			float scale = theItem.scale;

			const float SizeLimit = 20;

			if (frame.Width > SizeLimit || frame.Height > SizeLimit)
				scale = ((frame.Width <= frame.Height) ? (SizeLimit / frame.Height) : (SizeLimit / frame.Width));

			SpriteEffects effects = SpriteEffects.None;
			Color currentColor = Lighting.GetColor(NPC.Center.ToTileCoordinates());
			var pos = HeldItemPosition - screenPos;

			float modScale = 1f;
			ItemSlot.GetItemLight(ref currentColor, ref modScale, theItem);
			scale *= modScale;

			spriteBatch.Draw(value, pos, frame, currentColor, 0f, frame.Size() / 2f, scale, effects, 0f);

			if (theItem.color != default)
				spriteBatch.Draw(value, pos, frame, theItem.GetColor(currentColor), 0f, frame.Size() / 2f, scale, effects, 0f);
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{
			
		}
	}

	public override void SendExtraAI(BinaryWriter writer) => writer.Write(_heldItemType);
	public override void ReceiveExtraAI(BinaryReader reader) => _heldItemType = reader.ReadInt32();

	public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneBeach && Main.dayTime ? (spawnInfo.PlayerInTown ? 2f : 1f) : 0;
}