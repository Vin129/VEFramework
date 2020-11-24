/****************************************************************************
 * Copyright (c) 2020 vin129
 *  
 * May the Force be with you :)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/
namespace VEFramework.HotScriptKit
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    using VEFramework;
#if DEFINE_VE_LUA
    using LuaInterface;
#endif

    public static class UIHelper
    {
        #region Transform
        public static GameObject FindChildGameObj(GameObject obj,String path)
        {
            if (null == obj || null == path)
            {
                return null;
            }
            Transform kTs = obj.GetComponent<Transform>();
            var tTs = kTs.Find(path);
            if (tTs == null)
                return null;
            return tTs.gameObject;
        }

        public static void SetParent(GameObject obj, GameObject parent)
        {
            if (null == obj || null == parent)
            {
                return;
            }
            RectTransform kTs = obj.GetComponent<RectTransform>();
            kTs.SetParent(parent.transform, true);
        }

        public static void SetTransform(GameObject kObj, RectTransform transform)
        {
            if (null == kObj)
                return;
            Vector3 pos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            Vector3 scale = transform.localScale;
            Vector2 anchoredPos = transform.anchoredPosition;
            Vector3 anchoredPos3D = transform.anchoredPosition3D;
            Vector2 offsetMax = transform.offsetMax;
            Vector2 offsetMin = transform.offsetMin;
            RectTransform kTs = kObj.GetComponent<RectTransform>();
            kTs.localPosition = pos;
            kTs.localRotation = rot;
            kTs.localScale = scale;
            kTs.anchoredPosition = anchoredPos;
            kTs.anchoredPosition3D = anchoredPos3D;
            kTs.offsetMin = offsetMin;
            kTs.offsetMax = offsetMax;
        }

        public static void SetLocalPosition(GameObject kObj, Vector3 pos)
        {
            if (null == kObj || null == pos)
                return;
            kObj.transform.localPosition = pos;
        }
        #endregion

        #region Text
        public static readonly string no_breaking_space = "\u00A0";
        public static void SetLabelText(GameObject kObj, string strText)
        {
            if (null == kObj || String.IsNullOrEmpty(strText))
                return;
            Text kTextComponet = kObj.GetComponent<Text>();
            if (null == kTextComponet)
                return;
            kTextComponet.text = strText;
            kTextComponet.text = kTextComponet.text.Replace("\\n", "\n");
            kTextComponet.text = kTextComponet.text.Replace(" ", no_breaking_space);
        }
        #endregion

        #region Button
        public static void SetButtonInteractable(GameObject kObj, bool interactable)
        {
            if (null == kObj)
                return;
            Button kButton = kObj.GetComponent<Button>();
            if (null == kButton)
                return;

            kButton.interactable = interactable;
        }
        public static void SetButtonAble(GameObject kObj, bool able)
        {
            if (null == kObj)
                return;
            Button kButton = kObj.GetComponent<Button>();
            if (null == kButton)
                return;

            kButton.enabled = able;
        }
        public static void SetButtonClickEvent(GameObject kObj, object kLuaFunc)
        {
            if (null == kObj)
                return;

            Button kButton = kObj.GetComponent<Button>();
            if (null == kButton)
                return;

            kButton.onClick.RemoveAllListeners();
            kButton.onClick.AddListener(delegate()
            {
                LuaPerformer.Call(kLuaFunc,kObj);
            });
        }

        public static void RemoveButtonClickEvent(GameObject kObj)
        {
            if (null == kObj)
                return;

            Button kButton = kObj.GetComponent<Button>();
            if (null == kButton)
                return;

            kButton.onClick.RemoveAllListeners();
        }

    //模拟按钮点击
        public static void SetButtonClicked(GameObject kObj)
        {
            if (null == kObj)
                return;

            Button kButton = kObj.GetComponent<Button>();
            if (null == kButton)
                return;

            kButton.onClick.Invoke();
        }

        #endregion

        #region Dropdown 
        public static void AddDropdownEvent(GameObject kObj, object kLuaFunc)
        {
            if (null == kObj)
                return;

            Dropdown dd = kObj.GetComponent<Dropdown>();
            if (null == dd)
                return;

            dd.onValueChanged.AddListener(delegate(int index)
            {
                Dropdown.OptionData _data = dd.options[index];
                LuaPerformer.Call(kLuaFunc,index, _data.text);
            });
        }

        public static void AddDropdownOption(GameObject kObj, string text, string textDst)
        {
            if (null == kObj)
                return;

            Dropdown dd = kObj.GetComponent<Dropdown>();
            if (null == dd)
                return;

            Dropdown.OptionData _data = new Dropdown.OptionData();
            _data.text = text;
            dd.options.Add(_data);

            if (text == textDst)
            {
                dd.value = dd.options.IndexOf(_data);
            }
        }

        public static void RemoveDropdownOption(GameObject kObj, int index)
        {
            if (null == kObj)
                return;

            Dropdown dd = kObj.GetComponent<Dropdown>();
            if (null == dd)
                return;
            if (dd.options.Count <= index)
                return;
            dd.options.RemoveAt(index);
        }

        public static void UpdateDropdownText(GameObject kObj, int index, string text)
        {
            if (null == kObj)
                return;

            Dropdown dd = kObj.GetComponent<Dropdown>();
            if (null == dd)
                return;
            if (dd.options.Count > index)
            {
                dd.options[index].text = text;
            }
            else
            {
                AddDropdownOption(kObj, text, " ");
            }
        }
        #endregion

        #region InputField 
        public static void SetInputEvent(GameObject kObj, object kLuaFunc)
        {
            if (null == kObj)
                return;

            InputField kInput = kObj.GetComponent<InputField>();
            if (null == kInput)
                return;
            kInput.onEndEdit.RemoveAllListeners();
            kInput.onEndEdit.AddListener(delegate (String str)
            {
                kInput.text = kInput.text.Replace(" ", no_breaking_space);
                LuaPerformer.Call(kLuaFunc,str);
            });
        }
        #endregion

        #region Toggle

        public static void SetToggleEvent(GameObject kObj, object kLuaFunc)
        {
            if (null == kObj)
                return;

            Toggle kToggle = kObj.GetComponent<Toggle>();
            if (null == kToggle)
                return;
            kToggle.onValueChanged.RemoveAllListeners();
            kToggle.onValueChanged.AddListener(delegate(bool isOn)
            {
                LuaPerformer.Call(kLuaFunc,isOn,kObj);
            });
        }
        public static bool GetToggleIsOn(GameObject kObj)
        {
            return kObj.GetComponent<Toggle>().isOn;
        }
        public static void SetToggleIsOn(GameObject kObj, bool isOn)
        {
            if (null == kObj)
                return;
            Toggle kToggle = kObj.GetComponent<Toggle>();
            if (null == kToggle)
                return;

            kToggle.isOn = isOn;
            kToggle.enabled = false;
            kToggle.enabled = true;
        }
        public static void SetToggleEnabled(GameObject kObj, bool enabled)
        {
            if (null == kObj)
                return;
            Toggle kToggle = kObj.GetComponent<Toggle>();
            if (null == kToggle)
                return;
            kToggle.isOn = enabled;
            kToggle.enabled = enabled;
        }

        public static void SetToggleInteractable(GameObject kObj, bool interactable)
        {
            if (null == kObj)
                return;
            Toggle kToggle = kObj.GetComponent<Toggle>();
            if (null == kToggle)
                return;

            kToggle.interactable = interactable;
        }

        public static void SetToggleGroup(GameObject kObj, GameObject gObj)
        {
            if (null == kObj || null == gObj)
                return;
            Toggle kToggle = kObj.GetComponent<Toggle>();
            if (null == kToggle)
                return;

            ToggleGroup kGroup = gObj.GetComponent<ToggleGroup>();
            if (null == kGroup)
            {
                return;
            }
            kToggle.group = kGroup;
        }

        #endregion

        #region EventTriggerListener 

        public static void RegisterDropdownClickEvent(GameObject obj, object kLuaFunc)
        {
            CustomDropdown kComponent = obj.GetComponent<CustomDropdown>();
            if (null == kComponent)
                return;
            kComponent.onClick = delegate (GameObject sender, BaseEventData kEvtData)
            {
                LuaPerformer.Call(kLuaFunc,sender, kEvtData);
            };
        }
        public static void RegisterClickEvent(GameObject obj, object kLuaFunc)
        {
            EventTriggerListener.Get(obj).onClick = delegate (GameObject sender, BaseEventData kEvtData)
            {
                LuaPerformer.Call(kLuaFunc,sender, kEvtData);
            };
        }
        public static void RegisterPressedDownEvent(GameObject obj, object kLuaFunc)
        {
            EventTriggerListener.Get(obj).onDown = delegate (GameObject sender, BaseEventData kEvtData)
            {
                LuaPerformer.Call(kLuaFunc,sender, kEvtData);
            };
        }

        public static void RegisterPressedUpEvent(GameObject obj, object kLuaFunc)
        {
            EventTriggerListener.Get(obj).onUp = delegate (GameObject sender, BaseEventData kEvtData)
            {
                LuaPerformer.Call(kLuaFunc,sender, kEvtData);
            };
        }

        public static void RegisterBeginDragEvent(GameObject obj, object kLuaFunc)
        {
            DragEventListener.Get(obj).onBeginDrag = delegate (GameObject sender, BaseEventData kEvtData)
            {
                CanvasGroup group = obj.GetComponent<CanvasGroup>();
                if (null == group)
                    group = obj.AddComponent<CanvasGroup>();
                group.blocksRaycasts = false;
                LuaPerformer.Call(kLuaFunc,sender, kEvtData);
            };
        }
        public static void RegisterDragEvent(GameObject obj, object kLuaFunc)
        {
            DragEventListener.Get(obj).onDrag = delegate (GameObject sender, BaseEventData kEvtData)
            {
                RectTransform kTs = sender.GetComponent<RectTransform>();
                kTs.pivot.Set(0, 0);
                PointerEventData kData = kEvtData as PointerEventData;
                Vector3 globalMousePos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(kTs, kData.position, kData.pressEventCamera, out globalMousePos))
                    kTs.position = globalMousePos;
                LuaPerformer.Call(kLuaFunc,sender, kEvtData);
            };
        }
        public static void RegisterEndDragEvent(GameObject obj, object kLuaFunc)
        {
            DragEventListener.Get(obj).onEndDrag = delegate (GameObject sender, BaseEventData kEvtData)
            {
                CanvasGroup group = obj.GetComponent<CanvasGroup>();
                if (null != group)
                    GameObject.Destroy(group);
                LuaPerformer.Call(kLuaFunc,sender, kEvtData);
            };
        }
        public static void RegisterNotDragEvent(GameObject obj, object kLuaFunc)
        {
            DragEventListener.Get(obj).onNotDrag = delegate (GameObject sender, BaseEventData kEvtData, bool canClick)
            {
                LuaPerformer.Call(kLuaFunc,sender, kEvtData, canClick);
            };
        }

        public static void RemoveAllDragEvent(GameObject obj)
        {
            DragEventListener.Get(obj).ClearDelegate();
        }
        public static void RevertAllDragEvent(GameObject obj)
        {
            DragEventListener.Get(obj).RevertDelegate();
        }


        public static void RegisterDropEvent(GameObject obj, object kLuaFunc)
        {
            DragEventListener.Get(obj).onDrop = delegate (GameObject sender, BaseEventData kEvtData)
            {
                PointerEventData kData = kEvtData as PointerEventData;
                GameObject kDragger = kData.pointerDrag;

                LuaPerformer.Call(kLuaFunc,sender, kDragger, kEvtData);
            };
        }

        public static void RegisterDropCallback(GameObject obj, object luaFunc)
        {
            if (null == obj)
                return;
            //TODO
        }

        public static void UnRegisterDropCallback(GameObject obj)
        {
            if (null == obj)
                return;
            //TODO
        }

        #endregion
    }
}
