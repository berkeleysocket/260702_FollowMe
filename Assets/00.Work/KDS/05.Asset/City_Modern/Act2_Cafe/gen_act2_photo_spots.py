"""Act2 cafe photo subjects — denser, larger cells than Stage2 (Act1).

Landscape 96x64 · Portrait 64x96 · Round 64x64
Pipeline: shade_rect + add_outline(solid_alpha=200) + soft backdrop.
People are slightly larger than Act1 draw_person.
"""
from PIL import Image
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act2_Cafe"
os.makedirs(OUT, exist_ok=True)

CLR = (0, 0, 0, 0)
OUTLINE = (12, 10, 14, 255)

SKIN = (236, 204, 176, 255)
SKIN_S = (210, 168, 140, 255)
HAIR_A = (52, 38, 34, 255)
HAIR_B = (90, 48, 40, 255)
HAIR_C = (40, 44, 58, 255)
HAIR_D = (180, 140, 90, 255)

SHIRT_A = (86, 148, 220, 255)
SHIRT_AH = (130, 178, 235, 255)
SHIRT_B = (228, 92, 128, 255)
SHIRT_BH = (245, 140, 168, 255)
SHIRT_C = (64, 176, 132, 255)
SHIRT_CH = (110, 205, 165, 255)
SHIRT_D = (255, 170, 90, 255)
SHIRT_DH = (255, 200, 140, 255)
PANTS = (58, 54, 68, 255)
PANTS_H = (78, 74, 90, 255)
SHOE = (36, 32, 34, 255)

WOOD = (148, 104, 64, 255)
WOOD_H = (178, 132, 88, 255)
WOOD_D = (110, 72, 42, 255)
CREAM = (248, 240, 228, 255)
CREAM_D = (220, 206, 186, 255)
PINK = (255, 140, 178, 255)
PINK_H = (255, 190, 210, 255)
MINT = (120, 210, 180, 255)
MINT_H = (170, 235, 210, 255)
COFFEE = (110, 70, 42, 255)
COFFEE_H = (150, 105, 70, 255)
FOAM = (250, 246, 236, 255)
WHITE = (246, 246, 250, 255)
BLACK = (28, 26, 34, 255)
GRAY = (120, 118, 128, 255)
GRAY_H = (170, 168, 178, 255)
METAL = (150, 156, 168, 255)
METAL_H = (200, 206, 218, 255)
METAL_D = (90, 94, 104, 255)
NEON_P = (255, 96, 205, 255)
NEON_C = (90, 230, 255, 255)
NEON_Y = (255, 232, 96, 255)
NEON_G = (120, 255, 160, 255)
LEAF = (70, 150, 90, 255)
LEAF_H = (110, 190, 120, 255)
LEAF_D = (48, 110, 70, 255)
GLASS = (180, 220, 240, 200)
GOLD = (230, 180, 70, 255)
BRICK = (170, 90, 78, 255)
BRICK_H = (198, 120, 100, 255)
BRICK_D = (130, 60, 52, 255)
NIGHT = (28, 34, 58, 255)
NIGHT_H = (48, 56, 88, 255)
WARM = (255, 210, 130, 255)
WARM_H = (255, 230, 180, 255)
CITY = (70, 82, 104, 255)
CITY_D = (48, 56, 74, 255)
WIN = (255, 220, 120, 255)
BOARD = (28, 26, 38, 255)
BOARD_H = (48, 46, 64, 255)
SHADOW = (20, 18, 22, 90)
SKY_T = (186, 214, 242, 190)
SKY_B = (140, 168, 210, 120)
SUMMER = (255, 196, 150, 160)
LAVENDER = (200, 170, 220, 255)


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


def add_outline(img, color=OUTLINE, solid_alpha=200):
    """1px outline around solid pixels only (ignores soft glow/shadow/sky)."""
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


def with_backdrop_then_outline(draw_fn, backdrop_fn=None):
    outlined = add_outline(draw_fn())
    if backdrop_fn is None:
        return outlined
    back = backdrop_fn()
    back.paste(outlined, (0, 0), outlined)
    return back


def sky_grad(w, h, y_max=22, top=SKY_T, bot=SKY_B):
    img = new(w, h)
    for y in range(0, y_max):
        t = y / max(1, y_max - 1)
        a = int(top[3] * (1 - t) + bot[3] * t)
        c = (
            int(top[0] * (1 - t) + bot[0] * t),
            int(top[1] * (1 - t) + bot[1] * t),
            int(top[2] * (1 - t) + bot[2] * t),
            a,
        )
        hline(img, 0, w - 1, y, c)
    return img


