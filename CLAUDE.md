# CLAUDE.md

Guidance for Claude Code when working in this repository.

## Coding Discipline

- Read existing code before suggesting modifications.
- If multiple interpretations of a task exist, present them — do not pick silently.
- Frame tasks as verifiable goals: "fix the bug" → "write a test that reproduces it, then make it pass."
- When a code quality tool flags your code, fix the code first. Only suppress a warning if the fix would be significantly worse.
- Don't add features, refactor code, or make "improvements" beyond what was asked.
- Keep lines to 100 characters or fewer, unless wrapping would break the compiler, reduce clarity, or produce visually unappealing formatting.

## Prefer Forward-Facing Comments Over Backward-Facing Ones

A code comment earns its place only if it helps someone who reads or changes the code later and never saw this PR. Before keeping any comment you add or change, apply one test:

**Strip the PR/development context — does the comment still help?** If it only explains how the code reached its current state during this PR, cut it. If it points forward — to open work, a constraint, or rationale that can't be inlined — keep it.

### Backward-facing — cut these

These record development history. They rot the moment the PR merges and mean nothing to a future reader:

- Ticket/PR references for work done *in this PR* (e.g. `HT-156`, `PR #307`, review-finding IDs).
- Change narration with no ID at all — "previously", "used to", "the review found", "now returns early instead of throwing". This is the most common AI-authored form, and dropping the ticket number doesn't make it forward-facing: describe what the code does, not what it stopped doing.
- Stage/round tags: "review round 2", "final pass".
- Dated development notes.

The right home for "this change was made because of HT-156" is the commit message or PR description, not the source file.

### Forward-facing — keep these

These tell a future maintainer something they still need:

- An **open** ticket for follow-up work deliberately *not* done in this PR — best written as a TODO naming the ticket, so the ID signals exactly when the code can be revisited: `// TODO(HT-XXX): ...`
- A prose pointer to where deferred work or discussion lives (e.g. a PR comment thread), when it isn't something that can be inlined.
- A constraint, invariant, or gotcha the code depends on.
- Why the code takes a surprising or non-obvious approach.

### Why this needs a deliberate pass

AI-authored diffs over-produce backward-facing comments *systematically*: the failing test, the review finding, and the ticket in play are exactly what is in the model's context while the comment is written, so it narrates provenance instead of what a maintainer needs.

**Before treating a change as complete, re-read every comment you added or changed in the diff, apply the strip-the-PR-context test to each, and delete the backward-facing ones** — moving anything still worth saying into the commit message or PR description.

(When *reviewing* someone else's diff and the pattern is pervasive, raise it once at the PR level rather than flagging every site.)
