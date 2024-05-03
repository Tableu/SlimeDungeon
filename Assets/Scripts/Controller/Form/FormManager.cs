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
    private Form _currentForm;
    private int _currentFormIndex = 0;

    public Form CurrentForm => _currentForm;

    public List<Form> Forms => _forms;
    public int MaxFormCount => _maxFormCount;
    
    public Action OnFormChange;
    public Action<Form, int> OnFormAdd;
    public Action<Form> OnFormRemoved;

    public FormManager(PlayerController playerController, GameObject model)
    {
        _playerController = playerController;
        _model = model;
        _forms = new List<Form>();
        _maxFormCount = ((PlayerData)_playerController.CharacterData).MaxFormCount;
        
        _playerController.PlayerInputActions.Other.SwitchForms.started += SwitchForms;
        _playerController.OnDamage += OnDamage;
    }

    ~FormManager()
    {
        if (_playerController == null) return;
        _playerController.PlayerInputActions.Other.SwitchForms.started -= SwitchForms;
        _playerController.OnDamage -= OnDamage;
    }
    #region Public Methods

    public void InitializeForm()
    {
        Form form = new Form(((PlayerData)_playerController.CharacterData).StartForm, _playerController);
        _forms.Add(form);
        ChangeForm(form, _forms.Count-1);
        OnFormAdd?.Invoke(form, _forms.Count-1);
    }
    public void AddForm(Form form)
    {
        if (_forms.Count >= _maxFormCount)
        {
            if (_forms.Count > 0)
            {
                //todo send form to inventory
                OnFormRemoved?.Invoke(_currentForm);
            }
            ChangeForm(form, _currentFormIndex);
            _forms[_currentFormIndex] = form;
            OnFormAdd?.Invoke(form, _currentFormIndex);
        }
        else
        {
            _forms.Add(form);
            OnFormAdd?.Invoke(form, _forms.Count-1);
        }
    }
    
    public void SwitchForms(InputAction.CallbackContext context)
    {
        int oldIndex = _currentFormIndex;
        int diff = (int)context.ReadValue<float>();
        int formIndex = _currentFormIndex+diff;
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
                ChangeForm(_forms[formIndex],formIndex);
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
        if (Math.Abs(_currentForm.Health - _currentForm.Data.Health) > Mathf.Epsilon)
        {
            _currentForm.Health += amount;
        }
        else
        {
            foreach (var formInstance in _forms)
            {
                if (Math.Abs(_currentForm.Health - _currentForm.Data.Health) > Mathf.Epsilon)
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
    
    public void FormFainted()
    {
        int index = 0;
        foreach (Form form in _forms)
        {
            if (form.Health > 0)
            {
                ChangeForm(form, index);
                return;
            }

            index++;
        }
        Object.Destroy(_playerController.gameObject);
    }
    #endregion
    #region Private Methods

    private void ChangeForm(Form newForm, int newIndex)
    {
        if(_currentForm is not null)
            _currentForm.Drop();
        _currentForm = newForm;
        _currentFormIndex = newIndex;
        ChangeModel(newForm.Data);
        _currentForm.Equip(_model, _playerController.PlayerInputActions.Other.BasicAttack);
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
        _currentForm.Health = _playerController.Health;
    }
    #endregion
}
