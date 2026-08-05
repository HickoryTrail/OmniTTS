<div align="center">

# <image src="assests/Logo.png" Height="28" Width="28"/> OmniTTS <br>
<image src="assests/Banner-Dark.png"/>
[![正式版 Release](https://img.shields.io/github/v/release/HickoryTrail/OmniTTS?style=flat-square&color=%233fb950&label=正式版)](https://github.com/HickoryTrail/OmniTTS/releases/latest)
[![下载量](https://img.shields.io/github/downloads/HickoryTrail/OmniTTS/total?style=social&label=下载量&logo=github)](https://github.com/HickoryTrail/OmniTTS/releases/latest)
[![GitHub Repo Languages](https://img.shields.io/github/languages/top/HickoryTrail/OmniTTS?style=flat-square)](https://github.com/HickoryTrail/OmniTTS/search?l=c%23)
</div>


OmniTTS 是面向 [ClassIsland 2.x](https://github.com/ClassIsland/ClassIsland) 的多提供方 TTS 插件：把不同云端语音服务统一成一个配置入口和一套 .NET API，让课堂播报、通知和自动化脚本都能用同样的方式生成并播放语音。

## 为什么选择 OmniTTS

- **一个入口，多种提供方**：OpenAI、Gemini、FishAudio、ElevenLabs、MiniMax、MiMo。
- **直接播放或只生成缓存**：适合实时播报，也适合预生成提示音、批量生成音频。
- **统一参数模型**：提供方、模型、音色、语速、音量和文本由 `TtsOption` 描述。
- **并发与取消**：后台生成队列支持并发数配置；播放任务可取消或清空。
- **可扩展集成**：`OmniTTS.Shared` 提供公开的 `IOmniTTS` 接口，其他 ClassIsland 插件可直接注入使用。

## 适用场景

课堂开始/结束提示、临时通知、事件播报、无障碍朗读、批量生成课程音频，以及任何需要“文本 → MP3 → 播放”的 ClassIsland 自动化流程。

## 快速开始

1. 从 [Releases](https://github.com/HickoryTrail/OmniTTS/Release) 或 ClassIsland 插件市场下载 `OmniTTS.Plugin.cipx`，在 ClassIsland 的插件管理器中安装。
2. 打开 **设置 → OmniTTS 设置**，启用至少一个提供方并填写 API Key、模型和音色。
3. 选择默认提供方并保存。
4. 在 ClassIsland 中触发语音，或参考 [Shared API 文档](OmniTTS.Shared/README.md) 在自己的插件中调用。

插件的详细配置、发布和排错步骤见 [OmniTTS.Plugin/README.md](OmniTTS.Plugin/README.md)。

## 项目结构

| 目录               | 说明                                            |
| ---------------- | --------------------------------------------- |
| `OmniTTS.Plugin` | ClassIsland 插件、设置页面、提供方适配器和音频队列               |
| `OmniTTS.Shared` | 可被其他插件引用的 `IOmniTTS`、`TtsOption` 与 `Provider` |
| `assests`        | 项目 Logo 与 Banner 资源                           |
| `docs`           | 项目文档、更新日志等                                    |

## 支持与反馈

欢迎通过 [Issues](https://github.com/HickoryTrail/OmniTTS/issues) 报告问题或提出新的 TTS 提供方需求。提交问题时请附上 ClassIsland 版本、OmniTTS 版本、提供方和相关日志；不要粘贴 API Key。

## 感谢

本项目使用了一下第三方项目和框架：

- [ClassIsland.PluginSdk](https://github.com/ClassIsland/ClassIsland/blob/master/ClassIsland.PluginSdk)

- [Svg.Controls.Skia.Avalonia](https://github.com/wieslawsoltes/Svg.Skia)

- [OpenAI](https://github.com/openai/openai-dotnet)

- [RestSharp](https://github.com/restsharp/restsharp)

## 许可证

本项目中的以下项目基于 GNU Lesser General Public License v3.0 获得许可：

- [OmniTTS.Shared](https://github.com/HickoryTrail/OmniTTS/blob/master/OmniTTS.Shared)

本项目的其余部分（包括但不限于应用本体）基于 [GNU General Public License v3.0](https://github.com/ClassIsland/ClassIsland/blob/master/LICENSE.txt) 获得许可。