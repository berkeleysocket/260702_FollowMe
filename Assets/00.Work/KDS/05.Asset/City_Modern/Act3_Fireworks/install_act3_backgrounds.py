"""Install Act3 parallax background layers into KDS Act3_Fireworks folder."""
from PIL import Image, ImageEnhance
import os

SRC = r"C:\Users\김동선\.cursor\projects\c-Github-260702-FollowMe\assets"
OUT = r"C:\Github\260702_FollowMe\Assets\00.Work\KDS\05.Asset\City_Modern\Act3_Fireworks"

# keep full frame — backgrounds are opaque strips
LAYERS = {
    "Act3_BG_SkyFireworks.png": (512, 256),
    "Act3_BG_CitySkyline.png": (512, 192),
    "Act3_BG_RiverWater.png": (512, 160),
    "Act3_BG_Promenade.png": (512, 192),
    "Act3_BG_EmptyVoid.png": (512, 192),
    "Act3_BG_FestivalLights.png": (512, 128),
    # refresh old name as alias of sky
    "Act3_BG_NightSky.png": (512, 256),
}


def soft_trim_white(img: Image.Image) -> Image.Image:
    """Only clear pure white margins; keep painted sky/water intact."""
    img = img.convert("RGBA")
    px = img.load()
    w, h = img.size
    for y in range(h):
        for x in range(w):
            r, g, b, a = px[x, y]
            if r >= 250 and g >= 250 and b >= 250:
                # only if near edge fringe
                if x < 3 or y < 3 or x >= w - 3 or y >= h - 3:
                    px[x, y] = (0, 0, 0, 0)
    return img


def process(name: str, size):
    src_name = name
    if name == "Act3_BG_NightSky.png":
        src_name = "Act3_BG_SkyFireworks.png"
    path = os.path.join(SRC, src_name)
    if not os.path.isfile(path):
        print("missing", src_name)
        return False
    img = Image.open(path)
    img = soft_trim_white(img)
    img = img.resize(size, Image.Resampling.LANCZOS)
    img = ImageEnhance.Color(img).enhance(1.08)
    img = ImageEnhance.Contrast(img).enhance(1.05)
    img = ImageEnhance.Sharpness(img).enhance(1.15)
    dest = os.path.join(OUT, name)
    img.save(dest)
    print(f"bg {name} -> {img.size}")
    return True


def main():
    ok = 0
    for name, size in LAYERS.items():
        if process(name, size):
            ok += 1
    print(f"done {ok}/{len(LAYERS)}")


if __name__ == "__main__":
    main()
