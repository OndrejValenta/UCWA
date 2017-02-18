using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NLog;

namespace UCWA.Provider
{
	public class UCWAConnector
	{
		static ILogger logger = LogManager.GetCurrentClassLogger();

		private HttpClient httpClient;
		string ucwaHostName = string.IsNullOrEmpty(System.Net.Dns.GetHostName())?"ondrejvalenta.com": System.Net.Dns.GetHostName();

		string ucwaSipDomain = "ondrejvalenta.com";
		string ucwaSipAddress = "me@ondrejvalenta.com";
		string ucwaAdDomain = "";
		string ucwaAdUser = "";
		string ucwaAdPassword = "";

		string ucwaAccessLocation = "";
		string ucwaExternalAutodiscover = "";
		string ucwaUrlUser = "";
		string ucwaUrlOAuth = "";
		string ucwaUrlSelf = "";
		string ucwaWwwAuthenticate = "";
		string ucwaMsRtcOAuth = "";
		string ucwaAccessToken = "";
		string ucwaTokenType = "";
		string ucwaApplications = "";
		string ucwaApplication = "";
		string ucwaEvents = "";
		string ucwaMe = "";
		string ucwaNote = "";
		string ucwaLocation = "";
		string ucwaPresence = "";
		string ucwaReportMyActivity = "";
		string ucwaMakeMeAvailable = "";
		string ucwaMessagingInvitations = "";
		string meLocation = "";
		string meNote = "";
		string mePresence = "";
		string ucwaConversation = "";
		string ucwaMessaging = "";
		string ucwaStopMessaging = "";
		string ucwaStartPhoneAudio = "";
		string ucwaOperationID = "";
		string ucwaDestination = "";
		string ucwaSendImSubject = "";
		string ucwaSendImBody = "";
		string ucwaCallViaWorkSubject = "";
		string ucwaCallViaWorkFrom = "";
		string ucwaCallViaWorkTo = "";

		string sipServerInternalAccess = "";
		string sipClientInternalAccess = "";
		string sipServerExternalAccess = "";
		string sipClientExternalAccess = "";

		public UCWAConnector()
		{
			
			WebRequestHandler webRequestHandler = new WebRequestHandler();
			webRequestHandler.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
			{
				logger.Warn($"Invalid certificate: {certificate.Subject}");
				return true;
			};

			httpClient = new HttpClient(webRequestHandler);

		}

		public async void Logon_Step00()
		{

			string url_00 = "https://lyncdiscoverinternal." + ucwaSipDomain + "/";
			httpClient.DefaultRequestHeaders.Clear();

			
			logger.Info(String.Format("Step 00 : GET : {0}", url_00));

			try
			{
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.rtc.autodiscover+xml;v=1");

				logger.Debug(String.Format(">> Request: {0}", "GET"));
				logger.Debug(String.Format(">> URL: {0}", url_00));
				logger.Debug(String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));

				
				var res_00 = await httpClient.GetAsync(url_00);
				string res_00_request = res_00.RequestMessage.ToString();
				string res_00_headers = res_00.Headers.ToString();
				string res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(String.Format(">> Response: {0}", res_00_status));
				logger.Debug(String.Format("{0}", res_00_headers));
				logger.Debug(String.Format("\r\n{0}", res_00_content));

				if (res_00_status == "OK")
				{
					XmlTextReader xml00 = new XmlTextReader(new StringReader(res_00_content));
					while (xml00.Read())
					{
						switch (xml00.Name)
						{
							case "AutodiscoverResponse":
								if (xml00.GetAttribute("AccessLocation") != null)
								{
									ucwaAccessLocation = xml00.GetAttribute("AccessLocation");
									logger.Debug(String.Format("<< ucwaAccessLocation : {0}", ucwaAccessLocation));
								}
								break;
							case "Link":
								switch (xml00.GetAttribute("token"))
								{
									case "User": // User ":  
										ucwaUrlUser = xml00.GetAttribute("href");
										logger.Debug(String.Format("<< ucwaUrlUser : {0}", ucwaUrlUser));
										break;
									case "OAuth": // OAuth":  
										ucwaUrlOAuth = xml00.GetAttribute("href");
										logger.Debug(String.Format("<< ucwaUrlOAuth : {0}", ucwaUrlOAuth));
										break;
									default:
										break;
								}
								break;
							default:
								break;
						}
					}
					Logon_Step02();
				}
				else
				{
					logger.Info(String.Format(">> Error in step 00. {0}", "No OK received"));
					Logon_Step01();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(String.Format(">> {0}", ex.InnerException.Message));
				Logon_Step01();
			}

		}

