﻿// -----------------------------------------------------------------------
// <copyright file="SoilState.cs" company="APSIM Initiative">
//     Copyright (c) APSIM Initiative
// </copyright>
//-----------------------------------------------------------------------
namespace Models.Soils.Arbitrator
{
    using System;
    using System.Collections.Generic;
    using Models.Core;

    /// <summary>
    /// Encapsulates the state of water and N in multiple zones.
    /// </summary>
    public class SoilState
    {
        /// <summary>The parent model.</summary>
        private IModel Parent;

        /// <summary>Initializes a new instance of the <see cref="SoilState"/> class.</summary>
        /// <param name="parent">The parent model.</param>
        public SoilState(IModel parent)
        {
            Parent = parent;
            Zones = new List<ZoneWaterAndN>();
        }

        /// <summary>Initialises this instance.</summary>
        public void Initialise()
        {
            foreach (Zone Z in Apsim.Children(this.Parent, typeof(Zone)))
            {
                ZoneWaterAndN NewZ = new ZoneWaterAndN();
                NewZ.Name = Z.Name;
                Soil soil = Apsim.Child(Z, typeof(Soil)) as Soil;
                NewZ.Water = soil.Water;
                NewZ.NO3N = soil.NO3N;
                NewZ.NH4N = soil.NH4N;
                Zones.Add(NewZ);
            }
        }

        /// <summary>Gets all zones in this soil state.</summary>
        public List<ZoneWaterAndN> Zones { get; private set; }

        /// <summary>Implements the operator -.</summary>
        /// <param name="state">The soil state.</param>
        /// <param name="estimate">The estimate to subtract from the soil state.</param>
        /// <returns>The result of the operator.</returns>
        public static SoilState operator -(SoilState state, Estimate estimate)
        {
            SoilState NewState = new SoilState(state.Parent);
            foreach (ZoneWaterAndN Z in state.Zones)
            {
                ZoneWaterAndN NewZ = new ZoneWaterAndN();
                NewZ.Name = Z.Name;
                NewZ.Water = Z.Water;
                NewZ.NO3N = Z.NO3N;
                NewZ.NH4N = Z.NH4N;
                NewState.Zones.Add(NewZ);
            }

            foreach (CropUptakes C in estimate.Values)
                foreach (ZoneWaterAndN Z in C.Zones)
                    foreach (ZoneWaterAndN NewZ in NewState.Zones)
                        if (Z.Name == NewZ.Name)
                        {
                            NewZ.Water = Utility.Math.Subtract(NewZ.Water, Z.Water);
                            NewZ.NO3N = Utility.Math.Subtract(NewZ.NO3N, Z.NO3N);
                            NewZ.NH4N = Utility.Math.Subtract(NewZ.NH4N, Z.NH4N);
                        }
            return NewState;
        }
    }
}
