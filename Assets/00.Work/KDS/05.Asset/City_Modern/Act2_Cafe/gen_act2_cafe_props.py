"""Act2 cafe street props — outlined pixel art for map placement."""
from PIL import Image
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act2_Cafe"
os.makedirs(OUT, exist_ok=True)

CLR = (0, 0, 0, 0)
OUTLINE = (18, 14, 16, 255)

WOOD = (168, 118, 78, 255)
WOOD_H = (198, 150, 108, 255)
WOOD_D = (118, 78, 48, 255)
CREAM = (248, 240, 228, 255)
CREAM_D = (220, 206, 186, 255)
PINK = (255, 140, 178, 255)
PINK_H = (255, 190, 210, 255)
MINT = (120, 210, 180, 255)
MINT_H = (170, 235, 210, 255)
COFFEE = (110, 70, 42, 255)
COFFEE_H = (150, 105, 70, 255)
FOAM = (250, 246, 236, 255)
WHITE = (250, 250, 252, 255)
BLACK = (32, 28, 34, 255)
GRAY = (120, 118, 128, 255)
GRAY_H = (170, 168, 178, 255)
NEON_P = (255, 90, 180, 255)
NEON_C = (80, 220, 255, 255)
NEON_Y = (255, 220, 90, 255)
LEAF = (70, 150, 90, 255)
LEAF_H = (110, 190, 120, 255)
GLASS = (180, 220, 240, 160)
GOLD = (230, 180, 70, 255)
BRICK = (170, 90, 78, 255)
BRICK_D = (130, 60, 52, 255)
NIGHT = (28, 34, 58, 255)
WARM = (255, 210, 130, 255)
SHADOW = (20, 16, 18, 80)


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
            if ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 <= 1.05:
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

def prop_latte_table():
    img = new(48, 48)
    # table
    shade_rect(img, 8, 28, 40, 34, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 14, 34, 18, 46, WOOD_D, WOOD, BLACK)
    shade_rect(img, 30, 34, 34, 46, WOOD_D, WOOD, BLACK)
    oval(img, 24, 30, 16, 4, SHADOW)
    # cup
    shade_rect(img, 20, 18, 28, 28, WHITE, CREAM, GRAY)
    rect(img, 21, 19, 27, 24, COFFEE)
    oval(img, 24, 19, 3, 2, FOAM)
    # latte heart foam
    px(img, 23, 20, FOAM)
    px(img, 25, 20, FOAM)
    px(img, 24, 21, FOAM)
    # handle
    shade_rect(img, 28, 20, 32, 25, WHITE, CREAM, GRAY)
    rect(img, 29, 21, 31, 24, CLR)
    # saucer
    oval(img, 24, 28, 8, 2, CREAM_D)
    # window light hint
    rect(img, 6, 8, 10, 22, GLASS)
    return img


def prop_macaron_sculpture():
    img = new(48, 64)
    # pedestal
    shade_rect(img, 12, 48, 36, 58, GRAY_H, WHITE, GRAY)
    shade_rect(img, 16, 58, 32, 62, GRAY, GRAY_H, BLACK)
    # stacked macarons
    colors = [
        (PINK, PINK_H),
        (MINT, MINT_H),
        (NEON_Y, (255, 240, 160, 255)),
        (NEON_C, (160, 240, 255, 255)),
        (PINK_H, WHITE),
    ]
    y = 46
    for i, (base, hi) in enumerate(colors):
        w = 18 - i
        oval(img, 24, y, w, 5, base)
        oval(img, 24, y - 2, w - 2, 3, hi)
        oval(img, 24, y + 1, w - 4, 2, CREAM)
        y -= 8
    # sparkle
    px(img, 10, 20, WHITE)
    px(img, 38, 14, WHITE)
    px(img, 30, 8, GOLD)
    return img


def prop_donut_wall():
    img = new(64, 48)
    shade_rect(img, 4, 4, 60, 44, WOOD, WOOD_H, WOOD_D)
    # pegboard holes
    for y in range(8, 40, 6):
        for x in range(10, 56, 6):
            px(img, x, y, WOOD_D)
    donuts = [
        (12, 14, PINK), (24, 12, MINT), (36, 15, NEON_Y), (48, 13, NEON_C),
        (16, 28, COFFEE_H), (28, 30, PINK_H), (40, 27, WHITE), (50, 29, GOLD),
    ]
    for x, y, c in donuts:
        oval(img, x, y, 5, 4, c)
        oval(img, x, y, 2, 2, CLR)
        # glaze drip
        px(img, x - 1, y + 3, c)
        px(img, x + 1, y + 3, WHITE)
    return img