def draw_person(img, ox, oy, shirt, shirt_hi, hair, frame=0, pose="phone"):
    """Larger than Act1 (~14px wide torso, ~28px tall). Strong 4-frame idle."""
    bob = [0, 1, 2, 1][frame % 4]
    arm_swing = [0, 1, 2, 1][frame % 4]
    head_y = oy - 28 + bob
    # shadow
    rect(img, ox + 2, oy - 1, ox + 15, oy + 1, SHADOW)
    # shoes (weight shift)
    if frame % 2 == 0:
        rect(img, ox + 2, oy - 3, ox + 7, oy, SHOE)
        rect(img, ox + 9, oy - 2, ox + 14, oy, SHOE)
    else:
        rect(img, ox + 2, oy - 2, ox + 7, oy, SHOE)
        rect(img, ox + 9, oy - 3, ox + 14, oy, SHOE)
    # legs
    shade_rect(img, ox + 3, oy - 11, ox + 7, oy - 3, PANTS, PANTS_H, SHOE)
    shade_rect(img, ox + 9, oy - 11, ox + 13, oy - 3, PANTS, PANTS_H, SHOE)
    # torso
    shade_rect(img, ox + 2, oy - 21 + (bob // 2), ox + 14, oy - 11, shirt, shirt_hi, PANTS)
    # head
    oval(img, ox + 8, head_y, 5, 5, SKIN)
    px(img, ox + 5, head_y + 1, SKIN_S)
    px(img, ox + 11, head_y + 1, SKIN_S)
    px(img, ox + 6, head_y, OUTLINE)
    px(img, ox + 10, head_y, OUTLINE)
    # blink on frame 3
    if frame % 4 == 3:
        hline(img, ox + 5, ox + 7, head_y, OUTLINE)
        hline(img, ox + 9, ox + 11, head_y, OUTLINE)
    # hair
    rect(img, ox + 3, head_y - 6, ox + 14, head_y - 1, hair)
    px(img, ox + 4, head_y - 7, hair)
    px(img, ox + 8, head_y - 7, hair)
    px(img, ox + 11, head_y - 6, hair)
    px(img, ox + 13, head_y - 5, hair)

    arm_y = oy - 18 + arm_swing
    if pose == "phone":
        # raise / lower phone clearly across frames
        lift = [0, -2, -4, -2][frame % 4]
        shade_rect(img, ox + 13, arm_y - 8 + lift, ox + 20, arm_y + lift, BLACK, GRAY, BLACK)
        flash = [NEON_C, NEON_P, WHITE, NEON_Y][frame % 4]
        px(img, ox + 15, arm_y - 5 + lift, flash)
        px(img, ox + 16, arm_y - 4 + lift, WHITE)
        px(img, ox + 17, arm_y - 5 + lift, flash)
        rect(img, ox + 11, arm_y - 3 + lift, ox + 15, arm_y + 1 + lift, SKIN)
        rect(img, ox - 1, oy - 17, ox + 2, oy - 12, SKIN)
        if frame % 4 == 2:
            px(img, ox + 7, head_y + 1, WARM_H)
            px(img, ox + 9, head_y + 2, WARM)
            px(img, ox + 14, arm_y - 9 + lift, WHITE)
            px(img, ox + 18, arm_y - 10 + lift, WARM_H)
    elif pose == "camera":
        lift = [0, -1, -3, -1][frame % 4]
        shade_rect(img, ox + 13, arm_y - 4 + lift, ox + 23, arm_y + 4 + lift, BOARD, BOARD_H, OUTLINE)
        oval(img, ox + 18, arm_y + lift, 4, 4, METAL)
        oval(img, ox + 18, arm_y + lift, 2, 2, METAL_D)
        oval(img, ox + 18, arm_y + lift, 1, 1, [NEON_C, NEON_Y, WHITE, NEON_P][frame % 4])
        shade_rect(img, ox + 19, arm_y - 7 + lift, ox + 23, arm_y - 4 + lift, WHITE, WHITE, METAL)
        rect(img, ox + 11, arm_y - 1 + lift, ox + 14, arm_y + 2 + lift, SKIN)
        if frame % 2 == 0:
            px(img, ox + 21, arm_y - 8 + lift, WARM_H)
            px(img, ox + 22, arm_y - 9 + lift, WHITE)
            px(img, ox + 20, arm_y - 10 + lift, WHITE)
    elif pose == "cup":
        lift = [0, 1, 2, 1][frame % 4]
        shade_rect(img, ox + 13, arm_y - 2 - lift, ox + 20, arm_y + 5 - lift, WHITE, CREAM, GRAY)
        rect(img, ox + 14, arm_y - 1 - lift, ox + 19, arm_y + 3 - lift, COFFEE)
        oval(img, ox + 16, arm_y - 1 - lift, 2, 1, FOAM)
        rect(img, ox + 11, arm_y - lift, ox + 14, arm_y + 2 - lift, SKIN)
        rect(img, ox - 1, oy - 17, ox + 2, oy - 12, SKIN)
        if frame % 2 == 0:
            px(img, ox + 16, arm_y - 4 - lift, WHITE)
            px(img, ox + 17, arm_y - 6 - lift, WHITE)
    else:
        # idle wave
        wave = [0, 2, 4, 2][frame % 4]
        rect(img, ox + 13, arm_y - wave, ox + 16, arm_y + 3 - wave, SKIN)
        rect(img, ox + 16, arm_y - 1 - wave, ox + 18, arm_y + 1 - wave, SKIN)
        rect(img, ox - 1, oy - 17, ox + 2, oy - 12, SKIN)


def draw_spark(img, x, y, c):
    px(img, x, y, c)
    px(img, x + 1, y, WHITE)
    px(img, x, y - 1, c)
    px(img, x - 1, y, c)
    px(img, x, y + 1, c)


def draw_macaron(img, cx, cy, rx, ry, shell, shell_h, cream=CREAM):
    oval(img, cx, cy - 1, rx, max(2, ry - 1), shell_h)
    oval(img, cx, cy, rx, ry, shell)
    oval(img, cx, cy + 1, rx - 1, max(1, ry - 2), cream)
    oval(img, cx, cy + 2, rx - 2, max(1, ry - 3), shell)
    for dx in range(-rx + 1, rx, 2):
        px(img, cx + dx, cy + ry - 1, shell_h)


def draw_donut(img, cx, cy, c, icing, scale=1):
    rx, ry = 7 * scale, 6 * scale
    oval(img, cx, cy, rx, ry, c)
    oval(img, cx, cy - 1, rx - 1, ry - 2, icing)
    oval(img, cx, cy, max(2, 2 * scale), max(2, 2 * scale), CREAM)
    # hole punch as cream center ring then clear-ish
    oval(img, cx, cy, max(1, scale), max(1, scale), CREAM_D)
    for dx in range(-(rx - 2), rx - 1, 2):
        px(img, cx + dx, cy - 2, WHITE)
        px(img, cx + dx + 1, cy - 1, NEON_Y if (dx // 2) % 2 == 0 else PINK_H)


def draw_neon_letter(img, x, y, letter, on_color, on, h=14, w=8):
    c = on_color if on else (on_color[0] // 3, on_color[1] // 3, on_color[2] // 3, 255)
    hi = WHITE if on else c
    if letter == "C":
        shade_rect(img, x, y, x + w, y + h, c, hi, OUTLINE)
        rect(img, x + 2, y + 2, x + w, y + h - 2, BOARD)
    elif letter == "A":
        shade_rect(img, x, y, x + w, y + h, c, hi, OUTLINE)
        rect(img, x + 2, y + 2, x + w - 2, y + 6, BOARD)
        rect(img, x + 2, y + 8, x + w - 2, y + h, BOARD)
    elif letter == "F":
        shade_rect(img, x, y, x + w, y + h, c, hi, OUTLINE)
        rect(img, x + 2, y + 2, x + w, y + 4, BOARD)
        rect(img, x + 2, y + 5, x + w - 2, y + 7, BOARD)
        rect(img, x + 2, y + 8, x + w, y + h, BOARD)
    elif letter == "E":
        shade_rect(img, x, y, x + w, y + h, c, hi, OUTLINE)
        rect(img, x + 2, y + 2, x + w, y + 4, BOARD)
        rect(img, x + 2, y + 5, x + w - 2, y + 7, BOARD)
        rect(img, x + 2, y + 8, x + w, y + 10, BOARD)


def brick_wall(img, x0, y0, x1, y1):
    shade_rect(img, x0, y0, x1, y1, BRICK, BRICK_H, BRICK_D)
    row_h = 8
    for y in range(y0 + 4, y1, row_h):
        hline(img, x0, x1 - 1, y, BRICK_D)
        off = 4 if ((y - y0) // row_h) % 2 == 0 else 0
        for x in range(x0 + 2 + off, x1 - 1, 8):
            vline(img, x, y, min(y + row_h, y1 - 1), BRICK_D)


def build_sheet(tiles, path, cell_w, cell_h):
    cols = len(tiles)
    sheet = Image.new("RGBA", (cols * cell_w, cell_h), CLR)
    for i, fn in enumerate(tiles):
        sheet.paste(fn(), (i * cell_w, 0))
    sheet.save(path)
    print("sheet", os.path.basename(path), sheet.size)


# ===================== subjects =====================

def sub_latte(frame):
    """96x64 — window seat latte art, person + cup + steam."""
    steam_off = [0, 1, 2, 1][frame % 4]

    def solid():
        img = new(96, 64)
        brick_wall(img, 0, 4, 52, 54)
        # large window frame
        shade_rect(img, 4, 6, 48, 50, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 8, 10, 44, 46, GLASS, WHITE, METAL_D)
        hline(img, 8, 43, 28, WOOD_H)
        vline(img, 26, 10, 45, WOOD_H)
        # sun bloom through glass
        oval(img, 36, 18, 7, 6, WARM)
        oval(img, 36, 18, 3, 3, WARM_H)
        # curtains
        shade_rect(img, 8, 10, 13, 46, PINK_H, WHITE, PINK)
        shade_rect(img, 39, 10, 44, 46, PINK_H, WHITE, PINK)
        for y in range(12, 44, 4):
            px(img, 10, y + (frame % 2), PINK)
            px(img, 41, y + 1 - (frame % 2), PINK)
        # plant on sill
        shade_rect(img, 14, 42, 22, 48, WOOD_D, WOOD, BLACK)
        oval(img, 18, 40 - (frame % 2), 5, 4, LEAF)
        oval(img, 16, 38, 3, 3, LEAF_H)
        oval(img, 20, 37, 2, 2, PINK)
        # table
        shade_rect(img, 52, 36, 90, 44, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 56, 44, 62, 58, WOOD_D, WOOD, BLACK)
        shade_rect(img, 78, 44, 84, 58, WOOD_D, WOOD, BLACK)
        oval(img, 70, 38, 14, 4, SHADOW)
        # big latte cup
        shade_rect(img, 62, 20, 78, 36, WHITE, CREAM, GRAY)
        rect(img, 64, 22, 76, 32, COFFEE)
        oval(img, 70, 22, 5, 3, FOAM)
        # heart latte art bob
        hy = 24 - (frame % 2)
        px(img, 68, hy, FOAM)
        px(img, 69, hy - 1, FOAM)
        px(img, 70, hy, FOAM)
        px(img, 71, hy - 1, FOAM)
        px(img, 72, hy, FOAM)
        px(img, 70, hy + 1, FOAM)
        px(img, 70, hy + 2, FOAM)
        # handle
        shade_rect(img, 78, 24, 84, 32, WHITE, CREAM, GRAY)
        rect(img, 79, 25, 83, 31, CLR)
        oval(img, 70, 36, 8, 3, CREAM_D)
        # saucer detail
        shade_rect(img, 64, 36, 76, 38, CREAM, WHITE, GRAY)
        # steam columns (4 distinct phases)
        sx = 68 + steam_off
        for i, (dx, dy) in enumerate(((0, -6), (2, -10), (-1, -14), (3, -12))):
            if (frame + i) % 4 != 3:
                px(img, sx + dx, 18 + dy, WHITE)
                px(img, sx + dx + 1, 17 + dy, (255, 255, 255, 180))
        # pastry plate
        oval(img, 56, 32, 6, 3, CREAM)
        oval(img, 56, 30, 4, 2, GOLD)
        px(img, 55, 29, WHITE)
        draw_person(img, 28, 58, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        draw_person(img, 8, 58, SHIRT_A, SHIRT_AH, HAIR_A, frame + 1, "cup")
        return img

    def soft():
        img = sky_grad(96, 64, 18, SUMMER, SKY_B)
        rect(img, 6, 60, 90, 63, SHADOW)
        if frame == 2:
            for x in range(62, 78):
                px(img, x, 18, (255, 240, 200, 40))
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_macaron(frame):
    """64x96 — towering macaron stack."""
    bob = frame % 2
    sparkle = frame % 4

    def solid():
        img = new(64, 96)
        # striped backdrop wall
        shade_rect(img, 0, 8, 64, 88, CREAM, WHITE, CREAM_D)
        for y in range(10, 86, 6):
            hline(img, 0, 63, y, CREAM_D if (y // 6) % 2 == 0 else PINK_H)
        # pedestal
        shade_rect(img, 12, 72, 52, 80, METAL_H, WHITE, METAL)
        shade_rect(img, 16, 80, 48, 86, METAL, METAL_H, METAL_D)
        shade_rect(img, 20, 86, 44, 92, METAL_D, METAL, BLACK)
        # stacked macarons (6 layers)
        stack = [
            (32, 68, 18, 6, PINK, PINK_H),
            (32, 58, 17, 6, MINT, MINT_H),
            (32, 48, 16, 5, NEON_Y, WHITE),
            (32, 39, 15, 5, NEON_C, WHITE),
            (32, 30, 14, 5, LAVENDER, WHITE),
            (32, 22, 13, 4, PINK_H, WHITE),
        ]
        for i, (cx, cy, rx, ry, shell, hi) in enumerate(stack):
            yoff = bob if i == 0 else (1 if (frame + i) % 3 == 0 else 0)
            draw_macaron(img, cx, cy - yoff, rx, ry, shell, hi)
        # cherry + ribbon on top
        ty = 14 - bob
        shade_rect(img, 26, ty + 2, 38, ty + 7, GOLD, NEON_Y, WOOD_D)
        px(img, 24, ty + 3, GOLD)
        px(img, 38, ty + 3, GOLD)
        oval(img, 32, ty, 3, 3, PINK)
        px(img, 32, ty - 2, LEAF)
        # side sparkles (4 phases)
        sparks = [
            [(6, 28, WHITE), (54, 20, GOLD), (8, 50, NEON_Y)],
            [(10, 22, GOLD), (56, 36, WHITE), (4, 44, PINK_H)],
            [(8, 34, WHITE), (52, 18, NEON_C), (12, 56, GOLD)],
            [(6, 40, GOLD), (58, 28, WHITE), (10, 18, PINK_H)],
        ]
        for x, y, c in sparks[sparkle]:
            draw_spark(img, x, y, c)
        draw_person(img, 2, 90, SHIRT_A, SHIRT_AH, HAIR_A, frame, "camera")
        draw_person(img, 44, 90, SHIRT_B, SHIRT_BH, HAIR_B, frame + 1, "phone")
        return img

    def soft():
        img = sky_grad(64, 96, 20, SUMMER, SKY_B)
        rect(img, 10, 92, 54, 95, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_donut_wall(frame):
    """96x64 — dense donut pegboard wall."""
    def solid():
        img = new(96, 64)
        shade_rect(img, 2, 2, 94, 48, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 5, 5, 91, 45, CREAM_D, CREAM, WOOD_D)
        # peg grid
        for y in range(10, 42, 5):
            for x in range(10, 88, 5):
                px(img, x, y, WOOD_D)
        donuts = [
            (14, 14, PINK, PINK_H), (28, 12, MINT, MINT_H), (42, 14, NEON_Y, WHITE),
            (56, 12, NEON_C, WHITE), (70, 14, GOLD, NEON_Y), (84, 13, LAVENDER, WHITE),
            (16, 28, COFFEE_H, COFFEE), (30, 30, PINK_H, WHITE), (44, 28, WHITE, CREAM),
            (58, 30, MINT, MINT_H), (72, 28, NEON_P, PINK_H), (84, 29, NEON_Y, WHITE),
        ]
        for i, (x, y, c, icing) in enumerate(donuts):
            yy = y + ((frame + i) % 2)
            # subtle sway left/right by frame
            xx = x + (1 if (frame + i) % 4 == 1 else (-1 if (frame + i) % 4 == 3 else 0))
            draw_donut(img, xx, yy, c, icing)
        # shelf lip + crumbs
        shade_rect(img, 2, 46, 94, 52, WOOD_D, WOOD, BLACK)
        for x in (20, 40, 60, 80):
            px(img, x + frame, 50, CREAM)
        # sign
        shade_rect(img, 30, 0, 66, 8, PINK, PINK_H, OUTLINE)
        for i, ch_x in enumerate((34, 42, 50, 58)):
            px(img, ch_x, 3, WHITE)
            px(img, ch_x + 1, 4, WHITE)
        draw_person(img, 36, 62, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        draw_person(img, 8, 62, SHIRT_C, SHIRT_CH, HAIR_C, frame + 1, "idle")
        draw_person(img, 68, 62, SHIRT_D, SHIRT_DH, HAIR_D, frame, "camera")
        return img

    def soft():
        img = new(96, 64)
        rect(img, 8, 60, 88, 63, SHADOW)
        if frame % 2 == 0:
            for x in range(10, 86, 8):
                px(img, x, 4, (255, 220, 180, 50))
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_neon_cafe(frame):
    """96x64 — neon CAFE facade with queue."""
    def solid():
        img = new(96, 64)
        brick_wall(img, 0, 14, 96, 56)
        # striped awning
        for i in range(14):
            c = PINK if i % 2 == 0 else WHITE
            shade_rect(img, 2 + i * 6, 4, 9 + i * 6, 16, c, WHITE if c == PINK else CREAM, GRAY)
        # neon board
        shade_rect(img, 14, 18, 82, 42, BOARD_H, METAL_H, OUTLINE)
        shade_rect(img, 17, 20, 79, 40, BOARD, BOARD_H, OUTLINE)
        rect(img, 20, 22, 76, 38, (16, 14, 24, 255))
        cols = [NEON_P, NEON_C, NEON_Y, NEON_G]
        for i, ch in enumerate("CAFE"):
            # each frame dims a different letter (very readable anim)
            on = (frame % 4) != i
            draw_neon_letter(img, 24 + i * 13, 23, ch, cols[i], on, h=14, w=10)
        # door + windows
        shade_rect(img, 38, 42, 58, 58, WOOD_D, WOOD, BLACK)
        px(img, 54, 50, GOLD)
        shade_rect(img, 10, 44, 30, 56, GLASS, WARM if frame % 2 == 0 else WARM_H, WOOD_D)
        shade_rect(img, 66, 44, 86, 56, GLASS, WARM_H if frame % 2 == 0 else WARM, WOOD_D)
        # menu board
        shade_rect(img, 60, 30, 74, 40, CREAM, WHITE, WOOD_D)
        for y in (32, 34, 36, 38):
            hline(img, 62, 72, y, COFFEE if y % 4 == 0 else GRAY)
        # queue of 3
        draw_person(img, 4, 62, SHIRT_A, SHIRT_AH, HAIR_A, frame, "phone")
        draw_person(img, 22, 62, SHIRT_C, SHIRT_CH, HAIR_C, frame + 1, "idle")
        draw_person(img, 72, 62, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        return img

    def soft():
        img = new(96, 64)
        glow = [NEON_P, NEON_C, NEON_Y, NEON_G][frame % 4]
        for x in range(18, 80):
            px(img, x, 17, (glow[0], glow[1], glow[2], 55 + 20 * (frame % 2)))
            px(img, x, 41, (glow[0], glow[1], glow[2], 40))
        # flicker halo
        if frame != 3:
            for x in range(24, 72, 3):
                px(img, x, 22, (glow[0], glow[1], glow[2], 35))
        rect(img, 4, 60, 92, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_terrace(frame):
    """96x64 — rooftop terrace with city + lights."""
    off = frame % 2

    def solid():
        img = new(96, 64)
        heights = [10, 16, 12, 20, 14, 18, 11, 17, 13, 19, 12, 15, 10]
        for i, h in enumerate(heights):
            x0 = 1 + i * 7
            col = CITY if i % 2 == 0 else CITY_D
            shade_rect(img, x0, 22 - h, x0 + 6, 22, col, METAL, OUTLINE)
            if i % 2 == 0:
                px(img, x0 + 2, 22 - h + 3, WIN if (frame + i) % 3 != 0 else NEON_Y)
                px(img, x0 + 4, 22 - h + 6, WIN)
            if i % 3 == 0:
                px(img, x0 + 1, 22 - h + 2, NEON_C)
        # railing
        shade_rect(img, 1, 30, 95, 35, METAL, METAL_H, METAL_D)
        for x in range(4, 92, 5):
            shade_rect(img, x, 35, x + 2, 48, METAL, METAL_H, METAL_D)
        shade_rect(img, 0, 48, 96, 54, WOOD, WOOD_H, WOOD_D)
        # planter boxes
        shade_rect(img, 10, 38, 40, 48, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 56, 38, 86, 48, WOOD, WOOD_H, WOOD_D)
        blooms = [
            (0, 0), (5, -2), (-5, 1), (3, 3), (-3, -3), (8, 1), (-7, 0),
            (2, -5), (10, -1), (-9, 2), (6, 4), (-4, -4),
        ]
        for dx, dy in blooms:
            c = PINK if (dx + dy) % 2 == 0 else MINT
            oval(img, 25 + dx, 34 + dy - off, 3, 2, c)
            px(img, 25 + dx, 33 + dy - off, PINK_H if c == PINK else LEAF_H)
            oval(img, 71 + dx, 34 + dy - off, 3, 2, MINT if c == PINK else PINK)
        # string lights (bobbing bulbs)
        hline(img, 8, 88, 6, METAL_D)
        for i, x in enumerate(range(10, 88, 7)):
            y = 8 + (2 if (i + frame) % 2 == 0 else 5)
            vline(img, x, 6, y, METAL_D)
            on = ((frame + i) % 4) != 3
            bulb = (NEON_Y if i % 2 == 0 else NEON_P) if on else (GOLD if i % 2 == 0 else PINK)
            oval(img, x, y + 2, 2, 3, bulb)
            if on and frame % 2 == 0:
                px(img, x, y + 5, WHITE)
        # cafe table + drinks
        shade_rect(img, 42, 42, 54, 50, WOOD_H, CREAM, WOOD_D)
        shade_rect(img, 44, 36, 48, 42, WHITE, CREAM, GRAY)
        shade_rect(img, 50, 36, 54, 42, WHITE, CREAM, GRAY)
        draw_person(img, 30, 62, SHIRT_C, SHIRT_CH, HAIR_C, frame, "phone")
        draw_person(img, 54, 62, SHIRT_B, SHIRT_BH, HAIR_B, frame + 1, "cup")
        return img

    def soft():
        img = sky_grad(96, 64, 20, SUMMER, (255, 160, 120, 140))
        rect(img, 0, 60, 96, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_mirror(frame):
    """64x96 — vanity mirror room."""
    bob = frame % 2

    def solid():
        img = new(64, 96)
        shade_rect(img, 0, 4, 64, 90, CREAM_D, CREAM, WOOD_D)
        # wallpaper dots
        for y in range(8, 88, 8):
            for x in range(4, 60, 8):
                px(img, x + (y // 8) % 2 * 2, y, PINK_H)
        # ornate frame
        shade_rect(img, 6, 8, 58, 72, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 9, 11, 55, 69, GOLD, NEON_Y, WOOD_D)
        shade_rect(img, 13, 15, 51, 65, GLASS, WHITE, METAL)
        # reflection (bobbing)
        oval(img, 32, 28 + bob, 6, 6, (210, 180, 160, 230))
        px(img, 29, 27 + bob, OUTLINE)
        px(img, 35, 27 + bob, OUTLINE)
        rect(img, 26, 26 + bob, 38, 28 + bob, HAIR_B)
        shade_rect(img, 26, 34 + bob, 38, 50 + bob, (200, 120, 150, 210), (230, 160, 180, 210), None)
        shade_rect(img, 27, 50 + bob, 31, 58 + bob, PANTS, None, None)
        shade_rect(img, 33, 50 + bob, 37, 58 + bob, PANTS, None, None)
        # phone in reflection
        shade_rect(img, 38, 36 + bob, 44, 44 + bob, BLACK, GRAY, BLACK)
        if frame == 2:
            px(img, 40, 39 + bob, WHITE)
            px(img, 41, 38 + bob, WARM_H)
        # vanity lights blink pattern
        for i, y in enumerate((16, 28, 40, 52, 62)):
            on = ((frame + i) % 4) != 3
            c = WARM_H if on else GOLD
            oval(img, 10, y, 3, 3, c)
            oval(img, 54, y, 3, 3, c)
            if on:
                px(img, 10, y, WHITE)
                px(img, 54, y, WHITE)
        # floor tiles
        for x in range(4, 60, 8):
            shade_rect(img, x, 76, x + 7, 88, CREAM if (x // 8) % 2 == 0 else CREAM_D, WHITE, WOOD_D)
        draw_person(img, 24, 92, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        return img

    def soft():
        img = new(64, 96)
        for i, y in enumerate((16, 28, 40, 52, 62)):
            if ((frame + i) % 4) != 3:
                px(img, 8, y, (255, 230, 160, 55))
                px(img, 56, y, (255, 230, 160, 55))
        rect(img, 12, 90, 52, 94, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_plant(frame):
    """64x96 — greenhouse plant cafe."""
    bob = [0, 1, 2, 1][frame % 4]

    def solid():
        img = new(64, 96)
        # greenhouse frame
        shade_rect(img, 2, 4, 62, 88, METAL_H, WHITE, METAL)
        shade_rect(img, 6, 8, 58, 82, GLASS, WHITE, METAL_D)
        vline(img, 32, 8, 81, METAL_H)
        hline(img, 6, 57, 44, METAL_H)
        # hanging pots top
        for hx in (14, 32, 50):
            shade_rect(img, hx - 4, 10, hx + 4, 16, WOOD_D, WOOD, BLACK)
            oval(img, hx, 18 + bob, 6, 5, LEAF)
            oval(img, hx - 3, 20 + bob, 3, 4, LEAF_D)
            oval(img, hx + 3, 19 + bob, 3, 3, LEAF_H)
            vline(img, hx, 8, 10, METAL_D)
        # big floor pot
        shade_rect(img, 18, 66, 46, 78, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 20, 78, 44, 84, WOOD_D, WOOD, BLACK)
        rect(img, 20, 64, 44, 68, COFFEE)
        # dense foliage
        oval(img, 32, 48 - bob, 16, 14, LEAF)
        oval(img, 32, 44 - bob, 12, 10, LEAF_H)
        oval(img, 18, 40 - bob, 9, 11, LEAF_D)
        oval(img, 46, 38 - bob, 9, 10, LEAF)
        oval(img, 32, 28 - bob, 8, 9, MINT)
        oval(img, 24, 34 - bob, 5, 6, LEAF_H)
        oval(img, 40, 32 - bob, 5, 6, LEAF_D)
        # hanging vines sides
        for y in range(12, 70, 3):
            px(img, 8, y + (bob if y % 2 == 0 else 0), LEAF)
            px(img, 9, y + 1, LEAF_H)
            px(img, 55, y + 1 - (bob if y % 2 else 0), LEAF)
            px(img, 54, y + 2, LEAF_D)
        # flowers
        for fx, fy, fc in ((20, 26, PINK), (36, 22, NEON_Y), (44, 30, PINK_H), (26, 36, WHITE)):
            oval(img, fx, fy - bob, 2, 2, fc)
            px(img, fx, fy - 1 - bob, GOLD)
        # watering can
        shade_rect(img, 48, 70, 58, 78, METAL, METAL_H, METAL_D)
        shade_rect(img, 56, 68, 62, 72, METAL_H, WHITE, METAL)
        if frame % 2 == 0:
            px(img, 60, 66, NEON_C)
            px(img, 61, 65, WHITE)
        draw_person(img, 4, 90, SHIRT_C, SHIRT_CH, HAIR_C, frame, "phone")
        draw_person(img, 40, 90, SHIRT_A, SHIRT_AH, HAIR_A, frame + 1, "idle")
        return img

    def soft():
        img = sky_grad(64, 96, 16, (200, 240, 210, 150), SKY_B)
        rect(img, 8, 90, 56, 94, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_night(frame):
    """64x96 — night cafe window glow."""
    glow = frame % 2 == 0
    flicker = frame % 4

    def solid():
        img = new(64, 96)
        shade_rect(img, 0, 2, 64, 90, NIGHT, NIGHT_H, BLACK)
        for y in (14, 28, 42, 56, 70):
            hline(img, 0, 63, y, (36, 42, 68, 255))
            off = 4 if (y // 14) % 2 == 0 else 0
            for x in range(4 + off, 60, 8):
                vline(img, x, y, min(y + 14, 89), (36, 42, 68, 255))
        # glowing window
        warm = WARM_H if glow else WARM
        shade_rect(img, 8, 12, 56, 52, WOOD_D, WOOD, BLACK)
        shade_rect(img, 12, 16, 52, 48, warm, (255, 240, 190, 255), GOLD)
        hline(img, 12, 51, 32, WOOD_D)
        vline(img, 32, 16, 47, WOOD_D)
        # interior silhouettes
        oval(img, 22, 28 + (frame % 2), 4, 4, (80, 50, 40, 200))
        shade_rect(img, 18, 32 + (frame % 2), 26, 44, (70, 40, 35, 180))
        oval(img, 42, 30, 4, 4, (80, 50, 40, 200))
        shade_rect(img, 38, 34, 46, 44, (70, 40, 35, 180))
        # neon sign above
        on = flicker != 3
        shade_rect(
            img, 16, 4, 48, 11,
            NEON_P if on else (90, 30, 70, 255),
            WHITE if on else NEON_P,
            OUTLINE,
        )
        for x in (20, 28, 36, 42):
            px(img, x, 6, WHITE if on else GRAY)
            px(img, x + 1, 7, WHITE if on else GRAY)
        # sill plants
        shade_rect(img, 10, 52, 54, 58, WOOD, WOOD_H, WOOD_D)
        oval(img, 18, 50, 5, 4, LEAF)
        oval(img, 32, 49, 4, 4, LEAF_H)
        oval(img, 46, 50, 5, 4, LEAF)
        px(img, 18, 48, PINK)
        px(img, 46, 47, NEON_Y)
        # street lamp
        shade_rect(img, 56, 60, 60, 88, METAL_D, METAL, BLACK)
        oval(img, 58, 58, 4, 3, WARM_H if glow else GOLD)
        draw_person(img, 22, 90, SHIRT_B, SHIRT_BH, HAIR_B, frame, "camera")
        return img

    def soft():
        img = new(64, 96)
        if glow:
            for y in range(16, 50):
                for x in range(10, 54):
                    if (x + y + frame) % 6 == 0:
                        px(img, x, y, (255, 210, 140, 30))
            rect(img, 6, 50, 58, 62, (255, 190, 110, 45))
        if flicker != 3:
            for x in range(16, 48):
                px(img, x, 3, (255, 96, 205, 40))
        rect(img, 12, 90, 52, 94, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_round_window(frame):
    """64x64 — round window portrait shot."""
    bob = frame % 2
    leaf_bob = [0, 1, 0, -1][frame % 4]

    def solid():
        img = new(64, 64)
        brick_wall(img, 0, 0, 64, 64)
        # round window frame (thick)
        oval(img, 32, 28, 22, 22, WOOD)
        oval(img, 32, 28, 19, 19, WOOD_H)
        oval(img, 32, 28, 17, 17, GLASS)
        hline(img, 16, 48, 28, WOOD)
        vline(img, 32, 12, 44, WOOD)
        # warm interior glow + silhouette
        oval(img, 32, 30, 10, 8, (255, 200, 140, 180))
        oval(img, 32, 22 + bob, 5, 5, (70, 50, 60, 230))
        rect(img, 27, 20 + bob, 37, 23 + bob, HAIR_A)
        shade_rect(img, 28, 27 + bob, 36, 38 + bob, (60, 40, 55, 210))
        # phone flash in window
        if frame == 2:
            px(img, 36, 26 + bob, WHITE)
            px(img, 37, 25 + bob, WARM_H)
            oval(img, 32, 28, 6, 5, (255, 240, 200, 80))
        # ivy around frame
        vines = [
            (-20, 2), (-18, 10), (-16, 16), (18, 4), (20, 12), (16, 18),
            (-12, -16), (14, -14), (-8, -20), (10, -18), (-22, -6), (22, -4),
        ]
        for dx, dy in vines:
            oval(img, 32 + dx, 28 + dy + leaf_bob, 4, 3, LEAF if (dx + dy) % 2 == 0 else LEAF_H)
            px(img, 32 + dx, 27 + dy + leaf_bob, LEAF_D if dx < 0 else PINK)
        # flower accents
        for fx, fy in ((12, 8), (52, 10), (10, 40), (54, 38)):
            oval(img, fx, fy + leaf_bob, 2, 2, PINK if (fx + frame) % 2 == 0 else NEON_Y)
        draw_person(img, 6, 60, SHIRT_A, SHIRT_AH, HAIR_A, frame, "phone")
        draw_person(img, 42, 60, SHIRT_B, SHIRT_BH, HAIR_B, frame + 1, "camera")
        return img

    def soft():
        img = new(64, 64)
        oval(img, 32, 28, 12, 12, (255, 220, 160, 45))
        if frame == 2:
            oval(img, 32, 28, 8, 8, (255, 250, 220, 60))
        rect(img, 8, 60, 56, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_dessert_cart(frame):
    """96x64 — street dessert cart packed with sweets."""
    bob = frame % 2

    def solid():
        img = new(96, 64)
        # cart body
        shade_rect(img, 10, 22, 70, 42, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 12, 14, 68, 26, WHITE, CREAM, GRAY)
        shade_rect(img, 14, 8, 66, 22, GLASS, WHITE, METAL)
        # glass shelves of desserts
        desserts = [
            (20, 14, PINK, PINK_H), (30, 13, MINT, MINT_H), (40, 14, GOLD, NEON_Y),
            (50, 13, NEON_C, WHITE), (60, 14, LAVENDER, WHITE),
            (25, 18, NEON_Y, WHITE), (35, 18, PINK_H, WHITE), (45, 18, COFFEE_H, COFFEE),
            (55, 18, MINT, WHITE),
        ]
        for i, (x, y, c, hi) in enumerate(desserts):
            yy = y - ((frame + i) % 2)
            oval(img, x, yy, 5, 4, c)
            oval(img, x, yy - 1, 4, 2, hi)
            px(img, x, yy - 3, WHITE)
            if i % 3 == 0:
                px(img, x + 2, yy - 2, GOLD)
        # striped canopy
        for i in range(9):
            c = NEON_P if i % 2 == 0 else WHITE
            shade_rect(img, 12 + i * 6, 0, 19 + i * 6, 10, c, WHITE, GRAY)
        # wheels
        oval(img, 22, 50, 8, 8, BLACK)
        oval(img, 22, 50, 4, 4, METAL_H)
        oval(img, 58, 50, 8, 8, BLACK)
        oval(img, 58, 50, 4, 4, METAL_H)
        # spokes rotate by frame
        for ang in range(4):
            dx = (ang + frame) % 4 - 1
            px(img, 22 + dx, 50, WHITE)
            px(img, 58 - dx, 50, WHITE)
        # price board
        shade_rect(img, 70, 18, 88, 34, NEON_Y, WHITE, OUTLINE)
        for y in (21, 24, 27, 30):
            hline(img, 72, 86, y, BOARD)
        # balloon
        oval(img, 80, 10 - bob, 5, 6, PINK if frame % 2 == 0 else NEON_C)
        vline(img, 80, 16 - bob, 22, GRAY)
        # sparkles
        sparks = [
            [(24, 4, WHITE), (48, 3, GOLD), (64, 6, WHITE)],
            [(28, 5, GOLD), (44, 2, WHITE), (60, 5, PINK_H)],
            [(22, 3, WHITE), (52, 4, GOLD), (66, 3, NEON_Y)],
            [(30, 6, GOLD), (40, 2, WHITE), (58, 4, WHITE)],
        ]
        for x, y, c in sparks[frame % 4]:
            draw_spark(img, x, y, c)
        draw_person(img, 72, 60, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        draw_person(img, 2, 60, SHIRT_C, SHIRT_CH, HAIR_C, frame + 1, "idle")
        draw_person(img, 40, 60, SHIRT_D, SHIRT_DH, HAIR_D, frame, "cup")
        return img

    def soft():
        img = new(96, 64)
        for y in range(0, 14):
            t = y / 13.0
            c = (
                int(40 * (1 - t) + 28 * t),
                int(48 * (1 - t) + 34 * t),
                int(78 * (1 - t) + 58 * t),
                int(130 * (1 - t) + 40 * t),
            )
            hline(img, 0, 95, y, c)
        if frame % 2 == 0:
            for x in range(14, 66, 4):
                px(img, x, 2, (255, 220, 140, 45))
        rect(img, 8, 60, 88, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


SUBJECTS = [
    ("Act2_PhotoSubject_LatteArt_96x64.png", "Act2_PhotoStill_LatteArt.png", 96, 64, sub_latte),
    ("Act2_PhotoSubject_Macaron_64x96.png", "Act2_PhotoStill_Macaron.png", 64, 96, sub_macaron),
    ("Act2_PhotoSubject_DonutWall_96x64.png", "Act2_PhotoStill_DonutWall.png", 96, 64, sub_donut_wall),
    ("Act2_PhotoSubject_NeonCafe_96x64.png", "Act2_PhotoStill_NeonCafe.png", 96, 64, sub_neon_cafe),
    ("Act2_PhotoSubject_Terrace_96x64.png", "Act2_PhotoStill_Terrace.png", 96, 64, sub_terrace),
    ("Act2_PhotoSubject_MirrorRoom_64x96.png", "Act2_PhotoStill_MirrorRoom.png", 64, 96, sub_mirror),
    ("Act2_PhotoSubject_PlantCafe_64x96.png", "Act2_PhotoStill_PlantCafe.png", 64, 96, sub_plant),
    ("Act2_PhotoSubject_NightWindow_64x96.png", "Act2_PhotoStill_NightWindow.png", 64, 96, sub_night),
    ("Act2_PhotoSubject_RoundWindow_64x64.png", "Act2_PhotoStill_RoundWindow.png", 64, 64, sub_round_window),
    ("Act2_PhotoSubject_DessertCart_96x64.png", "Act2_PhotoStill_DessertCart.png", 96, 64, sub_dessert_cart),
]

OLD_SUBJECTS = [
    "Act2_PhotoSubject_LatteArt_64x48.png",
    "Act2_PhotoSubject_Macaron_48x64.png",
    "Act2_PhotoSubject_DonutWall_64x48.png",
    "Act2_PhotoSubject_NeonCafe_64x48.png",
    "Act2_PhotoSubject_Terrace_64x48.png",
    "Act2_PhotoSubject_MirrorRoom_48x64.png",
    "Act2_PhotoSubject_PlantCafe_48x64.png",
    "Act2_PhotoSubject_NightWindow_48x64.png",
    "Act2_PhotoSubject_RoundWindow_48x48.png",
    "Act2_PhotoSubject_DessertCart_64x48.png",
]


if __name__ == "__main__":
    for sheet, still, w, h, fn in SUBJECTS:
        tiles = [lambda f=f, func=fn: func(f) for f in range(4)]
        build_sheet(tiles, os.path.join(OUT, sheet), w, h)
        frame = 1 if ("Neon" in sheet or "Night" in sheet) else 0
        fn(frame).save(os.path.join(OUT, still))
        print("still", still)

    for old in OLD_SUBJECTS:
        path = os.path.join(OUT, old)
        if os.path.exists(path):
            os.remove(path)
            print("removed obsolete", old)
            meta = path + ".meta"
            # leave .meta for Unity to clean; don't manually edit/delete per AGENTS.md
            _ = meta

    print(f"done subjects={len(SUBJECTS)}")
