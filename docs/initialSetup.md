In referintele de la sfarsit sunt cateva tutoriale pe care ar trebui sa le citeasca
 voluntarii nefamiliarizati cu tehnologiile folosite in acest proiect.

Tutorial pentru set-up-ul aplicatiei auth-sso local.

1. Instalati Microsoft Visual Studio Community

2. Instalati NET Core SDK si Tools de aici:
https://www.microsoft.com/net/download/core

3. Verficati folderele pe care le gasiti in 
C:\Program Files\dotnet\sdk
Si ca unul este versiunea 1.0.0-preview2-003131. E nevoie de versiunea aceasta 
pentru ca este configurata in fisierul global.json al solutiei

4. E nevoie si de MySQL deci daca aveti XAMPP deja instalat, MySQL e deja instalat cu el.
 Daca nu, instalati XAMPP sau alt bundle care contine MySQL

Pentru pasii de instalare de pe auth-sso de pe pagina proiectului din GitHub:

5. Instalati Git si Source Tree
Pentru cei care nu au mai lucrat cu Git:
a) sa le instalezi pe amandoua e ca si cum ai avea 2 Git-uri; de preferat ar fi sa lucrati cu Git in consola
si sa va uitati in repository-ul din Source Tree pentru a avea o idee vizuala despre branches si commit-uri
b) o data creat repository-ul intr-un folder local prin Source Tree/Git va insemna ca orice schimbare
facuta in folder-ul respectiv va aparea ca "uncommited changes" in Source Tree si "unstaged changes" pentru
comanda "git status" in consola.
Asadar orice fisier nou, fisier sters, fisier adaugat sau modificat il veti avea pana la stergerea repository-ului
in lista asta. Deci alegeti bine folder-ul si faceti doar schimbari legate de cod si de user story-ul pe care il lucrati
in interiorul acestui folder.
bb) recomandare de locatie root pentru repository-urile din  govithub: [drive letter]:\projects\gov-ithub

6. In consola, in directorul unde vreti sa descarcati solutia local de pe platforma GitHub dati comenzile:

git clone https://github.com/gov-ithub/auth-sso.git //  face o clona locala a codului de pe master
git checkout -b numeio-env //  face un branch unde putem pune codul odata terminat de modificat local

7. Daca branch-ul nu incepe de la cea mai recenta versiune executati urmatoarele comenzi in consola:

git checkout master - ca sa treceti din nou pe master

git pull - ca sa aduceti local ultima versiune

daca nu da nicio eroare,

git checkout numeio-env
 
git merge master - aduce modificarile din master in branch-ul curent

Set-up-ul solutiei este acum complet. Solutia insasi si toate proiectele din solutie
 ar trebui sa se poata deschide corect in VS Community.
Pentru a porni aplicatia, deci ca sa vedem ce poate pana in momentul asta, mai avem nevoie de urmatoarele:

8.Creati baza de date sso

a) Daca aveti intstalat XAMPP, porniti-l si porniti Serverul Appache si baza de date MySQL.
Pentru ca aplicatia si site-ul ei sa poata functiona, e nevoie ca acestea 2 sa fie pornite de fiecare data cand vreti sa testati aplicatia.
b) mergeti in browser pe localhost/phpMyAdmin si creati o noua baza de date cu numele "sso" fara nicio configurare suplimentara
c) Faceti click pe ea si mergeti apoi la tab-ul Privileges unde aveti lista de useri acreditati la aceasta baza de date
d) Creati un user now si parola ascoiata in 'local' deci pe localhost (ex: user= myUser, parssword = myPass) 
Oferiti-i acestui user drepturi doar pentru aceasta baza de date, nu global! 
Daca va avea drepturi globale, aplicatia nu se va putea conecta la baza de date.

9. Creati urmatorul fisier in solutia voastra sub urmatorul director, nume si tip de fisier 
./src/GotITHub.Auth.Identity/connectionstrings.json cu urmatorul continut:

{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;userid=<user id>;pwd=<password>;port=3306;database=sso;sslmode=none;"
  }
}

user id = myUser
password = myPass in cazul asta

Set-up-ul aplicatiei este acum aproape complet. Dupa cativa pasi vom putea intra pe browser
 la adresa localhost:5000 si sa vedem aplicatia cu ce poate face ea pana in prezent.

10. Instalati Node de la  https://nodejs.org/en/download/ . 
Preferati installer-ul pentru a putea folosi mai apoi comanda nmp din consola fara bataie de cap

11. Pornirea Identity server

Deschideti o consola
Navigati la  \govithub-auth-sso\src\GovITHub.Auth.Identity
Dati comenzile:
dotnet restore
dotnet ef database update
dotnet run

Daca totul merge OK, ultima linie va fi asta:
"Application started. Press Ctrl+C to shut down."

12. Pornirea Javascript client sample

Deschideti o consola (una noua)
Navigati la \govithub-auth-sso\src\samples\JavaScriptClient
Dati comenzile:
npm install
dotnet run

Daca totul merge OK, ultima linie va fi asta:
"Application started. Press Ctrl+C to shut down."

13. Deschideti un browser si intrati pe adresa localhost:5000 unde ar trebui sa se vada site-ul aplicatiei

Daca site-ul nu se incarca si in consola unde ati pornit Identity Server-ul apare urmatoarea exceptie:

An exception occurred in the database while iterating the results of a query.
MySql.Data.MySqlClient.MySqlException: Table 'sso.localizationrecord' doesn't exist

opriti aplicatia si clientul cu Ctrl+C si scrieti in consola aplicatiei:
dotnet ef database update --context LocalizationModelContext

Reporniti aplicatia si clientul cu "dotnet run" in fiecare consola, reincercati punctul 13.


Observatie: dupa ce acest set-up e facut, pentru a vedea schimbarile pe care le-ati facut pe site e suficient 
sa opriti aplicatia si clientul cu Ctrl+C si apoi sa reporniti aplicatia si clientul cu "dotnet run" (care face si compilarea sursei).

Referinte:

[1] https://github.com/gov-ithub/auth-sso.git  - pagina Git a proiectului
[2] https://www.microsoft.com/net/download/core  - NET Core SDK si Tooling
[3] http://juristr.com/blog/2013/04/git-explained/  - Git explained for beginners
[4] https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db - tutorial pentru entity framework (ef) in .net core 
[5] https://www.npmjs.com/ - npm is the package manager for javascript