# LangGraph 完整指南

> 来源：LangGraph 官方文档
> 翻译日期：2026-04-13

---

## 什么是 LangGraph？

**官方定义**：
> LangGraph 是一个"用于构建、管理和部署长时间运行、有状态 Agent 的低级编排框架"。

由 LangChain Inc 构建，它使开发者能够构建在扩展操作中维护状态的弹性 AI 系统。

**GitHub**: https://github.com/langchain-ai/langgraph
**官网**: https://www.langchain.com/langgraph

---

## 核心特性

### 1. 持久化执行（Durable Execution）
- Agent 在失败后持续运行
- 从最后检查点自动恢复
- 不会丢失进度

**使用场景**：
```python
# Agent 运行 2 小时后崩溃
# LangGraph 自动从第 1.5 小时处恢复
# 无需重新开始
```

---

### 2. 人机协作（Human-in-the-Loop）
- 在任何执行点检查状态
- 修改 Agent 状态
- 无缝人工监督

**工作流**：
```
Agent 执行 → 暂停等待批准 → 人工审核 → 继续或修改
```

**代码示例**：
```python
# Agent 请求批准
graph.add_node("request_approval", request_approval_node)
graph.add_edge("request_approval", "wait_for_human")

# 人工批准后继续
graph.add_edge("wait_for_human", "continue_execution")
```

---

### 3. 全面记忆（Comprehensive Memory）
- **工作记忆**：当前推理的即时状态
- **长期记忆**：跨会话的持久化信息

**记忆类型**：
| 类型 | 用途 | 持久性 |
|------|------|--------|
| 短期 | 当前对话上下文 | 会话期间 |
| 工作 | Agent 当前任务状态 | 任务期间 |
| 长期 | 历史信息、知识库 | 永久 |

---

### 4. LangSmith 集成
- 深入了解 Agent 行为
- 可视化工具追踪执行路径
- 追踪状态变化

**可视化示例**：
```
开始 → 节点A → 节点B → [条件] → 节点C → 结束
                          ↓
                       节点D
```

---

### 5. 生产部署
- 专为有状态、长时间运行工作流设计
- 大规模处理独特需求
- 企业级基础设施

---

## 快速开始

### 安装

```bash
pip install -U langgraph
```

### 最简示例

```python
from langgraph.graph import StateGraph

# 定义状态
class AgentState(TypedDict):
    messages: list[str]
    current_step: int

# 创建图
graph = StateGraph(AgentState)

# 添加节点
def process_node(state: AgentState):
    state["current_step"] += 1
    return state

graph.add_node("process", process_node)

# 设置入口和出口
graph.set_entry_point("process")
graph.set_finish_point("process")

# 编译
app = graph.compile()

# 运行
result = app.invoke({"messages": [], "current_step": 0})
```

---

## 主要用例

LangGraph 适合任何涉及扩展、有状态操作的场景：

### 1. 多步 AI 工作流
- 需要持久化状态的复杂流程
- 跨多个步骤的数据传递
- 状态依赖的决策

**示例**：
```
用户查询 → 理解意图 → 检索信息 → 分析 → 生成响应
   ↓           ↓            ↓         ↓          ↓
 状态1       状态2        状态3     状态4      状态5
```

---

### 2. 需要人工干预的 Agent 系统
- 关键决策需要人工批准
- 监督 AI 操作
- 错误纠正和引导

**场景**：
- 医疗诊断建议（需医生确认）
- 金融交易（需人工批准）
- 法律文档生成（需律师审核）

---

### 3. 需要详细执行可见性的应用
- 调试复杂 Agent 行为
- 监控决策过程
- 性能分析和优化

---

### 4. 复杂推理系统的生产部署
- 企业级可靠性
- 错误恢复机制
- 监控和日志

---

## 核心概念

### 状态图（State Graph）

**定义**：
有向图，其中节点代表计算步骤，边代表状态转换。

**组成部分**：
1. **状态（State）**：包含所有必要信息的数据结构
2. **节点（Nodes）**：执行操作并更新状态的函数
3. **边（Edges）**：定义执行流程
4. **条件边（Conditional Edges）**：基于状态的动态路由

