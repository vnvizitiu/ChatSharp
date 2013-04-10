using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp
{
    public class MaskCollection : IEnumerable<string>
    {
        internal MaskCollection()
        {
            Masks = new List<string>();
        }

        private List<string> Masks { get; set; }

        internal void Add(string mask)
        {
            Masks.Add(mask);
        }

        internal void Remove(string mask)
        {
            Masks.Remove(mask);
        }

        public bool Contains(string mask)
        {
            return Masks.Contains(mask);
        }

        public string this[int index]
        {
            get
            {
                return Masks[index];
            }
        }

        public bool ContainsMatch(IrcUser user)
        {
            return Masks.Any(m => user.Match(m));
        }

        public string GetMatch(IrcUser user)
        {
            var match = Masks.FirstOrDefault(m => user.Match(m));
            if (match == null)
                throw new KeyNotFoundException("No mask matches the specified user.");
            return match;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Masks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
