"""Act3 fireworks festival props — outlined pixel art for park/night map placement."""
from PIL import Image
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act3_Fireworks"
os.makedirs(OUT, exist_ok=True)

CLR = (0, 0, 0, 0)
OUTLINE = (12, 10, 18, 255)

NIGHT = (18, 22, 48, 255)
NIGHT_H = (32, 40, 72, 255)
WATER = (28, 48, 98, 255)
WATER_H = (60, 110, 170, 255)
STEEL = (120, 128, 148, 255)
STEEL_H = (180, 190, 210, 255)
STEEL_D = (70, 78, 96, 255)
WOOD = (140, 96, 58, 255)
WOOD_H = (180, 130, 80, 255)
WOOD_D = (90, 58, 34, 255)
ORANGE = (255, 140, 60, 255)
ORANGE_H = (255, 190, 110, 255)
PINK = (255, 90, 160, 255)
CYAN = (80, 220, 255, 255)
GOLD = (255, 210, 90, 255)
RED = (220, 60, 70, 255)
LEAF = (180, 90, 50, 255)
LEAF_H = (220, 140, 70, 255)
GREEN_D = (40, 70, 48, 255)
WHITE = (245, 246, 250, 255)
GRAY = (90, 94, 110, 255)
WARM = (255, 200, 120, 255)
SIL = (40, 36, 48, 255)


def new(w, h):
    return Image.new("RGBA", (w, h), CLR)


def px(img, x, y, c):
    if 0 <= x < img.width and 0 <= y < img.height and c[3] > 0:
        img.putpixel((x, y), c)


def rect(img, x0, y0, x1, y1, c):
    for y in range(y0, y1):
        for x in range(x0, x1):
            px(img, x, y, c)


def hline(img, x0, x1, y, c):
    for x in range(x0, x1 + 1):
        px(img, x, y, c)


def vline(img, x, y0, y1, c):
    for y in range(y0, y1 + 1):
        px(img, x, y, c)


def oval(img, cx, cy, rx, ry, c):
    for y in range(cy - ry, cy + ry + 1):
        for x in range(cx - rx, cx + rx + 1):
            if rx == 0 or ry == 0:
                continue
            if ((x - cx) / float(rx)) ** 2 + ((y - cy) / float(ry)) ** 2 <= 1.05:
                px(img, x, y, c)


def shade_rect(img, x0, y0, x1, y1, base, hi=None, lo=None):
    rect(img, x0, y0, x1, y1, base)
    if hi:
        hline(img, x0, x1 - 1, y0, hi)
        vline(img, x0, y0, y1 - 1, hi)
    if lo:
        hline(img, x0, x1 - 1, y1 - 1, lo)
        vline(img, x1 - 1, y0, y1 - 1, lo)


def add_outline(img, color=OUTLINE, solid_alpha=180):
    w, h = img.size
    src = img.copy()
    out = Image.new("RGBA", (w, h), CLR)
    solid = [[src.getpixel((x, y))[3] >= solid_alpha for x in range(w)] for y in range(h)]
    for y in range(h):
        for x in range(w):
            if solid[y][x]:
                continue
            edge = False
            for dy in (-1, 0, 1):
                for dx in (-1, 0, 1):
                    if dx == 0 and dy == 0:
                        continue
                    nx, ny = x + dx, y + dy
                    if 0 <= nx < w and 0 <= ny < h and solid[ny][nx]:
                        edge = True
                        break
                if edge:
                    break
            if edge:
                out.putpixel((x, y), color)
    out.paste(src, (0, 0), src)
    return out


def save(img, name):
    path = os.path.join(OUT, name)
    add_outline(img).save(path)
    print("wrote", path)


# --- props ---

def prop_river_rail():
    img = new(64, 32)
    shade_rect(img, 2, 18, 62, 22, STEEL, STEEL_H, STEEL_D)
    for x in range(6, 60, 10):
        vline(img, x, 6, 18, STEEL)
        oval(img, x, 5, 2, 2, STEEL_H)
    hline(img, 2, 61, 6, STEEL_H)
    return img


