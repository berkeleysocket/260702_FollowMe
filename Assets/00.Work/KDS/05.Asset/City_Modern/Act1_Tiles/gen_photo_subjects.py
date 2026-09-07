"""Act1 photo subjects — higher quality + 1px dark outline."""
from PIL import Image
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act1_Tiles"
os.makedirs(OUT, exist_ok=True)

CLR = (0, 0, 0, 0)
OUTLINE = (12, 10, 14, 255)

SKIN = (236, 204, 176, 255)
SKIN_S = (210, 168, 140, 255)
HAIR_A = (52, 38, 34, 255)
HAIR_B = (90, 48, 40, 255)
HAIR_C = (40, 44, 58, 255)

SHIRT_A = (86, 148, 220, 255)
SHIRT_AH = (130, 178, 235, 255)
SHIRT_B = (228, 92, 128, 255)
SHIRT_BH = (245, 140, 168, 255)
SHIRT_C = (64, 176, 132, 255)
SHIRT_CH = (110, 205, 165, 255)
PANTS = (58, 54, 68, 255)
PANTS_H = (78, 74, 90, 255)
SHOE = (36, 32, 34, 255)

GUITAR = (196, 132, 68, 255)
GUITAR_H = (220, 168, 100, 255)
GUITAR_D = (128, 76, 38, 255)
STRING = (230, 210, 170, 255)

MIC = (108, 110, 118, 255)
MIC_H = (180, 184, 192, 255)
DRUM = (168, 118, 72, 255)
DRUM_H = (200, 155, 105, 255)
DRUM_SKIN = (240, 232, 214, 255)

AMP = (42, 40, 52, 255)
AMP_H = (70, 68, 84, 255)
CASE = (150, 108, 68, 255)
COIN = (240, 196, 70, 255)

NEON_P = (255, 96, 205, 255)
NEON_C = (90, 230, 255, 255)
NEON_Y = (255, 232, 96, 255)
NEON_G = (120, 255, 160, 255)
BOARD = (28, 26, 38, 255)
BOARD_H = (48, 46, 64, 255)
METAL = (150, 156, 168, 255)
METAL_H = (200, 206, 218, 255)
METAL_D = (90, 94, 104, 255)

WOOD = (148, 104, 64, 255)
WOOD_H = (178, 132, 88, 255)
SKY_T = (168, 204, 238, 180)
SKY_B = (120, 150, 190, 120)
CITY = (70, 82, 104, 255)
CITY_D = (48, 56, 74, 255)
WIN = (255, 220, 120, 255)
CHERRY = (255, 170, 196, 255)
CHERRY_H = (255, 210, 225, 255)
LEAF = (110, 170, 90, 255)
WHITE = (246, 246, 250, 255)
SHADOW = (20, 18, 22, 90)


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
    """Draw solid subject, outline it, then optional soft backdrop underneath."""
    solid = draw_fn()
    outlined = add_outline(solid)
    if backdrop_fn is None:
        return outlined
    back = backdrop_fn()
    back.paste(outlined, (0, 0), outlined)
    return back


def draw_note(img, x, y, color):
    px(img, x, y, color)
    px(img, x + 1, y, color)
    px(img, x, y + 1, color)
    px(img, x + 2, y - 1, color)
    px(img, x + 2, y - 2, color)
    px(img, x + 3, y - 2, color)


