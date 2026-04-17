# Claude 工具使用（Tool Use）指南

> 来源：Anthropic 官方文档
> 翻译日期：2026-04-13

---

## 什么是工具使用（Tool Use）？

工具使用使 Claude 能够调用您定义的函数或 Anthropic 提供的函数。Claude 会根据用户请求和工具描述自主决定何时调用工具，然后返回结构化的调用供执行。

**核心概念**：Claude 不直接执行工具，而是告诉您应该使用哪个工具以及使用什么参数。

---

## 工具的工作原理

工具分为两大类：

### 1. 客户端工具（Client Tools）

**特点**：
- 用户定义的工具（如 `bash`、`text_editor`）
- 在您的应用程序中执行
- 您需要处理工具调用和返回结果

**工作流程**：
1. Claude 响应 `stop_reason: "tool_use"` 和 tool_use 块
2. 您的代码执行操作
3. 返回 `tool_result` 给 Claude

**示例**：
```json
{
  "stop_reason": "tool_use",
  "content": [
    {
      "type": "tool_use",
      "id": "toolu_123",
      "name": "get_weather",
      "input": {
        "location": "San Francisco"
      }
    }
  ]
}
```

您需要执行 `get_weather` 并返回结果：
```json
{
  "role": "user",
  "content": [
    {
      "type": "tool_result",
      "tool_use_id": "toolu_123",
      "content": "72°F, sunny"
    }
  ]
}
```

---

### 2. 服务器工具（Server Tools）

**特点**：
- Anthropic 提供的工具（如 `web_search`、`code_execution`、`web_fetch`）
- 在 Anthropic 基础设施上运行
- 结果直接出现，无需处理执行

**可用的服务器工具**：
- `web_search` - 网络搜索
- `code_execution` - 代码执行
- `web_fetch` - 获取网页内容
- `tool_search` - 工具搜索

---

## 基本实现

### 使用服务器工具（最简单）

Anthropic 管理执行过程：

```bash
curl https://api.anthropic.com/v1/messages \
  -H "x-api-key: $ANTHROPIC_API_KEY" \
  -H "anthropic-version: 2023-06-01" \
  -H "content-type: application/json" \
  -d '{
    "model": "claude-opus-4-6",
    "max_tokens": 1024,
    "tools": [
      {
        "type": "web_search_20260209",
        "name": "web_search"
      }
    ],
    "messages": [
      {
        "role": "user",
        "content": "What'\''s the latest on Mars rover?"
      }
    ]
  }'
```

### 定义自定义工具

```python
tools = [
    {
        "name": "get_weather",
        "description": "获取指定位置的当前天气",
        "input_schema": {
            "type": "object",
            "properties": {
                "location": {
                    "type": "string",
                    "description": "城市名称，例如：北京、上海"
                },
                "unit": {
                    "type": "string",
                    "enum": ["celsius", "fahrenheit"],
                    "description": "温度单位"
                }
            },
            "required": ["location"]
        }
    }
]
```

---

## 最佳实践

### 1. 使用严格模式（Strict Mode）

设置 `strict: true` 以确保模式符合性：

```python
tools = [
    {
        "name": "calculator",
        "description": "执行数学计算",
        "input_schema": {...},
        "strict": True  # 保证严格遵守模式
    }
]
```

**优势**：
- 保证参数类型正确
- 减少解析错误
- 提高可靠性

---

### 2. 清晰的工具描述

**好的描述**：
```python
{
    "name": "search_database",
    "description": "在用户数据库中搜索。使用SQL查询语法。返回匹配的用户记录，包括姓名、邮箱和注册日期。"
}
```

**不好的描述**：
```python
{
    "name": "search",
    "description": "搜索"  # 太模糊！
}
```

---

### 3. 参数设计原则

**明确性**：
- 使用描述性的参数名
- 提供详细的参数说明
- 指定必需参数

**示例**：
```python
"properties": {
    "query": {
        "type": "string",
        "description": "搜索查询，支持关键词和短语。示例：'Python教程' 或 'AI Agent开发'"
    },
    "max_results": {
        "type": "integer",
        "description": "返回的最大结果数，范围：1-100",
        "minimum": 1,
        "maximum": 100,
        "default": 10
    }
}
```

---

### 4. 错误处理

当工具执行失败时，返回有用的错误信息：

```python
{
    "type": "tool_result",
    "tool_use_id": "toolu_123",
    "content": "错误：找不到城市 'XYZ'。请检查城市名称拼写。",
    "is_error": True
}
```

---

## 性能考虑

### Token 使用

