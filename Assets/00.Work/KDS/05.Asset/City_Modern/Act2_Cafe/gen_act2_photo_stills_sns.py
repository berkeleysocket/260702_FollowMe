"""Act2 SNS photo stills — 192x192, Stage2(gen_photo_stills_sns) 동일 퀄리티.
여름 성수/카페거리 「달콤함」 분위기.
"""
from PIL import Image
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act2_Cafe"
os.makedirs(OUT, exist_ok=True)

S = 192
CLR = (0, 0, 0, 0)
OUTL = (18, 14, 16, 255)

SKIN = (236, 204, 176, 255)
SKIN_S = (210, 168, 140, 255)
HAIR_A = (52, 38, 34, 255)
HAIR_B = (98, 52, 44, 255)
HAIR_C = (40, 44, 58, 255)
SHIRT_A = (86, 148, 220, 255)
SHIRT_AH = (130, 178, 235, 255)
SHIRT_B = (228, 92, 128, 255)
SHIRT_BH = (245, 140, 168, 255)
SHIRT_C = (64, 176, 132, 255)
SHIRT_CH = (110, 205, 165, 255)
PANTS = (58, 54, 68, 255)
SHOE = (36, 32, 34, 255)

WOOD = (148, 104, 64, 255)
WOOD_H = (178, 132, 88, 255)
WOOD_D = (110, 72, 42, 255)
CREAM = (255, 248, 240, 255)
CREAM_D = (230, 214, 196, 255)
WHITE = (246, 246, 250, 255)
PINK = (255, 140, 178, 255)
PINK_H = (255, 190, 210, 255)
PINK_UI = (255, 120, 170, 255)
MINT = (120, 210, 180, 255)
MINT_H = (170, 235, 210, 255)
COFFEE = (110, 70, 42, 255)
COFFEE_H = (150, 105, 70, 255)
FOAM = (250, 246, 236, 255)
NEON_P = (255, 96, 205, 255)
NEON_C = (90, 230, 255, 255)
NEON_Y = (255, 232, 96, 255)
NEON_G = (120, 255, 160, 255)
BOARD = (28, 26, 38, 255)
BOARD_H = (48, 46, 64, 255)
METAL = (150, 156, 168, 255)
METAL_H = (200, 206, 218, 255)
METAL_D = (90, 94, 104, 255)
LEAF = (70, 150, 90, 255)
LEAF_H = (110, 190, 120, 255)
LEAF_D = (48, 110, 70, 255)
BRICK = (170, 90, 78, 255)
BRICK_H = (198, 120, 100, 255)
BRICK_D = (130, 60, 52, 255)
GOLD = (230, 180, 70, 255)
WARM = (255, 210, 130, 255)
WARM_H = (255, 230, 180, 255)
NIGHT = (28, 34, 58, 255)
NIGHT_H = (48, 56, 88, 255)
CITY = (70, 82, 104, 255)
CITY_D = (48, 56, 74, 255)
WIN = (255, 220, 120, 255)
GRAY = (120, 118, 128, 255)
SUMMER_TOP = (255, 220, 190, 255)
SUMMER_BOT = (255, 180, 200, 255)
SOFT_MINT = (220, 245, 235, 255)


def new():
    return Image.new("RGBA", (S, S), CLR)


def px(img, x, y, c):
    if 0 <= x < S and 0 <= y < S and c[3] > 0:
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
            if rx and ry and ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 <= 1.05:
                px(img, x, y, c)


def shade_rect(img, x0, y0, x1, y1, base, hi=None, lo=None):
    rect(img, x0, y0, x1, y1, base)
    if hi:
        hline(img, x0, x1 - 1, y0, hi)
        vline(img, x0, y0, y1 - 1, hi)
    if lo:
        hline(img, x0, x1 - 1, y1 - 1, lo)
        vline(img, x1 - 1, y0, y1 - 1, lo)


def add_outline(img, color=OUTL, solid_alpha=200):
    w, h = img.size
    src = img.copy()
    out = Image.new("RGBA", (w, h), CLR)
    solid = [[src.getpixel((x, y))[3] >= solid_alpha for x in range(w)] for y in range(h)]
    for y in range(h):
        for x in range(w):
            if solid[y][x]:
                continue
            edge = any(
                0 <= x + dx < w and 0 <= y + dy < h and solid[y + dy][x + dx]
                for dy in (-1, 0, 1) for dx in (-1, 0, 1) if not (dx == 0 and dy == 0)
            )
            if edge:
                out.putpixel((x, y), color)
    out.paste(src, (0, 0), src)
    return out


