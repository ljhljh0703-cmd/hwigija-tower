#!/usr/bin/env python3
"""아트 대조 시트 — 실제 표시 크기로 줄여 실제 배경색 위에 깐다.

  python3 Tools/make_art_sheet.py
자동 채점이 판독성을 못 재는 영역을 사람이 3초에 훑게 하는 도구.
"""
from PIL import Image, ImageDraw, ImageFont
import glob, os, sys, json

GROUND=(0x17,0x12,0x0D); PAD=18; LABELH=26
OUT="Docs/Portfolio/assets/concept-to-ui"

def sheet(paths, title, disp, out, cols=8):
    CELL=disp; COLS=cols
    rows=max(1,(len(paths)+COLS-1)//COLS)
    W=COLS*(CELL+PAD)+PAD; H=rows*(CELL+PAD+LABELH)+PAD+44
    im=Image.new("RGB",(W,H),GROUND); d=ImageDraw.Draw(im)
    try:
        f=ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",11)
        ft=ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",16)
    except Exception:
        f=ft=ImageFont.load_default()
    d.text((PAD,14), title, fill=(0xC9,0xA2,0x4A), font=ft)
    for i,p in enumerate(paths):
        c,r=i%COLS,i//COLS
        x=PAD+c*(CELL+PAD); y=44+PAD+r*(CELL+PAD+LABELH)
        d.rectangle([x-1,y-1,x+CELL,y+CELL], outline=(0x33,0x2A,0x1E))
        try:
            a=Image.open(p).convert("RGBA"); a.thumbnail((disp,disp), Image.LANCZOS)
            im.paste(a,(x+(CELL-a.width)//2, y+(CELL-a.height)//2), a)
        except Exception: pass
        n=os.path.basename(p)[:-4]
        d.text((x,y+CELL+5), (n[:15]+"…") if len(n)>16 else n, fill=(0xA2,0x95,0x7F), font=f)
    im.save(out); return len(paths)

GROUPS=[
 ("아이콘 · 실제 96px", sorted(glob.glob('Assets/_Project/Art/UI/*Actions/*.png'))+sorted(glob.glob('Assets/_Project/Art/Icons/*.png')), 96, f"{OUT}/art_sheet_icons.png"),
 ("맵 노드 · 96px",     sorted(glob.glob('Assets/_Project/Art/Nodes/*.png')), 96, f"{OUT}/art_sheet_nodes.png"),
 ("적 · 96px 축소",     sorted(glob.glob('Assets/_Project/Art/Enemies/*.png')), 96, f"{OUT}/art_sheet_enemies.png"),
 ("인물 · 실제 300px 표시", sorted(glob.glob('Assets/_Project/Art/Characters/**/*.png', recursive=True)), 300, f"{OUT}/art_sheet_characters.png", 5),
]
if __name__=="__main__":
    for g in GROUPS:
        t,ps,disp,out = g[0],g[1],g[2],g[3]
        cols = g[4] if len(g)>4 else 8
        if ps: print(f"{sheet(ps,t,disp,out,cols):>3}장 → {out}")