def draw_person(img, ox, oy, shirt, shirt_hi, hair, frame=0, instrument="none"):
    bob = frame % 2
    head_y = oy - 23 + bob
    # shadow under feet
    rect(img, ox + 2, oy - 1, ox + 12, oy + 1, SHADOW)
    # shoes
    rect(img, ox + 2, oy - 2, ox + 6, oy, SHOE)
    rect(img, ox + 7, oy - 2, ox + 11, oy, SHOE)
    # legs
    shade_rect(img, ox + 3, oy - 9, ox + 6, oy - 2, PANTS, PANTS_H, SHOE)
    shade_rect(img, ox + 7, oy - 9, ox + 10, oy - 2, PANTS, PANTS_H, SHOE)
    # torso
    shade_rect(img, ox + 2, oy - 17, ox + 11, oy - 9, shirt, shirt_hi, PANTS)
    # head
    oval(img, ox + 6, head_y, 4, 4, SKIN)
    px(img, ox + 4, head_y + 1, SKIN_S)
    px(img, ox + 8, head_y + 1, SKIN_S)
    # eyes
    px(img, ox + 5, head_y, OUTLINE)
    px(img, ox + 8, head_y, OUTLINE)
    # hair
    rect(img, ox + 2, head_y - 5, ox + 11, head_y - 1, hair)
    px(img, ox + 3, head_y - 6, hair)
    px(img, ox + 7, head_y - 6, hair)
    px(img, ox + 9, head_y - 5, hair)

    arm_y = oy - 15 + (0 if frame % 2 == 0 else 1)
    if instrument == "guitar":
        # body of guitar
        oval(img, ox + 15, oy - 11, 7, 5, GUITAR)
        oval(img, ox + 15, oy - 11, 5, 3, GUITAR_H)
        oval(img, ox + 14, oy - 11, 2, 2, GUITAR_D)
        # neck
        shade_rect(img, ox + 19, oy - 20, ox + 21, oy - 11, GUITAR_D, GUITAR, GUITAR_D)
        hline(img, ox + 19, ox + 20, oy - 19, STRING)
        hline(img, ox + 19, ox + 20, oy - 17, STRING)
        # arm strum
        rect(img, ox + 9, arm_y, ox + 17, arm_y + 2, SKIN)
        px(img, ox + 16, arm_y + 1, SKIN_S)
    elif instrument == "mic":
        shade_rect(img, ox + 10, oy - 21, ox + 12, oy - 7, MIC, MIC_H, METAL_D)
        oval(img, ox + 11, oy - 22, 3, 2, MIC_H)
        oval(img, ox + 11, oy - 22, 2, 1, WHITE)
        rect(img, ox + 7, arm_y - 1, ox + 12, arm_y + 1, SKIN)
        # open mouth pulse
        if frame % 2 == 0:
            px(img, ox + 6, head_y + 2, OUTLINE)
    elif instrument == "drum":
        shade_rect(img, ox - 1, oy - 11, ox + 14, oy - 4, DRUM, DRUM_H, GUITAR_D)
        shade_rect(img, ox, oy - 12, ox + 13, oy - 10, DRUM_SKIN, WHITE, DRUM)
        stick = -2 if frame % 2 == 0 else 2
        shade_rect(img, ox + 4 + stick, oy - 19, ox + 6 + stick, oy - 11, METAL, METAL_H, METAL_D)
        rect(img, ox + 8, arm_y, ox + 12, arm_y + 2, SKIN)
    else:
        rect(img, ox + 10, oy - 14, ox + 13, oy - 10, SKIN)


