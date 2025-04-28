using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    public int AssignedSkillKey { get; private set; } = -1;

    public void AssignSkill(int skillKey)
    {
        AssignedSkillKey = skillKey;
        Debug.Log($"SkillSlot {gameObject.name}�� ��ų {skillKey} ��ϵ�");
    }

    public bool HasAssignedSkill(int skillKey)
    {
        return AssignedSkillKey == skillKey;
    }

    public void ClearSlot()
    {
        AssignedSkillKey = -1;
        Debug.Log($"SkillSlot {gameObject.name} �����");
    }
}
