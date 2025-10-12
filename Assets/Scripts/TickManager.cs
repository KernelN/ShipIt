using System.Collections.Generic;
using UnityEngine;

namespace ShipIt.TickManaging
{
    public class TickManager : Universal.Singleton<TickManager>
    {
        List<TickGroup> tickGroups;
        Dictionary<float, TickGroup> groupsByTime;
        List<TickGroup> uTickGroups;
        Dictionary<float, TickGroup> groupsByUtime;
        internal override void Awake()
        {
            base.Awake();
            if(this != inst) return;

            tickGroups = new List<TickGroup>();
            groupsByTime = new Dictionary<float, TickGroup>();
            uTickGroups = new List<TickGroup>();
            groupsByUtime = new Dictionary<float, TickGroup>();
        }
        void Update()
        {
            for (int i = 0; i < tickGroups.Count; i++)
                tickGroups[i].Update(Time.deltaTime);
            
            for(int i = 0; i < uTickGroups.Count; i++)
                uTickGroups[i].Update(Time.unscaledDeltaTime);
        }
        void LateUpdate()
        {
            for (int i = 0; i < tickGroups.Count; i++)
                tickGroups[i].LateUpdate();
            
            for(int i = 0; i < uTickGroups.Count; i++)
                uTickGroups[i].LateUpdate();
        }

        void Suscribe(List<TickGroup> groups, Dictionary<float, TickGroup> groupsDict,
                        float time, System.Action action, bool isLate = false)
        {
            if(groupsDict.TryGetValue(time, out TickGroup group))
                group.Add(action, isLate);
            else
            {
                TickGroup newGroup = new TickGroup(time);
                newGroup.Add(action, isLate);
                groupsDict.Add(time, newGroup);
                groups.Add(newGroup);
            }
        }
        void Remove(List<TickGroup> groups, Dictionary<float, TickGroup> groupsDict,
            float time, System.Action action, bool isLate = false)
        {
            if(!groupsDict.TryGetValue(time, out TickGroup group)) return;

            if (group.Remove(action, isLate))
            {
                groupsDict.Remove(time);
                groups.Remove(group);
            }
        }
        public void SuscribeToScaled(float time, System.Action action) 
            => Suscribe(tickGroups, groupsByTime, time, action);
        public void RemoveFromScaled(float time, System.Action action) 
            => Remove(tickGroups, groupsByTime, time, action);
        public void SuscribeToUnscaled(float time, System.Action action)
            => Suscribe(uTickGroups, groupsByUtime, time, action);
        public void RemoveFromUnscaled(float time, System.Action action)
            => Remove(uTickGroups, groupsByUtime, time, action);
        public void SuscribeToLateScaled(float time, System.Action action)
            => Suscribe(tickGroups, groupsByTime, time, action, true);
        public void RemoveFromLateScaled(float time, System.Action action)
            => Remove(tickGroups, groupsByTime, time, action, true);
        public void SuscribeToLateUnscaled(float time, System.Action action)
            => Suscribe(uTickGroups, groupsByUtime, time, action, true);
        public void RemoveFromLateUnscaled(float time, System.Action action)
            => Remove(uTickGroups, groupsByUtime, time, action, true);
    }

    class TickGroup
    {
        public float time, timer;
        public List<System.Action> actions;
        public List<System.Action> lateActions;
        bool tickReady;

        public TickGroup(float time)
        {
            this.time = time;
            timer = 0;
            actions = new List<System.Action>();
            lateActions = new List<System.Action>();
        }
        
        public void Update(float dt)
        {
            timer += dt;
            if (timer >= time)
            {
                tickReady = true;
                for (int i = 0; i < actions.Count; i++) 
                    actions[i]();
            }
        }
        public void LateUpdate()
        {
            if (tickReady)
            {
                timer %= timer;
                for (int i = 0; i < lateActions.Count; i++)
                    lateActions[i]();
            }
        }
        public void Add(System.Action action, bool isLate)
        {
            if(isLate)
                lateActions.Add(action);
            else 
                actions.Add(action);
        }
        /// Returns true if empty
        public bool Remove(System.Action action, bool isLate)
        {
            if(isLate)
                lateActions.Remove(action);
            else 
                actions.Remove(action);
            
            return lateActions.Count == 0 && actions.Count == 0;
        }
    }
}