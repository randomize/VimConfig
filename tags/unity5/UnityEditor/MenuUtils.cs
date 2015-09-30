namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class MenuUtils
    {
        public static void ExtractMenuItemWithPath(string menuString, GenericMenu menu, string replacementMenuString, UnityEngine.Object[] temporaryContext)
        {
            MenuCallbackObject userData = new MenuCallbackObject {
                menuItemPath = menuString,
                temporaryContext = temporaryContext
            };
            menu.AddItem(new GUIContent(replacementMenuString), false, new GenericMenu.MenuFunction2(MenuUtils.MenuCallback), userData);
        }

        public static void ExtractSubMenuWithPath(string path, GenericMenu menu, string replacementPath, UnityEngine.Object[] temporaryContext)
        {
            HashSet<string> set = new HashSet<string>(Unsupported.GetSubmenus(path));
            foreach (string str in Unsupported.GetSubmenusIncludingSeparators(path))
            {
                string replacementMenuString = replacementPath + str.Substring(path.Length);
                if (set.Contains(str))
                {
                    ExtractMenuItemWithPath(str, menu, replacementMenuString, temporaryContext);
                }
            }
        }

        public static void MenuCallback(object callbackObject)
        {
            MenuCallbackObject obj2 = callbackObject as MenuCallbackObject;
            if (obj2.temporaryContext != null)
            {
                EditorApplication.ExecuteMenuItemWithTemporaryContext(obj2.menuItemPath, obj2.temporaryContext);
            }
            else
            {
                EditorApplication.ExecuteMenuItem(obj2.menuItemPath);
            }
        }

        private class MenuCallbackObject
        {
            public string menuItemPath;
            public UnityEngine.Object[] temporaryContext;
        }
    }
}

