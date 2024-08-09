using System;
using Object = UnityEngine.Object;

namespace Arknights.Tools
{
    [Serializable]
    public class Injection
    {
        public string name;
        public Object value;
    }
}
