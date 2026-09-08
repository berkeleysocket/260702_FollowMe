"""Act3 photo SUBJECTS — Act2와 같은 '맵 랜드마크 소품' 결.

스틸(SNS 카드)은 건드리지 않음. 월드에 배치되는 4프레임 시트만 재생성.
투명 배경 + soft tint, 피사체(부스/노점/난간/보트 등)가 주인공.
"""
from PIL import Image
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act3_Fireworks"
os.makedirs(OUT, exist_ok=True)

CLR = (0, 0, 0, 0)
OUTLINE = (12, 10, 18, 255)

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

WOOD = (148, 104, 64, 255)
WOOD_H = (178, 132, 88, 255)
WOOD_D = (110, 72, 42, 255)
CREAM = (248, 240, 228, 255)
WHITE = (246, 246, 250, 255)
BLACK = (28, 26, 34, 255)
GRAY = (120, 118, 128, 255)
GRAY_H = (170, 168, 178, 255)
METAL = (150, 156, 168, 255)
METAL_H = (200, 206, 218, 255)
METAL_D = (90, 94, 104, 255)
BOARD = (28, 26, 38, 255)
BOARD_H = (48, 46, 64, 255)

NIGHT = (28, 34, 58, 255)
NIGHT_H = (48, 56, 88, 255)
PINK = (255, 140, 178, 255)
PINK_H = (255, 190, 210, 255)
CYAN = (90, 230, 255, 255)
GOLD = (230, 180, 70, 255)
WARM = (255, 210, 130, 255)
WARM_H = (255, 230, 180, 255)
ORANGE = (255, 140, 60, 255)
PURPLE = (120, 80, 180, 255)
RED = (220, 70, 90, 255)
SHADOW = (12, 14, 24, 90)
SOFT_TOP = (40, 36, 70, 70)
SOFT_BOT = (24, 40, 70, 35)


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


def soft_night(w, h, y_max=18):
    """Act2 soft sky처럼 얇은 tint만 — 전경 파노라마 금지."""
    img = new(w, h)
    for y in range(0, y_max):
        t = y / max(1, y_max - 1)
        c = (
            int(SOFT_TOP[0] * (1 - t) + SOFT_BOT[0] * t),
            int(SOFT_TOP[1] * (1 - t) + SOFT_BOT[1] * t),
            int(SOFT_TOP[2] * (1 - t) + SOFT_BOT[2] * t),
            int(SOFT_TOP[3] * (1 - t) + SOFT_BOT[3] * t),
        )
        hline(img, 0, w - 1, y, c)
    rect(img, 4, h - 4, w - 4, h - 1, SHADOW)
    return img


def with_backdrop(draw_fn, soft_fn):
    outlined = add_outline(draw_fn())
    back = soft_fn()
    back.paste(outlined, (0, 0), outlined)
    return back