**示例图**：
```python
from langgraph.graph import StateGraph, END

graph = StateGraph(AgentState)

# 添加节点
graph.add_node("start", start_node)
graph.add_node("process", process_node)
graph.add_node("review", review_node)
graph.add_node("end", end_node)

# 添加边
graph.add_edge("start", "process")

# 添加条件边
def should_review(state):
    return state["needs_review"]

graph.add_conditional_edges(
    "process",
    should_review,
    {
        True: "review",
        False: "end"
    }
)

graph.add_edge("review", "end")

# 设置入口
graph.set_entry_point("start")
```

---

### 检查点（Checkpoints）

**作用**：
- 保存状态快照
- 支持恢复和回滚
- 实现持久化执行

**实现**：
```python
from langgraph.checkpoint import MemorySaver

# 添加检查点
checkpointer = MemorySaver()
app = graph.compile(checkpointer=checkpointer)

# 运行时自动保存检查点
result = app.invoke(initial_state)

# 从检查点恢复
resumed_result = app.invoke(
    state,
    config={"configurable": {"thread_id": "123"}}
)
```

---

### 子图（Subgraphs）

**用途**：
- 模块化复杂工作流
- 重用工作流组件
- 分层设计

**示例**：
```python
# 创建子图
data_processing = StateGraph(DataState)
data_processing.add_node("clean", clean_node)
data_processing.add_node("transform", transform_node)
data_processing.add_edge("clean", "transform")

# 在主图中使用
main_graph.add_node("process_data", data_processing.compile())
```

---

## 实战示例

### 示例 1：简单 ReAct Agent

```python
from langgraph.graph import StateGraph, END
from langchain.agents import tool
from typing import TypedDict, Annotated

# 定义工具
@tool
def search(query: str) -> str:
    """搜索互联网"""
    return f"搜索结果: {query}"

@tool
def calculator(expression: str) -> str:
    """计算数学表达式"""
    return str(eval(expression))

tools = [search, calculator]

# 定义状态
class AgentState(TypedDict):
    messages: Annotated[list, "对话历史"]
    next_action: Annotated[str, "下一步行动"]

# 定义节点
def call_model(state: AgentState):
    """让 LLM 决定下一步"""
    response = llm_with_tools.invoke(state["messages"])
    return {"messages": [response]}

def call_tool(state: AgentState):
    """执行工具"""
    last_message = state["messages"][-1]
    tool_call = last_message.tool_calls[0]

    # 执行工具
    tool = {t.name: t for t in tools}[tool_call["name"]]
    result = tool.invoke(tool_call["args"])

    return {"messages": [ToolMessage(content=result)]}

def should_continue(state: AgentState):
    """决定是否继续"""
    last_message = state["messages"][-1]
    if hasattr(last_message, 'tool_calls'):
        return "continue"
    return "end"

# 构建图
workflow = StateGraph(AgentState)

# 添加节点
workflow.add_node("agent", call_model)
workflow.add_node("action", call_tool)

# 设置入口点
workflow.set_entry_point("agent")

# 添加条件边
workflow.add_conditional_edges(
    "agent",
    should_continue,
    {
        "continue": "action",
        "end": END
    }
)

# 从 action 回到 agent
workflow.add_edge("action", "agent")

# 编译
app = workflow.compile()

# 运行
result = app.invoke({
    "messages": [HumanMessage(content="北京的天气怎么样？")]
})
```

---

### 示例 2：多 Agent 协作

```python
class MultiAgentState(TypedDict):
    task: str
    research_results: str
    draft: str
    final_output: str

# 研究 Agent
def research_agent(state: MultiAgentState):
    results = search_tool.invoke(state["task"])
    return {"research_results": results}

# 写作 Agent
def writer_agent(state: MultiAgentState):
    draft = llm.invoke(f"基于: {state['research_results']}")
    return {"draft": draft}

# 审核 Agent
def reviewer_agent(state: MultiAgentState):
    review = llm.invoke(f"审核: {state['draft']}")
    return {"final_output": review}

# 构建工作流
workflow = StateGraph(MultiAgentState)

workflow.add_node("research", research_agent)
workflow.add_node("write", writer_agent)
workflow.add_node("review", reviewer_agent)

workflow.add_edge("research", "write")
workflow.add_edge("write", "review")

workflow.set_entry_point("research")
workflow.set_finish_point("review")

app = workflow.compile()
```

---

### 示例 3：人机协作审批流程

