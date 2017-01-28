# Citizen Single Sign On - GovIT Hub Authenticator [![Build Status](https://travis-ci.org/gov-ithub/auth-sso.svg?branch=master)](https://travis-ci.org/gov-ithub/auth-sso) 
Un singur set de credențiale pentru cetățean. La bază este un identity server ce expune un endpoint OAuth2.

## Instalare
### Update submodules
- branch from master
- open `Git Bash` and update submodules with the following commands:
```
cd /path/to/repository
git submodule update --init --recursive
```
### Database
- install mysql / mariaDB
- create database sso


### Docker
- install docker [Windows](https://docs.docker.com/docker-for-windows/)
- open shell (command prompt)
- go to `\\auth-sso\docker\redis`
- `docker build -t "auth-sso-redis" .`
- `docker run auth-sso-redis`

### Mailcatcher
Mailcatcher poate fi folosit ca si server SMTP de test https://mailcatcher.me/

### Identity server
- go to `\\auth-sso\src\GovItHub.Auth.Identity`
- copy file `connectionstrings.Sample.json` to `connectionstrings.json` and update all keys
- open shell (command prompt) and navigate to `\\auth-sso\src\GovITHub.Auth.Identity`
- `dotnet restore`
- `dotnet ef database update`
- `npm install`
- `bower install`
- `dotnet run`

### Admin panel
- go to `\\auth-sso\src\GovItHub.Auth.Admin`
- copy file `connectionstrings.Sample.json` to `connectionstrings.json` and update all keys
- open shell (command prompt) and navigate to `\\auth-sso\src\GovITHub.Auth.Admin`
- `dotnet restore`
- `dotnet ef database update`
- `npm install`
- `bower install`
- `dotnet run`

### Javascript client sample
- open shell (command prompt)
- go to \\govithub-auth-sso\src\samples\JavaScriptClient
- `npm install`
- `dotnet run`

## Tehnologii folosite
- [Identity Server](https://identityserver.io/)
- [.NET Core](https://www.microsoft.com/net/core)
- [AngularJS](https://angularjs.org/)
- [Docker](https://docs.docker.com/engine/installation/) [Windows](https://docs.docker.com/docker-for-windows/)
- [npm](https://github.com/npm/npm)
- [1][MySQL](http://www.mysql.com/)

```
[1] Posibil să se schimbe în viitorul apropiat
```

### Extra techs (folosite la SDKs, samples & integration)
- [React](https://facebook.github.io/react/)
- [PHP](http://www.php.net/)
- [Java SE] (http://www.oracle.com/technetwork/java/javase/overview/index.html)
- [Java EE](http://www.oracle.com/technetwork/java/javaee/overview/index.html)
- [Python] (https://www.python.org/)
- Others :-)

## Contribuie

Dacă vrei să contribui - ești binevenit(ă) - but we don't have cookies (yet) 

### Proces
- Vezi lista de tehnologii folosite - îți sunt familiare?
- Dacă nu ești încă înscris(ă) în comunitate, te rog fă-o pe [site-ul de voluntari GovITHub](http://voluntari.ithub.gov.ro/)
- Aruncă o privire la [guidelines](https://github.com/gov-ithub/guidelines/blob/master/CODE_REVIEW.md) pentru code review 
- Caută în issues un task interesant pentru tine (sau, dacă ai o propunere, deschide un issue / trimite un pull request). 
- După ce trecem prin code review - celebrate! :star: :star2: :star:

## Cum poti intra in contact cu echipa?
- Email: alexandru.chiraples@ithub.gov.ro
- [Slack](https://govithub.slack.com/messages/gov-authenticator/details/). Pentru invite intrați pe http://govitslack.herokuapp.com/

Mai multe detalii în curând! 
