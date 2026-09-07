"""Act2 photo subjects — 고퀄 무인물 스틸 베이스 + 4프레임 애니메이션.
인물 제거 (소품·네온·스팀만). 스틸 PNG는 덮어쓰지 않음.
"""
from PIL import Image, ImageEnhance
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act2_Cafe"
CLR = (0, 0, 0, 0)

SPOTS = [
    {
        "still": "Act2_PhotoStill_LatteArt.png",
        "subject": "Act2_PhotoSubject_LatteArt_96x64.png",
        "cw": 96, "ch": 64, "bias_y": 0.05,
        "sparks": [
            (55, 22, 255, 255, 255), (58, 18, 255, 240, 220), (52, 16, 255, 255, 255),
            (40, 12, 255, 140, 178), (70, 28, 255, 210, 130),
        ],
        "hearts": [(30, 10), (72, 14)],
    },
    {
        "still": "Act2_PhotoStill_Macaron.png",
        "subject": "Act2_PhotoSubject_Macaron_64x96.png",
        "cw": 64, "ch": 96, "bias_y": 0.0,
        "sparks": [
            (32, 30, 255, 140, 178), (28, 40, 120, 210, 180), (36, 48, 255, 210, 90),
            (24, 55, 255, 140, 178), (40, 36, 255, 255, 255),
        ],
        "hearts": [(18, 20), (46, 24)],
    },
    {
        "still": "Act2_PhotoStill_DonutWall.png",
        "subject": "Act2_PhotoSubject_DonutWall_96x64.png",
        "cw": 96, "ch": 64, "bias_y": 0.0,
        "sparks": [
            (30, 20, 255, 140, 178), (48, 16, 255, 210, 90), (66, 20, 120, 210, 180),
            (40, 28, 255, 255, 255), (58, 30, 255, 140, 178),
        ],
        "hearts": [(24, 10), (72, 12)],
    },
    {
        "still": "Act2_PhotoStill_NeonCafe.png",
        "subject": "Act2_PhotoSubject_NeonCafe_96x64.png",
        "cw": 96, "ch": 64, "bias_y": 0.0,
        "sparks": [
            (28, 18, 255, 96, 205), (40, 18, 90, 230, 255), (52, 18, 255, 232, 96),
            (64, 18, 120, 255, 160), (36, 30, 255, 96, 205), (48, 32, 90, 230, 255),
            (70, 28, 255, 210, 110),
        ],
        "hearts": [(42, 28), (50, 34)],
        "chase": True,
    },
    {
        "still": "Act2_PhotoStill_Terrace.png",
        "subject": "Act2_PhotoSubject_Terrace_96x64.png",
        "cw": 96, "ch": 64, "bias_y": -0.02,
        "sparks": [
            (24, 20, 255, 196, 150), (48, 14, 255, 255, 255), (70, 18, 120, 210, 180),
            (36, 28, 255, 140, 178), (60, 24, 255, 210, 90),
        ],
        "hearts": [(32, 10), (64, 12)],
    },
    {
        "still": "Act2_PhotoStill_MirrorRoom.png",
        "subject": "Act2_PhotoSubject_MirrorRoom_64x96.png",
        "cw": 64, "ch": 96, "bias_y": 0.0,
        "sparks": [
            (32, 28, 255, 255, 255), (24, 40, 200, 170, 220), (40, 40, 255, 140, 178),
            (32, 52, 90, 230, 255), (28, 20, 255, 210, 90),
        ],
        "hearts": [(18, 24), (46, 30)],
    },
    {
        "still": "Act2_PhotoStill_PlantCafe.png",
        "subject": "Act2_PhotoSubject_PlantCafe_64x96.png",
        "cw": 64, "ch": 96, "bias_y": 0.0,
        "sparks": [
            (32, 24, 110, 190, 120), (24, 36, 70, 150, 90), (40, 40, 255, 255, 255),
            (28, 50, 120, 210, 180), (36, 18, 255, 196, 150),
        ],
        "hearts": [(20, 16), (44, 22)],
    },
    {
        "still": "Act2_PhotoStill_NightWindow.png",
        "subject": "Act2_PhotoSubject_NightWindow_64x96.png",
        "cw": 64, "ch": 96, "bias_y": 0.0,
        "sparks": [
            (32, 30, 255, 210, 130), (24, 40, 255, 96, 205), (40, 42, 90, 230, 255),
            (32, 20, 255, 232, 96), (28, 55, 255, 210, 110),
        ],
        "hearts": [(18, 26), (46, 28)],
    },
    {
        "still": "Act2_PhotoStill_RoundWindow.png",
        "subject": "Act2_PhotoSubject_RoundWindow_64x64.png",
        "cw": 64, "ch": 64, "bias_y": 0.0,
        "sparks": [
            (32, 24, 255, 210, 130), (24, 30, 255, 140, 178), (40, 30, 90, 230, 255),
            (32, 18, 255, 255, 255), (28, 40, 255, 196, 150),
        ],
        "hearts": [(20, 14), (44, 16)],
    },
    {
        "still": "Act2_PhotoStill_DessertCart.png",
        "subject": "Act2_PhotoSubject_DessertCart_96x64.png",
        "cw": 96, "ch": 64, "bias_y": 0.05,
        "sparks": [
            (40, 14, 255, 255, 255), (48, 10, 255, 240, 220), (56, 14, 255, 255, 255),
            (36, 22, 255, 140, 178), (60, 20, 120, 210, 180), (72, 18, 255, 210, 90),
        ],
        "hearts": [(28, 8), (70, 10)],
    },
]


