# Final Review Prompt Template

Use this template after ALL tasks are complete, before calling `finishing-a-development-branch`.

**Purpose:** Cross-task consistency check — verify the entire implementation works as a coherent whole, not just that each task passed its individual reviews.

**Model:** Use the most capable available model (opus).

**Note:** This is NOT a repeat of per-task code quality reviews. The per-task reviewers checked each piece in isolation. This reviewer checks whether the pieces fit together correctly.

```
Task tool (general-purpose):
  description: "Final review: [feature name]"
  prompt: |
    You are doing a final overall review of a completed implementation.
    Each task has already passed individual spec compliance and code quality
    reviews. Your job is to look at the full picture.

    ## What Was Built

    [Feature name and 1-2 sentence description]

    ## Files Introduced or Modified

    [List all new/changed files from the implementation]

    ## Original Design Spec

    [Key requirements from the spec — API surface, behaviors, constraints]

    ## Review Focus

    ### 1. Cross-file consistency
    - Do method signatures and types agree across files? (A calls B with the
      right arguments?)
    - Are naming conventions consistent across the new files?
    - Are there any dangling references (A depends on B, but B was renamed or
      removed)?

    ### 2. Spec completeness
    - Does the combined implementation cover all requirements from the spec?
    - Are there requirements that each individual task "deferred to another task"
      but no task actually implemented?
    - Does the public API match what the spec defined?

    ### 3. Integration concerns
    - Does the feature interact correctly with existing systems it touches?
    - Are there any initialization order dependencies that could fail at runtime?
    - Are there resource lifecycle issues (leaks, double-frees) that only appear
      when the components are used together?

    ### 4. Known risk areas (if applicable)
    [List any areas the coordinator flagged as risky during development,
     or any DONE_WITH_CONCERNS that were noted but deferred]

    ## Output Format

    ### APPROVED
    [If everything checks out — brief summary of confidence]

    ### NEEDS_FIXES
    [If issues found — list with severity]

    #### Critical (must fix before merge)
    - file:line — description

    #### Important (should fix)
    - file:line — description

    **Do NOT re-raise issues that were already fixed during per-task reviews.**
    Focus on issues that only appear when viewing the implementation as a whole.
```

**After final review:**
- **APPROVED:** Proceed to `finishing-a-development-branch`
- **NEEDS_FIXES with Critical:** Fix directly if ≤3 lines and cause is clear; otherwise dispatch implementer
- After fixing: dispatch a quick re-check (same prompt, note the fix)
