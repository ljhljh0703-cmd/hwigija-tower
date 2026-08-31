#!/usr/bin/env python3
"""화면 사양 채점기 — game-ui-from-concept 4단.

사용법
  python3 Tools/check_layout.py --spec <spec.json>
      사양 자체 검사. 지시를 내보내기 전에 돌린다.
  python3 Tools/check_layout.py --spec <spec.json> --actual <actual_layout.json>
      런타임이 뱉은 실제 좌표와 사양을 대조한다.
  옵션 --rejections <file>   FAIL 시 반려 기록에 append
       --screen-note "<한 줄>"  기록에 남길 메모

원칙
  - 캡처 픽셀을 분석하지 않는다. 런타임이 자기 좌표를 보고하게 한다.
  - 사양을 고쳐서 통과시키지 않는다. 이 스크립트는 판정만 하고 사양을 수정하지 않는다.
종료 코드
  0 = PASS, 1 = FAIL, 2 = 입력 오류
"""
import argparse, json, sys, datetime, io, os

REF_H = 1920

# ---------- 슬롯 헬퍼 ----------

def rect(s):
    return (s.get("x", 0.0), s.get("y", 0.0),
            s.get("x", 0.0) + s.get("w", 0.0), s.get("y", 0.0) + s.get("h", 0.0))

def overlaps(a, b):
    ax0, ay0, ax1, ay1 = a; bx0, by0, bx1, by1 = b
    ix0, iy0 = max(ax0, bx0), max(ay0, by0)
    ix1, iy1 = min(ax1, bx1), min(ay1, by1)
    if ix1 > ix0 and iy1 > iy0:
        return (ix0, iy0, ix1, iy1)
    return None

def matches(slot, sel):
    if not sel:
        return True
    if "layer" in sel and slot.get("layer") not in sel["layer"]:
        return False
    if "role" in sel and slot.get("role") not in sel["role"]:
        return False
    if "interactive" in sel and bool(slot.get("interactive")) != sel["interactive"]:
        return False
    if "hasField" in sel and not slot.get(sel["hasField"]):
        return False
    return True

def selected(slots, sel, exempt_layers=None):
    out = {}
    for name, s in slots.items():
        if exempt_layers and s.get("layer") in exempt_layers:
            continue
        if matches(s, sel):
            out[name] = s
    return out

# ---------- 규칙 ----------

def r_min_y(slots, mc):
    bad = []
    sel = mc.get("applyTo", {"interactive": True})
    for n, s in selected(slots, sel, mc.get("exemptLayers")).items():
        if n in mc.get("exemptSlots", []):
            continue
        if s.get("y", 0) < mc["value"]:
            bad.append(f"{n} y={s['y']:.3f} < {mc['value']}")
    return bad

def r_count(slots, mc):
    n = len(selected(slots, mc.get("applyTo", {})))
    if "eq" in mc and n != mc["eq"]:
        return [f"해당 슬롯 {n}개, 기대 {mc['eq']}개"]
    if "max" in mc and n > mc["max"]:
        return [f"해당 슬롯 {n}개, 상한 {mc['max']}개"]
    return []

def r_no_overlap(slots, mc):
    reg = mc["region"]
    region = (reg["x0"], reg["y0"], reg["x1"], reg["y1"])
    bad = []
    anyof = mc.get("applyToAnyOf") or [mc.get("applyTo", {})]
    seen = set()
    for sel in anyof:
        for n, s in selected(slots, sel, mc.get("exemptLayers")).items():
            if n in seen or n in mc.get("exemptSlots", []):
                continue
            seen.add(n)
            it = overlaps(rect(s), region)
            if it:
                bad.append(f"{n} 교집합 x={it[0]:.3f}~{it[2]:.3f}, y={it[1]:.3f}~{it[3]:.3f}")
    return bad

def r_text_absent(slots, mc):
    needle = mc["value"]
    return [f"{n} content 에 「{needle}」" for n, s in slots.items()
            if needle in str(s.get("content", ""))]

