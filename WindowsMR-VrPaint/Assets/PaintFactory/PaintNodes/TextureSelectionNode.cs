using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TextureSelectionNode : PaintSelectionNode {

    [HideInInspector] public Texture texture;
    
    protected override void OnSelected()
    {
        PaintFactory.ChangeActivePaintTexture(texture);
        PaintFactory.SetBrushTipMaterialToActivePaint();
    }

    public override void CreateDisplayMaterial()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material display_material = new Material(renderer.sharedMaterial);
        display_material.SetTexture("_MainTex", texture);
        renderer.material = display_material;

    }
}
