namespace UCWA.Provider.Models
{
	public class Discovery
	{
		// {"_links":{"self":{"href":"https://webdir0e.online.lync.com/Autodiscover/AutodiscoverService.svc/root?originalDomain=ondrejvalenta.com"},"xframe":{"href":"https://webdir1E.online.lync.com/Autodiscover/AutodiscoverService.svc/root/xframe"},"redirect":{"href":"https://webdir1E.online.lync.com/Autodiscover/AutodiscoverService.svc/root?originalDomain=ondrejvalenta.com"}}}
		public Links _links { get; set; }

	}


	public class Self
	{
		public string href { get; set; }
	}

	public class Xframe
	{
		public string href { get; set; }
	}

	public class Redirect
	{
		public string href { get; set; }
	}

	public class User
	{
		public string href { get; set; }
	}

	public class OAuth
	{
		public string href { get; set; }
	}

	

	public class Links
	{
		public Self self { get; set; }
		public Xframe xframe { get; set; }
		public Redirect redirect { get; set; }
		public User user { get; set; }
		public OAuth oauth { get; set; }
	}
}