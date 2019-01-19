using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[CustomGridBrush(true, false, false, "Bold Brush")]
public class BoldBrush : GridBrush {
	public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				var location = position;
				location.x += x; location.y += y;
				base.Paint(gridLayout, brushTarget, location);
			}
		}
	}
	public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				var location = position;
				location.x += x; location.y += y;
				base.Erase(gridLayout, brushTarget, location);
			}
		}
	}
}

[CustomEditor(typeof(BoldBrush))]
public class BoldBrushEditor : GridBrushEditor
{
	public override void PaintPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
	{
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				var location = new Vector3Int(position.x + x, position.y + y, 0);
				base.PaintPreview(gridLayout, brushTarget, location);
			}
		}
	}
	public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
	{
		base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
		if (brushTarget != null) {
			var tilemap = brushTarget.GetComponent<Tilemap>();
			tilemap.ClearAllEditorPreviewTiles();
		}

		PaintPreview(gridLayout, brushTarget, new Vector3Int(position.x, position.y, position.z));

        var min = new Vector3Int( position.x - 1, position.y - 1, position.z );
        var max = new Vector3Int( position.x + 2, position.y + 2, position.z );

        var p1 = new Vector3( min.x, min.y, min.z );
        var p2 = new Vector3( max.x, min.y, min.z );
        var p3 = new Vector3( max.x, max.y, min.z );
        var p4 = new Vector3( min.x, max.y, min.z );

        Handles.DrawLine( p1, p2 );
        Handles.DrawLine( p2, p3 );
        Handles.DrawLine( p3, p4 );
        Handles.DrawLine( p4, p1 );
	}
}
