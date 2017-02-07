using System;
using System.Collections.Generic;
using Hash17.Programs;

namespace Hash17.Campaign
{
    [Serializable]
    public class CampaignItem
    {
        public int Id;
        public CampaignTriggerType Trigger;
        public CampaignActionType Action;
        public List<int> Dependecies;
        public string TriggerAditionalData;
        public string ActionAditionalData;
    }
}