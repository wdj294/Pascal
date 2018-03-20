// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A world model is a collection of facts that an observer believes to be true,
    /// representing the observer's knowledge of the current world state.
    /// </summary>
    [System.Serializable]
    public class WorldModel
    {

        [SerializeField]
        private Fact m_observer;

        [SerializeField]
        private Fact m_observed;

        [SerializeField]
        private List<Fact> m_facts = new List<Fact>();

        public Fact observer
        {
            get { return m_observer; }
            set { m_observer = value; }
        }

        public Fact observed
        {
            get { return m_observed; }
            set { m_observed = value; }
        }

        public List<Fact> facts
        {
            get { return m_facts; }
            set { m_facts = value; }
        }

        public WorldModel(Fact observer)
        {
            this.observer = observer;
        }
        
        public WorldModel(WorldModel source)
        {
            observer = new Fact(source.observer); //[TODO] Pool.
            observed = new Fact(source.observed);
            foreach (var fact in source.facts)
            {
                facts.Add(new Fact(fact));
            }
        }

        public Fact FindFact(DomainType domainType, EntityType entityType)
        {
            return facts.Find(f => (f.domainType == domainType && f.entityType == entityType));
        }

        public bool ContainsFact(DomainType domainType, EntityType entityType, int min, int max)
        {
            var fact = FindFact(domainType, entityType);
            return (fact != null && min <= fact.count && fact.count <= max);
        }

        public void AddEntityType(DomainType domainType, EntityType entityType, int count = 1)
        {
            var fact = FindFact(domainType, entityType);
            if (fact != null)
            {
                fact.count += count;
            }
            else
            {
                facts.Add(new Fact(domainType, entityType, count));
            }
        }

        public void RemoveEntityType(DomainType domainType, EntityType entityType, int count = 1)
        {
            var existingFact = FindFact(domainType, entityType);
            if (existingFact != null)
            {
                if (count < existingFact.count)
                {
                    existingFact.count -= count;
                }
                else
                {
                    facts.Remove(existingFact);
                }
            }
        }

        public void AddFact(Fact fact)
        {
            var existingFact = FindFact(fact.domainType, fact.entityType);
            if (existingFact != null)
            {
                existingFact.count = Mathf.Max(existingFact.count, fact.count);
            }
            else
            {
                facts.Add(new Fact(fact));
            }
        }

        public void RemoveFact(Fact fact)
        {
            var existingFact = FindFact(fact.domainType, fact.entityType);
            if (existingFact != null)
            {
                if (fact.count < existingFact.count)
                {
                    existingFact.count -= fact.count;
                }
                else
                {
                    facts.Remove(existingFact);
                }
            }
        }

        public float ComputeUrgency(UrgentFactSelectionMode factSelectionMode, out Fact[] mostUrgentFacts, string[] ignoreList = null, bool debug = false)
        {
            var mostUrgentFactsList = new List<Fact>();
            Fact mostUrgentFact = null;
            float cumulativeUrgency = 0;
            float topUrgency = Mathf.NegativeInfinity;
            var debugInfo = QuestMachine.debug ? "WORLD MODEL: (observer:" + observer.entityType.name + ")\n" : string.Empty;
            for (int i = 0; i < facts.Count; i++)
            {
                var fact = facts[i];
                if (fact == null) continue;
                if (ShouldIgnore(fact.entityType.name, ignoreList)) continue;
                observed = fact;
                var urgency = GetFactUrgency(fact);
                cumulativeUrgency += urgency;
                if (urgency > topUrgency)
                {
                    topUrgency = urgency;
                    mostUrgentFactsList.Clear();
                    mostUrgentFactsList.Add(fact);
                    mostUrgentFact = fact;
                }
                if (debug)
                {
                    debugInfo += string.Format("Domain:{0}, EntityType:{1}, Count:{2}\n", new object[] { fact.domainType.name, fact.entityType.name, fact.count });
                    debugInfo += string.Format("   Urgency:{0}\n", new object[] { urgency });
                }
            }
            if (debug)
            {
                if (mostUrgentFact != null)
                {
                    debugInfo += string.Format("MOST URGENT: Domain:{0}, EntityType:{1}, Count:{2}, Urgency:{3}\n", new object[] { mostUrgentFact.domainType.name, mostUrgentFact.entityType.name, mostUrgentFact.count, topUrgency });
                }
                debugInfo += string.Format("CUMULATIVE URGENCY: {0}", new object[] { cumulativeUrgency });
                Debug.Log(debugInfo);
            }
            mostUrgentFacts = mostUrgentFactsList.ToArray();
            return cumulativeUrgency;
        }

        public float ComputeUrgency(out Fact mostUrgentFact, string[] ignoreList = null)
        {
            Fact[] mostUrgentFacts;
            float urgency;
            urgency = ComputeUrgency(UrgentFactSelectionMode.mostUrgent, out mostUrgentFacts, ignoreList);
            mostUrgentFact = (mostUrgentFacts.Length > 0) ? mostUrgentFacts[0] : null;
            return urgency;
        }

        public float GetFactUrgency(Fact fact)
        {
            float urgency = 0;
            foreach (var urgencyFunction in fact.entityType.urgencyFunctions)
            {
                urgency += urgencyFunction.Compute(this);
            }
            return urgency;
        }

        private bool ShouldIgnore(string entityTypeName, string[] ignoreList)
        {
            if (string.IsNullOrEmpty(entityTypeName) || ignoreList == null) return false;
            for (int i = 0; i < ignoreList.Length; i++)
            {
                if (string.Equals(entityTypeName, ignoreList[i])) return true;
            }
            return false;
        }

        public void ApplyAction(Fact fact, Action action)
        {
            observed = fact;
            foreach (var effect in action.effects)
            {
                var domainType = effect.domainSpecifier.GetDomainType(this);
                var entityType = effect.entitySpecifier.GetEntityType(this);
                switch (effect.operation)
                {
                    case ActionEffect.Operation.Add:
                        AddEntityType(domainType, entityType, effect.count);
                        break;
                    case ActionEffect.Operation.Remove:
                        RemoveEntityType(domainType, entityType, effect.count);
                        break;
                }
            }
        }

        public bool AreRequirementsMet(ActionRequirement[] requirements)
        {
            if (requirements != null)
            {
                foreach (var requirement in requirements)
                {
                    var contains = ContainsFact(requirement.domainSpecifier.GetDomainType(this), requirement.entitySpecifier.GetEntityType(this), requirement.min, requirement.max);
                    if (requirement.not == false && !contains) return false;
                    if (requirement.not == true && contains) return false;
                    if (requirement.requirementFunction != null && !requirement.requirementFunction.IsTrue(this)) return false;
                }
            }
            return true;
        }

    }

}