def draw_neon_letter(img, x, y, letter, on_color, on):
    c = on_color if on else (on_color[0] // 3, on_color[1] // 3, on_color[2] // 3, 255)
    if letter == "O":
        shade_rect(img, x, y, x + 7, y + 12, c, WHITE if on else c, OUTLINE)
        rect(img, x + 2, y + 3, x + 5, y + 9, BOARD)
    elif letter == "P":
        shade_rect(img, x, y, x + 7, y + 12, c, WHITE if on else c, OUTLINE)
        rect(img, x + 2, y + 2, x + 5, y + 6, BOARD)
        rect(img, x + 2, y + 7, x + 6, y + 12, BOARD)
    elif letter == "E":
        shade_rect(img, x, y, x + 7, y + 12, c, WHITE if on else c, OUTLINE)
        rect(img, x + 2, y + 2, x + 6, y + 4, BOARD)
        rect(img, x + 2, y + 5, x + 5, y + 7, BOARD)
        rect(img, x + 2, y + 8, x + 6, y + 10, BOARD)
    elif letter == "N":
        shade_rect(img, x, y, x + 7, y + 12, c, WHITE if on else c, OUTLINE)
        rect(img, x + 2, y + 2, x + 3, y + 10, BOARD)
        rect(img, x + 4, y + 2, x + 5, y + 10, BOARD)


def tile_busking(frame):
    def draw_solid():
        img = new(64, 48)
        # amp
        shade_rect(img, 1, 32, 14, 45, AMP, AMP_H, OUTLINE)
        rect(img, 3, 34, 12, 42, (20, 18, 28, 255))
        for yy in (36, 38, 40):
            hline(img, 4, 10, yy, (60, 70, 80, 255))
        px(img, 5, 35, NEON_C)
        px(img, 9, 35, NEON_P)

        # tip jar / guitar case lid
        shade_rect(img, 50, 37, 60, 45, CASE, WOOD_H, GUITAR_D)
        oval(img, 55, 37, 5, 2, WOOD_H)
        px(img, 53, 36, COIN)
        px(img, 56, 35, COIN)

        draw_person(img, 15, 45, SHIRT_A, SHIRT_AH, HAIR_A, frame, "guitar")
        draw_person(img, 29, 45, SHIRT_B, SHIRT_BH, HAIR_B, frame + 1, "mic")
        draw_person(img, 43, 45, SHIRT_C, SHIRT_CH, HAIR_C, frame, "drum")

        if frame % 2 == 0:
            draw_note(img, 26, 10, NEON_Y)
            draw_note(img, 38, 6, NEON_P)
            draw_note(img, 48, 12, NEON_C)
        else:
            draw_note(img, 30, 8, NEON_C)
            draw_note(img, 42, 11, NEON_Y)
            draw_note(img, 22, 14, NEON_P)
        return img

    def draw_soft():
        img = new(64, 48)
        rect(img, 6, 45, 58, 47, SHADOW)
        return img

    return with_backdrop_then_outline(draw_solid, draw_soft)


def tile_neon(frame):
    def draw_solid():
        img = new(48, 64)
        shade_rect(img, 16, 58, 32, 63, METAL_D, METAL, OUTLINE)
        shade_rect(img, 22, 30, 26, 58, METAL, METAL_H, METAL_D)
        px(img, 23, 36, METAL_D)
        px(img, 24, 46, METAL_D)
        px(img, 23, 52, METAL_D)

        shade_rect(img, 3, 2, 45, 30, BOARD_H, METAL_H, OUTLINE)
        shade_rect(img, 5, 4, 43, 28, BOARD, BOARD_H, OUTLINE)
        rect(img, 7, 6, 41, 26, (18, 16, 28, 255))

        colors = [NEON_P, NEON_C, NEON_Y, NEON_G]
        for i, ch in enumerate("OPEN"):
            on = ((frame + i) % 4) != 3
            draw_neon_letter(img, 9 + i * 8, 9, ch, colors[i], on)

        shade_rect(img, 20, 28, 28, 32, METAL, METAL_H, METAL_D)
        return img

    def draw_soft():
        img = new(48, 64)
        glow = [NEON_P, NEON_C, NEON_Y, NEON_G][frame % 4]
        if frame % 2 == 0:
            for x in range(6, 42):
                px(img, x, 1, (glow[0], glow[1], glow[2], 80))
                px(img, x, 30, (glow[0], glow[1], glow[2], 60))
        return img

    return with_backdrop_then_outline(draw_solid, draw_soft)


def tile_viewpoint(frame):
    off = frame % 2

    def draw_solid():
        img = new(64, 48)
        heights = [8, 14, 11, 18, 12, 16, 9, 15, 13]
        for i, h in enumerate(heights):
            x0 = 2 + i * 7
            col = CITY if i % 2 == 0 else CITY_D
            shade_rect(img, x0, 22 - h, x0 + 6, 22, col, METAL, OUTLINE)
            if i % 2 == 0:
                px(img, x0 + 2, 22 - h + 3, WIN)
                px(img, x0 + 4, 22 - h + 6, WIN)
            if i % 3 == 0:
                px(img, x0 + 1, 22 - h + 2, NEON_C)

        shade_rect(img, 1, 28, 63, 31, METAL, METAL_H, METAL_D)
        for x in range(4, 60, 5):
            shade_rect(img, x, 31, x + 2, 41, METAL, METAL_H, METAL_D)
        shade_rect(img, 0, 40, 64, 44, WOOD, WOOD_H, GUITAR_D)

        shade_rect(img, 48, 22, 52, 41, WOOD, WOOD_H, GUITAR_D)
        blooms = [
            (0, 0), (5, -2), (-5, 1), (3, 4), (-3, -4), (8, 1), (-7, -1),
            (2, -6), (6, 3), (-1, 5), (9, -3), (-6, 3),
        ]
        for dx, dy in blooms:
            oval(img, 50 + dx, 14 + dy + off, 3, 2, CHERRY)
            px(img, 50 + dx, 13 + dy + off, CHERRY_H)
        px(img, 47, 24, LEAF)
        px(img, 53, 26, LEAF)

        shade_rect(img, 18, 24, 22, 41, METAL, METAL_H, METAL_D)
        by = 18 - off
        oval(img, 16, by + 2, 4, 3, BOARD)
        oval(img, 24, by + 2, 4, 3, BOARD)
        oval(img, 16, by + 2, 2, 2, NEON_C if frame % 2 == 0 else NEON_P)
        oval(img, 24, by + 2, 2, 2, NEON_C if frame % 2 == 0 else NEON_P)
        shade_rect(img, 14, by, 26, by + 2, METAL, METAL_H, METAL_D)

        shade_rect(img, 34, 22, 42, 28, WHITE, WHITE, OUTLINE)
        px(img, 36, 24, NEON_P)
        px(img, 38, 25, NEON_C)
        return img

    def draw_soft():
        img = new(64, 48)
        for y in range(0, 20):
            t = y / 19.0
            a = int(SKY_T[3] * (1 - t) + SKY_B[3] * t)
            c = (
                int(SKY_T[0] * (1 - t) + SKY_B[0] * t),
                int(SKY_T[1] * (1 - t) + SKY_B[1] * t),
                int(SKY_T[2] * (1 - t) + SKY_B[2] * t),
                a,
            )
            hline(img, 0, 63, y, c)
        rect(img, 0, 44, 64, 47, SHADOW)
        return img

    return with_backdrop_then_outline(draw_solid, draw_soft)


def tile_camera_prop(frame):
    img = new(32, 32)
    bob = frame % 2
    y = 7 - bob
    # body
    shade_rect(img, 5, y + 8, 27, y + 22, BOARD, BOARD_H, OUTLINE)
    shade_rect(img, 6, y + 9, 26, y + 21, (36, 34, 48, 255), AMP_H, OUTLINE)
    # lens rings
    oval(img, 16, y + 15, 6, 6, METAL)
    oval(img, 16, y + 15, 4, 4, METAL_D)
    oval(img, 16, y + 15, 2, 2, NEON_C if frame % 2 == 0 else NEON_P)
    # flash
    shade_rect(img, 20, y + 4, 25, y + 9, WHITE, WHITE, METAL)
    # grip / dial
    shade_rect(img, 7, y + 5, 14, y + 9, METAL_D, METAL, OUTLINE)
    px(img, 9, y + 11, COIN)
    return add_outline(img)


def build_sheet(tiles, path, cell_w, cell_h, cols):
    rows = (len(tiles) + cols - 1) // cols
    sheet = Image.new("RGBA", (cols * cell_w, rows * cell_h), CLR)
    for i, fn in enumerate(tiles):
        c, r = i % cols, i // cols
        sheet.paste(fn(), (c * cell_w, r * cell_h))
    sheet.save(path)
    print("saved", path, sheet.size, len(tiles))


busking = [lambda f=f: tile_busking(f) for f in range(4)]
neon = [lambda f=f: tile_neon(f) for f in range(4)]
view = [lambda f=f: tile_viewpoint(f) for f in range(4)]
cam = [lambda f=f: tile_camera_prop(f) for f in range(2)]

build_sheet(busking, f"{OUT}/Act1_PhotoSubject_Busking_64x48.png", 64, 48, 4)
build_sheet(neon, f"{OUT}/Act1_PhotoSubject_Neon_48x64.png", 48, 64, 4)
build_sheet(view, f"{OUT}/Act1_PhotoSubject_Viewpoint_64x48.png", 64, 48, 4)
build_sheet(cam, f"{OUT}/Act1_PhotoCamera_32x32.png", 32, 32, 2)

tile_busking(0).save(f"{OUT}/Act1_PhotoStill_Busking.png")
tile_neon(1).save(f"{OUT}/Act1_PhotoStill_Neon.png")
tile_viewpoint(0).save(f"{OUT}/Act1_PhotoStill_Viewpoint.png")
print("stills saved")
