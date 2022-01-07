namespace Projapocsur.Engine
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class ObjectPool
    {
        private Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();

        public void AssureMinimumPoolSizeForPath(string path, int size)
        {
            ValidationUtility.ThrowIfStringEmptyNullOrWhitespace(nameof(path), path);

            int poolSize = 0;

            if (pool.TryGetValue(path, out List<GameObject> pooledObjects))
            {
                poolSize = pooledObjects.Count;
            }

            if (poolSize < size)
            {
                this.Instantiate(path, count: size);
            }
        } 

        /// <summary>
        /// Returns an available pooled object, otherwise it instantiates a new one into the pool and returns it.
        /// </summary>
        /// <param name="path"> The resource path where the object prefab can be found.</param>
        /// <returns> The gameobject.</returns>
        public GameObject GetOrInstantiate(string path)
        {
            ValidationUtility.ThrowIfStringEmptyNullOrWhitespace(nameof(path), path);

            GameObject objectToReturn = null;

            if (this.pool.TryGetValue(path, out List<GameObject> pooledObjects))
            {
                foreach (GameObject gameObject in pooledObjects)
                {
                    if (!gameObject.activeInHierarchy)
                    {
                        objectToReturn = gameObject;
                        break;
                    }
                }
            }

            objectToReturn ??= this.Instantiate(path).FirstOrDefault();
            objectToReturn?.SetActive(true);

            return objectToReturn;
        }

        private List<GameObject> Instantiate(string path, int count = 5)
        {
            ValidationUtility.ThrowIfStringEmptyNullOrWhitespace(nameof(path), path);

            GameObject sourceObject;

            if (this.pool.TryGetValue(path, out List<GameObject> pooledObjects))
            {
                sourceObject = pooledObjects.First();
            }
            else
            {
                sourceObject = Resources.Load(path) as GameObject;
                pooledObjects = new List<GameObject>(count);
                this.pool[path] = pooledObjects;
            }

            var newObjects = new List<GameObject>(count);

            if (sourceObject != null)
            {
                for(int currentCount = 0; currentCount < count; currentCount++)
                {
                    var newObject = Object.Instantiate(sourceObject);
                    newObject.SetActive(false);
                    newObjects.Add(newObject);
                }

                pooledObjects.AddRange(newObjects);
            }

            return newObjects;
        }
    }
}
