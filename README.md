# Cloudflare DDNS in Windows Service

This program will automatically update all your A records hosted in your provided account.

Your IP address is retrieved from http://checkip.amazonaws.com.
Your A records are updated at https://api.cloudflare.com/client/v4/.

## Getting started
1. Download from https://github.com/leoiii12/cloudflare-ddns-windows-service/releases
2. Retrieve your account api key from https://www.cloudflare.com/a/profile.
3. Modify ./Release/CloudflareDDNS.exe.config. Only api_key and email are needed.
4. Run ```CloudflareDDNS.exe``` as Administrator

## Uninstall
1. Run ```cmd.exe``` as Administrator
2. ```sc delete CloudflareDDNSService```

## Log
1. C:\ProgramData\CloudflareDDNS\log.txt