def prop_cafe_sign():
    img = new(64, 48)
    # wall strip
    shade_rect(img, 2, 10, 62, 42, BRICK, BRICK, BRICK_D)
    for y in (16, 24, 32):
        hline(img, 2, 61, y, BRICK_D)
    # sign board
    shade_rect(img, 10, 14, 54, 36, BLACK, GRAY, BLACK)
    # neon CAFE
    neon = [NEON_P, NEON_C, NEON_Y, NEON_P]
    letters = [(14, 18), (24, 18), (34, 18), (44, 18)]
    for (x, y), col in zip(letters, neon):
        shade_rect(img, x, y, x + 7, y + 12, col, WHITE, OUTLINE)
    # awning
    for i in range(8):
        c = PINK if i % 2 == 0 else WHITE
        shade_rect(img, 8 + i * 6, 6, 14 + i * 6, 14, c, WHITE if c == PINK else CREAM, GRAY)
    return img


def prop_terrace():
    img = new(64, 48)
    # railing
    shade_rect(img, 2, 28, 62, 32, GRAY_H, WHITE, GRAY)
    for x in range(6, 60, 8):
        vline(img, x, 18, 28, GRAY)
    # city silhouette
    for i, h in enumerate([10, 16, 12, 20, 14, 18, 11]):
        shade_rect(img, 6 + i * 7, 28 - h, 12 + i * 7, 28, (70, 82, 110, 255), (90, 100, 130, 255), BLACK)
        if h > 12:
            px(img, 8 + i * 7, 28 - h + 3, WARM)
    # planter
    shade_rect(img, 20, 32, 44, 40, WOOD, WOOD_H, WOOD_D)
    for x in range(24, 42, 4):
        oval(img, x, 30, 3, 4, LEAF)
        px(img, x, 27, LEAF_H)
    return img


def prop_mirror_wall():
    img = new(48, 64)
    shade_rect(img, 6, 4, 42, 60, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 10, 8, 38, 54, GLASS, WHITE, GRAY)
    # reflection figure silhouette
    oval(img, 24, 22, 5, 5, (200, 210, 220, 200))
    shade_rect(img, 20, 27, 28, 42, (180, 190, 210, 180), None, None)
    # frame lights
    for y in (10, 28, 46):
        px(img, 8, y, NEON_P)
        px(img, 40, y, NEON_C)
    return img


def prop_greenhouse():
    img = new(48, 64)
    # pot
    shade_rect(img, 14, 48, 34, 58, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 16, 58, 32, 62, WOOD_D, WOOD, BLACK)
    # soil
    rect(img, 16, 46, 32, 50, COFFEE)
    # leaves / vines
    oval(img, 24, 36, 14, 12, LEAF)
    oval(img, 18, 28, 8, 10, LEAF_H)
    oval(img, 30, 26, 7, 9, LEAF)
    oval(img, 24, 20, 6, 8, MINT)
    # hanging vine
    for y in range(12, 40, 3):
        px(img, 10, y, LEAF)
        px(img, 38, y + 1, LEAF_H)
    # greenhouse glass hint
    hline(img, 8, 40, 8, GLASS)
    vline(img, 8, 8, 48, GLASS)
    vline(img, 40, 8, 48, GLASS)
    return img


def prop_book_wall():
    img = new(48, 64)
    shade_rect(img, 4, 4, 44, 60, WOOD_D, WOOD, BLACK)
    shelves = [10, 22, 34, 46]
    palette = [PINK, MINT, NEON_Y, NEON_C, COFFEE_H, GOLD, WHITE, GRAY_H]
    for si, y in enumerate(shelves):
        hline(img, 6, 41, y + 8, WOOD)
        x = 8
        for bi in range(6):
            c = palette[(si * 3 + bi) % len(palette)]
            h = 6 + (bi % 3)
            shade_rect(img, x, y + 8 - h, x + 5, y + 8, c, WHITE, OUTLINE)
            x += 6
    # ladder
    shade_rect(img, 36, 12, 40, 56, WOOD_H, CREAM, WOOD_D)
    for y in range(16, 52, 8):
        hline(img, 36, 40, y, WOOD)
    return img


def prop_round_window():
    img = new(48, 48)
    # wall
    shade_rect(img, 2, 2, 46, 46, BRICK, BRICK, BRICK_D)
    oval(img, 24, 24, 18, 18, WOOD)
    oval(img, 24, 24, 15, 15, GLASS)
    # cross
    hline(img, 10, 38, 24, WOOD_H)
    vline(img, 24, 10, 38, WOOD_H)
    # warm interior
    oval(img, 24, 26, 8, 6, WARM)
    return img


