// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Pathing
{
	// algorithm based on WikiPedia: http://en.wikipedia.org/wiki/A*_search_algorithm
	//
	// implements the static GetPath(...) function that will return a IList of IAStarNodes that is the shortest path
	// between the two IAStarNodes that are passed as parameters to the function
	public static class AStar
	{
		private class OpenSorter : IComparer<IAStarNode>
		{
			private Dictionary<IAStarNode, float> fScore;

			public OpenSorter(Dictionary<IAStarNode, float> f)
			{
				fScore = f;
			}

			public int Compare(IAStarNode x, IAStarNode y)
			{
				if (x != null && y != null)
					return fScore[x].CompareTo(fScore[y]);
				else
					return 0;
			}
		}

		private static List<IAStarNode> closed;
		private static List<IAStarNode> open;
		private static Dictionary<IAStarNode, IAStarNode> cameFrom;
		private static Dictionary<IAStarNode, float> gScore;
		private static Dictionary<IAStarNode, float> hScore;
		private static Dictionary<IAStarNode, float> fScore;

		static AStar()
		{
			closed = new List<IAStarNode>();
			open = new List<IAStarNode>();
			cameFrom = new Dictionary<IAStarNode, IAStarNode>();
			gScore = new Dictionary<IAStarNode, float>();
			hScore = new Dictionary<IAStarNode, float>();
			fScore = new Dictionary<IAStarNode, float>();
		}

		// this function is the C# implementation of the algorithm presented on the wikipedia page
		// start and goal are the nodes in the graph we should find a path for
		//
		// returns null if no path is found
		//
		// this function is NOT thread-safe (due to using static data for GC optimization)
		public static IList<IAStarNode> GetPath(IAStarNode start, IAStarNode goal)
		{
			if (start == null || goal == null)
			{
				return null;
			}

			closed.Clear();
			open.Clear();
			open.Add(start);

			cameFrom.Clear();
			gScore.Clear();
			hScore.Clear();
			fScore.Clear();

			gScore.Add(start, 0f);
			hScore.Add(start, start.EstimatedCostTo(goal));
			fScore.Add(start, hScore[start]);

			OpenSorter sorter = new OpenSorter(fScore);
			IAStarNode current,
						from = null;
			float tentativeGScore;
			bool tentativeIsBetter;

			while (open.Count > 0)
			{
				current = open[0];
				if (current == goal)
				{
					return ReconstructPath(new List<IAStarNode>(), cameFrom, goal);
				}
				open.Remove(current);
				closed.Add(current);

				if (current != start)
				{
					from = cameFrom[current];
				}
				foreach (IAStarNode next in current.Neighbours)
				{
					if (from != next && !closed.Contains(next))
					{
						tentativeGScore = gScore[current] + current.CostTo(next);
						tentativeIsBetter = true;

						if (!open.Contains(next))
						{
							open.Add(next);
						}
						else
						if (tentativeGScore >= gScore[next])
						{
							tentativeIsBetter = false;
						}

						if (tentativeIsBetter)
						{
							cameFrom[next] = current;
							gScore[next] = tentativeGScore;
							hScore[next] = next.EstimatedCostTo(goal);
							fScore[next] = gScore[next] + hScore[next];
						}
					}
				}
				open.Sort(sorter);
			}
			return null;
		}

		private static IList<IAStarNode> ReconstructPath(IList<IAStarNode> path, Dictionary<IAStarNode, IAStarNode> cameFrom, IAStarNode currentNode)
		{
			if (cameFrom.ContainsKey(currentNode))
			{
				ReconstructPath(path, cameFrom, cameFrom[currentNode]);
			}
			path.Add(currentNode);
			return path;
		}
	}
}