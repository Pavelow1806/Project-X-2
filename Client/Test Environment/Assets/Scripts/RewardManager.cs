using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RewardList = System.Collections.Generic.Dictionary<int, int>;

public class RewardManager : MonoBehaviour
{
    //-----------------------------------------------
    // Public Variables
    //-----------------------------------------------

    // Where is the request for a reward coming from
    public enum RewardSource
    {
        Quest,
    };

    //-----------------------------------------------
    // Private Variables
    //-----------------------------------------------

    // Dictionary of reward item Id and quantity
    private RewardList m_rewards = new RewardList();

    // If reward comes from quest use quest Id
    private int m_questId = -1;

    //-----------------------------------------------
    // Public Methods
    //-----------------------------------------------

    public void Init(int sourceId, RewardSource sourceType) 
    {
        switch (sourceType)
        {
            case RewardSource.Quest:
                m_questId = sourceId;
                FillRewardList(m_questId);
                break;

            default:
                break;
        }
    }

    public RewardList GetRewardList()
    {
        return m_rewards;
    }


    //-----------------------------------------------
    // Private Methods
    //-----------------------------------------------

    private void FillRewardList(int id)
    {
        if (id == 10)
        {
            m_rewards.Add(1, 200);
        }
    }
}
