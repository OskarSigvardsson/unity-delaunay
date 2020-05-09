/**
 * Copyright 2019 Oskar Sigvardsson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
		/// The vertices of the voronoi diagram
		/// </summary>
		public readonly List<Vector2> Vertices;
		
		/// <summary>
		/// The edges of the voronoi diagram, grouped by site and ordered
		/// counter-clockwise. 
		/// </summary>
		public readonly List<Edge> Edges;

		/// <summary>
		/// The first edge of every site. 
		/// </summary>
		public readonly List<int> FirstEdgeBySite;

		internal VoronoiDiagram() {
			Triangulation = new DelaunayTriangulation();
			Sites = Triangulation.Vertices;
			Vertices = new List<Vector2>();
			Edges = new List<Edge>();
			FirstEdgeBySite = new List<int>();
		}

		internal void Clear() {
			Triangulation.Clear();
			Sites.Clear();
			Vertices.Clear();
			Edges.Clear();
			FirstEdgeBySite.Clear();
		}

		/// <summary>
		/// Enum representing a type of voronoi edge. A "line" is an infinite line
		/// in both directions (only valid for Voronoi diagrams with 2 vertices
		/// or ones with all collinear points), a "ray" is a voronoi edge
		/// starting at a given vertex and extending infinitely in one direction,
		/// a "segment" is a regular line segment.
		///
		/// There are two different varietes of edges, clock-wise and
		/// counter-clockwise. Regular edges are all clockwise, but rays can be
		/// either, so use the type to indicate which it is. 
		/// <summary>
		public enum EdgeType {
			Line,
			RayCCW,
			RayCW,
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
			/// Construct the edge. 
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
