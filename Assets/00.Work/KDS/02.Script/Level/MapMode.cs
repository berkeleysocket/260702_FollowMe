namespace FollowMe.KDS
{
    /// <summary>
    /// 맵 상태. 장애물·괴물은 씬에 미리 배치 후 모드에 따라 활성/비활성.
    /// 흐름: 안정 → 경고 → 추격 → 회복 (S16만 굴레).
    /// </summary>
    public enum MapMode
    {
        Stable = 0,   // 안정
        Warning = 1,  // 경고
        Chase = 2,    // 추격
        Recovery = 3, // 회복
        Torment = 4   // 굴레 (S16)
    }
}
