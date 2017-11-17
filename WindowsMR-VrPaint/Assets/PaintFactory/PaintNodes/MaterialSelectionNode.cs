using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MaterialSelectionNode : PaintSelectionNode {

    [HideInInspector]
    public Material material;
    
    protected override void OnSelected()
    {
        PaintFactory.ChangeActivePaintMaterial(material);
        PaintFactory.SetBrushTipMaterialToActivePaint();
    }

    public override void CreateDisplayMaterial()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = material;

    }
}
