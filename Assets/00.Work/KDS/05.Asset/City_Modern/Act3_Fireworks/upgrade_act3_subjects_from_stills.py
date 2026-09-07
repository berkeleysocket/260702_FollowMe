"""Act3 photo subjects — 고퀄 스틸 베이스 + 4프레임 스파크/보빙 애니메이션.
BridgeView와 동일 파이프라인. 스틸은 덮어쓰지 않음.
"""
from PIL import Image, ImageEnhance
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act3_Fireworks"
CLR = (0, 0, 0, 0)

# id, still, subject, cell_w, cell_h, sparks[(x,y,r,g,b)...], hearts[(x,y)...], crop bias
SPOTS = [
    {
        "still": "Act3_PhotoStill_BridgeView.png",
        "subject": "Act3_PhotoSubject_BridgeView_96x64.png",
        "cw": 96, "ch": 64, "bias_y": -0.05,
        "sparks": [
            (22, 10, 255, 96, 180), (48, 6, 255, 210, 90), (70, 8, 90, 230, 255),
            (35, 14, 180, 100, 255), (60, 12, 255, 140, 60),
            (18, 28, 255, 210, 110), (78, 28, 255, 210, 110),
        ],
        "hearts": [(40, 16), (55, 9), (68, 18)],
        "reflect_y": 50,
    },
    {
        "still": "Act3_PhotoStill_FireworkPeak.png",
        "subject": "Act3_PhotoSubject_FireworkPeak_64x96.png",
        "cw": 64, "ch": 96, "bias_y": -0.1,
        "sparks": [
            (32, 18, 255, 96, 180), (18, 28, 255, 210, 90), (48, 24, 90, 230, 255),
            (28, 40, 180, 100, 255), (40, 36, 255, 140, 60), (22, 14, 255, 255, 255),
            (44, 16, 255, 96, 180), (32, 50, 90, 230, 255),
        ],
        "hearts": [(20, 22), (44, 20), (32, 12)],
        "reflect_y": 72,
    },
    {
        "still": "Act3_PhotoStill_PhoneCrowd.png",
        "subject": "Act3_PhotoSubject_PhoneCrowd_96x64.png",
        "cw": 96, "ch": 64, "bias_y": 0.05,
        "sparks": [
            (20, 40, 90, 230, 255), (36, 38, 255, 96, 180), (52, 40, 255, 210, 90),
            (68, 38, 90, 230, 255), (80, 40, 255, 96, 180),
            (48, 8, 255, 96, 180), (70, 10, 255, 210, 90), (28, 6, 90, 230, 255),
        ],
        "hearts": [(44, 14), (60, 10)],
        "reflect_y": 52,
    },
    {
        "still": "Act3_PhotoStill_FoodTruck.png",
        "subject": "Act3_PhotoSubject_FoodTruck_96x64.png",
        "cw": 96, "ch": 64, "bias_y": 0.0,
        "sparks": [
            (30, 18, 255, 230, 180), (40, 14, 255, 255, 255), (48, 16, 255, 210, 130),
            (74, 12, 255, 96, 100), (84, 12, 255, 140, 80),
            (36, 28, 255, 220, 140), (50, 8, 255, 210, 90),
        ],
        "hearts": [(22, 10), (60, 8)],
        "reflect_y": 54,
    },
    {
        "still": "Act3_PhotoStill_DroneShow.png",
        "subject": "Act3_PhotoSubject_DroneShow_96x64.png",
        "cw": 96, "ch": 64, "bias_y": -0.08,
        "sparks": [
            (48, 16, 255, 96, 180), (40, 22, 90, 230, 255), (56, 22, 255, 210, 90),
            (34, 28, 180, 100, 255), (62, 28, 90, 230, 255), (48, 32, 255, 96, 180),
            (28, 20, 255, 255, 255), (68, 18, 90, 230, 255), (48, 10, 255, 210, 90),
        ],
        "hearts": [(48, 20), (42, 26), (54, 26)],
        "reflect_y": 48,
    },
    {
        "still": "Act3_PhotoStill_PhotoBooth.png",
        "subject": "Act3_PhotoSubject_PhotoBooth_64x96.png",
        "cw": 64, "ch": 96, "bias_y": 0.0,
        "sparks": [
            (32, 20, 255, 255, 255), (20, 28, 255, 210, 110), (44, 28, 255, 210, 110),
            (20, 40, 255, 210, 110), (44, 40, 255, 210, 110),
            (32, 8, 255, 96, 205), (28, 50, 255, 96, 180), (36, 50, 90, 230, 255),
        ],
        "hearts": [(16, 14), (48, 14)],
        "reflect_y": None,
        "flash_frames": (1, 2),
    },
    {
        "still": "Act3_PhotoStill_PierBoat.png",
        "subject": "Act3_PhotoSubject_PierBoat_96x64.png",
        "cw": 96, "ch": 64, "bias_y": 0.05,
        "sparks": [
            (60, 28, 255, 220, 140), (70, 26, 255, 210, 110), (80, 28, 255, 220, 140),
            (72, 8, 255, 96, 180), (50, 10, 255, 210, 90),
            (20, 36, 90, 230, 255), (30, 34, 255, 210, 90),
        ],
        "hearts": [(40, 12), (64, 10)],
        "reflect_y": 48,
        "bob_extra": True,
    },
    {
        "still": "Act3_PhotoStill_EmptyRiver.png",
        "subject": "Act3_PhotoSubject_EmptyRiver_96x64.png",
        "cw": 96, "ch": 64, "bias_y": 0.05,
        "sparks": [
            (40, 12, 180, 100, 80), (48, 16, 140, 90, 70),
            (70, 36, 255, 140, 60), (30, 40, 100, 120, 140),
        ],
        "hearts": [],
        "reflect_y": 52,
        "subtle": True,
    },
    {
        "still": "Act3_PhotoStill_BridgeLights.png",
        "subject": "Act3_PhotoSubject_BridgeLights_96x64.png",
        "cw": 96, "ch": 64, "bias_y": -0.05,
        "sparks": [
            (16, 24, 255, 96, 180), (28, 22, 90, 230, 255), (40, 20, 255, 210, 90),
            (52, 20, 180, 100, 255), (64, 22, 90, 230, 255), (76, 24, 255, 96, 180),
            (20, 12, 255, 210, 90), (80, 12, 90, 230, 255),
        ],
        "hearts": [(48, 10), (36, 14)],
        "reflect_y": 48,
        "chase": True,
    },
    {
        "still": "Act3_PhotoStill_WaterReflect.png",
        "subject": "Act3_PhotoSubject_WaterReflect_96x64.png",
        "cw": 96, "ch": 64, "bias_y": 0.15,
        "sparks": [
            (24, 20, 255, 96, 180), (48, 16, 90, 230, 255), (72, 22, 255, 210, 90),
            (32, 36, 255, 96, 180), (48, 40, 90, 230, 255), (68, 38, 255, 210, 90),
            (40, 48, 180, 100, 255), (60, 50, 255, 140, 60),
        ],
        "hearts": [(50, 12)],
        "reflect_y": 42,
    },
]


