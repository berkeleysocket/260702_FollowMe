"""BridgeView subject — Act3 고퀄 스틸을 베이스로 4프레임 애니메이션 시트 제작.
셀 96x64 · Stage2/Act2 스틸과 동일 밀도.
"""
from PIL import Image, ImageEnhance, ImageChops, ImageFilter
import os

OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act3_Fireworks"
STILL = os.path.join(OUT, "Act3_PhotoStill_BridgeView.png")
DEST = os.path.join(OUT, "Act3_PhotoSubject_BridgeView_96x64.png")
CELL_W, CELL_H = 96, 64
CLR = (0, 0, 0, 0)

# 애니메이션용 점광 (불꽃/랜턴/하트)
SPARKS = [
    # (x, y, r,g,b) in cell coords — 프레임마다 위상만 바뀜
    (22, 10, 255, 96, 180),
    (48, 6, 255, 210, 90),
    (70, 8, 90, 230, 255),
    (35, 14, 180, 100, 255),
    (60, 12, 255, 140, 60),
    (18, 28, 255, 210, 110),  # lantern
    (78, 28, 255, 210, 110),
]


def crop_focus(img: Image.Image) -> Image.Image:
    """스틸에서 다리 난간·불꽃 구도가 살아있게 중앙-하단 크롭."""
    w, h = img.size
    # 16:9 스틸 → 3:2에 가깝게 가로 중심 + 약간 하단
    side = min(w, int(h * 1.5))
    left = (w - side) // 2
    top = max(0, (h - int(side * CELL_H / CELL_W)) // 2 - h // 20)
    box_h = int(side * CELL_H / CELL_W)
    top = min(top, h - box_h)
    crop = img.crop((left, top, left + side, top + box_h))
    return crop.resize((CELL_W, CELL_H), Image.Resampling.LANCZOS)


def to_pixel(img: Image.Image) -> Image.Image:
    """살짝 도트 느낌 — 중간 해상도에서 팔레트 정리 후 업."""
    # 다운→업으로 픽셀 클러스터감
    small = img.resize((CELL_W // 2 * 2, CELL_H // 2 * 2), Image.Resampling.BILINEAR)  # noop safety
    # 실제: 72x48로 줄였다가 니어스트로 복원하면 너무 뭉개짐 → 대신 샤프닝
    out = img.convert("RGBA")
    out = ImageEnhance.Color(out).enhance(1.15)
    out = ImageEnhance.Contrast(out).enhance(1.08)
    out = ImageEnhance.Sharpness(out).enhance(1.6)
    return out


def put_spark(layer: Image.Image, x, y, rgb, bright=1.0):
    r, g, b = rgb
    a = int(220 * bright)
    c = (r, g, b, a)
    w = (255, 255, 255, int(200 * bright))
    px = layer.load()
    for dx, dy, col in (
        (0, 0, w),
        (1, 0, c),
        (-1, 0, c),
        (0, 1, c),
        (0, -1, c),
        (1, -1, (r, g, b, int(120 * bright))),
        (-1, 1, (r, g, b, int(120 * bright))),
    ):
        xx, yy = x + dx, y + dy
        if 0 <= xx < CELL_W and 0 <= yy < CELL_H:
            px[xx, yy] = col


def make_frame(base: Image.Image, frame: int) -> Image.Image:
    bob = [0, -1, -2, -1][frame % 4]
    # 프레임 보빙: 세로 1~2px
    canvas = Image.new("RGBA", (CELL_W, CELL_H), CLR)
    canvas.paste(base, (0, bob))
    if bob < 0:
        # 빈 하단을 베이스 하단으로 채움
        strip = base.crop((0, CELL_H + bob, CELL_W, CELL_H))
        canvas.paste(strip, (0, CELL_H + bob))

    fx = Image.new("RGBA", (CELL_W, CELL_H), CLR)
    for i, (x, y, r, g, b) in enumerate(SPARKS):
        phase = (frame + i) % 4
        if phase == 3 and i % 2 == 0:
            continue  # 깜빡임
        yy = y + [0, -1, -2, -1][phase]
        bright = [0.7, 1.0, 1.15, 0.85][phase]
        put_spark(fx, x + (1 if phase == 2 else 0), yy, (r, g, b), bright)
        # 반사 점
        if y < 30:
            put_spark(fx, x, min(CELL_H - 3, 50 + (i % 5) + phase), (r, g, b), bright * 0.45)

    # 하트 파티클
    hearts = [(40, 16), (55, 9), (68, 18)]
    for i, (hx, hy) in enumerate(hearts):
        if (frame + i) % 4 == 3:
            continue
        put_spark(fx, hx, hy - (frame % 2), (255, 96, 180), 0.9)

    return Image.alpha_composite(canvas.convert("RGBA"), fx)


def main():
    still = Image.open(STILL).convert("RGBA")
    print("still", still.size, os.path.getsize(STILL))
    base = to_pixel(crop_focus(still))
    tiles = [make_frame(base, f) for f in range(4)]
    sheet = Image.new("RGBA", (CELL_W * 4, CELL_H), CLR)
    for i, t in enumerate(tiles):
        sheet.paste(t, (i * CELL_W, 0))
    sheet.save(DEST, optimize=True)
    print("saved", DEST, sheet.size, os.path.getsize(DEST))


if __name__ == "__main__":
    main()
