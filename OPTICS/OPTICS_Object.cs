using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OPTICS.Clustering.Core
{
    public class OPTICS_Object<T> : IComparable<OPTICS_Object<T>>
    {
        public T Element { get; }

        public bool Processed { get; set; }

        public double? CoreDistance { get; set; }

        public double? ReachabilityDistance { get; set; }

        public OPTICS_Object(T obj) => Element = obj;

        int IComparable<OPTICS_Object<T>>.CompareTo(OPTICS_Object<T> other)
        {
            if (ReferenceEquals(Element, other.Element)) return 0;
            if (ReachabilityDistance == null) return 1;
            if (other.ReachabilityDistance == null) return -1;
            return (int)ReachabilityDistance.Value - (int)other.ReachabilityDistance.Value;
        }

        //  public static implicit operator OPTICS_Object<T>(T point) => new OPTICS_Object<T>(point);

        //  public static implicit operator T(OPTICS_Object<T> obj) => obj.Element;
    }

    /*public class ReachabilityDistComparer<T> : IComparer<OPTICS_Object<T>>
    {
        public static ReachabilityDistComparer<T> Instance = new ReachabilityDistComparer<T>();

        int IComparer<OPTICS_Object<T>>.Compare(OPTICS_Object<T> x, OPTICS_Object<T> y)
        {
            if (ReferenceEquals(x.Element, y.Element)) return 0;
            if (x.ReachabilityDistance == null && y.ReachabilityDistance == null) return 1;
            if (x.ReachabilityDistance == null) return 1;
            if (y.ReachabilityDistance == null) return -1;
            return x.ReachabilityDistance.Value - y.ReachabilityDistance.Value > 0 ? 1 : -1;
        }
    }*/
}
