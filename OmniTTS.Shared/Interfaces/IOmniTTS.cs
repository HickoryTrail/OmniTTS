namespace OmniTTS.Shared;

public interface IOmniTTS
{
    /// <summary>
    /// 生成并播放音频
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public Task PlayAudio(string text, CancellationToken cts);

    /// <summary>
    /// 清除所有音频播放任务
    /// </summary>
    public void CancelAllAudio();

    /// <summary>
    /// 生成音频缓存
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public Task<string> GenerateCache(string text, CancellationToken cts);

    /// <summary>
    /// 批量生成音频缓存
    /// </summary>
    /// <param name="texts"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public Task<Dictionary<string,string>> GenerateCache(List<string> texts, CancellationToken cts);

    /// <summary>
    /// 清除所有音频缓存
    /// </summary>
    /// <returns></returns>
    public Task ClearCache();
}
