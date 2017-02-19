using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using NLog;
using UCWA.Provider.Models;

namespace UCWA.Provider
{
	public class UCWAConnector
	{
		private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

		private readonly HttpClient httpClient;
		private string meLocation = "";
		private string meNote = "";
		private string mePresence = "";
		private string sipClientExternalAccess = "";
		private string sipClientInternalAccess = "";
		private string sipServerExternalAccess = "";

		private string sipServerInternalAccess = "";

		private string ucwaAccessLocation = "";
		private string ucwaAccessToken = "";
		private string ucwaAdDomain = "";
		private string ucwaAdPassword = "";
		private string ucwaAdUser = "me";
		private string ucwaApplication = "";
		private string ucwaApplications = "";
		private string ucwaCallViaWorkFrom = "";
		private string ucwaCallViaWorkSubject = "";
		private string ucwaCallViaWorkTo = "";
		private string ucwaConversation = "";
		private string ucwaDestination = "";
		private string ucwaEvents = "";
		private string ucwaExternalAutodiscover = "";
		private string ucwaHostName = string.IsNullOrEmpty(Dns.GetHostName()) ? "ondrejvalenta.com" : Dns.GetHostName();
		private string ucwaLocation = "";
		private string ucwaMakeMeAvailable = "";
		private string ucwaMe = "";
		private string ucwaMessaging = "";
		private string ucwaMessagingInvitations = "";
		private string ucwaMsRtcOAuth = "";
		private string ucwaNote = "";
		private string ucwaOperationID = "";
		private string ucwaPresence = "";
		private string ucwaReportMyActivity = "";
		private string ucwaSendImBody = "";
		private string ucwaSendImSubject = "";
		private string ucwaSipAddress = "me@ondrejvalenta.com";

		private readonly string ucwaSipDomain = "ondrejvalenta.com";
		private string ucwaStartPhoneAudio = "";
		private string ucwaStopMessaging = "";
		private readonly string ucwaTokenType = "";
		private string ucwaUrlOAuth = "";
		private string ucwaUrlSelf = "";
		private string ucwaUrlUser = "";
		private string ucwaWwwAuthenticate = "";

		public UCWAConnector()
		{
			var webRequestHandler = new WebRequestHandler();
			webRequestHandler.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
			{
				logger.Warn($"Invalid certificate: {certificate.Subject}");
				return true;
			};

			httpClient = new HttpClient(webRequestHandler);
		}

		private async Task<Response<Discovery>> callDiscoveryService(string url)
		{
			httpClient.DefaultRequestHeaders.Clear();

			httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			httpClient.DefaultRequestHeaders.Add("Accept", "text/html, application/xhtml+xml, */*");
			httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.7,cs;q=0.3");


			logger.Debug($"Calling url: {url} with headers: {httpClient.DefaultRequestHeaders}");
			var r = await httpClient.GetAsync(url).ConfigureAwait(false); ;
			logger.Trace($"Request message: {r.RequestMessage}");
			logger.Trace($"Response code: {r.StatusCode}");
			logger.Trace($"Response headers: {r.Headers}");

			var response = new Response<Discovery>();

			response.StatusCode = r.StatusCode;
			response.Successful = r.IsSuccessStatusCode;

			response.ResponseHeaders = r.Headers;

			var content = await r.Content.ReadAsStringAsync();

			if (response.Successful)
			{
				response.Data = JsonConvert.DeserializeObject<Discovery>(content);
			}


			return response;
		}

		//private async object doWebCall()
		//{
			
		//}


