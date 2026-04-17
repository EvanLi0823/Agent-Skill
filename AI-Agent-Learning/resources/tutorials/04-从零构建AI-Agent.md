# 从零构建 AI Agent 教程

> 来源：AI Agents from Scratch (GitHub)
> 作者：pguso
> 翻译日期：2026-04-13

---

## 项目简介

**GitHub**: https://github.com/pguso/ai-agents-from-scratch

这个仓库教开发者使用本地 LLM 和 node-llama-cpp 从第一原理构建 AI Agent。核心理念强调实践学习：在采用生产框架之前深入理解基础原理。

**核心标语**：
> "使用本地模型从零学习构建 AI Agent，无需框架。
> 在使用生产框架前理解底层机制。"

---

## 学习目标

本课程分为两个阶段：

### 📚 阶段一：基础知识（10 个课程）

#### 课程 1：基本 LLM 交互
- 加载本地模型
- 发送提示词
- 接收响应

**学习要点**：
```javascript
// 基本对话
const model = await loadModel('model.gguf');
const response = await model.chat('你好，世界！');
console.log(response);
```

---

#### 课程 2：系统提示词（System Prompts）
- 行为专业化
- 角色定义
- 上下文设置

**示例**：
```javascript
const systemPrompt = `
你是一个专业的Python编程助手。
你的回答应该：
1. 简洁明了
2. 包含代码示例
3. 解释关键概念
`;
```

---

#### 课程 3：推理能力
- 复杂问题求解
- 逻辑推理
- 数学计算

**技术**：
- Chain of Thought（思维链）
- Step-by-step reasoning
- 问题分解

---

#### 课程 4：并行处理
- 性能优化
- 批量请求
- 异步处理

**代码模式**：
```javascript
// 并行处理多个请求
const promises = questions.map(q => model.chat(q));
const results = await Promise.all(promises);
```

---

#### 课程 5：流式响应（Streaming）
- Token 管理
- 实时输出
- 用户体验优化

**实现**：
```javascript
for await (const chunk of model.stream(prompt)) {
  process.stdout.write(chunk);
}
```

---

#### 课程 6：函数调用（Function Calling / Tool Use）
- **最重要的课程！**
- 启用 Agent 能力
- 外部工具集成
- 结构化输出

**核心概念**：
```javascript
const tools = {
  calculator: (expression) => eval(expression),
  search: (query) => searchAPI(query),
  weather: (city) => getWeather(city)
};

// LLM 决定使用哪个工具
const decision = await model.decideToolUse(userQuery, tools);
const result = tools[decision.toolName](decision.args);
```

---

#### 课程 7：持久化记忆
- 跨会话状态保持
- 对话历史管理
- 上下文窗口优化

**存储策略**：
```javascript
// 简单记忆
const memory = {
  shortTerm: [],  // 最近对话
  longTerm: {}    // 重要信息
};

// 保存到文件/数据库
fs.writeFileSync('memory.json', JSON.stringify(memory));
```

---

#### 课程 8：ReAct 模式
- **迭代推理**
- 思考-行动-观察循环
- 自适应问题解决

**ReAct 循环**：
```
1. THOUGHT: 我需要找出...
2. ACTION: 调用搜索工具
3. OBSERVATION: 搜索返回...
4. THOUGHT: 基于结果，接下来...
5. ACTION: 调用计算工具
6. OBSERVATION: 计算结果是...
7. FINAL_ANSWER: 根据以上信息...
```

---

#### 课程 9：Atom of Thought（AoT）规划
- 高级计划方法论
- 任务分解策略
- 目标驱动推理

**AoT 结构**：
```
目标：[主要目标]
├── 子目标1
│   ├── 步骤1.1
│   └── 步骤1.2
├── 子目标2
│   └── 步骤2.1
└── 子目标3
```

---

#### 课程 10：完整 Agent 实现
- 整合所有概念
- 构建生产级 Agent
- 最佳实践应用

---

### 🏗️ 阶段二：框架设计（类似 LangChain/LangGraph）

构建生产级抽象：

#### 模块 1：基础（Foundation）
- Runnable 接口
- 消息系统
- 核心抽象

**Runnable 模式**：
```javascript
class Runnable {
  async invoke(input) {
    throw new Error('Must implement invoke');
  }

  pipe(other) {
    return new RunnableSequence([this, other]);
  }
}
```

---

#### 模块 2：组合（Composition）
- 链（Chains）构建
- 组件连接
- 数据流管理

**链式调用**：
```javascript
const chain = prompt
  .pipe(model)
  .pipe(outputParser);

const result = await chain.invoke(input);
```

---

#### 模块 3：代理（Agency）
- 工具编排
- Agent 架构
- 决策逻辑

**Agent 结构**：
```javascript
class Agent extends Runnable {
  constructor(tools, model) {
    this.tools = tools;
    this.model = model;
  }

  async invoke(input) {
    // ReAct 循环实现
  }
}
```

