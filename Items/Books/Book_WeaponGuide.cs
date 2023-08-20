using SpiritMod.Items.Sets.BismiteSet;
using SpiritMod.Items.Sets.FloranSet;
using SpiritMod.Items.Sets.FrigidSet;
using Terraria.ModLoader;

namespace SpiritMod.Items.Books
{
    class Book_WeaponGuide : Book
    {
        public override string BookText => "[c/95a3ab:The following is a transcript of an informational speech made by adventurer Aurelius Finch, Knight-Crusader.]\n\n---\n\n" +
            "Hey, all! Knight-Crusader Aurelius here, again. Before we get started (and on a completely unrelated note), my last guide on Armor didn't sell as well as I want to, and I have no clue why! But I need YOUR money to support my lavish hero lifestyle. So buy this new weapon guide for your dad, your grandma, and maybe even your cat. Please? I promise it'll be useful!\n\n" +
            "[c/80deed:The Beginner's Guide to Weaponry]\n\n" +
            "Aside from a strong set of armor, you'll need a trusty weapon by your side to really stand out as an adventurer. Now, not all of us can have a weapon that's as flashy as my trusted fireblade. And some of you never will! But that's okay. Let's start small, with weapons that are reliable and easy to forge.\n\n" +
            "[i:" + SpiritMod.Instance.Find<ModItem>("IcySpear").Type + "] [i:" + ModContent.ItemType<FrigidFragment>() + "] [c/80deed: Frigid Javelin:] Do you live close to a snowy tundra or boreal forest? Then this weapon is perfect for you! Simply harvest some [c/72a4c2:Frigid Fragments] and temper them at an anvil to make this ice-cold, razor sharp weapon. Unlike other inaccurate throwing weapons (who uses those, anyway?), you can aim for precise strikes with javelins!\n\n" +
			"[i:" + SpiritMod.Instance.Find<ModItem>("BismiteSummonStaff").Type + "] [i:" + ModContent.ItemType<BismiteCrystal>() + "] [c/80deed: Bismite Crystal Staff:] Normally, I wouldn't recommend summoned creatures, since they love to bite me and rip up my things. But you won't have that problem with a semi-sentient poisonous rock, that's for sure. Made of [c/72a4c2:Bismite Shards], it's sure to help deal with enemies for you.\n\n" +
			"[i:" + SpiritMod.Instance.Find<ModItem>("ButterflyStaff").Type + "] [i:1994] [c/80deed: Ethereal Butterfly Staff] Again, familiars and I don't get along, but this one has no teeth or claws! All it takes is a wild [c/72a4c2:butterfly], [c/72a4c2:wood], and some [c/72a4c2:fallen stars], and you'll have yourself a new agile companion. Next time you see a [c/72a4c2:butterfly], get to netting! \n\n" +
			"[i:" + SpiritMod.Instance.Find<ModItem>("ClawCannon").Type + "] [i:" + ModContent.ItemType<Sets.ReefhunterSet.IridescentScale>() + "] [c/80deed: Claw Cannon:] The ocean can be terrifying. But there's no better way to fight your fears than by harnessing them. With some [c/72a4c2:iridescent scales] from hostile deep-sea creatures, [c/72a4c2:sulfur deposits], and [c/72a4c2:iron], you can create the ultimate bubble blower. I think. A bubble gun, hypothetically, might be cooler. \n\n" +
            "[i:" + ModContent.ItemType<Florang>() + "] [i:" + ModContent.ItemType<FloranBar>() + "] [c/80deed: Floran Cutter:] Now, if you want a sleek, cool, and potent weapon, look no further. The caveat is that you'll need to venture into the depths of the dark Briar to harvest [c/72a4c2:Floran Bars]. Are you capable of doing that? Probably not! But if you do manage to luck out, throw these sharp disks along the ground and watch them cut up your foes.\n\n" +
			"[i:" + SpiritMod.Instance.Find<ModItem>("CactusStaff").Type + "] [i:276] [c/80deed: Cactus Staff:] Would you ever think that [c/72a4c2:cacti] would have magical properties? I sure wouldn't! The only interaction I've had with cacti is falling straight into a pit of pricklethorns when I was a novice adventurer, like you. But I've been told that Cactus Staves are quite powerful and poisonous.\n\n" +
            "---\n\n" +
            "I'm barely breaking even anymore. These pieces of junk better tide me over so I can afford that solid gold statue I commissioned. Well, the people still love me. I'm sure they'll buy my book this time. Hey, why are you still here? The speech is over. Shoo!";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Beginner's Guide to Weaponry");
            // Tooltip.SetDefault("by Knight-Crusader Aurelius");
        }
    }
}