# OmniTTS.Shared

`OmniTTS.Shared` 是 OmniTTS 对其他 ClassIsland 插件公开的最小 SDK，目标框架为 `net10.0`。它只包含三类公开类型：

- `IOmniTTS`：播放、生成缓存、批量缓存和清理缓存。
- `TtsOption`：一次请求的完整描述。
- `Provider`：可用的 TTS 提供方枚举。

## 安装与注入

在插件项目中添加项目引用或发布后的程序集引用：

```xml
<ProjectReference Include="..\OmniTTS.Shared\OmniTTS.Shared.csproj" />
```

在 ClassIsland 启动完成后从服务容器获取服务。OmniTTS.Plugin 已注册 `IOmniTTS`，调用方无需自行创建客户端或管理 API Key：

```csharp
using ClassIsland.Shared;
using OmniTTS.Shared;

// 例如在 AppBase.Current.AppStarted 之后执行
var tts = IAppHost.GetService<IOmniTTS>();
await tts.PlayAudioAsync("欢迎使用 OmniTTS。", CancellationToken.None);
```

## 最小快速请求

只传文本时，插件使用设置页中的默认提供方、模型、音色、语速和音量：

```csharp
var cts = new CancellationTokenSource();
var tts = IAppHost.GetService<IOmniTTS>();

await tts.PlayAudioAsync("请准备开始上课。", cts.Token);
```

`PlayAudio` 是不等待完成的 fire-and-forget 版本；需要知道生成和播放何时结束时，优先使用 `PlayAudioAsync`。

## 完整请求解析

```csharp
var request = new TtsOption
{
    Provider = Provider.OpenAI,
    Model = "gpt-4o-mini-tts",
    Voice = "alloy",
    Text = "这是一条使用显式参数的测试语音。",
    Speed = 1.0f,
    Volume = 0.8f
};

string cachePath = await tts.GenerateCacheAsync(request, CancellationToken.None);
await tts.PlayAudioAsync(request, CancellationToken.None);
```

| 字段 | 类型 | 省略时的行为 |
| --- | --- | --- |
| `Provider` | `Provider?` | 使用设置页的默认提供方；默认值为 `None` 时抛出异常 |
| `Model` | `string?` | 使用该提供方的设置值 |
| `Voice` | `string?` | 使用该提供方的设置值 |
| `Text` | `string`（必填） | 空白文本会抛出 `ArgumentException` |
| `Speed` | `float?` | 使用该提供方设置的语速，默认 `1.0` |
| `Volume` | `float?` | 使用该提供方设置的音量，默认 `1.0` |

请求进入后台生成队列后，适配器会按选中的提供方发起请求并写入 MP3。单条缓存请求若命中同一组参数的缓存，会直接返回已有文件；缓存路径形如：

```text
<插件配置目录>/Cache/<参数哈希>.mp3
```

## API 方法

下面的 `cts` 都是调用方传入的 `CancellationToken`。不需要取消时传 `CancellationToken.None`；需要在页面关闭、插件卸载或用户点击“停止”时中止请求时，传入 `CancellationTokenSource.Token`。

### 签名总览

以下签名与 `Interfaces/IOmniTTS.cs` 一致，可直接作为调用方的类型参考：

```csharp
Task PlayAudioAsync(TtsOption option, CancellationToken cts);
void PlayAudio(TtsOption option, CancellationToken cts);
Task PlayAudioAsync(string text, CancellationToken cts);
void PlayAudio(string text, CancellationToken cts);
void CancelAllAudio();

void GenerateCache(TtsOption option, CancellationToken cts);
void GenerateCache(string text, CancellationToken cts);
Task<string> GenerateCacheAsync(TtsOption option, CancellationToken cts);
Task<string> GenerateCacheAsync(string text, CancellationToken cts);

void GenerateCache(List<string> texts, CancellationToken cts);
Task<Dictionary<string, string>> GenerateCacheAsync(
    List<string> texts, CancellationToken cts);
void GenerateCache(List<TtsOption> options, CancellationToken cts);
Task<Dictionary<string, string>> GenerateCacheAsync(
    List<TtsOption> options, CancellationToken cts);

Task ClearCache();
```

