using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.Characters;
using StardewValley.Locations;
using StardewValley.Projectiles;
using StardewValley.Tools;
using ZinkleStardewMod.Content.ContentTypes;
using ZinkleStardewMod.Utils;

namespace ZinkleStardewMod.Content.Items;

[HarmonyPatch(typeof(BasicProjectile), nameof(BasicProjectile.behaviorOnCollisionWithMonster), [typeof(NPC), typeof(GameLocation)])]
public class RockThatTurnsMarnieTurquoise : ModObject
{
    public override string DisplayName => "Rock That Turns Marnie Turquoise";
    public override string Description => 
        "hmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm";
    public override int SpriteIndex => 0;
    public override void SetProperties()
    {
        Type = Types.Basic;
        Category = Categories.Mineral;
    }
    public override IEnumerable<(string, int)> GetRecipe(ref int outputAmount) => [(SObject.aquamarineID, 1)];
    public override bool CanAttachTo(Tool tool, int slot) => tool is Slingshot;
    [HarmonyPrefix]
    private static bool PrefixShootMarnie(BasicProjectile __instance, NPC n, GameLocation location)
    {
        if (!n.displayName.Contains("Marnie") || __instance.itemId.Value != $"(O){RockThatTurnsMarnieTurquoise.ItemID}")
            return true;
        MarnieData.SetMarnieTurquoise(n, true);
        for (int i = 0; i < 10; i++)
            Game1.createRadialDebris(location, 1, (int)(n.position.X + 32f + Game1.random.Next(-16, 16)) / 64, (int)(n.position.Y + 32f + Game1.random.Next(-16, 16)) / 64, 6, false, 1, false, Color.Turquoise);
        for (int x = (int)n.Position.X - 50; x < n.Position.X + 50; x++)
        {
            if (Game1.random.NextBool(0.95f)) continue;
            for (int y = (int)n.Position.Y - 80; y < n.Position.Y + 50; y++)
            {
                if (Game1.random.NextBool(0.95f)) continue;
                var pos = new Vector2(x, y);
                location.temporarySprites.Add(new TemporaryAnimatedSprite(13, pos, Color.Turquoise, 2, false, 100f, 0, -1, 1f, -1, Game1.random.Next(60)));
            }
        }

        return false;
    }
}
[HarmonyPatch(typeof(NPC))]
internal static class MarnieTurquoisePatch
{
    [HarmonyPatch(nameof(NPC.draw))]
    [HarmonyPrefix]
    private static bool PrefixDraw(NPC __instance, SpriteBatch b, float alpha)
    {
        if (__instance.Name.Contains("Marnie") && MarnieData.GetMarnieTurquoise(__instance))
        {
            var color = (MarnieData.GetMarnieTurquoise(__instance) ? Color.Turquoise : Color.White) * (1 - MarnieData.GetMarnieResurrectionTime(__instance) / (float)MarnieData.MarnieResurrectionTimeMax);
            int standingY = __instance.StandingPixel.Y;
            float mainLayerDepth = Math.Max(0f, __instance.drawOnTop ? 0.991f : ((float)standingY / 10000f));
            if (__instance.Sprite.Texture == null)
            {
                Vector2 position = Game1.GlobalToLocal(Game1.viewport, __instance.Position);
                Rectangle spriteArea = new((int)position.X, (int)position.Y - __instance.Sprite.SpriteWidth * 4, __instance.Sprite.SpriteWidth * 4, __instance.Sprite.SpriteHeight * 4);
                Utility.DrawErrorTexture(b, spriteArea, mainLayerDepth);
            }
            else if (!__instance.IsInvisible && (Utility.isOnScreen(__instance.Position, 128) || (__instance.EventActor && __instance.currentLocation is Summit)))
            {
                if (__instance.swimming.Value)
                {
                    b.Draw(__instance.Sprite.Texture, __instance.getLocalPosition(Game1.viewport) + new Vector2(32f, 80 + __instance.yJumpOffset * 2) + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero) - new Vector2(0f, __instance.yOffset), new Rectangle(__instance.Sprite.SourceRect.X, __instance.Sprite.SourceRect.Y, __instance.Sprite.SourceRect.Width, __instance.Sprite.SourceRect.Height / 2 - (int)(__instance.yOffset / 4f)), color, __instance.rotation, new Vector2(32f, 96f) / 4f, Math.Max(0.2f, __instance.scale.Value) * 4f, __instance.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, mainLayerDepth);
                    Vector2 localPosition = __instance.getLocalPosition(Game1.viewport);
                    b.Draw(Game1.staminaRect, new Rectangle((int)localPosition.X + (int)__instance.yOffset + 8, (int)localPosition.Y - 128 + __instance.Sprite.SourceRect.Height * 4 + 48 + __instance.yJumpOffset * 2 - (int)__instance.yOffset, __instance.Sprite.SourceRect.Width * 4 - (int)__instance.yOffset * 2 - 16, 4), Game1.staminaRect.Bounds, color * 0.75f, 0f, Vector2.Zero, SpriteEffects.None, (float)standingY / 10000f + 0.001f);
                }
                else
                {
                    b.Draw(__instance.Sprite.Texture, __instance.getLocalPosition(Game1.viewport) + new Vector2(__instance.GetSpriteWidthForPositioning() * 4 / 2, __instance.GetBoundingBox().Height / 2) + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), __instance.Sprite.SourceRect, color * alpha, __instance.rotation, new Vector2(__instance.Sprite.SpriteWidth / 2, (float)__instance.Sprite.SpriteHeight * 3f / 4f), Math.Max(0.2f, __instance.scale.Value) * 4f, (__instance.flip || (__instance.Sprite.CurrentAnimation != null && __instance.Sprite.CurrentAnimation[__instance.Sprite.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, mainLayerDepth);
                }
                __instance.DrawBreathing(b, alpha);
                __instance.DrawGlow(b);
                if (!Game1.eventUp)
                {
                    __instance.DrawEmote(b);
                }
            }
            return false;
        }
        return true;
    }
    [HarmonyPatch(nameof(NPC.DrawBreathing))]
    [HarmonyPrefix]
    private static bool PrefixDrawBreathing(NPC __instance, SpriteBatch b, float alpha)
    {
        if (__instance.Name.Contains("Marnie") && MarnieData.GetMarnieTurquoise(__instance))
        {
            var color = (MarnieData.GetMarnieTurquoise(__instance) ? Color.Turquoise : Color.White) * (1 - MarnieData.GetMarnieResurrectionTime(__instance) / (float)MarnieData.MarnieResurrectionTimeMax);
            if (!__instance.Breather || __instance.shakeTimer > 0 || __instance.swimming.Value || __instance.farmerPassesThrough)
            {
                return false;
            }
            AnimatedSprite animatedSprite = __instance.Sprite;
            if (animatedSprite != null && animatedSprite.SpriteHeight > 32)
            {
                return false;
            }
            AnimatedSprite animatedSprite2 = __instance.Sprite;
            if (animatedSprite2 != null && animatedSprite2.SpriteWidth > 16)
            {
                return false;
            }
            AnimatedSprite sprite = __instance.Sprite;
            if (sprite.currentFrame >= 16)
            {
                return false;
            }
            CharacterData data = __instance.GetData();
            Rectangle spriteRect = sprite.SourceRect;
            Rectangle chestBox;
            if (data != null && data.BreathChestRect.HasValue)
            {
                Rectangle dataRect = data.BreathChestRect.Value;
                chestBox = new Rectangle(spriteRect.X + dataRect.X, spriteRect.Y + dataRect.Y, dataRect.Width, dataRect.Height);
            }
            else
            {
                chestBox = new Rectangle(spriteRect.X + sprite.SpriteWidth / 4, spriteRect.Y + sprite.SpriteHeight / 2 + sprite.SpriteHeight / 32, sprite.SpriteHeight / 4, sprite.SpriteWidth / 2);
                if (__instance.Age == 2)
                {
                    chestBox.Y += sprite.SpriteHeight / 6 + 1;
                    chestBox.Height /= 2;
                }
                else if (__instance.Gender == Gender.Female)
                {
                    chestBox.Y++;
                    chestBox.Height /= 2;
                }
            }
            Vector2 chestPosition;
            if (data != null && data.BreathChestPosition.HasValue)
            {
                chestPosition = Utility.PointToVector2(data.BreathChestPosition.Value);
            }
            else
            {
                chestPosition = new Vector2(sprite.SpriteWidth * 4 / 2, 8f);
                if (__instance.Age == 2)
                {
                    chestPosition.Y += sprite.SpriteHeight / 8 * 4;
                    if (__instance is Child { Age: var num })
                    {
                        switch (num)
                        {
                            case 0:
                                chestPosition.X -= 12f;
                                break;
                            case 1:
                                chestPosition.X -= 4f;
                                break;
                        }
                    }
                }
                else if (__instance.Gender == Gender.Female)
                {
                    chestPosition.Y -= 4f;
                }
            }
            float breathScale = Math.Max(0f, (float)Math.Ceiling(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 600.0 + (double)(__instance.defaultPosition.X * 20f))) / 4f);
            int standingY = __instance.StandingPixel.Y;
            b.Draw(sprite.Texture, __instance.getLocalPosition(Game1.viewport) + chestPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), chestBox, color * alpha, __instance.rotation, new Vector2(chestBox.Width / 2, chestBox.Height / 2 + 1), Math.Max(0.2f, __instance.scale.Value) * 4f + breathScale, __instance.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, __instance.drawOnTop ? 0.992f : (((float)standingY + 0.01f) / 10000f)));
            return false;
        }
        return true;
    }
}