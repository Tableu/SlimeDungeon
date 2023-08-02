using System;
using System.Collections.Generic;
using Controller.Form;
using UnityEngine;
using UnityEngine.InputSystem;

public class FormManager
{
    private List<FormInstance> _formInstances;
    private int _maxFormCount;
    private int _formIndex;
    private Form _currentForm;
    private FormInstance _currentFormInstance;
    private PlayerController _playerController;
    private GameObject _model;
    
    public Form CurrentForm => _currentForm;
    public List<FormInstance> formInstances => _formInstances;
    public int MaxFormCount => _maxFormCount;
    public FormInstance CurrentFormInstance => _currentFormInstance;
    
    public Action OnFormChange;
    public Action<FormInstance, int> OnFormAdd;

    public FormManager(PlayerController playerController,PlayerData playerData, GameObject model)
    {
        _formIndex = 0;
        _maxFormCount = playerData.MaxFormCount;
        _playerController = playerController;
        _model = model;
    }
    
    public void AddForm(FormData formData)
    {
        if (_formInstances.Count >= _maxFormCount)
        {
            if (_formInstances.Count > 0)
            {
                GameObject item = GameObject.Instantiate(_formInstances[_formIndex].Data.Item, _playerController.transform.position, Quaternion.identity);
                FormItem script = item.GetComponent<FormItem>();
                script.Initialize(_formInstances[_formIndex].Data);
                _formInstances.RemoveAt(_formIndex);
            }

            FormInstance formInstance = new FormInstance(formData);
            _currentFormInstance = formInstance;
            _formInstances.Insert(_formIndex, formInstance);
            EquipForm(formData);
            OnFormAdd?.Invoke(formInstance, _formIndex);
        }
        else
        {
            FormInstance formInstance = new FormInstance(formData);
            _formInstances.Add(formInstance);
            OnFormAdd?.Invoke(formInstance, _formInstances.Count-1);
        }
        
    }
    
    public void EquipForm(FormData formData)
    {
        if (_currentForm is not null)
        {
            _currentForm.Drop();
            GameObject.Destroy(_currentForm);
        }
        ChangeModel(formData);
        _currentForm = formData.AttachScript(_model);
        _currentForm.Equip(_playerController);
        OnFormChange?.Invoke();
    }
    
    public void SwitchForms(InputAction.CallbackContext context)
    {
        int oldIndex = _formIndex;
        _formIndex += (int)context.ReadValue<float>();
        if (_formIndex >= _formInstances.Count)
        {
            _formIndex = 0;
        }

        if (_formIndex < 0)
        {
            _formIndex = _formInstances.Count - 1;
        }

        if (_formIndex != oldIndex)
        {
            _currentFormInstance = _formInstances[_formIndex];
            EquipForm(_formInstances[_formIndex].Data);
        }
    }
    
    private void ChangeModel(FormData data)
    {
        _model.SetActive(false);
        GameObject.Destroy(_model);
        _model = GameObject.Instantiate(data.Model, _playerController.transform);
        _model.layer = _playerController.gameObject.layer;
    }
}
