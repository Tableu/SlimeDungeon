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
    private List<Form> _forms;
    private int _maxFormCount;
    private int _formIndex;
    
    public Form CurrentForm
    {
        get => _forms[_formIndex];
        private set => _forms[_formIndex] = value;
    }

    public List<Form> Forms => _forms;
    public int MaxFormCount => _maxFormCount;
    
    public Action OnFormChange;
    public Action<Form, int> OnFormAdd;

    public FormManager(PlayerController playerController, GameObject model)
    {
        _playerController = playerController;
        _model = model;
        _forms = new List<Form>();
        _formIndex = 0;
        _maxFormCount = ((PlayerData)_playerController.CharacterData).MaxFormCount;
        AddForm(((PlayerData)_playerController.CharacterData).BaseForm);
        EquipForm(((PlayerData)_playerController.CharacterData).BaseForm);
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
    #region Public Methods
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

            Form formInstance = new Form(formData, _playerController);
            _forms.Insert(_formIndex, formInstance);
            EquipForm(formData);
            OnFormAdd?.Invoke(formInstance, _formIndex);
        }
        else
        {
            Form formInstance = new Form(formData, _playerController);
            _forms.Add(formInstance);
            OnFormAdd?.Invoke(formInstance, _forms.Count-1);
        }
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
    }

    public void HealForm(float amount)
    {
        if (Math.Abs(CurrentForm.Health - CurrentForm.Data.Health) > Mathf.Epsilon)
        {
            CurrentForm.Health += amount;
        }
        else
        {
            foreach (var formInstance in _forms)
            {
                if (Math.Abs(CurrentForm.Health - CurrentForm.Data.Health) > Mathf.Epsilon)
                {
                    formInstance.Health += amount;
                }
            }
        }
    }

    public void HealForms(float amount)
    {
        foreach (var form in _forms)
        {
            form.Health += amount;
        }
    }
    
    #endregion
    #region Private Methods
    private void EquipForm(FormData formData)
    {
        if (CurrentForm is not null)
        {
            CurrentForm.Drop();
        }
        ChangeModel(formData);
        CurrentForm = new Form(formData, _playerController);
        CurrentForm.Equip(_model);
        OnFormChange?.Invoke();
    }
    
    private void ChangeModel(FormData data)
    {
        _model.SetActive(false);
        Object.Destroy(_model);
        _model = Object.Instantiate(data.Model, _playerController.transform);
        _model.layer = _playerController.gameObject.layer;
    }
    #endregion
    #region Event Functions
    private void OnDamage()
    {
        CurrentForm.Health = _playerController.Health;
    }

    private void OnFormFaint()
    {
        foreach (Form form in _forms)
        {
            if (form.Health > 0)
            {
                EquipForm(form.Data);
                return;
            }
        }
        Object.Destroy(_playerController.gameObject);
    }
    #endregion
}
