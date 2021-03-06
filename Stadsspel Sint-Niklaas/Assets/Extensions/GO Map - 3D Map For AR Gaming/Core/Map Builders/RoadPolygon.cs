using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoMap
{

	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class RoadPolygon : MonoBehaviour
	{
		public List<Vector3> _verts;

		public void Initialize(List<Vector3> verts, string kind, Layer layer, int sort)
		{
			_verts = verts;

			RenderingOptions renderingOptions = null;
			foreach (RenderingOptions r in layer.renderingOptions) {
				if (r.kind == kind) {
					renderingOptions = r;
					break;
				}
			}
			float width = layer.defaultRendering.lineWidth;
			float defaultZ = -layer.defaultRendering.distanceFromFloor;
			Material material = layer.defaultRendering.material;
			Material outlineMaterial = layer.defaultRendering.outlineMaterial;
			float outlineWidth = width + layer.defaultRendering.outlineWidth;
			if (renderingOptions != null) {
				width = renderingOptions.lineWidth;
				material = renderingOptions.material;
				defaultZ = renderingOptions.distanceFromFloor;
				outlineMaterial = renderingOptions.outlineMaterial;
				outlineWidth = width + renderingOptions.outlineWidth;
			}

			if (defaultZ == 0f)
				defaultZ = -sort / 1000.0f;

			if (material)
				material.renderQueue = -sort;
			if (outlineMaterial)
				outlineMaterial.renderQueue = -sort;

			SimpleRoad road = gameObject.AddComponent<SimpleRoad>();

			Vector3[] vertices = _verts.Select(x => new Vector3(x.x, x.y, 0)).ToArray();
			road.verts = vertices;
			road.width = width;

			road.load();

			Vector3 position = transform.position;
			position.z = defaultZ;
			transform.position = position;

			gameObject.GetComponent<Renderer>().material = material;

			if (outlineMaterial != null) {
				CreateRoadOutline(verts, outlineMaterial, outlineWidth, sort, defaultZ);
			}

		}

		GameObject CreateRoadOutline(List<Vector3> verts, Material material, float width, int sort, float y)
		{

			GameObject outline = new GameObject("outline");
			outline.transform.parent = transform;

			material.renderQueue = -(sort - 1);

			SimpleRoad road = outline.AddComponent<SimpleRoad>();

			Vector3[] vertices = _verts.Select(x => new Vector3(x.x, x.y, 0)).ToArray();
			road.verts = vertices;
			road.width = width;

			road.load();

			Vector3 position = outline.transform.position;
			position.z = 0.029f;
			outline.transform.localPosition = position;

			outline.GetComponent<Renderer>().material = material;

			return outline;
		}

	}

}
