using UnityEngine;
using System.Collections.Generic;

public class TubeView : MonoBehaviour
{
    public Transform layerContainer;
    public GameObject liquidLayerPrefab;
    public int index;
    private float layerHeight = 0.5f;
    public GameController controller;

    private TubeModel _model;

    //bind view to model data
    public void Initialize(TubeModel model)
    {
       _model = model;
        Refresh();
    }

  
    public void Refresh()
    {
        foreach (Transform child in layerContainer)
        {
            Destroy(child.gameObject);
        }

        List<ColorType> layers = _model.GetLayers();

        for (int i = 0; i < layers.Count; i++)
        {
            GameObject layer = Instantiate(liquidLayerPrefab, layerContainer);

            layer.transform.localPosition =
                new Vector3(0, i * layerHeight, 0);

            layer.GetComponent<SpriteRenderer>().color =
                ConvertColor(layers[i]);
        }
    }

    private Color ConvertColor(ColorType type)
    {
        return type switch
        {
            ColorType.Red => Color.red,
            ColorType.Blue => Color.blue,
            ColorType.Yellow => Color.yellow,
            ColorType.Green => Color.green,
            ColorType.Purple => Color.magenta,
            _ => Color.white
        };
    }

    private void OnMouseDown()
    {
        controller.OnTubeClicked(index);
    }
}
