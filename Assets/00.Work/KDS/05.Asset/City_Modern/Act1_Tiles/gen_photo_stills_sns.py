"""Stage2 SNS polaroid stills — 192x192 Instagram-style pixel illustrations."""
from PIL import Image, ImageDraw
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act1_Tiles"
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

GUITAR = (196, 132, 68, 255)
GUITAR_H = (220, 168, 100, 255)
GUITAR_D = (128, 76, 38, 255)
MIC = (108, 110, 118, 255)
MIC_H = (180, 184, 192, 255)
DRUM = (168, 118, 72, 255)
DRUM_H = (200, 155, 105, 255)
WOOD = (148, 104, 64, 255)
WOOD_H = (178, 132, 88, 255)

NEON_P = (255, 96, 205, 255)
NEON_C = (90, 230, 255, 255)
NEON_Y = (255, 232, 96, 255)
NEON_G = (120, 255, 160, 255)
BOARD = (28, 26, 38, 255)
BOARD_H = (48, 46, 64, 255)
METAL = (150, 156, 168, 255)
METAL_H = (200, 206, 218, 255)
METAL_D = (90, 94, 104, 255)
CITY = (70, 82, 104, 255)
CITY_D = (48, 56, 74, 255)
WIN = (255, 220, 120, 255)
CHERRY = (255, 170, 196, 255)
CHERRY_H = (255, 210, 225, 255)
WHITE = (246, 246, 250, 255)
CREAM = (255, 248, 240, 255)
PINK_UI = (255, 120, 170, 255)
SOFT_BG = (255, 236, 244, 255)


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


def fill_gradient(img, top, bot):
    for y in range(S):
        t = y / (S - 1)
        c = tuple(int(top[i] * (1 - t) + bot[i] * t) for i in range(4))
        hline(img, 0, S - 1, y, c)


def draw_heart(img, x, y, c=PINK_UI, s=1):
    # tiny pixel heart
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
    for dx, dy in [(0, -2), (0, -1), (0, 0), (0, 1), (0, 2), (-2, 0), (-1, 0), (1, 0), (2, 0), (-1, -1), (1, -1), (-1, 1), (1, 1)]:
        px(img, x + dx, y + dy, c)


def draw_note(img, x, y, c):
    px(img, x, y, c)
    px(img, x + 1, y, c)
    px(img, x, y + 1, c)
    px(img, x + 2, y - 1, c)
    px(img, x + 2, y - 2, c)
    px(img, x + 3, y - 2, c)
    px(img, x + 3, y - 1, c)


def sns_chrome(img):
    """Phone-photo frame vibe: soft vignette corners + sticker hearts/stars."""
    # top status bar fake
    rect(img, 0, 0, S, 10, (0, 0, 0, 50))
    # corner stickers
    draw_heart(img, 12, 14, PINK_UI)
    draw_heart(img, S - 28, 18, NEON_P)
    draw_star(img, 24, S - 24, NEON_Y)
    draw_star(img, S - 22, S - 30, NEON_C)
    # sparkles
    for x, y in [(40, 20), (150, 28), (60, 170), (170, 160), (96, 16)]:
        px(img, x, y, WHITE)
        px(img, x + 1, y, WHITE)


def draw_person(img, ox, oy, shirt, shirt_hi, hair, scale=2, instrument="none"):
    """Scaled-up person for SNS shot (feet at oy)."""
    def R(x0, y0, x1, y1, c):
        rect(img, ox + x0 * scale, oy + y0 * scale, ox + x1 * scale, oy + y1 * scale, c)

    # shadow
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

    if instrument == "guitar":
        oval(img, ox + 16 * scale, oy - 11 * scale, 7 * scale, 5 * scale, GUITAR)
        oval(img, ox + 16 * scale, oy - 11 * scale, 4 * scale, 3 * scale, GUITAR_H)
        shade_rect(img, ox + 20 * scale, oy - 20 * scale, ox + 22 * scale, oy - 11 * scale, GUITAR_D, GUITAR, GUITAR_D)
        rect(img, ox + 9 * scale, oy - 15 * scale, ox + 18 * scale, oy - 13 * scale, SKIN)
    elif instrument == "mic":
        shade_rect(img, ox + 10 * scale, oy - 22 * scale, ox + 12 * scale, oy - 7 * scale, MIC, MIC_H, METAL_D)
        oval(img, ox + 11 * scale, oy - 23 * scale, 3 * scale, 2 * scale, MIC_H)
        rect(img, ox + 7 * scale, oy - 15 * scale, ox + 12 * scale, oy - 13 * scale, SKIN)
    elif instrument == "drum":
        shade_rect(img, ox - scale, oy - 12 * scale, ox + 14 * scale, oy - 4 * scale, DRUM, DRUM_H, GUITAR_D)
        shade_rect(img, ox, oy - 13 * scale, ox + 13 * scale, oy - 11 * scale, CREAM, WHITE, DRUM)


