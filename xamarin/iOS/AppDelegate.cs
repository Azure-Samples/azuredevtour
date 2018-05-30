using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Microsoft.Identity.Client;


namespace Reviewer.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new PhotoTour.Core.App());

			return base.FinishedLaunching(app, options);
		}

		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		{
			AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);

			return true;
		}
	}
}
