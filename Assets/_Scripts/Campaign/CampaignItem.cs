using System;
using System.Collections.Generic;
using Hash17.Programs;

namespace Hash17.Campaign
{
    [Serializable]
    public class CampaignItem
    {
        public int Id;
        public CampaignTriggetType Type;
        public CampaignActionType Action;
        public List<int> Dependecies;
        public int EntityId;
        public string TriggerAditionalData;
        public string ActionAditionalData;
    }
}