def photo_busking():
    img = new()
    fill_gradient(img, (255, 210, 230, 255), (255, 170, 200, 255))
    # stage glow circle
    oval(img, 96, 130, 70, 28, (255, 255, 255, 70))
    # amp
    shade_rect(img, 18, 110, 48, 165, BOARD, BOARD_H, OUTL)
    rect(img, 24, 118, 42, 150, (20, 18, 28, 255))
    for yy in range(124, 146, 4):
        hline(img, 26, 40, yy, (70, 80, 95, 255))
    px(img, 28, 120, NEON_C)
    px(img, 36, 120, NEON_P)

    draw_person(img, 48, 168, SHIRT_A, SHIRT_AH, HAIR_A, 2, "guitar")
    draw_person(img, 84, 168, SHIRT_B, SHIRT_BH, HAIR_B, 2, "mic")
    draw_person(img, 122, 168, SHIRT_C, SHIRT_CH, HAIR_C, 2, "drum")

    # tip jar
    shade_rect(img, 155, 145, 178, 168, WOOD, WOOD_H, GUITAR_D)
    oval(img, 166, 145, 10, 4, WOOD_H)
    px(img, 162, 142, NEON_Y)
    px(img, 168, 140, NEON_Y)

    draw_note(img, 70, 48, NEON_Y)
    draw_note(img, 100, 36, NEON_P)
    draw_note(img, 130, 52, NEON_C)
    draw_note(img, 88, 60, WHITE)

    # SNS caption strip look
    rect(img, 0, 176, S, S, (255, 255, 255, 210))
    for i, c in enumerate([PINK_UI, NEON_C, NEON_Y]):
        draw_heart(img, 14 + i * 18, 180, c)

    img = add_outline(img)
    sns_chrome(img)
    return img


def photo_neon():
    img = new()
    fill_gradient(img, (24, 18, 48, 255), (60, 30, 80, 255))
    # city back
    for i, h in enumerate([40, 70, 55, 90, 60, 80, 50, 75]):
        x0 = 8 + i * 22
        col = CITY if i % 2 == 0 else CITY_D
        shade_rect(img, x0, 150 - h, x0 + 18, 150, col, METAL, OUTL)
        if i % 2 == 0:
            px(img, x0 + 6, 150 - h + 10, WIN)
            px(img, x0 + 12, 150 - h + 22, NEON_C)

    # giant neon board
    shade_rect(img, 28, 36, 164, 118, BOARD_H, METAL_H, OUTL)
    shade_rect(img, 34, 42, 158, 112, BOARD, BOARD_H, OUTL)
    rect(img, 40, 48, 152, 106, (12, 10, 22, 255))

    # OPEN big letters
    letters = [
        ("O", 48, NEON_P),
        ("P", 74, NEON_C),
        ("E", 100, NEON_Y),
        ("N", 126, NEON_G),
    ]
    for ch, x, col in letters:
        shade_rect(img, x, 56, x + 22, 98, col, WHITE, OUTL)
        if ch == "O":
            rect(img, x + 6, 66, x + 16, 88, BOARD)
        elif ch == "P":
            rect(img, x + 6, 64, x + 16, 76, BOARD)
            rect(img, x + 6, 78, x + 20, 98, BOARD)
        elif ch == "E":
            rect(img, x + 6, 64, x + 18, 70, BOARD)
            rect(img, x + 6, 74, x + 16, 80, BOARD)
            rect(img, x + 6, 84, x + 18, 90, BOARD)
        else:
            rect(img, x + 6, 64, x + 10, 90, BOARD)
            rect(img, x + 14, 64, x + 18, 90, BOARD)

    # glow fringe
    for x in range(36, 156):
        px(img, x, 34, (NEON_P[0], NEON_P[1], NEON_P[2], 90))
        px(img, x, 118, (NEON_C[0], NEON_C[1], NEON_C[2], 80))

    # pole
    shade_rect(img, 90, 118, 102, 168, METAL, METAL_H, METAL_D)
    shade_rect(img, 78, 164, 114, 176, METAL_D, METAL, OUTL)

    # selfie phone silhouette bottom-left (SNS feel)
    shade_rect(img, 16, 148, 46, 182, BOARD, BOARD_H, OUTL)
    rect(img, 20, 152, 42, 172, NEON_C)
    draw_heart(img, 26, 158, PINK_UI)

    img = add_outline(img)
    sns_chrome(img)
    return img