def fill_gradient(img, top, bot):
    for y in range(S):
        t = y / (S - 1)
        c = tuple(int(top[i] * (1 - t) + bot[i] * t) for i in range(4))
        hline(img, 0, S - 1, y, c)


def draw_heart(img, x, y, c=PINK_UI, s=1):
    pts = [
        (1, 0), (2, 0), (4, 0), (5, 0),
        (0, 1), (1, 1), (2, 1), (3, 1), (4, 1), (5, 1), (6, 1),
        (0, 2), (1, 2), (2, 2), (3, 2), (4, 2), (5, 2), (6, 2),
        (1, 3), (2, 3), (3, 3), (4, 3), (5, 3),
        (2, 4), (3, 4), (4, 4),
        (3, 5),
    ]
    for dx, dy in pts:
        px(img, x + dx * s, y + dy * s, c)


def draw_star(img, x, y, c=NEON_Y):
    for dx, dy in [
        (0, -2), (0, -1), (0, 0), (0, 1), (0, 2),
        (-2, 0), (-1, 0), (1, 0), (2, 0),
        (-1, -1), (1, -1), (-1, 1), (1, 1),
    ]:
        px(img, x + dx, y + dy, c)


def sns_chrome(img):
    rect(img, 0, 0, S, 10, (0, 0, 0, 50))
    draw_heart(img, 12, 14, PINK_UI)
    draw_heart(img, S - 28, 18, NEON_P)
    draw_star(img, 24, S - 24, NEON_Y)
    draw_star(img, S - 22, S - 30, NEON_C)
    for x, y in [(40, 20), (150, 28), (60, 170), (170, 160), (96, 16)]:
        px(img, x, y, WHITE)
        px(img, x + 1, y, WHITE)


def draw_person(img, ox, oy, shirt, shirt_hi, hair, scale=2, pose="none"):
    def R(x0, y0, x1, y1, c):
        rect(img, ox + x0 * scale, oy + y0 * scale, ox + x1 * scale, oy + y1 * scale, c)

    rect(img, ox + 2 * scale, oy - scale, ox + 12 * scale, oy + scale, (0, 0, 0, 70))
    R(2, -2, 6, 0, SHOE)
    R(7, -2, 11, 0, SHOE)
    R(3, -9, 6, -2, PANTS)
    R(7, -9, 10, -2, PANTS)
    shade_rect(img, ox + 2 * scale, oy - 17 * scale, ox + 11 * scale, oy - 9 * scale, shirt, shirt_hi, PANTS)
    oval(img, ox + 6 * scale, oy - 22 * scale, 4 * scale, 4 * scale, SKIN)
    px(img, ox + 5 * scale, oy - 22 * scale, OUTL)
    px(img, ox + 8 * scale, oy - 22 * scale, OUTL)
    rect(img, ox + 2 * scale, oy - 27 * scale, ox + 11 * scale, oy - 21 * scale, hair)

    if pose == "phone":
        shade_rect(img, ox + 12 * scale, oy - 20 * scale, ox + 18 * scale, oy - 12 * scale, BOARD, BOARD_H, OUTL)
        rect(img, ox + 13 * scale, oy - 19 * scale, ox + 17 * scale, oy - 13 * scale, NEON_C)
        rect(img, ox + 10 * scale, oy - 16 * scale, ox + 13 * scale, oy - 14 * scale, SKIN)
    elif pose == "camera":
        shade_rect(img, ox + 12 * scale, oy - 16 * scale, ox + 20 * scale, oy - 10 * scale, BOARD, BOARD_H, OUTL)
        oval(img, ox + 16 * scale, oy - 13 * scale, 3 * scale, 3 * scale, METAL)
        oval(img, ox + 16 * scale, oy - 13 * scale, scale, scale, NEON_P)
    elif pose == "cup":
        shade_rect(img, ox + 12 * scale, oy - 14 * scale, ox + 18 * scale, oy - 8 * scale, WHITE, CREAM, GRAY)
        rect(img, ox + 13 * scale, oy - 13 * scale, ox + 17 * scale, oy - 9 * scale, COFFEE)


