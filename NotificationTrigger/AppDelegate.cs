using System;
using Foundation;
using UIKit;
using UserNotifications;
using CoreLocation;

namespace NotificationTrigger
{
   // The UIApplicationDelegate for the application. This class is responsible for launching the
   // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
   [Register("AppDelegate")]
   public class AppDelegate : UIApplicationDelegate
   {
      public override UIWindow Window { get; set; }

      private const string ProximityUUID = "3DA62A3E-D6C4-B5C1-3F5F-5342C9165DC0";

      public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
      {
         UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (a, e) => { });
         var locationManager = new CLLocationManager();
         locationManager.RequestWhenInUseAuthorization();

         var prefName = new NSString("HasRegisteredNotification");
         var hasRegisteredNotification = NSUserDefaults.StandardUserDefaults.BoolForKey(prefName);

         if (!hasRegisteredNotification)
         {
            NSUserDefaults.StandardUserDefaults.SetBool(true, prefName);
                          
            var region = new CLBeaconRegion(new NSUuid(ProximityUUID), "MyRegion");
            region.NotifyOnEntry = true;
            region.NotifyOnExit = false;

            var content = new UNMutableNotificationContent();
            content.Title = "iBeacon notification test";
            content.Body = "iBeacon detected";

            var trigger = UNLocationNotificationTrigger.CreateTrigger(region, true);
            var requestID = Guid.NewGuid().ToString("N");
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
            UNUserNotificationCenter.Current.AddNotificationRequest(request, e => { });
         }

         return true;
      }
   }
}

