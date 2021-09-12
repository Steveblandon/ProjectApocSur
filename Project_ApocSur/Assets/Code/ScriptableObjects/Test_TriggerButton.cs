namespace Projapocsur.ScriptableObjects
{
    using Projapocsur.Common;
    using Projapocsur.CustomAttributes;
    using Projapocsur.Entities.Definitions;
    using UnityEngine;

    [CreateAssetMenu]
    public class Test_TriggerButton : ScriptableObject
    {
        [Button(nameof(TriggeredAction), ButtonWidth = 300)]
        [SerializeField]
        private bool triggerAction;

        protected void TriggeredAction()
        {
            /*DefinitionIndex<Def>.Defs data = new DefinitionIndex<Def>.Defs();*/

            //StorageUtility.SaveData(defs, "StatDefs.xml");

            /*StorageUtility.LoadDataFromDirectory(out data, "Definitions");

            if (data == null)
            {
                Debug.LogError($"{data.GetType()}:{nameof(data)} null");
            }*/

            //int count = data.statDefs.Count + data.bodyDefs.Count;

            /*DefinitionIndex<StatDef>.Add(data.statDefs);
            DefinitionIndex<StatDef>.TryFind("HitPoints", out StatDef def);*/
            DefinitionFinder.Init(true);
            DefinitionFinder.TryFind("HitPoints", out StatDef def);
            Debug.Log($"{def?.Name}: {def?.Description} found");
            DefinitionFinder.TryFind("Human", out BodyDef bodyDef);
            Debug.Log($"{bodyDef?.Name}: {bodyDef?.MaxHitPointsBase} found");
            DefinitionFinder.TryFind("Torso", out BodyPartDef bodyPartDef);
            Debug.Log($"{bodyPartDef?.Name}: {bodyPartDef?.MaxHitpointsBase} found");
            DefinitionFinder.TryFind("Laceration", out InjuryDef injuryDef);
            Debug.Log($"{injuryDef?.Name}: {injuryDef?.Description} found");
        }
    }
}