def draw_macaron(img, cx, cy, rx, ry, shell, shell_h):
    oval(img, cx, cy - 2, rx, max(2, ry - 1), shell_h)
    oval(img, cx, cy, rx, ry, shell)
    oval(img, cx, cy + 2, rx - 2, max(1, ry - 2), CREAM)
    oval(img, cx, cy + 4, rx - 1, max(1, ry - 2), shell)


def draw_donut(img, cx, cy, shell, icing):
    oval(img, cx, cy, 14, 11, shell)
    oval(img, cx, cy - 2, 11, 7, icing)
    oval(img, cx, cy, 5, 4, CREAM)
    oval(img, cx, cy, 3, 2, CREAM_D)
    for dx in range(-8, 9, 3):
        px(img, cx + dx, cy - 4, WHITE)


def finish(img):
    img = add_outline(img)
    sns_chrome(img)
    return img


# ---------- 10 Act2 stills ----------

def still_latte():
    img = new()
    fill_gradient(img, SUMMER_TOP, (255, 230, 210, 255))
    # cafe brick + window
    shade_rect(img, 8, 28, 110, 150, BRICK, BRICK_H, BRICK_D)
    for y in (50, 72, 94, 116):
        hline(img, 8, 109, y, BRICK_D)
    shade_rect(img, 22, 40, 96, 120, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 28, 46, 90, 114, (180, 220, 245, 220), WHITE, METAL_D)
    hline(img, 28, 89, 80, WOOD_H)
    vline(img, 59, 46, 113, WOOD_H)
    oval(img, 72, 62, 14, 12, WARM)
    oval(img, 72, 62, 6, 5, WARM_H)
    shade_rect(img, 28, 46, 36, 114, PINK_H, WHITE, PINK)
    shade_rect(img, 82, 46, 90, 114, PINK_H, WHITE, PINK)
    # table + latte hero
    shade_rect(img, 100, 118, 180, 148, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 112, 148, 124, 176, WOOD_D, WOOD, OUTL)
    shade_rect(img, 156, 148, 168, 176, WOOD_D, WOOD, OUTL)
    oval(img, 140, 122, 28, 8, (0, 0, 0, 50))
    shade_rect(img, 118, 78, 162, 118, WHITE, CREAM, GRAY)
    rect(img, 124, 84, 156, 110, COFFEE)
    oval(img, 140, 84, 14, 8, FOAM)
    # heart latte art
    draw_heart(img, 132, 90, FOAM, 2)
    shade_rect(img, 162, 90, 174, 110, WHITE, CREAM, GRAY)
    rect(img, 164, 94, 172, 106, CLR)
    oval(img, 140, 116, 20, 6, CREAM_D)
    # steam
    for x, y in [(132, 68), (140, 60), (148, 66), (136, 54)]:
        px(img, x, y, WHITE)
        px(img, x + 1, y - 1, WHITE)
    # pastry
    oval(img, 108, 108, 10, 6, GOLD)
    oval(img, 108, 106, 7, 4, NEON_Y)
    draw_person(img, 40, 168, SHIRT_B, SHIRT_BH, HAIR_B, 2, "phone")
    rect(img, 0, 176, S, S, (255, 255, 255, 210))
    for i, c in enumerate([PINK_UI, NEON_C, GOLD]):
        draw_heart(img, 14 + i * 18, 180, c)
    return finish(img)


