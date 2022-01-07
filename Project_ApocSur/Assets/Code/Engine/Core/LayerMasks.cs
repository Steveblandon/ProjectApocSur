namespace Projapocsur.Engine
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class LayerMasks
    {
        public static readonly string[] DefaultArray = new string[] { Default };
        
        private const string ClassName = nameof(LayerMasks);

        public const string Default = "Default";
        public const string TransparentFX = "TransparentFX";
        public const string IgnoreRaycast = "Ignore Raycast";
        public const string Water = "Water";
        public const string UI = "UI";
        public const string Static = "Static";

        private const string Invalid = "Invalid-rand02415599901";
        private static string[][] allLayers = new string[][] 
        { 
            new string[] { Invalid }, 
            new string[] { Default }, 
            new string[] { TransparentFX }, 
            new string[] { IgnoreRaycast }, 
            new string[] { Water },
            new string[] { UI },
            new string[] { Static } 
        };

        public static void Validate()
        {
            ISet<int> processed = new HashSet<int>(allLayers.Length);

            foreach (string[] layer in allLayers)
            {
                int mask = LayerMask.GetMask(layer);
                if (processed.Contains(mask))
                {
                    Debug.LogError($"{ClassName}: Invalid layer detected '{layer[0]}'... Make sure the layer exists in Unity editor layers");
                }
                else
                {
                    processed.Add(mask);
                }
            }
        }
    }
}