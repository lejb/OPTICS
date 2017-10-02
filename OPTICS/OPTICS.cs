using System;
using System.Collections.Generic;
using System.Linq;

namespace OPTICS.Clustering.Core
{
    using OBJ = OPTICS_Object<HighDimPoint>;
    using OBJList = List<OPTICS_Object<HighDimPoint>>;

    public class OPTICS_Runner
    {
        public OBJList ObjectSet { get; private set; }
        public OBJList OrderFile { get; private set; }
        public int Epsilon { get; }
        public int MinPoints { get; }

        private OBJList orderSeeds = new OBJList();

        public OPTICS_Runner(int epsilon, int minPts)
        {
            Epsilon = epsilon;
            MinPoints = minPts;
        }

        public OBJList Cluster(OBJList objs)
        {
            ObjectSet = objs;
            OrderFile = new OBJList();

            foreach (var obj in ObjectSet)
            {
                if (obj.Processed == false) ExpandClusterOrder(obj);
            }

            return OrderFile;
        }

        private void ExpandClusterOrder(OBJ obj)
        {
            ProcessObj(obj);

            while (orderSeeds.Count > 0)
            {
                var cur = orderSeeds.First();
                orderSeeds.Remove(cur);
                ProcessObj(cur);
            }
        }

        private void ProcessObj(OBJ obj)
        {
            obj.Processed = true;
            OrderFile.Add(obj);

            var neighbors =
            (
                from o in ObjectSet
                where o.Processed == false
                let dis = Distance.EuclideanDistance(obj.Element, o.Element)
                where dis < Epsilon
                orderby dis
                select o
            ).ToList();

            if (neighbors.Count >= MinPoints)
            {
                obj.CoreDistance = Distance.EuclideanDistance(obj.Element, neighbors[MinPoints - 1].Element);
                UpdateOrderSeeds(neighbors, obj);
            }
        }

        private void UpdateOrderSeeds(OBJList neighbors, OBJ center)
        {
            var c_dist = center.CoreDistance.Value;

            foreach (OBJ obj in neighbors)
            {
                if (obj.Processed == false)
                {
                    var new_r_dist = Math.Max(c_dist, Distance.EuclideanDistance(center.Element, obj.Element));

                    if (obj.ReachabilityDistance == null)
                    {
                        obj.ReachabilityDistance = new_r_dist;
                        orderSeeds.Add(obj);
                    }
                    else if (new_r_dist < obj.ReachabilityDistance.Value)
                    {
                        obj.ReachabilityDistance = new_r_dist;
                        orderSeeds.Sort();
                    }
                }
            }
        }
    }
}
