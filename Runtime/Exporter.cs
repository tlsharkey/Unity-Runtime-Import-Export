using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeExport
{
    public abstract class Exporter
    {
        public GameObject target;

        public Exporter(GameObject target)
        {
            this.target = target;
        }

        public abstract Dictionary<string, string> Export();
    }
}
