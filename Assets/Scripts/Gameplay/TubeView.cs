using UnityEngine;
using System.Collections.Generic;

//this class controls the view displayed
public class TubeView : MonoBehaviour
{
    [SerializeField] private GameObject _tubeCap;
    public Transform layerContainer;
    public GameObject liquidLayerPrefab;
    private int _tubeIndex;
    public TubeModel Model { get; private set; }
    private GameController _controller;
    private float layerHeight = 0.5f;
   
    //bind view to model data
    public void Initialize(TubeModel model, GameController controller, int index)
    {
        Model = model;
        _controller = controller;
        _tubeIndex = index;
        Refresh();
    }

  
    //destroy all previous color to turn reload new tube data
    public void Refresh()
    {
        foreach (Transform child in layerContainer)
        {
            Destroy(child.gameObject);
        }

        List<ColorType> layers = Model.GetLayers();

        for (int i = 0; i < layers.Count; ++i)
        {
            GameObject layer = Instantiate(liquidLayerPrefab, layerContainer);

            layer.transform.localPosition =
                new Vector3(0, i * layerHeight, 0);

            layer.GetComponent<SpriteRenderer>().color =
                ConvertColor(layers[i]);
        }
        SetSelected(false);
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
        Debug.Log("clicked on tube " + _tubeIndex);
        _controller.OnTubeClicked(_tubeIndex);
    }

    public void SetSelected(bool isSelected)
    {
        transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
    }
}
