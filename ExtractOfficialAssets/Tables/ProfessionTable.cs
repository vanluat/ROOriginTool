using Serializer;

namespace ExtractOfficialAssets.Tables;

public class ProfessionTable
{
    public int Id;
    public string Name;
    public string EnglishName;
    public string ProfessionIcon;
    public int ParentProfession;
    public int PresentM;
    public int PresentF;
    public int DefaultEquipM;
    public int DefaultEquipF;
    public float MoveSpeed;
    public string AI;
    public MSeqList<int> SkillIds;
    public MSeqList<int> ParentTransfromSkills;
    public MSeqList<int> RootTransfromSkills;
    public int BaseLvRequired;
    public int JobLvRequired;
    public int MissionRequired;
    public int SkillPointRequired;
    public MSeqList<int> FixedSkillIds;
    public string AttrRecommend;
    public int HatredCount;
    public int ASPD;
    public MSeqList<int> BornBuff;
    public int PreinstallAttr;
    public string SkillTabName;
    public string SkillTabIcon;
    public int CommonAttackSkillID;
    public int IsCommonAttack;
    public int DefaultWeaponID;
    public uint SkillLineNum;
    public MSeqList<int> AttrAriseList;
    public int[] FixedUnlockSkillIds;
    public uint ProfessionType;
    public string BagShowSkillSlot;
    public int[] FixedCommonSkillIds;
    public int ProfessionPreSwitch;
    public int[] MercenaryRecommend;
    public int[] MatchDuty;
    public MSeq<string> RrofessionPainting;
    public string SkillTabAtlas;
    public MSeq<string> ProfessionSharePic;
    public MSeq<string> ChooseJobPic;
    public int Gender;

    public void Parse(int idx,object[,] row)
    {
        Id = int.Parse(row[idx, 0].ToString());
        AI = (string)row[idx, 10];
        ASPD = int.Parse(row[idx, 21].ToString());
        AttrAriseList = (MSeqList<int>)row[idx, 30];
        AttrRecommend = (string)row[idx, 19];
        BagShowSkillSlot = (string)row[idx, 33];
        BaseLvRequired = int.Parse(row[idx, 14].ToString());
        BornBuff = (MSeqList<int>)row[idx, 22];
        ChooseJobPic = (MSeq<string>)row[idx, 41];
        CommonAttackSkillID = int.Parse(row[idx, 26].ToString());
        DefaultEquipF = int.Parse(row[idx, 8].ToString());
        DefaultEquipM = int.Parse(row[idx, 7].ToString());
        DefaultWeaponID = int.Parse(row[idx, 28].ToString());
        EnglishName = (string)row[idx, 2];
        FixedCommonSkillIds = (int[])row[idx, 34];
        FixedSkillIds = (MSeqList<int>)row[idx, 18];
        FixedUnlockSkillIds = (int[])row[idx, 31];
        Gender = int.Parse(row[idx, 42].ToString());
        HatredCount = int.Parse(row[idx, 20].ToString());
        IsCommonAttack = int.Parse(row[idx, 27].ToString());
        JobLvRequired = int.Parse(row[idx, 15].ToString());
        MatchDuty = (int[])row[idx, 37];
        MissionRequired = int.Parse(row[idx, 16].ToString());
        MoveSpeed = float.Parse(row[idx, 9].ToString());
        Name = (string)row[idx, 1];
        ParentProfession = int.Parse(row[idx, 4].ToString());
        ParentTransfromSkills = (MSeqList<int>)row[idx, 12];
        PreinstallAttr = int.Parse(row[idx, 23].ToString());
        PresentF = int.Parse(row[idx, 6].ToString());
        PresentM = int.Parse(row[idx, 5].ToString());
        ProfessionIcon = (string)row[idx, 3];
        ProfessionPreSwitch = int.Parse(row[idx, 35].ToString());
        ProfessionSharePic = (MSeq<string>)row[idx, 40];
        ProfessionType = uint.Parse(row[idx, 32].ToString());
        RootTransfromSkills = (MSeqList<int>)row[idx, 13];
        RrofessionPainting = (MSeq<string>)row[idx, 38];
        SkillIds = (MSeqList<int>)row[idx, 11];
        SkillLineNum = uint.Parse(row[idx, 29].ToString());
        SkillPointRequired = int.Parse(row[idx, 17].ToString());
        SkillTabAtlas = (string)row[idx, 39];
        SkillTabIcon = (string)row[idx, 25];
        SkillTabName = (string)row[idx, 24];
    }
}