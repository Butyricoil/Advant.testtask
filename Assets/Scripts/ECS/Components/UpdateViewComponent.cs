using System.Collections.Generic;
using UnityEngine;

public struct UpdateViewComponent
{
    private List<BusinessView> _businessViews;

    public void RegisterBusinessView(BusinessView view)
    {
        if (_businessViews == null)
        {
            _businessViews = new List<BusinessView>();
        }
        
        if (!_businessViews.Contains(view))
        {
            _businessViews.Add(view);
        }
    }

    public void UnregisterBusinessView(BusinessView view)
    {
        if (_businessViews != null)
        {
            _businessViews.Remove(view);
        }
    }

    public void UpdateAllViews()
    {
        if (_businessViews == null) return;
        
        foreach (var view in _businessViews)
        {
            if (view != null)
            {
                view.UpdateView();
            }
        }
    }
} 