# Postmark

## Postmark app setup
Setup account on https://account.postmarkapp.com/login
Add server
Add new Sender Signature email address

## Dev setup
add appsettings.Development.json file with the following content
```
{
	"EmailSender" :{
		"Postmark": {
			"ServerToken": "POSTMARK_SERVER_TOKEN_AS_GUID",
			"OriginEmail": "EMAIL_ADDRESS_CONFIGURED_AS_SENDER_SIGNATURE"
		}
	}
}
```

enjoy :)

# Smtp
on appsettings.Development.json insert the following
```
{
	"EmailSender" : {
		"SMTP": {
			"Address": "smtp_uri_address",
			"Username": "authentication username, optional"
			"Password": "authentication password, optional"
			"Port": "smtp port, optional, if not specified defaults to 25"
			"UseSSL": "optional, True or False"
		}
	}
}
```

