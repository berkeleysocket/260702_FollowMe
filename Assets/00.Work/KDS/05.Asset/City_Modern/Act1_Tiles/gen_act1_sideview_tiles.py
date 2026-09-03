"""Act1 side-view brick terrain — higher-quality pixel art (32x32)."""
from PIL import Image, ImageDraw

T = 32
OUT_DIR = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act1_Tiles"

# 16-color Act1 spring sidewalk palette
CLR = (0, 0, 0, 0)
OUT = (34, 30, 28, 255)
SHADOW = (58, 52, 46, 255)
MORTAR = (140, 132, 120, 255)
BRICK_A = (188, 178, 164, 255)
BRICK_B = (172, 162, 148, 255)
BRICK_HI = (210, 202, 188, 255)
CURB = (236, 228, 214, 255)
CURB_SH = (200, 192, 178, 255)
FACE_A = (108, 100, 90, 255)
FACE_B = (88, 80, 72, 255)
FACE_D = (66, 60, 54, 255)
CLIFF_A = (78, 72, 64, 255)
CLIFF_B = (56, 50, 44, 255)
GRASS_D = (64, 118, 56, 255)
GRASS_M = (96, 158, 78, 255)
GRASS_L = (148, 210, 118, 255)
PETAL = (255, 168, 192, 255)

SURF_TOP = 2
SURF_BOT = 11
CURB_Y = 11
BODY_TOP = 12

# grass tuft patterns (dx, dy, color) relative to y_top
TUFTS = [
    [(0, 0, GRASS_D), (0, 1, GRASS_M), (1, 2, GRASS_L), (-1, 1, GRASS_M)],
    [(0, 0, GRASS_D), (1, 1, GRASS_L), (0, 2, GRASS_M)],
    [(0, 0, GRASS_D), (-1, 1, GRASS_L), (1, 1, GRASS_M), (0, 3, GRASS_L)],
]


def new_tile():
    return Image.new("RGBA", (T, T), CLR)


def px(img, x, y, c):
    if 0 <= x < T and 0 <= y < T:
        img.putpixel((x, y), c)


def in_rect(x, y, x0, y0, w, h):
    return x0 <= x < x0 + w and y0 <= y < y0 + h