		async public void Logon_Step01()
		{
			httpClient.DefaultRequestHeaders.Clear();

			string url_00 = "https://lyncdiscover." + ucwaSipDomain + "/";
			logger.Info(String.Format("Step 01 : GET : {0}", url_00));

			try
			{
				httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.rtc.autodiscover+xml;v=1");

				logger.Debug(String.Format(">> Request: {0}", "GET"));
				logger.Debug(String.Format(">> URL: {0}", url_00));
				logger.Debug(String.Format("\r\n{0}", httpClient.DefaultRequestHeaders));

				var res_00 = await httpClient.GetAsync(url_00);
				string res_00_request = res_00.RequestMessage.ToString();
				string res_00_headers = res_00.Headers.ToString();
				string res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(String.Format(">> Response: {0}", res_00_status));
				logger.Debug(String.Format("{0}", res_00_headers));
				logger.Debug(String.Format("\r\n{0}", res_00_content));

				if (res_00_status == "OK")
				{
					XmlTextReader xml01 = new XmlTextReader(new StringReader(res_00_content));
					while (xml01.Read())
					{
						switch (xml01.Name)
						{
							case "AutodiscoverResponse":
								if (xml01.GetAttribute("AccessLocation") != null)
								{
									ucwaAccessLocation = xml01.GetAttribute("AccessLocation");
									logger.Debug(String.Format("<< ucwaAccessLocation : {0}", ucwaAccessLocation));
								}
								break;
							case "Link":
								switch (xml01.GetAttribute("token"))
								{
									case "User": // User ":  
										ucwaUrlUser = xml01.GetAttribute("href");
										logger.Debug(String.Format("<< ucwaUrlUser : {0}", ucwaUrlUser));
										break;
									case "OAuth": // OAuth":  
										ucwaUrlOAuth = xml01.GetAttribute("href");
										logger.Debug(String.Format("<< ucwaUrlOAuth : {0}", ucwaUrlOAuth));
										break;
									default:
										break;
								}
								break;
							default:
								break;
						}
					}
					Logon_Step02();
				}
				else
				{
					logger.Info(String.Format(">> Error in step 01. {0}", "No OK received"));
					logger.Info(String.Format("Error. Logon ended abnormally."));
					logger.Error(String.Format("Error. Logon ended abnormally."));
				}
			}
			catch (Exception ex)
			{
				logger.Info(String.Format(">> Error in step 01. {0}", ex.InnerException.Message));
				logger.Info(String.Format("Error. Logon ended abnormally."));
				logger.Error(String.Format("Error. Logon ended abnormally."));
			}

		}

