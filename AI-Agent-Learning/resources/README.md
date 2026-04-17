# Resources 资源目录

存放下载的学习资料、论文、示例代码等

## 🎉 已添加中文教程！

已成功下载并翻译以下核心学习资源，所有文档均为中文版本。

### 📚 教程列表

所有教程位于 `tutorials/` 目录：

1. **01-ReAct模式详解.md** - ReAct 核心概念和实现
2. **02-Claude工具使用指南.md** - 工具调用完整指南
3. **03-Anthropic提示工程教程.md** - 9章完整提示词教程
4. **04-从零构建AI-Agent.md** - 10个渐进式课程
5. **05-LangGraph完整指南.md** - 生产级框架指南
6. **06-免费课程完整指南.md** - 在线课程汇总

### 🗺️ 快速开始

**查看总览**：
```bash
open 中文学习资源总览.md
```

**从这里开始学习**：
```bash
cd tutorials
open 03-Anthropic提示工程教程.md  # 先学这个
```

## 目录结构建议

```
resources/
├── papers/            # 学术论文
│   ├── react-paper.pdf
│   ├── cot-paper.pdf
│   └── agent-surveys.pdf
├── tutorials/         # 教程和指南
│   ├── langchain-guide/
│   ├── prompt-engineering/
│   └── deployment-guide/
├── code-examples/     # 代码示例
│   ├── official-examples/
│   └── community-examples/
└── datasets/          # 数据集（如有需要）
    └── sample-data/
```

## 推荐下载的资源

### 核心论文
- **ReAct论文**: "ReAct: Synergizing Reasoning and Acting in Language Models"
- **Chain-of-Thought论文**: "Chain-of-Thought Prompting Elicits Reasoning in Large Language Models"
- **Agent Survey**: 最新的AI Agent综述论文

### 官方文档（可离线查看）
- [ ] OpenAI API文档
- [ ] Anthropic Claude文档
- [ ] LangChain官方教程
- [ ] LangGraph文档

### 代码仓库（可clone）
```bash
# AutoGPT
git clone https://github.com/Significant-Gravitas/AutoGPT

# AI Agents from Scratch
git clone https://github.com/pguso/ai-agents-from-scratch

# LangGraph
git clone https://github.com/langchain-ai/langgraph
```

### 有用的Cheat Sheets
- Prompt Engineering速查表
- LangChain常用API
- Python异步编程指南

## 外部资源链接汇总

### 官方文档
- [OpenAI Documentation](https://platform.openai.com/docs)
- [Anthropic Documentation](https://docs.anthropic.com/)
- [LangChain Docs](https://python.langchain.com/)
- [LangGraph Docs](https://langchain-ai.github.io/langgraph/)

### 学习平台
- [DeepLearning.AI](https://www.deeplearning.ai/)
- [LangChain Academy](https://academy.langchain.com/)
- [Coursera](https://www.coursera.org/)
- [Udemy](https://www.udemy.com/)

### 社区
- [LangChain Discord](https://discord.gg/langchain)
- [OpenAI Developer Forum](https://community.openai.com/)
- [Reddit r/LangChain](https://www.reddit.com/r/LangChain/)

### 博客和文章
- [Anthropic Blog](https://www.anthropic.com/blog)
- [OpenAI Blog](https://openai.com/blog)
- [LangChain Blog](https://blog.langchain.dev/)

## 资源管理建议

1. **定期整理**: 每周整理新增资源
2. **分类存储**: 按类型和主题分类
3. **添加索引**: 在此README中记录重要资源位置
4. **版本控制**: 大型文件可考虑使用Git LFS
