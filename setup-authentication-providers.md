# Setup authentication providers #

## Prerequisites ##

Install [Secret Manager tool](https://docs.asp.net/en/latest/security/app-secrets.html#secret-manager) to be able to store credentials.
Otherwise, create a new file `./src/GovITHub.Auth.Identity/appsettings.development.json` and add a section named `Authentication`.

## Setup Facebook authentication ##

### Register Facebook app ###

- Navigate to [https://developers.facebook.com/apps](https://developers.facebook.com/apps) and add new Facebook app
- Open the `Settings` tab and click on `Add new platform`
- Select `Website` as the platform
- In `Site URL` box put `https://localhost:44301/`

### Add Facebook app credentials ###

#### With `Secret Manager tool` ####

- Open shell
- Navigate to `./src/GovITHub.Auth.Identity/`
- Execute the following commands:
```
dotnet user-secrets set Authentication:Facebook:AppId <app-Id>
dotnet user-secrets set Authentication:Facebook:AppSecret <app-secret>
```
#### In `appsettings.development.json` file ####

Add `Facebook` section to `Authentication` section; the file should look like this:
```json
{
  "Authentication": {
    "Facebook": {
      "AppId": "<app-id>",
      "AppSecret": "<app-secret>"
    }
  }
}
```

## Setup Google authentication ##

### Register Google app ###

- Log in to [https://console.developers.google.com](https://console.developers.google.com/) and setup a new app
- On credentials edit form set **Authorized redirect URIs** to `http://localhost:5000/signin-google`

### Add Google app credentials ###

#### With `Secret Manager tool` ####

- Open shell
- Navigate to `./src/GovITHub.Auth.Identity/`
- Execute the following commands:
```
dotnet user-secrets set Authentication:Google:GoogleClientId <client-id>
dotnet user-secrets set Authentication:Google:GoogleClientSecret <client-secret>
```

#### In `appsettings.development.json` file ####

Add `Google` section to `Authentication` section; the file should look like this:
```json
{
  "Authentication": {
    "Google": {
      "GoogleClientId": "<client-id>",
      "GoogleClientSecret": "<client-secret>"
    }
  }
}
```

## Setup LinkedIn authentication ##

### Register LinkedIn application ###

- Log in to [https://developer.linkedin.com/](https://developer.linkedin.com/)
- From the navigation bar select [My Apps](https://www.linkedin.com/developer/apps) and then click on `Create Application`
- Fill in the required info and in **Website URL** box type `http://localhost:5000`
- Press `Submit` and you will be redirected to the new application

### LinkedIn application setup ###

- In the **Authentication** tab
  - In **Default application permissions** section check `r_basicprofile` and `r_emailaddress`
  - In **OAuth 2.0** section add `http://localhost:5000/linkedin-auth` to the **Authorized Redirect URLs**

### Add LinkedIn app credentials ###

#### With `Secret Manager tool` ####

- Open shell
- Navigate to `./src/GovITHub.Auth.Identity/`
- Execute the following commands:
```
dotnet user-secrets set Authentication:LinkedIn:ClientId <client-id>
dotnet user-secrets set Authentication:LinkedIn:ClientSecret <client-secret>
```

#### In `appsettings.development.json` file ####

Add `LinkedIn` section to `Authentication` section; the file should look like this:
```json
{
  "Authentication": {
    "LinkedIn": {
      "ClientId": "<client-id>",
      "ClientSecret": "<client-secret>"
    }
  }
}
```