def still_macaron():
    img = new()
    fill_gradient(img, SOFT_MINT, SUMMER_BOT)
    oval(img, 96, 150, 70, 24, (255, 255, 255, 80))
    # pedestal
    shade_rect(img, 56, 140, 136, 158, METAL_H, WHITE, METAL)
    shade_rect(img, 64, 158, 128, 172, METAL, METAL_H, METAL_D)
    shade_rect(img, 72, 172, 120, 180, METAL_D, METAL, OUTL)
    stack = [
        (96, 132, 34, 12, PINK, PINK_H),
        (96, 112, 32, 11, MINT, MINT_H),
        (96, 94, 30, 10, NEON_Y, WHITE),
        (96, 78, 28, 10, NEON_C, WHITE),
        (96, 64, 26, 9, PINK_H, WHITE),
        (96, 52, 22, 8, GOLD, NEON_Y),
    ]
    for cx, cy, rx, ry, a, b in stack:
        draw_macaron(img, cx, cy, rx, ry, a, b)
    # cherry + ribbon
    oval(img, 96, 40, 8, 7, NEON_P)
    px(img, 96, 34, LEAF_H)
    shade_rect(img, 86, 44, 106, 50, GOLD, NEON_Y, WOOD_D)
    for x, y in [(40, 50), (150, 60), (48, 90), (148, 100), (70, 36), (130, 42)]:
        draw_star(img, x, y, WHITE if (x + y) % 2 == 0 else GOLD)
    draw_person(img, 18, 172, SHIRT_A, SHIRT_AH, HAIR_A, 2, "camera")
    draw_person(img, 148, 172, SHIRT_B, SHIRT_BH, HAIR_B, 2, "phone")
    rect(img, 0, 176, S, S, (255, 255, 255, 210))
    draw_heart(img, 16, 180, PINK_UI)
    draw_heart(img, 36, 180, MINT)
    return finish(img)


def still_donut():
    img = new()
    fill_gradient(img, (255, 236, 220, 255), (255, 200, 210, 255))
    # pegboard wall
    shade_rect(img, 12, 20, 180, 130, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 18, 26, 174, 122, CREAM_D, CREAM, WOOD_D)
    for y in range(34, 118, 10):
        for x in range(28, 168, 10):
            px(img, x, y, WOOD_D)
    donuts = [
        (48, 50, PINK, PINK_H), (96, 44, MINT, MINT_H), (144, 52, NEON_Y, WHITE),
        (48, 90, COFFEE_H, COFFEE), (96, 86, NEON_C, WHITE), (144, 92, GOLD, NEON_Y),
        (72, 68, PINK_H, WHITE), (120, 70, WHITE, CREAM),
    ]
    for cx, cy, a, b in donuts:
        draw_donut(img, cx, cy, a, b)
    shade_rect(img, 12, 124, 180, 138, WOOD_D, WOOD, OUTL)
    # sign
    shade_rect(img, 60, 8, 132, 24, PINK, PINK_H, OUTL)
    for x in (70, 84, 98, 112):
        px(img, x, 14, WHITE)
        px(img, x + 1, 15, WHITE)
    draw_person(img, 70, 170, SHIRT_B, SHIRT_BH, HAIR_B, 2, "phone")
    draw_person(img, 24, 170, SHIRT_C, SHIRT_CH, HAIR_C, 2, "none")
    draw_person(img, 140, 170, SHIRT_A, SHIRT_AH, HAIR_A, 2, "camera")
    rect(img, 0, 176, S, S, (255, 255, 255, 210))
    for i, c in enumerate([PINK_UI, NEON_Y, NEON_C]):
        draw_heart(img, 14 + i * 18, 180, c)
    return finish(img)


def still_neon_cafe():
    img = new()
    fill_gradient(img, (40, 24, 60, 255), (90, 40, 70, 255))
    # brick facade
    shade_rect(img, 8, 50, 184, 150, BRICK, BRICK_H, BRICK_D)
    for y in (70, 90, 110, 130):
        hline(img, 8, 183, y, BRICK_D)
    # awning
    for i in range(16):
        c = PINK if i % 2 == 0 else WHITE
        shade_rect(img, 10 + i * 11, 28, 22 + i * 11, 52, c, WHITE if c == PINK else CREAM, GRAY)
    # neon CAFE
    shade_rect(img, 36, 60, 156, 118, BOARD_H, METAL_H, OUTL)
    shade_rect(img, 42, 66, 150, 112, BOARD, BOARD_H, OUTL)
    rect(img, 48, 72, 144, 106, (12, 10, 22, 255))
    letters = [("C", 54, NEON_P), ("A", 78, NEON_C), ("F", 102, NEON_Y), ("E", 126, NEON_G)]
    for ch, x, col in letters:
        shade_rect(img, x, 76, x + 20, 102, col, WHITE, OUTL)
        if ch == "C":
            rect(img, x + 6, 82, x + 20, 96, BOARD)
        elif ch == "A":
            rect(img, x + 6, 82, x + 14, 90, BOARD)
            rect(img, x + 6, 94, x + 14, 102, BOARD)
        elif ch == "F":
            rect(img, x + 6, 82, x + 18, 88, BOARD)
            rect(img, x + 6, 90, x + 16, 96, BOARD)
            rect(img, x + 6, 98, x + 20, 102, BOARD)
        else:
            rect(img, x + 6, 82, x + 18, 88, BOARD)
            rect(img, x + 6, 90, x + 16, 96, BOARD)
            rect(img, x + 6, 98, x + 18, 102, BOARD)
    for x in range(44, 148):
        px(img, x, 58, (NEON_P[0], NEON_P[1], NEON_P[2], 90))
        px(img, x, 116, (NEON_C[0], NEON_C[1], NEON_C[2], 70))
    shade_rect(img, 82, 118, 110, 150, WOOD_D, WOOD, OUTL)
    px(img, 104, 134, GOLD)
    draw_person(img, 24, 168, SHIRT_A, SHIRT_AH, HAIR_A, 2, "phone")
    draw_person(img, 130, 168, SHIRT_B, SHIRT_BH, HAIR_B, 2, "phone")
    # phone selfie chrome
    shade_rect(img, 14, 148, 44, 178, BOARD, BOARD_H, OUTL)
    rect(img, 18, 152, 40, 170, NEON_P)
    draw_heart(img, 24, 156, WHITE)
    return finish(img)


