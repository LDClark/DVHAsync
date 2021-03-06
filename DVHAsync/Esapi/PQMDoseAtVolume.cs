﻿using System.Text.RegularExpressions;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace DVHAsync
{
    class PQMDoseAtVolume
    {
        public static string GetDoseAtVolume(StructureSet structureSet, PlanningItemViewModel planningItem, StructureViewModel evalStructure, MatchCollection testMatch, Group evalunit)
        {
            //check for sufficient dose and sampling coverage
            DVHData dvh = planningItem.PlanningItemObject.GetDVHCumulativeData(evalStructure.Structure, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);
            if (dvh != null)
            {
                if ((dvh.SamplingCoverage < 0.9) || (dvh.Coverage < 0.9))
                    return "Unable to calculate - insufficient dose or sampling coverage";
                Group eval = testMatch[0].Groups["evalpt"];
                Group unit = testMatch[0].Groups["unit"];
                DoseValue.DoseUnit du = (unit.Value.CompareTo("%") == 0) ? DoseValue.DoseUnit.Percent :
                        (unit.Value.CompareTo("cGy") == 0) ? DoseValue.DoseUnit.cGy : DoseValue.DoseUnit.Unknown;
                VolumePresentation vp = (unit.Value.CompareTo("%") == 0) ? VolumePresentation.Relative : VolumePresentation.AbsoluteCm3;
                DoseValue dv = new DoseValue(double.Parse(eval.Value), du);
                double volume = double.Parse(eval.Value);
                VolumePresentation vpFinal = (evalunit.Value.CompareTo("%") == 0) ? VolumePresentation.Relative : VolumePresentation.AbsoluteCm3;
                DoseValuePresentation dvpFinal = (evalunit.Value.CompareTo("%") == 0) ? DoseValuePresentation.Relative : DoseValuePresentation.Absolute;
                DoseValue dvAchieved = planningItem.PlanningItemObject.GetDoseAtVolume(evalStructure.Structure, volume, vp, dvpFinal);
                //checking dose output unit and adapting to template
                if (dvAchieved.UnitAsString.CompareTo(evalunit.Value.ToString()) != 0)
                {
                    if ((evalunit.Value.CompareTo("cGy") == 0) && (dvAchieved.Unit.CompareTo(DoseValue.DoseUnit.cGy) == 0))
                        dvAchieved = new DoseValue(dvAchieved.Dose / 100, DoseValue.DoseUnit.cGy);
                    else
                        return "Unable to calculate";
                }

                return dvAchieved.ToString();
            }
            else
            {
                return "Unable to calculate - structure is empty";
            }
        }
    }
}