### 播放方法

#### `Task PlayAudioAsync(TtsOption option, CancellationToken cts)`

使用 `option` 描述的参数生成音频，并在生成完成后交给 ClassIsland 播放服务；任务会在播放完成后才结束。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `option` | `TtsOption` | 完整请求。`Text` 必填；`Provider`、`Model`、`Voice`、`Speed`、`Volume` 为 `null` 时从默认提供方设置补全。 |
| `cts` | `CancellationToken` | 贯穿生成、写入缓存和播放过程的取消令牌。 |

返回值为表示整个流程的 `Task`，可用 `await` 等待并用 `try/catch` 处理异常。文本为空、未设置默认提供方、提供方请求失败或播放失败时会失败。

#### `Task PlayAudioAsync(string text, CancellationToken cts)`

文本请求的快捷重载。内部创建 `TtsOption { Text = text }`，其余字段全部使用设置页中默认提供方的配置，然后等待生成与播放完成。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `text` | `string` | 要朗读的文本，不能为 `null`、空字符串或全空白。 |
| `cts` | `CancellationToken` | 取消生成或播放。 |

#### `void PlayAudio(TtsOption option, CancellationToken cts)`

完整参数的非阻塞版本：启动后台任务后立即返回，不等待生成或播放结束。适合事件回调、无需等待语音完成的通知场景。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `option` | `TtsOption` | 与 `PlayAudioAsync(TtsOption, ...)` 相同。 |
| `cts` | `CancellationToken` | 与后台任务绑定；令牌应由调用方持有并在适当时机取消。 |

该方法没有返回值，后台异常不会回传给调用方，而是由 OmniTTS 记录日志。若必须确认完成或捕获异常，请使用异步重载。

#### `void PlayAudio(string text, CancellationToken cts)`

文本请求的非阻塞版本。参数含义与 `PlayAudioAsync(string, ...)` 相同，使用默认配置并立即返回；后台异常只写入日志。

#### `void CancelAllAudio()`

取消当前播放队列中尚未开始播放的项目，并将这些项目标记为取消。该方法：

- 没有参数，也没有返回值；
- 不会删除已经生成的 MP3 缓存；
- 不会强制终止已经在提供方处执行的网络生成；
- 若要同时中止生成，应另外取消传给请求的 `CancellationTokenSource`。

### 单条缓存方法

#### `Task<string> GenerateCacheAsync(TtsOption option, CancellationToken cts)`

根据完整参数生成 MP3 缓存，但不播放。成功时返回缓存文件的绝对路径；如果同一参数的缓存已存在，会直接返回已有路径。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `option` | `TtsOption` | 生成参数；字段补全和校验规则与 `PlayAudioAsync(TtsOption, ...)` 相同。 |
| `cts` | `CancellationToken` | 取消排队、网络请求或文件写入。 |

`Task<string>` 完成后即可把路径交给其他音频处理逻辑。示例：

```csharp
var path = await tts.GenerateCacheAsync(
    new TtsOption { Text = "课前提示音" },
    CancellationToken.None);
Console.WriteLine(path);
```

#### `Task<string> GenerateCacheAsync(string text, CancellationToken cts)`

单条文本缓存的快捷重载。内部使用默认提供方设置创建选项，返回生成或命中的 MP3 路径，不播放音频。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `text` | `string` | 要转换为语音的文本，不可为空白。 |
| `cts` | `CancellationToken` | 取消本次缓存生成。 |

#### `void GenerateCache(TtsOption option, CancellationToken cts)`

完整参数缓存生成的非阻塞版本。方法只负责把任务交给后台队列并立即返回，不提供文件路径；生成失败、取消或网络异常通过日志体现。需要路径或可靠错误处理时使用异步版本。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `option` | `TtsOption` | 与异步完整参数版本相同。 |
| `cts` | `CancellationToken` | 与后台生成任务绑定。 |

#### `void GenerateCache(string text, CancellationToken cts)`

