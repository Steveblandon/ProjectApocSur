namespace Projapocsur.Common
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A special stack and set composite that guarantees non-null unique values are added to stack.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StackSet<T>: IEnumerable, IEnumerable<T>
    {
        private readonly ISet<T> set;
        private readonly Stack<T> stack;

        public StackSet()
        {
            this.set = new HashSet<T>();
            this.stack = new Stack<T>();
        }

        public int Count => set.Count;

        /// <summary>
        /// Add an element to the <see cref="StackSet{T}"/>
        /// </summary>
        /// <param name="element"> The element to add.</param>
        /// <returns>true, if the element is unique in the StackSet.</returns>
        public bool Add(T element)
        {
            if (element != null && !this.set.Contains(element))
            {
                this.stack.Push(element);
                this.set.Add(element);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Preview element at the top of the <see cref="StackSet{T}"/> without removing it.
        /// </summary>
        /// <returns>the element.</returns>
        public T Peek()
        {
            return this.stack.Peek();
        }

        /// <summary>
        /// Retrieve element at the top of the <see cref="StackSet{T}"/> and remove it.
        /// </summary>
        /// <returns>the element.</returns>
        public T Pop()
        {
            T element = this.stack.Pop();
            this.set.Remove(element);
            return element;
        }

        public IEnumerator GetEnumerator()
        {
            return this.set.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.set.GetEnumerator();
        }
    }

}