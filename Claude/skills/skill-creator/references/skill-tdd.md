---
# Skill TDD 方法论

**核心原则**：创建 Skill = 为流程文档应用 TDD

---

**NEVER 在未运行"基线测试"的情况下发布新 Skill**
原因：未测试的 Skill 和未测试的代码一样——看起来对，用起来错。必须先观察 Agent 不使用该 Skill 时会犯什么错，才能确认 Skill 解决了正确问题。

**NEVER 在 description 中描述 Skill 的工作流程**
原因：当 description 包含工作流摘要时，Claude 会直接按 description 行事而不读 SKILL.md 全文。description 只应描述"何时触发"，不描述"如何执行"。

---

## Iron Law

```
NO SKILL WITHOUT A FAILING TEST FIRST
```

无例外：不适用于"简单补充"、"仅加章节"、"文档更新"。违反即删除重来。

## TDD 映射

| TDD 概念 | Skill 创作 |
|---------|-----------|
| 测试用例 | 给子 Agent 设置压力场景 |
| 生产代码 | Skill 文档（SKILL.md） |
| 红灯（RED） | 无 Skill 时 Agent 违规（基线） |
| 绿灯（GREEN） | 有 Skill 时 Agent 合规 |
| 重构 | 堵住漏洞，维持合规 |

## RED-GREEN-REFACTOR 流程

1. **RED**：无 Skill 运行压力场景 → 记录 Agent 的借口（逐字）
2. **GREEN**：针对借口编写最小 Skill → 验证 Agent 合规
3. **REFACTOR**：发现新借口 → 添加反制 → 重新测试至无新漏洞

## CSO：description 字段规则

```yaml
# 错误：描述了工作流 → Claude 会按 description 执行而不读 SKILL.md
description: Use when executing plans - dispatches subagent per task with code review between tasks

# 正确：只描述触发条件
description: Use when executing implementation plans with independent tasks in the current session
```

- 以 "Use when..." 开头
- 只写触发条件和症状，不写步骤和流程
- 用第三人称，不超过 500 字符

## 跳过测试的常见借口

| 借口 | 现实 |
|------|------|
| "这个 Skill 显然很清楚" | 对你清楚 ≠ 对其他 Agent 清楚，测试它 |
| "只是参考文档" | 参考文档有缺口，测试检索场景 |
| "测试太麻烦" | 15 分钟测试省去数小时调试 |
| "等出问题再测" | 问题 = Agent 无法使用，先测后发布 |
| "上次这样做可以" | 上次 ≠ 这次，每个 Skill 独立测试 |

## Skill 类型与测试重点

| Skill 类型 | 测试方法 | 成功标准 |
|-----------|---------|---------|
| 纪律执行型（TDD/verification） | 施压场景：时间+沉没成本+疲惫 | 最大压力下仍合规 |
| 技术指导型（how-to） | 应用场景 + 边界用例 | 正确应用到新场景 |
| 模式型（心智模型） | 识别 + 反例 | 知道何时用/不用 |
| 参考文档型（API docs） | 检索场景 + 应用验证 | 找到并正确使用 |

## 部署检查清单（每个 Skill 必须完成）

**RED 阶段**
- [ ] 创建 3+ 组合压力场景（针对纪律执行型 Skill）
- [ ] 无 Skill 运行场景，逐字记录 Agent 的违规行为

**GREEN 阶段**
- [ ] name 只含字母、数字、连字符
- [ ] description 以 "Use when..." 开头，不含工作流
- [ ] 正文针对 RED 阶段发现的具体违规编写
- [ ] 有 Skill 运行场景，验证 Agent 合规

**REFACTOR 阶段**
- [ ] 识别新借口 → 添加显式反制
- [ ] 建立借口反驳表
- [ ] 重测直到无新漏洞