		public async void Logon_Step00()
		{
			var url_00 = "https://lyncdiscoverinternal." + ucwaSipDomain + "/";
			httpClient.DefaultRequestHeaders.Clear();


			logger.Info("Step 00 : GET : {0}", url_00);

			try
			{
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.rtc.autodiscover+xml;v=1");

				logger.Debug(">> Request: {0}", "GET");
				logger.Debug(">> URL: {0}", url_00);
				logger.Debug("\r\n{0}", httpClient.DefaultRequestHeaders);


				var res_00 = await httpClient.GetAsync(url_00);
				var res_00_request = res_00.RequestMessage.ToString();
				var res_00_headers = res_00.Headers.ToString();
				var res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(">> Response: {0}", res_00_status);
				logger.Debug("{0}", res_00_headers);
				logger.Debug("\r\n{0}", res_00_content);

				if (res_00_status == "OK")
				{
					var xml00 = new XmlTextReader(new StringReader(res_00_content));
					while (xml00.Read())
						switch (xml00.Name)
						{
							case "AutodiscoverResponse":
								if (xml00.GetAttribute("AccessLocation") != null)
								{
									ucwaAccessLocation = xml00.GetAttribute("AccessLocation");
									logger.Debug("<< ucwaAccessLocation : {0}", ucwaAccessLocation);
								}
								break;
							case "Link":
								switch (xml00.GetAttribute("token"))
								{
									case "User": // User ":  
										ucwaUrlUser = xml00.GetAttribute("href");
										logger.Debug("<< ucwaUrlUser : {0}", ucwaUrlUser);
										break;
									case "OAuth": // OAuth":  
										ucwaUrlOAuth = xml00.GetAttribute("href");
										logger.Debug("<< ucwaUrlOAuth : {0}", ucwaUrlOAuth);
										break;
									default:
										break;
								}
								break;
							default:
								break;
						}
					Logon_Step02();
				}
				else
				{
					logger.Info(">> Error in step 00. {0}", "No OK received");
					Logon_Step01();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(">> {0}", ex.InnerException.Message);
				Logon_Step01();
			}
		}

		public async void Logon_Step01()
		{
			var url_00 = "https://lyncdiscover." + ucwaSipDomain + "/";
			logger.Info("Step 01 : GET : {0}", url_00);

			try
			{
				Response<Discovery> discovery = await callDiscoveryService(url_00);


				logger.Debug(">> Request: {0}", "GET");
				logger.Debug(">> URL: {0}", url_00);
				logger.Debug("\r\n{0}", httpClient.DefaultRequestHeaders);

				if (discovery.Successful)
				{
					Links links = discovery.Data._links;
					if (links.user != null)
					{
						ucwaUrlUser = links.user.href;
					} else if (links.oauth != null)
					{
						ucwaUrlOAuth = links.oauth.href;
					}
					else if(links.redirect != null)
					{
						var redirectedDiscovery = await callDiscoveryService(links.redirect.href).ConfigureAwait(false);

						if (redirectedDiscovery.Data._links.user != null)
						{
							ucwaUrlUser = redirectedDiscovery.Data._links.user.href;
						}
						else
						{
							logger.Warn("Could not load user web service url. TERMINATING");
							return;
						}
					}


					//var xml01 = new XmlTextReader(new StringReader(res_00_content));
					//while (xml01.Read())
					//	switch (xml01.Name)
					//	{
					//		case "AutodiscoverResponse":
					//			if (xml01.GetAttribute("AccessLocation") != null)
					//			{
					//				ucwaAccessLocation = xml01.GetAttribute("AccessLocation");
					//				logger.Debug("<< ucwaAccessLocation : {0}", ucwaAccessLocation);
					//			}
					//			break;
					//		case "Link":
					//			switch (xml01.GetAttribute("token"))
					//			{
					//				case "User": // User ":  
					//					ucwaUrlUser = xml01.GetAttribute("href");
					//					logger.Debug("<< ucwaUrlUser : {0}", ucwaUrlUser);
					//					break;
					//				case "OAuth": // OAuth":  
					//					ucwaUrlOAuth = xml01.GetAttribute("href");
					//					logger.Debug("<< ucwaUrlOAuth : {0}", ucwaUrlOAuth);
					//					break;
					//				default:
					//					break;
					//			}
					//			break;
					//		default:
					//			break;
					//	}
					Logon_Step02();
				}
				else
				{
					logger.Info(">> Error in step 01. {0}", "No OK received");
					logger.Info("Error. Logon ended abnormally.");
					logger.Error("Error. Logon ended abnormally.");
				}
			}
			catch (Exception ex)
			{
				logger.Info(">> Error in step 01. {0}", ex.InnerException.Message);
				logger.Info("Error. Logon ended abnormally.");
				logger.Error("Error. Logon ended abnormally.");
			}
		}

		public async void Logon_Step02()
		{
			try
			{
				//string url_00 = ucwaUrlOAuth; // needed for Lync Server 2013  
				var url_00 = ucwaUrlUser; // needed for Skype for Business Server 2015  
				logger.Info("Step 02 : GET : {0}", url_00);

				logger.Debug(">> Request: {0}", "GET");
				logger.Debug(">> URL: {0}", url_00);
				logger.Debug("\r\n{0}", httpClient.DefaultRequestHeaders);

				var res_00 = await httpClient.GetAsync(url_00);

				var res_00_request = res_00.RequestMessage.ToString();
				var res_00_headers = res_00.Headers;
				var res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(">> Response: {0}", res_00_status);
				logger.Debug("{0}", res_00_headers);
				logger.Debug("\r\n{0}", res_00_content);

				if (res_00_status == "Unauthorized")
				{
					// using string manipulation  
							ucwaWwwAuthenticate = res_00_headers.WwwAuthenticate.ToString().Substring(18);
					var authDict =
						ucwaWwwAuthenticate.Split(',')
							.Where(x => x.Contains("="))
							.Select(x => new {key = x.Substring(0, x.IndexOf('=')).Trim(), value = x.Substring(x.IndexOf('=') + 1).Trim('"')})
							.ToDictionary(x => x.key, x => x.value);

					ucwaMsRtcOAuth = authDict["MsRtcOAuth href"];
							

					Logon_Step03();
				}
				else
				{
					logger.Info(">> Error in step 02. {0}", "No Unauthorized received");
					logger.Info("Error. Logon ended abnormally.");
					logger.Error("Error. Logon ended abnormally.");
				}
			}
			catch (Exception ex)
			{
				

				logger.Info(">> Error in step 02. {0}", ex.InnerException?.Message);
				logger.Info("Error. Logon ended abnormally.");
				logger.Error("Error. Logon ended abnormally.");
			}
		}

		public async void Logon_Step03()
		{
			try
			{
				var url_00 = ucwaMsRtcOAuth;
				logger.Info("Step 03 : POST : {0}", url_00);
				var authDic_00 = new Dictionary<string, string>();
				authDic_00.Add("grant_type", "urn:microsoft.rtc:passive");
				authDic_00.Add("username", string.Format("{0}\\{1}", ucwaAdDomain, ucwaAdUser));
				authDic_00.Add("password", ucwaAdPassword);

				httpClient.DefaultRequestHeaders.Remove("Accept");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

				logger.Debug(">> Request: {0}", "POST");
				logger.Debug(">> URL: {0}", url_00);
				logger.Debug("\r\n{0}", httpClient.DefaultRequestHeaders);
				logger.Debug(">> {0}={1}", "grant_type", "password");
				logger.Debug(">> {0}={1}", "username", string.Format("{0}\\{1}", ucwaAdDomain, ucwaAdUser));
				logger.Debug(">> {0}={1}", "password", ucwaAdPassword);

				var res_00 = await httpClient.PostAsync(url_00, new FormUrlEncodedContent(authDic_00));

				var res_00_request = res_00.RequestMessage.ToString();
				var res_00_headers = res_00.Headers.ToString();
				var res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(">> Response: {0}", res_00_status);
				logger.Debug("{0}", res_00_headers);
				logger.Debug("\r\n{0}", res_00_content);

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
					logger.Info(">> Error in step 03. {0}", res_00_content);
					logger.Info("Error. Logon ended abnormally.");
					logger.Error("Error. Logon ended abnormally.");
				}
			}
			catch (Exception ex)
			{
				logger.Info(">> Error in step 03. {0}", ex.InnerException.Message);
				logger.Info("Error. Logon ended abnormally.");
				logger.Error("Error. Logon ended abnormally.");
			}
		}