def person(img, ox, oy, shirt, shirt_hi, hair, frame=0, pose="phone"):
    bob = [0, 1, 2, 1][frame % 4]
    head_y = oy - 24 + bob
    rect(img, ox + 2, oy - 1, ox + 13, oy + 1, SHADOW)
    shade_rect(img, ox + 3, oy - 10, ox + 6, oy - 2, PANTS, PANTS_H, SHOE)
    shade_rect(img, ox + 8, oy - 10, ox + 11, oy - 2, PANTS, PANTS_H, SHOE)
    shade_rect(img, ox + 2, oy - 18 + bob // 2, ox + 12, oy - 10, shirt, shirt_hi, PANTS)
    oval(img, ox + 7, head_y, 4, 4, SKIN)
    px(img, ox + 5, head_y, OUTLINE)
    px(img, ox + 9, head_y, OUTLINE)
    rect(img, ox + 3, head_y - 5, ox + 12, head_y - 1, hair)
    if pose == "phone":
        lift = [0, -2, -4, -1][frame % 4]
        shade_rect(img, ox + 11, oy - 16 + lift, ox + 17, oy - 10 + lift, BLACK, GRAY, BLACK)
        px(img, ox + 13, oy - 14 + lift, CYAN if frame % 2 == 0 else PINK)
    elif pose == "look_up":
        rect(img, ox + 11, oy - 20 - bob, ox + 14, oy - 14, SKIN)


def spark(img, x, y, c):
    px(img, x, y, c)
    px(img, x + 1, y, WHITE)
    px(img, x, y - 1, c)
    px(img, x - 1, y, c)


def build_sheet(tiles, path, cw, ch):
    sheet = Image.new("RGBA", (len(tiles) * cw, ch), CLR)
    for i, fn in enumerate(tiles):
        sheet.paste(fn(), (i * cw, 0))
    sheet.save(path)
    print("sheet", os.path.basename(path), sheet.size)


# ---------- subjects (landmark props) ----------

def sub_bridge_view(frame):
    bob = frame % 2

    def solid():
        img = new(96, 64)
        # 짧은 난간 + 쌍안경 전망대 (전경 소품)
        shade_rect(img, 4, 36, 92, 42, METAL, METAL_H, METAL_D)
        for x in range(8, 90, 8):
            shade_rect(img, x, 42, x + 2, 54, METAL, METAL_H, METAL_D)
            if (frame + x) % 4 != 3:
                px(img, x, 35, GOLD if x % 16 < 8 else CYAN)
        shade_rect(img, 0, 54, 96, 60, WOOD, WOOD_H, WOOD_D)
        # coin binocular
        shade_rect(img, 62, 20 + bob, 86, 40 + bob, METAL_D, METAL, BLACK)
        oval(img, 70, 28 + bob, 5, 5, BOARD)
        oval(img, 80, 28 + bob, 5, 5, BOARD)
        oval(img, 70, 28 + bob, 2, 2, CYAN if frame % 2 == 0 else GOLD)
        oval(img, 80, 28 + bob, 2, 2, GOLD if frame % 2 == 0 else CYAN)
        shade_rect(img, 72, 40 + bob, 78, 54, METAL, METAL_H, METAL_D)
        person(img, 18, 54, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        person(img, 40, 54, SHIRT_A, SHIRT_AH, HAIR_A, frame + 1, "look_up")
        if frame % 2 == 0:
            spark(img, 28, 12, PINK)
            spark(img, 50, 8, GOLD)
        return img

    return with_backdrop(solid, lambda: soft_night(96, 64, 16))


def sub_firework_peak(frame):
    def solid():
        img = new(64, 96)
        # 관람 스탠드 + 위쪽 작은 불꽃 스파크만
        shade_rect(img, 6, 70, 58, 78, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 10, 78, 54, 88, WOOD_D, WOOD, BLACK)
        for x in range(14, 52, 8):
            shade_rect(img, x, 62, x + 3, 70, METAL, METAL_H, METAL_D)
        # small bursts (not full sky fill)
        cx, cy = 32, 28
        phase = frame % 4
        cols = [PINK, GOLD, CYAN, PURPLE]
        r = [3, 6, 9, 7][phase]
        for i, (dx, dy) in enumerate(((0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1))):
            c = cols[i % 4]
            px(img, cx + dx * r, cy + dy * r, c)
            if phase >= 2:
                spark(img, cx + dx * (r // 2), cy + dy * (r // 2), c)
        oval(img, cx, cy, 2, 2, WHITE)
        person(img, 12, 88, SHIRT_B, SHIRT_BH, HAIR_B, frame, "look_up")
        person(img, 34, 88, SHIRT_C, SHIRT_CH, HAIR_C, frame + 1, "phone")
        return img

    return with_backdrop(solid, lambda: soft_night(64, 96, 22))


def sub_phone_crowd(frame):
    def solid():
        img = new(96, 64)
        shade_rect(img, 0, 50, 96, 58, WOOD, WOOD_H, WOOD_D)
        # barrier
        shade_rect(img, 4, 40, 92, 46, METAL, METAL_H, METAL_D)
        people = [
            (4, SHIRT_A, SHIRT_AH, HAIR_A),
            (22, SHIRT_B, SHIRT_BH, HAIR_B),
            (40, SHIRT_C, SHIRT_CH, HAIR_C),
            (58, SHIRT_A, SHIRT_AH, HAIR_B),
            (74, SHIRT_B, SHIRT_BH, HAIR_A),
        ]
        for i, (ox, s, sh, h) in enumerate(people):
            person(img, ox, 58, s, sh, h, frame + i, "phone")
        return img

    return with_backdrop(solid, lambda: soft_night(96, 64, 14))


def sub_food_truck(frame):
    bob = [0, 1, 2, 1][frame % 4]

    def solid():
        img = new(96, 64)
        y = bob
        # truck body — Act2 마카롱처럼 중앙 랜드마크
        shade_rect(img, 14, 18 - y, 78, 48 - y, PURPLE, PINK, BOARD)
        shade_rect(img, 18, 22 - y, 56, 40 - y, BOARD, BOARD_H, BLACK)
        shade_rect(img, 20, 24 - y, 54, 38 - y, WARM if frame % 2 == 0 else WARM_H, GOLD, ORANGE)
        # awning
        for i in range(10):
            c = PINK if i % 2 == 0 else CYAN
            shade_rect(img, 14 + i * 6, 10 - y, 20 + i * 6, 18 - y, c, WHITE, GRAY)
        # wheels
        oval(img, 28, 52, 6, 6, BLACK)
        oval(img, 28, 52, 2, 2, METAL_H)
        oval(img, 64, 52, 6, 6, BLACK)
        oval(img, 64, 52, 2, 2, METAL_H)
        # steam
        for i, sx in enumerate((30, 40, 48)):
            if (frame + i) % 4 != 3:
                px(img, sx, 20 - y - (frame + i) % 3 * 2, WHITE)
        person(img, 78, 58, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        return img

    return with_backdrop(solid, lambda: soft_night(96, 64, 12))


def sub_drone_show(frame):
    def solid():
        img = new(96, 64)
        shade_rect(img, 8, 48, 88, 56, WOOD, WOOD_H, WOOD_D)
        # drone dots forming diamond
        base = [(48, 14), (40, 20), (56, 20), (34, 28), (48, 28), (62, 28), (40, 36), (56, 36), (48, 42)]
        for i, (x, y) in enumerate(base):
            c = [CYAN, PINK, GOLD, WHITE][(i + frame) % 4]
            oval(img, x, y + (frame % 2), 2, 2, c)
            if frame % 2 == 0:
                px(img, x, y - 1, WHITE)
        person(img, 16, 56, SHIRT_A, SHIRT_AH, HAIR_A, frame, "look_up")
        person(img, 64, 56, SHIRT_B, SHIRT_BH, HAIR_B, frame + 1, "phone")
        return img

    return with_backdrop(solid, lambda: soft_night(96, 64, 14))


def sub_photo_booth(frame):
    bob = frame % 2

    def solid():
        img = new(64, 96)
        # booth cabinet like Act2 mirror room prop
        shade_rect(img, 8, 12, 56, 86, BOARD, BOARD_H, BLACK)
        shade_rect(img, 12, 18, 52, 58, NIGHT_H, CYAN if frame % 2 == 0 else PINK, METAL)
        # curtain
        shade_rect(img, 14, 20, 28, 56, PINK, PINK_H, RED)
        shade_rect(img, 36, 20, 50, 56, PINK, PINK_H, RED)
        # camera flash
        oval(img, 32, 28 + bob, 4, 3, WARM_H if frame % 2 == 0 else GOLD)
        if frame % 2 == 0:
            spark(img, 32, 24, WHITE)
        # strip photos hanging
        for i, x in enumerate((16, 28, 40)):
            shade_rect(img, x, 62 + (i + frame) % 2, x + 8, 78, CREAM, WHITE, GRAY)
            px(img, x + 3, 66, PINK)
            px(img, x + 4, 70, CYAN)
        shade_rect(img, 20, 86, 44, 92, METAL, METAL_H, METAL_D)
        return img

    return with_backdrop(solid, lambda: soft_night(64, 96, 16))


def sub_pier_boat(frame):
    bob = [0, 1, 2, 1][frame % 4]

    def solid():
        img = new(96, 64)
        # pier planks
        shade_rect(img, 0, 44, 96, 54, WOOD, WOOD_H, WOOD_D)
        for x in range(4, 92, 8):
            vline(img, x, 44, 53, WOOD_D)
        # small boat
        shade_rect(img, 28, 28 + bob, 70, 42 + bob, WHITE, CREAM, METAL)
        shade_rect(img, 34, 20 + bob, 64, 30 + bob, CYAN, WHITE, METAL_D)
        # mast light
        vline(img, 48, 8 + bob, 20 + bob, METAL_D)
        oval(img, 48, 8 + bob, 2, 2, GOLD if frame % 2 == 0 else WARM_H)
        # bollards
        shade_rect(img, 12, 36, 18, 48, METAL, METAL_H, METAL_D)
        shade_rect(img, 78, 36, 84, 48, METAL, METAL_H, METAL_D)
        person(img, 8, 54, SHIRT_C, SHIRT_CH, HAIR_C, frame, "phone")
        return img

    return with_backdrop(solid, lambda: soft_night(96, 64, 12))


def sub_empty_river(frame):
    def solid():
        img = new(96, 64)
        # empty bench + rail — sparse Act11 vibe
        shade_rect(img, 8, 38, 88, 42, METAL, METAL_H, METAL_D)
        for x in range(12, 86, 10):
            shade_rect(img, x, 42, x + 2, 52, METAL, METAL_H, METAL_D)
        shade_rect(img, 24, 44, 56, 52, WOOD_D, WOOD, BLACK)
        shade_rect(img, 26, 38, 32, 44, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 48, 38, 54, 44, WOOD, WOOD_H, WOOD_D)
        # trash / cone only
        shade_rect(img, 70, 40, 78, 52, ORANGE, GOLD, WOOD_D)
        hline(img, 70, 77, 44, WHITE)
        if frame == 2:
            spark(img, 40, 16, GRAY_H)
        return img

    return with_backdrop(solid, lambda: soft_night(96, 64, 14))


def sub_bridge_lights(frame):
    def solid():
        img = new(96, 64)
        # pillar landmark + string lights
        shade_rect(img, 40, 8, 56, 54, METAL_D, METAL, BLACK)
        shade_rect(img, 36, 6, 60, 12, METAL, METAL_H, METAL_D)
        for i, y in enumerate(range(16, 50, 6)):
            on = ((frame + i) % 4) != 3
            c = GOLD if on else CYAN
            oval(img, 48, y, 2, 2, c)
            if on:
                px(img, 48, y, WHITE)
        # ground rail
        shade_rect(img, 4, 50, 92, 56, WOOD, WOOD_H, WOOD_D)
        # hanging lights left/right
        hline(img, 8, 40, 14, METAL_D)
        hline(img, 56, 88, 14, METAL_D)
        for i, x in enumerate(range(10, 40, 6)):
            y = 16 + ((frame + i) % 2) * 2
            oval(img, x, y, 2, 3, PINK if i % 2 == 0 else CYAN)
        for i, x in enumerate(range(58, 88, 6)):
            y = 16 + ((frame + i + 1) % 2) * 2
            oval(img, x, y, 2, 3, GOLD if i % 2 == 0 else PINK)
        person(img, 18, 56, SHIRT_A, SHIRT_AH, HAIR_A, frame, "phone")
        return img

    return with_backdrop(solid, lambda: soft_night(96, 64, 12))


def sub_water_reflect(frame):
    def solid():
        img = new(96, 64)
        # short pier edge + reflect poles (not full river panorama)
        shade_rect(img, 0, 40, 96, 48, WOOD, WOOD_H, WOOD_D)
        shade_rect(img, 0, 48, 96, 58, NIGHT, NIGHT_H, BOARD)
        for i, x in enumerate(range(12, 90, 14)):
            c = [PINK, CYAN, GOLD, PURPLE][(i + frame) % 4]
            vline(img, x, 50, 56, (c[0], c[1], c[2], 180))
            px(img, x, 49, c)
        shade_rect(img, 8, 34, 88, 40, METAL, METAL_H, METAL_D)
        person(img, 30, 40, SHIRT_B, SHIRT_BH, HAIR_B, frame, "phone")
        person(img, 58, 40, SHIRT_C, SHIRT_CH, HAIR_C, frame + 1, "look_up")
        return img

    return with_backdrop(solid, lambda: soft_night(96, 64, 14))


SUBJECTS = [
    ("Act3_PhotoSubject_BridgeView_96x64.png", 96, 64, sub_bridge_view),
    ("Act3_PhotoSubject_FireworkPeak_64x96.png", 64, 96, sub_firework_peak),
    ("Act3_PhotoSubject_PhoneCrowd_96x64.png", 96, 64, sub_phone_crowd),
    ("Act3_PhotoSubject_FoodTruck_96x64.png", 96, 64, sub_food_truck),
    ("Act3_PhotoSubject_DroneShow_96x64.png", 96, 64, sub_drone_show),
    ("Act3_PhotoSubject_PhotoBooth_64x96.png", 64, 96, sub_photo_booth),
    ("Act3_PhotoSubject_PierBoat_96x64.png", 96, 64, sub_pier_boat),
    ("Act3_PhotoSubject_EmptyRiver_96x64.png", 96, 64, sub_empty_river),
    ("Act3_PhotoSubject_BridgeLights_96x64.png", 96, 64, sub_bridge_lights),
    ("Act3_PhotoSubject_WaterReflect_96x64.png", 96, 64, sub_water_reflect),
]


if __name__ == "__main__":
    for name, w, h, fn in SUBJECTS:
        tiles = [lambda f=f, func=fn: func(f) for f in range(4)]
        build_sheet(tiles, os.path.join(OUT, name), w, h)
    print(f"done landmark subjects={len(SUBJECTS)}")
