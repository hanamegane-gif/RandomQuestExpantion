namespace RandomQuestExpantion.ModQuests.QuestAttribute
{
    // 野外エンカウント処理のオーバーライドを持つクラスにつける
    interface IOverrideWildernessEncounter
    {
        public void OnWildernessEncounted(Zone newZone);

        public bool ShouldOverrideEncounter();

        // 複数受けている際のイベント発生の優先順位の取得
        public int GetOverlappingPriority();
    }
}