def still_terrace():
    img = new()
    fill_gradient(img, (180, 220, 255, 255), SUMMER_TOP)
    # city
    for i, h in enumerate([40, 62, 48, 78, 54, 70, 44, 66, 52]):
        x0 = 4 + i * 21
        col = CITY if i % 2 == 0 else CITY_D
        shade_rect(img, x0, 100 - h, x0 + 18, 100, col, METAL, OUTL)
        if i % 2 == 0:
            px(img, x0 + 5, 100 - h + 12, WIN)
            px(img, x0 + 11, 100 - h + 24, NEON_C)
    # railing + deck
    shade_rect(img, 0, 108, S, 118, METAL, METAL_H, METAL_D)
    for x in range(6, S, 12):
        shade_rect(img, x, 118, x + 4, 148, METAL, METAL_H, METAL_D)
    shade_rect(img, 0, 148, S, 176, WOOD, WOOD_H, WOOD_D)
    # planter flowers
    shade_rect(img, 50, 120, 142, 142, WOOD, WOOD_H, WOOD_D)
    for dx, dy, c in [
        (0, 0, PINK), (12, -6, MINT), (-12, 4, PINK_H), (20, 6, NEON_Y),
        (-18, -4, MINT_H), (8, 10, PINK), (-8, 8, LEAF), (26, -2, PINK_H),
    ]:
        oval(img, 96 + dx, 116 + dy, 8, 6, c)
        px(img, 96 + dx, 112 + dy, WHITE if c != LEAF else LEAF_H)
    # string lights
    hline(img, 20, 172, 28, METAL_D)
    for i, x in enumerate(range(28, 170, 16)):
        y = 34 + (4 if i % 2 == 0 else 10)
        vline(img, x, 28, y, METAL_D)
        oval(img, x, y + 4, 4, 5, NEON_Y if i % 2 == 0 else NEON_P)
    draw_person(img, 78, 170, SHIRT_C, SHIRT_CH, HAIR_C, 2, "phone")
    draw_heart(img, 50, 40, PINK_UI)
    draw_star(img, 140, 48, NEON_Y)
    rect(img, 0, 176, S, S, (255, 255, 255, 210))
    draw_heart(img, 14, 180, PINK_UI)
    draw_heart(img, 34, 180, NEON_C)
    return finish(img)


def still_mirror():
    img = new()
    fill_gradient(img, (255, 230, 240, 255), (230, 210, 255, 255))
    shade_rect(img, 20, 16, 172, 168, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 28, 24, 164, 160, GOLD, NEON_Y, WOOD_D)
    shade_rect(img, 36, 32, 156, 152, (200, 220, 240, 230), WHITE, METAL)
    # reflection person
    oval(img, 96, 70, 16, 16, SKIN)
    px(img, 90, 68, OUTL)
    px(img, 102, 68, OUTL)
    rect(img, 84, 52, 108, 64, HAIR_B)
    shade_rect(img, 78, 86, 114, 130, SHIRT_B, SHIRT_BH, PANTS)
    shade_rect(img, 82, 130, 94, 150, PANTS, None, None)
    shade_rect(img, 98, 130, 110, 150, PANTS, None, None)
    # vanity lights
    for y in (40, 70, 100, 130):
        oval(img, 30, y, 5, 5, WARM_H)
        oval(img, 162, y, 5, 5, WARM_H)
    draw_person(img, 70, 172, SHIRT_B, SHIRT_BH, HAIR_B, 2, "phone")
    draw_heart(img, 48, 36, PINK_UI)
    draw_star(img, 140, 40, NEON_Y)
    return finish(img)


