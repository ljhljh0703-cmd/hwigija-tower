#!/usr/bin/env python3
"""Analyze RoguelikeSim seed replay CSVs.

The default path uses only the Python standard library so the AI QA track does
not add project dependencies. Matplotlib charting is optional.
"""

from __future__ import annotations

import argparse
import csv
from collections import Counter, defaultdict
from dataclasses import dataclass
from pathlib import Path
from statistics import mean


@dataclass(frozen=True)
class ReplayRow:
    seed: int
    policy: str
    win_loss: str
    floor_reached: int
    turns_to_kill: int
    remaining_hp: int
    gold: int
    item_picks: str
    ability_picks: str
    synergy_completion: str
    boss_result: str


@dataclass(frozen=True)
class PolicySummary:
    policy: str
    runs: int
    win_rate: float
    avg_floor_reached: float
    avg_turns_to_kill: float
    avg_remaining_hp: float
    avg_gold: float
    boss_win_rate: float
    synergy_completion_rate: float


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--input", required=True, help="Path to seed_replay.csv")
    parser.add_argument("--output-dir", default="Tools/RoguelikeSimPython/output")
    return parser.parse_args()


def read_rows(path: Path) -> list[ReplayRow]:
    with path.open("r", encoding="utf-8", newline="") as handle:
        reader = csv.DictReader(handle)
        return [
            ReplayRow(
                seed=int(row["seed"]),
                policy=row["policy"],
                win_loss=row["win_loss"],
                floor_reached=int(row["floor_reached"]),
                turns_to_kill=int(row["turns_to_kill"]),
                remaining_hp=int(row["remaining_hp"]),
                gold=int(row["gold"]),
                item_picks=row["item_picks"],
                ability_picks=row["ability_picks"],
                synergy_completion=row["synergy_completion"],
                boss_result=row["boss_result"],
            )
            for row in reader
        ]


def summarize(rows: list[ReplayRow]) -> list[PolicySummary]:
    by_policy: dict[str, list[ReplayRow]] = defaultdict(list)
    for row in rows:
        by_policy[row.policy].append(row)

    summaries: list[PolicySummary] = []
    for policy, policy_rows in sorted(by_policy.items()):
        runs = len(policy_rows)
        summaries.append(
            PolicySummary(
                policy=policy,
                runs=runs,
                win_rate=count_rate(policy_rows, lambda row: row.win_loss == "win"),
                avg_floor_reached=mean(row.floor_reached for row in policy_rows),
                avg_turns_to_kill=mean(row.turns_to_kill for row in policy_rows),
                avg_remaining_hp=mean(row.remaining_hp for row in policy_rows),
                avg_gold=mean(row.gold for row in policy_rows),
                boss_win_rate=count_rate(policy_rows, lambda row: row.boss_result == "win"),
                synergy_completion_rate=count_rate(policy_rows, lambda row: bool(row.synergy_completion)),
            )
        )

    return summaries


def count_rate(rows: list[ReplayRow], predicate) -> float:
    if not rows:
        return 0.0
    return sum(1 for row in rows if predicate(row)) / len(rows)


def write_summary_csv(path: Path, summaries: list[PolicySummary]) -> None:
    with path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.writer(handle)
        writer.writerow(
            [
                "policy",
                "runs",
                "win_rate",
                "avg_floor_reached",
                "avg_turns_to_kill",
                "avg_remaining_hp",
                "avg_gold",
                "boss_win_rate",
                "synergy_completion_rate",
            ]
        )
        for summary in summaries:
            writer.writerow(
                [
                    summary.policy,
                    summary.runs,
                    f"{summary.win_rate:.4f}",
                    f"{summary.avg_floor_reached:.2f}",
                    f"{summary.avg_turns_to_kill:.2f}",
                    f"{summary.avg_remaining_hp:.2f}",
                    f"{summary.avg_gold:.2f}",
                    f"{summary.boss_win_rate:.4f}",
                    f"{summary.synergy_completion_rate:.4f}",
                ]
            )


def write_summary_markdown(path: Path, summaries: list[PolicySummary]) -> None:
    lines = [
        "# Seed Replay Policy Summary",
        "",
        "| policy | runs | win rate | avg floor | avg turns | avg HP | avg Gold | boss win | synergy |",
        "|---|---:|---:|---:|---:|---:|---:|---:|---:|",
    ]
    for summary in summaries:
        lines.append(
            "| "
            + " | ".join(
                [
                    summary.policy,
                    str(summary.runs),
                    f"{summary.win_rate:.2%}",
                    f"{summary.avg_floor_reached:.2f}",
                    f"{summary.avg_turns_to_kill:.2f}",
                    f"{summary.avg_remaining_hp:.2f}",
                    f"{summary.avg_gold:.2f}",
                    f"{summary.boss_win_rate:.2%}",
                    f"{summary.synergy_completion_rate:.2%}",
                ]
            )
            + " |"
        )
    lines.extend(
        [
            "",
            "Interpretation rule: this summary is QA evidence, not a GDD balance lock.",
        ]
    )
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def write_floor_histogram(path: Path, rows: list[ReplayRow]) -> None:
    counts = Counter((row.policy, row.floor_reached) for row in rows)
    policies = sorted({row.policy for row in rows})
    lines: list[str] = []
    for policy in policies:
        lines.append(policy)
        for floor in range(1, 6):
            count = counts[(policy, floor)]
            lines.append(f"  floor {floor}: {'#' * count} {count}")
        lines.append("")
    path.write_text("\n".join(lines), encoding="utf-8")


def try_write_png(path: Path, rows: list[ReplayRow]) -> bool:
    try:
        import matplotlib.pyplot as plt  # type: ignore
    except ImportError:
        return False

    policies = sorted({row.policy for row in rows})
    data = [[row.floor_reached for row in rows if row.policy == policy] for policy in policies]
    plt.figure(figsize=(8, 4))
    plt.boxplot(data, labels=policies)
    plt.title("Floor Reached by Policy")
    plt.ylabel("floor reached")
    plt.tight_layout()
    plt.savefig(path)
    plt.close()
    return True


def main() -> None:
    args = parse_args()
    input_path = Path(args.input)
    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)

    rows = read_rows(input_path)
    summaries = summarize(rows)
    write_summary_csv(output_dir / "policy_summary.csv", summaries)
    write_summary_markdown(output_dir / "policy_summary.md", summaries)
    write_floor_histogram(output_dir / "floor_reached_histogram.txt", rows)
    wrote_png = try_write_png(output_dir / "floor_reached_histogram.png", rows)

    print(f"read {len(rows)} rows from {input_path}")
    print(f"wrote summary -> {output_dir}")
    if not wrote_png:
        print("matplotlib not found; wrote ASCII histogram only")


if __name__ == "__main__":
    main()