def prop_patio_umbrella():
    img = new(48, 64)
    # pole
    shade_rect(img, 22, 20, 26, 58, GRAY_H, WHITE, GRAY)
    # table
    shade_rect(img, 10, 48, 38, 54, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 16, 54, 20, 62, WOOD_D, WOOD, BLACK)
    shade_rect(img, 28, 54, 32, 62, WOOD_D, WOOD, BLACK)
    # umbrella canopy
    for i in range(6):
        c = PINK if i % 2 == 0 else WHITE
        x0 = 6 + i * 6
        shade_rect(img, x0, 10, x0 + 7, 22, c, WHITE if c == PINK else CREAM, GRAY)
    oval(img, 24, 12, 18, 6, PINK_H)
    # drinks
    shade_rect(img, 14, 42, 18, 48, MINT, MINT_H, GRAY)
    shade_rect(img, 30, 42, 34, 48, PINK, PINK_H, GRAY)
    return img


def prop_night_window():
    img = new(48, 64)
    # night wall
    shade_rect(img, 4, 4, 44, 60, NIGHT, (40, 48, 78, 255), BLACK)
    # window
    shade_rect(img, 10, 12, 38, 44, WARM, (255, 230, 170, 255), GOLD)
    # panes
    hline(img, 10, 37, 28, WOOD_D)
    vline(img, 24, 12, 43, WOOD_D)
    # sill plants
    oval(img, 16, 46, 4, 3, LEAF)
    oval(img, 32, 46, 4, 3, LEAF_H)
    # glow spill
    rect(img, 8, 44, 40, 48, (255, 200, 120, 60))
    return img


def prop_dessert_cart():
    img = new(64, 48)
    # cart body
    shade_rect(img, 8, 18, 52, 36, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 10, 12, 50, 20, WHITE, CREAM, GRAY)
    # wheels
    oval(img, 16, 40, 6, 6, BLACK)
    oval(img, 16, 40, 3, 3, GRAY_H)
    oval(img, 44, 40, 6, 6, BLACK)
    oval(img, 44, 40, 3, 3, GRAY_H)
    # canopy
    for i in range(6):
        c = NEON_P if i % 2 == 0 else WHITE
        shade_rect(img, 10 + i * 7, 4, 17 + i * 7, 14, c, WHITE, GRAY)
    # desserts
    oval(img, 20, 16, 4, 3, PINK)
    oval(img, 30, 15, 4, 3, MINT)
    oval(img, 40, 16, 4, 3, GOLD)
    # neon price tag
    shade_rect(img, 48, 20, 58, 30, NEON_Y, WHITE, OUTLINE)
    return img