def crop_focus(img: Image.Image, cw: int, ch: int, bias_y: float) -> Image.Image:
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
    crop = img.crop((left, top, left + box_w, top + box_h))
    return crop.resize((cw, ch), Image.Resampling.LANCZOS)


def to_pixel(img: Image.Image) -> Image.Image:
    out = img.convert("RGBA")
    out = ImageEnhance.Color(out).enhance(1.12)
    out = ImageEnhance.Contrast(out).enhance(1.06)
    out = ImageEnhance.Sharpness(out).enhance(1.55)
    return out


def put_spark(layer: Image.Image, x, y, rgb, bright=1.0):
    r, g, b = rgb
    a = int(220 * bright)
    c = (r, g, b, a)
    wcol = (255, 255, 255, int(200 * bright))
    px = layer.load()
    cw, ch = layer.size
    for dx, dy, col in (
        (0, 0, wcol),
        (1, 0, c),
        (-1, 0, c),
        (0, 1, c),
        (0, -1, c),
        (1, -1, (r, g, b, int(110 * bright))),
        (-1, 1, (r, g, b, int(110 * bright))),
    ):
        xx, yy = x + dx, y + dy
        if 0 <= xx < cw and 0 <= yy < ch:
            px[xx, yy] = col


def make_frame(base: Image.Image, frame: int, spot: dict) -> Image.Image:
    cw, ch = spot["cw"], spot["ch"]
    bob_amp = 2 if spot.get("bob_extra") else 1
    bob = [0, -bob_amp, -bob_amp * 2, -bob_amp][frame % 4]
    if spot.get("subtle"):
        bob = [0, 0, -1, 0][frame % 4]

    canvas = Image.new("RGBA", (cw, ch), CLR)
    canvas.paste(base, (0, bob))
    if bob < 0:
        strip = base.crop((0, ch + bob, cw, ch))
        canvas.paste(strip, (0, ch + bob))

    fx = Image.new("RGBA", (cw, ch), CLR)
    sparks = spot["sparks"]
    subtle = spot.get("subtle", False)
    chase = spot.get("chase", False)
    flash_frames = spot.get("flash_frames")

    for i, (x, y, r, g, b) in enumerate(sparks):
        phase = (frame + i) % 4
        if chase:
            # chase: only nearby lights on
            if abs((frame * 2) - i) % len(sparks) > 2 and phase == 3:
                continue
        elif subtle:
            if phase not in (0, 1):
                continue
        elif phase == 3 and i % 2 == 0:
            continue

        yy = y + [0, -1, -2, -1][phase]
        xx = x + (1 if phase == 2 and not subtle else 0)
        bright = [0.55, 0.85, 1.1, 0.7][phase]
        if subtle:
            bright *= 0.55
        put_spark(fx, xx, yy, (r, g, b), bright)

        ry = spot.get("reflect_y")
        if ry is not None and y < ry - 8:
            put_spark(
                fx,
                x,
                min(ch - 2, ry + (i % 5) + phase),
                (r, g, b),
                bright * (0.35 if not subtle else 0.2),
            )

    for i, (hx, hy) in enumerate(spot.get("hearts", [])):
        if (frame + i) % 4 == 3:
            continue
        put_spark(fx, hx, hy - (frame % 2), (255, 96, 180), 0.85)

    # photo booth flash wash
    if flash_frames and frame in flash_frames:
        wash = Image.new("RGBA", (cw, ch), (255, 255, 255, 35 if frame == 1 else 55))
        canvas = Image.alpha_composite(canvas, wash)
        put_spark(fx, cw // 2, ch // 4, (255, 255, 255), 1.2)

    return Image.alpha_composite(canvas.convert("RGBA"), fx)


def build_one(spot: dict) -> None:
    still_path = os.path.join(OUT, spot["still"])
    dest = os.path.join(OUT, spot["subject"])
    if not os.path.exists(still_path):
        print("MISSING still", still_path)
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
    print(f"done {len(SPOTS)} subjects")


if __name__ == "__main__":
    main()
