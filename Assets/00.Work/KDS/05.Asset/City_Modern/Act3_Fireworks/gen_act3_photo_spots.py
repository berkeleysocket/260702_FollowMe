"""Act3 fireworks festival photo subjects — night palette, dense fill like Act2.

Landscape 96x64 · Portrait 64x96
Pipeline: shade_rect + add_outline(solid_alpha=200) + soft night backdrop.
People are slightly larger than Act1 draw_person (same as Act2).
"""
from PIL import Image
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act3_Fireworks"
os.makedirs(OUT, exist_ok=True)

CLR = (0, 0, 0, 0)
OUTLINE = (10, 8, 18, 255)

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

# Night / fireworks palette
NIGHT = (18, 22, 48, 255)
NIGHT_H = (34, 40, 72, 255)
NIGHT_D = (10, 12, 28, 255)
SKY_TOP = (22, 18, 52, 210)
SKY_BOT = (12, 24, 58, 120)
PURPLE = (88, 48, 140, 255)
PURPLE_H = (130, 80, 190, 255)
FW_PINK = (255, 96, 180, 255)
FW_PINK_H = (255, 170, 210, 255)
FW_GOLD = (255, 210, 90, 255)
FW_GOLD_H = (255, 240, 170, 255)
FW_CYAN = (90, 230, 255, 255)
FW_CYAN_H = (180, 245, 255, 255)
FW_ORANGE = (255, 140, 60, 255)
FW_RED = (255, 70, 90, 255)
FW_VIOLET = (180, 100, 255, 255)
RIVER = (28, 48, 88, 255)
RIVER_H = (48, 78, 120, 255)
RIVER_D = (16, 28, 52, 255)
BRIDGE = (70, 78, 98, 255)
BRIDGE_H = (110, 120, 145, 255)
BRIDGE_D = (42, 48, 64, 255)
CITY = (40, 48, 72, 255)
CITY_D = (28, 34, 54, 255)
WIN = (255, 220, 120, 255)
NEON_P = (255, 96, 205, 255)
NEON_C = (90, 230, 255, 255)
NEON_Y = (255, 232, 96, 255)
WHITE = (246, 246, 250, 255)
BLACK = (28, 26, 34, 255)
GRAY = (120, 118, 128, 255)
GRAY_H = (170, 168, 178, 255)
METAL = (150, 156, 168, 255)
METAL_H = (200, 206, 218, 255)
METAL_D = (90, 94, 104, 255)
WOOD = (148, 104, 64, 255)
WOOD_H = (178, 132, 88, 255)
WOOD_D = (110, 72, 42, 255)
BOARD = (28, 26, 38, 255)
BOARD_H = (48, 46, 64, 255)
WARM = (255, 210, 130, 255)
WARM_H = (255, 230, 180, 255)
GOLD = (230, 180, 70, 255)
CREAM = (248, 240, 228, 255)
SHADOW = (8, 10, 20, 110)
GLOW_P = (255, 96, 180, 55)
GLOW_C = (90, 230, 255, 50)
GLOW_G = (255, 210, 90, 50)


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