def brick_at_surface(x, y, y0, variant=0):
    """7x3 brick grid on walk surface."""
    row = (y - y0) // 4
    by = y0 + row * 4
    off = 0 if row % 2 == 0 else 4
    bx = off + ((x - off) // 8) * 8
    lx, ly = x - bx, y - by
    if lx < 0 or lx >= 8 or ly >= 4:
        return MORTAR
    if lx == 0 or ly == 0 or ly == 3:
        return MORTAR
    base = BRICK_A if ((bx // 8) + row + variant) % 2 == 0 else BRICK_B
    if lx == 1 and ly == 1:
        return BRICK_HI
    if lx == 6 or ly == 2:
        return SHADOW if base == BRICK_A else FACE_D
    return base


def brick_at_face(x, y, y0, x0=0, x1=T, variant=0):
    """6x5 brick grid on vertical face."""
    row = (y - y0) // 5
    by = y0 + row * 5
    off = 0 if row % 2 == 0 else 3
    bx = x0 + off + ((x - x0 - off) // 7) * 7
    lx, ly = x - bx, y - by
    if x < x0 + 1 or x >= x1 - 1:
        return None
    if lx < 0 or lx >= 7 or ly >= 5:
        return MORTAR
    if lx == 0 or ly == 0 or ly == 4:
        return MORTAR
    base = FACE_A if ((bx // 7) + row + variant) % 2 == 0 else FACE_B
    if lx == 1 and ly == 1:
        return BRICK_HI if row == 0 else base
    if lx >= 5 or ly >= 3:
        return FACE_D
    return base


def draw_grass_row(img, x0, x1, y_top, seed=0):
    pos = x0 + 2
    ti = 0
    while pos < x1 - 2:
        tuft = TUFTS[(seed + ti) % len(TUFTS)]
        for dx, dy, col in tuft:
            px(img, pos + dx, y_top + dy, col)
        if (seed + ti) % 5 == 0:
            px(img, pos + 2, y_top + 1, PETAL)
        pos += 5 + (ti % 3)
        ti += 1


def draw_surface_block(img, x0, x1, y0=SURF_TOP, y1=SURF_BOT, variant=0, grass=True):
    if grass:
        draw_grass_row(img, x0, x1, y0 - 1, seed=variant * 3)
    for y in range(y0, y1):
        for x in range(x0, x1):
            px(img, x, y, brick_at_surface(x, y, y0, variant))
    for x in range(x0, x1):
        px(img, x, CURB_Y, CURB)
        px(img, x, CURB_Y + 1, CURB_SH if x % 2 == 0 else MORTAR)


def draw_face_block(img, x0, x1, y0=BODY_TOP, y1=T, variant=0, left_edge=False, right_edge=False):
    for y in range(y0, y1):
        for x in range(x0, x1):
            if left_edge and x == x0:
                px(img, x, y, OUT)
                continue
            if right_edge and x == x1 - 1:
                px(img, x, y, OUT)
                continue
            col = brick_at_face(x, y, y0, x0, x1, variant)
            if col:
                px(img, x, y, col)
    if left_edge:
        for y in range(y0, y1):
            px(img, x0, y, OUT)
    if right_edge:
        for y in range(y0, y1):
            px(img, x1 - 1, y, OUT)


def draw_cliff(img, x0, x1, y0=BODY_TOP, y1=T):
    for y in range(y0, y1):
        for x in range(x0, x1):
            row = (y - y0) // 4
            col = CLIFF_A if (row + x) % 2 == 0 else CLIFF_B
            if x == x0:
                px(img, x, y, OUT)
            elif (x + y) % 5 == 0:
                px(img, x, y, FACE_D)
            else:
                px(img, x, y, col)


def tile_ground_fill(variant=0):
    img = new_tile()
    draw_surface_block(img, 0, T, variant=variant)
    draw_face_block(img, 0, T, variant=variant)
    return img


def tile_ground_left():
    img = new_tile()
    draw_cliff(img, 0, 6)
    draw_surface_block(img, 5, T, variant=1)
    draw_face_block(img, 5, T, variant=1, left_edge=True)
    for y in range(SURF_TOP, BODY_TOP):
        px(img, 5, y, OUT)
    return img


def tile_ground_right():
    return tile_ground_left().transpose(Image.FLIP_LEFT_RIGHT)


def tile_vertical_fill():
    img = new_tile()
    draw_face_block(img, 0, T, y0=0, variant=2)
    for x in range(T):
        px(img, x, 0, OUT)
    return img


def tile_column():
    img = new_tile()
    draw_face_block(img, 11, 21, y0=0, y1=T, variant=3, left_edge=True, right_edge=True)
    draw_surface_block(img, 11, 21, y0=0, y1=SURF_BOT, variant=3, grass=True)
    return img


def surface_y_at(x, mode, variant=0):
    if mode == "steep_up":
        return int(23 - x * 17 / 31)
    if mode == "steep_down":
        return int(6 + x * 17 / 31)
    if mode == "gentle_up":
        return int(23 - x * 9 / 31)
    if mode == "gentle_down":
        return int(14 + x * 9 / 31)
    return 23


def tile_slope(mode):
    img = new_tile()
    sy_map = [surface_y_at(x, mode) for x in range(T)]

    for x in range(T):
        sy = sy_map[x]
        grass_y = sy - SURF_BOT + SURF_TOP - 1
        draw_grass_row(img, max(0, x - 1), min(T, x + 2), grass_y, seed=x)

        for y in range(T):
            if y < sy - SURF_BOT + SURF_TOP:
                continue
            if y < sy - 2:
                rel_y = y - (sy - SURF_BOT + SURF_TOP)
                col = brick_at_surface(x, rel_y + SURF_TOP, SURF_TOP, variant=x % 3)
                px(img, x, y, col)
            elif y < sy:
                px(img, x, y, CURB if y == sy - 2 else CURB_SH)
            elif y == sy:
                px(img, x, y, OUT)
            else:
                col = brick_at_face(x, y, sy + 1, 0, T, variant=x % 2)
                px(img, x, y, col if col else FACE_A)

    return img


def tile_step_up():
    img = new_tile()
    draw_surface_block(img, 0, 17, variant=0)
    draw_face_block(img, 0, 17, variant=0, right_edge=True)
    # raised block right
    draw_surface_block(img, 16, T, y0=0, y1=8, variant=1)
    for y in range(8, T):
        for x in range(16, T):
            col = brick_at_face(x, y, 8, 16, T, 1)
            px(img, x, y, col if col else FACE_B)
    for y in range(0, T):
        px(img, 16, y, OUT)
    return img


def tile_step_down():
    return tile_step_up().transpose(Image.FLIP_LEFT_RIGHT)


def tile_outer_corner():
    img = tile_ground_fill(1)
    for y in range(0, 13):
        for x in range(21, T):
            if y < SURF_TOP:
                draw_grass_row(img, x, x + 1, y, seed=x)
            elif y < SURF_BOT:
                px(img, x, y, brick_at_surface(x, y, SURF_TOP, 2))
            elif y <= CURB_Y + 1:
                px(img, x, y, CURB if y == CURB_Y else CURB_SH)
            else:
                col = brick_at_face(x, y, BODY_TOP, 21, T, 1)
                px(img, x, y, col if col else FACE_A)
    return img


def tile_inner_corner():
    img = tile_ground_fill(0)
    for y in range(0, 14):
        for x in range(19, T):
            px(img, x, y, CLR)
    for y in range(0, 14):
        px(img, 18, y, OUT)
    for x in range(18, T):
        px(img, x, 13, OUT)
    # notch shadow inside
    for x in range(19, T):
        px(img, x, 12, SHADOW)
    return img


def tile_platform(cap="center"):
    img = new_tile()
    if cap == "left":
        x0, x1 = 0, T - 6
        draw_cliff(img, 0, 5, y0=15, y1=T)
    elif cap == "right":
        x0, x1 = 6, T
        draw_cliff(img, T - 5, T, y0=15, y1=T)
    else:
        x0, x1 = 0, T

    plat_top = 6
    plat_bot = 15
    draw_grass_row(img, x0 + 2, x1 - 2, plat_top, seed=4)
    for y in range(plat_top + 1, plat_bot):
        for x in range(x0 + 1, x1 - 1):
            px(img, x, y, brick_at_surface(x, y, plat_top + 1, 2))
    for x in range(x0, x1):
        px(img, x, plat_bot, CURB)
        px(img, x, plat_bot + 1, SHADOW)
    for y in range(plat_bot + 2, plat_bot + 8):
        for x in range(x0 + 2, x1 - 2):
            col = brick_at_face(x, y, plat_bot + 2, x0, x1, 3)
            if col and col != MORTAR:
                px(img, x, y, col)
    # hanging shadow
    d = ImageDraw.Draw(img)
    d.line([(x0 + 2, plat_bot + 8), (x1 - 3, plat_bot + 8)], fill=OUT)
    return img


def tile_underside():
    img = new_tile()
    d = ImageDraw.Draw(img)
    d.line([(4, 8), (T - 5, 8)], fill=OUT)
    for x in range(5, T - 5):
        px(img, x, 9, CURB_SH)
    for y in range(10, 16):
        for x in range(6, T - 6):
            if (x + y) % 2:
                px(img, x, y, FACE_D)
            else:
                px(img, x, y, CLIFF_B)
    return img


def tile_floating_slab():
    img = new_tile()
    x0, x1 = 5, T - 5
    y0 = 13
    draw_grass_row(img, x0, x1, y0, seed=7)
    for y in range(y0 + 1, y0 + 7):
        for x in range(x0, x1):
            px(img, x, y, brick_at_surface(x, y, y0 + 1, 1))
    for x in range(x0, x1):
        px(img, x, y0 + 7, CURB)
        px(img, x, y0 + 8, SHADOW)
    return img


def build_sheet(tiles, path, cols=4):
    rows = (len(tiles) + cols - 1) // cols
    sheet = Image.new("RGBA", (cols * T, rows * T), CLR)
    for i, fn in enumerate(tiles):
        c, r = i % cols, i // cols
        sheet.paste(fn(), (c * T, r * T))
    sheet.save(path)
    print("saved", path, len(tiles), "tiles")


ground_tiles = [
    lambda: tile_ground_fill(0),
    lambda: tile_ground_fill(1),
    lambda: tile_ground_fill(2),
    tile_ground_left,
    tile_ground_right,
    tile_vertical_fill,
    tile_column,
    lambda: tile_ground_fill(3),
]

slope_tiles = [
    lambda: tile_slope("steep_up"),
    lambda: tile_slope("steep_down"),
    lambda: tile_slope("steep_up").transpose(Image.FLIP_LEFT_RIGHT),
    lambda: tile_slope("steep_down").transpose(Image.FLIP_LEFT_RIGHT),
    lambda: tile_slope("gentle_up"),
    lambda: tile_slope("gentle_down"),
    tile_step_up,
    tile_step_down,
    tile_outer_corner,
    tile_inner_corner,
    lambda: tile_platform("left"),
    lambda: tile_platform("center"),
    lambda: tile_platform("right"),
    tile_underside,
    tile_floating_slab,
]

build_sheet(ground_tiles, f"{OUT_DIR}/Act1_Brick_Ground_32x32.png")
build_sheet(slope_tiles, f"{OUT_DIR}/Act1_Brick_Slope_32x32.png", cols=4)
