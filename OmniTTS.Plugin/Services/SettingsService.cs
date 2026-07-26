using Microsoft.Extensions.Logging;
using OmniTTS.Plugin.Models;
using ReactiveUI;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace OmniTTS.Plugin.Services
{
    internal class SettingsService : ReactiveObject
    {
        // 内部属性
        private ILogger<SettingsService>? Logger { get; set; }
        public string? PluginConfigFolder { get; set; }

        // 构造函数
        internal SettingsService(ILogger<SettingsService> logger)
        {
            Logger = logger;
        }

        internal async Task<SettingsService> InitializeAsync(string pluginConfigFolder)
        {
            if (string.IsNullOrWhiteSpace(pluginConfigFolder) || !Directory.Exists(pluginConfigFolder))
            {
                PluginConfigFolder = pluginConfigFolder;
            }
            else
            {
                Logger?.LogError("Invalid plugin config folder path: {PluginConfigFolder}", pluginConfigFolder);
                throw new ArgumentException("Invalid plugin config folder path", nameof(pluginConfigFolder));
            }
            await LoadSettings();
            await SaveSettings(); // 确保首次运行时生成 settings.json 文件
            // 设置变化时自动保存，涵盖所有层级子项
            ObserveSettingTree();
            return this;
        }

        // 设置内容
        internal SettingsModel Setting { get; private set; } = new SettingsModel();

        // ========== 自动保存：监听设置树中任意属性的变化 ==========

        // 已订阅的 ReactiveObject 集合，防止重复订阅
        private readonly HashSet<INotifyPropertyChanged> _observedObjects = new();
        private bool _autoSavePending;

        /// <summary>
        /// 在构造函数中调用，开始监听当前 Setting 及其所有子项的属性变化。
        /// </summary>
        private void ObserveSettingTree()
        {
            // 监听自身，当 Setting 实例被整体替换时重新订阅
            ObserveReactiveObject(this);
            // 递归监听当前 Setting 树
            ObserveRecursive(Setting);
        }

        /// <summary>
        /// 递归订阅 obj 及其所有 ReactiveObject 子属性的 PropertyChanged 事件。
        /// </summary>
        private void ObserveRecursive(ReactiveObject? obj)
        {
            if (obj == null) return;
            if (!_observedObjects.Add(obj)) return;

            obj.PropertyChanged += OnSettingPropertyChanged;

            // 继续递归子级 ReactiveObject 属性
            if (obj is SettingsModel sm)
            {
                ObserveRecursive(sm.ProviderSetting);
            }
            else if (obj is SettingsModel.ProviderSettings ps)
            {
                ObserveRecursive(ps.OpenAISetting);
                ObserveRecursive(ps.FishAudioSetting);
                ObserveRecursive(ps.ElevenLabsSetting);
                ObserveRecursive(ps.GeminiSetting);
                ObserveRecursive(ps.MiniMaxSetting);
            }
            // 叶子级设置类（OpenAISettings 等）无嵌套 ReactiveObject，递归自动终止
        }

        /// <summary>
        /// 任意层级设置变化时触发，负责订阅新替换的子实例并自动保存。
        /// </summary>
        private void OnSettingPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Setting 实例被整体替换（如从文件加载后），清除旧订阅，重新观察新实例
            if (sender == this && e.PropertyName == nameof(Setting))
            {
                _observedObjects.Clear();
                ObserveReactiveObject(this);
                ObserveRecursive(Setting);
                TriggerAutoSave();
                return;
            }

            // 检测 ProviderSettings 中某个子设置实例被替换（设为新对象或 null）
            if (sender is SettingsModel.ProviderSettings ps)
            {
                switch (e.PropertyName)
                {
                    case nameof(SettingsModel.ProviderSettings.OpenAISetting):
                        ObserveRecursive(ps.OpenAISetting); break;
                    case nameof(SettingsModel.ProviderSettings.FishAudioSetting):
                        ObserveRecursive(ps.FishAudioSetting); break;
                    case nameof(SettingsModel.ProviderSettings.ElevenLabsSetting):
                        ObserveRecursive(ps.ElevenLabsSetting); break;
                    case nameof(SettingsModel.ProviderSettings.GeminiSetting):
                        ObserveRecursive(ps.GeminiSetting); break;
                    case nameof(SettingsModel.ProviderSettings.MiniMaxSetting):
                        ObserveRecursive(ps.MiniMaxSetting); break;
                }
            }
            // SettingsModel 中 ProviderSetting 实例被替换
            else if (sender is SettingsModel sm && e.PropertyName == nameof(SettingsModel.ProviderSetting))
            {
                ObserveRecursive(sm.ProviderSetting);
            }

            TriggerAutoSave();
        }

        /// <summary>
        /// 对 INotifyPropertyChanged 对象订阅，用于统一处理 Setting 实例替换。
        /// </summary>
        private void ObserveReactiveObject(INotifyPropertyChanged obj)
        {
            if (obj == null || !_observedObjects.Add(obj)) return;
            obj.PropertyChanged += OnSettingPropertyChanged;
        }

        /// <summary>
        /// 防重入的自动保存触发。
        /// </summary>
        private void TriggerAutoSave()
        {
            if (_autoSavePending) return;
            _autoSavePending = true;
            _ = AutoSaveAsync();
        }

        private async Task AutoSaveAsync()
        {
            try
            {
                await SaveSettings();
            }
            finally
            {
                _autoSavePending = false;
            }
        }

        // ========== JSON 序列化配置 ==========

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { IncludeNonPublicProperties }
            },
            Converters = { new JsonStringEnumConverter() }
        };

        private static void IncludeNonPublicProperties(JsonTypeInfo typeInfo)
        {
            if (typeInfo.Kind != JsonTypeInfoKind.Object) return;

            // 所有属性设为非必需，JSON 中缺少字段时静默保留实例默认值，不抛异常
            foreach (var prop in typeInfo.Properties)
            {
                prop.IsRequired = false;
            }

            // 纳入 internal 属性的 getter/setter，无需在 Model 上加 [JsonInclude]
            var existingNames = new HashSet<string>(
                typeInfo.Properties.Select(p => p.Name),
                StringComparer.OrdinalIgnoreCase);
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var propInfo in typeInfo.Type.GetProperties(flags))
            {
                if (existingNames.Contains(propInfo.Name)) continue;
                if (propInfo.GetMethod is null || propInfo.SetMethod is null) continue;
                if (propInfo.GetIndexParameters().Length > 0) continue;

                var jsonProp = typeInfo.CreateJsonPropertyInfo(propInfo.PropertyType, propInfo.Name);
                jsonProp.Get = propInfo.GetValue;
                jsonProp.Set = propInfo.SetValue;
                jsonProp.IsRequired = false;
                typeInfo.Properties.Add(jsonProp);
            }
        }

        // ========== 加载 / 保存 ==========

        private async Task LoadSettings()
        {
            var filepath = Path.Combine(PluginConfigFolder ?? "", "settings.json");
            if (!File.Exists(filepath))
            {
                Logger?.LogInformation("设置文件不存在 {Path}，使用默认设置。", filepath);
                return;
            }

            try
            {
                var json = await File.ReadAllTextAsync(filepath);
                var loaded = JsonSerializer.Deserialize<SettingsModel>(json, JsonOptions);
                if (loaded != null)
                {
                    Setting = loaded;
                    Logger?.LogInformation("设置已从 {Path} 加载。", filepath);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "读取设置文件 {Path} 失败，使用默认设置。", filepath);
            }
        }

        private async Task SaveSettings()
        {
            var filepath = Path.Combine(PluginConfigFolder ?? "", "settings.json");
            var dir = Path.GetDirectoryName(filepath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(Setting, JsonOptions);
            await File.WriteAllTextAsync(filepath, json);
            Logger?.LogInformation("设置已保存到 {Path}。", filepath);
        }
    }
}
