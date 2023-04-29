using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.DonatorVanity;
using SpiritMod.NPCs;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items;

public abstract class BossBagItem : ModItem
{
	internal abstract string BossName { get; }

	public sealed override void SetStaticDefaults()
	{
		DisplayName.SetDefault($"Treasure Bag ({BossName})");
		Tooltip.SetDefault("Consumable\nRight Click to open");

		StaticDefaults();
	}

	public sealed override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.rare = -2;
		Item.maxStack = 30;
		Item.expert = true;
		Defaults();
	}

	public virtual void StaticDefaults() { }
	public virtual void Defaults() { }

	public sealed override bool CanRightClick() => true;

	/// <summary>
	/// Adds all donator vanity, gold coins, mask and trophy to the ItemLoot as a OneFromOptions rule.
	/// </summary>
	public static void AddBossItems<TMask, TTrophy>(ItemLoot loot, Range goldCoinRange) where TTrophy : ModItem where TMask : ModItem
	{
		loot.AddCommon(ItemID.GoldCoin, 1, goldCoinRange.Start.Value, goldCoinRange.End.Value);
		loot.AddCommon<TMask>(7);
		loot.AddCommon<TTrophy>(10);
		loot.AddOneFromOptions<WaasephiVanity, MeteorVanity, PixelatedFireballVanity, LightNovasVanity>(20);
	}

	public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

	public sealed override void PostUpdate()
	{
		// Spawn some light and dust when dropped in the world
		Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

		if (Item.timeSinceItemSpawned % 12 == 0)
		{
			Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);

			// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
			Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
			float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
			Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

			Dust dust = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
			dust.scale = 0.5f;
			dust.fadeIn = 1.1f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.alpha = 0;
		}
	}

	public sealed override bool PreDrawInWorld(SpriteBatch spriteBatch, Color light, Color a, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D texture = TextureAssets.Item[Item.type].Value;
		Rectangle frame;

		if (Main.itemAnimations[Item.type] != null)
			frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
		else
			frame = texture.Frame();

		Vector2 frameOrigin = frame.Size() / 2f;
		Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
		Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

		float time = Main.GlobalTimeWrappedHourly;
		float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

		time = time % 4f / 2f;

		if (time >= 1f)
			time = 2f - time;

		time = time * 0.5f + 0.5f;

		for (float i = 0f; i < 1f; i += 0.25f)
		{
			float radians = (i + timer) * MathHelper.TwoPi;
			spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
		}

		for (float i = 0f; i < 1f; i += 0.34f)
		{
			float radians = (i + timer) * MathHelper.TwoPi;
			spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
		}

		return true;
	}
}
