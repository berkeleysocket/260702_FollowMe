"""Upgrade Act3 props: copy stil-referenced GenerateImage outputs into KDS folder
with chroma-key transparency, content crop, and game-ready sizing.
"""
from PIL import Image, ImageEnhance
import os
import shutil

SRC = r"C:\Users\김동선\.cursor\projects\c-Github-260702-FollowMe\assets"
OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act3_Fireworks"

# name -> max (w, h)
SIZES = {
    "Act3_Prop_FoodStall.png": (96, 96),
    "Act3_Prop_RiverRail.png": (128, 64),
    "Act3_Prop_BridgePillar.png": (48, 128),
    "Act3_Prop_CrowdSilhouette.png": (128, 64),
    "Act3_Prop_ParkBench.png": (80, 48),
    "Act3_Prop_LanternPole.png": (40, 112),
    "Act3_Prop_FestivalFlag.png": (56, 112),
    "Act3_Prop_PierPlank.png": (128, 48),
    "Act3_Prop_TreeAutumn.png": (80, 128),
    "Act3_Prop_StringLanterns.png": (144, 48),
    "Act3_Prop_VendorCart.png": (96, 72),
    "Act3_Prop_TrashBin.png": (40, 48),
    "Act3_Prop_SpeakerStack.png": (48, 80),
    "Act3_Prop_BarrierCone.png": (32, 48),
    "Act3_Prop_PhotoFrame.png": (64, 96),
    "Act3_Prop_EmptyBench.png": (80, 48),
    "Act3_Prop_RiverEdge.png": (128, 40),
    "Act3_Prop_NightBush.png": (72, 48),
    "Act3_BG_NightSky.png": (256, 128),
    "Act3_Tile_NightPath.png": (64, 64),
    "Act3_Tile_BridgeDeck.png": (64, 32),
    "Act3_Hazard_Ember.png": (24, 24),
    "Act3_Hazard_CrowdBump.png": (64, 48),
    "Act3_Vfx_FireworkWindow.png": (64, 64),
}


def is_bg(px, corners_avg):
    r, g, b, a = px
    if a < 8:
        return True
    # white / near-white studio
    if r >= 230 and g >= 230 and b >= 230:
        return True
    if r >= 215 and g >= 215 and b >= 215 and abs(r - g) < 12 and abs(g - b) < 12:
        return True
    # near pure black / dark checker
    if r < 18 and g < 18 and b < 18:
        return True
    # checkerboard grey
    if abs(r - g) < 8 and abs(g - b) < 8 and 28 <= r <= 55:
        return True
    # close to sampled corner average (studio backdrop)
    cr, cg, cb = corners_avg
    if abs(r - cr) + abs(g - cg) + abs(b - cb) < 36 and r + g + b < 90:
        return True
    if abs(r - cr) + abs(g - cg) + abs(b - cb) < 40 and r + g + b > 600:
        return True
    return False


def corner_avg(img):
    w, h = img.size
    pts = [(0, 0), (w - 1, 0), (0, h - 1), (w - 1, h - 1), (w // 2, 0), (0, h // 2)]
    rs = gs = bs = 0
    for x, y in pts:
        r, g, b, a = img.getpixel((x, y))
        rs += r
        gs += g
        bs += b
    n = len(pts)
    return rs // n, gs // n, bs // n


def key_out(img: Image.Image) -> Image.Image:
    img = img.convert("RGBA")
    avg = corner_avg(img)
    px = img.load()
    w, h = img.size
    for y in range(h):
        for x in range(w):
            if is_bg(px[x, y], avg):
                px[x, y] = (0, 0, 0, 0)
    return img


def content_bbox(img: Image.Image, pad=2):
    alpha = img.split()[-1]
    bbox = alpha.getbbox()
    if not bbox:
        return img
    x0, y0, x1, y1 = bbox
    x0 = max(0, x0 - pad)
    y0 = max(0, y0 - pad)
    x1 = min(img.width, x1 + pad)
    y1 = min(img.height, y1 + pad)
    return img.crop((x0, y0, x1, y1))


def fit(img: Image.Image, max_w: int, max_h: int) -> Image.Image:
    img.thumbnail((max_w, max_h), Image.Resampling.LANCZOS)
    # optional slight pixel crispness
    out = ImageEnhance.Sharpness(img).enhance(1.25)
    out = ImageEnhance.Color(out).enhance(1.08)
    out = ImageEnhance.Contrast(out).enhance(1.05)
    return out


def process(name: str):
    src = os.path.join(SRC, name)
    if not os.path.isfile(src):
        print("missing src", name)
        return False
    img = Image.open(src)
    img = key_out(img)
    img = content_bbox(img)
    mw, mh = SIZES.get(name, (96, 96))
    img = fit(img, mw, mh)
    dest = os.path.join(OUT, name)
    img.save(dest)
    print(f"upgraded {name} -> {img.size}")
    return True


def main():
    ok = 0
    for name in SIZES:
        if process(name):
            ok += 1
    print(f"done {ok}/{len(SIZES)}")


if __name__ == "__main__":
    main()