		public async void Logon_Step04()
		{
			try
			{
				httpClient.DefaultRequestHeaders.Remove("Accept");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.rtc.autodiscover+xml;v=1");

				var url_00 = ucwaUrlOAuth;
				logger.Info("Step 04 : GET : {0}", url_00);
				httpClient.DefaultRequestHeaders.Remove("Authorisation"); // for recurvice  
				httpClient.DefaultRequestHeaders.Add("Authorization",
					ucwaTokenType + " " + ucwaAccessToken);

				logger.Debug(">> Request: {0}", "GET");
				logger.Debug(">> URL: {0}", url_00);
				logger.Debug("\r\n{0}", httpClient.DefaultRequestHeaders);

				var res_00 = await httpClient.GetAsync(url_00);

				var res_00_request = res_00.RequestMessage.ToString();
				var res_00_headers = res_00.Headers.ToString();
				var res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(">> Response: {0}", res_00_status);
				logger.Debug("{0}", res_00_headers);
				logger.Debug("\r\n{0}", res_00_content);

				if (res_00_status == "OK")
				{
					var xml04 = new XmlTextReader(new StringReader(res_00_content));

					while (xml04.Read())
						switch (xml04.Name)
						{
							case "AutodiscoverResponse":
								if (xml04.GetAttribute("AccessLocation") != null)
								{
									ucwaAccessLocation = xml04.GetAttribute("AccessLocation");
									logger.Debug("<< ucwaAccessLocation : {0}", ucwaAccessLocation);
								}
								break;
							case "SipServerInternalAccess":
								if (xml04.GetAttribute("fqdn") != null)
								{
									sipServerInternalAccess = xml04.GetAttribute("fqdn") + ":" + xml04.GetAttribute("port");
									logger.Debug("<< sipServerInternalAccess : {0}", sipServerInternalAccess);
								}
								break;
							case "SipClientInternalAccess":
								if (xml04.GetAttribute("fqdn") != null)
								{
									sipClientInternalAccess = xml04.GetAttribute("fqdn") + ":" + xml04.GetAttribute("port");
									logger.Debug("<< sipCientInternalAccess : {0}", sipClientInternalAccess);
								}
								break;
							case "SipServerExternalAccess":
								if (xml04.GetAttribute("fqdn") != null)
								{
									sipServerExternalAccess = xml04.GetAttribute("fqdn") + ":" + xml04.GetAttribute("port");
									logger.Debug("<< sipServerExternalAccess : {0}", sipServerExternalAccess);
								}
								break;
							case "SipClientExternalAccess":
								if (xml04.GetAttribute("fqdn") != null)
								{
									sipClientExternalAccess = xml04.GetAttribute("fqdn") + ":" + xml04.GetAttribute("port");
									logger.Debug("<< sipCientExternalAccess : {0}", sipClientExternalAccess);
								}
								break;
							case "Link":
								switch (xml04.GetAttribute("token"))
								{
									case "Internal/Ucwa":
										if (ucwaAccessLocation == "Internal")
										{
											ucwaApplications = xml04.GetAttribute("href");
											logger.Debug("<< ucwaApplications : {0}", ucwaApplications);
										}
										break;
									case "External/Ucwa":
										if (ucwaAccessLocation == "External")
										{
											ucwaApplications = xml04.GetAttribute("href");
											logger.Debug("<< ucwaApplications : {0}", ucwaApplications);
										}
										break;
									case "External/Autodiscover":
										if (ucwaAccessLocation == "External")
										{
											ucwaExternalAutodiscover = xml04.GetAttribute("href");
											logger.Debug("<< ucwaExternalAutodiscover : {0}", ucwaExternalAutodiscover);
										}
										break;
									case "Self":
										ucwaUrlSelf = xml04.GetAttribute("href");
										logger.Debug("<< ucwaUrlSelf : {0}", ucwaUrlSelf);
										break;

									default:
										break;
								}
								break;
							default:
								break;
						}
					Logon_Step05();
				}
				else
				{
					logger.Info(">> Error in step 04. {0}", "No OK received");
					logger.Info("Error. Logon ended abnormally.");
				}
			}
			catch (Exception ex)
			{
				logger.Info(">> Error in step 04. {0}", ex.InnerException.Message);
				logger.Info("Error. Logon ended abnormally.");
				logger.Error("Error. Logon ended abnormally.");
			}
		}