---

#### 模块 4：图（Graphs）
- 状态机
- 复杂工作流
- 条件分支

**状态图**：
```javascript
const graph = new StateGraph()
  .addNode('start', startNode)
  .addNode('process', processNode)
  .addNode('end', endNode)
  .addEdge('start', 'process')
  .addConditionalEdge('process', shouldContinue, {
    continue: 'process',
    finish: 'end'
  });
```

---

## 项目结构

```
ai-agents-from-scratch/
├── examples/                    # 示例代码
│   ├── 01_intro/               # 课程1
│   │   ├── index.js            # 代码
│   │   ├── CODE.md             # 逐行解释
│   │   └── CONCEPT.md          # 概念深入
│   ├── 02_system-prompts/      # 课程2
│   ├── 03_reasoning/           # 课程3
│   ├── 04_parallel/            # 课程4
│   ├── 05_streaming/           # 课程5
│   ├── 06_function-calling/    # 课程6 ⭐
│   ├── 07_memory/              # 课程7
│   ├── 08_react-agent/         # 课程8 ⭐⭐
│   ├── 09_planning/            # 课程9
│   └── 10_aot-agent/           # 课程10
│
├── tutorial/                    # 阶段二框架设计
│   ├── 01-foundation/          # 核心抽象
│   ├── 02-composition/         # 构建链
│   ├── 03-agency/              # 工具与 Agent
│   └── 04-graphs/              # 状态机
│
├── helper/
│   └── prompt-debugger.js      # 可视化工具
│
├── models/                      # GGUF 模型存储
└── logs/                        # 调试输出
```

---

## 核心特性

### 1. 渐进式学习
- 每个示例基于前面的概念
- 循序渐进
- 由简到繁

---

### 2. 多格式文档
每个课程包含三部分：

**📄 CODE.md**
```markdown
第1-3行: 导入依赖
第5-10行: 初始化模型
第12-15行: 发送请求
...
```

**📘 CONCEPT.md**
```markdown
## 为什么需要System Prompts？
## 工作原理
## 最佳实践
## 常见陷阱
```

**💻 index.js**
```javascript
// 可运行的完整代码
```

---

### 3. 本地优先（Local-First）
- 无需 API 密钥
- 完全隐私
- 离线运行
- 成本为零

**优势**：
- ✅ 数据隐私
- ✅ 无成本限制
- ✅ 完全控制
- ✅ 快速迭代

---

### 4. 框架无关模式
- 概念可迁移到任何框架
- 理解核心原理
- 不被特定工具束缚

**适用于**：
- LangChain
- LangGraph
- LlamaIndex
- 自定义解决方案

---

### 5. 生产模式
- 错误处理
- 流式处理
- 性能优化
- 监控和日志

**完整示例**：
```javascript
async function robustAgentCall(input) {
  try {
    const startTime = Date.now();

    // 流式处理
    for await (const chunk of agent.stream(input)) {
      console.log(chunk);
    }

    // 性能监控
    const duration = Date.now() - startTime;
    logger.info(`Completed in ${duration}ms`);

  } catch (error) {
    // 错误处理
    logger.error('Agent failed:', error);
    return fallbackResponse;
  }
}
```

---

### 6. 提示词调试器（Prompt Debugger）
**独特工具**：可视化模型接收的确切内容

```bash
node helper/prompt-debugger.js examples/06_function-calling
```

**显示**：
- 完整提示词
- 系统消息
- 用户输入
- 模型响应
- Token 统计

---

## 学习路径

### 第 1-2 周：基础阶段
1. **完成课程 1-3**
   - 基本交互
   - 系统提示词
   - 推理能力

2. **动手实践**
   - 运行每个示例
   - 修改代码实验
   - 理解输出

3. **阅读文档**
   - CODE.md 理解实现
   - CONCEPT.md 深入理论

---

### 第 3-4 周：工具使用
1. **课程 4-6**
   - 并行处理
   - 流式响应
   - **函数调用（重点）**

2. **项目实践**
   - 创建自己的工具
   - 集成外部 API
   - 构建简单 Agent

---

### 第 5-6 周：Agent 模式
1. **课程 7-10**
   - 持久化记忆
   - **ReAct 模式（重点）**
   - 规划方法
   - 完整 Agent

2. **综合项目**
   - 整合所有概念
   - 构建实用 Agent
   - 优化性能

---

### 第 7+ 周：框架设计
1. **阶段二学习**
   - 抽象设计
   - 框架模式
   - 生产实践

2. **深入研究**
   - 阅读 LangChain 源码
   - 比较设计模式
   - 构建自己的框架

---

## 实践建议

### 设置环境

```bash
# 克隆仓库
git clone https://github.com/pguso/ai-agents-from-scratch
cd ai-agents-from-scratch

# 安装依赖
npm install

# 下载模型（GGUF 格式）
# 将模型放到 models/ 目录
```

