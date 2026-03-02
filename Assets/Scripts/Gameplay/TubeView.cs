using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Threading.Tasks;

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
    private Vector3 _originalPos;
    private Quaternion _originalRot;

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
        {
            _tubeCap.SetActive(true);   
        }

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

            layer.GetComponent<SpriteRenderer>().color = ConvertColor(layers[i]);
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
        //PlaySelect();
    }

    public void PlaySelect()
    {
        transform.DOMoveY(_originalPos.y + 0.5f, 0.2f);
    }

    public void ResetSelect()
    {
        transform.DOMove(_originalPos, 0.2f);
    }

    public async Task PlayPourAnimation(Transform target)
    {
        float moveDuration = 0.3f;
        float rotateDuration = 0.2f;
        float horizontalOffset = 1.5f; 
        float verticalLift = 2f;
        Vector3 start = _originalPos;
        bool isRight = target.position.x > start.x;
        float offsetX = isRight ? -horizontalOffset : horizontalOffset;
        Vector3 pourPosition = new Vector3(target.position.x + offsetX,
                                           target.position.y + verticalLift,
                                           start.z);
        float direction = target.position.x > transform.position.x ? -45f : 45f;
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
        await Task.Delay(300);

        //return to straight 
        await transform.DORotate(_originalRot.eulerAngles, rotateDuration)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();

        //return to original position
        await transform.DOMove(start, moveDuration)
            .SetEase(Ease.InQuad)
            .AsyncWaitForCompletion();
    }
}