def photo_viewpoint():
    img = new()
    fill_gradient(img, (170, 210, 245, 255), (255, 210, 230, 255))

    # distant city
    heights = [36, 58, 44, 72, 50, 66, 40, 60, 48]
    for i, h in enumerate(heights):
        x0 = 6 + i * 20
        col = CITY if i % 2 == 0 else CITY_D
        shade_rect(img, x0, 110 - h, x0 + 16, 110, col, METAL, OUTL)
        if i % 2 == 0:
            px(img, x0 + 4, 110 - h + 12, WIN)
            px(img, x0 + 10, 110 - h + 24, WIN)

    # railing deck
    shade_rect(img, 0, 118, S, 128, METAL, METAL_H, METAL_D)
    for x in range(8, S, 14):
        shade_rect(img, x, 128, x + 5, 158, METAL, METAL_H, METAL_D)
    shade_rect(img, 0, 158, S, 176, WOOD, WOOD_H, GUITAR_D)
    rect(img, 0, 176, S, S, (40, 36, 34, 255))

    # cherry tree
    shade_rect(img, 142, 90, 152, 158, WOOD, WOOD_H, GUITAR_D)
    for dx, dy in [
        (0, 0), (14, -8), (-14, 4), (8, 12), (-10, -12), (18, 6), (-18, -2),
        (4, -18), (22, -4), (-6, 14), (12, 16), (-20, 8), (0, 10),
    ]:
        oval(img, 147 + dx, 70 + dy, 10, 8, CHERRY)
        px(img, 147 + dx, 66 + dy, CHERRY_H)

    # binoculars
    shade_rect(img, 58, 100, 68, 158, METAL, METAL_H, METAL_D)
    oval(img, 52, 92, 12, 10, BOARD)
    oval(img, 78, 92, 12, 10, BOARD)
    oval(img, 52, 92, 6, 5, NEON_C)
    oval(img, 78, 92, 6, 5, NEON_P)
    shade_rect(img, 44, 84, 86, 90, METAL, METAL_H, METAL_D)

    # polaroid sticker floating
    shade_rect(img, 18, 28, 58, 68, WHITE, CREAM, OUTL)
    rect(img, 22, 32, 54, 56, (120, 180, 230, 255))
    px(img, 30, 40, CHERRY)
    px(img, 40, 44, NEON_Y)

    # hearts rising
    draw_heart(img, 100, 30, PINK_UI)
    draw_heart(img, 118, 42, NEON_P)
    draw_star(img, 86, 48, NEON_Y)

    img = add_outline(img)
    sns_chrome(img)
    return img


paths = {
    "Act1_PhotoStill_Busking.png": photo_busking,
    "Act1_PhotoStill_Neon.png": photo_neon,
    "Act1_PhotoStill_Viewpoint.png": photo_viewpoint,
}

for name, fn in paths.items():
    img = fn()
    path = os.path.join(OUT, name)
    img.save(path)
    print("saved", path, img.size)
print("done")
