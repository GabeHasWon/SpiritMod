using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.Items.Sets.WhipsMisc.PoolNoodle;

public class PoolNoodle : ModItem
{
	private const int numStyles = 3;
	private byte style;

	public override void SetStaticDefaults()
		=> Main.RegisterItemAnimation(Type, new DrawAnimationVertical(2, numStyles) { NotActuallyAnimating = true });

	public override void SetDefaults()
	{
		Item.DefaultToWhip(ModContent.ProjectileType<PoolNoodleProj>(), 14, 0, 4);
		Item.width = Item.height = 38;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 30);
		style = (byte)Main.rand.Next(numStyles);
	}

	protected override bool CloneNewInstances => true;

	public override ModItem Clone(Item itemClone)
	{
		var myClone = (PoolNoodle)base.Clone(itemClone);
		myClone.style = style;

		return myClone;
	}

	public override bool MeleePrefix() => true;

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: style);
		return false;
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		Texture2D texture = TextureAssets.Item[Type].Value;
		frame = texture.Frame(1, numStyles, 0, style, 0, -2);

		spriteBatch.Draw(texture, position, frame, Item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D texture = TextureAssets.Item[Type].Value;
		Rectangle frame = texture.Frame(1, numStyles, 0, style, 0, -2);

		spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, Item.GetAlpha(lightColor), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override void SaveData(TagCompound tag) => tag[nameof(style)] = style;
	public override void LoadData(TagCompound tag) => style = tag.Get<byte>(nameof(style));
	public override void NetSend(BinaryWriter writer) => writer.Write(style);
	public override void NetReceive(BinaryReader reader) => style = reader.ReadByte();
}