单条文本缓存生成的非阻塞快捷版本。使用默认配置，参数与 `GenerateCacheAsync(string, ...)` 相同；立即返回且不返回路径。

### 批量缓存方法

批量异步方法会为每个有效项目创建生成任务，等待本批次任务完成后返回字典。字典的键是原文本，值是对应 MP3 路径。每项仍会执行 `TtsOption` 的默认值补全和文本校验。

#### `Task<Dictionary<string, string>> GenerateCacheAsync(List<string> texts, CancellationToken cts)`

批量处理只包含文本的请求。每个文本都使用设置页默认提供方、模型、音色、语速和音量。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `texts` | `List<string>` | 待生成文本列表；空白项会被跳过，正常文本作为字典键。 |
| `cts` | `CancellationToken` | 取消尚未完成的批量请求。 |

返回值是 `text -> cachePath` 字典；列表为空或所有项均为空白时返回空字典。

#### `Task<Dictionary<string, string>> GenerateCacheAsync(List<TtsOption> options, CancellationToken cts)`

批量处理带独立参数的请求，适合同一批文本使用不同提供方、模型或音色。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `options` | `List<TtsOption>` | 每个元素是一条完整请求；可只填写 `Text`，其余字段按默认配置补全。 |
| `cts` | `CancellationToken` | 取消整个批次的等待和未完成请求。 |

返回字典仍以每项的 `Text` 为键。如果列表中存在重复文本，字典无法建立重复键，应在调用前去重或改用唯一文本。

#### `void GenerateCache(List<string> texts, CancellationToken cts)`

文本列表批量生成的非阻塞版本。使用默认配置，立即返回，不返回路径；后台异常只记录日志。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `texts` | `List<string>` | 待生成的文本列表。 |
| `cts` | `CancellationToken` | 批量任务的取消令牌。 |

#### `void GenerateCache(List<TtsOption> options, CancellationToken cts)`

带独立 `TtsOption` 列表的非阻塞版本。立即返回，不返回路径；参数含义与异步列表重载相同，后台异常只记录日志。

| 参数 | 类型 | 说明 |
| --- | --- | --- |
| `options` | `List<TtsOption>` | 每项可以指定不同的提供方、模型、音色、语速和音量。 |
| `cts` | `CancellationToken` | 批量后台任务的取消令牌。 |

### 缓存管理

#### `Task ClearCache()`

异步删除插件配置目录下 `Cache` 文件夹中的所有文件和子目录，返回的 `Task` 在清理完成后结束。

该方法没有参数，也不会修改 `settings.json`、提供方配置或默认提供方。清理后再次调用 `GenerateCacheAsync` 会重新生成 MP3。

```csharp
await tts.ClearCache();
```

### 批量示例

```csharp
var paths = await tts.GenerateCacheAsync(
    new List<TtsOption>
    {
        new() { Text = "第一项" },
        new() { Text = "第二项", Provider = Provider.Gemini, Voice = "Kore" }
    },
    CancellationToken.None);

// key 为原文本，value 为对应的 MP3 路径
foreach (var (text, path) in paths)
    Console.WriteLine($"{text} -> {path}");
```

也可以传入 `List<string>`，此时所有项目都使用默认提供方：

```csharp
var paths = await tts.GenerateCacheAsync(
    new List<string> { "第一项", "第二项" },
    CancellationToken.None);
```

## 取消、异常与并发

- 传入已取消的 `CancellationToken` 时，请求会尽早返回或抛出 `OperationCanceledException`；网络请求、写文件和播放均使用该令牌。
- 异步方法的异常可用 `try/catch` 捕获。非异步 `void` 方法无法把异常返回给调用方，内部会记录日志。
- 生成并发数由插件设置中的“最大并发请求数”控制；调用方无需自行创建线程或队列。
- 调用前必须先在 OmniTTS 设置页启用提供方并设置默认提供方，否则无法解析省略的 `Provider`。

## 枚举值

`Provider.None`、`Provider.FishAudio`、`Provider.Elevenlabs`、`Provider.OpenAI`、`Provider.Gemini`、`Provider.MiniMax`、`Provider.MiMo`。
