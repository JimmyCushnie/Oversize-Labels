using PiTung;
using PiTung.Components;
using System;
using UnityEngine;

public class OversizeLabels : Mod
{
    public override string Name => "Oversize Labels";
    public override string PackageName => "me.jimmy.StretchedLabels";
    public override string Author => "Iamsodarncool";
    public override Version ModVersion => new Version("1.0");

    public override void BeforePatch()
    {
        ComponentRegistry.CreateNew<CustomLabel>("WidePanelLabel", "Wide Panel Label", CreatePanelLabelOfSize(3, 1));
        ComponentRegistry.CreateNew<CustomLabel>("TallPanelLabel", "Tall Panel Label", CreatePanelLabelOfSize(1, 3));
        ComponentRegistry.CreateNew<CustomLabel>("BigPanelLabel", "Big Panel Label", CreatePanelLabelOfSize(2, 2));

        // the world is not ready
        // ComponentRegistry.CreateNew<CustomLabel>("CollosallyWidePanelLabel", "Collosally Wide Panel Label", PanelLabelOfSize(51, 1));
        // ComponentRegistry.CreateNew<CustomLabel>("CollosallyTallPanelLabel", "Collosally Tall Panel Label", PanelLabelOfSize(1, 51));
        // ComponentRegistry.CreateNew<CustomLabel>("TitanicPanelLabel", "Titanic Panel Label", PanelLabelOfSize(400, 700));
    }

    private static CustomBuilder CreatePanelLabelOfSize(int x, int z)
    {
        return PrefabBuilder
            .Custom(() =>
            {
                var obj = new GameObject();
                var label = UnityEngine.Object.Instantiate(References.Prefabs.PanelLabel, obj.transform);
                label.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(x, z);

                // remove the existing white block of the label, since we're replacing it with a bigger one
                MegaMeshManager.RemoveComponentsImmediatelyIn(label);
                UnityEngine.Object.Destroy(label.GetComponent<MegaMeshComponent>());
                UnityEngine.Object.Destroy(label.GetComponent<Renderer>());
                UnityEngine.Object.Destroy(label.GetComponent<MeshFilter>());

                // create the new geometry
                var geometry = UnityEngine.Object.Instantiate(References.Prefabs.WhiteCube, obj.transform);
                geometry.transform.localScale = new Vector3(x * 0.3f, 0.1f, z * 0.3f);

                // make sure the collider of the label is big enough, so you can click on all parts of it
                label.GetComponent<BoxCollider>().size = new Vector3(x, 1, z);

                // ...and get rid of the collider of the geometry, so it doesn't interfere with clicking on the label.
                // Destorying it causes bugs for some reason so I just do this ¯\_(ツ)_/¯
                geometry.GetComponent<BoxCollider>().size = Vector3.zero;

                // if it is an even number high, we have to shift everything in the component so that it still lines up with the grid
                if (z % 2 == 0)
                {
                    for (int i = 0; i < obj.transform.childCount; i++)
                    {
                        obj.transform.GetChild(i).transform.localPosition += new Vector3(0, 0, 0.15f);
                    }
                }
                // ditto if it is an even number wide
                if (x % 2 == 0)
                {
                    for (int i = 0; i < obj.transform.childCount; i++)
                    {
                        obj.transform.GetChild(i).transform.localPosition += new Vector3(0.15f, 0, 0);
                    }
                }

                return obj;
            });
    }
}