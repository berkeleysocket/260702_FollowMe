"""Remove white/light studio backgrounds from Act3 prop sprites (edge flood-fill)."""
from PIL import Image, ImageEnhance
import os
from collections import deque

SRC = r"C:\Users\김동선\.cursor\projects\c-Github-260702-FollowMe\assets"
OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act3_Fireworks"

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


def is_backdrop(r, g, b, a):
    if a < 10:
        return True
    # white / near-white
    if r >= 235 and g >= 235 and b >= 235:
        return True
    if r >= 220 and g >= 220 and b >= 220 and abs(r - g) < 12 and abs(g - b) < 12:
        return True
    # light grey checker
    if abs(r - g) < 10 and abs(g - b) < 10 and r >= 200:
        return True
    # near black studio
    if r < 20 and g < 20 and b < 20:
        return True
    if abs(r - g) < 8 and abs(g - b) < 8 and 25 <= r <= 60:
        return True
    return False


def flood_key(img: Image.Image) -> Image.Image:
    """Make backdrop transparent by flooding from image edges."""
    img = img.convert("RGBA")
    w, h = img.size
    px = img.load()
    visited = [[False] * w for _ in range(h)]
    q = deque()

    def try_push(x, y):
        if 0 <= x < w and 0 <= y < h and not visited[y][x]:
            r, g, b, a = px[x, y]
            if is_backdrop(r, g, b, a):
                visited[y][x] = True
                q.append((x, y))

    for x in range(w):
        try_push(x, 0)
        try_push(x, h - 1)
    for y in range(h):
        try_push(0, y)
        try_push(w - 1, y)

    while q:
        x, y = q.popleft()
        px[x, y] = (0, 0, 0, 0)
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
            try_push(x + dx, y + dy)

    # second pass: any remaining near-white islands that are mostly backdrop-sized
    # soften fringe: near-white pixels adjacent to transparent become transparent
    for _ in range(2):
        for y in range(h):
            for x in range(w):
                r, g, b, a = px[x, y]
                if a == 0:
                    continue
                if not (r >= 210 and g >= 210 and b >= 210):
                    continue
                for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                    nx, ny = x + dx, y + dy
                    if 0 <= nx < w and 0 <= ny < h and px[nx, ny][3] == 0:
                        px[x, y] = (0, 0, 0, 0)
                        break
    return img


def content_bbox(img: Image.Image, pad=2):
    bbox = img.split()[-1].getbbox()
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
    out = ImageEnhance.Sharpness(img).enhance(1.2)
    out = ImageEnhance.Color(out).enhance(1.06)
    return out


def load_source(name: str) -> Image.Image:
    src = os.path.join(SRC, name)
    if os.path.isfile(src):
        return Image.open(src)
    # fallback: current OUT (already partially keyed)
    dest = os.path.join(OUT, name)
    return Image.open(dest)


def process(name: str):
    img = load_source(name)
    # Night sky / tiles: keep full rectangle, only key pure white margins if any
    if name.startswith("Act3_BG_") or name.startswith("Act3_Tile_"):
        img = img.convert("RGBA")
        # only remove pure white border via flood
        img = flood_key(img)
        # if sky became empty, restore from source without key
        if img.split()[-1].getbbox() is None:
            img = load_source(name).convert("RGBA")
    else:
        img = flood_key(img)
        img = content_bbox(img)

    mw, mh = SIZES.get(name, (96, 96))
    img = fit(img, mw, mh)
    dest = os.path.join(OUT, name)
    img.save(dest)
    # verify corners transparent for props
    w, h = img.size
    corners = [img.getpixel((0, 0)), img.getpixel((w - 1, 0))]
    print(f"{name} size={img.size} cornerA={corners[0][3]}")
    return True


def main():
    ok = 0
    for name in SIZES:
        try:
            if process(name):
                ok += 1
        except Exception as e:
            print("FAIL", name, e)
    print(f"done {ok}/{len(SIZES)}")


if __name__ == "__main__":
    main()