def prop_park_bench():
    img = new(48, 32)
    shade_rect(img, 4, 14, 44, 18, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 4, 8, 44, 12, WOOD, WOOD_H, WOOD_D)
    vline(img, 8, 18, 28, WOOD_D)
    vline(img, 39, 18, 28, WOOD_D)
    shade_rect(img, 6, 26, 12, 30, STEEL_D)
    shade_rect(img, 36, 26, 42, 30, STEEL_D)
    return img


def prop_food_stall():
    img = new(64, 64)
    # cart body
    shade_rect(img, 8, 28, 56, 52, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 10, 20, 54, 28, ORANGE, ORANGE_H, RED)
    # awning stripes
    for x in range(10, 54, 6):
        c = PINK if (x // 6) % 2 == 0 else CYAN
        rect(img, x, 14, x + 6, 20, c)
    # window
    shade_rect(img, 18, 32, 38, 46, (40, 50, 70, 220), CYAN, STEEL_D)
    # wheels
    oval(img, 16, 54, 5, 5, SIL)
    oval(img, 48, 54, 5, 5, SIL)
    oval(img, 16, 54, 2, 2, GRAY)
    oval(img, 48, 54, 2, 2, GRAY)
    # steam
    oval(img, 44, 24, 3, 2, WHITE)
    oval(img, 48, 20, 2, 2, WHITE)
    return img


def prop_lantern_pole():
    img = new(24, 64)
    vline(img, 11, 8, 60, STEEL_D)
    vline(img, 12, 8, 60, STEEL)
    shade_rect(img, 6, 4, 18, 16, ORANGE, ORANGE_H, RED)
    oval(img, 12, 10, 4, 5, WARM)
    shade_rect(img, 8, 58, 16, 62, STEEL_D)
    return img


def prop_festival_flag():
    img = new(40, 56)
    vline(img, 6, 4, 52, WOOD_D)
    # pennants
    for i, col in enumerate((PINK, CYAN, GOLD, ORANGE)):
        y = 8 + i * 10
        for x in range(8, 34):
            tip = 8 + int((x - 8) * 0.35)
            if y <= tip + 6:
                px(img, x, tip + (x % 3), col)
                px(img, x, tip + 1 + (x % 3), col)
    return img


def prop_crowd_silhouette():
    img = new(64, 40)
    # several people blobs
    people = [(10, 28, 6), (22, 26, 7), (34, 28, 6), (46, 25, 8), (56, 29, 5)]
    for cx, cy, r in people:
        oval(img, cx, cy - r, r - 1, r, SIL)
        oval(img, cx, cy - 2 * r - 2, max(2, r // 2), max(2, r // 2), SIL)
        # phone glow
        if cx % 20 < 12:
            rect(img, cx + 2, cy - 2 * r, cx + 5, cy - 2 * r + 6, CYAN)
    return img


def prop_bridge_pillar():
    img = new(32, 64)
    shade_rect(img, 8, 4, 24, 60, STEEL, STEEL_H, STEEL_D)
    for y in range(10, 56, 10):
        hline(img, 8, 23, y, STEEL_D)
    shade_rect(img, 4, 56, 28, 62, STEEL_D, STEEL, None)
    # light
    oval(img, 16, 8, 3, 3, GOLD)
    return img


def prop_pier_plank():
    img = new(64, 24)
    shade_rect(img, 2, 8, 62, 20, WOOD, WOOD_H, WOOD_D)
    for x in range(6, 60, 8):
        vline(img, x, 8, 19, WOOD_D)
    # water hint under
    for x in range(4, 60, 5):
        px(img, x, 21, WATER_H)
    return img


def prop_tree_autumn():
    img = new(48, 64)
    shade_rect(img, 20, 28, 28, 60, WOOD_D, WOOD, None)
    oval(img, 24, 22, 16, 14, LEAF)
    oval(img, 14, 26, 10, 9, LEAF_H)
    oval(img, 34, 24, 11, 10, ORANGE)
    oval(img, 24, 14, 9, 8, GOLD)
    return img


def prop_trash_bin():
    img = new(24, 32)
    shade_rect(img, 4, 8, 20, 28, GRAY, STEEL_H, STEEL_D)
    shade_rect(img, 3, 6, 21, 10, STEEL, STEEL_H, STEEL_D)
    hline(img, 6, 17, 14, STEEL_D)
    return img


def prop_vendor_cart():
    img = new(56, 40)
    shade_rect(img, 6, 14, 48, 30, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 8, 8, 46, 14, PINK, ORANGE_H, RED)
    oval(img, 14, 32, 4, 4, SIL)
    oval(img, 40, 32, 4, 4, SIL)
    # balloons
    oval(img, 46, 6, 4, 4, CYAN)
    oval(img, 50, 10, 3, 3, GOLD)
    vline(img, 46, 10, 14, GRAY)
    return img


def prop_string_lanterns():
    img = new(80, 32)
    hline(img, 2, 77, 6, STEEL_D)
    for i, col in enumerate((ORANGE, PINK, GOLD, CYAN, ORANGE, PINK)):
        x = 8 + i * 12
        vline(img, x, 6, 12, GRAY)
        oval(img, x, 16, 4, 5, col)
        oval(img, x, 16, 2, 2, WARM)
    return img


def prop_speaker_stack():
    img = new(32, 48)
    shade_rect(img, 6, 8, 26, 42, SIL, GRAY, STEEL_D)
    oval(img, 16, 18, 6, 6, STEEL_D)
    oval(img, 16, 32, 7, 7, STEEL_D)
    oval(img, 16, 18, 2, 2, CYAN)
    oval(img, 16, 32, 3, 3, PINK)
    return img


def prop_barrier_cone():
    img = new(24, 32)
    for y in range(6, 28):
        half = max(1, (y - 6) // 3)
        for x in range(12 - half, 12 + half + 1):
            c = ORANGE if (y // 3) % 2 == 0 else WHITE
            px(img, x, y, c)
    shade_rect(img, 4, 28, 20, 31, STEEL_D)
    return img


def prop_photo_frame():
    img = new(40, 48)
    shade_rect(img, 4, 4, 36, 40, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 8, 8, 32, 32, NIGHT_H, CYAN, None)
    # mini firework
    for a in range(-2, 3):
        px(img, 20 + a, 18, GOLD)
        px(img, 20, 18 + a, PINK)
    shade_rect(img, 14, 40, 26, 46, WOOD_D)
    return img


def prop_empty_bench():
    img = new(48, 28)
    shade_rect(img, 4, 12, 44, 16, STEEL_D, GRAY, SIL)
    vline(img, 8, 16, 24, STEEL_D)
    vline(img, 39, 16, 24, STEEL_D)
    # lonely phone glow on ground
    rect(img, 22, 20, 26, 24, CYAN)
    return img


def prop_river_edge():
    img = new(64, 24)
    shade_rect(img, 0, 10, 64, 16, STEEL_D, STEEL, None)
    for x in range(0, 64, 3):
        px(img, x, 16 + (x % 4), WATER_H)
        px(img, x + 1, 18 + (x % 3), WATER)
    return img


def prop_night_bush():
    img = new(40, 28)
    oval(img, 20, 16, 16, 10, GREEN_D)
    oval(img, 10, 18, 8, 7, (30, 55, 40, 255))
    oval(img, 30, 17, 9, 8, (50, 80, 45, 255))
    # autumn leaves speck
    px(img, 14, 12, LEAF)
    px(img, 24, 10, ORANGE)
    px(img, 28, 14, GOLD)
    return img


def tile_night_path():
    img = new(32, 32)
    shade_rect(img, 0, 0, 32, 32, (48, 52, 64, 255), (70, 76, 92, 255), (30, 32, 44, 255))
    for y in range(0, 32, 8):
        for x in range(0, 32, 8):
            if (x // 8 + y // 8) % 2 == 0:
                rect(img, x, y, x + 8, y + 8, (56, 60, 74, 255))
    return img


def tile_bridge_deck():
    img = new(32, 16)
    shade_rect(img, 0, 2, 32, 14, WOOD, WOOD_H, WOOD_D)
    for x in range(4, 32, 8):
        vline(img, x, 2, 13, WOOD_D)
    return img


def bg_night_sky():
    img = new(128, 64)
    for y in range(64):
        t = y / 63.0
        r = int(12 + t * 30)
        g = int(14 + t * 20)
        b = int(40 + t * 50)
        hline(img, 0, 127, y, (r, g, b, 255))
    # stars
    for x, y in ((10, 8), (30, 14), (55, 6), (80, 18), (100, 10), (118, 22), (40, 28), (70, 12)):
        px(img, x, y, WHITE)
        if x % 20 == 10:
            px(img, x + 1, y, (200, 210, 255, 200))
    # distant firework dots
    oval(img, 90, 20, 3, 3, PINK)
    oval(img, 40, 16, 2, 2, GOLD)
    oval(img, 110, 28, 2, 2, CYAN)
    return img


def hazard_ember():
    img = new(16, 16)
    oval(img, 8, 9, 4, 5, ORANGE)
    oval(img, 8, 8, 2, 3, GOLD)
    px(img, 8, 3, WHITE)
    px(img, 7, 5, ORANGE_H)
    px(img, 9, 5, RED)
    return img


def hazard_crowd_bump():
    img = new(32, 24)
    oval(img, 10, 14, 6, 8, SIL)
    oval(img, 22, 14, 6, 8, (50, 44, 60, 255))
    oval(img, 10, 6, 3, 3, SIL)
    oval(img, 22, 6, 3, 3, (50, 44, 60, 255))
    # bump arrows
    hline(img, 12, 20, 4, PINK)
    px(img, 20, 3, PINK)
    px(img, 20, 5, PINK)
    return img


def vfx_firework_window():
    img = new(48, 48)
    # soft burst rings
    for r, c in ((18, (255, 90, 160, 90)), (12, (255, 200, 90, 120)), (6, (255, 255, 220, 180))):
        for y in range(48):
            for x in range(48):
                d2 = (x - 24) ** 2 + (y - 24) ** 2
                if (r - 1) ** 2 <= d2 <= (r + 1) ** 2:
                    px(img, x, y, c)
    for a in range(0, 360, 30):
        import math
        rad = math.radians(a)
        for dist in range(4, 20):
            x = int(24 + math.cos(rad) * dist)
            y = int(24 + math.sin(rad) * dist)
            col = GOLD if dist < 10 else PINK if dist < 15 else CYAN
            px(img, x, y, col)
    oval(img, 24, 24, 3, 3, WHITE)
    return img


def main():
    props = [
        ("Act3_Prop_RiverRail.png", prop_river_rail),
        ("Act3_Prop_ParkBench.png", prop_park_bench),
        ("Act3_Prop_FoodStall.png", prop_food_stall),
        ("Act3_Prop_LanternPole.png", prop_lantern_pole),
        ("Act3_Prop_FestivalFlag.png", prop_festival_flag),
        ("Act3_Prop_CrowdSilhouette.png", prop_crowd_silhouette),
        ("Act3_Prop_BridgePillar.png", prop_bridge_pillar),
        ("Act3_Prop_PierPlank.png", prop_pier_plank),
        ("Act3_Prop_TreeAutumn.png", prop_tree_autumn),
        ("Act3_Prop_TrashBin.png", prop_trash_bin),
        ("Act3_Prop_VendorCart.png", prop_vendor_cart),
        ("Act3_Prop_StringLanterns.png", prop_string_lanterns),
        ("Act3_Prop_SpeakerStack.png", prop_speaker_stack),
        ("Act3_Prop_BarrierCone.png", prop_barrier_cone),
        ("Act3_Prop_PhotoFrame.png", prop_photo_frame),
        ("Act3_Prop_EmptyBench.png", prop_empty_bench),
        ("Act3_Prop_RiverEdge.png", prop_river_edge),
        ("Act3_Prop_NightBush.png", prop_night_bush),
        ("Act3_Tile_NightPath.png", tile_night_path),
        ("Act3_Tile_BridgeDeck.png", tile_bridge_deck),
        ("Act3_BG_NightSky.png", bg_night_sky),
        ("Act3_Hazard_Ember.png", hazard_ember),
        ("Act3_Hazard_CrowdBump.png", hazard_crowd_bump),
        ("Act3_Vfx_FireworkWindow.png", vfx_firework_window),
    ]
    for name, fn in props:
        save(fn(), name)
    print(f"done {len(props)} assets → {OUT}")


if __name__ == "__main__":
    main()