		async public void Logon_Step02()
		{
			try
			{
				//string url_00 = ucwaUrlOAuth; // needed for Lync Server 2013  
				string url_00 = ucwaUrlUser; // needed for Skype for Business Server 2015  
				logger.Info(String.Format("Step 02 : GET : {0}", url_00));

				logger.Debug(String.Format(">> Request: {0}", "GET"));
				logger.Debug(String.Format(">> URL: {0}", url_00));
				logger.Debug(String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));

				var res_00 = await httpClient.GetAsync(url_00);

				string res_00_request = res_00.RequestMessage.ToString();
				string res_00_headers = res_00.Headers.ToString();
				string res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(String.Format(">> Response: {0}", res_00_status));
				logger.Debug(String.Format("{0}", res_00_headers));
				logger.Debug(String.Format("\r\n{0}", res_00_content));

				if (res_00_status == "Unauthorized")
				{
					// using string manipulation  
					string[] res_00_A = res_00_headers.Split('\n');
					foreach (string res_00_a in res_00_A)
					{
						// ucwaWwwAuthenticate = "Bearer trusted_issuers=\"\", client_id=\"00000004-0000-0ff1-ce00-000000000000\", MsRtcOAuth href=\"https://lync01.missiaen.local/WebTicket/oauthtoken\",grant_type=\"urn:microsoft.rtc:windows,urn:microsoft.rtc:anonmeeting,password\"\r"  
						if (res_00_a.StartsWith("WWW-Authenticate:"))
						{
							ucwaWwwAuthenticate = res_00_a.Substring(18);
							string[] res_00_B = ucwaWwwAuthenticate.Split(',');
							foreach (string res_00_b in res_00_B)
							{
								if (res_00_b.StartsWith(" MsRtcOAuth href="))
								{
									ucwaMsRtcOAuth = res_00_b.Substring(17).Replace("\\", "").Replace("\"", "");
									logger.Debug(String.Format("<< ucwaMsRtcOAuth : {0}", ucwaMsRtcOAuth));
								}
							}

						}
					}
					Logon_Step03();
				}
				else
				{
					logger.Info(String.Format(">> Error in step 02. {0}", "No Unauthorized received"));
					logger.Info(String.Format("Error. Logon ended abnormally."));
					logger.Error(String.Format("Error. Logon ended abnormally."));
				}
			}
			catch (Exception ex)
			{
				logger.Info(String.Format(">> Error in step 02. {0}", ex.InnerException.Message));
				logger.Info(String.Format("Error. Logon ended abnormally."));
				logger.Error(String.Format("Error. Logon ended abnormally."));
			}
		}

