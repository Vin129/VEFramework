--
-- Author: Your Name
-- Date: 2016-10-29 14:54:12
--
WWW = UnityEngine.WWW;

--LuaKit
LuaHelper = LuaKit.LuaHelper
UIHelper = LuaKit.UIHelper
-- GameObject = UnityEngine.GameObject;
-- Resources = UnityEngine.Resources;
-- Image = UnityEngine.UI.Image;
-- Input = UnityEngine.Input;
-- Screen = UnityEngine.Screen
-- Camera = UnityEngine.Camera
-- PlayerPrefs = UnityEngine.PlayerPrefs
--UI
-- ScrollRect = UnityEngine.UI.ScrollRect
-- Toggle = UnityEngine.UI.Toggle
-- ToggleEvent = UnityEngine.UI.Toggle.ToggleEvent
-- RectTransform = UnityEngine.RectTransform
--2D
-- Sprite = UnityEngine.Sprite

-- Rect = UnityEngine.Rect

function ccp(x,y)
	return Vector2.New(x - 320,y - 480)
end




TexturePool = {}
