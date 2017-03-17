using UnityEngine;
using System;

namespace GameSupport
{
	public abstract class UIScreenBase : MonoBehaviour 
	{
		public bool IsHidden { get; set; }

		public void Initialize(UIScreenArgs args)
		{
			OnInitialize(args);
		}

		public void ReleaseResources()
		{
			OnReleaseResources();
		}

		public void Hide()
		{
			IsHidden = true;
			OnHide();
			gameObject.SetActive(false);
		}
		
		public void UnHide()
		{
			IsHidden = false;
			gameObject.SetActive(true);
			OnUnHide();
		}

		public void AddWidget(GameObject widget)
		{
			OnAddWidget(widget);
		}

		public void RemoveWidget(GameObject widget)
		{
			OnRemoveWidget(widget);
		}

		public void StartTransitionIn(Action onFinishedCallback)
		{
			OnStartTransitionIn(onFinishedCallback);
		}

		public void StartTransitionOut(Action onFinishedCallback)
		{
			OnStartTransitionOut(onFinishedCallback);
		}

		protected abstract void OnInitialize(UIScreenArgs args);
		protected abstract void OnReleaseResources();

		protected virtual void OnHide() {}
		protected virtual void OnUnHide() {}

		protected virtual void OnAddWidget(GameObject widget) {}
		protected virtual void OnRemoveWidget(GameObject widget) {}
		
		protected virtual void OnStartNoTransition() {}

		protected virtual void OnStartTransitionIn(Action onFinishedCallback)
		{
			Debug.LogError("Attempting to start a transition in on a screen that does not handle it!");
			if(onFinishedCallback != null)
			{
				onFinishedCallback();
			}
		}

		protected virtual void OnStartTransitionOut(Action onFinishedCallback)
		{
			Debug.LogError("Attempting to start a transition out on a screen that does not handle it!");
			if(onFinishedCallback != null)
			{
				onFinishedCallback();
			}
		}
	}
}