---

### 推荐模型

**初学者**：
- Llama 3.2 (3B) - 快速，适合学习
- Phi-3 Mini - 轻量级

**进阶**：
- Llama 3.1 (8B) - 更强能力
- Mistral 7B - 优秀性能

**高级**：
- Llama 3.1 (70B) - 最佳质量
- Mixtral 8x7B - 专家混合

---

### 运行示例

```bash
# 运行课程1
cd examples/01_intro
node index.js

# 使用调试器
node ../../helper/prompt-debugger.js .

# 运行课程6（函数调用）
cd ../06_function-calling
node index.js
```

---

### 修改和实验

```javascript
// 原始代码
const systemPrompt = "你是一个有帮助的助手";

// 实验：改变角色
const systemPrompt = "你是一个Python专家";

// 实验：添加约束
const systemPrompt = `
你是一个Python专家。
- 总是提供代码示例
- 解释复杂概念
- 使用简单语言
`;
```

---

## 关键概念深入

### 函数调用（课程 6）

**为什么重要**：
- 这是 Agent "智能"的核心
- 使 LLM 能采取行动
- 连接 AI 和真实世界

**实现步骤**：
1. 定义工具模式
2. 让 LLM 理解工具
3. 解析 LLM 决策
4. 执行工具
5. 返回结果给 LLM

**示例实现**：
```javascript
// 1. 定义工具
const tools = {
  get_weather: {
    description: "获取城市天气",
    parameters: {
      city: "string"
    },
    execute: async (city) => {
      return await weatherAPI.get(city);
    }
  }
};

// 2. 工具描述给 LLM
const toolDescriptions = Object.entries(tools)
  .map(([name, tool]) =>
    `${name}: ${tool.description}`
  ).join('\n');

const prompt = `
可用工具：
${toolDescriptions}

用户问题：北京天气怎么样？

请决定使用哪个工具。回复格式：
TOOL: 工具名
ARGS: {"参数": "值"}
`;

// 3. 解析 LLM 响应
const response = await model.chat(prompt);
const { tool, args } = parseToolCall(response);

// 4. 执行工具
const result = await tools[tool].execute(args);

// 5. 返回结果
const finalPrompt = `
工具返回: ${result}
请基于此回答用户。
`;
```

---

### ReAct 模式（课程 8）

**核心循环**：
```javascript
async function reactAgent(question) {
  const maxIterations = 5;
  let iteration = 0;

  while (iteration < maxIterations) {
    // THOUGHT
    const thought = await model.chat(`
      问题: ${question}
      当前信息: ${context}

      你的思考：
    `);

    console.log('THOUGHT:', thought);

    // 决定是否需要行动
    if (shouldTakeAction(thought)) {
      // ACTION
      const action = await model.chat(`
        基于思考决定使用哪个工具：
        ${thought}
      `);

      console.log('ACTION:', action);

      // OBSERVATION
      const observation = await executeAction(action);
      console.log('OBSERVATION:', observation);

      context += `\n${observation}`;
      iteration++;
    } else {
      // FINAL ANSWER
      return thought;
    }
  }
}
```

---

## 常见问题解答

### Q: 为什么使用本地模型？
**A**:
- 学习底层机制
- 完全控制
- 无成本限制
- 隐私保护

### Q: 本地模型够用吗？
**A**:
- 学习：完全够用
- 生产：可能需要更强模型（API）
- 关键是理解原理

### Q: 需要多少 GPU？
**A**:
- 3B 模型：CPU 也可以
- 7B-8B：8GB VRAM
- 70B：24GB+ VRAM

### Q: 学完后做什么？
**A**:
1. 探索 LangChain/LangGraph
2. 构建自己的项目
3. 理解生产框架设计
4. 贡献开源项目

---

## 项目示例灵感

### 初级项目
- 天气查询 Agent
- 计算器 Agent
- 文档问答 Agent

### 中级项目
- 个人助理
- 代码审查 Agent
- 数据分析 Agent

### 高级项目
- 多 Agent 协作系统
- 自主研究 Agent
- 客户服务系统

---

## 资源链接

- **GitHub 仓库**: https://github.com/pguso/ai-agents-from-scratch
- **node-llama-cpp**: https://github.com/withcatai/node-llama-cpp
- **GGUF 模型**: https://huggingface.co/models?library=gguf

---

## 总结

这个项目提供：

🎓 **完整学习路径**
- 10 个渐进课程
- 框架设计教程
- 实践项目指导

🛠️ **实用技能**
- 理解 Agent 原理
- 掌握核心模式
- 构建生产系统

📚 **丰富文档**
- 代码注释
- 概念解释
- 最佳实践

💡 **深入理解**
- 无黑盒
- 完全透明
- 可调试可控

**立即开始**：克隆仓库，运行第一个示例，开启你的 AI Agent 开发之旅！

**记住**：理解原理比使用框架更重要。掌握基础，框架只是工具。
