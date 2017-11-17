using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Interaction Notes:
//1) Main control script should interact with this script by setting PaintFactory.ApplyToTrailRenderer when a new trail renderer is spawned.

//TODO: Write new surface shader to use vertex colors.  Apply material color to lineRenderer (vertex colors), use vertex colors in surface shader.  This should allow for color mixing without the need to track color as a material property.
public class PaintFactory : MonoBehaviour {
    public List<Color> colors;          //All available colors
    public List<Material> materials;    //All available materials
    public List<Texture> textures;      //All available textures
    
    public float node_spacing = 0.5f;            //Distance between individual nodes
    public float node_axis_distance = 0.25f;     //Distance of nodes from 'central' axis of world_anchor
    
    public Axis color_axis;
    public Axis material_axis;
    public Axis texture_axis;
    
    //Static access
    private static Color active_color;                  //Selection Nodes set these values
    private static Material active_material;
    private static Texture active_texture;

    private static PaintFactory self;                    //This instance

    public static Paint active_paint { get; private set; }
    
    private static List<Paint> paints = new List<Paint>();      //Track paint combinations to avoid excessive instancing of duplicate materials

    [System.Serializable]
    public class Axis
    {
        public float angle;
        public float offset;
        public GameObject prefab;
    }

    public class Paint
    {
        public Color color;         
        public Material material;   //Base material properties
        public Texture texture;     //Overrides material albedo
        
        public Paint(Color color, Material material, Texture texture)
        {
            this.color = color;
            this.material = new Material(material);
            this.texture = texture;

            this.material.SetTexture("_MainTex", texture);
            this.material.SetColor("_Color", color);
        }

        public override string ToString()
        {
            string ret = "Paint: " + color.ToString() + ", material=";
            ret += material == null ? "null" : material.name;
            ret += ", texture=";
            ret += texture == null ? "null" : texture.name;
            return ret;
        }
    }
    
    private static Paint CreatePaint(Color color, Material material, Texture texture)
    {
        Paint paint = paints.FirstOrDefault(p => p.color == color && p.material == material && p.texture == texture);
        if(paint == null)
        {
            paint = new Paint(color, material, texture);
            paints.Add(paint);
        }
        
        return paint;
    }
    
    public static void ApplyPaintToTrailRenderer(TrailRenderer trailRenderer) {
        if(trailRenderer != null)
        {
            trailRenderer.material = active_paint.material;
        }
    }

    public static void ChangeActivePaintColor(Color color)
    {
        active_paint = CreatePaint(color, active_paint.material, active_paint.texture);
    }

    public static void ChangeActivePaintMaterial(Material material)
    {
        active_paint = CreatePaint(active_paint.color, material, active_paint.texture);
    }

    public static void ChangeActivePaintTexture(Texture texture)
    {
        active_paint = CreatePaint(active_paint.color, active_paint.material, texture);
    }
    
    private void Start()
    {
        self = this;
        
        active_color = colors[0];
        active_material = materials[0];
        active_texture = textures[0];

        active_paint = CreatePaint(active_color, active_material, active_texture);

        GameObject world_anchor = HandTracking.paletteHand;

        Vector3 dir = Quaternion.AngleAxis(color_axis.angle, world_anchor.transform.forward) * world_anchor.transform.up;
        for(int i = 0; i < colors.Count; i++)
        {
            Vector3 pos = dir * self.node_axis_distance + world_anchor.transform.forward * self.node_spacing * (float)i - world_anchor.transform.forward * color_axis.offset * self.node_spacing;
            InstantiateColorNode(colors[i], pos);
        }

        dir = Quaternion.AngleAxis(material_axis.angle, world_anchor.transform.forward) * world_anchor.transform.up;
        for (int i = 0; i < materials.Count; i++)
        {
            Vector3 pos = dir * self.node_axis_distance + world_anchor.transform.forward * self.node_spacing * (float)i - world_anchor.transform.forward * material_axis.offset * self.node_spacing;
            InstantiateMaterialNode(materials[i], pos);
        }

        dir = Quaternion.AngleAxis(texture_axis.angle, world_anchor.transform.forward) * world_anchor.transform.up;
        for (int i = 0; i < textures.Count; i++)
        {
            Vector3 pos = dir * self.node_axis_distance + world_anchor.transform.forward * self.node_spacing * (float)i - world_anchor.transform.forward * texture_axis.offset * self.node_spacing;
            InstantiateTextureNode(textures[i], pos);
        }

        SetBrushTipMaterialToActivePaint();
    }

    private static PaintSelectionNode InstantiateColorNode(Color color, Vector3 local_position)
    {
        ColorSelectionNode node = GameObject.Instantiate(self.color_axis.prefab, HandTracking.paletteHand.transform).GetComponent<ColorSelectionNode>();
        node.color = color;
        node.CreateDisplayMaterial();

        node.transform.localPosition = local_position;

        return node;
    }

    private static PaintSelectionNode InstantiateMaterialNode(Material material, Vector3 local_position)
    {
        MaterialSelectionNode node = GameObject.Instantiate(self.material_axis.prefab, HandTracking.paletteHand.transform).GetComponent<MaterialSelectionNode>();
        node.material = material;
        node.CreateDisplayMaterial();

        node.transform.localPosition = local_position;

        return node;
    }

    private static PaintSelectionNode InstantiateTextureNode(Texture texture, Vector3 local_position)
    {
        TextureSelectionNode node = GameObject.Instantiate(self.texture_axis.prefab, HandTracking.paletteHand.transform).GetComponent<TextureSelectionNode>();
        node.texture = texture;
        node.CreateDisplayMaterial();

        node.transform.localPosition = local_position;

        return node;
    }

    public static void SetBrushTipMaterialToActivePaint()
    {
        if (HandTracking.brushTip != null)
        {
            MeshRenderer renderer = HandTracking.brushTip.GetComponent<MeshRenderer>();
            renderer.material = active_paint.material;
        }
    }
    

}