def still_plant():
    img = new()
    fill_gradient(img, (200, 245, 220, 255), (255, 240, 220, 255))
    # greenhouse frame
    shade_rect(img, 16, 16, 176, 168, METAL_H, WHITE, METAL)
    shade_rect(img, 24, 24, 168, 160, (180, 230, 220, 180), WHITE, METAL_D)
    vline(img, 96, 24, 159, METAL_H)
    hline(img, 24, 167, 92, METAL_H)
    # big pot + foliage
    shade_rect(img, 64, 130, 128, 158, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 72, 158, 120, 172, WOOD_D, WOOD, OUTL)
    rect(img, 70, 124, 122, 132, COFFEE)
    oval(img, 96, 100, 40, 28, LEAF)
    oval(img, 96, 92, 28, 20, LEAF_H)
    oval(img, 70, 88, 18, 22, LEAF_D)
    oval(img, 122, 84, 18, 20, LEAF)
    oval(img, 96, 64, 16, 18, MINT)
    oval(img, 78, 70, 6, 6, PINK)
    oval(img, 114, 62, 5, 5, NEON_Y)
    for y in range(30, 120, 6):
        px(img, 32, y, LEAF)
        px(img, 33, y + 1, LEAF_H)
        px(img, 160, y + 2, LEAF)
        px(img, 159, y, LEAF_D)
    draw_person(img, 28, 170, SHIRT_C, SHIRT_CH, HAIR_C, 2, "phone")
    rect(img, 0, 176, S, S, (255, 255, 255, 210))
    draw_heart(img, 14, 180, LEAF_H)
    draw_heart(img, 34, 180, PINK_UI)
    return finish(img)


def still_night():
    img = new()
    fill_gradient(img, NIGHT, (50, 30, 60, 255))
    shade_rect(img, 20, 20, 172, 160, NIGHT_H, (60, 70, 100, 255), OUTL)
    for y in (40, 70, 100, 130):
        hline(img, 20, 171, y, (36, 42, 68, 255))
    # warm window
    shade_rect(img, 40, 36, 152, 110, WOOD_D, WOOD, OUTL)
    shade_rect(img, 48, 44, 144, 102, WARM_H, (255, 240, 190, 255), GOLD)
    hline(img, 48, 143, 73, WOOD_D)
    vline(img, 96, 44, 101, WOOD_D)
    # silhouettes inside
    oval(img, 72, 64, 8, 8, (80, 50, 40, 200))
    shade_rect(img, 64, 72, 80, 96, (70, 40, 35, 180))
    oval(img, 120, 66, 8, 8, (80, 50, 40, 200))
    shade_rect(img, 112, 74, 128, 96, (70, 40, 35, 180))
    # neon strip
    shade_rect(img, 56, 22, 136, 34, NEON_P, WHITE, OUTL)
    for x in range(64, 128, 10):
        px(img, x, 26, WHITE)
    # sill plants
    shade_rect(img, 44, 110, 148, 122, WOOD, WOOD_H, WOOD_D)
    oval(img, 64, 108, 10, 8, LEAF)
    oval(img, 96, 106, 8, 8, LEAF_H)
    oval(img, 128, 108, 10, 8, LEAF)
    # glow spill
    rect(img, 36, 110, 156, 140, (255, 190, 110, 40))
    draw_person(img, 74, 170, SHIRT_B, SHIRT_BH, HAIR_B, 2, "camera")
    for x in range(48, 144):
        px(img, x, 36, (255, 210, 140, 50))
    return finish(img)


