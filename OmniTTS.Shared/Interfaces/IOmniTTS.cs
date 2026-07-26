namespace OmniTTS.Shared;

public interface IOmniTTS
{
    /// <summary>
    /// 异步生成并播放音频
    /// </summary>
    /// <param name="option"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public Task PlayAudioAsync(TtsOption option, CancellationToken cts);

    /// <summary>
    /// 生成并播放音频
    /// </summary>
    /// <param name="option"></param>
    /// <param name="cts"></param>
    public void PlayAudio(TtsOption option, CancellationToken cts);

    /// <summary>
    /// 异步生成并播放音频
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public Task PlayAudioAsync(string text, CancellationToken cts);

    /// <summary>
    /// 生成并播放音频
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public void PlayAudio(string text, CancellationToken cts);

    /// <summary>
    /// 清除所有音频播放任务
    /// </summary>
    public void CancelAllAudio();





    /// <summary>
    /// 生成音频缓存
    /// </summary>
    /// <param name="option"></param>
    /// <param name="cts"></param>
    public void GenerateCache(TtsOption option, CancellationToken cts);

    /// <summary>
    /// 生成音频缓存
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public void GenerateCache(string text, CancellationToken cts);

    /// <summary>
    /// 异步生成音频缓存
    /// </summary>
    /// <param name="option"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public Task<string> GenerateCacheAsync(TtsOption option, CancellationToken cts);

    /// <summary>
    /// 异步生成音频缓存
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public Task<string> GenerateCacheAsync(string text, CancellationToken cts);




    /// <summary>
    /// 批量生成音频缓存
    /// </summary>
    /// <param name="texts"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public void GenerateCache(List<string> texts, CancellationToken cts);

    /// <summary>
    /// 异步批量生成音频缓存
    /// </summary>
    /// <param name="texts"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public Task<Dictionary<string,string>> GenerateCacheAsync(List<string> texts, CancellationToken cts);

    /// <summary>
    /// 批量生成音频缓存
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public void GenerateCache(List<TtsOption> options, CancellationToken cts);

    /// <summary>
    /// 异步批量生成音频缓存
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public Task<Dictionary<string, string>> GenerateCacheAsync(List<TtsOption> options, CancellationToken cts);



    /// <summary>
    /// 清除所有音频缓存
    /// </summary>
    /// <returns></returns>
    public Task ClearCache();
}