def r_field_required(slots, mc):
    return [f"{n} 에 {mc['field']} 없음"
            for n, s in selected(slots, mc.get("applyTo", {})).items()
            if not s.get(mc["field"])]

def r_field_absent(slots, mc):
    return [f"{n} 에 {mc['field']} 있음(금지)"
            for n, s in selected(slots, mc.get("applyTo", {})).items()
            if s.get(mc["field"])]

def r_min_touch(slots, mc):
    bad = []
    for n, s in selected(slots, mc.get("applyTo", {"interactive": True})).items():
        px = s.get("h", 0) * REF_H
        if px < mc["value"]:
            bad.append(f"{n} 높이 {px:.0f}px < {mc['value']}px")
    return bad

def r_band_order(slots, mc, spec=None):
    order = list((spec or {}).get("bands", {}).keys())
    if order != mc["value"]:
        return [f"층 순서 {order} != 기대 {mc['value']}"]
    return []

def r_relative(slots, mc):
    a, b = slots.get(mc["a"]), slots.get(mc["b"])
    if not a or not b:
        return [f"슬롯 없음: {mc['a']} 또는 {mc['b']}"]
    va, vb = a.get(mc["field"], 0), b.get(mc["field"], 0)
    op = mc["op"]
    ok = (va < vb) if op == "lt" else (va > vb) if op == "gt" else None
    if ok is None:
        return [f"알 수 없는 연산 {op}"]
    return [] if ok else [f"{mc['a']}.{mc['field']}={va} {op} {mc['b']}.{mc['field']}={vb} 불만족"]


def r_field_bound(slots, mc):
    s = slots.get(mc["slot"])
    if not s:
        return [f"슬롯 없음: {mc['slot']}"]
    v = s.get(mc["field"])
    if v is None:
        return [f"{mc['slot']}.{mc['field']} 없음"]
    if "lt" in mc and not v < mc["lt"]:
        return [f"{mc['slot']}.{mc['field']}={v} < {mc['lt']} 불만족"]
    if "gt" in mc and not v > mc["gt"]:
        return [f"{mc['slot']}.{mc['field']}={v} > {mc['gt']} 불만족"]
    return []

def r_field_equals(slots, mc):
    s = slots.get(mc["slot"])
    if not s:
        return [f"슬롯 없음: {mc['slot']}"]
    v = s.get(mc["field"])
    if v != mc["value"]:
        return [f"{mc['slot']}.{mc['field']}={v!r}, 기대 {mc['value']!r}"]
    return []

RULES = {
    "minY": r_min_y, "count": r_count, "noOverlap": r_no_overlap,
    "textAbsent": r_text_absent, "fieldRequired": r_field_required,
    "fieldAbsent": r_field_absent, "minTouchPx": r_min_touch,
    "bandOrder": r_band_order, "relative": r_relative,
    "fieldBound": r_field_bound, "fieldEquals": r_field_equals,
}

# ---------- 실행 ----------

def check_spec(spec):
    slots, results = spec.get("slots", {}), []
    for c in spec.get("constraints", []):
        mc = c.get("machineCheck")
        if not mc:
            results.append((c["id"], "SKIP", ["machineCheck 없음 — 사람이 봐야 한다"]))
            continue
        fn = RULES.get(mc.get("type"))
        if not fn:
            results.append((c["id"], "SKIP", [f"알 수 없는 규칙 {mc.get('type')}"]))
            continue
        bad = fn(slots, mc, spec) if mc["type"] == "bandOrder" else fn(slots, mc)
        results.append((c["id"], "FAIL" if bad else "PASS", bad))
    return results