def still_round():
    img = new()
    fill_gradient(img, SUMMER_TOP, (240, 210, 200, 255))
    shade_rect(img, 0, 0, S, S, BRICK, BRICK_H, BRICK_D)
    for y in range(8, S, 14):
        hline(img, 0, S - 1, y, BRICK_D)
    oval(img, 96, 88, 58, 58, WOOD)
    oval(img, 96, 88, 50, 50, WOOD_H)
    oval(img, 96, 88, 44, 44, (180, 220, 245, 230))
    hline(img, 52, 140, 88, WOOD)
    vline(img, 96, 44, 132, WOOD)
    oval(img, 96, 96, 18, 14, (255, 200, 140, 160))
    # silhouette
    oval(img, 96, 74, 12, 12, (70, 50, 60, 230))
    shade_rect(img, 86, 86, 106, 116, (60, 40, 55, 210))
    # ivy
    for dx, dy in [(-48, 10), (-42, 30), (44, 16), (48, 36), (-30, -40), (32, -36), (-50, -10), (50, -8)]:
        oval(img, 96 + dx, 88 + dy, 8, 6, LEAF if (dx + dy) % 2 == 0 else LEAF_H)
    draw_person(img, 20, 172, SHIRT_A, SHIRT_AH, HAIR_A, 2, "phone")
    draw_heart(img, 150, 40, PINK_UI)
    draw_star(img, 40, 36, NEON_Y)
    return finish(img)


def still_dessert_cart():
    img = new()
    fill_gradient(img, NIGHT_H, (70, 40, 55, 255))
    # cart
    shade_rect(img, 28, 80, 150, 130, WOOD, WOOD_H, WOOD_D)
    shade_rect(img, 34, 58, 144, 86, WHITE, CREAM, GRAY)
    shade_rect(img, 40, 42, 138, 78, (180, 220, 240, 180), WHITE, METAL)
    desserts = [
        (56, 60, PINK, PINK_H), (80, 56, MINT, MINT_H), (104, 60, GOLD, NEON_Y),
        (128, 58, NEON_C, WHITE), (68, 70, PINK_H, WHITE), (112, 70, NEON_Y, WHITE),
    ]
    for cx, cy, a, b in desserts:
        oval(img, cx, cy, 10, 8, a)
        oval(img, cx, cy - 2, 7, 5, b)
        px(img, cx, cy - 4, WHITE)
    # canopy
    for i in range(10):
        c = NEON_P if i % 2 == 0 else WHITE
        shade_rect(img, 30 + i * 12, 18, 44 + i * 12, 44, c, WHITE, GRAY)
    # wheels
    oval(img, 50, 148, 16, 16, BOARD)
    oval(img, 50, 148, 8, 8, METAL_H)
    oval(img, 128, 148, 16, 16, BOARD)
    oval(img, 128, 148, 8, 8, METAL_H)
    # price neon
    shade_rect(img, 152, 70, 180, 100, NEON_Y, WHITE, OUTL)
    px(img, 160, 80, BOARD)
    px(img, 166, 86, BOARD)
    draw_star(img, 60, 30, WHITE)
    draw_star(img, 120, 26, GOLD)
    draw_person(img, 150, 170, SHIRT_B, SHIRT_BH, HAIR_B, 2, "phone")
    draw_person(img, 10, 170, SHIRT_C, SHIRT_CH, HAIR_C, 2, "none")
    rect(img, 0, 176, S, S, (255, 255, 255, 210))
    draw_heart(img, 14, 180, NEON_P)
    draw_heart(img, 34, 180, NEON_Y)
    return finish(img)


PATHS = {
    "Act2_PhotoStill_LatteArt.png": still_latte,
    "Act2_PhotoStill_Macaron.png": still_macaron,
    "Act2_PhotoStill_DonutWall.png": still_donut,
    "Act2_PhotoStill_NeonCafe.png": still_neon_cafe,
    "Act2_PhotoStill_Terrace.png": still_terrace,
    "Act2_PhotoStill_MirrorRoom.png": still_mirror,
    "Act2_PhotoStill_PlantCafe.png": still_plant,
    "Act2_PhotoStill_NightWindow.png": still_night,
    "Act2_PhotoStill_RoundWindow.png": still_round,
    "Act2_PhotoStill_DessertCart.png": still_dessert_cart,
}

if __name__ == "__main__":
    for name, fn in PATHS.items():
        img = fn()
        path = os.path.join(OUT, name)
        img.save(path)
        print("saved", name, img.size)
    print(f"done {len(PATHS)} stills")