**输入 Tokens**：
- 工具定义计入输入 tokens
- 简化工具描述以减少成本
- 只包含必要的工具

**输出 Tokens**：
- Claude 的响应（包括工具调用）
- 工具结果返回的内容

**服务器工具费用**：
- 基于使用情况的额外费用
- 查看定价页面了解详情

---

### 基准测试结果

根据官方数据，工具访问在以下基准测试中产生显著的能力提升：

- **LAB-Bench FigQA**: +X% 准确率提升
- **SWE-bench**: +Y% 任务完成率提升

（注：具体数值请查看最新基准测试报告）

---

## MCP（Model Context Protocol）集成

连接 MCP 服务器以扩展工具生态系统：

```python
# MCP 服务器提供额外的工具
# Claude 可以无缝访问这些工具

from anthropic import Anthropic

client = Anthropic()
# MCP 服务器会自动注册工具
```

**优势**：
- 访问更广泛的工具集
- 社区贡献的工具
- 标准化的工具协议

---

## 学习路径

### 新手

1. **从教程开始**
   - 动手实践演练
   - 了解基本概念
   - 构建简单示例

2. **使用服务器工具**
   - 无需复杂设置
   - 快速获得结果
   - 理解工作流程

### 进阶

1. **定义自定义工具**
   - 设计工具架构
   - 实现复杂逻辑
   - 优化性能

2. **参考概念文档**
   - 深入了解特定实现细节
   - 高级模式和技巧
   - 生产环境最佳实践

---

## 常见用例

### 1. 信息检索
```python
tools = [
    {
        "name": "search_docs",
        "description": "搜索内部文档库",
        "input_schema": {...}
    }
]
```

### 2. 数据处理
```python
tools = [
    {
        "name": "process_csv",
        "description": "处理和分析CSV文件",
        "input_schema": {...}
    }
]
```

### 3. API 调用
```python
tools = [
    {
        "name": "create_ticket",
        "description": "在项目管理系统中创建工单",
        "input_schema": {...}
    }
]
```

### 4. 计算和分析
```python
tools = [
    {
        "name": "analyze_data",
        "description": "执行统计分析",
        "input_schema": {...}
    }
]
```

---

## 调试技巧

1. **记录工具调用**
   ```python
   print(f"Claude 调用: {tool_name}")
   print(f"参数: {tool_input}")
   ```

2. **验证输入**
   ```python
   def validate_tool_input(input_data, schema):
       # 验证逻辑
       pass
   ```

3. **提供详细错误**
   ```python
   if error:
       return {
           "is_error": True,
           "content": f"详细错误信息: {error_details}"
       }
   ```

---

## 完整示例

```python
from anthropic import Anthropic

client = Anthropic(api_key="your-api-key")

# 定义工具
tools = [
    {
        "name": "get_stock_price",
        "description": "获取股票当前价格",
        "input_schema": {
            "type": "object",
            "properties": {
                "symbol": {
                    "type": "string",
                    "description": "股票代码，如 AAPL"
                }
            },
            "required": ["symbol"]
        }
    }
]

# 发送消息
message = client.messages.create(
    model="claude-3-5-sonnet-20241022",
    max_tokens=1024,
    tools=tools,
    messages=[
        {
            "role": "user",
            "content": "苹果公司的股价是多少？"
        }
    ]
)

# 处理工具调用
if message.stop_reason == "tool_use":
    tool_use = message.content[-1]

    # 执行工具
    if tool_use.name == "get_stock_price":
        price = get_stock_price(tool_use.input["symbol"])

        # 返回结果
        response = client.messages.create(
            model="claude-3-5-sonnet-20241022",
            max_tokens=1024,
            tools=tools,
            messages=[
                {"role": "user", "content": "苹果公司的股价是多少？"},
                {"role": "assistant", "content": message.content},
                {
                    "role": "user",
                    "content": [
                        {
                            "type": "tool_result",
                            "tool_use_id": tool_use.id,
                            "content": f"${price}"
                        }
                    ]
                }
            ]
        )

        print(response.content[0].text)
```

---

## 资源链接

- **官方文档**: https://docs.anthropic.com/en/docs/build-with-claude/tool-use
- **API 参考**: https://docs.anthropic.com/en/api/messages
- **示例代码库**: https://github.com/anthropics/anthropic-cookbook

---

## 总结

Claude 的工具使用功能让 AI 能够：
- 🔧 调用外部函数和 API
- 🤖 自主决定何时使用工具
- 📊 处理结构化数据
- 🌐 访问实时信息

掌握工具使用是构建强大 AI Agent 的基础！
