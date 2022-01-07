namespace Projapocsur
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.World;

    public class BooleanPropTrackerManager
    {
        private Dictionary<string, Func<Character, Prop<bool>>> characterPropHandler = new Dictionary<string, Func<Character, Prop<bool>>>();
        private Dictionary<string, BooleanPropTracker> trackers = new Dictionary<string, BooleanPropTracker>();

        public IReadOnlyDictionary<string, BooleanPropTracker> Trackers => this.trackers;

        public void AddNew(string name, bool initialValue, Func<Character, Prop<bool>> trackedProp)
        {
            this.trackers[name] = new BooleanPropTracker(initialValue);
            this.characterPropHandler[name] = trackedProp;
        }

        public void Track(Character character)
        {
            foreach (var handler in this.characterPropHandler)
            {
                this.trackers[handler.Key].Track(handler.Value(character));
            }
        }

        public void Untrack(Character character)
        {
            foreach (var handler in this.characterPropHandler)
            {
                this.trackers[handler.Key].Untrack(handler.Value(character));
            }
        }

        public void OnSelect(Character character)
        {
            foreach (var handler in this.characterPropHandler)
            {
                if (this.trackers[handler.Key].Latest.Value)
                {
                    this.trackers[handler.Key].Latest.Value = handler.Value(character).Value;
                }
            }
        }
    }
}
