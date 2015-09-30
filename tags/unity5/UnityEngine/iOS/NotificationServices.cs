namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class NotificationServices
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CancelAllLocalNotifications();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CancelLocalNotification(UnityEngine.iOS.LocalNotification notification);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearLocalNotifications();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearRemoteNotifications();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.iOS.LocalNotification GetLocalNotification(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern UnityEngine.iOS.RemoteNotification GetRemoteNotification(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PresentLocalNotificationNow(UnityEngine.iOS.LocalNotification notification);
        public static void RegisterForNotifications(NotificationType notificationTypes)
        {
            RegisterForNotifications(notificationTypes, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RegisterForNotifications(NotificationType notificationTypes, bool registerForRemote);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ScheduleLocalNotification(UnityEngine.iOS.LocalNotification notification);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UnregisterForRemoteNotifications();

        public static byte[] deviceToken { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static NotificationType enabledNotificationTypes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int localNotificationCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static UnityEngine.iOS.LocalNotification[] localNotifications
        {
            get
            {
                int localNotificationCount = UnityEngine.iOS.NotificationServices.localNotificationCount;
                UnityEngine.iOS.LocalNotification[] notificationArray = new UnityEngine.iOS.LocalNotification[localNotificationCount];
                for (int i = 0; i < localNotificationCount; i++)
                {
                    notificationArray[i] = GetLocalNotification(i);
                }
                return notificationArray;
            }
        }

        public static string registrationError { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int remoteNotificationCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static UnityEngine.iOS.RemoteNotification[] remoteNotifications
        {
            get
            {
                int remoteNotificationCount = UnityEngine.iOS.NotificationServices.remoteNotificationCount;
                UnityEngine.iOS.RemoteNotification[] notificationArray = new UnityEngine.iOS.RemoteNotification[remoteNotificationCount];
                for (int i = 0; i < remoteNotificationCount; i++)
                {
                    notificationArray[i] = GetRemoteNotification(i);
                }
                return notificationArray;
            }
        }

        public static UnityEngine.iOS.LocalNotification[] scheduledLocalNotifications { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