def check_actual(spec, actual):
    """런타임 보고 좌표를 사양과 대조."""
    tol = spec.get("tolerance", {})
    off = tol.get("slotOffsetPx", 8) / REF_H
    fs_pct = tol.get("fontScalePct", 5) / 100.0
    out, aslots = [], actual.get("slots", {})

    for name in tol.get("requiredVisible", []):
        a = aslots.get(name)
        if not a or not a.get("visible", True):
            out.append((f"visible:{name}", "FAIL", ["캡처에 없음"]))
        else:
            out.append((f"visible:{name}", "PASS", []))

    for name, s in spec.get("slots", {}).items():
        a = aslots.get(name)
        if a is None:
            out.append((f"slot:{name}", "FAIL", ["런타임 보고에 없음"]))
            continue
        bad = []
        for f in ("x", "y", "w", "h"):
            if f in s and f in a and abs(s[f] - a[f]) > off:
                bad.append(f"{f} 사양 {s[f]:.3f} ⟂ 실제 {a[f]:.3f} (허용 {off:.4f})")
        if "fontSize" in s and "fontSize" in a and s["fontSize"]:
            if abs(a["fontSize"] - s["fontSize"]) / s["fontSize"] > fs_pct:
                bad.append(f"fontSize 사양 {s['fontSize']} ⟂ 실제 {a['fontSize']}")
        out.append((f"slot:{name}", "FAIL" if bad else "PASS", bad))
    return out

def render(results):
    npass = sum(1 for _, v, _ in results if v == "PASS")
    nfail = sum(1 for _, v, _ in results if v == "FAIL")
    nskip = sum(1 for _, v, _ in results if v == "SKIP")
    lines = []
    for rid, verdict, bad in results:
        mark = {"PASS": "PASS", "FAIL": "FAIL", "SKIP": "SKIP"}[verdict]
        lines.append(f"  [{mark}] {rid}")
        for b in bad:
            lines.append(f"         - {b}")
    return "\n".join(lines), npass, nfail, nskip

def append_rejection(path, screen, rnd, results, note):
    today = datetime.date.today().isoformat()
    fails = [(rid, bad) for rid, v, bad in results if v == "FAIL"]
    if not fails:
        return
    body = [f"\n### {today} · {screen} · 라운드 {rnd} · 자동 채점"]
    for rid, bad in fails:
        body.append(f"- 위반: **{rid}**")
        for b in bad:
            body.append(f"  - {b}")
    body.append("- 처리: 3단 재위탁 (사양 수정 아님)")
    body.append("- 승격 후보: no")
    if note:
        body.append(f"- 비고: {note}")
    with io.open(path, "a", encoding="utf-8") as f:
        f.write("\n".join(body) + "\n")

def main():
    p = argparse.ArgumentParser()
    p.add_argument("--spec", required=True)
    p.add_argument("--actual")
    p.add_argument("--rejections")
    p.add_argument("--round", default="?")
    p.add_argument("--screen-note", default="")
    a = p.parse_args()

    try:
        spec = json.load(io.open(a.spec, encoding="utf-8"))
    except Exception as e:
        print(f"사양을 못 읽었다: {e}"); return 2

    screen = spec.get("screen", "?")
    print(f"\n=== 사양 자체 검사 · {screen} ===")
    res = check_spec(spec)
    txt, np_, nf, ns = render(res)
    print(txt)
    print(f"  → PASS {np_} · FAIL {nf} · SKIP {ns}")

    all_res = list(res)
    if a.actual:
        try:
            actual = json.load(io.open(a.actual, encoding="utf-8"))
        except Exception as e:
            print(f"런타임 보고를 못 읽었다: {e}"); return 2
        print(f"\n=== 런타임 대조 · {screen} ===")
        res2 = check_actual(spec, actual)
        txt2, np2, nf2, ns2 = render(res2)
        print(txt2)
        print(f"  → PASS {np2} · FAIL {nf2}")
        all_res += res2
        nf += nf2

    verdict = "FAIL" if nf else "PASS"
    print(f"\n판정: {verdict}\n")
    if a.rejections and nf:
        append_rejection(a.rejections, screen, a.round, all_res, a.screen_note)
        print(f"반려 기록 append: {a.rejections}\n")
    return 1 if nf else 0

if __name__ == "__main__":
    sys.exit(main())
