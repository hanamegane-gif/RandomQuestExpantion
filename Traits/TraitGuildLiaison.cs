class TraitGuildLiaison : TraitChara
{
    // "ギルドクエストでない通常の"ランダムクエストは生成させない
    // ギルドクエストは直接付与される
    public override bool CanGiveRandomQuest => false;

    public virtual string AffiliatedGuild => "";
}