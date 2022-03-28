using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeExport
{
    public abstract class Exporter
    {
        public MeshFilter filter;

        public Exporter(MeshFilter filter)
        {
            this.filter = filter;
        }

        public abstract string Export();
    }
}
