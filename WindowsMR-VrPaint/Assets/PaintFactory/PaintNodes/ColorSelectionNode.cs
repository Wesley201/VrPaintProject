using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColorSelectionNode : PaintSelectionNode {

    [HideInInspector]
    public Color color;
    
    protected override void OnSelected()
    {
        PaintFactory.ChangeActivePaintColor(color);
        PaintFactory.SetBrushTipMaterialToActivePaint();
    }

    public override void CreateDisplayMaterial()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material display_material = new Material(renderer.sharedMaterial);
        display_material.SetColor("_Color", color);
        renderer.material = display_material;

    }

}
