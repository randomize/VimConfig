namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public static class ExecuteEvents
    {
        [CompilerGenerated]
        private static UnityAction<List<IEventSystemHandler>> <>f__am$cache13;
        private static readonly EventFunction<IBeginDragHandler> s_BeginDragHandler = new EventFunction<IBeginDragHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<ICancelHandler> s_CancelHandler = new EventFunction<ICancelHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IDeselectHandler> s_DeselectHandler = new EventFunction<IDeselectHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IDragHandler> s_DragHandler = new EventFunction<IDragHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IDropHandler> s_DropHandler = new EventFunction<IDropHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IEndDragHandler> s_EndDragHandler = new EventFunction<IEndDragHandler>(ExecuteEvents.Execute);
        private static readonly ObjectPool<List<IEventSystemHandler>> s_HandlerListPool;
        private static readonly EventFunction<IInitializePotentialDragHandler> s_InitializePotentialDragHandler = new EventFunction<IInitializePotentialDragHandler>(ExecuteEvents.Execute);
        private static readonly List<Transform> s_InternalTransformList;
        private static readonly EventFunction<IMoveHandler> s_MoveHandler = new EventFunction<IMoveHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IPointerClickHandler> s_PointerClickHandler = new EventFunction<IPointerClickHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IPointerDownHandler> s_PointerDownHandler = new EventFunction<IPointerDownHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IPointerEnterHandler> s_PointerEnterHandler = new EventFunction<IPointerEnterHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IPointerExitHandler> s_PointerExitHandler = new EventFunction<IPointerExitHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IPointerUpHandler> s_PointerUpHandler = new EventFunction<IPointerUpHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IScrollHandler> s_ScrollHandler = new EventFunction<IScrollHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<ISelectHandler> s_SelectHandler = new EventFunction<ISelectHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<ISubmitHandler> s_SubmitHandler = new EventFunction<ISubmitHandler>(ExecuteEvents.Execute);
        private static readonly EventFunction<IUpdateSelectedHandler> s_UpdateSelectedHandler = new EventFunction<IUpdateSelectedHandler>(ExecuteEvents.Execute);

        static ExecuteEvents()
        {
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = new UnityAction<List<IEventSystemHandler>>(ExecuteEvents.<s_HandlerListPool>m__0);
            }
            s_HandlerListPool = new ObjectPool<List<IEventSystemHandler>>(null, <>f__am$cache13);
            s_InternalTransformList = new List<Transform>(30);
        }

        [CompilerGenerated]
        private static void <s_HandlerListPool>m__0(List<IEventSystemHandler> l)
        {
            l.Clear();
        }

        public static bool CanHandleEvent<T>(GameObject go) where T: IEventSystemHandler
        {
            List<IEventSystemHandler> results = s_HandlerListPool.Get();
            GetEventList<T>(go, results);
            int count = results.Count;
            s_HandlerListPool.Release(results);
            return (count != 0);
        }

        private static void Execute(IBeginDragHandler handler, BaseEventData eventData)
        {
            handler.OnBeginDrag(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(ICancelHandler handler, BaseEventData eventData)
        {
            handler.OnCancel(eventData);
        }

        private static void Execute(IDeselectHandler handler, BaseEventData eventData)
        {
            handler.OnDeselect(eventData);
        }

        private static void Execute(IDragHandler handler, BaseEventData eventData)
        {
            handler.OnDrag(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IDropHandler handler, BaseEventData eventData)
        {
            handler.OnDrop(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IEndDragHandler handler, BaseEventData eventData)
        {
            handler.OnEndDrag(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IInitializePotentialDragHandler handler, BaseEventData eventData)
        {
            handler.OnInitializePotentialDrag(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IMoveHandler handler, BaseEventData eventData)
        {
            handler.OnMove(ValidateEventData<AxisEventData>(eventData));
        }

        private static void Execute(IPointerClickHandler handler, BaseEventData eventData)
        {
            handler.OnPointerClick(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IPointerDownHandler handler, BaseEventData eventData)
        {
            handler.OnPointerDown(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IPointerEnterHandler handler, BaseEventData eventData)
        {
            handler.OnPointerEnter(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IPointerExitHandler handler, BaseEventData eventData)
        {
            handler.OnPointerExit(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IPointerUpHandler handler, BaseEventData eventData)
        {
            handler.OnPointerUp(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IScrollHandler handler, BaseEventData eventData)
        {
            handler.OnScroll(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(ISelectHandler handler, BaseEventData eventData)
        {
            handler.OnSelect(eventData);
        }

        private static void Execute(ISubmitHandler handler, BaseEventData eventData)
        {
            handler.OnSubmit(eventData);
        }

        private static void Execute(IUpdateSelectedHandler handler, BaseEventData eventData)
        {
            handler.OnUpdateSelected(eventData);
        }

        public static bool Execute<T>(GameObject target, BaseEventData eventData, EventFunction<T> functor) where T: IEventSystemHandler
        {
            List<IEventSystemHandler> results = s_HandlerListPool.Get();
            GetEventList<T>(target, results);
            for (int i = 0; i < results.Count; i++)
            {
                T local;
                try
                {
                    local = results[i];
                }
                catch (Exception exception)
                {
                    IEventSystemHandler handler = results[i];
                    Debug.LogException(new Exception(string.Format("Type {0} expected {1} received.", typeof(T).Name, handler.GetType().Name), exception));
                    continue;
                }
                try
                {
                    functor(local, eventData);
                }
                catch (Exception exception2)
                {
                    Debug.LogException(exception2);
                }
            }
            int count = results.Count;
            s_HandlerListPool.Release(results);
            return (count > 0);
        }

        public static GameObject ExecuteHierarchy<T>(GameObject root, BaseEventData eventData, EventFunction<T> callbackFunction) where T: IEventSystemHandler
        {
            GetEventChain(root, s_InternalTransformList);
            for (int i = 0; i < s_InternalTransformList.Count; i++)
            {
                Transform transform = s_InternalTransformList[i];
                if (Execute<T>(transform.gameObject, eventData, callbackFunction))
                {
                    return transform.gameObject;
                }
            }
            return null;
        }

        private static void GetEventChain(GameObject root, IList<Transform> eventChain)
        {
            eventChain.Clear();
            if (root != null)
            {
                for (Transform transform = root.transform; transform != null; transform = transform.parent)
                {
                    eventChain.Add(transform);
                }
            }
        }

        public static GameObject GetEventHandler<T>(GameObject root) where T: IEventSystemHandler
        {
            if (root != null)
            {
                for (Transform transform = root.transform; transform != null; transform = transform.parent)
                {
                    if (CanHandleEvent<T>(transform.gameObject))
                    {
                        return transform.gameObject;
                    }
                }
            }
            return null;
        }

        private static void GetEventList<T>(GameObject go, IList<IEventSystemHandler> results) where T: IEventSystemHandler
        {
            if (results == null)
            {
                throw new ArgumentException("Results array is null", "results");
            }
            if ((go != null) && go.activeInHierarchy)
            {
                List<Component> list = ListPool<Component>.Get();
                go.GetComponents<Component>(list);
                for (int i = 0; i < list.Count; i++)
                {
                    if (ShouldSendToComponent<T>(list[i]))
                    {
                        results.Add(list[i] as IEventSystemHandler);
                    }
                }
                ListPool<Component>.Release(list);
            }
        }

        private static bool ShouldSendToComponent<T>(Component component) where T: IEventSystemHandler
        {
            if (!(component is T))
            {
                return false;
            }
            Behaviour behaviour = component as Behaviour;
            if (behaviour != null)
            {
                return behaviour.isActiveAndEnabled;
            }
            return true;
        }

        public static T ValidateEventData<T>(BaseEventData data) where T: class
        {
            if (!(data is T))
            {
                throw new ArgumentException(string.Format("Invalid type: {0} passed to event expecting {1}", data.GetType(), typeof(T)));
            }
            return (data as T);
        }

        public static EventFunction<IBeginDragHandler> beginDragHandler
        {
            get
            {
                return s_BeginDragHandler;
            }
        }

        public static EventFunction<ICancelHandler> cancelHandler
        {
            get
            {
                return s_CancelHandler;
            }
        }

        public static EventFunction<IDeselectHandler> deselectHandler
        {
            get
            {
                return s_DeselectHandler;
            }
        }

        public static EventFunction<IDragHandler> dragHandler
        {
            get
            {
                return s_DragHandler;
            }
        }

        public static EventFunction<IDropHandler> dropHandler
        {
            get
            {
                return s_DropHandler;
            }
        }

        public static EventFunction<IEndDragHandler> endDragHandler
        {
            get
            {
                return s_EndDragHandler;
            }
        }

        public static EventFunction<IInitializePotentialDragHandler> initializePotentialDrag
        {
            get
            {
                return s_InitializePotentialDragHandler;
            }
        }

        public static EventFunction<IMoveHandler> moveHandler
        {
            get
            {
                return s_MoveHandler;
            }
        }

        public static EventFunction<IPointerClickHandler> pointerClickHandler
        {
            get
            {
                return s_PointerClickHandler;
            }
        }

        public static EventFunction<IPointerDownHandler> pointerDownHandler
        {
            get
            {
                return s_PointerDownHandler;
            }
        }

        public static EventFunction<IPointerEnterHandler> pointerEnterHandler
        {
            get
            {
                return s_PointerEnterHandler;
            }
        }

        public static EventFunction<IPointerExitHandler> pointerExitHandler
        {
            get
            {
                return s_PointerExitHandler;
            }
        }

        public static EventFunction<IPointerUpHandler> pointerUpHandler
        {
            get
            {
                return s_PointerUpHandler;
            }
        }

        public static EventFunction<IScrollHandler> scrollHandler
        {
            get
            {
                return s_ScrollHandler;
            }
        }

        public static EventFunction<ISelectHandler> selectHandler
        {
            get
            {
                return s_SelectHandler;
            }
        }

        public static EventFunction<ISubmitHandler> submitHandler
        {
            get
            {
                return s_SubmitHandler;
            }
        }

        public static EventFunction<IUpdateSelectedHandler> updateSelectedHandler
        {
            get
            {
                return s_UpdateSelectedHandler;
            }
        }

        public delegate void EventFunction<T1>(T1 handler, BaseEventData eventData);
    }
}

