﻿// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Logistics.Objective;
using System.Collections.Generic;

namespace Itinero.Logistics.Solvers.GA
{
    /// <summary>
    /// Abstract representation of an operator to select a solution for reproduction.
    /// </summary>
    public interface ISelectionOperator<TProblem, TSolution, TObjective, TFitness>
        where TObjective : ObjectiveBase<TFitness>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Selects a new solution for reproduction.
        /// </summary>
        /// <returns></returns>
        int Select(TProblem problem, TObjective objective, Individual<TSolution, TFitness>[] population, ISet<int> exclude);
    }
}