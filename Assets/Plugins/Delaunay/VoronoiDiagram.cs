using System;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public class VoronoiDiagram {

			
		/// <summary>
		/// The voronoi diagram is calculated from a Delaunay triangulation.
		/// This is a reference to the "source" triangulation, in case you need
		/// it again. 
		/// </summary>
		public readonly DelaunayTriangulation Triangulation;
		
		/// <summary>
		/// The sites of the voronoi triangulation.
		/// </summary>
		public readonly List<Vector2> Sites;
		
		/// <summary>
		///
		/// </summary>
		public readonly List<Vector2> Vertices;
		
		/// <summary>
		///
		/// </summary>
		public readonly List<Edge> Edges;

		internal VoronoiDiagram() {
			Triangulation = new DelaunayTriangulation();
			Sites = Triangulation.Vertices;
			Vertices = new List<Vector2>();
			Edges = new List<Edge>();
		}

		internal void Clear() {
			Triangulation.Clear();
			Sites.Clear();
			Vertices.Clear();
			Edges.Clear();
		}

		public void GetSiteEdgeRange(int site, out int firstEdge, out int lastEdge) {
			if (Edges.Count == 0) {
				throw new InvalidOperationException("VoronoiDiagram is empty");
			}

			if (site < 0 || site >= Sites.Count) {
				throw new ArgumentOutOfRangeException("site");
			}

			var foundSite = false;

			firstEdge = lastEdge = -1;

			for (int i = 0; i < Edges.Count; i++) {
				var edge = Edges[i];
				var rightSite = edge.Site == site;

				if (!foundSite && rightSite) {
					foundSite = true;
					firstEdge = i;
				} else if (foundSite && !rightSite) {
					lastEdge = i - 1;

#if UNITY_ASSERTIONS	
					for (int j = i + 1; j < Edges.Count; j++) {
						Debug.Assert(Edges[i].Site != site);
					}
#endif
					break;
				}
			}

			if (firstEdge > lastEdge) {
				Debug.Assert(lastEdge == -1);
				Debug.Assert(site == Sites.Count - 1);
				lastEdge = Edges.Count - 1;
			}
		}

		/// <summary>
		/// Enum representing a type of voronoi edge. A "line" is an infinite line
		/// in both directions (only valid for Voronoi diagrams with 2 vertices
		/// or ones with all collinear points), a "ray" is a voronoi edge
		/// starting at a given vertex and extending infinitely in one direction,
		/// a "segment" is a regular line segment.
		/// <summary>
		public enum EdgeType {
			Line,
			Ray,
			Segment
		}

		/// <summary>
		/// An edge in the voronoi diagram.
		/// </summary>
		public struct Edge {

			/// <summary>
			/// The type of edge, line, ray or segment.
			/// </summary>
			readonly public EdgeType Type;

			/// <summary>
			/// The site associted with the edge (indexed to the "Sites" array
			/// in the parent VoronoiDiagram object)
			/// </summary>
			readonly public int Site;

			/// <summary>
			/// The first vertex of the voronoi edge. 
			///
			/// If the edge is a line, it's a point on that line. 
			/// If the edge is a ray, it's the point where the ray originates. 
			/// If the edge is a line segment, it's one of the two endpoints. 
			/// </summary>
			readonly public int Vert0;

			/// <summary>
			/// The second vertex of the voronoi edge. 
			///
			/// Only defined for segment edge types, otherwise equal to -1
			/// </summary>
			readonly public int Vert1;

			/// <summary>
			/// The direction vector of a line or ray segment. Not normalized. 
			///
			/// For segment edge types this is equal to a vector with both
			/// components equal to NaN. 
			/// </summary>
			public Vector2 Direction;
			
			/// <summary>
			/// Construct the diagram. 
			/// </summary>
			public Edge(EdgeType type, int site, int vert0, int vert1, Vector2 direction) {
				this.Type = type;
				this.Site = site;
				this.Vert0 = vert0;
				this.Vert1 = vert1;
				this.Direction = direction;
			}

			/// <summary>
			/// For debugging purposes. 
			/// </summary>
			public override string ToString() {
				if (Type == EdgeType.Segment) {
					return string.Format("VoronoiEdge(Segment, {0}, {1}, {2})",
							Site, Vert0, Vert1);
				} else if (Type == EdgeType.Segment) {
					return string.Format("VoronoiEdge(Line, {0}, {1}, {2})",
							Site, Vert0, Direction);
				} else {
					return string.Format("VoronoiEdge(Ray, {0}, {1}, ({2}, {3}))",
							Site, Vert0, Direction.x, Direction.y);
				}
			}
		}
	}
}