		public async void Logon_Step05()
		{
			if (ucwaUrlUser.StartsWith(ucwaUrlSelf))
			{
			}
			else
			{
				logger.Info("Environment contains a Director or Multiple Pools with user registered to different pool...");
				ucwaUrlOAuth = string.Format("{0}/?originalDomain={1}", ucwaUrlSelf, ucwaSipDomain);
				logger.Debug("<< ucwaUrlOAuth : {0}", ucwaUrlOAuth);
				httpClient.DefaultRequestHeaders.Remove("Authorization");
				ucwaUrlUser = ucwaUrlSelf;
				Logon_Step02();
				return;
			}
			try
			{
				httpClient.DefaultRequestHeaders.Remove("Accept");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

				var url_00 = ucwaApplications;

				logger.Info("Step 05 : POST : {0}", url_00);

				var guiId = Guid.NewGuid().ToString();
				HttpContent json_00 = new StringContent("{\"userAgent\":\"UCWA Francis Missiaen\",\"endpointId\":\""
				                                        + guiId + "\",\"culture\":\"en-US\"}", Encoding.UTF8, "application/json");

				logger.Debug(">> Request: {0}", "POST");
				logger.Debug(">> URL: {0}", url_00);
				logger.Debug("\r\n{0}", httpClient.DefaultRequestHeaders);
				logger.Debug(">> {0}={1}", "userAgent", "UCWA Francis Missiaen");
				logger.Debug(">> {0}={1}", "endpointId", guiId);
				logger.Debug(">> {0}={1}", "culture", "en-US");

				var res_00 = await httpClient.PostAsync(url_00, json_00);

				var res_00_request = res_00.RequestMessage.ToString();
				var res_00_headers = res_00.Headers.ToString();
				var res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(">> Response: {0}", res_00_status);
				logger.Debug("{0}", res_00_headers);
				logger.Debug("\r\n{0}", res_00_content);

				if (res_00_status == "Created")
				{
					var xml05 = new XmlTextReader(new StringReader(res_00_content));
					while (xml05.Read())
						switch (xml05.GetAttribute("rel"))
						{
							case "application":
								ucwaApplication = xml05.GetAttribute("href");
								logger.Debug("<< ucwaApplication : {0}", ucwaApplication);
								break;
							case "me":
								ucwaMe = xml05.GetAttribute("href");
								logger.Debug("<< ucwaMe : {0}", ucwaMe);
								break;
							case "events":
								ucwaEvents = xml05.GetAttribute("href");
								logger.Debug("<< ucwaEvents : {0}", ucwaEvents);
								break;
							case "makeMeAvailable":
								ucwaMakeMeAvailable = xml05.GetAttribute("href");
								logger.Debug("<< ucwaMakeMeAvailable : {0}", ucwaMakeMeAvailable);
								break;
							case "startMessaging":
								ucwaMessagingInvitations = xml05.GetAttribute("href");
								logger.Debug("<< ucwaMessagingInvitations : {0}", ucwaMessagingInvitations);
								break;
							case "startPhoneAudio":
								ucwaStartPhoneAudio = xml05.GetAttribute("href");
								logger.Debug("<< ucwaStartPhoneAudio : {0}", ucwaStartPhoneAudio);
								break;
							default:
								break;
						}
					Logon_Step06();
				}
				else
				{
					logger.Info(">> Error in step 05. {0} - {1}", "No OK received", res_00_status);
					logger.Info("Error. Logon ended abnormally.");
					logger.Error("Error. Logon ended abnormally.");
				}
			}
			catch (Exception ex)
			{
				logger.Info(">> Error in step 05. {0}", ex.InnerException.Message);
				logger.Info("Error. Logon ended abnormally.");
				logger.Error("Error. Logon ended abnormally.");
			}
		}

