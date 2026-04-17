---
name: code-review
description: Use when completing tasks, implementing major features, or before merging (to dispatch a code reviewer subagent); OR when receiving code review feedback, before implementing suggestions, especially if feedback seems unclear or technically questionable.
---

# Code Review

## 本技能禁令 (NEVER / DO NOT)

**NEVER 因为"改动很简单"就跳过 Review**
原因：简单改动导致的 Bug 和复杂改动一样难排查，跳过 Review 节省的 5 分钟会在后续调试中加倍消耗

**NEVER 收到反馈后立即盲目实施**
原因：反馈可能基于错误的前提或不完整的上下文，先验证再实施是基本技术纪律

**DO NOT 对反馈做表演性认同（"你说得对！"、"很好的建议！"）**
原因：表演性回应不提供任何信息，反而会让真实技术判断变得模糊

---

## Phase 1：发起 Code Review（Requesting）

**核心原则**：Review early, review often。给 Reviewer 精准上下文，不传递你的会话历史。

### 何时发起

**必须发起**：
- 完成每个任务后（在 subagent-driven-development 流程中）
- 完成重要功能后
- 合并到 main 分支前

**可选（但有价值）**：
- 卡住时（换个视角）
- 复杂 Bug 修复后

### 如何发起

```bash
# 1. 获取 git SHA
BASE_SHA=$(git rev-parse HEAD~1)  # 或 origin/main
HEAD_SHA=$(git rev-parse HEAD)
```

用 Task 工具派发 code-reviewer 子 Agent，填写 `code-reviewer.md` 模板：
- `{WHAT_WAS_IMPLEMENTED}` — 刚实现了什么
- `{PLAN_OR_REQUIREMENTS}` — 应该实现什么
- `{BASE_SHA}` / `{HEAD_SHA}` — 变更范围

### 如何处理反馈

- **Critical**：立即修复
- **Important**：推进前修复
- **Minor**：记录，稍后处理
- **有异议**：给出技术理由反驳（见 Phase 2）

模板文件：[code-reviewer.md](code-reviewer.md)

---

## Phase 2：接收 Code Review（Receiving）

**核心原则**：技术评估，不是情绪表演。验证后再实施，不明确先询问。

### 响应流程

```
1. READ   — 完整读完反馈，不中途反应
2. UNDERSTAND — 用自己的话复述需求（或提问）
3. VERIFY — 对照代码库核实
4. EVALUATE — 技术上对当前代码库是否正确？
5. RESPOND — 技术性确认或有理由的反驳
6. IMPLEMENT — 一次一项，每项测试
```

### 禁止的回应

```
❌ "你说得对！"
❌ "很好的建议！"
❌ "谢谢你的反馈！"
❌ 不验证就直接实施

✅ 复述技术需求
✅ 提出澄清问题
✅ 有技术理由时反驳
✅ 直接动手（行动胜于表达）
```

### 不清楚的反馈：先全部澄清

```
IF 任何反馈项不清楚:
  STOP — 不要先实施其他项
  询问所有不清楚的项

原因：各项可能有关联，部分理解 = 错误实施
```

### 来源区分

| 来源 | 处理方式 |
|------|---------|
| 用户 | 信任，但不明确时仍要询问范围 |
| 外部审查者 | 先验证：技术正确吗？会破坏现有功能吗？理解全部上下文了吗？ |

### 何时反驳

- 建议会破坏现有功能
- Reviewer 缺乏完整上下文
- 违反 YAGNI（grep 确认功能未被使用）
- 技术上对当前技术栈不正确
- 与架构决策冲突

**反驳方式**：技术理由，不是防御性辩解；引用代码/测试；必要时升级给用户决策。

### 正确确认反馈

```
✅ "Fixed. [简短描述变更]"
✅ "Good catch - [具体问题]. Fixed in [位置]."
✅ 直接修复并展示代码

❌ 任何感谢表达
❌ 过长解释
```

---

## 常见错误速查

| 错误 | 修正 |
|------|------|
| 表演性认同 | 直接复述需求或动手 |
| 盲目实施 | 先对照代码库验证 |
| 批量实施不测试 | 一次一项，每项测试 |
| 模糊的项直接跳过 | 全部澄清后再动手 |
| 回避反驳 | 技术正确性 > 社交舒适度 |
