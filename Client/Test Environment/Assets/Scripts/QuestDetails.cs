using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RewardList = System.Collections.Generic.Dictionary<int, int>;

public class QuestDetails : MonoBehaviour
{
    //-----------------------------------------------
    // Public Variables
    //-----------------------------------------------

    // Manager to get information about the rewards
    public GameObject m_rewardManager;

    //-----------------------------------------------
    // Private Variables
    //-----------------------------------------------

    // The quest ID 
    private int m_questId = -1;

    // The quest description
    private string m_questDescription = "";

    // Dictionary of reward item Id and quantity
    private RewardList m_rewards;

    //-----------------------------------------------
    // Public Methods
    //-----------------------------------------------

    public void Init(int questId)
    {
        // Initialise questId
        m_questId = questId;

        // Gather information about quest
        PopulateQuest();

        // Get the Reward Manager
        RewardManager managerScript = m_rewardManager.GetComponent<RewardManager>();

        // Initialise information 
        managerScript.Init(m_questId, RewardManager.RewardSource.Quest);

        // Get the reward list from the manager
        m_rewards = managerScript.GetRewardList();
    }

    public RewardList GetRewardList()
    {
        return m_rewards;
    }

    public void PopulateQuest()
    {

    }

    public string GetQuestDescription()
    {
        return m_questDescription;
    }
}
