using UnityEngine;

public class BaseBehaviour : MonoBehaviour
{
    private GameObject _gameObject;
    private Transform _transform;

    public new GameObject gameObject
    {
        get
        {
            GameObject result;
            if ((result = _gameObject) == null)
            {
                result = (_gameObject = base.gameObject);
            }

            return result;
        }
    }

    public new Transform trtransform
    {
        get
        {
            Transform result;
            if ((result = _transform) == null)
            {
                result = (_transform = base.transform);
            }

            return result;
        }
    }

    public Vector3 position
    {
        set => _transform.position = value;
        get => _transform.position;
    }
}