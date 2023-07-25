using Controller.Form;
using UnityEngine;

public class FormItem : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float height;
    [SerializeField] private float rotateSpeed;
    private Vector3 _pos;
    private GameObject _model;
    private FormData _data;
    public void Initialize(FormData data)
    {
        _data = data;
    }

    private void Start()
    {
        _pos = transform.position;
    }

    private void Update()
    {
        float newY = Mathf.Sin(Time.time * speed)*height + _pos.y;
        transform.position = new Vector3(_pos.x, newY, _pos.z);
        transform.Rotate(Vector3.up, rotateSpeed);
    }

    public void PickUp(PlayerController character)
    {
        character.EquipForm(_data);
        Destroy(gameObject);
    }
}