def crop_focus(img, cw, ch, bias_y):
    w, h = img.size
    aspect = cw / ch
    if w / h > aspect:
        box_h = h
        box_w = int(h * aspect)
    else:
        box_w = w
        box_h = int(w / aspect)
    left = (w - box_w) // 2
    top = (h - box_h) // 2 + int(h * bias_y)
    top = max(0, min(top, h - box_h))
    left = max(0, min(left, w - box_w))
    return img.crop((left, top, left + box_w, top + box_h)).resize(
        (cw, ch), Image.Resampling.LANCZOS
    )


def to_pixel(img):
    out = img.convert("RGBA")
    out = ImageEnhance.Color(out).enhance(1.1)
    out = ImageEnhance.Contrast(out).enhance(1.05)
    out = ImageEnhance.Sharpness(out).enhance(1.5)
    return out


def put_spark(layer, x, y, rgb, bright=1.0):
    r, g, b = rgb
    a = int(210 * bright)
    c = (r, g, b, a)
    wcol = (255, 255, 255, int(190 * bright))
    px = layer.load()
    cw, ch = layer.size
    for dx, dy, col in (
        (0, 0, wcol),
        (1, 0, c),
        (-1, 0, c),
        (0, 1, c),
        (0, -1, c),
        (1, -1, (r, g, b, int(100 * bright))),
    ):
        xx, yy = x + dx, y + dy
        if 0 <= xx < cw and 0 <= yy < ch:
            px[xx, yy] = col


def make_frame(base, frame, spot):
    cw, ch = spot["cw"], spot["ch"]
    bob = [0, -1, -2, -1][frame % 4]
    canvas = Image.new("RGBA", (cw, ch), CLR)
    canvas.paste(base, (0, bob))
    if bob < 0:
        canvas.paste(base.crop((0, ch + bob, cw, ch)), (0, ch + bob))

    fx = Image.new("RGBA", (cw, ch), CLR)
    chase = spot.get("chase", False)
    for i, (x, y, r, g, b) in enumerate(spot["sparks"]):
        phase = (frame + i) % 4
        if chase and abs((frame * 2) - i) % max(1, len(spot["sparks"])) > 3 and phase == 3:
            continue
        if phase == 3 and i % 2 == 0 and not chase:
            continue
        yy = y + [0, -1, -2, -1][phase]
        xx = x + (1 if phase == 2 else 0)
        bright = [0.6, 0.9, 1.1, 0.75][phase]
        put_spark(fx, xx, yy, (r, g, b), bright)

    for i, (hx, hy) in enumerate(spot.get("hearts", [])):
        if (frame + i) % 4 == 3:
            continue
        put_spark(fx, hx, hy - (frame % 2), (255, 140, 178), 0.85)

    return Image.alpha_composite(canvas.convert("RGBA"), fx)


def build_one(spot):
    still_path = os.path.join(OUT, spot["still"])
    dest = os.path.join(OUT, spot["subject"])
    if not os.path.exists(still_path):
        print("MISSING", still_path)
        return
    still = Image.open(still_path).convert("RGBA")
    base = to_pixel(crop_focus(still, spot["cw"], spot["ch"], spot.get("bias_y", 0.0)))
    tiles = [make_frame(base, f, spot) for f in range(4)]
    sheet = Image.new("RGBA", (spot["cw"] * 4, spot["ch"]), CLR)
    for i, t in enumerate(tiles):
        sheet.paste(t, (i * spot["cw"], 0))
    sheet.save(dest, optimize=True)
    print("OK", spot["subject"], sheet.size, os.path.getsize(dest))


def main():
    for spot in SPOTS:
        build_one(spot)
    print(f"done {len(SPOTS)} Act2 subjects (no people)")


if __name__ == "__main__":
    main()