def prop_closed_shutter():
    img = new(48, 64)
    shade_rect(img, 6, 6, 42, 58, GRAY, GRAY_H, BLACK)
    for y in range(10, 56, 4):
        hline(img, 8, 39, y, GRAY_H if (y // 4) % 2 == 0 else GRAY)
    # stickers
    shade_rect(img, 12, 18, 22, 26, PINK, PINK_H, OUTLINE)
    shade_rect(img, 26, 30, 36, 38, NEON_C, WHITE, OUTLINE)
    shade_rect(img, 14, 40, 24, 48, NEON_Y, WHITE, OUTLINE)
    # lock
    shade_rect(img, 22, 50, 28, 56, BLACK, GRAY, BLACK)
    return img


def prop_piano_corner():
    img = new(64, 48)
    # piano body
    shade_rect(img, 8, 20, 48, 38, BLACK, GRAY, BLACK)
    shade_rect(img, 10, 16, 46, 22, BLACK, GRAY_H, BLACK)
    # keys
    for i in range(14):
        shade_rect(img, 12 + i * 2, 18, 14 + i * 2, 26, WHITE, CREAM, GRAY)
    for i in (1, 2, 4, 5, 6, 8, 9, 11, 12):
        shade_rect(img, 13 + i * 2, 18, 15 + i * 2, 23, BLACK, GRAY, BLACK)
    # stool
    shade_rect(img, 50, 28, 60, 34, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 52, 34, 56, 42, WOOD_D, WOOD, BLACK)
    # lamp
    shade_rect(img, 4, 10, 10, 28, GOLD, NEON_Y, WOOD_D)
    oval(img, 7, 8, 5, 3, WARM)
    return img


def prop_photo_spot_mark():
    img = new(48, 32)
    # floor plate
    oval(img, 24, 22, 18, 6, GRAY_H)
    oval(img, 24, 22, 16, 5, CREAM)
    # camera icon
    shade_rect(img, 16, 8, 32, 20, PINK, PINK_H, OUTLINE)
    oval(img, 24, 14, 4, 4, WHITE)
    oval(img, 24, 14, 2, 2, BLACK)
    shade_rect(img, 20, 6, 28, 9, PINK_H, WHITE, PINK)
    return img


def prop_parfait_tower():
    img = new(32, 64)
    # glass
    shade_rect(img, 8, 16, 24, 52, GLASS, WHITE, GRAY)
    layers = [PINK, CREAM, MINT, COFFEE_H, PINK_H, WHITE]
    y = 48
    for c in layers:
        rect(img, 10, y - 5, 22, y, c)
        y -= 5
    # whipped cream + cherry
    oval(img, 16, 16, 7, 4, FOAM)
    oval(img, 16, 12, 3, 3, PINK)
    # stem
    vline(img, 16, 8, 11, LEAF)
    # base
    shade_rect(img, 10, 52, 22, 56, WOOD, WOOD_H, WOOD_D)
    return img


def prop_string_lights():
    img = new(64, 32)
    hline(img, 2, 61, 8, GRAY)
    colors = [NEON_Y, PINK, NEON_C, NEON_Y, MINT, PINK, GOLD, NEON_C]
    for i, c in enumerate(colors):
        x = 6 + i * 7
        y = 10 + (2 if i % 2 == 0 else 6)
        vline(img, x, 8, y, GRAY)
        oval(img, x, y + 3, 3, 4, c)
        px(img, x, y + 1, WHITE)
    return img


def prop_brick_alley_door():
    img = new(48, 64)
    shade_rect(img, 2, 2, 46, 62, BRICK, BRICK, BRICK_D)
    for y in range(6, 60, 6):
        hline(img, 2, 45, y, BRICK_D)
        off = 4 if (y // 6) % 2 == 0 else 0
        for x in range(4 + off, 44, 8):
            vline(img, x, y, min(y + 6, 61), BRICK_D)
    # door
    shade_rect(img, 14, 22, 34, 58, WOOD_D, WOOD, BLACK)
    shade_rect(img, 16, 24, 32, 40, WOOD, WOOD_H, WOOD_D)
    # neon open
    shade_rect(img, 16, 10, 32, 18, NEON_P, WHITE, OUTLINE)
    px(img, 30, 38, GOLD)  # knob
    return img


def prop_checker_spot():
    img = new(48, 32)
    for y in range(4):
        for x in range(6):
            c = BLACK if (x + y) % 2 == 0 else WHITE
            rect(img, 6 + x * 6, 6 + y * 5, 12 + x * 6, 11 + y * 5, c)
    # heart sticker
    oval(img, 24, 14, 4, 3, PINK)
    px(img, 22, 12, PINK)
    px(img, 26, 12, PINK)
    return img


PROPS = [
    ("Act2_Prop_LatteTable.png", prop_latte_table),
    ("Act2_Prop_MacaronSculpture.png", prop_macaron_sculpture),
    ("Act2_Prop_DonutWall.png", prop_donut_wall),
    ("Act2_Prop_CafeSign.png", prop_cafe_sign),
    ("Act2_Prop_Terrace.png", prop_terrace),
    ("Act2_Prop_MirrorWall.png", prop_mirror_wall),
    ("Act2_Prop_Greenhouse.png", prop_greenhouse),
    ("Act2_Prop_BookWall.png", prop_book_wall),
    ("Act2_Prop_RoundWindow.png", prop_round_window),
    ("Act2_Prop_PatioUmbrella.png", prop_patio_umbrella),
    ("Act2_Prop_NightWindow.png", prop_night_window),
    ("Act2_Prop_DessertCart.png", prop_dessert_cart),
    ("Act2_Prop_ClosedShutter.png", prop_closed_shutter),
    ("Act2_Prop_PianoCorner.png", prop_piano_corner),
    ("Act2_Prop_PhotoSpotMark.png", prop_photo_spot_mark),
    ("Act2_Prop_ParfaitTower.png", prop_parfait_tower),
    ("Act2_Prop_StringLights.png", prop_string_lights),
    ("Act2_Prop_BrickAlleyDoor.png", prop_brick_alley_door),
    ("Act2_Prop_CheckerSpot.png", prop_checker_spot),
]

if __name__ == "__main__":
    for name, fn in PROPS:
        save(fn(), name)
    print(f"done: {len(PROPS)} props")
