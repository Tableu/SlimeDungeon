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
    public Action<Form> OnFormRemoved;

    public FormManager(PlayerController playerController, GameObject model)
    {
        _playerController = playerController;
        _model = model;
        _forms = new List<Form>();
        _formIndex = 0;
        _maxFormCount = ((PlayerData)_playerController.CharacterData).MaxFormCount;
        
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

    public void InitializeForm()
    {
        Form form = new Form(((PlayerData)_playerController.CharacterData).StartForm, _playerController);
        _forms.Add(form);
        OnFormAdd?.Invoke(form, _forms.Count-1);
        ChangeModel(form.Data);
        CurrentForm.Equip(_model, _playerController.PlayerInputActions.Other.BasicAttack);
        OnFormChange?.Invoke();
    }
    public void AddForm(Form form)
    {
        if (_forms.Count >= _maxFormCount)
        {
            if (_forms.Count > 0)
            {
                GameObject item = Object.Instantiate(CurrentForm.Data.Item, _playerController.transform.position, Quaternion.identity);
                FormItem script = item.GetComponent<FormItem>();
                script.Initialize(CurrentForm);
                OnFormRemoved?.Invoke(CurrentForm);
            }
            EquipForm(form);
            OnFormAdd?.Invoke(form, _formIndex);
        }
        else
        {
            _forms.Add(form);
            OnFormAdd?.Invoke(form, _forms.Count-1);
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
            if (CurrentForm.Health > 0)
            {
                CurrentForm.Drop();
                _formIndex = formIndex;
                ChangeModel(CurrentForm.Data);
                CurrentForm.Equip(_model, _playerController.PlayerInputActions.Other.BasicAttack);
                OnFormChange?.Invoke();
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
    private void EquipForm(Form form)
    {
        if (CurrentForm is not null)
        {
            CurrentForm.Drop();
        }
        ChangeModel(form.Data);
        CurrentForm = form;
        CurrentForm.Equip(_model, _playerController.PlayerInputActions.Other.BasicAttack);
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
                EquipForm(form);
                return;
            }
        }
        Object.Destroy(_playerController.gameObject);
    }
    #endregion
}