		public async void Logon_Step06()
		{
			try
			{
				var url_00 = ucwaApplications + ucwaMakeMeAvailable.Substring(27);
				logger.Info("Step 06 : POST : {0}", url_00);

				httpClient.DefaultRequestHeaders.Remove("Accept");
				HttpContent json_06 = new StringContent("{\"SupportedModalities\":[\"Messaging\"]}", Encoding.UTF8,
					"application/json");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

				logger.Debug(">> Request: {0}", "POST");
				logger.Debug(">> URL: {0}", url_00);
				logger.Debug("\r\n{0}", httpClient.DefaultRequestHeaders);
				logger.Debug(">> {0}={1}", "SupportedModalities", "Messaging");

				var res_00 = await httpClient.PostAsync(url_00, json_06);

				var res_00_request = res_00.RequestMessage.ToString();
				var res_00_headers = res_00.Headers.ToString();
				var res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(">> Response: {0}", res_00_status);
				logger.Debug("{0}", res_00_headers);
				logger.Debug("\r\n{0}", res_00_content);

				if (res_00_status == "NoContent")
				{
					Logon_Step07();
				}
				else
				{
					logger.Info(">> Error in step 06. {0}", "No OK received");
					logger.Info("Error. Logon ended abnormally.");
				}
			}
			catch (Exception ex)
			{
				logger.Info(">> Error in step 06. {0}", ex.InnerException.Message);
				logger.Info("Error. Logon ended abnormally.");
			}
		}

