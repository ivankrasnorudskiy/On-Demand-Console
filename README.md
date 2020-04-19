# On Demand Console
This product able to pass requests from On Demand to On Premise through AzureRelay retranslator

 1. Add AzureRelay parameters for relay listener and sender in Repository.ConsoleMVC (AppSettings.json) and Repository.Gateway (App.config)
 2. Run the ITSS.Repository.ConsoleMVC(Asp Net MVC Core app)
 3. Run the ITSS.Repository.Gateway (Console App), you can also run application as Windows-Service with powershell script (InstallWinService.ps1)
 4. Now you can send request with Query parameter to Console and try to search matches in events_data.json(as test). But you can set a personal repository with data or another search mechanism to ITSS.Repository.SearchModule and use it. I had been forced delete previous repository with a search mechanism because it belongs private company in which I worked.
 
 P.S 
 For usability you can use GatewayClient and pass Query as commandline parameter.
 Actually this application was used to search in real product, but I need to delete original SearchModule and put here mock service.
