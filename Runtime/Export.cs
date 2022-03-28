using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace RuntimeExport
{
    public class Export
    {
        private MeshFilter filter;

        public Export(MeshFilter filter)
        {
            this.filter = filter;
        }

        public Dictionary<string, string> As(Type t)
        {
            if (!t.BaseType.Equals(typeof(Exporter))) throw new System.Exception($"Cannot export as type {t.ToString()}. Must be a child of RuntimeExport.Exporter type.");
            
            ConstructorInfo c = t.GetConstructor(new[] { typeof(MeshFilter) });
            Debug.Log(c);
            Exporter exporter = c.Invoke(new object[] { this.filter }) as Exporter;
            return exporter.Export();
        }
    }
}