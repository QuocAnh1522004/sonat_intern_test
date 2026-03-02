using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Threading.Tasks;

//this class controls the view displayed
public class TubeView : MonoBehaviour
{
    [SerializeField] private GameObject _tubeCap;
    //[SerializeField] private Transform _contentRoot;
    public Transform layerContainer;
    public GameObject liquidLayerPrefab;
    private int _tubeIndex;
    public TubeModel Model { get; private set; }
    private GameController _controller;
    private float layerHeight = 0.5f;
    private Vector3 _originalPos;
    private Quaternion _originalRot;
    private List<GameObject> _layerVisuals = new();
    //[SerializeField] private GameObject _layerPrefab;

    //bind view to model data
    public void Initialize(TubeModel model, GameController controller, int index)
    {
        Model = model;
        _controller = controller;
        _tubeIndex = index;
        _originalPos = transform.position;
        _originalRot = transform.rotation;
        Refresh();
    }


    //destroy all previous color to turn reload new tube data
    public void Refresh()
    {
        if (Model.IsFullAndFilledWithOneColor())
            _tubeCap.SetActive(true);

        foreach (Transform child in layerContainer)
            Destroy(child.gameObject);

        _layerVisuals.Clear();

        List<ColorType> layers = Model.GetLayers();

        for (int i = 0; i < layers.Count; ++i)
        {
            SpawnLayerInstant(layers[i], i);
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
        if (isSelected)
        {
            transform.localScale = Vector3.one * 1.1f;
            PlaySelect();
            return;
        }
        transform.localScale = Vector3.one;
        ResetSelect();
    }

    public void PlaySelect()
    {
        transform.DOMoveY(_originalPos.y + 0.5f, 0.2f);
    }

    public void ResetSelect()
    {
        transform.DOMove(_originalPos, 0.2f);
    }

    public async Task PlayPourAnimation(TubeView target, int count)
    {
        float moveDuration = 0.3f;
        float rotateDuration = 0.2f;
        float horizontalOffset = 1.5f;
        float verticalLift = 2f;
        Vector3 start = _originalPos;
        bool isRight = target.transform.position.x > start.x;
        float offsetX = isRight ? -horizontalOffset : horizontalOffset;
        Vector3 pourPosition = new Vector3(target.transform.position.x + offsetX,
                                           target.transform.position.y + verticalLift,
                                           start.z);
        float direction = target.transform.position.x > transform.position.x ? -45f : 45f;
        //Vector3 mid = (start + target.position) / 2 + Vector3.up * 1.2f;
        //move to middle
        await transform.DOMove(pourPosition, moveDuration)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();

        //lean

        await transform.DORotate(new Vector3(0, 0, direction), rotateDuration)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();

        //pouring animation time
        //await Task.Delay(300);
        await AnimateLiquidPour(target, count);

        //return to straight 
        await transform.DORotate(_originalRot.eulerAngles, rotateDuration)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();

        //return to original position
        await transform.DOMove(start, moveDuration)
            .SetEase(Ease.InQuad)
            .AsyncWaitForCompletion();
    }

    public async Task AnimateLiquidPour(TubeView target, int count)
    {
        float durationPerLayer = 0.35f;
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < count; i++)
        {
            if (_layerVisuals.Count == 0) break;

            int topIndex = _layerVisuals.Count - 1;
            GameObject topLayer = _layerVisuals[topIndex];
            _layerVisuals.RemoveAt(topIndex);
            ColorType color = Model.PeekTop().Value;
            int targetIndex = target._layerVisuals.Count;
            GameObject newLayer = Instantiate(liquidLayerPrefab, target.layerContainer);
            newLayer.GetComponent<SpriteRenderer>().color = ConvertColor(color);
            newLayer.transform.localPosition =
                new Vector3(0, targetIndex * layerHeight, 0);
            Vector3 originalScale = newLayer.transform.localScale;
            newLayer.transform.localScale =
                new Vector3(originalScale.x, 0f, originalScale.z);
            target._layerVisuals.Add(newLayer);
            seq.Append(
                topLayer.transform
                    .DOScaleY(0f, durationPerLayer)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        Destroy(topLayer);
                    })
            );

            // GROW TARGET
            seq.Join(
                newLayer.transform
                    .DOScaleY(originalScale.y, durationPerLayer)
                    .SetEase(Ease.InOutSine)
            );
        }
        await seq.AsyncWaitForCompletion();
    }

    private GameObject GetTopLayerVisual()
    {
        if (_layerVisuals.Count == 0)
            return null;

        return _layerVisuals[_layerVisuals.Count - 1];
    }
    private async Task SpawnLayerVisual(ColorType color, int index)
    {
        GameObject layer = Instantiate(liquidLayerPrefab, layerContainer);

        layer.GetComponent<SpriteRenderer>().color = ConvertColor(color);

        layer.transform.localPosition = new Vector3(0, index * layerHeight, 0);

        // bắt đầu từ scale Y = 0
        Vector3 originalScale = layer.transform.localScale;
        layer.transform.localScale = new Vector3(originalScale.x, 0f, originalScale.z);

        _layerVisuals.Add(layer);

        // mọc từ dưới lên
        await layer.transform
            .DOScaleY(originalScale.y, 0.25f)
            .SetEase(Ease.OutSine)
            .AsyncWaitForCompletion();
    }

    private void RemoveTopLayerVisual()
    {
        if (_layerVisuals.Count == 0)
            return;

        GameObject top = _layerVisuals[_layerVisuals.Count - 1];

        _layerVisuals.RemoveAt(_layerVisuals.Count - 1);

        Destroy(top);
    }

    private void SpawnLayerInstant(ColorType color, int index)
    {
        GameObject layer = Instantiate(liquidLayerPrefab, layerContainer);

        layer.GetComponent<SpriteRenderer>().color = ConvertColor(color);

        layer.transform.localPosition = new Vector3(0, index * layerHeight, 0);

        _layerVisuals.Add(layer);
    }
}