def night_sky(w, h, y_max=28, top=SKY_TOP, bot=SKY_BOT):
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
    # Act2급 보빙: 프레임마다 2~3px 상하 + 다리 교대
    bob = [0, 2, 3, 1][frame % 4]
    arm_swing = [0, 2, 3, 1][frame % 4]
    head_y = oy - 28 + bob
    rect(img, ox + 2, oy - 1, ox + 15, oy + 1, SHADOW)
    if frame % 2 == 0:
        rect(img, ox + 2, oy - 4, ox + 7, oy, SHOE)
        rect(img, ox + 9, oy - 2, ox + 14, oy, SHOE)
    else:
        rect(img, ox + 2, oy - 2, ox + 7, oy, SHOE)
        rect(img, ox + 9, oy - 4, ox + 14, oy, SHOE)
    shade_rect(img, ox + 3, oy - 11, ox + 7, oy - 3, PANTS, PANTS_H, SHOE)
    shade_rect(img, ox + 9, oy - 11, ox + 13, oy - 3, PANTS, PANTS_H, SHOE)
    shade_rect(img, ox + 2, oy - 21 + (bob // 2), ox + 14, oy - 11, shirt, shirt_hi, PANTS)
    oval(img, ox + 8, head_y, 5, 5, SKIN)
    px(img, ox + 5, head_y + 1, SKIN_S)
    px(img, ox + 11, head_y + 1, SKIN_S)
    px(img, ox + 6, head_y, OUTLINE)
    px(img, ox + 10, head_y, OUTLINE)
    if frame % 4 == 3:
        hline(img, ox + 5, ox + 7, head_y, OUTLINE)
        hline(img, ox + 9, ox + 11, head_y, OUTLINE)
    rect(img, ox + 3, head_y - 6, ox + 14, head_y - 1, hair)
    px(img, ox + 4, head_y - 7, hair)
    px(img, ox + 8, head_y - 7, hair)
    px(img, ox + 11, head_y - 6, hair)
    px(img, ox + 13, head_y - 5, hair)

    arm_y = oy - 18 + arm_swing
    if pose == "phone":
        lift = [0, -3, -6, -2][frame % 4]
        shade_rect(img, ox + 13, arm_y - 8 + lift, ox + 20, arm_y + lift, BLACK, GRAY, BLACK)
        flash = [FW_CYAN, FW_PINK, WHITE, FW_GOLD][frame % 4]
        px(img, ox + 15, arm_y - 5 + lift, flash)
        px(img, ox + 16, arm_y - 4 + lift, WHITE)
        px(img, ox + 17, arm_y - 5 + lift, flash)
        rect(img, ox + 11, arm_y - 3 + lift, ox + 15, arm_y + 1 + lift, SKIN)
        rect(img, ox - 1, oy - 17 + bob // 2, ox + 2, oy - 12 + bob // 2, SKIN)
        if frame % 4 in (1, 2):
            px(img, ox + 7, head_y + 1, WARM_H)
            px(img, ox + 9, head_y + 2, WARM)
            px(img, ox + 14, arm_y - 9 + lift, WHITE)
            px(img, ox + 18, arm_y - 10 + lift, WARM_H)
            if frame % 4 == 2:
                draw_spark(img, ox + 19, arm_y - 12 + lift, flash)
    elif pose == "camera":
        lift = [0, -2, -5, -1][frame % 4]
        shade_rect(img, ox + 13, arm_y - 4 + lift, ox + 23, arm_y + 4 + lift, BOARD, BOARD_H, OUTLINE)
        oval(img, ox + 18, arm_y + lift, 4, 4, METAL)
        oval(img, ox + 18, arm_y + lift, 2, 2, METAL_D)
        oval(img, ox + 18, arm_y + lift, 1, 1, [FW_CYAN, FW_GOLD, WHITE, FW_PINK][frame % 4])
        shade_rect(img, ox + 19, arm_y - 7 + lift, ox + 23, arm_y - 4 + lift, WHITE, WHITE, METAL)
        rect(img, ox + 11, arm_y - 1 + lift, ox + 14, arm_y + 2 + lift, SKIN)
        # shutter pop every other frame
        if frame % 2 == 0:
            px(img, ox + 21, arm_y - 8 + lift, WARM_H)
            px(img, ox + 22, arm_y - 9 + lift, WHITE)
            px(img, ox + 20, arm_y - 10 + lift, WHITE)
            draw_spark(img, ox + 22, arm_y - 11 + lift, WHITE)
        else:
            oval(img, ox + 18, arm_y + lift, 5, 5, (255, 240, 200, 90))
    elif pose == "look_up":
        lift = [0, 2, 4, 1][frame % 4]
        rect(img, ox + 12, arm_y - 12 - lift, ox + 16, arm_y - 2 - lift, SKIN)
        rect(img, ox - 2, arm_y - 10 - lift, ox + 2, arm_y - 1 - lift, SKIN)
        px(img, ox + 7, head_y - 1, WARM_H if frame % 2 == 0 else FW_GOLD)
        if frame % 2 == 0:
            px(img, ox + 8, head_y - 8, FW_PINK_H)
    else:
        wave = [0, 3, 5, 2][frame % 4]
        rect(img, ox + 13, arm_y - wave, ox + 16, arm_y + 3 - wave, SKIN)
        rect(img, ox + 16, arm_y - 1 - wave, ox + 19, arm_y + 1 - wave, SKIN)
        rect(img, ox - 1, oy - 17, ox + 2, oy - 12, SKIN)


def draw_spark(img, x, y, c):
    px(img, x, y, c)
    px(img, x + 1, y, WHITE)
    px(img, x, y - 1, c)
    px(img, x - 1, y, c)
    px(img, x, y + 1, c)


def draw_firework_burst(img, cx, cy, frame, colors, scale=1):
    """Radial burst — launch → bloom → sparkle → fall (프레임 차이가 크게)."""
    phase = frame % 4
    radii = [2 * scale, 5 * scale, 9 * scale, 11 * scale]
    r = radii[phase]
    dirs = (
        (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1),
        (2, -1), (-2, 1), (1, -2), (-1, 2), (2, 0), (-2, 0), (0, -2), (0, 2),
    )
    if phase == 0:
        # 로켓 상승
        vline(img, cx, cy + 2, cy + 8 * scale, FW_GOLD)
        oval(img, cx, cy, 2 * scale, 2 * scale, FW_GOLD_H)
        px(img, cx, cy, WHITE)
        px(img, cx, cy + 9 * scale, (FW_ORANGE[0], FW_ORANGE[1], FW_ORANGE[2], 160))
        return
    for i, (dx, dy) in enumerate(dirs):
        c = colors[i % len(colors)]
        step = r // 3 + 1
        x = cx + dx * step
        y = cy + dy * step
        px(img, x, y, c)
        if phase >= 2:
            px(img, x + dx, y + dy, WHITE if i % 2 == 0 else c)
            draw_spark(img, x + dx, y + dy - 1, c)
        if phase == 3:
            # 낙하 불꽃
            fall = 2 + (i % 3)
            px(img, cx + dx * 2, cy + dy * 2 + fall, (c[0], c[1], c[2], 200))
            px(img, cx + dx * 3, cy + dy * 2 + fall + 2, (c[0], c[1], c[2], 120))
    oval(img, cx, cy, (2 + phase) * scale, (2 + phase) * scale, (colors[0][0], colors[0][1], colors[0][2], 70))
    px(img, cx, cy, WHITE)


def draw_city_skyline(img, y_base, w, frame):
    heights = [8, 14, 10, 18, 12, 16, 9, 15, 11, 17, 10, 13, 8]
    for i, h in enumerate(heights):
        x0 = 1 + i * max(1, (w - 2) // len(heights))
        x1 = min(w - 1, x0 + max(4, (w - 2) // len(heights) - 1))
        col = CITY if i % 2 == 0 else CITY_D
        shade_rect(img, x0, y_base - h, x1, y_base, col, BRIDGE_H, OUTLINE)
        if i % 2 == 0 and (frame + i) % 3 != 0:
            px(img, x0 + 1, y_base - h + 3, WIN)
            px(img, x0 + 3, y_base - h + 6, FW_GOLD if i % 3 == 0 else WIN)
        if i % 4 == 0:
            px(img, x0 + 2, y_base - h + 2, FW_CYAN)


def draw_river(img, y0, y1, w, frame, reflect=True):
    shade_rect(img, 0, y0, w, y1, RIVER, RIVER_H, RIVER_D)
    for x in range(2, w - 2, 6):
        yy = y0 + 2 + ((frame + x) % 3)
        hline(img, x, min(x + 4, w - 2), yy, RIVER_H)
    if reflect:
        cols = [FW_PINK, FW_GOLD, FW_CYAN, FW_VIOLET]
        for i, x in enumerate(range(8, w - 8, 10)):
            c = cols[(i + frame) % 4]
            px(img, x, y0 + 4 + (frame % 2), (c[0], c[1], c[2], 160))
            px(img, x + 1, y0 + 6, (c[0], c[1], c[2], 100))


def draw_bridge_rail(img, y, w, frame):
    shade_rect(img, 0, y, w, y + 4, BRIDGE, BRIDGE_H, BRIDGE_D)
    for x in range(3, w - 2, 6):
        shade_rect(img, x, y + 4, x + 2, y + 14, BRIDGE, BRIDGE_H, BRIDGE_D)
        if (frame + x) % 4 != 3:
            px(img, x, y - 1, FW_GOLD if x % 12 < 6 else FW_CYAN)
            px(img, x + 1, y - 2, WHITE)


def build_sheet(tiles, path, cell_w, cell_h):
    cols = len(tiles)
    sheet = Image.new("RGBA", (cols * cell_w, cell_h), CLR)
    for i, fn in enumerate(tiles):
        sheet.paste(fn(), (i * cell_w, 0))
    sheet.save(path)
    print("sheet", os.path.basename(path), sheet.size)


# ===================== subjects =====================

def sub_bridge_view(frame):
    """96x64 — Han river bridge railing night view."""
    def solid():
        img = new(96, 64)
        draw_city_skyline(img, 22, 96, frame)
        # bridge deck + towers
        shade_rect(img, 8, 10, 14, 36, BRIDGE_D, BRIDGE, BLACK)
        shade_rect(img, 82, 8, 88, 36, BRIDGE_D, BRIDGE, BLACK)
        hline(img, 8, 88, 14, BRIDGE_H)
        for x in range(14, 82, 8):
            vline(img, x, 14, 20, BRIDGE)
        draw_bridge_rail(img, 34, 96, frame)
        draw_river(img, 48, 58, 96, frame)
        # distant fireworks
        draw_firework_burst(img, 28, 10, frame, [FW_PINK, FW_GOLD, FW_PINK_H], 1)
        draw_firework_burst(img, 70, 8, frame + 1, [FW_CYAN, FW_VIOLET, WHITE], 1)
        draw_firework_burst(img, 50, 6, frame + 2, [FW_GOLD, FW_ORANGE, WHITE], 1)
        draw_person(img, 20, 48, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        draw_person(img, 48, 48, SHIRT_A, SHIRT_AH, HAIR_A, frame + 1, "look_up")
        draw_person(img, 68, 48, SHIRT_C, SHIRT_CH, HAIR_C, frame, "camera")
        return img

    def soft():
        img = night_sky(96, 64, 26)
        if frame % 2 == 0:
            for x in range(20, 80, 6):
                px(img, x, 4, GLOW_P if x % 12 < 6 else GLOW_C)
        rect(img, 4, 58, 92, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_firework_peak(frame):
    """64x96 — peak fireworks moment, vertical."""
    def solid():
        img = new(64, 96)
        draw_city_skyline(img, 70, 64, frame)
        draw_river(img, 70, 82, 64, frame)
        # big central burst
        draw_firework_burst(img, 32, 28, frame, [FW_PINK, FW_GOLD, FW_CYAN, FW_VIOLET], 2)
        draw_firework_burst(img, 14, 18, frame + 1, [FW_CYAN, WHITE, FW_GOLD], 1)
        draw_firework_burst(img, 50, 16, frame + 2, [FW_PINK, FW_ORANGE, WHITE], 1)
        draw_firework_burst(img, 22, 42, frame + 3, [FW_GOLD, FW_PINK_H], 1)
        draw_firework_burst(img, 44, 40, frame, [FW_VIOLET, FW_CYAN_H], 1)
        # launch trails
        for i, x in enumerate((18, 32, 46)):
            trail_y = 55 - [0, 4, 8, 4][(frame + i) % 4]
            vline(img, x, trail_y, 68, FW_GOLD if i % 2 == 0 else FW_PINK)
            px(img, x, trail_y - 1, WHITE)
        draw_person(img, 8, 90, SHIRT_B, SHIRT_BH, HAIR_B, frame, "look_up")
        draw_person(img, 36, 90, SHIRT_D, SHIRT_DH, HAIR_D, frame + 1, "phone")
        return img

    def soft():
        img = night_sky(64, 96, 40, (40, 20, 70, 220), (16, 28, 60, 100))
        glow = [FW_PINK, FW_GOLD, FW_CYAN, FW_VIOLET][frame % 4]
        oval(img, 32, 28, 14, 12, (glow[0], glow[1], glow[2], 40 + 15 * (frame % 2)))
        rect(img, 6, 90, 58, 94, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_phone_crowd(frame):
    """96x64 — crowd watching through phones."""
    def solid():
        img = new(96, 64)
        draw_city_skyline(img, 18, 96, frame)
        draw_firework_burst(img, 48, 8, frame, [FW_PINK, FW_GOLD, WHITE], 1)
        draw_firework_burst(img, 20, 6, frame + 2, [FW_CYAN, FW_VIOLET], 1)
        draw_firework_burst(img, 76, 7, frame + 1, [FW_GOLD, FW_ORANGE], 1)
        # dense crowd row
        people = [
            (2, SHIRT_A, SHIRT_AH, HAIR_A, "phone"),
            (18, SHIRT_B, SHIRT_BH, HAIR_B, "phone"),
            (34, SHIRT_C, SHIRT_CH, HAIR_C, "look_up"),
            (50, SHIRT_D, SHIRT_DH, HAIR_D, "phone"),
            (66, SHIRT_A, SHIRT_AH, HAIR_C, "camera"),
            (78, SHIRT_B, SHIRT_BH, HAIR_A, "phone"),
        ]
        for i, (ox, s, sh, h, pose) in enumerate(people):
            draw_person(img, ox, 60, s, sh, h, frame + i, pose)
        # phone glow screens above crowd
        for i, x in enumerate((14, 30, 46, 62, 78)):
            on = ((frame + i) % 4) != 3
            if on:
                oval(img, x, 28 + (i % 2), 3, 2, FW_CYAN if i % 2 == 0 else FW_PINK)
                px(img, x, 28 + (i % 2), WHITE)
        return img

    def soft():
        img = night_sky(96, 64, 22)
        for x in range(10, 90, 8):
            px(img, x, 4 + (frame % 2), GLOW_G if x % 16 < 8 else GLOW_P)
        rect(img, 2, 60, 94, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_food_truck(frame):
    """96x64 — festival food truck night market."""
    bob = [0, 1, 2, 1][frame % 4]

    def solid():
        img = new(96, 64)
        # truck body (slight bounce)
        yb = bob
        shade_rect(img, 8, 18 - yb, 72, 46 - yb, FW_ORANGE, FW_GOLD, WOOD_D)
        shade_rect(img, 10, 20 - yb, 70, 30 - yb, WHITE, CREAM, GRAY)
        shade_rect(img, 14, 12 - yb, 40, 22 - yb, NEON_P, FW_PINK_H, OUTLINE)
        for x in (18, 24, 30, 34):
            px(img, x, 15 - yb, WHITE)
            px(img, x + 1, 16 - yb, WHITE)
        # service window — warm flicker
        shade_rect(img, 18, 24 - yb, 52, 40 - yb, BOARD, BOARD_H, BLACK)
        win = [WARM, WARM_H, FW_GOLD_H, WARM][frame % 4]
        shade_rect(img, 20, 26 - yb, 50, 38 - yb, win, FW_GOLD_H, GOLD)
        if frame % 2 == 0:
            rect(img, 22, 28 - yb, 48, 36 - yb, (255, 220, 140, 50))
        # menu board blink rows
        shade_rect(img, 54, 22 - yb, 68, 40 - yb, BOARD_H, METAL, OUTLINE)
        for i, y in enumerate((25, 28, 31, 34, 37)):
            on = ((frame + i) % 4) != 3
            hline(img, 56, 66, y - yb, FW_GOLD if on else GRAY)
        # awning stripes bob
        for i in range(10):
            c = FW_PINK if i % 2 == 0 else WHITE
            shade_rect(img, 10 + i * 6, 8 - yb, 17 + i * 6, 16 - yb, c, WHITE, GRAY)
        # wheels
        oval(img, 22, 52, 7, 7, BLACK)
        oval(img, 22, 52, 3, 3, METAL_H)
        oval(img, 58, 52, 7, 7, BLACK)
        oval(img, 58, 52, 3, 3, METAL_H)
        px(img, 22 + (frame % 3) - 1, 52, WHITE)
        px(img, 58 - (frame % 3) + 1, 52, WHITE)
        # rising steam plumes (clear per-frame motion)
        for i, (sx, sy) in enumerate(((28, 22), (36, 20), (44, 22), (32, 18))):
            phase = (frame + i) % 4
            if phase != 3:
                px(img, sx + bob, sy - 4 - phase * 2 - i, WHITE)
                px(img, sx + 1 + bob, sy - 6 - phase * 2 - i, (255, 255, 255, 180))
                px(img, sx - 1 + bob, sy - 8 - phase * 2, (255, 240, 200, 120))
        # lanterns swing + glow
        for lx in (74, 84):
            swing = bob if lx == 74 else -bob
            shade_rect(img, lx + swing, 10 + bob, lx + 6 + swing, 18 + bob, FW_RED, FW_PINK, OUTLINE)
            vline(img, lx + 3 + swing, 6, 10 + bob, METAL_D)
            if frame % 2 == 0:
                px(img, lx + 3 + swing, 14 + bob, FW_GOLD_H)
                draw_spark(img, lx + 4 + swing, 12 + bob, FW_GOLD)
        draw_person(img, 72, 58, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        draw_person(img, 2, 58, SHIRT_C, SHIRT_CH, HAIR_C, frame + 1, "idle")
        draw_person(img, 40, 58, SHIRT_D, SHIRT_DH, HAIR_D, frame, "idle")
        return img

    def soft():
        img = night_sky(96, 64, 16, (30, 24, 50, 180), (20, 30, 55, 80))
        rect(img, 16, 24, 54, 42, (255, 190, 100, 35 + 10 * (frame % 2)))
        rect(img, 6, 58, 90, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_drone_show(frame):
    """96x64 — drone light show formation."""
    def solid():
        img = new(96, 64)
        draw_city_skyline(img, 48, 96, frame)
        draw_river(img, 48, 58, 96, frame, reflect=True)
        # drone grid forming heart / diamond by frame
        formations = [
            # diamond
            [(48, 12), (40, 18), (56, 18), (34, 26), (48, 26), (62, 26), (40, 34), (56, 34), (48, 40),
             (20, 16), (76, 16), (24, 30), (72, 30)],
            # wider
            [(48, 10), (38, 16), (58, 16), (30, 24), (48, 24), (66, 24), (38, 32), (58, 32), (48, 38),
             (16, 20), (80, 20), (22, 34), (74, 34)],
            # peak glow
            [(48, 8), (36, 14), (60, 14), (28, 22), (48, 20), (68, 22), (36, 30), (60, 30), (48, 36),
             (14, 18), (82, 18), (20, 32), (76, 32)],
            # settle
            [(48, 14), (42, 20), (54, 20), (36, 28), (48, 28), (60, 28), (42, 36), (54, 36), (48, 42),
             (22, 22), (74, 22), (26, 36), (70, 36)],
        ]
        colors = [FW_CYAN, FW_PINK, FW_GOLD, FW_VIOLET, WHITE, FW_CYAN_H, FW_PINK_H]
        for i, (x, y) in enumerate(formations[frame % 4]):
            c = colors[i % len(colors)]
            oval(img, x, y, 2, 2, c)
            px(img, x, y, WHITE)
            # tiny drone body
            px(img, x - 2, y, METAL)
            px(img, x + 2, y, METAL)
        draw_person(img, 24, 58, SHIRT_A, SHIRT_AH, HAIR_A, frame, "phone")
        draw_person(img, 52, 58, SHIRT_B, SHIRT_BH, HAIR_B, frame + 1, "look_up")
        return img

    def soft():
        img = night_sky(96, 64, 24, (16, 30, 60, 210), (12, 20, 48, 100))
        for x, y in ((48, 20), (36, 24), (60, 24)):
            px(img, x, y, GLOW_C)
            px(img, x + 4, y + 2, GLOW_P)
        rect(img, 8, 58, 88, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_photo_booth(frame):
    """64x96 — self-cam photo booth."""
    flash = frame in (1, 2)

    def solid():
        img = new(64, 96)
        # booth cabin
        shade_rect(img, 6, 8, 58, 78, PURPLE, PURPLE_H, NIGHT_D)
        shade_rect(img, 10, 12, 54, 48, BOARD, BOARD_H, BLACK)
        # screen
        screen = FW_CYAN_H if flash else (60, 80, 120, 255)
        shade_rect(img, 14, 16, 50, 44, screen, WHITE if flash else FW_CYAN, METAL_D)
        # on-screen silhouette pose
        bob = [0, 1, 2, 1][frame % 4]
        oval(img, 32, 24 + bob, 5, 5, (80, 60, 70, 220))
        shade_rect(img, 26, 30 + bob, 38, 42 + bob, (90, 50, 90, 200), (120, 70, 110, 200), None)
        if flash:
            oval(img, 32, 28, 12, 10, (255, 255, 255, 120))
            draw_spark(img, 20, 18, WHITE)
            draw_spark(img, 44, 18, WHITE)
        # curtain sway (more amplitude)
        for i in range(6):
            c = FW_PINK if i % 2 == 0 else PURPLE_H
            sway = [0, 1, 2, 1][(frame + i) % 4]
            shade_rect(img, 12 + i * 6 + sway, 48, 19 + i * 6 + sway, 72, c, FW_PINK_H, PURPLE)
            px(img, 14 + i * 6 + sway, 70, c)
        # neon FRAME sign flicker
        on = frame % 4 != 3
        shade_rect(
            img, 16, 2, 48, 9,
            NEON_P if on else (90, 30, 70, 255),
            WHITE if on else NEON_P,
            OUTLINE,
        )
        for x in (20, 28, 36, 42):
            px(img, x, 4, WHITE if on else GRAY)
        # print slot pops photo
        shade_rect(img, 24, 74, 40, 80, METAL_D, METAL, BLACK)
        if frame % 4 in (0, 1):
            shade_rect(img, 26, 76, 38, 82, CREAM, WHITE, GRAY)
        elif frame % 4 == 2:
            shade_rect(img, 26, 78, 38, 86, CREAM, WHITE, GRAY)
        # vanity lights chase
        for i, y in enumerate((16, 28, 40)):
            lit = ((frame + i) % 4) != 3
            oval(img, 12, y, 2, 2, WARM_H if lit else GOLD)
            oval(img, 52, y, 2, 2, WARM_H if lit else GOLD)
        draw_person(img, 22, 92, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        return img

    def soft():
        img = night_sky(64, 96, 14, (40, 20, 60, 160), (20, 16, 40, 60))
        if flash:
            rect(img, 12, 14, 52, 46, (255, 255, 255, 50))
        if frame % 4 != 3:
            for x in range(16, 48):
                px(img, x, 1, (255, 96, 205, 45))
        rect(img, 10, 90, 54, 94, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_pier_boat(frame):
    """96x64 — pier & river cruise boat."""
    bob = [0, 2, 3, 1][frame % 4]

    def solid():
        img = new(96, 64)
        draw_city_skyline(img, 20, 96, frame)
        draw_firework_burst(img, 70, 8, frame, [FW_PINK, FW_GOLD], 1)
        # pier planks
        shade_rect(img, 0, 40, 40, 54, WOOD, WOOD_H, WOOD_D)
        for x in range(2, 38, 5):
            vline(img, x, 40, 53, WOOD_D)
        # pier posts
        for x in (4, 18, 32):
            shade_rect(img, x, 48, x + 3, 60, WOOD_D, WOOD, BLACK)
        draw_river(img, 42, 60, 96, frame)
        # cruise boat
        bx = 44 + (1 if frame % 2 == 0 else 0)
        shade_rect(img, bx, 30 + bob, bx + 44, 46 + bob, WHITE, CREAM, METAL_D)
        shade_rect(img, bx + 4, 22 + bob, bx + 36, 32 + bob, FW_CYAN, FW_CYAN_H, METAL)
        # cabin windows lit
        for i, wx in enumerate((bx + 8, bx + 16, bx + 24, bx + 30)):
            on = ((frame + i) % 4) != 3
            shade_rect(img, wx, 24 + bob, wx + 5, 30 + bob, WARM_H if on else GOLD, WHITE, OUTLINE)
        # smokestack
        shade_rect(img, bx + 34, 14 + bob, bx + 40, 24 + bob, METAL_D, METAL, BLACK)
        if frame % 2 == 0:
            px(img, bx + 36, 12 + bob, GRAY_H)
            px(img, bx + 37, 10 + bob, WHITE)
        # hull stripe
        hline(img, bx, bx + 43, 40 + bob, FW_PINK)
        hline(img, bx, bx + 43, 41 + bob, FW_GOLD)
        # water wake
        for i, dx in enumerate((0, 4, 8)):
            hline(img, bx - 4 - dx, bx - 1 - dx, 46 + bob + (i % 2), RIVER_H)
        draw_person(img, 8, 48, SHIRT_A, SHIRT_AH, HAIR_A, frame, "phone")
        draw_person(img, 22, 48, SHIRT_B, SHIRT_BH, HAIR_B, frame + 1, "look_up")
        return img

    def soft():
        img = night_sky(96, 64, 22)
        rect(img, 44, 42, 90, 52, (90, 230, 255, 25))
        rect(img, 2, 58, 94, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_empty_river(frame):
    """96x64 — emptied riverside after festival (void mood)."""
    def solid():
        img = new(96, 64)
        # desaturated skyline
        heights = [8, 12, 9, 15, 10, 13, 8, 14, 11, 12]
        for i, h in enumerate(heights):
            x0 = 2 + i * 9
            shade_rect(img, x0, 24 - h, x0 + 7, 24, CITY_D, CITY, OUTLINE)
            if i % 3 == 0 and frame % 4 == 0:
                px(img, x0 + 2, 24 - h + 3, (120, 110, 80, 180))
        # empty railing
        shade_rect(img, 0, 34, 96, 38, BRIDGE_D, BRIDGE, BRIDGE_D)
        for x in range(4, 92, 8):
            shade_rect(img, x, 38, x + 2, 48, BRIDGE_D, BRIDGE, BLACK)
        # murky river, weak leftover reflection
        shade_rect(img, 0, 48, 96, 60, RIVER_D, RIVER, NIGHT_D)
        for x in range(10, 90, 14):
            px(img, x + (frame % 2), 52, (80, 70, 90, 120))
            px(img, x + 1, 54, (60, 50, 70, 80))
        # leftover trash / cone
        shade_rect(img, 70, 42, 76, 50, FW_ORANGE, FW_GOLD, WOOD_D)
        shade_rect(img, 72, 40, 74, 42, WHITE, WHITE, GRAY)
        # faint dying spark (last ember)
        if frame % 4 == 0:
            px(img, 40, 10, (180, 100, 80, 200))
            px(img, 41, 9, (120, 80, 60, 120))
        elif frame % 4 == 1:
            px(img, 42, 12, (140, 90, 70, 150))
        # lone person looking at empty sky
        draw_person(img, 36, 50, SHIRT_C, SHIRT_CH, HAIR_C, frame, "look_up")
        # distant empty phone glow only one
        if frame % 2 == 0:
            px(img, 48, 28, (100, 120, 140, 180))
        return img

    def soft():
        img = night_sky(96, 64, 28, (18, 16, 28, 200), (10, 14, 24, 90))
        # colder, emptier
        for y in range(0, 20):
            if y % 4 == frame % 4:
                hline(img, 20, 76, y, (40, 36, 50, 20))
        rect(img, 8, 58, 88, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_bridge_lights(frame):
    """96x64 — bridge night neon lighting."""
    def solid():
        img = new(96, 64)
        draw_city_skyline(img, 18, 96, frame)
        # suspension bridge
        shade_rect(img, 6, 8, 12, 40, BRIDGE_D, BRIDGE, BLACK)
        shade_rect(img, 84, 8, 90, 40, BRIDGE_D, BRIDGE, BLACK)
        # cables
        for i, x in enumerate(range(14, 84, 5)):
            y1 = 12 + abs(i - 7)
            vline(img, x, 10, y1, METAL)
            px(img, x, 10, BRIDGE_H)
        shade_rect(img, 4, 28, 92, 34, BRIDGE, BRIDGE_H, BRIDGE_D)
        # neon light string along deck
        colors = [FW_PINK, FW_CYAN, FW_GOLD, FW_VIOLET, NEON_Y]
        for i, x in enumerate(range(8, 90, 4)):
            on = ((frame + i) % 4) != 3
            if on:
                oval(img, x, 26, 2, 2, colors[i % len(colors)])
                px(img, x, 26, WHITE)
            else:
                px(img, x, 26, METAL_D)
        # tower crowns glow
        for tx in (8, 86):
            on = frame % 2 == 0
            oval(img, tx + 1, 6, 3, 2, FW_GOLD_H if on else GOLD)
            px(img, tx + 1, 5, WHITE if on else FW_GOLD)
        draw_river(img, 42, 56, 96, frame)
        draw_person(img, 28, 48, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        draw_person(img, 52, 48, SHIRT_A, SHIRT_AH, HAIR_A, frame + 1, "camera")
        return img

    def soft():
        img = night_sky(96, 64, 24)
        glow = [FW_PINK, FW_CYAN, FW_GOLD, FW_VIOLET][frame % 4]
        for x in range(10, 88, 3):
            px(img, x, 24, (glow[0], glow[1], glow[2], 40))
        rect(img, 4, 56, 92, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


def sub_water_reflect(frame):
    """96x64 — fireworks mirrored on river surface."""
    def solid():
        img = new(96, 64)
        draw_city_skyline(img, 22, 96, frame)
        # sky bursts
        bursts = [
            (24, 10, [FW_PINK, FW_GOLD, WHITE]),
            (48, 6, [FW_CYAN, FW_VIOLET, WHITE]),
            (72, 12, [FW_GOLD, FW_ORANGE, FW_PINK]),
        ]
        for i, (cx, cy, cols) in enumerate(bursts):
            draw_firework_burst(img, cx, cy, frame + i, cols, 1)
        # river with strong vertical mirror streaks
        shade_rect(img, 0, 28, 96, 58, RIVER, RIVER_H, RIVER_D)
        mirror_cols = [
            (24, FW_PINK), (48, FW_CYAN), (72, FW_GOLD),
            (32, FW_VIOLET), (60, FW_PINK_H), (80, FW_ORANGE),
        ]
        for i, (mx, mc) in enumerate(mirror_cols):
            wobble = ((frame + i) % 4) - 1
            for dy in range(0, 22, 2):
                a = max(40, 180 - dy * 7)
                c = (mc[0], mc[1], mc[2], a)
                px(img, mx + wobble, 30 + dy, c)
                px(img, mx + 1 + wobble, 31 + dy, (255, 255, 255, a // 2))
            if frame % 2 == 0:
                draw_spark(img, mx, 34 + (i % 3) * 4, mc)
        # ripples
        for x in range(4, 92, 8):
            hline(img, x, x + 3, 40 + ((frame + x) % 3), RIVER_H)
        draw_bridge_rail(img, 24, 96, frame)
        draw_person(img, 10, 40, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        draw_person(img, 70, 40, SHIRT_C, SHIRT_CH, HAIR_C, frame + 1, "camera")
        return img

    def soft():
        img = night_sky(96, 64, 20)
        for cx, cy in ((24, 10), (48, 6), (72, 12)):
            glow = [GLOW_P, GLOW_C, GLOW_G][(cx // 24 + frame) % 3]
            oval(img, cx, cy, 6, 4, glow)
            # soft mirror glow
            oval(img, cx, 42, 5, 8, (glow[0], glow[1], glow[2], 30))
        rect(img, 4, 56, 92, 63, SHADOW)
        return img

    return with_backdrop_then_outline(solid, soft)


SUBJECTS = [
    ("Act3_PhotoSubject_BridgeView_96x64.png", "Act3_PhotoStill_BridgeView.png", 96, 64, sub_bridge_view),
    ("Act3_PhotoSubject_FireworkPeak_64x96.png", "Act3_PhotoStill_FireworkPeak.png", 64, 96, sub_firework_peak),
    ("Act3_PhotoSubject_PhoneCrowd_96x64.png", "Act3_PhotoStill_PhoneCrowd.png", 96, 64, sub_phone_crowd),
    ("Act3_PhotoSubject_FoodTruck_96x64.png", "Act3_PhotoStill_FoodTruck.png", 96, 64, sub_food_truck),
    ("Act3_PhotoSubject_DroneShow_96x64.png", "Act3_PhotoStill_DroneShow.png", 96, 64, sub_drone_show),
    ("Act3_PhotoSubject_PhotoBooth_64x96.png", "Act3_PhotoStill_PhotoBooth.png", 64, 96, sub_photo_booth),
    ("Act3_PhotoSubject_PierBoat_96x64.png", "Act3_PhotoStill_PierBoat.png", 96, 64, sub_pier_boat),
    ("Act3_PhotoSubject_EmptyRiver_96x64.png", "Act3_PhotoStill_EmptyRiver.png", 96, 64, sub_empty_river),
    ("Act3_PhotoSubject_BridgeLights_96x64.png", "Act3_PhotoStill_BridgeLights.png", 96, 64, sub_bridge_lights),
    ("Act3_PhotoSubject_WaterReflect_96x64.png", "Act3_PhotoStill_WaterReflect.png", 96, 64, sub_water_reflect),
]


if __name__ == "__main__":
    # 스틸(SNS 카드)은 AI 고퀄본을 덮어쓰지 않음 — 배치용 피사체 시트만 생성
    for sheet, _still, w, h, fn in SUBJECTS:
        tiles = [lambda f=f, func=fn: func(f) for f in range(4)]
        build_sheet(tiles, os.path.join(OUT, sheet), w, h)

    print(f"done subjects-only={len(SUBJECTS)}")
