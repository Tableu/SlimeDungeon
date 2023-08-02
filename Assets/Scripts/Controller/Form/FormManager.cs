using System;
using System.Collections.Generic;
using Controller.Form;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public class FormManager
{
    private GameObject _model;
    private PlayerController _playerController;
    private Form _currentForm;
    private FormInstance _currentFormInstance;
    private List<FormInstance> _forms;
    private int _maxFormCount;
    private int _formIndex;
    
    public Form CurrentForm => _currentForm;
    public List<FormInstance> Forms => _forms;
    public int MaxFormCount => _maxFormCount;
    
    public Action OnFormChange;
    public Action<FormInstance, int> OnFormAdd;

    public FormManager(PlayerController playerController, GameObject model)
    {
        _playerController = playerController;
        _model = model;
        _forms = new List<FormInstance>();
        _formIndex = 0;
        _maxFormCount = ((PlayerData)_playerController.CharacterData).MaxFormCount;
        AddForm(((PlayerData)_playerController.CharacterData).BaseForm);
        EquipForm(((PlayerData)_playerController.CharacterData).BaseForm);
        _currentFormInstance = _forms[0];
        _playerController.PlayerInputActions.Other.SwitchForms.started += SwitchForms;
        _playerController.OnDamage += OnDamage;
        _playerController.OnFormFaint += OnFormFaint;
    }

    ~FormManager()
    {
        if (_playerController == null) return;
        _playerController.PlayerInputActions.Other.SwitchForms.started -= SwitchForms;
        _playerController.OnDamage -= OnDamage;
        _playerController.OnFormFaint -= OnFormFaint;
    }
    
    public void AddForm(FormData formData)
    {
        if (_forms.Count >= _maxFormCount)
        {
            if (_forms.Count > 0)
            {
                GameObject item = Object.Instantiate(_forms[_formIndex].Data.Item, _playerController.transform.position, Quaternion.identity);
                FormItem script = item.GetComponent<FormItem>();
                script.Initialize(_forms[_formIndex].Data);
                _forms.RemoveAt(_formIndex);
            }

            FormInstance formInstance = new FormInstance(formData);
            _currentFormInstance = formInstance;
            _forms.Insert(_formIndex, formInstance);
            EquipForm(formData);
            OnFormAdd?.Invoke(formInstance, _formIndex);
        }
        else
        {
            FormInstance formInstance = new FormInstance(formData);
            _forms.Add(formInstance);
            OnFormAdd?.Invoke(formInstance, _forms.Count-1);
        }
    }
    
    private void EquipForm(FormData formData)
    {
        if (_currentForm is not null)
        {
            _currentForm.Drop();
            Object.Destroy(_currentForm);
        }
        ChangeModel(formData);
        _currentForm = formData.AttachScript(_model);
        _currentForm.Equip(_playerController);
        OnFormChange?.Invoke();
    }
    
    public void SwitchForms(InputAction.CallbackContext context)
    {
        int oldIndex = _formIndex;
        int diff = (int)context.ReadValue<float>();
        int formIndex = _formIndex+diff;
        if (formIndex >= _forms.Count)
        {
            formIndex = 0;
        }

        if (formIndex < 0)
        {
            formIndex = _forms.Count - 1;
        }
        
        while (formIndex != oldIndex)
        {
            if (_forms[formIndex].Health > 0)
            {
                _formIndex = formIndex;
                _currentFormInstance = _forms[_formIndex];
                EquipForm(_forms[_formIndex].Data);
                return;
            }

            formIndex += diff;
            if (formIndex >= _forms.Count)
            {
                formIndex = 0;
            }

            if (formIndex < 0)
            {
                formIndex = _forms.Count - 1;
            }
        }
        
        /*_formIndex += (int)context.ReadValue<float>();
        if (_formIndex >= _forms.Count)
        {
            _formIndex = 0;
        }

        if (_formIndex < 0)
        {
            _formIndex = _forms.Count - 1;
        }

        if (_formIndex != oldIndex)
        {
            _currentFormInstance = _forms[_formIndex];
            EquipForm(_forms[_formIndex].Data);
        }*/
    }

    private void OnDamage()
    {
        _currentFormInstance.Health = _playerController.Health;
    }

    private void OnFormFaint()
    {
        foreach (FormInstance formInstance in _forms)
        {
            if (formInstance.Health > 0)
            {
                _currentFormInstance = formInstance;
                EquipForm(formInstance.Data);
                return;
            }
        }
        Object.Destroy(_playerController.gameObject);
    }
    
    private void ChangeModel(FormData data)
    {
        _model.SetActive(false);
        Object.Destroy(_model);
        _model = Object.Instantiate(data.Model, _playerController.transform);
        _model.layer = _playerController.gameObject.layer;
    }
}
