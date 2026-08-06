# OmniTTS.Plugin

OmniTTS 的 ClassIsland 2.x 插件实现。插件负责设置页面、云端 TTS 提供方适配、音频生成队列和播放队列；对外的稳定调用接口位于 [`OmniTTS.Shared`](../OmniTTS.Shared/README.md)。

## 安装使用

### 普通用户

1. 在项目 [Releases](https://github.com/HickoryTrail/OmniTTS/releases) 或ClassIsland插件市场下载最新的 `HickoryTrail.OmniTTS.cipx`。
2. 在 ClassIsland 的 **插件管理** 中导入 `.cipx`，按提示重启（如有提示）。
3. 打开 **设置 → OmniTTS 设置**。
4. 对需要使用的提供方点击编辑，打开“启用”，填写 Base URL、API Key、模型和 Voice ID。
5. 回到基本设置，选择已启用的默认提供方，并按机器与账号限流情况调整“最大并发请求数”。

设置会自动保存到插件配置目录的 `settings.json`；生成的 MP3 缓存位于同目录下的 `Cache` 文件夹。API Key 仅保存在本机，泄露给他人。

### 当前提供方

| `Provider`   | 显示名称       | 常用配置项                           |
| ------------ | ---------- | ------------------------------- |
| `OpenAI`     | OpenAI     | Base URL、API Key、Model、Voice    |
| `Gemini`     | Gemini     | Base URL、API Key、Model、Voice    |
| `FishAudio`  | FishAudio  | Base URL、API Key、Model、Voice ID |
| `Elevenlabs` | ElevenLabs | Base URL、API Key、Model、Voice ID |
| `MiniMax`    | MiniMax    | Base URL、API Key、Model、Voice ID |
| `MiMo`       | MiMo       | Base URL、API Key、Model、Voice ID |

Base URL 可用于兼容网关或自建代理；具体路径由对应适配器补全。模型和音色必须使用提供方支持的值。

## 排错

- **默认提供方不可选**：先在对应提供方编辑页启用并填写必需字段。
- **请求失败**：检查 Base URL、API Key、模型/音色是否与服务商控制台一致；确认网络和额度。
- **没有声音**：检查 ClassIsland 音量与输出设备，并确认请求没有被取消。
- **需要重新生成**：在设置页中清除缓存，然后再次请求，注意该操作会清空之前生成的所有缓存。