		async public void Logon_Step03()
		{
			try
			{
				string url_00 = ucwaMsRtcOAuth;
				logger.Info(String.Format("Step 03 : POST : {0}", url_00));
				var authDic_00 = new Dictionary<string, string> ();
				authDic_00.Add("grant_type", "password");
				authDic_00.Add("username", String.Format("{0}\\{1}", ucwaAdDomain, ucwaAdUser));
				authDic_00.Add("password", ucwaAdPassword);

				httpClient.DefaultRequestHeaders.Remove("Accept");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

				logger.Debug(String.Format(">> Request: {0}", "POST"));
				logger.Debug(String.Format(">> URL: {0}", url_00));
				logger.Debug(String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));
				logger.Debug(String.Format(">> {0}={1}", "grant_type", "password"));
				logger.Debug(String.Format(">> {0}={1}", "username", String.Format("{0}\\{1}", ucwaAdDomain, ucwaAdUser)));
				logger.Debug(String.Format(">> {0}={1}", "password", ucwaAdPassword));

				var res_00 = await httpClient.PostAsync(url_00, new FormUrlEncodedContent(authDic_00));

				string res_00_request = res_00.RequestMessage.ToString();
				string res_00_headers = res_00.Headers.ToString();
				string res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(String.Format(">> Response: {0}", res_00_status));
				logger.Debug(String.Format("{0}", res_00_headers));
				logger.Debug(String.Format("\r\n{0}", res_00_content));

				if (res_00_status == "OK")
				{
					//DataContractJsonSerializer json03 = new DataContractJsonSerializer(typeof(Step03Result));
					//MemoryStream stream03 = new MemoryStream(Encoding.UTF8.GetBytes(res_00_content));
					//Step03Result objStep03Result = (Step03Result)json03.ReadObject(stream03);
					//stream03.Close();
					//ucwaAccessToken = objStep03Result.access_token;
					//ucwaTokenType = objStep03Result.token_type;
					//logger.Debug(String.Format("<< ucwaAccessToken : {0}", ucwaAccessToken));
					//logger.Debug(String.Format("<< ucwaTokenType : {0}", ucwaTokenType));
					Logon_Step04();
				}
				else
				{
					logger.Info(String.Format(">> Error in step 03. {0}", res_00_content));
					logger.Info(String.Format("Error. Logon ended abnormally."));
					logger.Error(String.Format("Error. Logon ended abnormally."));
				}
			}
			catch (Exception ex)
			{
				logger.Info(String.Format(">> Error in step 03. {0}", ex.InnerException.Message));
				logger.Info(String.Format("Error. Logon ended abnormally."));
				logger.Error(String.Format("Error. Logon ended abnormally."));
			}

		}

		async public void Logon_Step04()
		{
			try
			{
				httpClient.DefaultRequestHeaders.Remove("Accept");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.rtc.autodiscover+xml;v=1");

				string url_00 = ucwaUrlOAuth;
				logger.Info(String.Format("Step 04 : GET : {0}", url_00));
				httpClient.DefaultRequestHeaders.Remove("Authorisation"); // for recurvice  
				httpClient.DefaultRequestHeaders.Add("Authorization",
						ucwaTokenType + " " + ucwaAccessToken);

				logger.Debug(String.Format(">> Request: {0}", "GET"));
				logger.Debug(String.Format(">> URL: {0}", url_00));
				logger.Debug(String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));

				var res_00 = await httpClient.GetAsync(url_00);

				string res_00_request = res_00.RequestMessage.ToString();
				string res_00_headers = res_00.Headers.ToString();
				string res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(String.Format(">> Response: {0}", res_00_status));
				logger.Debug(String.Format("{0}", res_00_headers));
				logger.Debug(String.Format("\r\n{0}", res_00_content));

				if (res_00_status == "OK")
				{

					XmlTextReader xml04 = new XmlTextReader(new StringReader(res_00_content));

					while (xml04.Read())
					{
						switch (xml04.Name)
						{
							case "AutodiscoverResponse":
								if (xml04.GetAttribute("AccessLocation") != null)
								{
									ucwaAccessLocation = xml04.GetAttribute("AccessLocation");
									logger.Debug(String.Format("<< ucwaAccessLocation : {0}", ucwaAccessLocation));
								}
								break;
							case "SipServerInternalAccess":
								if (xml04.GetAttribute("fqdn") != null)
								{
									sipServerInternalAccess = xml04.GetAttribute("fqdn") + ":" + xml04.GetAttribute("port");
									logger.Debug(String.Format("<< sipServerInternalAccess : {0}", sipServerInternalAccess));
								}
								break;
							case "SipClientInternalAccess":
								if (xml04.GetAttribute("fqdn") != null)
								{
									sipClientInternalAccess = xml04.GetAttribute("fqdn") + ":" + xml04.GetAttribute("port");
									logger.Debug(String.Format("<< sipCientInternalAccess : {0}", sipClientInternalAccess));
								}
								break;
							case "SipServerExternalAccess":
								if (xml04.GetAttribute("fqdn") != null)
								{
									sipServerExternalAccess = xml04.GetAttribute("fqdn") + ":" + xml04.GetAttribute("port");
									logger.Debug(String.Format("<< sipServerExternalAccess : {0}", sipServerExternalAccess));
								}
								break;
							case "SipClientExternalAccess":
								if (xml04.GetAttribute("fqdn") != null)
								{
									sipClientExternalAccess = xml04.GetAttribute("fqdn") + ":" + xml04.GetAttribute("port");
									logger.Debug(String.Format("<< sipCientExternalAccess : {0}", sipClientExternalAccess));
								}
								break;
							case "Link":
								switch (xml04.GetAttribute("token"))
								{
									case "Internal/Ucwa":
										if (ucwaAccessLocation == "Internal")
										{
											ucwaApplications = xml04.GetAttribute("href");
											logger.Debug(String.Format("<< ucwaApplications : {0}", ucwaApplications));
										}
										break;
									case "External/Ucwa":
										if (ucwaAccessLocation == "External")
										{
											ucwaApplications = xml04.GetAttribute("href");
											logger.Debug(String.Format("<< ucwaApplications : {0}", ucwaApplications));
										}
										break;
									case "External/Autodiscover":
										if (ucwaAccessLocation == "External")
										{
											ucwaExternalAutodiscover = xml04.GetAttribute("href");
											logger.Debug(String.Format("<< ucwaExternalAutodiscover : {0}", ucwaExternalAutodiscover));
										}
										break;
									case "Self":
										ucwaUrlSelf = xml04.GetAttribute("href");
										logger.Debug(String.Format("<< ucwaUrlSelf : {0}", ucwaUrlSelf));
										break;

									default:
										break;
								}
								break;
							default:
								break;
						}

					}
					Logon_Step05();
				}
				else
				{
					logger.Info(String.Format(">> Error in step 04. {0}", "No OK received"));
					logger.Info(String.Format("Error. Logon ended abnormally."));
				}
			}
			catch (Exception ex)
			{
				logger.Info(String.Format(">> Error in step 04. {0}", ex.InnerException.Message));
				logger.Info(String.Format("Error. Logon ended abnormally."));
				logger.Error(String.Format("Error. Logon ended abnormally."));
			}
		}

		async public void Logon_Step05()
		{
			if (ucwaUrlUser.StartsWith(ucwaUrlSelf))
			{
			}
			else
			{
				logger.Info(String.Format("Environment contains a Director or Multiple Pools with user registered to different pool..."));
				ucwaUrlOAuth = String.Format("{0}/?originalDomain={1}", ucwaUrlSelf, ucwaSipDomain);
				logger.Debug(String.Format("<< ucwaUrlOAuth : {0}", ucwaUrlOAuth));
				httpClient.DefaultRequestHeaders.Remove("Authorization");
				ucwaUrlUser = ucwaUrlSelf;
				Logon_Step02();
				return;
			}
			try
			{
				httpClient.DefaultRequestHeaders.Remove("Accept");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

				string url_00 = ucwaApplications;

				logger.Info(String.Format("Step 05 : POST : {0}", url_00));

				string guiId = Guid.NewGuid().ToString();
				HttpContent json_00 = new StringContent("{\"userAgent\":\"UCWA Francis Missiaen\",\"endpointId\":\""
				+ guiId + "\",\"culture\":\"en-US\"}", Encoding.UTF8, "application/json");

				logger.Debug(String.Format(">> Request: {0}", "POST"));
				logger.Debug(String.Format(">> URL: {0}", url_00));
				logger.Debug(String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));
				logger.Debug(String.Format(">> {0}={1}", "userAgent", "UCWA Francis Missiaen"));
				logger.Debug(String.Format(">> {0}={1}", "endpointId", guiId));
				logger.Debug(String.Format(">> {0}={1}", "culture", "en-US"));

				var res_00 = await httpClient.PostAsync(url_00, json_00);

				string res_00_request = res_00.RequestMessage.ToString();
				string res_00_headers = res_00.Headers.ToString();
				string res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(String.Format(">> Response: {0}", res_00_status));
				logger.Debug(String.Format("{0}", res_00_headers));
				logger.Debug(String.Format("\r\n{0}", res_00_content));

				if (res_00_status == "Created")
				{
					XmlTextReader xml05 = new XmlTextReader(new StringReader(res_00_content));
					while (xml05.Read())
					{
						switch (xml05.GetAttribute("rel"))
						{
							case "application":
								ucwaApplication = xml05.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaApplication : {0}", ucwaApplication));
								break;
							case "me":
								ucwaMe = xml05.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaMe : {0}", ucwaMe));
								break;
							case "events":
								ucwaEvents = xml05.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaEvents : {0}", ucwaEvents));
								break;
							case "makeMeAvailable":
								ucwaMakeMeAvailable = xml05.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaMakeMeAvailable : {0}", ucwaMakeMeAvailable));
								break;
							case "startMessaging":
								ucwaMessagingInvitations = xml05.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaMessagingInvitations : {0}", ucwaMessagingInvitations));
								break;
							case "startPhoneAudio":
								ucwaStartPhoneAudio = xml05.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaStartPhoneAudio : {0}", ucwaStartPhoneAudio));
								break;
							default:
								break;
						}
					}
					Logon_Step06();
				}
				else
				{
					logger.Info(String.Format(">> Error in step 05. {0} - {1}", "No OK received", res_00_status));
					logger.Info(String.Format("Error. Logon ended abnormally."));
					logger.Error(String.Format("Error. Logon ended abnormally."));
				}
			}
			catch (Exception ex)
			{
				logger.Info(String.Format(">> Error in step 05. {0}", ex.InnerException.Message));
				logger.Info(String.Format("Error. Logon ended abnormally."));
				logger.Error(String.Format("Error. Logon ended abnormally."));
			}
		}

		async public void Logon_Step06()
		{
			try
			{
				string url_00 = ucwaApplications + ucwaMakeMeAvailable.Substring(27);
				logger.Info(String.Format("Step 06 : POST : {0}", url_00));

				httpClient.DefaultRequestHeaders.Remove("Accept");
				HttpContent json_06 = new StringContent("{\"SupportedModalities\":[\"Messaging\"]}", Encoding.UTF8, "application/json");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

				logger.Debug(String.Format(">> Request: {0}", "POST"));
				logger.Debug(String.Format(">> URL: {0}", url_00));
				logger.Debug(String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));
				logger.Debug(String.Format(">> {0}={1}", "SupportedModalities", "Messaging"));

				var res_00 = await httpClient.PostAsync(url_00, json_06);

				string res_00_request = res_00.RequestMessage.ToString();
				string res_00_headers = res_00.Headers.ToString();
				string res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(String.Format(">> Response: {0}", res_00_status));
				logger.Debug(String.Format("{0}", res_00_headers));
				logger.Debug(String.Format("\r\n{0}", res_00_content));

				if (res_00_status == "NoContent")
				{
					Logon_Step07();
				}
				else
				{
					logger.Info(String.Format(">> Error in step 06. {0}", "No OK received"));
					logger.Info(String.Format("Error. Logon ended abnormally."));
				}
			}
			catch (Exception ex)
			{
				logger.Info(String.Format(">> Error in step 06. {0}", ex.InnerException.Message));
				logger.Info(String.Format("Error. Logon ended abnormally."));
			}
		}

		async public void Logon_Step07()
		{
			try
			{
				httpClient.DefaultRequestHeaders.Remove("Accept");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

				string url_00 = ucwaApplications + ucwaApplication.Substring(27);
				logger.Info(String.Format("Step 07 : GET : {0}", url_00));

				logger.Debug(String.Format(">> Request: {0}", "GET"));
				logger.Debug(String.Format(">> URL: {0}", url_00));
				logger.Debug(String.Format("\r\n{0}", httpClient.DefaultRequestHeaders.ToString()));

				var res_00 = await httpClient.GetAsync(url_00);

				string res_00_request = res_00.RequestMessage.ToString();
				string res_00_headers = res_00.Headers.ToString();
				string res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(String.Format(">> Response: {0}", res_00_status));
				logger.Debug(String.Format("{0}", res_00_headers));
				logger.Debug(String.Format("\r\n{0}", res_00_content));

				if (res_00_status == "OK")
				{
					string XML = String.Format(res_00_content);
					//webBrowser_XML.Invoke(new Action(() =>
					//		webBrowser_XML.DocumentText = XML));
					XmlTextReader xml07 = new XmlTextReader(new StringReader(res_00_content));
					while (xml07.Read())
					{
						switch (xml07.GetAttribute("rel"))
						{
							case "note":
								ucwaNote = xml07.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaNote : {0}", ucwaNote));
								break;
							case "location":
								ucwaLocation = xml07.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaLocation : {0}", ucwaLocation));
								break;
							case "presence":
								ucwaPresence = xml07.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaPresence : {0}", ucwaPresence));
								break;
							case "reportMyActivity":
								ucwaReportMyActivity = xml07.GetAttribute("href");
								logger.Debug(String.Format("<< ucwaReportMyActivity : {0}", ucwaReportMyActivity));
								break;
							default:
								break;
						}
						if (xml07.Name == "link")
						{
							logger.Info($"XML07-href value {xml07.GetAttribute("href")}");
						}
					}
					// <property name="1d6bd019-52ce-4ebb-9147-b43f5420d3a6">please pass this in a PUT request</property>  
					ucwaOperationID = "1d6bd019-52ce-4ebb-9147-b43f5420d3a6";
					int i = res_00_content.ToString().IndexOf("please pass this in a PUT request");
					if (i > 40)
					{
						ucwaOperationID = res_00_content.ToString().Substring(i - 38, 36);
						logger.Debug(String.Format("<< ucwaOperationID : {0}", ucwaOperationID));
					}
					Logon_Step08();
				}
				else
				{
					logger.Info(String.Format(">> Error in step 07. {0}", "No OK received"));
					
					logger.Error(String.Format("Error. Logon ended abnormally."));
				}
			}
			catch (Exception ex)
			{
				logger.Info(String.Format(">> Error in step 07. {0}", ex.InnerException.Message));
				
				logger.Error(String.Format("Error. Logon ended abnormally."));
			}

		}

		public void Logon_Step08()
		{
			logger.Info(String.Format("Logon completed normally."));
			
		}
	}
}