```python
from langgraph.checkpoint.sqlite import SqliteSaver

class ApprovalState(TypedDict):
    request: str
    analysis: str
    approved: bool
    feedback: str

def analyze_request(state: ApprovalState):
    analysis = llm.invoke(f"分析请求: {state['request']}")
    return {"analysis": analysis}

def wait_for_approval(state: ApprovalState):
    # 暂停，等待人工输入
    return state

def process_approval(state: ApprovalState):
    if state["approved"]:
        return {"feedback": "已批准，继续执行"}
    else:
        return {"feedback": "已拒绝，流程终止"}

# 使用 SQLite 保存状态
memory = SqliteSaver.from_conn_string(":memory:")

workflow = StateGraph(ApprovalState)

workflow.add_node("analyze", analyze_request)
workflow.add_node("wait", wait_for_approval)
workflow.add_node("process", process_approval)

workflow.add_edge("analyze", "wait")
workflow.add_edge("wait", "process")

workflow.set_entry_point("analyze")

app = workflow.compile(checkpointer=memory)

# 运行并暂停
config = {"configurable": {"thread_id": "1"}}
result = app.invoke({"request": "采购新设备"}, config)

# 人工审批后继续
app.invoke(
    {"approved": True},
    config
)
```

---

## 最佳实践

### 1. 状态设计

**原则**：
- 状态应该是可序列化的
- 包含所有必要信息
- 避免冗余数据

**好的状态**：
```python
class GoodState(TypedDict):
    user_input: str
    processed_data: dict
    current_step: int
    errors: list[str]
```

**不好的状态**：
```python
class BadState(TypedDict):
    everything: Any  # 太模糊！
```

---

### 2. 节点设计

**原则**：
- 单一职责
- 无副作用（除了更新状态）
- 可测试

**示例**：
```python
def good_node(state: State) -> dict:
    """清晰的输入输出"""
    result = process(state["data"])
    return {"processed": result}

# 避免
def bad_node(state: State):
    """副作用太多！"""
    global_var = state["data"]  # 修改全局状态
    db.save(state)  # 直接数据库操作
    print("Processing")  # 副作用
```

---

### 3. 错误处理

```python
def robust_node(state: State) -> dict:
    try:
        result = risky_operation(state["input"])
        return {"result": result, "error": None}
    except Exception as e:
        logger.error(f"节点失败: {e}")
        return {
            "result": None,
            "error": str(e),
            "retry_count": state.get("retry_count", 0) + 1
        }
```

---

### 4. 性能优化

**并行执行**：
```python
# 无依赖的节点可以并行
workflow.add_edge("start", ["node_a", "node_b"])
```

**缓存结果**：
```python
def cached_node(state: State) -> dict:
    cache_key = hash(state["input"])
    if cache_key in cache:
        return {"result": cache[cache_key]}

    result = expensive_operation(state["input"])
    cache[cache_key] = result
    return {"result": result}
```

---

## 与其他框架对比

### LangGraph vs. LangChain

| 特性 | LangGraph | LangChain |
|------|-----------|-----------|
| 状态管理 | ✅ 内置 | ⚠️ 有限 |
| 复杂工作流 | ✅ 图结构 | ⚠️ 链式 |
| 人机协作 | ✅ 原生支持 | ❌ 无 |
| 学习曲线 | 🔺 较陡 | ✅ 平缓 |
| 适用场景 | 复杂 Agent | 简单链式任务 |

**建议**：
- 简单任务 → LangChain
- 复杂 Agent → LangGraph
- 两者可以一起使用！

---

## 学习资源

### 官方资源
- **文档**: https://langchain-ai.github.io/langgraph/
- **GitHub**: https://github.com/langchain-ai/langgraph
- **教程**: https://academy.langchain.com/courses/intro-to-langgraph

### 免费课程
- **DeepLearning.AI**: AI Agents in LangGraph
- **LangChain Academy**: Introduction to LangGraph

### 社区
- Discord: LangChain 社区
- GitHub Discussions

---

## 总结

LangGraph 提供：

🎯 **强大能力**
- 持久化执行
- 人机协作
- 全面记忆
- 生产就绪

🛠️ **灵活架构**
- 图结构工作流
- 条件分支
- 子图模块化
- 检查点机制

📈 **企业级**
- LangSmith 集成
- 监控和调试
- 大规模部署
- 可靠性保证

**何时使用 LangGraph**：
- ✅ 复杂的多步骤工作流
- ✅ 需要状态管理
- ✅ 人机协作场景
- ✅ 生产环境部署

**立即开始**：
```bash
pip install -U langgraph
```

探索示例，构建你的第一个有状态 Agent！🚀