		public async void Logon_Step07()
		{
			try
			{
				httpClient.DefaultRequestHeaders.Remove("Accept");
				httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.microsoft.com.ucwa+xml");

				var url_00 = ucwaApplications + ucwaApplication.Substring(27);
				logger.Info("Step 07 : GET : {0}", url_00);

				logger.Debug(">> Request: {0}", "GET");
				logger.Debug(">> URL: {0}", url_00);
				logger.Debug("\r\n{0}", httpClient.DefaultRequestHeaders);

				var res_00 = await httpClient.GetAsync(url_00);

				var res_00_request = res_00.RequestMessage.ToString();
				var res_00_headers = res_00.Headers.ToString();
				var res_00_status = res_00.StatusCode.ToString();
				var res_00_content = await res_00.Content.ReadAsStringAsync();

				logger.Debug(">> Response: {0}", res_00_status);
				logger.Debug("{0}", res_00_headers);
				logger.Debug("\r\n{0}", res_00_content);

				if (res_00_status == "OK")
				{
					var XML = string.Format(res_00_content);
					//webBrowser_XML.Invoke(new Action(() =>
					//		webBrowser_XML.DocumentText = XML));
					var xml07 = new XmlTextReader(new StringReader(res_00_content));
					while (xml07.Read())
					{
						switch (xml07.GetAttribute("rel"))
						{
							case "note":
								ucwaNote = xml07.GetAttribute("href");
								logger.Debug("<< ucwaNote : {0}", ucwaNote);
								break;
							case "location":
								ucwaLocation = xml07.GetAttribute("href");
								logger.Debug("<< ucwaLocation : {0}", ucwaLocation);
								break;
							case "presence":
								ucwaPresence = xml07.GetAttribute("href");
								logger.Debug("<< ucwaPresence : {0}", ucwaPresence);
								break;
							case "reportMyActivity":
								ucwaReportMyActivity = xml07.GetAttribute("href");
								logger.Debug("<< ucwaReportMyActivity : {0}", ucwaReportMyActivity);
								break;
							default:
								break;
						}
						if (xml07.Name == "link")
							logger.Info($"XML07-href value {xml07.GetAttribute("href")}");
					}
					// <property name="1d6bd019-52ce-4ebb-9147-b43f5420d3a6">please pass this in a PUT request</property>  
					ucwaOperationID = "1d6bd019-52ce-4ebb-9147-b43f5420d3a6";
					var i = res_00_content.IndexOf("please pass this in a PUT request");
					if (i > 40)
					{
						ucwaOperationID = res_00_content.Substring(i - 38, 36);
						logger.Debug("<< ucwaOperationID : {0}", ucwaOperationID);
					}
					Logon_Step08();
				}
				else
				{
					logger.Info(">> Error in step 07. {0}", "No OK received");

					logger.Error("Error. Logon ended abnormally.");
				}
			}
			catch (Exception ex)
			{
				logger.Info(">> Error in step 07. {0}", ex.InnerException.Message);

				logger.Error("Error. Logon ended abnormally.");
			}
		}

		public void Logon_Step08()
		{
			logger.Info("Logon completed normally.");
		}
	}
}