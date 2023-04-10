// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Pathing
{
	// interface for classes that can be used with the A* algorithm defined in AStar.cs
	//
	// you will need to have the navigation targets in your world inherit from this interface for the algorithm to work
	public interface IAStarNode
	{
		// the neighbours property returns an enumeration of all the nodes adjacent to this node
		IEnumerable<IAStarNode> Neighbours
		{
			get;
		}

		// this function should calculate the exact cost of travelling from this node to "neighbour".
		// when the A* algorithm calls this function, the neighbour parameter is guaranteed to be one of the nodes in 'Neighbours'.
		//
		// 'cost' could be distance, time, anything countable, where smaller is considered better by the algorithm
		float CostTo(IAStarNode neighbour);

		// this function should estimate the distance to travel from this node to "goal". goal may be
		// any node in the graph, so there is no guarantee it is a direct neighbour. The better the estimation
		// the faster the AStar algorithm will find the optimal route. Be careful however, that the cost of calculating
		// this estimate doesn't outweigh any benefits for the AStar search.
		//
		// this estimation could be distance, time, anything countable, where smaller is considered better by the algorithm
		// the estimate needs to 'consistent' (also know as 'monotone')
		float EstimatedCostTo(IAStarNode goal);
	}
}